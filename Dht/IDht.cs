using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface IDht
    {
        object Get(string key);
        void Put(string key, object value);
        void Delete(string key);

        void Subscribe(string regex);
        void Unsubscribe(string regex);
        event Action<string, string> KeyMatch;      
    }
}
