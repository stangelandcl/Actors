using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Actors
{
	public class Serializer : ISerializer
	{
		Serializers s = new Serializers();

		public void Serialize<T>(System.IO.Stream stream, T item)
		{
			s.NamedTypeSerializer.Serialize(stream, item);
		}

		public T Deserialize<T>(System.IO.Stream stream)
		{
			return s.NamedTypeSerializer.Deserialize<T>(stream);
		}
	}
}

