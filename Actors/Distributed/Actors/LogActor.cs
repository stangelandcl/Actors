using System;
using System.IO;
using System.Diagnostics;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;

namespace Cls.Actors
{
	public class LogActor : DistributedActor
	{
		public LogActor (string shortName = "System.Log")
			: base(shortName)
		{
			writer = new StreamWriter(File.Open(Filename(), FileMode.Append,FileAccess.Write, FileShare.ReadWrite));
		}
		StreamWriter writer;

		static string Filename ()
		{
			var proc = Process.GetCurrentProcess ().MainModule.FileName;
			var filename = Path.Combine (Path.GetDirectoryName (proc), "Logs", Path.GetFileNameWithoutExtension (proc) + ".log");
			if (!Directory.Exists (Path.GetDirectoryName (filename)))
				Directory.CreateDirectory (Path.GetDirectoryName (filename));
			return filename;
		}

		void Write(IRpcMail mail, string name, string type, object[] args){
			var str = string.Format(DateTime.Now + " " +  name + "[" + type + "] " + args.ToDelimitedString());
			Console.WriteLine(str);
			lock(writer)
				writer.WriteLine(str);
		}

		protected override void Disposing (bool b)
		{
			base.Disposing (b);
			writer.Dispose();
		}

	}
}

