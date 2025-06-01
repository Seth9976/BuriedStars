using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundles;

public class AssetBundleManager : MonoBehaviour
{
	public enum LogMode
	{
		All,
		JustErrors
	}

	public enum LogType
	{
		Info,
		Warning,
		Error
	}

	private static LogMode m_LogMode = LogMode.All;

	private static string m_BaseDownloadingURL = string.Empty;

	private static string[] m_ActiveVariants = new string[0];

	private static AssetBundleManifest m_AssetBundleManifest = null;

	private static SortedDictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new SortedDictionary<string, LoadedAssetBundle>();

	private static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();

	private static SortedDictionary<string, string[]> m_Dependencies = new SortedDictionary<string, string[]>();

	public static LogMode logMode
	{
		get
		{
			return m_LogMode;
		}
		set
		{
			m_LogMode = value;
		}
	}

	public static string BaseDownloadingURL
	{
		get
		{
			return m_BaseDownloadingURL;
		}
		set
		{
			m_BaseDownloadingURL = value;
		}
	}

	public static string[] ActiveVariants
	{
		get
		{
			return m_ActiveVariants;
		}
		set
		{
			m_ActiveVariants = value;
		}
	}

	public static AssetBundleManifest AssetBundleManifestObject
	{
		get
		{
			return m_AssetBundleManifest;
		}
		set
		{
			m_AssetBundleManifest = value;
		}
	}

	private static void Log(LogType logType, string text)
	{
		if (logType != LogType.Error && m_LogMode != LogMode.All)
		{
		}
	}

	private static string GetStreamingAssetsPath()
	{
		return Application.streamingAssetsPath;
	}

	public static void SetSourceAssetBundleDirectory(string relativePath)
	{
		BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
	}

	public static void SetSourceAssetBundleURL(string absolutePath)
	{
		BaseDownloadingURL = absolutePath + Utility.GetPlatformName() + "/";
	}

	private static string GetLowerCaseAssetBundleName(string assetBundlePath)
	{
		return assetBundlePath.ToLower();
	}

	public static void SetDevelopmentAssetBundleServer()
	{
		TextAsset textAsset = Resources.Load("AssetBundleServerURL") as TextAsset;
		string text = ((!(textAsset != null)) ? null : textAsset.text.Trim());
		if (text != null && text.Length != 0)
		{
			SetSourceAssetBundleURL(text);
		}
	}

