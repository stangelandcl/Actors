using System;


namespace Actors
{
	public class EchoActor : DistributedActor
	{
        public EchoActor(string shortName = "System.Echo")
            : base(shortName)
        { }		

        void Echo(IMail m, string msg)
        {
            Node.Reply(m, "Server says: " + msg);
        }
	}
}

