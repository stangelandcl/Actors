using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Actors.Network
{
    [TypeConverter(typeof(ObjectTypeConverter<NodeId>))]
    public partial struct NodeId
    {
        public NodeId(string id)
        {
            this.id = id;
        }
        public NodeId(Guid id)
        {
            this.id = id.ToString();
        }

        public bool IsEmpty { get { return id == null; } }
        public static readonly NodeId Empty = new NodeId();

        string id;

        public override string ToString()
        {
            return id;
        }
        public bool Equals(NodeId n)
        {
            return n.id == id;
        }

        public static implicit operator string(NodeId id)
        {
            return id.ToString();
        }
        public static implicit operator NodeId(string s)
        {
            return new NodeId(s);
        }
        public static implicit operator NodeId(Guid id)
        {
            return new NodeId(id);
        }
        public static implicit operator Guid(NodeId s)
        {
            return Guid.Parse(s.id);
        }
    }
}
