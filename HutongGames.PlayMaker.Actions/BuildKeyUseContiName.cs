namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
public class BuildKeyUseContiName : FsmStateAction
{
	[ActionSection("Input")]
	[RequiredField]
	[Tooltip("Keyword Using ID")]
	public FsmString KeyUseID;

	[Tooltip("Suffix of conti1")]
	public FsmString ContiSuffix1 = "succ";

	[Tooltip("Suffix of conti2")]
	public FsmString ContiSuffix2 = "wrong";

	[Tooltip("Suffix of conti3")]
	public FsmString ContiSuffix3 = "fail";

	[Tooltip("Suffix of conti4")]
	public FsmString ContiSuffix4 = "re";

	[ActionSection("Result")]
	[Tooltip("Operation result name of conti Question")]
	[UIHint(UIHint.Variable)]
	public FsmString resultContiQuestion;

	[Tooltip("Operation result name of conti 1")]
	[UIHint(UIHint.Variable)]
	public FsmString resultConti1;

	[Tooltip("Operation result name of conti 2")]
	[UIHint(UIHint.Variable)]
	public FsmString resultConti2;

	[Tooltip("Operation result name of conti 3")]
	[UIHint(UIHint.Variable)]
	public FsmString resultConti3;

	[Tooltip("Operation result name of conti 4")]
	[UIHint(UIHint.Variable)]
	public FsmString resultConti4;

	public override void Reset()
	{
		KeyUseID = null;
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
		string text = "_";
		resultContiQuestion.Value = KeyUseID.Value + text + "q";
		resultConti1.Value = KeyUseID.Value + text + ContiSuffix1.Value;
		resultConti2.Value = KeyUseID.Value + text + ContiSuffix2.Value;
		resultConti3.Value = KeyUseID.Value + text + ContiSuffix3.Value;
		resultConti4.Value = KeyUseID.Value + text + ContiSuffix4.Value;
		Finish();
	}
}
