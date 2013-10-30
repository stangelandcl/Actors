using System;
namespace Actors
{
    partial struct ActorId : IEquatable<ActorId>
    {
        public override bool Equals(object other)
        {		
			if(other == null || other.GetType() != GetType())
				return false;
            return Equals((ActorId)other);
        }	

		/*public bool Equals(ActorId other){		
			return Equals((object)other); }*/		

         public static bool operator==(ActorId left, ActorId right)
		 {
            if (ReferenceEquals(left, right))
			     return true;
			else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
			      return false;
            else
                return left.Equals(right);
        }

        public static bool operator!=(ActorId left, ActorId right)
        {
            if (ReferenceEquals(left, right))
                return false;
            else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return true;
            else
                return !left.Equals(right);
        }
    }
}

namespace Actors.Network
{
    partial struct NodeId : IEquatable<NodeId>
    {
        public override bool Equals(object other)
        {		
			if(other == null || other.GetType() != GetType())
				return false;
            return Equals((NodeId)other);
        }	

		/*public bool Equals(NodeId other){		
			return Equals((object)other); }*/		

         public static bool operator==(NodeId left, NodeId right)
		 {
            if (ReferenceEquals(left, right))
			     return true;
			else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
			      return false;
            else
                return left.Equals(right);
        }

        public static bool operator!=(NodeId left, NodeId right)
        {
            if (ReferenceEquals(left, right))
                return false;
            else if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return true;
            else
                return !left.Equals(right);
        }
    }
}



