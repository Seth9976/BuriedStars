namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[Tooltip("Gets the Game Object that owns the FSM and stores it in a game object variable.")]
public class GetOwner : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeGameObject;

	public override void Reset()
	{
		storeGameObject = null;
	}

	public override void OnEnter()
	{
		storeGameObject.Value = base.Owner;
		Finish();
	}
}
