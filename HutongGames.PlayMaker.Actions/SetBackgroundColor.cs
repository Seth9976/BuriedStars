using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Camera)]
[Tooltip("Sets the Background Color used by the Camera.")]
public class SetBackgroundColor : ComponentAction<Camera>
{
	[RequiredField]
	[CheckForComponent(typeof(Camera))]
	public FsmOwnerDefault gameObject;

	[RequiredField]
	public FsmColor backgroundColor;

	public bool everyFrame;

	public override void Reset()
	{
		gameObject = null;
		backgroundColor = Color.black;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoSetBackgroundColor();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoSetBackgroundColor();
	}

	private void DoSetBackgroundColor()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (UpdateCache(ownerDefaultTarget))
		{
			base.camera.backgroundColor = backgroundColor.Value;
		}
	}
}
