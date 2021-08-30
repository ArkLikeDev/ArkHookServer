using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.Launcher
{
	public static class Program
	{
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
			static async Task AwaitWithCatch(Func<string[], Task<Commands.CommandExecResult>> func, string[] args, string rawInvokeInfo)
			{
				try
				{
					Commands.CommandExecResult result = await func(args);

					if(result == Commands.CommandExecResult.Failed)
						ALLog.GlobalLogger.LogError($"Failed to execute command: {rawInvokeInfo}");
				}
				catch (Exception e)
				{
					ALLog.GlobalLogger.LogError($"Error occurred while executing command: {rawInvokeInfo}\r\n{e}");
				}
			}
			
			ALLog.GlobalLogger.Log(LogLevel.None, $">>>{commandWithArgs}");
			if(!CommandUtils.ParseCommandWithArgs(commandWithArgs, out string command, out string[] args)
				|| string.IsNullOrWhiteSpace(command)
				|| !CommandUtils.TryGetCommand(command, out var func))
				return;
			
			Task.Run(() => AwaitWithCatch(func, args, commandWithArgs));
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

		}
	}
}
