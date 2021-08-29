using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArkLike.HookServer.GUI
{
	internal static class CommandUtils
	{
		public static bool TryGetCommand(string command, out Func<string[], Task<Commands.CommandExecResult>> func, 
			bool logHintWhenInvalid = true)
		{
			if (!Commands.CommandEntries.TryGetValue(command, out func))
			{
				if (!logHintWhenInvalid)
					return false;
				
				string hint = GetHintForInvalidCommand(command, Commands.CommandEntries.Keys);
				ALLog.GlobalLogger.LogInformation($"Unrecognizable command: {command}. {(!string.IsNullOrEmpty(hint) ? $"Do you mean '{hint}'?" : "" )}");
				return false;
			}

			return true;
		}

		public static bool TryGetHelpEntry(string command, out CommandHelpData commandHelpData,
			bool logHintWhenInvalid = true)
		{
			if (!Commands.HelpEntries.TryGetValue(command, out commandHelpData))
			{
				if (!logHintWhenInvalid)
					return false;
				
				string hint = GetHintForInvalidCommand(command, Commands.HelpEntries.Keys);
				ALLog.GlobalLogger.LogInformation($"Unrecognizable command: {command}. {(!string.IsNullOrEmpty(hint) ? $"Do you mean '{hint}'?" : "" )}");
				return false;
			}

			return true;
		}

		public static bool ParseCommandWithArgs(string commandWithArgs, out string command, out string[] args)
		{
			args = Array.Empty<string>();
			command = string.Empty;
			string[] subStrings = commandWithArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (subStrings.Length <= 0)
				return false;
			command = subStrings[0];
			args = subStrings[1..];
			return true;
		}
		
		public static string GetHintForInvalidCommand(string command, IEnumerable<string> commandSrc)
		{
			return commandSrc
				.OrderBy(x => x.LevenshteinDistance(command))
				.FirstOrDefault();
		}
		
	}

	public static class LevenshteinDistanceExtensions
	{
		/// <summary>
		/// Levenshtein Distance algorithm with transposition. <br />
		/// A value of 1 or 2 is okay, 3 is iffy and greater than 4 is a poor match
		/// </summary>
		/// <param name="input"></param>
		/// <param name="comparedTo"></param>
		/// <param name="caseSensitive"></param>
		/// <returns></returns>
		public static int LevenshteinDistance(this string input, string comparedTo, bool caseSensitive = false)
		{
			if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(comparedTo)) return int.MaxValue;
			if (!caseSensitive)
			{
				input = input.ToLower();
				comparedTo = comparedTo.ToLower();
			}

			int[,] matrix = new int[Math.Min(input.Length + 1, 128), Math.Min(comparedTo.Length + 1, 128)];

			//initialize
			for (int i = 0; i <= matrix.GetUpperBound(0); i++) matrix[i, 0] = i;
			for (int i = 0; i <= matrix.GetUpperBound(1); i++) matrix[0, i] = i;

			//analyze
			for (int i = 1; i <= matrix.GetUpperBound(0); i++)
			{
				char si = input[i - 1];
				for (int j = 1; j <= matrix.GetUpperBound(1); j++)
				{
					char tj = comparedTo[j - 1];
					int cost = (si == tj) ? 0 : 1;

					int above = matrix[i - 1, j];
					int left = matrix[i, j - 1];
					int diag = matrix[i - 1, j - 1];
					int cell = FindMinimum(above + 1, left + 1, diag + cost);

					//transposition
					if (i > 1 && j > 1)
					{
						int trans = matrix[i - 2, j - 2] + 1;
						if (input[i - 2] != comparedTo[j - 1]) trans++;
						if (input[i - 1] != comparedTo[j - 2]) trans++;
						if (cell > trans) cell = trans;
					}
					matrix[i, j] = cell;
				}
			}
			return matrix[matrix.GetUpperBound(0), matrix.GetUpperBound(1)];
		}

		private static int FindMinimum(params int[] p)
		{
			return p?.Prepend(int.MaxValue).Min() ?? int.MinValue;
		}
	}
}
