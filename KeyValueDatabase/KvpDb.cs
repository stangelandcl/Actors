using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace KeyValueDatabase
{
    public class KvpDb<TKey, TValue> : IKvpDb<TKey, TValue>
    {
        public KvpDb(IKvpByteDb db, ISerializer serializer)
        {
            this.Database = db;
            this.Serializer = serializer;
        }

        public TValue Get(TKey key)
        {
            var bytes = Database.Get(Serializer.Serialize(key));
            if (bytes == null) return default(TValue);
            return Serializer.Deserialize<TValue>(bytes);
        }

        public void Add(TKey key, TValue value)
        {
            Database.Add(Serializer.Serialize(key), Serializer.Serialize(value));
        }

        public void Remove(TKey key)
        {
            Database.Remove(Serializer.Serialize(key));
        }


        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            Database.AddRange(items.Select(n => KeyValuePair.New(
                Serializer.Serialize(n.Key),
                Serializer.Serialize(n.Value))));                      
        }

        public IEnumerable<TKey> Keys
        {
            get { return Database.Keys.Select(n => Serializer.Deserialize<TKey>(n)); }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Items
        {
            get
            {
                return Database.Items.Select(n =>
                    new KeyValuePair<TKey, TValue>(
                  Serializer.Deserialize<TKey>(n.Key),
                  Serializer.Deserialize<TValue>(n.Value)));
            }
        }

        public IEnumerable<TValue> Values
        {
            get { return Database.Values.Select(n => Serializer.Deserialize<TValue>(n)); }
        }

        public ISerializer Serializer { get; private set; }
        public IKvpByteDb Database { get; private set; }
    }
}
