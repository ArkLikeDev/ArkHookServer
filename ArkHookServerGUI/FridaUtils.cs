using System.Linq;

namespace ArkLike.HookServer.GUI
{
	internal static class FridaUtils
	{
		public static string GetFridaDeviceListAsString()
		{
			return FridaSharp.FridaUtils.GetDeviceList();
		}
	}
}
