using System;

namespace NetworkTransport
{
	public interface ISerializer
	{
		byte[] Serialize<T>(T item);
		T Deserialize<T>(byte[] bytes, int offset, int count);
		T Deserialize<T>(byte[] bytes);
	}
}

