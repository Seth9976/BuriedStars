using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[Tooltip("Set the IP address, port, update rate and dedicated server flag of the master server.")]
public class MasterServerSetProperties : FsmStateAction
{
	[Tooltip("Set the IP address of the master server.")]
	public FsmString ipAddress;

	[Tooltip("Set the connection port of the master server.")]
	public FsmInt port;

	[Tooltip("Set the minimum update rate for master server host information update. Default is 60 seconds.")]
	public FsmInt updateRate;

	[Tooltip("Set if this machine is a dedicated server.")]
	public FsmBool dedicatedServer;

	public override void Reset()
	{
		ipAddress = "127.0.0.1";
		port = 10002;
		updateRate = 60;
		dedicatedServer = false;
	}

	public override void OnEnter()
	{
		SetMasterServerProperties();
		Finish();
	}

	private void SetMasterServerProperties()
	{
		MasterServer.ipAddress = ipAddress.Value;
		MasterServer.port = port.Value;
		MasterServer.updateRate = updateRate.Value;
		MasterServer.dedicatedServer = dedicatedServer.Value;
	}
}
