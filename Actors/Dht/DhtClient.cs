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
           // dht.KeyMatch += HandleKeyMatch;
        }

        IByteDht dht;
        ISerializer serializer;        

        public T Get<T>(string key)
        {
            return serializer.Deserialize<T>(dht.Get(key));
        }    

        public void Remove(string key)
        {
            dht.Remove(key);
        }

        public void Add<T>(string key, T value)
        {
            dht.Add(key, serializer.Serialize(value));
        }

        public void Subscribe(DhtOperation operations, string keyRegex)
        {
            dht.Subscribe(operations, keyRegex);
        }

		public void Unsubscribe(DhtOperation operations, string keyRegex)
        {
            dht.Unsubscribe(operations, keyRegex);
        }

		public event Action<DhtOperation, string> KeyMatch;

		void HandleKeyMatch(DhtOperation operation, string key)
        {
            KeyMatch.FireEventAsync(operation, key);
        }

        public void Dispose()
        {
          //  dht.KeyMatch -= KeyMatch;
            dht.Dispose();
        }
    }
}
