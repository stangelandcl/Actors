using System;
using System.ComponentModel;
using Actors.Network;

namespace Actors
{   
	//[TypeConverter(typeof(ObjectTypeConverter<ActorId>))]
	public partial struct ActorId : IActorId
	{
		public ActorId(string computer, NodeId node, string name){
            this.machine = computer;
            this.node = node;
            this.name = name;
		}
        public ActorId(string computer, string name)
        {
            this.machine = computer;
            this.node = NodeId.Empty;
            this.name = name;
        }
		public ActorId(NodeId node, string name){
            this.machine = null;
            this.node = node;
            this.name = name;
		}
        public ActorId(string name)
        {
            this.machine = null;
            this.node = NodeId.Empty;
            this.name = name;
        }
        string machine, name;
        NodeId node;

        public bool IsLocal { get { return Machine == null && Node.IsEmpty; } }

        public string Machine { get { return machine; } set { machine = value; } }
        public NodeId Node { get { return node; } set { node = value; } }
        public string Name { get { return name; } set { name = value; } }
        
        public static readonly ActorId Empty = new ActorId();
        public bool IsEmpty { get { 
            return Machine == null &&
            Node.IsEmpty && Name == null; } }	

		public override string ToString (){
            return Machine + "/" + Node + "/" + Name;
		}

        public bool Equals(ActorId id)
        {
            return 
                id.machine == machine &&
                id.node == node &&
                id.name == name;
        }

        public bool Equals(IActorId other)
        {
            if (other == null || other.GetType() != GetType())
                return false;
            return Equals((ActorId)other);
        }
    }
}

