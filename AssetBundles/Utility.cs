using UnityEngine;

namespace AssetBundles;

public class Utility
{
	public const string AssetBundlesOutputPath = "Assets/StreamingAssets/AssetBundles";

	public static string GetPlatformName()
	{
		return GetPlatformForAssetBundles(Application.platform);
	}

	private static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		return platform switch
		{
			RuntimePlatform.Android => "Android", 
			RuntimePlatform.IPhonePlayer => "iOS", 
			RuntimePlatform.WebGLPlayer => "WebGL", 
			RuntimePlatform.WindowsPlayer => "Windows", 
			RuntimePlatform.OSXPlayer => "OSX", 
			RuntimePlatform.PS4 => "PS4", 
			RuntimePlatform.PSP2 => "PSVita", 
			RuntimePlatform.Switch => "Switch", 
			_ => null, 
		};
	}
}
