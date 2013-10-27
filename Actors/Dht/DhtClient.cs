using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serialization;
using System.Threading.Tasks;

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

        public void Join(ActorId[] other)
        {            
            dht.Join(other);        
        }

        public Task<T> Get<T>(string key)
        {
            return Task.Factory.StartNew(() =>
                serializer.Deserialize<T>(dht.Get(key)));
        }    

        public Task<IDht> Remove(string key)
        {
            return Task.Factory.StartNew<IDht>(() =>
            {
                dht.Remove(key);
                return this;
            });
        }

        public Task<IDht> Add<T>(string key, T value)
        {
            return Task.Factory.StartNew<IDht>(() =>
            {
                dht.Add(key, serializer.Serialize(value));
                return this;
            });
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
