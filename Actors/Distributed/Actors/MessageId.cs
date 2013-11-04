using System;
using System.ComponentModel;

namespace Cls.Actors
{
	//[TypeConverter(typeof(ObjectTypeConverter<MessageId>))]
	public struct MessageId : IMessageId
	{
		public MessageId(string id){
			this.id = id;
		}
		public MessageId(Guid id){
			this.id = id.ToString();
		}
		public static MessageId New(){
			return new MessageId(Guid.NewGuid());
		}
        public static readonly MessageId Empty = new MessageId();
        string id;
        public string Id { get { return id; } set { id = value; } }

		public bool IsEmpty{ get {return Id == null;}}

		public override string ToString ()
		{
			return Id;
		}

		public static implicit operator string(MessageId id){
			return id.ToString();
		}
		public static implicit operator MessageId(string s){
			return new MessageId(s);
		}
		public static implicit operator MessageId(Guid s){
			return new MessageId(s);
		}
		public static implicit operator MessageId(int o){
			return new MessageId(o.ToString());
		}

		#region IEquatable implementation

		public override bool Equals (object obj)
		{
			return Equals(obj as IMessageId);
		}

		public override int GetHashCode ()
		{
			return id.GetHashCode();
		}

		public bool Equals (IMessageId other)
		{
			if(other == null || other.GetType() != GetType())
				return false;
			return ((MessageId)other).id == id;
		}

		#endregion
	}
}

