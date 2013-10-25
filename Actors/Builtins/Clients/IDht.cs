using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Builtin.Clients
{
    public interface IDht
    {
        object this[object key] { get; set; }
        void Remove(object key);
        void Append(object key, object value);
        void Remove(object key, object value);
    }
}
