using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    class SubscriptionEventArgs : EventArgs
    {
        public Subscription Subscription { get; set; }
        public string Key { get; set; }
        public string Operation { get; set; }
    }
}
