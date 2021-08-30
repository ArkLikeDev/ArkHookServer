using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.Launcher
{
	internal static class CommandUtils
	{
		public static bool TryPrintHelpInfo(string entry, bool logHintWhenInvalid = true)
		{
			if (!TryGetHelpEntry(entry, out CommandHelpData helpData, logHintWhenInvalid))
				return false;

			PrintHelpInfo(helpData);
			return true;
		}

		public static void PrintWrongUsageInfo(string entry)
		{
			if (!TryGetHelpEntry(entry, out CommandHelpData helpData, false))
				return;

			ALLog.GlobalLogger.LogError($"Cannot resolve the usage of '{entry}'.");
			PrintHelpInfo(helpData);
		}
		
		public static void PrintHelpInfo(CommandHelpData helpData)
		{
			ALLog.GlobalLogger.LogInformation($"Usage: {helpData.Entry} {helpData.GetParamsString()}");
			if(!string.IsNullOrEmpty(helpData.HelpText))
				ALLog.GlobalLogger.LogInformation(helpData.HelpText);
		}
		
		public static bool TryGetCommand(string command, out Func<string[], Task<Commands.CommandExecResult>> func, 
			bool logHintWhenInvalid = true)
		{
			if (!Commands.CommandEntries.TryGetValue(command, out func))
			{
				if (!logHintWhenInvalid)
					return false;
				
				string hint = GetHintForInvalidCommand(command, Commands.CommandEntries.Keys);
				ALLog.GlobalLogger.LogInformation($"Unrecognizable command: {command}. {(!string.IsNullOrEmpty(hint) ? $"Do you mean '{hint}'?" : "" )}");
				return false;
			}

			return true;
		}

		public static bool TryGetHelpEntry(string command, out CommandHelpData commandHelpData,
			bool logHintWhenInvalid = true)
		{
			if (!Commands.HelpEntries.TryGetValue(command, out commandHelpData))
			{
				if (!logHintWhenInvalid)
					return false;
				
				string hint = GetHintForInvalidCommand(command, Commands.HelpEntries.Keys);
				ALLog.GlobalLogger.LogInformation($"Unrecognizable command: {command}. {(!string.IsNullOrEmpty(hint) ? $"Do you mean '{hint}'?" : "" )}");
				return false;
			}

			return true;
		}

		public static bool ParseCommandWithArgs(string commandWithArgs, out string command, out string[] args)
		{
			args = Array.Empty<string>();
			command = string.Empty;
			string[] subStrings = commandWithArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (subStrings.Length <= 0)
				return false;
			command = subStrings[0];
			args = subStrings[1..];
			return true;
		}
		
		public static string GetHintForInvalidCommand(string command, IEnumerable<string> commandSrc)
		{
			return commandSrc
				.OrderBy(x => x.LevenshteinDistance(command))
				.FirstOrDefault();
		}
		
	}
}
