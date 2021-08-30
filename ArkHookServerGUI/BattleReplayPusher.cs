using System.Net;

namespace ArkLike.HookServer.Launcher
{
	internal static class BattleReplayPusher
	{
		public static bool IsListening;
		public static IPEndPoint EndPoint;

		public static void Open(string connectStr)
		{

		}

		public static void Open(int port)
		{
			Open($"tcp://127.0.0.1:{port}");
		}

		public static void Close()
		{

		}

		public static void SendFrame(string data)
		{

		}
	}
}
