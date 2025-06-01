using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Store the current send rate for all NetworkViews")]
public class NetworkGetSendRate : FsmStateAction
{
	[RequiredField]
	[Tooltip("Store the current send rate for NetworkViews")]
	[UIHint(UIHint.Variable)]
	public FsmFloat sendRate;

	public override void Reset()
	{
		sendRate = null;
	}

	public override void OnEnter()
	{
		DoGetSendRate();
		Finish();
	}

	private void DoGetSendRate()
	{
		sendRate.Value = Network.sendRate;
	}
}
