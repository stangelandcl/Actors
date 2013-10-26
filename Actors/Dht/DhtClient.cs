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
            dht.KeyMatch += HandleKeyMatch;
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

        public void Subscribe(string operationRegex, string keyRegex)
        {
            dht.Subscribe(operationRegex, keyRegex);
        }

        public void Unsubscribe(string operationRegex, string keyRegex)
        {
            dht.Unsubscribe(operationRegex, keyRegex);
        }

        public event Action<string, string> KeyMatch;

        void HandleKeyMatch(string operation, string key)
        {
            KeyMatch.FireEventAsync(operation, key);
        }

        public void Dispose()
        {
            dht.KeyMatch -= KeyMatch;
            dht.Dispose();
        }
    }
}
