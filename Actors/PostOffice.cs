using System;
using System.Collections.Generic;

namespace NetworkTransport
{
	public class PostOffice
	{
		public PostOffice (MessageReader client)
		{
			client.MessageReceived += HandleMessageReceived;
		}

		Dictionary<int, Actor> mailboxes = new Dictionary<int, Actor>();

		void HandleMessageReceived (Message obj)
		{
			var message = 
		}
	}
}

