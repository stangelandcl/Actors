using System;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;

namespace Cls.Serialization
{
	public class ExprSerializer : SpecificSerializer<Expression>
	{
		public ExprSerializer(Serializers s)
		:base(s) {}

		ExpressionSerializer s = new ExpressionSerializer(
			new Serialize.Linq.Serializers.JsonSerializer());


		#region implemented abstract members of SpecificSerializer
		protected override void Serialize (System.IO.Stream stream, Expression item)
		{
			s.Serialize(stream, item);
		}
		protected override Expression Deserialize (System.IO.Stream stream)
		{
			return s.Deserialize(stream);
		}
		#endregion
	}
}

