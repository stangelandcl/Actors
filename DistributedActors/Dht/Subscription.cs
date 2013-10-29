﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dht;
using System.Text.RegularExpressions;

namespace Actors.Dht
{
	[Flags]
	public enum DhtOperation { 
		Add=1, 
		Remove=2, 
		AddRemove = 3,
		Get=4 ,
		
		All = unchecked((int)0xffffffff)
	}
    class Subscription 
    {
        public Subscription(ActorId id, DhtOperation op, string key)
        {
            Actor = id;
            Operations = op;
            KeyRegex = key;
        }
		public DhtOperation Operations {get;set;}
        public string KeyRegex { get; set; }
        public ActorId Actor { get; set; }
        public bool IsMatch(DhtOperation op, string key)
        {
			var match = (op & Operations) != 0;
            return match && key.IsMatch(KeyRegex);
        }

        public override int GetHashCode()
        {
            return Actor.GetHashCode() ^ KeyRegex.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;
            var o = (Subscription)obj;
            return o.Actor.Equals(Actor) && 
                Operations == o.Operations &&
                KeyRegex == o.KeyRegex;
        }
    }
}