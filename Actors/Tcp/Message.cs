using System;
using System.Net.Sockets;

namespace Actors
{
	public class Message{
		public byte[] Buffer;
		public int Count;
		public MessageReader Reader;
	}
}

