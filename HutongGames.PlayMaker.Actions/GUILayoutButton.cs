using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
public class GUILayoutButton : GUILayoutAction
{
	public FsmEvent sendEvent;

	[UIHint(UIHint.Variable)]
	public FsmBool storeButtonState;

	public FsmTexture image;

	public FsmString text;

	public FsmString tooltip;

	public FsmString style;

	public override void Reset()
	{
		base.Reset();
		sendEvent = null;
		storeButtonState = null;
		text = string.Empty;
		image = null;
		tooltip = string.Empty;
		style = string.Empty;
	}

	public override void OnGUI()
	{
		bool flag = ((!string.IsNullOrEmpty(style.Value)) ? GUILayout.Button(new GUIContent(text.Value, image.Value, tooltip.Value), style.Value, base.LayoutOptions) : GUILayout.Button(new GUIContent(text.Value, image.Value, tooltip.Value), base.LayoutOptions));
		if (flag)
		{
			base.Fsm.Event(sendEvent);
		}
		if (storeButtonState != null)
		{
			storeButtonState.Value = flag;
		}
	}
}
