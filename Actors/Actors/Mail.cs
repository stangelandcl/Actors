using System;
using System.Diagnostics;

namespace Actors
{
    /// <summary>
    /// A message passed between actors
    /// </summary>
    [DebuggerDisplay("Name={Name} From={From} To={To} Args={Args.Length}")]
    public class Mail
    {
        /// <summary>
        /// Used so a receiver can wait for a reply to this specific request.        
        /// </summary>
        public MessageId MessageId { get; set; }
        public ActorId From { get; set; }
        public ActorId To { get; set; }
        public FunctionId Name { get; set; }
        public object[] Args { get; set; }
    }
}

