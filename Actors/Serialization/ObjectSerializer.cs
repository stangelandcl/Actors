using System;
using System.IO;
using System.Collections.Generic;

namespace Actors
{
	public class ObjectSerializer : ISerializer
	{
		Dictionary<byte, ISerializer> keyToSerializer = new Dictionary<byte, ISerializer>();
		Dictionary<Type, Tuple<byte, ISerializer>> typeToSerializer = new Dictionary<Type, Tuple<byte, ISerializer>>();

		public ObjectSerializer(){
			Add(1, typeof(byte[]), new ByteArraySerializer());
			Add(2, typeof(string), new StringSerializer());
			Add(3, typeof(object), new JsonSerializer());
			Add(4, typeof(int), new Int32Serializer());
		}

		void Add(byte index, Type type, ISerializer serializer){
			keyToSerializer.Add(index, serializer);
			typeToSerializer.Add(type, Tuple.Create(index, serializer));
		}

		Tuple<byte, ISerializer> GetSerializer(Type t){
			Tuple<byte, ISerializer> s;
			if(typeToSerializer.TryGetValue(t, out s))
				return s;
			return typeToSerializer[typeof(object)];
		}

		public void Serialize<T>(System.IO.Stream stream, T item)
		{
			var serializer = GetSerializer(item.GetType());
			var w = new BinaryWriter(stream);
			w.Write(serializer.Item1);
			w.Flush();
			serializer.Item2.Serialize(stream, item);
		}

		public T Deserialize<T>(System.IO.Stream stream)
		{
			var index = new BinaryReader(stream).ReadByte();
			return keyToSerializer[index].Deserialize<T>(stream);
		}
	}
}

