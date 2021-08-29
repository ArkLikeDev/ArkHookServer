using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.GUI
{
	static class Program
	{
		public static Form MainForm;
		
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
			MainForm = mainForm;
			
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
			ALLog.GlobalLogger.LogInformation($"Committed battle replay.");
		}

		private static void HandleUserInterrupt(object sender, EventArgs e)
		{

		}

		
	}
}
