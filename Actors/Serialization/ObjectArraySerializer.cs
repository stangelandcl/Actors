using System;
using System.IO;

namespace Actors
{
	public class ObjectArraySerializer : SpecificSerializer<object[]>
	{
		public ObjectArraySerializer(Serializers s)
		: base(s){}

		protected override void Serialize(Stream stream, object[] args){
			serializer.Get(typeof(int)).Serialize(args.Length);
			for(int i=0;i<args.Length;i++)
				serializer.Get(typeof(object)).Serialize(args[0]);
		}

		protected override object[] Deserialize(Stream stream){
			var count=  serializer.Get(typeof(int)).Deserialize<int>(stream);
			var args = new object[count];
			for(int i=0;i<args.Length;i++)
				args[i] = serializer.Get(typeof(object)).Deserialize<object>(stream);
			return args;
		}


	}
}

