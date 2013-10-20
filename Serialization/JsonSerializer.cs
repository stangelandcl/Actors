using System;
using Newtonsoft.Json;
using System.Text;

namespace Serialization
{
	public class JsonSerializer : ISerializer
	{
		public byte[] Serialize<T>(T item){
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item, formatting, settings));
		}
		public T Deserialize<T>(byte[] bytes){
			return Deserialize<T>(bytes, 0, bytes.Length);
		}
		public T Deserialize<T>(byte[] bytes, int offset, int count){
			var str = Encoding.UTF8.GetString(bytes, offset, count);
			return JsonConvert.DeserializeObject<T>(str, settings);
		}

		Formatting formatting = 
#if DEBUG
			Formatting.Indented;
			#else
			Formatting.None;
			#endif

		JsonSerializerSettings settings =  new JsonSerializerSettings{
			TypeNameHandling = TypeNameHandling.All,
			TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
		};
	}
}

