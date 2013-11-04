using System;
using System.IO;

namespace Cls.Serialization
{
	public class StringSerializer : SpecificSerializer<string>
	{
		public StringSerializer(Serializers s)
		: base(s){}
		protected override void Serialize(System.IO.Stream stream, string item)
		{
			var w = new BinaryWriter(stream);
			w.Write(item);		
			w.Flush();
		}

		protected override string Deserialize(System.IO.Stream stream)
		{
			var r = new BinaryReader(stream);
			return r.ReadString();
		}
	}
}

