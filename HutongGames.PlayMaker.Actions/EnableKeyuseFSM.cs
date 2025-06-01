using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
[Tooltip("Set FSMname and KeyUseID. KeyUseID is set to specified FSM. Sends a ReUse event if the specified FSM is enable. Enable if it is disabled.")]
public class EnableKeyuseFSM : FsmStateAction
{
	public FsmEventTarget EventTarget;

	[RequiredField]
	[Tooltip("KeyWord Use ID")]
	public FsmString KeyUseID;

	[Tooltip("Prefix of conti that describing Keyword")]
	public FsmString KeywordExpPrefix;

	[Tooltip("Send event if the FSM is enable")]
	public FsmEvent ReuseEvent;

	[HasFloatSlider(0f, 10f)]
	[Tooltip("Optional delay in seconds.")]
	public FsmFloat delay;

	private PlayMakerFSM fsmComponent;

	private DelayedEvent delayedEvent;

	private GameObject go;

	private FsmOwnerDefault gameObject;

	private FsmString fsmName;

	private FsmString KeyUseIDVariableName = "A0_KwUsingID";

	private FsmString KeyPrefixUseIDVarName = "A0_KwPrefix";

	private FsmString FsmSentByVariableName = "FSMname";

	private FsmString FsmSentBy;

	public override void Reset()
	{
		KeyUseID = string.Empty;
		delay = 0f;
	}

	public override void OnEnter()
	{
		gameObject = EventTarget.gameObject;
		fsmName = EventTarget.fsmName;
		go = ((gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? gameObject.GameObject.Value : base.Owner);
		if (go == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(fsmName.Value))
		{
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
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
			fsmComponent = go.GetComponent<PlayMakerFSM>();
		}
		if (fsmComponent == null)
		{
			LogError("Missing FsmComponent!");
			return;
		}
		SetKeyUesID();
		if (fsmComponent.enabled)
		{
			SendEvent();
			return;
		}
		DoSetFsmVariable(fsmComponent);
		Finish();
	}

	public override void OnUpdate()
	{
		if (DelayedEvent.WasSent(delayedEvent))
		{
			Finish();
		}
	}

	private void DoSetFsmVariable(PlayMakerFSM fsmComponent)
	{
		FsmSentBy = base.Fsm.Name;
		if (KeyUseID != null)
		{
			FsmString fsmString = fsmComponent.FsmVariables.GetFsmString(FsmSentByVariableName.Value);
			if (fsmString != null)
			{
				fsmString.Value = FsmSentBy.Value;
			}
			else
			{
				LogWarning("Could not find variable: " + FsmSentByVariableName.Value);
			}
			fsmComponent.enabled = true;
		}
	}

	private void SendEvent()
	{
		if (delay.Value < 0.001f)
		{
			base.Fsm.Event(EventTarget, ReuseEvent);
		}
		else
		{
			delayedEvent = base.Fsm.DelayedEvent(EventTarget, ReuseEvent, delay.Value);
		}
	}

	private void SetKeyUesID()
	{
		FsmString fsmString = fsmComponent.FsmVariables.GetFsmString(KeyUseIDVariableName.Value);
		FsmString fsmString2 = fsmComponent.FsmVariables.GetFsmString(KeyPrefixUseIDVarName.Value);
		if (fsmString != null)
		{
			fsmString.Value = KeyUseID.Value;
		}
		else
		{
			LogWarning("Could not find variable: " + KeyUseIDVariableName.Value);
		}
		if (fsmString2 == null)
		{
			LogWarning("Could not find variable: " + fsmString2.Value);
		}
		else if (KeywordExpPrefix.Value != string.Empty)
		{
			fsmString2.Value = KeywordExpPrefix.Value;
		}
		else
		{
			fsmString2.Value = KeyUseID.Value;
		}
	}
}
