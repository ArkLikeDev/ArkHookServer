
namespace ArkLike.HookServer.Launcher
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.BattleReplayInputArea = new System.Windows.Forms.GroupBox();
			this.Base64InputBox = new System.Windows.Forms.TextBox();
			this.CommandInputBox = new System.Windows.Forms.TextBox();
			this.LogTimer = new System.Windows.Forms.Timer(this.components);
			this.ConsoleOutputBox = new System.Windows.Forms.RichTextBox();
			this.BattleReplayInputArea.SuspendLayout();
			this.SuspendLayout();
			// 
			// BattleReplayInputArea
			// 
			this.BattleReplayInputArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BattleReplayInputArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.BattleReplayInputArea.Controls.Add(this.Base64InputBox);
			this.BattleReplayInputArea.Font = new System.Drawing.Font("Courier New", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.BattleReplayInputArea.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.BattleReplayInputArea.Location = new System.Drawing.Point(11, 660);
			this.BattleReplayInputArea.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.BattleReplayInputArea.Name = "BattleReplayInputArea";
			this.BattleReplayInputArea.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.BattleReplayInputArea.Size = new System.Drawing.Size(1119, 57);
			this.BattleReplayInputArea.TabIndex = 0;
			this.BattleReplayInputArea.TabStop = false;
			this.BattleReplayInputArea.Text = "Battle Replay Input(Supported type: Compressed base64 string)";
			// 
			// Base64InputBox
			// 
			this.Base64InputBox.AllowDrop = true;
			this.Base64InputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Base64InputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
			this.Base64InputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Base64InputBox.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.Base64InputBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.Base64InputBox.Location = new System.Drawing.Point(6, 24);
			this.Base64InputBox.MaxLength = 65536;
			this.Base64InputBox.Name = "Base64InputBox";
			this.Base64InputBox.PlaceholderText = "Enter base64 data here. Press enter to commit. You may also drag the file that co" +
    "ntains base64 string here.";
			this.Base64InputBox.Size = new System.Drawing.Size(1107, 26);
			this.Base64InputBox.TabIndex = 2;
			this.Base64InputBox.TabStop = false;
			this.Base64InputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Base64InputBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Base64InputBox_DragDrop);
			this.Base64InputBox.DragOver += new System.Windows.Forms.DragEventHandler(this.Base64InputBox_DragOver);
			this.Base64InputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Base64InputBox_KeyDown);
			// 
			// CommandInputBox
			// 
			this.CommandInputBox.AcceptsReturn = true;
			this.CommandInputBox.AcceptsTab = true;
			this.CommandInputBox.AllowDrop = true;
			this.CommandInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.CommandInputBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.CommandInputBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.CommandInputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(47)))), ((int)(((byte)(54)))));
			this.CommandInputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CommandInputBox.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.CommandInputBox.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.CommandInputBox.Location = new System.Drawing.Point(10, 629);
			this.CommandInputBox.Name = "CommandInputBox";
			this.CommandInputBox.PlaceholderText = ">>>";
			this.CommandInputBox.Size = new System.Drawing.Size(1120, 26);
			this.CommandInputBox.TabIndex = 1;
			this.CommandInputBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.CommandInputBox_DragDrop);
			this.CommandInputBox.DragOver += new System.Windows.Forms.DragEventHandler(this.CommandInputBox_DragOver);
			this.CommandInputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CommandInputBox_KeyDown);
			// 
			// LogTimer
			// 
			this.LogTimer.Enabled = true;
			this.LogTimer.Interval = 10;
			this.LogTimer.Tick += new System.EventHandler(this.LogTimer_Tick);
			// 
			// ConsoleOutputBox
			// 
			this.ConsoleOutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ConsoleOutputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
			this.ConsoleOutputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ConsoleOutputBox.Cursor = System.Windows.Forms.Cursors.Default;
			this.ConsoleOutputBox.ForeColor = System.Drawing.SystemColors.Window;
			this.ConsoleOutputBox.Location = new System.Drawing.Point(12, 12);
			this.ConsoleOutputBox.MaxLength = 10000000;
			this.ConsoleOutputBox.Name = "ConsoleOutputBox";
			this.ConsoleOutputBox.ReadOnly = true;
			this.ConsoleOutputBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.ConsoleOutputBox.Size = new System.Drawing.Size(1119, 611);
			this.ConsoleOutputBox.TabIndex = 2;
			this.ConsoleOutputBox.TabStop = false;
			this.ConsoleOutputBox.Text = resources.GetString("ConsoleOutputBox.Text");
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
			this.ClientSize = new System.Drawing.Size(1143, 728);
			this.Controls.Add(this.ConsoleOutputBox);
			this.Controls.Add(this.CommandInputBox);
			this.Controls.Add(this.BattleReplayInputArea);
			this.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "ArkHookServer GUI > Shell Prompt";
			this.BattleReplayInputArea.ResumeLayout(false);
			this.BattleReplayInputArea.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox BattleReplayInputArea;
		private System.Windows.Forms.TextBox CommandInputBox;
		private System.Windows.Forms.TextBox Base64InputBox;
		private System.Windows.Forms.Timer LogTimer;
		private System.Windows.Forms.RichTextBox ConsoleOutputBox;
	}
}