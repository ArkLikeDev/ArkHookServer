using System;

namespace ArkLike.HookServer.Launcher
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class HelpEntryAttribute : Attribute
	{
		public CommandHelpData Data;

		public HelpEntryAttribute(string entry, string helpText = null, int optionalIndex = int.MaxValue, params string[] parameters)
		{
			Data = new CommandHelpData(entry, helpText, parameters, optionalIndex);
		}
	}
}