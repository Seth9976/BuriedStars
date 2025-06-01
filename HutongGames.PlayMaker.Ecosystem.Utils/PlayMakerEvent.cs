using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class PlayMakerEvent
{
	public string eventName;

	public bool allowLocalEvents;

	public string defaultEventName;

	public bool isNone => string.IsNullOrEmpty(eventName);

	public PlayMakerEvent()
	{
	}

	public PlayMakerEvent(string defaultEventName)
	{
		this.defaultEventName = defaultEventName;
		eventName = defaultEventName;
	}

	public PlayMakerFSM SanitizeFsmEventSender(PlayMakerFSM fsm)
	{
		if (fsm == null)
		{
			if (PlayMakerUtils.FsmEventSender == null)
			{
				PlayMakerUtils.FsmEventSender = new GameObject("PlayMaker Send Event Proxy").AddComponent<PlayMakerFSM>();
				PlayMakerUtils.FsmEventSender.FsmName = "Send Event Proxy";
				PlayMakerUtils.FsmEventSender.FsmDescription = "This Fsm was created at runtime, because a script or component is willing to send a PlayMaker event";
			}
			return PlayMakerUtils.FsmEventSender;
		}
		return fsm;
	}

	public bool SendEvent(PlayMakerFSM fromFsm, PlayMakerTimelineEventTarget eventTarget, bool debug = false)
	{
		fromFsm = SanitizeFsmEventSender(fromFsm);
		if (debug)
		{
		}
		if (eventTarget.eventTarget == ProxyEventTarget.BroadCastAll)
		{
			PlayMakerFSM.BroadcastEvent(eventName);
		}
		else if (eventTarget.eventTarget == ProxyEventTarget.Owner || eventTarget.eventTarget == ProxyEventTarget.GameObject)
		{
			PlayMakerUtils.SendEventToGameObject(fromFsm, eventTarget.gameObject, eventName, eventTarget.includeChildren);
		}
		else if (eventTarget.eventTarget == ProxyEventTarget.FsmComponent)
		{
			eventTarget.fsmComponent.SendEvent(eventName);
		}
		return true;
	}

	public bool SendEvent(PlayMakerFSM fromFsm, PlayMakerEventTarget eventTarget, bool debug = false)
	{
		fromFsm = SanitizeFsmEventSender(fromFsm);
		if (debug)
		{
		}
		if (eventTarget.eventTarget == ProxyEventTarget.BroadCastAll)
		{
			PlayMakerFSM.BroadcastEvent(eventName);
		}
		else if (eventTarget.eventTarget == ProxyEventTarget.Owner || eventTarget.eventTarget == ProxyEventTarget.GameObject)
		{
			PlayMakerUtils.SendEventToGameObject(fromFsm, eventTarget.gameObject, eventName, eventTarget.includeChildren);
		}
		else if (eventTarget.eventTarget == ProxyEventTarget.FsmComponent)
		{
			eventTarget.fsmComponent.SendEvent(eventName);
		}
		return true;
	}

	public override string ToString()
	{
		string arg = "<color=blue>" + eventName + "</color>";
		if (isNone)
		{
			arg = "<color=red>None</color>";
		}
		return $"PlayMaker Event : {arg}";
	}
}
