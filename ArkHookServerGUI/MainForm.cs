using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.GUI
{
	public partial class MainForm : Form
	{
		public event EventHandler<string> OnConsoleInput;
		public event EventHandler OnUserInterrupt;
		public event EventHandler<string> OnCommitBattleReplay;

		public ILogger<string> Logger
		{
			get => _logger;
		}

		private readonly List<string> inputHistory = new();
		private string commandBuffer;
		private int currentHistoryIdx;
		private readonly LoggerWrapper _logger;

		public MainForm()
		{
			InitializeComponent();
			_logger = new LoggerWrapper(ConsoleOutputBox);
		}

		public void AddCommandAutoCompletions(string[] commands)
		{
			CommandInputBox.AutoCompleteCustomSource.AddRange(commands);
		}

		private void CommandInputBox_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Up:
					RollUpwards();
					break;

				case Keys.Down:
					RollDownwards();
					break;
				
				case Keys.Escape:
					ResetHistoryLookupStatus();
					CommandInputBox.Clear();
					break;

				case Keys.Enter:
					FlushConsoleInput();
					break;

				case Keys.C when e.Control && CommandInputBox.SelectionLength <= 0:
					OnUserInterrupt?.Invoke(this, EventArgs.Empty);
					break;
			}
		}

		private void RollUpwards()
		{
			if (currentHistoryIdx >= inputHistory.Count - 1 || inputHistory.Count <= 0)
				return;
			else
				currentHistoryIdx++;

			if (currentHistoryIdx == 0)
				commandBuffer = CommandInputBox.Text ?? string.Empty;

			CommandInputBox.Text = inputHistory[currentHistoryIdx];
		}

		private void RollDownwards()
		{
			if (currentHistoryIdx < 0 || inputHistory.Count <= 0)
				return;
			else
				currentHistoryIdx--;

			CommandInputBox.Text = currentHistoryIdx > -1
				? inputHistory[currentHistoryIdx]
				: commandBuffer;

			if (currentHistoryIdx == -1)
				commandBuffer = null;
		}

		private void ResetHistoryLookupStatus()
		{
			currentHistoryIdx = -1;
			commandBuffer = null;
		}

		private void FlushConsoleInput()
		{
			string command = CommandInputBox.Text.Trim(' ', '\r', '\n');

			if (!string.IsNullOrWhiteSpace(command))
			{
				if (commandBuffer != null && command == inputHistory[currentHistoryIdx]) 
					inputHistory.RemoveAt(currentHistoryIdx);

				inputHistory.Insert(0, command);
			}

			ResetHistoryLookupStatus();
			OnConsoleInput?.Invoke(this, command);
			CommandInputBox.Clear();
		}

		private static void HandleDragOver(DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) 
				? DragDropEffects.Link 
				: DragDropEffects.None;
		}

		private void Base64InputBox_DragOver(object sender, DragEventArgs e)
		{
			HandleDragOver(e);
		}

		private static bool TryGetFilePathFromDragArgs(DragEventArgs e, out string path)
		{
			path = string.Empty;
			if(e.Data.GetData(DataFormats.FileDrop) is not Array array 
			   || array.GetValue(0) is not string filePath
			   || !File.Exists(filePath))
				return false;

			path = filePath;
			return true;
		}

		private void Base64InputBox_DragDrop(object sender, DragEventArgs e)
		{
			if(!TryGetFilePathFromDragArgs(e, out string filePath))
				return;
			
			OnCommitBattleReplay?.Invoke(this, 
				File.ReadAllText(filePath)
					.Replace(" ", null)
					.Replace("\n", null)
					.Replace("\r", null));
		}

		private void CommandInputBox_DragDrop(object sender, DragEventArgs e)
		{
			if(!TryGetFilePathFromDragArgs(e, out string filePath))
				return;

			if(!(string.IsNullOrEmpty(CommandInputBox.Text) || CommandInputBox.Text.EndsWith(' ')))
				CommandInputBox.AppendText(" ");

			CommandInputBox.AppendText(filePath);
			CommandInputBox.AppendText(" ");
		}

		private void CommandInputBox_DragOver(object sender, DragEventArgs e)
		{
			HandleDragOver(e);
		}

		private void Base64InputBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Enter || string.IsNullOrWhiteSpace(Base64InputBox.Text))
				return;
			
			OnCommitBattleReplay?.Invoke(this, Base64InputBox.
				Text.Replace(" ", null)
				.Replace("\n", null)
				.Replace("\r", null));
		}

		private class LoggerWrapper : ILogger<string>, IDisposable
		{
			private readonly TextBox textBox;

			private Queue<string> pendingLogs = new();

			public LoggerWrapper(TextBox textBox)
			{
				this.textBox = textBox;
			}
			
			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				string levelString = logLevel switch
				{
					LogLevel.Trace => "Trace",
					LogLevel.Debug => "Debug",
					LogLevel.Information => "Info ",
					LogLevel.Warning => "Warn ",
					LogLevel.Error => "Error",
					LogLevel.Critical => " !!! ",
					LogLevel.None => "",
					_ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
				};
				
				pendingLogs.Enqueue(
					$"[{DateTime.Now:hh:mm:ss}{(logLevel == LogLevel.None ? "      " : $"/{levelString}")}] {formatter.Invoke(state, exception)}");
			}

			public void Flush()
			{
				while (pendingLogs.Count > 0)
				{
					string logItem = pendingLogs.Dequeue().Trim('\r', '\n', ' ');
					textBox.AppendText(string.Concat("\r\n", logItem));
				}
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public IDisposable BeginScope<TState>(TState state)
			{
				return this;
			}

			public void Dispose()
			{
				textBox.Clear();
			}
		}

		private void LogTimer_Tick(object sender, EventArgs e)
		{
			_logger.Flush();
		}
	}
}
