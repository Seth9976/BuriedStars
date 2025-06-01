using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

public class TransformEventsBridge : MonoBehaviour
{
	public PlayMakerEventTarget eventTarget = new PlayMakerEventTarget(includeChildren: false);

	[EventTargetVariable("eventTarget")]
	public PlayMakerEvent parentChangedEvent;

	[EventTargetVariable("eventTarget")]
	public PlayMakerEvent childrenChangedEvent;

	public bool debug;

	private void OnTransformParentChanged()
	{
		if (debug)
		{
		}
		parentChangedEvent.SendEvent(null, eventTarget);
	}

	private void OnTransformChildrenChanged()
	{
		if (debug)
		{
		}
		childrenChangedEvent.SendEvent(null, eventTarget);
	}
}
