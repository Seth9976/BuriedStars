using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Get the next host data from the master server. \nEach time this action is called it gets the next connected host.This lets you quickly loop through all the connected hosts to get information on each one.")]
public class MasterServerGetNextHostData : FsmStateAction
{
	[ActionSection("Set up")]
	[Tooltip("Event to send for looping.")]
	public FsmEvent loopEvent;

	[Tooltip("Event to send when there are no more hosts.")]
	public FsmEvent finishedEvent;

	[ActionSection("Result")]
	[Tooltip("The index into the MasterServer Host List")]
	[UIHint(UIHint.Variable)]
	public FsmInt index;

	[Tooltip("Does this server require NAT punchthrough?")]
	[UIHint(UIHint.Variable)]
	public FsmBool useNat;

	[Tooltip("The type of the game (e.g., 'MyUniqueGameType')")]
	[UIHint(UIHint.Variable)]
	public FsmString gameType;

	[Tooltip("The name of the game (e.g., 'John Does's Game')")]
	[UIHint(UIHint.Variable)]
	public FsmString gameName;

	[Tooltip("Currently connected players")]
	[UIHint(UIHint.Variable)]
	public FsmInt connectedPlayers;

	[Tooltip("Maximum players limit")]
	[UIHint(UIHint.Variable)]
	public FsmInt playerLimit;

	[Tooltip("Server IP address.")]
	[UIHint(UIHint.Variable)]
	public FsmString ipAddress;

	[Tooltip("Server port")]
	[UIHint(UIHint.Variable)]
	public FsmInt port;

	[Tooltip("Does the server require a password?")]
	[UIHint(UIHint.Variable)]
	public FsmBool passwordProtected;

	[Tooltip("A miscellaneous comment (can hold data)")]
	[UIHint(UIHint.Variable)]
	public FsmString comment;

	[Tooltip("The GUID of the host, needed when connecting with NAT punchthrough.")]
	[UIHint(UIHint.Variable)]
	public FsmString guid;

	private int nextItemIndex;

	private bool noMoreItems;

	public override void Reset()
	{
		finishedEvent = null;
		loopEvent = null;
		index = null;
		useNat = null;
		gameType = null;
		gameName = null;
		connectedPlayers = null;
		playerLimit = null;
		ipAddress = null;
		port = null;
		passwordProtected = null;
		comment = null;
		guid = null;
	}

	public override void OnEnter()
	{
		DoGetNextHostData();
		Finish();
	}

	private void DoGetNextHostData()
	{
		if (nextItemIndex >= MasterServer.PollHostList().Length)
		{
			nextItemIndex = 0;
			base.Fsm.Event(finishedEvent);
			return;
		}
		HostData hostData = MasterServer.PollHostList()[nextItemIndex];
		index.Value = nextItemIndex;
		useNat.Value = hostData.useNat;
		gameType.Value = hostData.gameType;
		gameName.Value = hostData.gameName;
		connectedPlayers.Value = hostData.connectedPlayers;
		playerLimit.Value = hostData.playerLimit;
		ipAddress.Value = hostData.ip[0];
		port.Value = hostData.port;
		passwordProtected.Value = hostData.passwordProtected;
		comment.Value = hostData.comment;
		guid.Value = hostData.guid;
		if (nextItemIndex >= MasterServer.PollHostList().Length)
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
