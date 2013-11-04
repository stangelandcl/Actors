using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cls.Extensions;

namespace Cls.Serialization
{
	public class Serializers
	{
		TypeMap types = new TypeMap();
		List<KeyValuePair<Type, ISerializer>> serializers = new List<KeyValuePair<Type, ISerializer>>();
		Dictionary<Type, ISerializer> serializerMap;


		public ISerializer NamedTypeSerializer {get; private set;}

		public Serializers(){
			// order matters here
			NamedTypeSerializer = new ObjectSerializer(this, types);

			serializers.Add(KeyValuePair.New(
				typeof(byte[]), (ISerializer)new ByteArraySerializer(this)));
			serializers.Add(KeyValuePair.New(
				typeof(string), (ISerializer)new StringSerializer(this)));
			serializers.Add(KeyValuePair.New(
				typeof(int), (ISerializer)new Int32Serializer(this)));
            //serializers.Add(KeyValuePair.New(
            //    typeof(IRpcMail), (ISerializer)new RpcMailSerializer(this, types)));
			serializers.Add(KeyValuePair.New(
				typeof(object[]), (ISerializer)new ObjectArraySerializer(this)));
			serializers.Add(KeyValuePair.New(
				typeof(Expression), (ISerializer)new ExprSerializer(this)));
			serializers.Add(KeyValuePair.New(
				typeof(object), NamedTypeSerializer));		
			serializers.Add(KeyValuePair.New(
				(Type)null, (ISerializer)new JsonSerializer()));

			serializerMap = serializers.Take(serializers.Count()-1)
				.ToDictionary(n=>n.Key, n=>n.Value);
		}

		public ISerializer Get(Type t){
			ISerializer s;
			if(serializerMap.TryGetValue(t, out s))
				return s;
			return serializers
				.Where(n=> n.Key != typeof(object))
				.First(n=> n.Key == null || n.Key.IsAssignableFrom(t))
					.Value;
		}
	}
}

