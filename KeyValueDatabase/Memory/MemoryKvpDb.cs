using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace KeyValueDatabase
{
    public class MemoryKvpDb : KvpDb
    {
        public MemoryKvpDb(ISerializer serializer)
            : base(new MemoryKvpByteDb(), serializer)
        { }
        public MemoryKvpDb()
            : this(new JsonSerializer())
        { }
    }
}
