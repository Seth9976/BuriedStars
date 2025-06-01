using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;

public class ContentThumbnailManager
{
	private string m_thumbnailBundleName = string.Empty;

	private AssetBundle m_thumbnailBundle;

	private SortedDictionary<string, Sprite> m_thumbnailCaches = new SortedDictionary<string, Sprite>();

	public AssetBundle thumbnailBundle => m_thumbnailBundle;

	public SortedDictionary<string, Sprite> thumbnailCaches => m_thumbnailCaches;

	public ContentThumbnailManager(string bundleName)
	{
		m_thumbnailBundleName = ((!string.IsNullOrEmpty(bundleName)) ? bundleName.ToLower() : string.Empty);
	}

	public IEnumerator LoadThumbnailBundle()
	{
		if (m_thumbnailBundle != null)
		{
			yield break;
		}
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(m_thumbnailBundleName, string.Empty, typeof(Object));
		if (request != null)
		{
			while (!request.IsDone())
			{
				yield return null;
			}
			yield return null;
			yield return null;
			string errorMsg = string.Empty;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_thumbnailBundleName, out errorMsg);
			if (loadedAssetBundle != null && !(loadedAssetBundle.m_AssetBundle == null))
			{
				m_thumbnailBundle = loadedAssetBundle.m_AssetBundle;
			}
		}
	}

	public void UnloadThumbnailBundle()
	{
		if (!(m_thumbnailBundle == null))
		{
			AssetBundleManager.UnloadAssetBundle(m_thumbnailBundleName);
			m_thumbnailBundle = null;
		}
	}

	private string GetAssetNameByPath(string path)
	{
		char[] anyOf = new char[2] { '/', '\\' };
		int num = path.LastIndexOfAny(anyOf);
		string text = ((num < 0) ? path : path.Substring(num + 1));
		num = text.LastIndexOf('.');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		return text;
	}

	private IEnumerator LoadAsset(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			yield break;
		}
		char[] seperatores = new char[2] { '/', '\\' };
		int lastIndex = path.LastIndexOfAny(seperatores);
		string assetName = GetAssetNameByPath(path);
		if (string.IsNullOrEmpty(assetName) || m_thumbnailCaches.ContainsKey(assetName))
		{
			yield break;
		}
		Sprite loadedSprite = null;
		if (m_thumbnailBundle == null)
		{
			string error = string.Empty;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_thumbnailBundleName, out error);
			if (loadedAssetBundle == null || loadedAssetBundle.m_AssetBundle == null)
			{
				yield break;
			}
			m_thumbnailBundle = loadedAssetBundle.m_AssetBundle;
			if (m_thumbnailBundle == null)
			{
				yield break;
			}
		}
		AssetBundleRequest request = m_thumbnailBundle.LoadAssetAsync(assetName);
		if (request == null)
		{
			yield break;
		}
		while (!request.isDone)
		{
			yield return null;
		}
		if (!(request.asset == null))
		{
			if (request.asset is Sprite)
			{
				loadedSprite = request.asset as Sprite;
			}
			else if (request.asset is Texture2D)
			{
				Texture2D texture2D = request.asset as Texture2D;
				Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
				loadedSprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
			}
			if (loadedSprite != null && !m_thumbnailCaches.ContainsKey(assetName))
			{
				m_thumbnailCaches.Add(assetName, loadedSprite);
			}
		}
	}

	public IEnumerator LoadAssetsAll(MonoBehaviour coroutineMgr = null, GameDefine.EventProc fpFinished = null)
	{
		ClearThumbnailCaches();
		if (coroutineMgr == null)
		{
			coroutineMgr = MainLoadThing.instance;
			if (coroutineMgr == null)
			{
				goto IL_016a;
			}
		}
		yield return coroutineMgr.StartCoroutine(LoadThumbnailBundle());
		string[] assetNames = null;
		if (!(m_thumbnailBundle == null))
		{
			assetNames = m_thumbnailBundle.GetAllAssetNames();
			if (assetNames != null && assetNames.Length > 0)
			{
				string[] array = assetNames;
				foreach (string assetName in array)
				{
					yield return coroutineMgr.StartCoroutine(LoadAsset(assetName));
				}
			}
		}
		goto IL_016a;
		IL_016a:
		UnloadThumbnailBundle();
		fpFinished?.Invoke(this, null);
	}

	public IEnumerator LoadAssets_FormList(string[] assetPathes, MonoBehaviour coroutineMgr = null, bool isClearCache = true, GameDefine.EventProc fpFinished = null)
	{
		if (isClearCache)
		{
			ClearThumbnailCaches();
		}
		if (coroutineMgr == null)
		{
			coroutineMgr = MainLoadThing.instance;
			if (coroutineMgr == null)
			{
				goto IL_013d;
			}
		}
		if (assetPathes != null && assetPathes.Length > 0)
		{
			yield return coroutineMgr.StartCoroutine(LoadThumbnailBundle());
			foreach (string assetPath in assetPathes)
			{
				yield return coroutineMgr.StartCoroutine(LoadAsset(assetPath));
			}
		}
		goto IL_013d;
		IL_013d:
		UnloadThumbnailBundle();
		fpFinished?.Invoke(this, null);
	}

	public void ClearThumbnailCaches()
	{
		m_thumbnailCaches.Clear();
	}

	public Sprite GetThumbnailImageInCache(string imagePath)
	{
		string key = GetAssetNameByPath(imagePath).ToLower();
		return (!m_thumbnailCaches.ContainsKey(key)) ? null : m_thumbnailCaches[key];
	}
}
