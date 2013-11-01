using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public class BandwidthActor : DistributedActor
    {
        public BandwidthActor(string shortname = "System.Bandwidth")
            : base(shortname)
        { }       

        void Upload(IMail m, byte[] bytes)
        {
            Node.Reply(m, true);
        }

        void Download(IMail m, int count)
        {
            var bytes = RandomData((int)count);
            Node.Reply(m, bytes);
        }

        static readonly Random rand = new Random();
        /// <summary>
        ///  create random data so caching and compression do not affect the rate. this algorithm is faster than random.nextbytes()        
        /// </summary>
        /// <param name="byteCount">number of random bytes to generate. Rounded up to some nice boundary, like 4 bytes, for quickly creating
        /// the random data.</param>
        /// <returns></returns>
        public unsafe static byte[] RandomData(int byteCount)
        {
            var bytes = new byte[Math.Max((byteCount + 3) / 4 * 4, 4)];
            fixed (byte* b = bytes)
            {
                int* c = (int*)b;
                int h;
                lock (rand)
                    h = rand.Next();
                var end = c + bytes.Length / 4;
                *c++ = h;
                while (c < end)
                {
                    h *= 37;
                    *c++ = h;
                }
            }
            return bytes;
        } 
    }
}
