using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class PlayMakerTimelineEventTarget
{
	public ProxyEventTarget eventTarget;

	public GameObject gameObject;

	public ExposedReference<GameObject> GameObject;

	public bool includeChildren = true;

	public PlayMakerFSM fsmComponent;

	public ExposedReference<PlayMakerFSM> FsmComponent;

	public PlayMakerTimelineEventTarget()
	{
	}

	public PlayMakerTimelineEventTarget(bool includeChildren = true)
	{
		this.includeChildren = includeChildren;
	}

	public PlayMakerTimelineEventTarget(ProxyEventTarget evenTarget, bool includeChildren = true)
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

	public void Resolve(IExposedPropertyTable resolver)
	{
		if (eventTarget == ProxyEventTarget.GameObject)
		{
			gameObject = GameObject.Resolve(resolver);
		}
		else if (eventTarget == ProxyEventTarget.FsmComponent)
		{
			fsmComponent = FsmComponent.Resolve(resolver);
		}
	}

	public override string ToString()
	{
		string text = eventTarget.ToString();
		if (gameObject == null)
		{
			text += " : <color=red>unresolved GameObject</color>";
		}
		if (gameObject != null && eventTarget == ProxyEventTarget.GameObject)
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
