using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Actors
{
	public class MessageReader
	{
		public MessageReader(TcpClient client){
			this.client = client;
			Listen();
		}
		public event Action<Message> MessageReceived;

		TcpClient client;

		public static implicit operator MessageReader(TcpClient c){
			return new MessageReader(c);
		}
		public static implicit operator TcpClient(MessageReader r){
			return r.client;
		}

		void Listen ()
		{
			var message = new Message{Buffer = new byte[4], Reader = this};
			Listen (message, EndReadHeader);
		}

		void Listen(Message message, AsyncCallback cb)
		{
			client.Client.BeginReceive (message.Buffer, message.Count, message.Buffer.Length  - message.Count, SocketFlags.None, cb, message);
		}

		void EndReadHeader(IAsyncResult ar){
			try{
				var msg = (Message)ar.AsyncState;
				int count = client.Client.EndReceive(ar);
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
				int count = client.Client.EndReceive(ar);
				if(count <= 0) return;
				msg.Count += count;
				if(msg.Count > msg.Buffer.Length)
					return;
				if(msg.Count == msg.Buffer.Length){
					Task.Factory.StartNew(()=>FireReceived (msg));
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

