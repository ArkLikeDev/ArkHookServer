using System;

namespace ArkLike.HookServer.Launcher
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class InheritedHelpEntryAttribute : Attribute
	{
		public readonly string Entry;
		public readonly string Base;

		public InheritedHelpEntryAttribute(string entry, string @base)
		{
			Entry = entry;
			Base = @base;
		}
	}
}