using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Register this server on the master server.\n\nIf the master server address information has not been changed the default Unity master server will be used.")]
public class MasterServerRegisterHost : FsmStateAction
{
	[RequiredField]
	[Tooltip("The unique game type name.")]
	public FsmString gameTypeName;

	[RequiredField]
	[Tooltip("The game name.")]
	public FsmString gameName;

	[Tooltip("Optional comment")]
	public FsmString comment;

	public override void Reset()
	{
		gameTypeName = null;
		gameName = null;
		comment = null;
	}

	public override void OnEnter()
	{
		DoMasterServerRegisterHost();
		Finish();
	}

	private void DoMasterServerRegisterHost()
	{
		MasterServer.RegisterHost(gameTypeName.Value, gameName.Value, comment.Value);
	}
}
