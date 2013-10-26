using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;

namespace Actors.Dht
{
    public class DhtClient : IDht
    {
        public DhtClient(IByteDht dht, ISerializer serializer)
        {
            this.dht = dht;
            this.serializer = serializer;
        }

        IByteDht dht;
        ISerializer serializer;

        public List<object> Get(string key)
        {
            return dht.Get(key).Select(n => serializer.Deserialize<object>(n)).ToList();
        }

        public void Replace(string key, object value)
        {
            dht.Replace(key, serializer.Serialize(value));
        }

        public void Remove(string key)
        {
            dht.Remove(key);
        }

    }
}
