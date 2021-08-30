using System;

namespace ArkLike.HookServer.Launcher
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	internal sealed class InheritedCommandEntryAttribute : Attribute
	{
		public readonly string Entry;
		public readonly string Base;

		public InheritedCommandEntryAttribute(string entry, string @base)
		{
			Entry = entry;
			Base = @base;
		}
	}
}