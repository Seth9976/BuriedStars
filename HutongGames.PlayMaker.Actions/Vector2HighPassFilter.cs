using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Vector2)]
[Tooltip("Use a high pass filter to isolate sudden changes in a Vector2 Variable.")]
public class Vector2HighPassFilter : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input.")]
	public FsmVector2 vector2Variable;

	[Tooltip("Determines how much influence new changes have.")]
	public FsmFloat filteringFactor;

	private Vector2 filteredVector;

	public override void Reset()
	{
		vector2Variable = null;
		filteringFactor = 0.1f;
	}

	public override void OnEnter()
	{
		filteredVector = new Vector2(vector2Variable.Value.x, vector2Variable.Value.y);
	}

	public override void OnUpdate()
	{
		filteredVector.x = vector2Variable.Value.x - (vector2Variable.Value.x * filteringFactor.Value + filteredVector.x * (1f - filteringFactor.Value));
		filteredVector.y = vector2Variable.Value.y - (vector2Variable.Value.y * filteringFactor.Value + filteredVector.y * (1f - filteringFactor.Value));
		vector2Variable.Value = new Vector2(filteredVector.x, filteredVector.y);
	}
}
