using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[assembly: AssemblyProduct("Chroma")]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: TargetFramework(".NETStandard,Version=v2.0", FrameworkDisplayName = "")]
[assembly: AssemblyDescription("Wrapper for native RzChromaBroadcastAPI")]
[assembly: CompilationRelaxations(8)]
[assembly: AssemblyCompany("Razer")]
[assembly: AssemblyCopyright("Razer Inc.")]
[assembly: AssemblyTitle("RzChromaBroadcastAPI.NET")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace Razer.Chroma.Broadcast;

public class RzChromaBroadcastAPI : IDisposable
{
	private const int maxColors = 5;

	private RzChromaBroadcastAPINative.RegisterEventNotificationCallback notificationCallback;

	public event EventHandler<RzChromaBroadcastColorChangedEventArgs> ColorChanged;

	public event EventHandler<RzChromaBroadcastConnectionChangedEventArgs> ConnectionChanged;

	public RzResult Init(Guid guid)
	{
		BigInteger bigInteger = new BigInteger(guid.ToByteArray());
		uint a = (uint)((bigInteger >> 96) & uint.MaxValue);
		uint b = (uint)((bigInteger >> 64) & uint.MaxValue);
		uint c = (uint)((bigInteger >> 32) & uint.MaxValue);
		uint d = (uint)((bigInteger >> 0) & uint.MaxValue);
		RzResult rzResult = RzChromaBroadcastAPINative.Init(a, b, c, d);
		if (rzResult == RzResult.SUCCESS)
		{
			notificationCallback = EventNotificationCallback;
			rzResult = RzChromaBroadcastAPINative.RegisterEventNotification(notificationCallback);
		}
		return rzResult;
	}

	public RzResult UnInit()
	{
		return RzChromaBroadcastAPINative.UnInit();
	}

	private int EventNotificationCallback(int message, IntPtr data)
	{
		switch (message)
		{
		case 1:
			if (data != IntPtr.Zero)
			{
				Color[] array = new Color[5];
				int[] array2 = new int[5];
				Marshal.Copy(data, array2, 0, 5);
				for (int i = 0; i < array2.Length; i++)
				{
					int red = (byte)(array2[i] & 0xFF);
					int green = (byte)((array2[i] >> 8) & 0xFF);
					int blue = (byte)((array2[i] >> 16) & 0xFF);
					array[i] = Color.FromArgb(red, green, blue);
				}
				this.ColorChanged?.Invoke(this, new RzChromaBroadcastColorChangedEventArgs(array));
			}
			break;
		case 2:
			this.ConnectionChanged?.Invoke(this, new RzChromaBroadcastConnectionChangedEventArgs(data.ToInt32() == 1));
			break;
		}
		return 0;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		RzChromaBroadcastAPINative.UnRegisterEventNotification();
		RzChromaBroadcastAPINative.UnInit();
	}
}
internal class RzChromaBroadcastAPINative
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int RegisterEventNotificationCallback(int message, IntPtr data);

	[DllImport("RzChromaBroadcastAPI.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern RzResult Init(uint a, uint b, uint c, uint d);

	[DllImport("RzChromaBroadcastAPI.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern RzResult UnInit();

	[DllImport("RzChromaBroadcastAPI.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern RzResult RegisterEventNotification(RegisterEventNotificationCallback callback);

	[DllImport("RzChromaBroadcastAPI.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern RzResult UnRegisterEventNotification();
}
public class RzChromaBroadcastColorChangedEventArgs
{
	public Color[] Colors { get; }

	public RzChromaBroadcastColorChangedEventArgs(Color[] colors)
	{
		Colors = colors;
	}
}
public class RzChromaBroadcastConnectionChangedEventArgs
{
	public bool Connected { get; }

	public RzChromaBroadcastConnectionChangedEventArgs(bool connected)
	{
		Connected = connected;
	}
}
public enum RzResult
{
	INVALID = -1,
	SUCCESS = 0,
	ACCESS_DENIED = 5,
	INVALID_HANDLE = 6,
	NOT_SUPPORTED = 50,
	INVALID_PARAMETER = 87,
	SERVICE_NOT_ACTIVE = 1062,
	SINGLE_INSTANCE_APP = 1152,
	DEVICE_NOT_CONNECTED = 1167,
	NOT_FOUND = 1168,
	REQUEST_ABORTED = 1235,
	ALREADY_INITIALIZED = 1247,
	RESOURCE_DISABLED = 4309,
	DEVICE_NOT_AVAILABLE = 4319,
	NOT_VALID_STATE = 5023,
	NO_MORE_ITEMS = 259,
	FAILED = int.MaxValue
}
