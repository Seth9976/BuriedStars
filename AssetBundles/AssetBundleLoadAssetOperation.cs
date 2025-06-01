using UnityEngine;

namespace AssetBundles;

public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
	public abstract T GetAsset<T>() where T : Object;

	public abstract T GetAsset<T>(string assetName) where T : Object;
}
