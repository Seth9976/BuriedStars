using System;
using System.Collections;
using AssetBundles;
using UnityEngine;

public class AssetBundleObjectHandler : IDisposable
{
	private UnityEngine.Object m_loadedAssetBundleObject;

	private string m_assetBundleName = string.Empty;

	private string m_assetName = string.Empty;

	public string assetBundleName => m_assetBundleName;

	public string assetName => m_assetName;

	public UnityEngine.Object loadedAssetBundleObject => m_loadedAssetBundleObject;

	public AssetBundleObjectHandler()
	{
	}

	public AssetBundleObjectHandler(string _assetBundleName, bool isOneFileBundle = true)
	{
		SetAssetBundleInfo(_assetBundleName, isOneFileBundle);
	}

	public void Dispose()
	{
		UnloadAssetBundle();
	}

	public void SetAssetBundleInfo(string _assetBundleName, bool isOneFileBundle = true)
	{
		if (!string.Equals(m_assetBundleName, _assetBundleName))
		{
			UnloadAssetBundle();
		}
		string empty = string.Empty;
		char[] anyOf = new char[2] { '/', '\\' };
		int num = _assetBundleName.LastIndexOfAny(anyOf);
		if (!isOneFileBundle && num >= 0)
		{
			empty = _assetBundleName.Substring(num + 1);
			_assetBundleName = _assetBundleName.Substring(0, num + 1);
		}
		else
		{
			empty = _assetBundleName.Substring(num + 1);
		}
		m_assetBundleName = _assetBundleName.ToLower();
		m_assetName = empty;
	}

	public IEnumerator LoadAssetBundle(GameDefine.EventProc fpLoadComplete = null)
	{
		m_loadedAssetBundleObject = null;
		if (!string.IsNullOrEmpty(m_assetBundleName))
		{
			string errorString = string.Empty;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_assetBundleName, out errorString);
			if (loadedAssetBundle != null && loadedAssetBundle.m_AssetBundle != null)
			{
				AssetBundleRequest request = loadedAssetBundle.m_AssetBundle.LoadAssetAsync(m_assetName);
				if (request != null)
				{
					loadedAssetBundle.m_ReferencedCount++;
					while (!request.isDone)
					{
						yield return null;
					}
					m_loadedAssetBundleObject = request.asset;
				}
			}
			else
			{
				AssetBundleLoadAssetOperation request2 = AssetBundleManager.LoadAssetAsync(m_assetBundleName, typeof(UnityEngine.Object));
				if (request2 != null)
				{
					while (!request2.IsDone())
					{
						yield return null;
					}
					m_loadedAssetBundleObject = request2.GetAsset<UnityEngine.Object>();
				}
			}
		}
		fpLoadComplete?.Invoke(null, null);
	}

	public void UnloadAssetBundle(bool isUnloadLoadedAllAssets = false)
	{
		if (!string.IsNullOrEmpty(m_assetBundleName))
		{
			m_loadedAssetBundleObject = null;
			AssetBundleManager.UnloadAssetBundle(m_assetBundleName, isUnloadLoadedAllAssets);
			m_assetBundleName = null;
			m_assetName = null;
		}
	}

	public T GetLoadedAsset<T>() where T : class
	{
		return (!(m_loadedAssetBundleObject != null)) ? ((T)null) : (m_loadedAssetBundleObject as T);
	}

	public UnityEngine.Object GetLoadedAssetObject()
	{
		return m_loadedAssetBundleObject;
	}

	public GameObject GetLoadedAsset_ToGameObject()
	{
		return GetLoadedAsset<GameObject>();
	}

	public Sprite GetLoadedAsset_ToSprite()
	{
		if (m_loadedAssetBundleObject == null)
		{
			return null;
		}
		if (m_loadedAssetBundleObject is Sprite)
		{
			return m_loadedAssetBundleObject as Sprite;
		}
		if (m_loadedAssetBundleObject is Texture2D)
		{
			Texture2D texture2D = m_loadedAssetBundleObject as Texture2D;
			if (texture2D == null)
			{
				return null;
			}
			Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
			return Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
		}
		return null;
	}
}
