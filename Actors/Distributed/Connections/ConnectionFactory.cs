using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Connections;
using Cls.Extensions;

namespace Cls.Actors
{
    class ConnectionFactory
    {
        public static IDisposable Connect(Node node, Func<IConnection> create,bool isOutBound)
        {
            var c = new Conn();
            var dispose = Disposable.New(() =>
            {
                if (c.Connection != null)
                    c.Connection.Dispose();
                c = null;
            });
            ConnectInternal(c, node, create, isOutBound);
            return dispose;
        }

        class Conn
        {
            public IDisposable Connection;
        }

        static void ConnectInternal(Conn c, Node node, Func<IConnection> create, bool isOutBound)
        {
            try
            {
                if (c == null) return; // User disposed connection so stop trying.
                c.Connection = node.Connect(create(), isOutBound);
            }
            catch
            {
                TaskEx.Delay(1000 * 60 * 5).ContinueWith(n => ConnectInternal(c, node, create, isOutBound));
            }
        }
    }
}
