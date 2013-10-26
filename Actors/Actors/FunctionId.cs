using System;
using System.ComponentModel;

namespace Actors
{
	//[TypeConverter(typeof(ObjectTypeConverter<FunctionId>))]
	public struct FunctionId
	{
		public FunctionId(string id){
			this.id = id;
		}
		public FunctionId(Guid id){
			this.id = id.ToString();
		}
		public static FunctionId New(){
			return new FunctionId(Guid.NewGuid());
		}

        string id;
        public string Id { get { return id; } set { id = value; } }

		public override string ToString (){
			return Id;
		}

		public static implicit operator string(FunctionId id){
			return id.ToString();
		}
		public static implicit operator FunctionId(string s){
			return new FunctionId(s);
		}
	}
}

