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

		/// <summary>
		/// example actor://node.localhost/System.Echo
		/// </summary>
		/// <returns>The URL.</returns>
		/// <param name="url">URL.</param>
		public static ActorId FromUrl(string url){
			if(string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");			
			if(url.StartsWith("actor://")) url = url.Substring("actor://".Length);
			var s = url.Split('/');
			if(s.Length == 1) return new ActorId(s[0]);
			s[0] = s[0].ToLower();
			var parts = s[0].Split('.');
			if(parts.Length == 1) return new ActorId(parts[0], s[1]);
			return new ActorId(parts[1],new NodeId(parts[0]), s[1]);
		}

		public static implicit operator ActorId(string url){
			return FromUrl(url);
		}

        public bool Equals(ActorId id)
        {
            return 
                ((id.machine == null || machine == null) || (id.machine.Equals(machine, StringComparison.OrdinalIgnoreCase))) &&
                ((id.node.IsEmpty || node.IsEmpty) || (id.node.Equals(node))) &&
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

