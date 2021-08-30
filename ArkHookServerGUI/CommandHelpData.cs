using System;
using System.Linq;

namespace ArkLike.HookServer.Launcher
{
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