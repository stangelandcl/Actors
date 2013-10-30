using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;

namespace DistributedActors.Mail
{
    public class RpcMail : IRpcMail<ActorId, MessageId>
    {
        public ActorId From { get; set; }
        public ActorId To { get; set; }
        public MessageId MessageId { get; set; }
        public FunctionCall Message { get; set; }     
    }
}
