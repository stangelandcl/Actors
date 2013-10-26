using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface IDht : IDisposable
    {
        T Get<T>(string key);
        void Add<T>(string key, T value);
        void Remove(string key);

		void Subscribe(DhtOperation operation, string keyRegex);
		void Unsubscribe(DhtOperation operation, string regex);
		event Action<DhtOperation, string> KeyMatch;      
    }
}
