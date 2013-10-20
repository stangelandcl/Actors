using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Actors
{
	public class MessageReader
	{
		public event Action<Message> MessageReceived;

		public MessageReader(TcpClient client){
			Client = new MessageClient(client);
		}

		public MessageClient Client {get; private set;}

		public void Listen ()
		{
			var message = new Message{Buffer = new byte[4], Client = Client.Client, Reader = this};
			Listen (message, EndReadHeader);
		}

		void Listen(Message message, AsyncCallback cb)
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
					Listen(msg, EndReadMessage);					
				}else{
					Listen(msg, EndReadHeader);
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
					ThreadPool.QueueUserWorkItem(o=> FireReceived (msg));
					Listen();
				}else{
					Listen(msg, EndReadMessage);
				}
			}catch(Exception ex){
				Trace.WriteLine("Error in read message " + ex);
			}
		}

		void FireReceived (Message msg)
		{
			try {
				if (MessageReceived != null)
					MessageReceived (msg);
			}
			catch (Exception ex) {
				Trace.WriteLine ("Error sending message" + ex);
			}
		}
	}
}

