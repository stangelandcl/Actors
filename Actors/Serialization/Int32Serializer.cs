using System;
using System.IO;

namespace Actors
{
	public class Int32Serializer : ISerializer<int>, ISerializer
	{
		#region ISerializer implementation

		public void Serialize (System.IO.Stream stream, int item)
		{
			var w = new BinaryWriter(stream);
			w.Write(item);
			w.Flush();
		}

		public int Deserialize (System.IO.Stream stream)
		{
			return new BinaryReader(stream).ReadInt32();
		}

		#endregion

		#region ISerializer implementation

		public void Serialize<T> (System.IO.Stream stream, T item)
		{
			Serialize(stream, (int)(object)item);
		}

		public T Deserialize<T> (System.IO.Stream stream)
		{
			return (T)(object)Deserialize(stream);
		}

		#endregion


	}
}

