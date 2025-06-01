using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[Tooltip("Collections base action - don't use!")]
public abstract class CollectionsActions : FsmStateAction
{
	public enum FsmVariableEnum
	{
		FsmGameObject,
		FsmInt,
		FsmFloat,
		FsmString,
		FsmBool,
		FsmVector2,
		FsmVector3,
		FsmRect,
		FsmQuaternion,
		FsmColor,
		FsmMaterial,
		FsmTexture,
		FsmObject
	}

	protected PlayMakerHashTableProxy GetHashTableProxyPointer(GameObject aProxy, string nameReference, bool silent)
	{
		if (aProxy == null)
		{
			if (!silent)
			{
			}
			return null;
		}
		PlayMakerHashTableProxy[] components = aProxy.GetComponents<PlayMakerHashTableProxy>();
		if (components.Length > 1)
		{
			if (!(nameReference == string.Empty) || !silent)
			{
			}
			PlayMakerHashTableProxy[] array = components;
			foreach (PlayMakerHashTableProxy playMakerHashTableProxy in array)
			{
				if (playMakerHashTableProxy.referenceName == nameReference)
				{
					return playMakerHashTableProxy;
				}
			}
			if (nameReference != string.Empty)
			{
				if (!silent)
				{
				}
				return null;
			}
		}
		else if (components.Length > 0)
		{
			if (nameReference != string.Empty && nameReference != components[0].referenceName)
			{
				if (!silent)
				{
				}
				return null;
			}
			return components[0];
		}
		if (!silent)
		{
		}
		return null;
	}

	protected PlayMakerArrayListProxy GetArrayListProxyPointer(GameObject aProxy, string nameReference, bool silent)
	{
		if (aProxy == null)
		{
			if (!silent)
			{
			}
			return null;
		}
		PlayMakerArrayListProxy[] components = aProxy.GetComponents<PlayMakerArrayListProxy>();
		if (components.Length > 1)
		{
			if (!(nameReference == string.Empty) || !silent)
			{
			}
			PlayMakerArrayListProxy[] array = components;
			foreach (PlayMakerArrayListProxy playMakerArrayListProxy in array)
			{
				if (playMakerArrayListProxy.referenceName == nameReference)
				{
					return playMakerArrayListProxy;
				}
			}
			if (nameReference != string.Empty)
			{
				if (!silent)
				{
					LogError("ArrayList Proxy not found for reference <" + nameReference + ">");
				}
				return null;
			}
		}
		else if (components.Length > 0)
		{
			if (nameReference != string.Empty && nameReference != components[0].referenceName)
			{
				if (!silent)
				{
				}
				return null;
			}
			return components[0];
		}
		if (!silent)
		{
			LogError("ArrayList proxy not found");
		}
		return null;
	}
}
