using System;

using System.Threading;
using System.Collections.Generic;

using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Option;


namespace Actors
{
    /// <summary>
    /// If you override any functions in this class be careful to add a try-catch and call Die on failure    
    /// </summary>
	public abstract class DistributedActor : RpcActor, IDisposable
	{
        public DistributedActor(string shortName)
            : this(new ActorId(shortName), null)
        { }        

		public DistributedActor(ActorId id, Node node)
		{
			this.Node = node;
			this.Id = id;
            IsAlive = true;
			functions.Remove("SendTo");
		}

        int isDisposed;
		public ActorId Id {get; internal set;}
		public Node Node {get; internal set;}
        public bool IsAlive { get; private set; }   

		
		ConcurrentQueue<IRpcMail> messages = new ConcurrentQueue<IRpcMail>();

		void BoundQueueSize ()
		{
			while (messages.Count > this.maxMessages) {
				IRpcMail m;
				messages.TryDequeue (out m);
			}
		}

		protected override void HandleMessage (IRpcMail mail)
		{
			base.HandleMessage (mail);
			BoundQueueSize ();
			messages.Enqueue(mail);
		}

        public virtual void AttachNode(Node node)
        {
            Node = node;
            Id = new ActorId(System.Environment.MachineName, Node.Id ,Id.Name);
        }

        public static implicit operator ActorId(DistributedActor a){
            return a.Id;
        }	

        void Link(IMail mail, LinkStatus status, string msg)
        {
            switch (status)
            {
                case LinkStatus.Create: Node.Links.Add((ActorId)mail.As<RpcMail>().From, 
				                                       (ActorId)mail.As<RpcMail>().To);   
                    break;
                case LinkStatus.Died: Die("Linked: " + mail.As<RpcMail>().From + " Died");
                    break;
                case LinkStatus.Disconnected:
                    break;
                case LinkStatus.Heartbeat:
                    break;
                default:
                    break;
            }
        }
		
       
        /// <summary>
        /// Run async. IMPORTANT: Use this instead of Task, Thread or ThreadPool. 
        /// It will catch exceptions, kill the actor and remove it from the node which will
        /// send the died message to any linked actors.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayInMilliseconds"></param>
        protected void Run(Action action, int delayInMilliseconds = 0)
        {
            try
            {
                if (!IsAlive) return;
                if (delayInMilliseconds > 0)
                    TaskEx.Delay(delayInMilliseconds).ContinueWith(task => action());
                else 
                    Task.Factory.StartNew(action);
            }
            catch (Exception ex)
            {
                Die(ex.ToString());
            } 
        }

        protected void Die(string message) 
        {
            try
            {
                if (Interlocked.CompareExchange(ref isDisposed, 1, 0) != 0)
                    return;              
                IsAlive = false;                
                // order is important in this function
                try { Disposing(true); }
                catch { }
                               
                if (Node != null)
                    Node.Remove(this, message);
                //IsAlive = false;
                Node = null;               
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Actor.Dispose failed " + ex);
            }
        }

        protected virtual void Disposing(bool b) { }
       
		public void Dispose()
        {
            Die("Dispose called");          
		}
		

		public Task<Option<IRpcMail>> Receive (TimeSpan? timeout = null)
		{
			timeout = timeout ?? TimeSpan.FromSeconds (5);
            return TaskEx.Loop<IRpcMail>(() =>
            {
                IRpcMail mail;
                if(messages.TryDequeue(out mail))
                    return Option<IRpcMail>.Some(mail);
                return Option<IRpcMail>.None;
            }, timeout.Value);
			
		}

		public Task<Option<T>> Receive<T>(TimeSpan? timeout = null)
		{
            return Receive(timeout).ContinueWith(t =>
            {
                return t.Result.HasValue ?
                    (Option<T>)Option<T>.Some(t.Result.Value.Message.Args[0].Convert<T>()) :
                    (Option<T>)Option<T>.None;
            });			
		}

		public IRpcMail Receive (IMessageId msg,  TimeSpan? timeout = null)
		{
			timeout = timeout ?? TimeSpan.FromSeconds (5);
			IRpcMail mail;
			var sw = Stopwatch.StartNew ();
			while (!messages.TryDequeue(out mail) || !mail.MessageId.Equals(msg))
			{					
				if (sw.Elapsed > timeout)
					break;
				Thread.Yield ();
			}
			return mail;
		}

		public T Receive<T>(IMessageId msg, TimeSpan? timeout = null)
		{
			var mail = Receive (msg, timeout);
			if (mail == null) return default(T);
			return ConvertEx.Convert<T>(mail.Message.Args[0]);
		}

		public IMessageId SendTo(IActorId to, string name, params object[] args)
		{
			MessageId msg;
			Node.Send(new RpcMail { To = to, From = Id, MessageId = msg = MessageId.New(), Message = new FunctionCall(name, args) });
			return msg;
		}
	}
}

