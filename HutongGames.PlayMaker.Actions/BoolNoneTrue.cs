namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Tests if all the Bool Variables are False.\nSend an event or store the result.")]
public class BoolNoneTrue : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("The Bool variables to check.")]
	public FsmBool[] boolVariables;

	[Tooltip("Event to send if none of the Bool variables are True.")]
	public FsmEvent sendEvent;

	[UIHint(UIHint.Variable)]
	[Tooltip("Store the result in a Bool variable.")]
	public FsmBool storeResult;

	[Tooltip("Repeat every frame while the state is active.")]
	public bool everyFrame;

	public override void Reset()
	{
		boolVariables = null;
		sendEvent = null;
		storeResult = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoNoneTrue();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoNoneTrue();
	}

	private void DoNoneTrue()
	{
		if (boolVariables.Length == 0)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < boolVariables.Length; i++)
		{
			if (boolVariables[i].Value)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			base.Fsm.Event(sendEvent);
		}
		storeResult.Value = flag;
	}
}
