using System;
using ManyConsole;
using System.Collections.Generic;

namespace Actors
{
	public class InteractiveCommand : ConsoleModeCommand
	{
		public InteractiveCommand (IEnumerable<ConsoleCommand> commands)
		{
			this.commands = commands;
		}
		IEnumerable<ConsoleCommand> commands;

		public override IEnumerable<ConsoleCommand> GetNextCommands ()
		{
			return commands;
		}
	}
}

