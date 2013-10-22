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

            functions = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(n => n.GetParameters().Length > 0 && n.GetParameters()[0].ParameterType == typeof(Mail))
                .Where(n => n.Name != "Execute")
                .ToDictionary(n => n.Name);
            IsAlive = true;
		}

		public MailBox Box {get; internal set;}
		public Node Node {get; internal set;}
        public bool IsAlive { get; private set; }
        Dictionary<string, MethodInfo> functions;

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
                    args[i + 1] = ConvertEx.ChangeType(mail.Args[i], p[i + 1].ParameterType);
                Run(() => func.Invoke(this, args));
            }
            catch (Exception ex)
            {
                Die(ex.ToString());
            }
            return true;
        }
       
        /// <summary>
        /// Run async
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ms"></param>
        protected void Run(Action action, int ms = 0)
        {
            try
            {
                if (ms > 0) TaskEx.Delay(ms).ContinueWith(task => action());
                else Task.Factory.StartNew(action);
            }
            catch (Exception ex)
            {
                Die(ex.ToString());
            } 
        }

        protected virtual void Die(string message) 
        {           
            Dispose();
        }
       
		public virtual void Dispose()
        {
            try
            {
                IsAlive = false;
                if(Box != null)
                    Box.Received -= HandleReceived;
                if(Node != null)
                    Node.Remove(this);
                Node = null;
                Box = null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Actor.Dispose failed " + ex);
            }
		}
	}
}

