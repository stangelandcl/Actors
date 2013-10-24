using System;

namespace Serialization
{
#if SERIALIZATION_INTERNAL
	internal 
#else
    public
#endif
        
        interface ISerializer
	{
		byte[] Serialize<T>(T item);
		T Deserialize<T>(byte[] bytes, int offset, int count);
		T Deserialize<T>(byte[] bytes);
	}
}

