using System;
using System.IO;

namespace Actors
{
	public class StringSerializer : ISerializer<string>, ISerializer
	{
		public void Serialize(System.IO.Stream stream, string item)
		{
			var w = new BinaryWriter(stream);
			w.Write(item);		
			w.Flush();
		}

		public string Deserialize(System.IO.Stream stream)
		{
			var r = new BinaryReader(stream);
			return r.ReadString();
		}

		#region ISerializer implementation

		public void Serialize<T> (Stream stream, T item)
		{
			Serialize(stream,(string)(object) item);
		}

		public T Deserialize<T> (Stream stream)
		{
			return (T)(object)Deserialize(stream);
		}

		#endregion
	}
}

