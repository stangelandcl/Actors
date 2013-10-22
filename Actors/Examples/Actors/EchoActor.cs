using System;
using Serialization;

namespace Actors
{
	public class EchoActor : Actor
	{
        public EchoActor(string shortName = "System.Echo")
            : base(shortName)
        { }		

        void Echo(Mail m, string msg)
        {
            Node.Reply(m, "Server says: " + msg);
        }
	}
}

