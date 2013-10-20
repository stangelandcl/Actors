using System;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;

namespace Actors
{
	public class MessageClient
	{
		public MessageClient (TcpClient client)
		{
			this.Client = client;
		}

		public TcpClient Client {get; private set;}

		public void Send(byte[] bytes){
			SendRaw(BitConverter.GetBytes(bytes.Length), bytes);
		}

		void SendRaw(params byte[][] buffers){
			var b = buffers.Select(n=> new ArraySegment<byte>(n)).ToArray();		
			Client.Client.BeginSend(b, SocketFlags.None, EndSend, b);
		}

		void EndSend(IAsyncResult ar){
			try{
				var s = (ArraySegment<byte>[])ar.AsyncState;
				int count = Client.Client.EndSend(ar);
				int length = s.Sum(n=>n.Count);
				if(count == length)
					return;
				var buffer = s.SelectMany(n=>n.Array.Skip(n.Offset).Take(n.Count)).Skip(count).ToArray();
				SendRaw(buffer);
			}catch(Exception ex){
				Trace.WriteLine("Error " + ex);
			}
		}
	}
}

