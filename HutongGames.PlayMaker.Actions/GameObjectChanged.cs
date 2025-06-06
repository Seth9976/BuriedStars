using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Logic)]
[Tooltip("Tests if the value of a GameObject variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
public class GameObjectChanged : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("The GameObject variable to watch for a change.")]
	public FsmGameObject gameObjectVariable;

	[Tooltip("Event to send if the variable changes.")]
	public FsmEvent changedEvent;

	[UIHint(UIHint.Variable)]
	[Tooltip("Set to True if the variable changes.")]
	public FsmBool storeResult;

	private GameObject previousValue;

	public override void Reset()
	{
		gameObjectVariable = null;
		changedEvent = null;
		storeResult = null;
	}

	public override void OnEnter()
	{
		if (gameObjectVariable.IsNone)
		{
			Finish();
		}
		else
		{
			previousValue = gameObjectVariable.Value;
		}
	}

	public override void OnUpdate()
	{
		storeResult.Value = false;
		if (gameObjectVariable.Value != previousValue)
		{
			storeResult.Value = true;
			base.Fsm.Event(changedEvent);
		}
	}
}
