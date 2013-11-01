using System;
using System.Collections.Generic;
using Fasterflect;


namespace Actors
{
	public class Defaults
	{
		static readonly ServiceContainer container = new ServiceContainer();
		static Defaults(){
			container.Register<ISerializer, JsonSerializer>(new PerContainerLifetime());
		}

		public static TcpNode Node(int port = 0){
			return TcpNode.Open(port);
		}

		public static int Port = 12584;
		public static int GetPort (int defaultPort)
		{
			return defaultPort == 0 ? Port : defaultPort;
		}

		public static T Get<T>(T s)
		{
			if(typeof(T).IsClass)
			{
				if(s != null) return s;
			}
			else if(!s.Equals(default(T))) return s;
			return container.GetInstance<T>();

		}
	}
}

