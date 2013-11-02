using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace Actors
{
	public class RpcMailSerializer : SpecificSerializer<IRpcMail>
	{
		public RpcMailSerializer(Serializers s, TypeMap map)
			: base(s){this.map = map;}
		TypeMap map;

		protected override IRpcMail Deserialize(Stream stream){
			var typeName = serializer.Get(typeof(string)).Deserialize<string>(stream);
			var type = map.GetType(typeName);
			var mail = (IRpcMail)type.CreateInstance();
			mail.To = serializer.Get(typeof(IActorId)).Deserialize<IActorId>(stream);
			mail.From = serializer.Get(typeof(IActorId)).Deserialize<IActorId>(stream);
			mail.MessageId = serializer.Get(typeof(IMessageId)).Deserialize<IMessageId>(stream);
			var func = new FunctionCall();
			func.Name = serializer.Get(typeof(string)).Deserialize<string>(stream);
			func.Args = serializer.Get(typeof(object[])).Deserialize<object[]>(stream);
			mail.Message = func;
			return mail;
		}

		protected override void Serialize(Stream stream, IRpcMail mail){
			var type= mail.GetType();
			serializer.Get(typeof(string)).Serialize(type.FullName);
			serializer.Get(mail.To.GetType()).Serialize(stream, mail.To);
			serializer.Get(mail.From.GetType()).Serialize(stream, mail.From);
			serializer.Get(mail.MessageId.GetType()).Serialize(stream, mail.MessageId);
			serializer.Get(typeof(string)).Serialize(stream, mail.Message.Name);
			serializer.Get(typeof(object[])).Serialize(stream, mail.Message.Args);
		}
	}
}

