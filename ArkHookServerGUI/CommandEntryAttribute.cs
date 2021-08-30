using System;

namespace ArkLike.HookServer.Launcher
{
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
}