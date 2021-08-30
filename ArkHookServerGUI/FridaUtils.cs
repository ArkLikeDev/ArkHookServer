namespace ArkLike.HookServer.Launcher
{
	internal static class FridaUtils
	{
		public static string GetFridaDeviceListAsString()
		{
			return FridaSharp.FridaUtils.GetDeviceList();
		}
	}
}
