using System.Runtime.InteropServices;

public class PS4LargoPluginHandler
{
	public enum OverlayImageOriginType
	{
		LeftTop = 1,
		LeftCenter,
		LeftBottom,
		CenterTop,
		CenterCenter,
		CenterBottom,
		RightTop,
		RightCenter,
		RightBottom
	}

	public static void ReleaseAll()
	{
		ReleaseLibrary_ScreenShot();
		VideoRecord_Release();
		SharePlay_Release();
		LiveStreaming_Release();
	}

	[DllImport("PS4LargoPlugin")]
	private static extern int InitLibrary_ScreenShot();

	[DllImport("PS4LargoPlugin")]
	private static extern int ReleaseLibrary_ScreenShot();

	[DllImport("PS4LargoPlugin")]
	private static extern int SetScreenShotEnable(bool enable);

	[DllImport("PS4LargoPlugin")]
	private static extern int SetScreenShotOverlayImage(string filePath, int offsetX, int offsetY);

	[DllImport("PS4LargoPlugin")]
	private static extern int SetScreenShotOverlayImageWithOrigin(string filePath, int marginX, int marginY, int originType);

	public static bool ScreenShot_Init()
	{
		int num = InitLibrary_ScreenShot();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool ScreenShot_Release()
	{
		int num = ReleaseLibrary_ScreenShot();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool ScreenShot_SetEnable(bool enable)
	{
		int num = SetScreenShotEnable(enable);
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool ScreenShot_SetOverlayImage(string filePath, int offsetX, int offsetY)
	{
		int num = SetScreenShotOverlayImage(filePath, offsetX, offsetY);
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool ScreenShot_SetOverlayImageWithOrigin(string filePath, int marginX, int marginY, OverlayImageOriginType originType)
	{
		int num = SetScreenShotOverlayImageWithOrigin(filePath, marginX, marginY, (int)originType);
		if (num < 0)
		{
		}
		return num >= 0;
	}

	[DllImport("PS4LargoPlugin")]
	private static extern int InitLibrary_VideoRecord();

	[DllImport("PS4LargoPlugin")]
	private static extern int ReleaseLibrary_VideoRecord();

	[DllImport("PS4LargoPlugin")]
	private static extern int SetVideoRecordEnable(bool enable);

	public static bool VideoRecord_Init()
	{
		int num = InitLibrary_VideoRecord();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool VideoRecord_Release()
	{
		int num = ReleaseLibrary_VideoRecord();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool VideoRecord_SetEnable(bool enable)
	{
		int num = SetVideoRecordEnable(enable);
		if (num < 0)
		{
		}
		return num >= 0;
	}

	[DllImport("PS4LargoPlugin")]
	private static extern int InitLibrary_SharePlay();

	[DllImport("PS4LargoPlugin")]
	private static extern int ReleaseLibrary_SharePlay();

	[DllImport("PS4LargoPlugin")]
	private static extern int SetSharePlayEnable(bool enable);

	public static bool SharePlay_Init()
	{
		int num = InitLibrary_SharePlay();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool SharePlay_Release()
	{
		int num = ReleaseLibrary_SharePlay();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool SharePlay_SetEnable(bool enable)
	{
		int num = SetSharePlayEnable(enable);
		if (num < 0)
		{
		}
		return num >= 0;
	}

	[DllImport("PS4LargoPlugin")]
	private static extern int InitLibrary_LiveStreaming();

	[DllImport("PS4LargoPlugin")]
	private static extern int ReleaseLibrary_LiveStreaming();

	[DllImport("PS4LargoPlugin")]
	private static extern int SetLiveStreamingEnable(bool enable);

	public static bool LiveStreaming_Init()
	{
		int num = InitLibrary_LiveStreaming();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool LiveStreaming_Release()
	{
		int num = ReleaseLibrary_LiveStreaming();
		if (num < 0)
		{
		}
		return num >= 0;
	}

	public static bool LiveStreaming_SetEnable(bool enable)
	{
		int num = SetLiveStreamingEnable(enable);
		if (num < 0)
		{
		}
		return num >= 0;
	}
}
