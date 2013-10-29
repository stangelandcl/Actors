using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface IByteDht : IDisposable
    {
        byte[] Get(string key);
        void Add(string key, byte[] value);
        void Remove(string key);

        void Join(ActorId[] other);

		void Subscribe(DhtOperation operation, string keyRegex);
		void Unsubscribe(DhtOperation operation, string keyRegex);
		//event Action<DhtOperation, string> KeyMatch;    
    }

}
