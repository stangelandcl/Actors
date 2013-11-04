using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cls.Connections
{
    public interface IConnection : IDisposable
    {       
		IEndPoint Remote {get;}
        event Action<IConnection> Disconnected;
        void Send(object o);
        MessageQueue<object> Received { get; }      
		bool IsAlive {get;}
    }
}
