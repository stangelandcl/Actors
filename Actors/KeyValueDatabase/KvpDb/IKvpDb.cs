using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Serialization;


namespace Cls.Actors
{
    public interface IKvpDb<TKey,TValue>
    {        
        TValue Get(TKey key);
        void Add(TKey key, TValue value);
        void Remove(TKey key);

        void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items);

        IEnumerable<TKey> Keys { get; }
        IEnumerable<KeyValuePair<TKey, TValue>> Items { get; }
        IEnumerable<TValue> Values { get; }

        ISerializer Serializer { get; }
        IKvpByteDb Database { get; }
    }
}
