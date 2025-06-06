using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[Tooltip("Gets the top most parent of the Game Object.\nIf the game object has no parent, returns itself.")]
public class GetRoot : FsmStateAction
{
	[RequiredField]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeRoot;

	public override void Reset()
	{
		gameObject = null;
		storeRoot = null;
	}

	public override void OnEnter()
	{
		DoGetRoot();
		Finish();
	}

	private void DoGetRoot()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (!(ownerDefaultTarget == null))
		{
			storeRoot.Value = ownerDefaultTarget.transform.root.gameObject;
		}
	}
}
