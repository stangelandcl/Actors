using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dht;
using System.Text.RegularExpressions;

namespace Actors.Dht
{
    class Subscription 
    {
        public Subscription(DhtId id, string op, string key)
        {
            Node = id;
            OperationRegex = op;
            KeyRegex = key;
        }
        public string OperationRegex { get; set; }
        public string KeyRegex { get; set; }
        public DhtId Node { get; set; }
        public bool IsMatch(string op, string key)
        {
            return op.IsMatch(OperationRegex) && key.IsMatch(KeyRegex);
        }

        public override int GetHashCode()
        {
            return Node.GetHashCode() ^ KeyRegex.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var o = (Subscription)obj;
            return o.Node.Equals(Node) && 
                OperationRegex == o.OperationRegex &&
                KeyRegex == o.KeyRegex;
        }
    }
}
