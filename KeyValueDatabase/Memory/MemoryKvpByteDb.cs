using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyValueDatabase
{
    class MemoryKvpByteDb : IKvpByteDb
    {
        Dictionary<byte[], byte[]> map = new Dictionary<byte[], byte[]>(ByteArrayComparer.Default);
        public byte[] Get(byte[] key)
        {
            return map.GetOrDefault(key);
        }

        public void Add(byte[] key, byte[] value)
        {
            map[key] = value;
        }

        public void Remove(byte[] key)
        {
            map.Remove(key);
        }

        public void AddRange(IEnumerable<KeyValuePair<byte[], byte[]>> items)
        {
            foreach (var kvp in items) map[kvp.Key] = kvp.Value;
        }

        public IEnumerable<byte[]> Keys
        {
            get { return map.Keys; }
        }

        public IEnumerable<KeyValuePair<byte[], byte[]>> Items
        {
            get { return map; }
        }

        public IEnumerable<byte[]> Values
        {
            get { return map.Values; }
        }
    }
}
