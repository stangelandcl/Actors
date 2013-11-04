using System;
using System.IO;

namespace Cls.Serialization
{
    public interface ISerializer
	{
        void Serialize<T>(Stream stream, T item);
        T Deserialize<T>(Stream stream);
	}

	public interface ISerializer<T>
	{
		void Serialize(Stream stream, T item);
		T Deserialize(Stream stream);
	}

    public static class SerializerExtensions
    {
        public static ArraySegment<byte> Serialize<T>(this ISerializer serializer, T item)
        {
            var ms = new MemoryStream();
            serializer.Serialize(ms, item);
            return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Position);
        }
        public static byte[] SerializeToBytes<T>(this ISerializer serializer, T item)
        {
            var ms = new MemoryStream();
            serializer.Serialize(ms, item);
            var buf = ms.GetBuffer();
            Array.Resize(ref buf, (int)ms.Position);
            return buf;
        }
        public static T Deserialize<T>(this ISerializer serializer, ref ArraySegment<byte> bytes)
        {
            var ms = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count);
            return serializer.Deserialize<T>(ms);
        }
        public static T Deserialize<T>(this ISerializer serializer, byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            return serializer.Deserialize<T>(ms);
        }
        public static object Deserialize(this ISerializer serializer, Stream stream)
        {
            return serializer.Deserialize<object>(stream);
        }
    }
}

