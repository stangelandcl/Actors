using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace NetworkTransport
{
	public class MessageReader
	{
		public event Action<Message> MessageReceived;

		public MessageReader(TcpClient client){
			Client = new MessageClient(client);
		}

		public MessageClient Client {get; private set;}

		public void BeginRead (TcpClient socket)
		{
			var message = new Message{Buffer = new byte[4], Client = socket, Reader = this};
			BeginRead (message, EndReadHeader);
		}

		void BeginRead(Message message, AsyncCallback cb)
		{
			message.Client.Client.BeginReceive (message.Buffer, message.Count, message.Buffer.Length, SocketFlags.None, cb, message);
		}

		void EndReadHeader(IAsyncResult ar){
			try{
				var msg = (Message)ar.AsyncState;
				int count = msg.Client.Client.EndReceive(ar);
				if(count <= 0) return;
				msg.Count += count;
				if(msg.Count == msg.Buffer.Length){
					msg.Count = 0;
					msg.Buffer = new byte[BitConverter.ToInt32(msg.Buffer,0)];
					BeginRead(msg, EndReadMessage);					
				}else{
					BeginRead(msg, EndReadHeader);
				}
			}catch(Exception ex){
				Trace.WriteLine("Error in read header" + ex);
			}
		}

		void EndReadMessage(IAsyncResult ar){
			try{
				var msg = (Message)ar.AsyncState;
				int count = msg.Client.Client.EndReceive(ar);
				if(count <= 0) return;
				msg.Count += count;
				if(msg.Count > msg.Buffer.Length)
					return;
				if(msg.Count == msg.Buffer.Length){
					if(MessageReceived != null)
						MessageReceived(msg);
				}else{
					BeginRead(msg, EndReadMessage);
				}
			}catch(Exception ex){
				Trace.WriteLine("Error in read message " + ex);
			}
		}
	}
}

