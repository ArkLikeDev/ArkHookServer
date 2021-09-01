using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ArkLike.HookServer.Launcher
{
	internal static class CommandInitializer
	{
		private static readonly HashSet<Type> initializedTypes = new();
		
		public static void InitializeCommandsInType(Type type)
		{
			if(initializedTypes.Contains(type))
				return;

			initializedTypes.Add(type);
			
			List<InheritedCommandEntryAttribute> pendingCommandEntries= new();
			List<InheritedHelpEntryAttribute> pendingHelpEntries = new();
			
			foreach (MethodInfo method in type.GetMethods().Where(m => m.IsStatic))
			{
				foreach (Attribute attribute in method.GetCustomAttributes())
					switch (attribute)
					{
						case CommandEntryAttribute commandEntry:
							Commands.CommandEntries.Add(commandEntry.Entry, ToStandardFunction(method, commandEntry.FuncType));
							break;

						case HelpEntryAttribute helpEntry:
							Commands.HelpEntries.Add(helpEntry.Data.Entry, helpEntry.Data);
							break;

						case InheritedCommandEntryAttribute inheritedCommandEntry:
							pendingCommandEntries.Add(inheritedCommandEntry);
							break;

						case InheritedHelpEntryAttribute inheritedHelpEntry:
							pendingHelpEntries.Add(inheritedHelpEntry);
							break;
					}

				foreach (InheritedCommandEntryAttribute commandEntry in pendingCommandEntries)
				{
					Commands.CommandEntries.Add(commandEntry.Entry, Commands.CommandEntries[commandEntry.Base]);
				}

				foreach (InheritedHelpEntryAttribute helpEntry in pendingHelpEntries)
				{
					Commands.HelpEntries.Add(helpEntry.Entry, Commands.HelpEntries[helpEntry.Base]);
				}

				pendingCommandEntries.Clear();
				pendingHelpEntries.Clear();
			}
		}

		private static Func<string[], CancellationToken, Task<Commands.CommandExecResult>> ToStandardFunction(MethodInfo method, Type funcType)
		{
			return 114514 switch
			{
				114514 when funcType.IsAssignableTo(typeof(Action<string[]>)) =>
					async (args, ct) =>
					{
						await Task.Run(
							() => method.CreateDelegate<Action<string[]>>()(args), ct);
						return Commands.CommandExecResult.Succeeded;
					},
				
				114514 when funcType.IsAssignableTo(typeof(Func<string[], Commands.CommandExecResult>)) =>
					async (args, ct) =>
					{
						return await Task.Run(
							() => method.CreateDelegate<Func<string[], Commands.CommandExecResult>>()(args), ct);
					},

				114514 when funcType.IsAssignableTo(typeof(Func<string[], Task<Commands.CommandExecResult>>)) =>
					async (args, ct) =>
					{
						return await Task.Run(
							() => method.CreateDelegate<Func<string[], Task<Commands.CommandExecResult>>>()(args), ct);
					},

				114514 when funcType.IsAssignableTo(
					typeof(Func<string[], CancellationToken, Task<Commands.CommandExecResult>>)) => 
					method.CreateDelegate<Func<string[], CancellationToken, Task<Commands.CommandExecResult>>>(),

				_ => throw new ArgumentOutOfRangeException(nameof(funcType),
					$@"Unable to covert {funcType} to {typeof(Func<string[], CancellationToken, Task<Commands.CommandExecResult>>)}!")
			};
		}
	}
}
