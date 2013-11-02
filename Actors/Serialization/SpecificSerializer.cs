using System;
using System.IO;

namespace Actors
{
	public abstract class SpecificSerializer<T> : ISerializer
	{
		public SpecificSerializer (Serializers serializer)
		{
			this.serializer = serializer;
		}
		protected Serializers serializer;
		protected abstract void Serialize(Stream stream, T item);
		protected abstract T Deserialize(Stream stream);


		#region ISerializer implementation

		public void Serialize<T2> (System.IO.Stream stream, T2 item)
		{
			Serialize(stream, item.Convert<T>());
		}

		public T2 Deserialize<T2> (System.IO.Stream stream)
		{
				return Deserialize(stream).Convert<T2>();
		}

		#endregion
	}
}

