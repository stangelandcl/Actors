using System;
using System.IO;

namespace Actors
{
	public class Int32Serializer : SpecificSerializer<int>
	{
		public Int32Serializer(Serializers s)
		: base(s){}

		#region ISerializer implementation

		protected override void Serialize (System.IO.Stream stream, int item)
		{
			var w = new BinaryWriter(stream);
			w.Write(item);
			w.Flush();
		}

		protected override int Deserialize (System.IO.Stream stream)
		{
			return new BinaryReader(stream).ReadInt32();
		}

		#endregion


	}
}

