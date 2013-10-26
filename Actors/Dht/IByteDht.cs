using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface IByteDht : IDisposable
    {
        byte[] Get(string key);
        bool Add(string key, byte[] value);
        bool Remove(string key);

		void Subscribe(DhtOperation operation, string keyRegex);
		void Unsubscribe(DhtOperation operation, string keyRegex);
		//event Action<DhtOperation, string> KeyMatch;    
    }

}
