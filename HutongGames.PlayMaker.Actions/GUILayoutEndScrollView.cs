using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[Tooltip("Close a group started with GUILayout Begin ScrollView.")]
public class GUILayoutEndScrollView : FsmStateAction
{
	public override void OnGUI()
	{
		GUILayout.EndScrollView();
	}
}
