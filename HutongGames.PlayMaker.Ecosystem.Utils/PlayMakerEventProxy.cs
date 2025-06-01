using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

public class PlayMakerEventProxy : MonoBehaviour
{
	public PlayMakerEventTarget eventTarget = new PlayMakerEventTarget(includeChildren: false);

	[EventTargetVariable("eventTarget")]
	public PlayMakerEvent fsmEvent;

	public bool debug;

	protected void SendPlayMakerEvent()
	{
		if (debug || !Application.isPlaying)
		{
		}
		if (Application.isPlaying)
		{
			fsmEvent.SendEvent(null, eventTarget);
		}
	}
}
