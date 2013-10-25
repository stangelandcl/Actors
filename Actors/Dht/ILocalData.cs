using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Builtin.Clients;

namespace Actors.Dht
{
    public interface ILocalData : IDht
    {
        KeyValuePair<object, List<object>>[] Data { get; }
        void AddRange(IEnumerable<KeyValuePair<object, List<object>>> items);
    }
}
