using System;
using System.ComponentModel;

namespace Actors
{
	[TypeConverter(typeof(ObjectTypeConverter<MessageId>))]
	public struct MessageId
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

		public bool IsEmpty{ get {return id == null;}}

		public override string ToString ()
		{
			return id;
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
	}
}

