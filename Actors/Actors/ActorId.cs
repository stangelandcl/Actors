using System;
using System.ComponentModel;

namespace Actors
{
	[TypeConverter(typeof(ObjectTypeConverter<ActorId>))]
	public struct ActorId
	{
		public ActorId(string id){
			this.id = id;
		}
		public ActorId(string computer, string world, string id){
			this.id = computer + "/" + world + "/" + id;
		}
		public ActorId(string computer, string world, Guid id)
			: this(computer, world, id.ToString())
		{}
		public ActorId(string world, string id)
			: this(Environment.MachineName, world, id)
		{}	
		public ActorId(string world, Guid id)
			: this(world,id.ToString())
		{}		

		public bool IsEmpty{get{return id == null;}}
		public static readonly ActorId Empty = new ActorId();

		string id;

		public override string ToString (){
			return id;
		}

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(ActorId))
                return string.Equals(((ActorId)obj).id, id, StringComparison.OrdinalIgnoreCase);
            return string.Equals(id, obj as string, StringComparison.OrdinalIgnoreCase);
        }
        public override int GetHashCode()
        {
            return id.ToLower().GetHashCode();
        }

		public static implicit operator string(ActorId id){
            return id.ToString();
		}
		public static implicit operator ActorId(string s){		
			return new ActorId(s);
		}
	}
}

