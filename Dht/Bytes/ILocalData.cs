using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    public interface ILocalData : IByteDht
    {
        KeyValuePair<string, byte[]>[] Data { get; }
        void AddRange(IEnumerable<KeyValuePair<string, byte[]>> items);      
    }
}
