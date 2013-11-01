using System;
using System.IO;
using System.Diagnostics;

namespace Actors
{
	public class Log
	{
		internal static bool initialized;
		public static void Init(string filename = null)
		{
			if(initialized) return;
			var proc = Process.GetCurrentProcess().MainModule.FileName;
			filename = filename ?? Path.Combine(
				Path.GetDirectoryName(proc), "Logs", Path.GetFileNameWithoutExtension(proc) + ".log");
			if(!Directory.Exists(Path.GetDirectoryName(filename)))
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			main = new StreamWriter(File.Open(filename, FileMode.Append,FileAccess.Write, FileShare.ReadWrite));
			initialized = true;
		}

		public static Log GetClassLogger(){
			var type = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
			return new Log(type.Name);
		}

		static StreamWriter main;

		public Log()
		{
			Init();
			var type = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
			name = type.Name;
			writer = Log.main;
		}

		public Log(string name){
			Init();
			this.name = name;
			this.writer = Log.main;
		}

		Log(string name, StreamWriter write){
			Init();
			this.name = name;
			this.writer = write;
		}


		StreamWriter writer;
		string name;

		public virtual void Write(string type,  params object[] args){
			var str = string.Format(DateTime.Now + " " +  name + "[" + type + "] " + args.ToDelimitedString());
			Console.WriteLine(str);
			lock(writer)
				writer.WriteLine(str);
		}

		public void Info( params object[] args){
			Write("INFO", args);
		}
	}


}

