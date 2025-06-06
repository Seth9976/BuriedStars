using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Set the send rate for all networkViews. Default is 15")]
public class NetworkSetSendRate : FsmStateAction
{
	[RequiredField]
	[Tooltip("The send rate for all networkViews")]
	public FsmFloat sendRate;

	public override void Reset()
	{
		sendRate = 15f;
	}

	public override void OnEnter()
	{
		DoSetSendRate();
		Finish();
	}

	private void DoSetSendRate()
	{
		Network.sendRate = sendRate.Value;
	}
}
