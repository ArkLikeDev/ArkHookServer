using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ArkLike.HookServer.Launcher
{
	internal static class CommandInitializer
	{
		public static void InitializeCommandsInType(Type type)
		{
			List<InheritedCommandEntryAttribute> pendingCommandEntries= new();
			List<InheritedHelpEntryAttribute> pendingHelpEntries = new();
			
			foreach (MethodInfo method in type.GetMethods().Where(m => m.IsStatic))
			{
				foreach (Attribute attribute in method.GetCustomAttributes())
					switch (attribute)
					{
						case CommandEntryAttribute commandEntry:
							switch (114514)
							{
								case 114514 when commandEntry.FuncType.IsAssignableTo(typeof(Func<string[], Commands.CommandExecResult>)):
									Commands.CommandEntries.Add(commandEntry.Entry, async args =>
									{
										return await Task.Run(() => method.CreateDelegate<Func<string[], Commands.CommandExecResult>>()(args));
									});
									break;

								case 114514 when commandEntry.FuncType.IsAssignableTo(typeof(Func<string[], Task<Commands.CommandExecResult>>)):
									Commands.CommandEntries.Add(commandEntry.Entry, method.CreateDelegate<Func<string[], Task<Commands.CommandExecResult>>>());
									break;
							}
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
	}
}
