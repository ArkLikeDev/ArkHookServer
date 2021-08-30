using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.Launcher
{
	internal static class ServerLauncher
	{
		public const string SERVER_EXEC_NAME = "AHSCore";
		public const string SERVER_STDOUT_PREFIX = "[Server Out] ";
		public const string SERVER_STDERR_PREFIX = "[Server Err] ";
		public static Process ServerProcess;
		public static bool IsServerRunning
		{
			get => ServerProcess is {HasExited: false};
		}
		
		public static bool LaunchServer(string args)
		{
			ProcessStartInfo start = new(string.Concat(Application.StartupPath, SERVER_EXEC_NAME), args);
			ServerProcess = Process.Start(start);
			if (ServerProcess == null) 
				return false;

			ServerProcess.OutputDataReceived += HandleServerProcessStdOut;
			ServerProcess.ErrorDataReceived += HandleServerProcessStdErr;
			ServerProcess.Exited += HandleServerProcessExited;
			ServerProcess.Disposed += HandleServerProcessDisposed;
			ServerProcess.EnableRaisingEvents = true;
			Application.ApplicationExit += (_, _) =>
			{
				if (ServerProcess is {HasExited: false}) ServerProcess.Kill(true);
			};
			ALLog.GlobalLogger.LogInformation($"Server started. PID: {ServerProcess.Id}");

			return true;
		}

		private static void HandleServerProcessDisposed(object sender, EventArgs e)
		{
			ServerProcess = null;
		}

		private static void HandleServerProcessStdErr(object sender, DataReceivedEventArgs e)
		{
			string info = e.Data;
			if(!string.IsNullOrWhiteSpace(info))
				ALLog.GlobalLogger.LogError(string.Concat(SERVER_STDERR_PREFIX, info));
		}

		private static void HandleServerProcessExited(object sender, EventArgs e)
		{
			if (ServerProcess.ExitCode == 0)
				ALLog.GlobalLogger.LogInformation($"The server process exited with code: 0");
			else
				ALLog.GlobalLogger.LogWarning($"The server process exited with code: {ServerProcess.ExitCode}");
			
			ServerProcess.Dispose();
			ServerProcess = null;
		}

		private static void HandleServerProcessStdOut(object sender, DataReceivedEventArgs e)
		{
			string info = e.Data;
			if(!string.IsNullOrWhiteSpace(info))
				ALLog.GlobalLogger.LogInformation(string.Concat(SERVER_STDOUT_PREFIX, info));
		}
	}
}
