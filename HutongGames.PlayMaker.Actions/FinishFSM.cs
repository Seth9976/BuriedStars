namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[Note("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
[Tooltip("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
public class FinishFSM : FsmStateAction
{
	public override void OnEnter()
	{
		base.Fsm.Stop();
	}
}
