using System;
using System.Linq;
using ManyConsole;

namespace Actors
{
	public class Program
	{
		public static int Main (string[] args)
		{
			var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program))
				.ToArray();
			var interactive = new InteractiveCommand(commands);
			return ConsoleCommandDispatcher.DispatchCommand(commands
				.Concat(Enumerable.Repeat(interactive,1)), args,Console.Out);
		}
	}
}

