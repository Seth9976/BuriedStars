namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Builds a name of conti that Explain Keyword from CaseKeywords Event.")]
public class BuildEachKeywordConti : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString KeywordUsingID;

	public FsmString ContiSuffix;

	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString ContiName;

	public override void Reset()
	{
		KeywordUsingID = string.Empty;
		ContiSuffix = string.Empty;
		ContiName = string.Empty;
	}

	public override void OnEnter()
	{
		DoBuildString();
	}

	public override void OnUpdate()
	{
		DoBuildString();
	}

	private void DoBuildString()
	{
		string text = ((base.Fsm.LastTransition != null) ? base.Fsm.LastTransition.EventName : "START");
		string text2 = text.Replace("CaseKeyword", ContiSuffix.Value);
		ContiName.Value = KeywordUsingID.Value + "_" + text2;
		Finish();
	}
}
