using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

public abstract class DataMakerXmlNodeActions : FsmStateAction
{
	internal DataMakerXmlNodeProxy proxy;

	protected bool SetUpDataMakerXmlNodeProxyPointer(GameObject aProxyGO, string nameReference)
	{
		if (aProxyGO == null)
		{
			return false;
		}
		proxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlNodeProxy), aProxyGO, nameReference, silent: false) as DataMakerXmlNodeProxy;
		return proxy != null;
	}

	public bool isProxyValid()
	{
		if (proxy == null)
		{
			LogError("DataMaker Xml Node proxy is null");
			return false;
		}
		return true;
	}
}
