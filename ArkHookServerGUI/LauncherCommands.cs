using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.Launcher
{
	internal static class LauncherCommands
	{
		[HelpEntry("start", "Launches the server process.")]
		[CommandEntry("start", typeof(Func<string[], Commands.CommandExecResult>))]
		public static Commands.CommandExecResult LaunchServer(params string[] args)
		{
			return ServerLauncher.LaunchServer(args is {Length: > 0} ? args.Aggregate((s, s1) => $"{s} {s1}") : string.Empty)
				? Commands.CommandExecResult.Succeeded
				: Commands.CommandExecResult.Failed;
		}

		[HelpEntry("kill", "kills the server process.")]
		[CommandEntry("kill", typeof(Func<string[], Commands.CommandExecResult>))]
		public static Commands.CommandExecResult KillServer(params string[] args)
		{
			if (!ServerLauncher.IsServerRunning)
			{
				ALLog.GlobalLogger.LogInformation("Server is not running.");
				return Commands.CommandExecResult.Failed;
			}
			
			ServerLauncher.ServerProcess.Kill(true);
			ALLog.GlobalLogger.LogInformation("The server has been shut down.");
			return Commands.CommandExecResult.Succeeded;
		}
	}
}