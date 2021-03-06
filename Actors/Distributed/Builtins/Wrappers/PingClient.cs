﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Cls.Actors.Examples.Clients
{
    public class PingClient
    {
        public PingClient(IPing pingProxy)
        {
            this.ping = pingProxy;
        }

        IPing ping;

        /// <summary>
        /// Returns avg time in milliseconds
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public double Ping(int count = 4)
        {
            var bytes = new byte[1024];
            try
            {
                ping.Ping(bytes); // initialize proxies or whatever
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < count; i++)
                    ping.Ping(bytes);
                sw.Stop();
                return (double)sw.ElapsedMilliseconds / count;
            }
            catch
            {
                return double.PositiveInfinity;
            }           
        }      
    }
}
