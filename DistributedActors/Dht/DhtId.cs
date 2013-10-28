using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;

namespace Dht
{
    public partial struct DhtId : IComparable<DhtId>
    {
        public DhtId(byte[] b)
        {
            this.Bytes = b;
        }
        public DhtId(string b)
            : this(Encoding.UTF8.GetBytes(b))
        { }
        public byte[] Bytes;

        public int CompareTo(DhtId other)
        {
            return ByteArrayComparer.Default.Compare(Bytes, other.Bytes);
        }

        public override int GetHashCode()
        {
            return ByteArrayComparer.Default.GetHashCode(Bytes);
        }

        public bool Equals(DhtId id)
        {            
            return ByteArrayComparer.Default.Equals(Bytes, id.Bytes);
        }
    }
}
