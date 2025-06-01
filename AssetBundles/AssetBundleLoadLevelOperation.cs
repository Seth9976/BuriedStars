using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetBundles;

public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
	protected string m_AssetBundleName;

	protected string m_LevelName;

	protected bool m_IsAdditive;

	protected string m_DownloadingError;

	protected AsyncOperation m_Request;

	public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
	{
		m_AssetBundleName = assetbundleName;
		m_LevelName = levelName;
		m_IsAdditive = isAdditive;
	}

	public override bool Update()
	{
		if (m_Request != null)
		{
			return false;
		}
		LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
		if (loadedAssetBundle != null)
		{
			m_Request = SceneManager.LoadSceneAsync(m_LevelName, m_IsAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
			return false;
		}
		return true;
	}

	public override bool IsDone()
	{
		if (m_Request == null && m_DownloadingError != null)
		{
			return true;
		}
		return m_Request != null && m_Request.isDone;
	}
}
