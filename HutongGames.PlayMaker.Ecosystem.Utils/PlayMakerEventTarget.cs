using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class PlayMakerEventTarget
{
	public ProxyEventTarget eventTarget;

	public GameObject gameObject;

	public bool includeChildren = true;

	public PlayMakerFSM fsmComponent;

	public PlayMakerEventTarget()
	{
	}

	public PlayMakerEventTarget(bool includeChildren = true)
	{
		this.includeChildren = includeChildren;
	}

	public PlayMakerEventTarget(ProxyEventTarget evenTarget, bool includeChildren = true)
	{
		eventTarget = evenTarget;
		this.includeChildren = includeChildren;
	}

	public void SetOwner(GameObject owner)
	{
		if (eventTarget == ProxyEventTarget.Owner)
		{
			gameObject = owner;
		}
	}

	public override string ToString()
	{
		string text = eventTarget.ToString();
		if (eventTarget == ProxyEventTarget.GameObject)
		{
			text = text + " : " + gameObject.name;
		}
		if (eventTarget == ProxyEventTarget.GameObject || eventTarget == ProxyEventTarget.Owner)
		{
			text += ((!includeChildren) ? ", not " : ", ");
			text += "including children";
		}
		return text;
	}
}