	public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
	{
		error = string.Empty;
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value == null)
		{
			return null;
		}
		string[] value2 = null;
		if (!m_Dependencies.TryGetValue(assetBundleName, out value2))
		{
			return value;
		}
		string[] array = value2;
		foreach (string key in array)
		{
			m_LoadedAssetBundles.TryGetValue(key, out var value3);
			if (value3 == null)
			{
				return null;
			}
		}
		return value;
	}

	public static AssetBundleLoadManifestOperation Initialize()
	{
		return Initialize(Utility.GetPlatformName());
	}

	public static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
	{
		GameObject target = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
		UnityEngine.Object.DontDestroyOnLoad(target);
		LoadAssetBundle(manifestAssetBundleName, isLoadingAssetBundleManifest: true);
		AssetBundleLoadManifestOperation assetBundleLoadManifestOperation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
		m_InProgressOperations.Add(assetBundleLoadManifestOperation);
		return assetBundleLoadManifestOperation;
	}

	protected static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
	{
		Log(LogType.Info, "Loading Asset Bundle " + ((!isLoadingAssetBundleManifest) ? ": " : "Manifest: ") + assetBundleName);
		if ((isLoadingAssetBundleManifest || !(m_AssetBundleManifest == null)) && !LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest) && !isLoadingAssetBundleManifest)
		{
			LoadDependencies(assetBundleName);
		}
	}

	protected static string RemapVariantName(string assetBundleName)
	{
		string[] allAssetBundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();
		string[] array = assetBundleName.Split('.');
		int num = int.MaxValue;
		int num2 = -1;
		for (int i = 0; i < allAssetBundlesWithVariant.Length; i++)
		{
			string[] array2 = allAssetBundlesWithVariant[i].Split('.');
			if (!(array2[0] != array[0]))
			{
				int num3 = Array.IndexOf(m_ActiveVariants, array2[1]);
				if (num3 == -1)
				{
					num3 = 2147483646;
				}
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		if (num == 2147483646)
		{
		}
		if (num2 != -1)
		{
			return allAssetBundlesWithVariant[num2];
		}
		return assetBundleName;
	}

	protected static bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
	{
		LoadedAssetBundle value = null;
		m_LoadedAssetBundles.TryGetValue(assetBundleName, out value);
		if (value != null)
		{
			value.m_ReferencedCount++;
			Log(LogType.Info, $"RefCount UP - '{assetBundleName} : {value.m_ReferencedCount}' ");
			return true;
		}
		string path = m_BaseDownloadingURL + assetBundleName;
		AssetBundle assetBundle = (string.IsNullOrEmpty(assetBundleName) ? null : AssetBundle.LoadFromFile(path));
		m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle));
		Log(LogType.Info, $"Loaded Success!! - '{assetBundleName}'");
		return false;
	}

	protected static void LoadDependencies(string assetBundleName)
	{
		if (m_AssetBundleManifest == null)
		{
			return;
		}
		string[] allDependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
		if (allDependencies.Length != 0)
		{
			Log(LogType.Info, "LoadDependency START - " + assetBundleName);
			for (int i = 0; i < allDependencies.Length; i++)
			{
				allDependencies[i] = RemapVariantName(allDependencies[i]);
			}
			m_Dependencies.Add(assetBundleName, allDependencies);
			for (int j = 0; j < allDependencies.Length; j++)
			{
				LoadAssetBundleInternal(allDependencies[j], isLoadingAssetBundleManifest: false);
			}
			Log(LogType.Info, "LoadDependency END - " + assetBundleName);
		}
	}

	public static void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedAssets = false)
	{
		assetBundleName = assetBundleName.ToLower();
		UnloadAssetBundleInternal(assetBundleName, unloadAllLoadedAssets);
		UnloadDependencies(assetBundleName, unloadAllLoadedAssets);
	}

	public static void UnloadAssetBundle(string assetBundleName, bool isOneFileBundle, bool unloadAllLoadedAssets = false)
	{
		string assetName = string.Empty;
		SeperateAssetNameFromAssetBundleName(ref assetBundleName, out assetName, isOneFileBundle);
		UnloadAssetBundle(assetBundleName);
	}

	protected static void UnloadDependencies(string assetBundleName, bool unloadAllLoadedAssets)
	{
		string[] value = null;
		if (m_Dependencies.TryGetValue(assetBundleName, out value))
		{
			string[] array = value;
			foreach (string assetBundleName2 in array)
			{
				UnloadAssetBundleInternal(assetBundleName2, unloadAllLoadedAssets);
			}
			m_Dependencies.Remove(assetBundleName);
		}
	}

	protected static void UnloadAssetBundleInternal(string assetBundleName, bool unloadAllLoadedAssets)
	{
		LoadedAssetBundle loadedAssetBundle = GetLoadedAssetBundle(assetBundleName, out var _);
		if (loadedAssetBundle != null)
		{
			loadedAssetBundle.m_ReferencedCount--;
			if (loadedAssetBundle.m_ReferencedCount <= 0)
			{
				Log(LogType.Info, $"Unloaded Success!! - '{assetBundleName}'");
				loadedAssetBundle.m_AssetBundle.Unload(unloadAllLoadedAssets);
				m_LoadedAssetBundles.Remove(assetBundleName);
				loadedAssetBundle.m_AssetBundle = null;
				loadedAssetBundle = null;
			}
			else
			{
				Log(LogType.Info, $"RefCount DOWN - '{assetBundleName} : {loadedAssetBundle.m_ReferencedCount}'");
			}
		}
	}

	private void Update()
	{
		int num = m_InProgressOperations.Count;
		while (num > 0)
		{
			num--;
			if (!m_InProgressOperations[num].Update())
			{
				m_InProgressOperations.RemoveAt(num);
			}
		}
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, Type type, bool isOneFileBundle = true)
	{
		string assetName = string.Empty;
		SeperateAssetNameFromAssetBundleName(ref assetBundleName, out assetName, isOneFileBundle);
		return LoadAssetAsync(assetBundleName, assetName, type);
	}

	public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, Type type)
	{
		Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");
		assetBundleName = GetLowerCaseAssetBundleName(assetBundleName);
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = null;
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName);
		assetBundleLoadAssetOperation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);
		m_InProgressOperations.Add(assetBundleLoadAssetOperation);
		return assetBundleLoadAssetOperation;
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, bool isAdditive)
	{
		string assetName = string.Empty;
		SeperateAssetNameFromAssetBundleName(ref assetBundleName, out assetName, isOneFileBundle: true);
		return LoadLevelAsync(assetBundleName, assetName, isAdditive);
	}

	public static AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
	{
		Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
		AssetBundleLoadOperation assetBundleLoadOperation = null;
		assetBundleName = RemapVariantName(assetBundleName);
		LoadAssetBundle(assetBundleName);
		assetBundleLoadOperation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);
		m_InProgressOperations.Add(assetBundleLoadOperation);
		return assetBundleLoadOperation;
	}

	public static void SeperateAssetNameFromAssetBundleName(ref string assetBundleName, out string assetName, bool isOneFileBundle)
	{
		assetName = string.Empty;
		if (string.IsNullOrEmpty(assetBundleName))
		{
			return;
		}
		char[] anyOf = new char[2] { '/', '\\' };
		int num = assetBundleName.LastIndexOfAny(anyOf);
		if (num > 0)
		{
			assetName = assetBundleName.Substring(num + 1);
			if (!isOneFileBundle)
			{
				assetBundleName = assetBundleName.Substring(0, num);
			}
		}
		else
		{
			assetName = assetBundleName.Clone() as string;
		}
		assetBundleName = assetBundleName.ToLower();
	}

	public static void DebugPrint_LoadedAssetBundleCount()
	{
	}
}
