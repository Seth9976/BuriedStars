using System.Collections;
using UnityEngine;

public class LoadAssetHandler
{
	private string m_assetPath = string.Empty;

	private Object m_loadedAsset;

	public LoadAssetHandler(string _assetPath)
	{
		m_assetPath = _assetPath;
	}

	public IEnumerator LoadAssetAsync()
	{
		m_loadedAsset = null;
		if (string.IsNullOrEmpty(m_assetPath))
		{
			yield break;
		}
		ResourceRequest request = Resources.LoadAsync<Object>(m_assetPath);
		if (request != null)
		{
			while (!request.isDone)
			{
				yield return null;
			}
			m_loadedAsset = request.asset;
		}
	}

	public IEnumerator LoadAssetAsync<T>() where T : Object
	{
		m_loadedAsset = null;
		if (string.IsNullOrEmpty(m_assetPath))
		{
			yield break;
		}
		ResourceRequest request = Resources.LoadAsync<T>(m_assetPath);
		if (request != null)
		{
			while (!request.isDone)
			{
				yield return null;
			}
			m_loadedAsset = request.asset;
		}
	}

	public Object LoadAsset()
	{
		m_loadedAsset = null;
		if (string.IsNullOrEmpty(m_assetPath))
		{
			return null;
		}
		m_loadedAsset = Resources.Load<Object>(m_assetPath);
		return m_loadedAsset;
	}

	public void UnloadAsset()
	{
		if (!(m_loadedAsset == null))
		{
			if (!(m_loadedAsset is GameObject) && !(m_loadedAsset is Component))
			{
				Resources.UnloadAsset(m_loadedAsset);
			}
			m_loadedAsset = null;
		}
	}

	public T GetLoadedAsset<T>() where T : class
	{
		return (!(m_loadedAsset != null)) ? ((T)null) : (m_loadedAsset as T);
	}

	public Object GetLoadedAssetObject()
	{
		return m_loadedAsset;
	}

	public GameObject GetLoadedAsset_ToGameObject()
	{
		return GetLoadedAsset<GameObject>();
	}

	public Sprite GetLoadedAsset_ToSprite()
	{
		if (m_loadedAsset == null)
		{
			return null;
		}
		if (m_loadedAsset is Sprite)
		{
			return m_loadedAsset as Sprite;
		}
		if (m_loadedAsset is Texture2D)
		{
			Texture2D texture2D = m_loadedAsset as Texture2D;
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
