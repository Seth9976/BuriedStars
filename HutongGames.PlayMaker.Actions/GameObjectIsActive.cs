using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Tests if a GameObject Variable is Active or not.")]
public class GameObjectIsActive : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("The GameObject variable to test.")]
	public FsmOwnerDefault gameObject;

	[Tooltip("Event to send if the GamObject is Active.")]
	public FsmEvent isActive;

	[Tooltip("Event to send if the GamObject is NOT Active.")]
	public FsmEvent isNotActive;

	[Tooltip("Event to send if the GamObject is null.")]
	public FsmEvent isNull;

	[UIHint(UIHint.Variable)]
	[Tooltip("Store the result in a bool variable.")]
	public FsmBool storeResult;

	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	private GameObject go;

	public override void Reset()
	{
		gameObject = null;
		isActive = null;
		isNotActive = null;
		isNull = null;
		storeResult = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoIsGameObjectActive();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoIsGameObjectActive();
	}

	private void DoIsGameObjectActive()
	{
		go = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (go != null)
		{
			bool activeInHierarchy = go.activeInHierarchy;
			storeResult.Value = activeInHierarchy;
			base.Fsm.Event((!activeInHierarchy) ? isNotActive : isActive);
		}
		else
		{
			base.Fsm.Event(isNull);
		}
	}
}
