using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GameObject)]
[Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
public class GetTransform : FsmStateAction
{
	[RequiredField]
	public FsmGameObject gameObject;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(Transform))]
	public FsmObject storeTransform;

	public bool everyFrame;

	public override void Reset()
	{
		gameObject = new FsmGameObject
		{
			UseVariable = true
		};
		storeTransform = null;
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
		GameObject value = gameObject.Value;
		storeTransform.Value = ((!(value != null)) ? null : value.transform);
	}
}
