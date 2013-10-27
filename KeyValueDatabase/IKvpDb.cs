using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace KeyValueDatabase
{
    public interface IKvpDb
    {
        T Get<T>(object key);
        object Get(object key);
        void Add(object key, object value);
        void Remove(object key);

        void AddRange(IEnumerable<KeyValuePair<object, object>> items);

        IEnumerable<object> Keys { get; }
        IEnumerable<KeyValuePair<object, object>> Items { get; }
        IEnumerable<object> Values { get; }

        ISerializer Serializer { get; }
        IKvpByteDb Database { get; }
    }
}
