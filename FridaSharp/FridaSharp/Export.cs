using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Frida;

namespace FridaSharp
{
	public static class FridaUtils
	{
		public static readonly DeviceManager DeviceManager = new DeviceManager(Dispatcher.CurrentDispatcher);

		public static Device[] Devices
		{
			get => DeviceManager.EnumerateDevices();
		}

		public static Session AttachWithSpawn(Device device, string program, string[] argv, string[] envp, string[] env, string cwd)
		{
			uint pid = device.Spawn(program, argv, envp, env, cwd);
			return device.Attach(pid);
		}

		public static Session Attach(Device device, uint pid)
		{
			return device.Attach(pid);
		}
	}
}
