namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Gets the name of the current active state and stores it in a String Variable.")]
public class GetCurrentStateName : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmString storeName;

	public override void Reset()
	{
		storeName = null;
	}

	public override void OnEnter()
	{
		storeName.Value = ((base.Fsm.ActiveState != null) ? base.Fsm.ActiveStateName : null);
		Finish();
	}
}
