using System.Linq;

namespace ArkLike.HookServer.GUI
{
	internal static class FridaUtils
	{
		public static string GetFridaDeviceListAsString()
		{
			return FridaSharp.FridaUtils.DeviceManager.EnumerateDevices()
				.Aggregate("", (s, device) => string.Concat(s, device.ToString(), "\r\n"));
		}
	}
}
