using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;

namespace Dht
{
    public struct DhtId : IComparable<DhtId>
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

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            return ByteArrayComparer.Default.Equals(Bytes, ((DhtId)obj).Bytes);
        }

        public static implicit operator ActorId(DhtId id)
        {
            return new ActorId(Encoding.UTF8.GetString(id.Bytes));
        }

        public static implicit operator DhtId(ActorId id)
        {
            return new DhtId(Encoding.UTF8.GetBytes(id.ToString()));
        }

        public static implicit operator DhtId(string id)
        {
            return new DhtId(id);
        }
    }
}
