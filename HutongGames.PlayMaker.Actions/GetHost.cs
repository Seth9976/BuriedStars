namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[Tooltip("Gets the fsm name of the host  when runnin as a sub Fsm")]
public class GetHost : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString hostName;

	public override void Reset()
	{
		hostName = null;
	}

	public override void OnEnter()
	{
		hostName.Value = base.Fsm.Host.Name;
		Finish();
	}
}
