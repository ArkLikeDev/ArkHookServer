using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.Launcher
{
	public static class Program
	{
		private static readonly Stack<CtsHolder> interruptionStack = new();
		
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var mainForm = new MainForm();

			mainForm.OnConsoleInput += HandleConsoleInput;
			mainForm.OnCommitBattleReplay += HandleBattleReplayInput;
			mainForm.OnUserInterrupt += HandleUserInterrupt;
			ALLog.GlobalLogger = mainForm.Logger;
			
			mainForm.AddCommandAutoCompletions(Commands.HelpEntries.Keys.ToArray());
			
			Application.Run(mainForm);
		}

		private static void HandleConsoleInput(object sender, string commandWithArgs)
		{
			ALLog.GlobalLogger.Log(LogLevel.None, $">>>{commandWithArgs}");
			if(!CommandUtils.ParseCommandWithArgs(commandWithArgs, out string command, out string[] args)
				|| string.IsNullOrWhiteSpace(command)
				|| !CommandUtils.TryGetCommand(command, out var func))
				return;
			
			Task.Run(() => ExecuteCommand(func, args, commandWithArgs));
		}

		private static async Task ExecuteCommand(Func<string[], CancellationToken, Task<Commands.CommandExecResult>> func, string[] args, string rawInvokeInfo)
		{
			var cts = new CancellationTokenSource();
			var holder = new CtsHolder(cts);
			lock (interruptionStack)
			{
				interruptionStack.Push(holder);
			}
			
			cts.Token.Register(() =>
			{
				ALLog.GlobalLogger.LogError($"User Interruption: {rawInvokeInfo}");
			});
			try
			{
				Commands.CommandExecResult result = await func(args, cts.Token);

				if (result == Commands.CommandExecResult.Failed) ALLog.GlobalLogger.LogError($"Failed to execute command: {rawInvokeInfo}");
			}
			catch (OperationCanceledException e)
			{
				ALLog.GlobalLogger.LogError($"The operation was canceled: {rawInvokeInfo}\r\n{e}");
			}
			catch (Exception e)
			{
				ALLog.GlobalLogger.LogError($"Error occurred while executing command: {rawInvokeInfo}\r\n{e}");
			}
			finally
			{
				lock (interruptionStack)
				{
					cts.Dispose();
					holder.isDisposed = true;
				}
			}
		}

		private static void HandleBattleReplayInput(object sender, string replay)
		{
			if (!BattleReplayPusher.IsListening)
			{
				ALLog.GlobalLogger.LogInformation("Pusher has not been set up yet. Use 'bind' command to bind one.");
				return;
			}

			try
			{
				BattleReplayPusher.SendFrame(replay);
				ALLog.GlobalLogger.LogInformation("Committed battle replay.");
			}
			catch (Exception e)
			{
				ALLog.GlobalLogger.LogError($"Error occurred while committing battle replay:\r\n{e}");
			}
		}

		private static void HandleUserInterrupt(object sender, EventArgs e)
		{
			lock (interruptionStack)
			{
				while (interruptionStack.TryPop(out CtsHolder holder))
				{
					if (!holder.isDisposed)
					{
						holder.cts.Cancel();
						holder.cts.Dispose();
						return;
					}
				}
			}
		}

		private class CtsHolder
		{
			public readonly CancellationTokenSource cts;
			public bool isDisposed;

			public CtsHolder(CancellationTokenSource cts)
			{
				this.cts = cts;
				isDisposed = false;
			}
		}
	}
}
