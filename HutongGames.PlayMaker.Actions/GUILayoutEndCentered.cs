using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[Tooltip("End a centered GUILayout block started with GUILayoutBeginCentered.")]
public class GUILayoutEndCentered : FsmStateAction
{
	public override void Reset()
	{
	}

	public override void OnGUI()
	{
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
}
