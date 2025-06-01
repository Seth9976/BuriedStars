using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Get the network OnDisconnectedFromServer.")]
public class NetworkGetNetworkDisconnectionInfos : FsmStateAction
{
	[Tooltip("Disconnection label")]
	[UIHint(UIHint.Variable)]
	public FsmString disconnectionLabel;

	[Tooltip("The connection to the system has been lost, no reliable packets could be delivered.")]
	public FsmEvent lostConnectionEvent;

	[Tooltip("The connection to the system has been closed.")]
	public FsmEvent disConnectedEvent;

	public override void Reset()
	{
		disconnectionLabel = null;
		lostConnectionEvent = null;
		disConnectedEvent = null;
	}

	public override void OnEnter()
	{
		doGetNetworkDisconnectionInfo();
		Finish();
	}

	private void doGetNetworkDisconnectionInfo()
	{
		NetworkDisconnection disconnectionInfo = Fsm.EventData.DisconnectionInfo;
		disconnectionLabel.Value = disconnectionInfo.ToString();
		switch (disconnectionInfo)
		{
		case NetworkDisconnection.Disconnected:
			if (disConnectedEvent != null)
			{
				base.Fsm.Event(disConnectedEvent);
			}
			break;
		case NetworkDisconnection.LostConnection:
			if (lostConnectionEvent != null)
			{
				base.Fsm.Event(lostConnectionEvent);
			}
			break;
		}
	}
}
