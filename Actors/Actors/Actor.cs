using System;
using Serialization;
using System.Threading;
using System.Collections.Generic;
using Actors.Functions;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Actors.Tasks;
using System.Diagnostics;

namespace Actors
{
    /// <summary>
    /// If you override any functions in this class be careful to add a try-catch and call Die on failure    
    /// </summary>
	public abstract class Actor : IDisposable
	{
        public Actor(string shortName)
            : this(new MailBox(shortName), null)
        { }        

		public Actor(MailBox box, Node node)
		{
			Node = node;
			Box = box;
			Box.Received += HandleReceived;

            functions = GetTypes(GetType()).SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(n => n.GetParameters().Length > 0 && n.GetParameters()[0].ParameterType == typeof(Mail))
                .Where(n => n.Name != "Execute"))
                .ToDictionary(n => n.Name);
            if (!functions.ContainsKey("Link")) throw new Exception("No link");
            IsAlive = true;
		}

        IEnumerable<Type> GetTypes(Type t)
        {
            yield return t;
            if (typeof(Actor).IsAssignableFrom(t.BaseType))
                foreach (var x in GetTypes(t.BaseType))
                    yield return x;                    
        }

        int isDisposed;
		public MailBox Box {get; internal set;}
		public Node Node {get; internal set;}
        public bool IsAlive { get; private set; }
        Dictionary<string, MethodInfo> functions;
        public static implicit operator ActorId(Actor a){
            return a.Box.Id;
        }

        protected virtual void HandleReceived()
        {
            try
            {
                Box.Execute(Execute);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Actor.HandleReceived error " + ex);
                Die(ex.ToString());
            }
        }

        void Link(Mail mail, LinkStatus status, string msg)
        {
            switch (status)
            {
                case LinkStatus.Create: Node.Links.Add(mail.From, mail.To);   
                    break;
                case LinkStatus.Died: Die("Linked: " + mail.From + " Died");
                    break;
                case LinkStatus.Disconnected:
                    break;
                case LinkStatus.Heartbeat:
                    break;
                default:
                    break;
            }
        }

        private bool Execute(Mail mail)
        {
            try
            {
                MethodInfo func;
                if (!functions.TryGetValue(mail.Name, out func) ||
                    mail.Args.Length != func.GetParameters().Length - 1)
                    return false;
                var p = func.GetParameters();
                var args = new object[p.Length];
                args[0] = mail;
                for (int i = 0; i < mail.Args.Length; i++)
                    args[i + 1] = ConvertEx.Convert(mail.Args[i], p[i + 1].ParameterType);
                Run(() => func.Invoke(this, args));
            }
            catch (Exception ex)
            {
                Die(ex.ToString());
            }
            return true;
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
                
                if (Box != null)
                    Box.Received -= HandleReceived;
                if (Node != null)
                    Node.Remove(this, message);
                //IsAlive = false;
                Node = null;
                Box = null;
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
	}
}

