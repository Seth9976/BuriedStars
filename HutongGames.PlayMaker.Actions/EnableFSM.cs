using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Enables/Disables an FSM component on a GameObject.")]
public class EnableFSM : FsmStateAction
{
	[RequiredField]
	[Tooltip("The GameObject that owns the FSM component.")]
	public FsmOwnerDefault gameObject;

	[UIHint(UIHint.FsmName)]
	[Tooltip("Optional name of FSM on GameObject. Useful if you have more than one FSM on a GameObject.")]
	public FsmString fsmName;

	[Tooltip("Set to True to enable, False to disable.")]
	public FsmBool enable;

	[Tooltip("Reset the initial enabled state when exiting the state.")]
	public FsmBool resetOnExit;

	private PlayMakerFSM fsmComponent;

	public override void Reset()
	{
		gameObject = null;
		fsmName = string.Empty;
		enable = true;
		resetOnExit = true;
	}

	public override void OnEnter()
	{
		DoEnableFSM();
		Finish();
	}

	private void DoEnableFSM()
	{
		GameObject gameObject = ((this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? this.gameObject.GameObject.Value : base.Owner);
		if (gameObject == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(fsmName.Value))
		{
			PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
			PlayMakerFSM[] array = components;
			foreach (PlayMakerFSM playMakerFSM in array)
			{
				if (playMakerFSM.FsmName == fsmName.Value)
				{
					fsmComponent = playMakerFSM;
					break;
				}
			}
		}
		else
		{
			fsmComponent = gameObject.GetComponent<PlayMakerFSM>();
		}
		if (fsmComponent == null)
		{
			LogError("Missing FsmComponent!");
		}
		else
		{
			fsmComponent.enabled = enable.Value;
		}
	}

	public override void OnExit()
	{
		if (!(fsmComponent == null) && resetOnExit.Value)
		{
			fsmComponent.enabled = !enable.Value;
		}
	}
}
