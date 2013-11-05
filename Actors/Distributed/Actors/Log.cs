using System;
using System.Diagnostics;
using System.IO;
using Cls.Extensions;

namespace Cls.Actors
{
    /// <summary>
    /// TODO: add rolling
    /// </summary>
	public class Log
	{
		public static Log Get(){
			var name =  new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;			
			writer = new StreamWriter(File.Open(Filename(), FileMode.Append,FileAccess.Write, FileShare.ReadWrite));
			return new Log(name);
		}

		static StreamWriter writer;

		static string Filename ()
		{
			var proc = Process.GetCurrentProcess ().MainModule.FileName;
			var filename = Path.Combine (Path.GetDirectoryName (proc), "Logs", Path.GetFileNameWithoutExtension (proc) + ".log");
			if (!Directory.Exists (Path.GetDirectoryName (filename)))
				Directory.CreateDirectory (Path.GetDirectoryName (filename));
			return filename;
		}


		void Write(string name, string type, object[] args){
			var str = string.Format(DateTime.Now + " " +  name + "[" + type + "] " + args.ToDelimitedString());
			Console.WriteLine(str);
			lock(writer)
				writer.WriteLine(str);
		}

		Log (string name)
		{

			this.name = name;
		}
		string name;



		public void Info(params object[] args){
			Write("INFO", args);
		}

		public void Error(params object[] args){
			Write("ERROR", args);
		}

		void Write(string type,  params object[] args){
			Write(name, type, args);
		}

	}
}

