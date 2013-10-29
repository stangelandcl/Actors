using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Serialization
{
    class ByteArraySerializer 
    {
        public void Serialize(System.IO.Stream stream, byte[] item)
        {
            var w = new BinaryWriter(stream);
            w.Write(item.Length);
            w.Write(item, 0, item.Length);
            w.Flush();
        }

        public byte[] Deserialize<T>(System.IO.Stream stream)
        {
            var r = new BinaryReader(stream);
            int count = r.ReadInt32();
            return r.ReadBytes(count);
        }
    }
}
