using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    class DhtMemoryBackend : ILocalData
    {
        Dictionary<object, List<object>> data = new Dictionary<object, List<object>>();
        public KeyValuePair<object, List<object>>[] Data
        {
            get
            {
                return data.ToArray();
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<object, List<object>>> items)
        {
            foreach (var kvp in items)
                data[kvp] = new List<object>(Enumerable.Repeat(kvp.Value, 1));
        }

        public void Replace(object key, object value)
        {
            data[key] = new List<object>(Enumerable.Repeat(value, 1));            
        }

        public List<object> Get(object key)
        {
            return data.GetOrDefault(key);
        }

        public void Remove(object key)
        {
            data.Remove(key);
        }

        public void Append(object key, object value)
        {
            if (!data.ContainsKey(key))
                data[key].Add(value);
            else
                data[key].Add(value);
        }

        public void Remove(object key, object value)
        {
            if (data.ContainsKey(key))
            {
                var list = data[key];
                list.Remove(value);
                if (!list.Any())
                    data.Remove(key);
            }
        }
    }
}
