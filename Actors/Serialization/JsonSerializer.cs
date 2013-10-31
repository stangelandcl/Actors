using System;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace Serialization
{
#if SERIALIZATION_INTERNAL
    internal
#else
	public 
#endif        
        class JsonSerializer : ISerializer, ISerializer<object>
	{
		public void Serialize<T>(Stream stream, T item){
            var str = JsonConvert.SerializeObject(item, formatting, settings);
            var w = new BinaryWriter(stream);
            w.Write(str);
            w.Flush();
		}
		public T Deserialize<T>(Stream stream)  {
            var r = new BinaryReader(stream);
			var str = r.ReadString();
            return JsonConvert.DeserializeObject<T>(str, settings);
		}

		#region ISerializer implementation
		public void Serialize (Stream stream, object item)
		{
			Serialize<object>(stream, item);
		}
		public object Deserialize (Stream stream)
		{
			return Deserialize<object>(stream);
		}
		#endregion	
      
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

