using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Connections.Bytes
{
    public class EndPoint : IEndPoint
    {
        public EndPoint(string name)
        {
            this.name = name;
        }

        string name;
        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var ep = obj as EndPoint;
            if (ep != null)
                return string.Equals(name, ep.name, StringComparison.InvariantCultureIgnoreCase);
            return false;
        }
    }
}
