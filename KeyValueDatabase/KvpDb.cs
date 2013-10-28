using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace KeyValueDatabase
{
    public class KvpDb : IKvpDb
    {
        public KvpDb(IKvpByteDb db, ISerializer serializer)
        {
            this.Database = db;
            this.Serializer = serializer;
        }
        public object Get(object key)
        {
            return Get<object>(key);
        }

        public T Get<T>(object key)
        {
            var bytes = Database.Get(Serializer.Serialize(key));
            if (bytes == null) return default(T);
            return Serializer.Deserialize<T>(bytes);
        }

        public void Add(object key, object value)
        {
            Database.Add(Serializer.Serialize(key), Serializer.Serialize(value));
        }

        public void Remove(object key)
        {
            Database.Remove(Serializer.Serialize(key));
        }


        public void AddRange(IEnumerable<KeyValuePair<object, object>> items)
        {
            Database.AddRange(items.Select(n => KeyValuePair.New(
                Serializer.Serialize(n.Key),
                Serializer.Serialize(n.Value))));                      
        }

        public IEnumerable<object> Keys
        {
            get { return Database.Keys.Select(n => Serializer.Deserialize(n)); }
        }

        public IEnumerable<KeyValuePair<object, object>> Items
        {
            get
            {
                return Database.Items.Select(n =>
                    new KeyValuePair<object, object>(
                  Serializer.Deserialize(n.Key),
                  Serializer.Deserialize(n.Value)));
            }
        }

        public IEnumerable<object> Values
        {
            get { return Database.Values.Select(n => Serializer.Deserialize(n)); }
        }

        public ISerializer Serializer { get; private set; }
        public IKvpByteDb Database { get; private set; }
    }
}
