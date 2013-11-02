using System;

namespace Actors
{
	public class ObjectSerializer : SpecificSerializer<object> 
	{
		public ObjectSerializer (Serializers serializer, TypeMap type)
		: base(serializer)  { this.map = type;}
		TypeMap map;

		#region implemented abstract members of SpecificSerializer
		protected override void Serialize (System.IO.Stream stream, object item)
		{
			var name = map.ToString(item.GetType());
			serializer.Get(typeof(string)).Serialize(stream, name);
			serializer.Get(item.GetType()).Serialize(stream, item);
		}
		protected override object Deserialize (System.IO.Stream stream)
		{
			var name = serializer.Get(typeof(string)).Deserialize<string>(stream);
			var type = map.GetType(name);
			return serializer.Get(type).Deserialize(stream);
		}
		#endregion
	}
}

