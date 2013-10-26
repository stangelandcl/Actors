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

        void Subscribe(string operationRegex, string keyRegex);
        void Unsubscribe(string operationRegex, string keyRegex);
        event Action<string, string> KeyMatch;    
    }

}
