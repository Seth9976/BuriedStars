namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Sends Events based on scripting symbol flags")]
public class CheckScriptingSymbol : FsmStateAction
{
	public enum DebugFlags
	{
		ENABLE_NORMAL_END,
		ENABLE_TEST_START
	}

	[CompoundArray("Count", "Key", "Event")]
	public DebugFlags[] debugFlag;

	public FsmEvent[] events;

	public override void Reset()
	{
		debugFlag = new DebugFlags[1];
		debugFlag[0] = DebugFlags.ENABLE_NORMAL_END;
		events = new FsmEvent[1];
		events[0] = null;
	}

	public override void OnEnter()
	{
		int num = 0;
		DebugFlags[] array = debugFlag;
		foreach (DebugFlags debugFlags in array)
		{
			FsmEvent fsmEvent = events[num];
			num++;
		}
		Finish();
	}
}
