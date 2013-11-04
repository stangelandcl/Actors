using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Serialization;


namespace Cls.Actors
{
    public class MemoryKvpDb<TKey, TValue> : KvpDb<TKey,TValue>
    {
        public MemoryKvpDb(ISerializer serializer)
            : base(new MemoryKvpByteDb(), serializer)
        { }
        public MemoryKvpDb()
            : this(Defaults.Serializer)
        { }
    }
}
