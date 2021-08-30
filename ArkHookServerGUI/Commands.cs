using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace ArkLike.HookServer.Launcher
{
#pragma warning disable IDE0060 // 删除未使用的参数
	internal static class Commands
	{
		public enum CommandExecResult
		{
			Pending,
			Succeeded,
			Failed,
		}

		public static readonly SortedDictionary<string, CommandHelpData> HelpEntries = new();
		public static readonly Dictionary<string, Func<string[], Task<CommandExecResult>>> CommandEntries = new();

		static Commands()
		{
			CommandInitializer.InitializeCommandsInType(typeof(Commands));
			CommandInitializer.InitializeCommandsInType(typeof(LauncherCommands));
		}
		
		[HelpEntry("help", "List commands, or get help of specified command.", 0,"command")]
		[CommandEntry("help", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("man", "help")]
		[InheritedCommandEntry("info", "help")]
		public static CommandExecResult GetHelp(params string[] args)
		{
			if (args is not {Length: > 0})
			{
				ALLog.GlobalLogger.LogInformation(
					HelpEntries.Keys.Aggregate("Available Commands: ", (cur, command) => string.Concat(cur, command, ", ")).TrimEnd(',', ' '));
				ALLog.GlobalLogger.LogInformation("use 'help <command>' to get the help of specified command.");
			}
			else if(args.Length > 0)
			{
				CommandUtils.TryPrintHelpInfo(args[0]);
			}
				
			return CommandExecResult.Succeeded;
		}
		
		[HelpEntry("attach", "Attach to existing game process.")]
		[CommandEntry("attach", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult Attach(params string[] args)
		{
			throw new NotImplementedException();
		}
		
		[HelpEntry("attach-spawn", "Spawn game instance and attach.")]
		[InheritedHelpEntry("attach-with-spawn", "attach-spawn")]
		[InheritedHelpEntry("spawn", "attach-spawn")]
		[CommandEntry("attach-spawn", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("attach-with-spawn", "attach-spawn")]
		[InheritedCommandEntry("spawn", "attach-spawn")]
		public static CommandExecResult AttachWithSpawn(params string[] args)
		{
			throw new NotImplementedException();
		}
		
		[HelpEntry("echo", "Write output to console. For test purpose only.", parameters:"text")]
		[CommandEntry("echo", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult Echo(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation(args.Aggregate("", string.Concat));
			return CommandExecResult.Succeeded;
		}
		
		[HelpEntry("about", "Show version information.")]
		[InheritedHelpEntry("version", "about")]
		[CommandEntry("about", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("version", "about")]
		public static CommandExecResult About(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation(Assembly.GetExecutingAssembly().FullName);
			return CommandExecResult.Succeeded;
		}
		
		[HelpEntry("exit", "Does what it says.")]
		[InheritedHelpEntry("quit", "exit")]
		[CommandEntry("exit", typeof(Func<string[], Task<CommandExecResult>>))]
		[InheritedCommandEntry("quit", "exit")]
		public static async Task<CommandExecResult> Exit(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation("Goodbye");
			await Task.Delay(800);
			Environment.Exit(0);

			return CommandExecResult.Failed;//理论上不会执行到这一句
		}
		
		[HelpEntry("list-devices", "List existing devices in frida device manager.")]
		[InheritedHelpEntry("list", "list-devices")]
		[CommandEntry("list-devices", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("list", "list-devices")]
		public static CommandExecResult ListDevices(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation($"list of frida devices attached:\r\n{FridaUtils.GetFridaDeviceListAsString()}");

			return CommandExecResult.Succeeded;
		}

		[HelpEntry("bind", "Close existing one and set up a new battle replay pusher. Binding string example:'tcp://127.0.0.1:5555'", parameters:"binding string | -p port")]
		[CommandEntry("bind", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult BrServiceBind(params string[] args)
		{
			static void LogSucceededInfo()
			{
				ALLog.GlobalLogger.LogInformation($"Successfully bind to endpoint: {BattleReplayPusher.EndPoint}.");
			}
			
			try
			{
				switch (args)
				{
					case {Length: 1}:
						BattleReplayPusher.Open(args[0]);
						LogSucceededInfo();
						return CommandExecResult.Succeeded;

					case {Length: 2}:
						switch (args[0])
						{
							case "-p":
								if (int.TryParse(args[1], out int port))
								{
									BattleReplayPusher.Open(port);
									LogSucceededInfo();
									return CommandExecResult.Succeeded;
								}
								break;
						}
						break;
				}

				CommandUtils.PrintWrongUsageInfo("bind");
				return CommandExecResult.Failed;
			}
			catch (Exception)
			{
				ALLog.GlobalLogger.LogError($"Cannot bind to: {args.LastOrDefault()}");
				throw;
			}
		}


		[HelpEntry("unbind", "Unbind all endpoints of pusher.")]
		[CommandEntry("unbind", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult BrServiceUnbind(params string[] args)
		{
			if (!BattleReplayPusher.IsListening)
			{
				ALLog.GlobalLogger.LogInformation("You must bind a pusher before you unbind it.");
				return CommandExecResult.Failed;
			}

			ALLog.GlobalLogger.LogInformation($"Successfully unbind [{BattleReplayPusher.EndPoint}].");
			BattleReplayPusher.Close();
			return CommandExecResult.Succeeded;
		}
	}

#pragma warning restore IDE0060 // 删除未使用的参数
}
