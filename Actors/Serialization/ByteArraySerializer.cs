using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Serialization
{
    class ByteArraySerializer : ISerializer<byte[]>, ISerializer
    {
        public void Serialize(System.IO.Stream stream, byte[] item)
        {
            var w = new BinaryWriter(stream);
            w.Write(item.Length);
            w.Write(item, 0, item.Length);
            w.Flush();
        }

        public byte[] Deserialize(System.IO.Stream stream)
        {
            var r = new BinaryReader(stream);
            int count = r.ReadInt32();
            return r.ReadBytes(count);
        }

		#region ISerializer implementation

		public void Serialize<T> (Stream stream, T item)
		{
			Serialize(stream, (byte[])(object)item);
		}

		public T Deserialize<T> (Stream stream)
		{
			return (T)(object)Deserialize(stream);
		}

		#endregion
    }
}
