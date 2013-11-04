using System;
using System.Collections.Generic;
using Fasterflect;
using Cls.Serialization;


namespace Cls.Actors
{
	public class Defaults
	{
		static readonly ServiceContainer container = new ServiceContainer();
		static Defaults(){
			container.Register<ISerializer, Serializer>(new PerContainerLifetime());
		}

		public static TcpNode Node(int port = 0){
			return TcpNode.Open(port);
		}

		public static ISerializer Serializer = new Serializer();
		public static int Port = 12584;
		public static int GetPort (int defaultPort)
		{
			return defaultPort == 0 ? Port : defaultPort;
		}

		public static T Get<T>(T s)
		{
			if(!typeof(T).IsValueType)
			{
				if(s != null) return s;
			}
			else if(!s.Equals(default(T))) return s;
			return container.GetInstance<T>();

		}
	}
}

