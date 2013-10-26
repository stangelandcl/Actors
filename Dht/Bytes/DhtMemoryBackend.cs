using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public class DhtMemoryBackend : ILocalData
    {
        Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
        public KeyValuePair<string, byte[]>[] Data
        {
            get
            {
                return data.ToArray();
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<string, byte[]>> items)
        {
            foreach (var kvp in items)
                data[kvp.Key] = kvp.Value;
        }

        public void Put(string key, byte[] value)
        {
            data[key] = value;

        }

        public byte[] Get(string key)
        {
            return data.GetOrDefault(key);
        }

        public void Delete(string key)
        {
            data.Remove(key);
        }

        public void Subscribe(string regex)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(string regex)
        {
            throw new NotImplementedException();
        }

        public event Action<string, string> KeyMatch;
    }
}
