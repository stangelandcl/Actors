using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Actors.Builtins.Actors
{
    public class FileCopyActor : DistributedActor
    {
        public FileCopyActor(string shortName = "System.FileCopy")
            : base(shortName)
        { }

        class FileChunk
        {
            public string Name { get; set; }
            public long Position { get; set; }            
            public byte[] Bytes { get; set; }
        }

        static IEnumerable<FileChunk> Chunks(string name, string sourceFile)
        {
            using (var fs = File.OpenRead(sourceFile))
                foreach (var chunk in Chunks(name, fs))
                    yield return chunk;
        }

        static IEnumerable<FileChunk> Chunks(string name, Stream stream)
        {
            var r = new BinaryReader(stream);
            while (r.BaseStream.Position != r.BaseStream.Length)
            {
                var chunk = new FileChunk
                {
                    Name = name,
                    Position = r.BaseStream.Position,
                    Bytes = r.ReadBytes(
                        Math.Min(4 * 1024 * 1024, (int)(r.BaseStream.Length - r.BaseStream.Position))),
                };
                yield return chunk;
            }
        }

        void ReceiveFileChunk(Mail mail, FileChunk chunk)
        {
            using (var fs = File.OpenWrite(chunk.Name))
            {
                fs.Position = chunk.Position;
                fs.Write(chunk.Bytes, 0, chunk.Bytes.Length);
            }
        }

        public void SendFile(ActorId to, string sourceFile, string destFile)
        {
            foreach (var chunk in Chunks(destFile, sourceFile))
            {
                Node.Send(to, Box.Id, "ReceiveFileChunk", chunk);
            }
        }

        public void ReceiveFile(ActorId from, string remoteFile, string localFile)
        {
            Node.Send(from, Box.Id, "SendMeFile", remoteFile, localFile);
        }
    }
}
