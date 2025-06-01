namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Gets the name of Game Object that owns the FSM and store it in a game object variable.")]
public class GetOwnerName : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString storeName;

	public bool everyFrame;

	public override void Reset()
	{
		storeName = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoGetGameObjectName();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoGetGameObjectName();
	}

	private void DoGetGameObjectName()
	{
		string text = base.Owner.name;
		storeName.Value = text ?? string.Empty;
	}
}
