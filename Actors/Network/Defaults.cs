using System;
using Serialization;

namespace Actors
{
	public static class Defaults
	{
		public static ISerializer Serializer = new JsonSerializer();
		
	}
}

