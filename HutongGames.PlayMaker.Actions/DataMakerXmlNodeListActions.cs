using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

public abstract class DataMakerXmlNodeListActions : FsmStateAction
{
	internal DataMakerXmlNodeListProxy proxy;

	protected bool SetUpDataMakerXmlNodeListProxyPointer(GameObject aProxyGO, string nameReference)
	{
		if (aProxyGO == null)
		{
			return false;
		}
		proxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlNodeListProxy), aProxyGO, nameReference, silent: false) as DataMakerXmlNodeListProxy;
		return proxy != null;
	}

	public bool isProxyValid()
	{
		if (proxy == null)
		{
			LogError("DataMaker Xml Node List proxy is null");
			return false;
		}
		return true;
	}
}
