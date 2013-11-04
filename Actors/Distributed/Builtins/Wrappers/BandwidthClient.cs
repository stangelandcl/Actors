using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Cls.Actors.Examples.Clients
{
    public class BandwidthClient
    {
        public BandwidthClient(IBandwidth bandwidthActorProxy, IPing ping)
        {
            this.proxy = bandwidthActorProxy;
            this.ping = ping;
        }
        IBandwidth proxy;
        IPing ping;

        public BandwidthResult Test(int byteCount = 256 * 1024)
        {
            var r = new BandwidthResult();
            var pinger = new PingClient(ping);
            var pingTime = pinger.Ping();
            r.RountTripTimeInMilliseconds = pinger.Ping();                                      

            var data = BandwidthActor.RandomData(byteCount);
            var sw = Stopwatch.StartNew();
            proxy.Upload(data);
            sw.Stop();

            r.UploadTimeInSeconds = (sw.Elapsed.TotalSeconds - pingTime / 1000.0);
            r.UploadBytesPerSecond = data.Length / r.UploadTimeInSeconds;

            sw = Stopwatch.StartNew();
            proxy.Download((int)data.Length);
            sw.Stop();

            r.DownloadTimeInSeconds = (sw.Elapsed.TotalSeconds - pingTime / 1000.0);
            r.DownloadBytesPerSecond = data.Length / r.DownloadTimeInSeconds;

            return r;
        }

      
    }
    public class BandwidthResult
    {       
        public double UploadTimeInSeconds;
        public double DownloadTimeInSeconds;
        public double UploadBytesPerSecond;
        public double DownloadBytesPerSecond;
        public double RountTripTimeInMilliseconds;

        public override string ToString()
        {
            return "Up= " + UploadBytesPerSecond + " time= " + UploadTimeInSeconds + Environment.NewLine +
                "Down= " + DownloadBytesPerSecond + " time= " + DownloadTimeInSeconds + Environment.NewLine +
                "Ping= " + RountTripTimeInMilliseconds;
        }
    }
}
