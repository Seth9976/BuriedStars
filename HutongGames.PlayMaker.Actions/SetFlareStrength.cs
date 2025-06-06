using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.RenderSettings)]
[Tooltip("Sets the intensity of all Flares in the scene.")]
public class SetFlareStrength : FsmStateAction
{
	[RequiredField]
	public FsmFloat flareStrength;

	public bool everyFrame;

	public override void Reset()
	{
		flareStrength = 0.2f;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		DoSetFlareStrength();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoSetFlareStrength();
	}

	private void DoSetFlareStrength()
	{
		RenderSettings.flareStrength = flareStrength.Value;
	}
}
