using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface IByteDht
    {
        byte[] Get(string key);
        void Put(string key, byte[] value);
        void Delete(string key);

        void Subscribe(string regex);
        void Unsubscribe(string regex);
        event Action<string, string> KeyMatch;    
    }

}
