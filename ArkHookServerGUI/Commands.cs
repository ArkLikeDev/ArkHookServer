using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace ArkLike.HookServer.GUI
{
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
			foreach (MethodInfo method in typeof(Commands).GetMethods())
			{
				foreach (Attribute attribute in method.GetCustomAttributes())
					switch (attribute)
					{
						case CommandEntryAttribute commandEntry:
							switch (114514)
							{
								case 114514 when commandEntry.FuncType.IsAssignableTo(typeof(Func<string[], CommandExecResult>)):
									CommandEntries.Add(commandEntry.Entry, async args =>
									{
										return await Task.Run(() => method.CreateDelegate<Func<string[], CommandExecResult>>()(args));
									});
									break;

								case 114514 when commandEntry.FuncType.IsAssignableTo(typeof(Func<string[], Task<CommandExecResult>>)):
									CommandEntries.Add(commandEntry.Entry, method.CreateDelegate<Func<string[], Task<CommandExecResult>>>());
									break;
							}
							break;

						case HelpEntryAttribute helpEntry:
							HelpEntries.Add(helpEntry.Data.Entry, helpEntry.Data);
							break;
					}

				foreach (Attribute attribute in method.GetCustomAttributes())
					switch (attribute)
					{
						case InheritedCommandEntryAttribute commandEntry:
							CommandEntries.Add(commandEntry.Entry, CommandEntries[commandEntry.Base]);
							break;

						case InheritedHelpEntryAttribute helpEntry:
							HelpEntries.Add(helpEntry.Entry, HelpEntries[helpEntry.Base]);
							break;
					}
			}
		}
		
		[UsedImplicitly]
		[HelpEntry("help", "List commands, or get help of specified command.", 0,"command")]
		[CommandEntry("help", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("man", "help")]
		[InheritedCommandEntry("info", "help")]
		public static CommandExecResult GetHelp(params string[] args)
		{
			if ((args?.Length ?? 0) <= 0)
			{
				ALLog.GlobalLogger.LogInformation(
					HelpEntries.Keys.Aggregate("Available Commands: ", (cur, command) => string.Concat(cur, command, ", ")).TrimEnd(',', ' '));
				ALLog.GlobalLogger.LogInformation("use 'help <command>' to get the help of specified command.");
			}
			else if(args.Length > 0 && CommandUtils.TryGetHelpEntry(args[0], out CommandHelpData helpData))
			{
				ALLog.GlobalLogger.LogInformation($"Usage: {helpData.Entry} {helpData.GetParamsString()}");
				if(!string.IsNullOrEmpty(helpData.HelpText))
					ALLog.GlobalLogger.LogInformation(helpData.HelpText);
			}
				
			return CommandExecResult.Succeeded;
		}

		[UsedImplicitly]
		[HelpEntry("attach", "Attach to existing game process.")]
		[CommandEntry("attach", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult Attach(params string[] args)
		{
			throw new NotImplementedException();
		}

		[UsedImplicitly]
		[HelpEntry("attach-spawn", "Spawn game instance and attach.")]
		[InheritedHelpEntry("attach-with-spawn", "attach-spawn")]
		[InheritedHelpEntry("spawn", "attach-spawn")]
		[CommandEntry("attach-spawn", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("attach-with-spawn", "attach-spawn")]
		[InheritedCommandEntry("spawn", "attach-spawn")]
		public static CommandExecResult AttachWithSpawn(params string[] args)
		{
			return CommandExecResult.Succeeded;
		}

		[UsedImplicitly]
		[HelpEntry("echo", "Write output to console. For test purpose only.", parameters:"text")]
		[CommandEntry("echo", typeof(Func<string[], CommandExecResult>))]
		public static CommandExecResult Echo(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation(args.Aggregate("", string.Concat));
			return CommandExecResult.Succeeded;
		}

		[UsedImplicitly]
		[HelpEntry("about", "Show version information.")]
		[InheritedHelpEntry("version", "about")]
		[CommandEntry("about", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("version", "about")]
		public static CommandExecResult About(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation(Assembly.GetExecutingAssembly().FullName);
			return CommandExecResult.Succeeded;
		}

		[UsedImplicitly]
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

		[UsedImplicitly]
		[HelpEntry("list-devices", "List existing devices in frida device manager.")]
		[InheritedHelpEntry("list", "list-devices")]
		[CommandEntry("list-devices", typeof(Func<string[], CommandExecResult>))]
		[InheritedCommandEntry("list", "list-devices")]
		public static CommandExecResult ListDevices(params string[] args)
		{
			ALLog.GlobalLogger.LogInformation($"list of frida devices attached:\r\n{FridaUtils.GetFridaDeviceListAsString()}");

			return CommandExecResult.Succeeded;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class CommandEntryAttribute : Attribute
	{
		public readonly string Entry;
		public readonly Type FuncType;

		public CommandEntryAttribute(string entry, Type funcType)
		{
			Entry = entry;
			FuncType = funcType;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class InheritedCommandEntryAttribute : Attribute
	{
		public readonly string Entry;
		public readonly string Base;

		public InheritedCommandEntryAttribute(string entry, string @base)
		{
			Entry = entry;
			Base = @base;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class HelpEntryAttribute : Attribute
	{
		public CommandHelpData Data;

		public HelpEntryAttribute(string entry, [CanBeNull] string helpText = null, int optionalIndex = int.MaxValue, [CanBeNull] params string[] parameters)
		{
			Data = new CommandHelpData(entry, helpText, parameters, optionalIndex);
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class InheritedHelpEntryAttribute : Attribute
	{
		public readonly string Entry;
		public readonly string Base;

		public InheritedHelpEntryAttribute(string entry, string @base)
		{
			Entry = entry;
			Base = @base;
		}
	}

	internal readonly struct CommandHelpData
	{
		public readonly string Entry;
		public readonly string HelpText;
		public readonly string[] Parameters;
		public readonly int OptionalIndex;

		public CommandHelpData(string entry, string helpText, string[] parameters, int optionalIndex)
		{
			Entry = entry;
			HelpText = helpText;
			Parameters = parameters;
			OptionalIndex = Math.Clamp(optionalIndex, 0, parameters?.Length ?? 1);
		}

		public string GetParamsString()
		{
			static string ConnectParams(string cur, string param)
			{
				return string.Concat(cur, $"<{param}> ");
			}
			
			if (Parameters is not {Length: > 0})
				return "<No Parameters>";

			string[] necessaryParams = OptionalIndex > 0
				? Parameters[..OptionalIndex]
				: Array.Empty<string>();

			string[] optionalParams = OptionalIndex < Parameters.Length
				? Parameters[OptionalIndex..]
				: Array.Empty<string>();
			
			return $"{necessaryParams.Aggregate("", ConnectParams).Trim()}{(optionalParams.Length > 0 ? $" [{optionalParams.Aggregate("", ConnectParams).Trim()}]" : "")}".TrimStart();
		}
	}
}
