namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Set Each Array Value to B_ChkKwsAnswer~ Variables")]
public class SetStringValueFromArray : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("A String variable to store the converted value.")]
	public FsmArray AnswerKeywords;

	[Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
	public bool everyFrame;

	private int currentIndex;

	public override void Reset()
	{
		AnswerKeywords = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		currentIndex = 0;
		DoSetStringValueFromArray();
		if (!everyFrame)
		{
			Finish();
		}
		if (AnswerKeywords == null)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoSetStringValueFromArray();
	}

	private void DoSetStringValueFromArray()
	{
		for (currentIndex = 0; currentIndex < AnswerKeywords.Length; currentIndex++)
		{
			string text = currentIndex.ToString();
			text = "B_ChkKwsAnswer" + text;
			FsmString fsmString = base.Fsm.Variables.GetFsmString(text);
			object obj = AnswerKeywords.Get(currentIndex);
			if (obj == null)
			{
				break;
			}
			string value = (string)obj;
			if (fsmString != null)
			{
				fsmString.Value = value;
			}
			else
			{
				LogWarning("Could not find variable: " + text);
			}
		}
	}
}
