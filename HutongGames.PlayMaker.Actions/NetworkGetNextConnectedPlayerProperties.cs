using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Get the next connected player properties. \nEach time this action is called it gets the next child of a GameObject.This lets you quickly loop through all the connected player to perform actions on them.")]
public class NetworkGetNextConnectedPlayerProperties : FsmStateAction
{
	[ActionSection("Set up")]
	[Tooltip("Event to send for looping.")]
	public FsmEvent loopEvent;

	[Tooltip("Event to send when there are no more children.")]
	public FsmEvent finishedEvent;

	[ActionSection("Result")]
	[Tooltip("The player connection index.")]
	[UIHint(UIHint.Variable)]
	public FsmInt index;

	[Tooltip("Get the IP address of this player.")]
	[UIHint(UIHint.Variable)]
	public FsmString IpAddress;

	[Tooltip("Get the port of this player.")]
	[UIHint(UIHint.Variable)]
	public FsmInt port;

	[Tooltip("Get the GUID for this player, used when connecting with NAT punchthrough.")]
	[UIHint(UIHint.Variable)]
	public FsmString guid;

	[Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made.")]
	[UIHint(UIHint.Variable)]
	public FsmString externalIPAddress;

	[Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made.")]
	[UIHint(UIHint.Variable)]
	public FsmInt externalPort;

	private int nextItemIndex;

	public override void Reset()
	{
		finishedEvent = null;
		loopEvent = null;
		index = null;
		IpAddress = null;
		port = null;
		guid = null;
		externalIPAddress = null;
		externalPort = null;
	}

	public override void OnEnter()
	{
		DoGetNextPlayerProperties();
		Finish();
	}

	private void DoGetNextPlayerProperties()
	{
		if (nextItemIndex >= Network.connections.Length)
		{
			base.Fsm.Event(finishedEvent);
			nextItemIndex = 0;
			return;
		}
		NetworkPlayer networkPlayer = Network.connections[nextItemIndex];
		index.Value = nextItemIndex;
		IpAddress.Value = networkPlayer.ipAddress;
		port.Value = networkPlayer.port;
		guid.Value = networkPlayer.guid;
		externalIPAddress.Value = networkPlayer.externalIP;
		externalPort.Value = networkPlayer.externalPort;
		if (nextItemIndex >= Network.connections.Length)
		{
			base.Fsm.Event(finishedEvent);
			nextItemIndex = 0;
			return;
		}
		nextItemIndex++;
		if (loopEvent != null)
		{
			base.Fsm.Event(loopEvent);
		}
	}
}
