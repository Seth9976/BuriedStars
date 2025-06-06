using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
public class GUILayoutPasswordField : GUILayoutAction
{
	[UIHint(UIHint.Variable)]
	[Tooltip("The password Text")]
	public FsmString text;

	[Tooltip("The Maximum Length of the field")]
	public FsmInt maxLength;

	[Tooltip("The Style of the Field")]
	public FsmString style;

	[Tooltip("Event sent when field content changed")]
	public FsmEvent changedEvent;

	[Tooltip("Replacement character to hide the password")]
	public FsmString mask;

	public override void Reset()
	{
		text = null;
		maxLength = 25;
		style = "TextField";
		mask = "*";
		changedEvent = null;
	}

	public override void OnGUI()
	{
		bool changed = GUI.changed;
		GUI.changed = false;
		text.Value = GUILayout.PasswordField(text.Value, mask.Value[0], style.Value, base.LayoutOptions);
		if (GUI.changed)
		{
			base.Fsm.Event(changedEvent);
			GUIUtility.ExitGUI();
		}
		else
		{
			GUI.changed = changed;
		}
	}
}
