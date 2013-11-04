using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cls.Actors
{
    public interface IKvpByteDb
    {
        byte[] Get(byte[] key);
        void Add(byte[] key, byte[] value);
        void Remove(byte[] key);

        void AddRange(IEnumerable<KeyValuePair<byte[], byte[]>> items);

        IEnumerable<byte[]> Keys { get; }
        IEnumerable<KeyValuePair<byte[], byte[]>> Items { get; }
        IEnumerable<byte[]> Values { get; }        
    }
}
