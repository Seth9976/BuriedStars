namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Set Each Array Value to B_ChkKwsAnswer~ Variables")]
public class UseKeywordCase : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[Tooltip("The Array Variable to use.")]
	public FsmArray arrayExplainType;

	[Tooltip("Event to send to get the next item.")]
	public FsmEvent loopEvent;

	[Tooltip("Event to send when there are no more items.")]
	public FsmEvent finishedEvent;

	[Tooltip("Check Last Loop Finished")]
	public FsmBool ResetArrayIndex = true;

	[ActionSection("Set Values")]
	[Tooltip("List of event name to send after keyword description")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray ListSendEventName;

	[Tooltip("Clone ListKeyword_exp to this Array")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray ArrExpKeywords;

	[ActionSection("Result")]
	[UIHint(UIHint.Variable)]
	public FsmInt c_Index;

	private int nextItemIndex;

	private FsmArray AnswerKeywords;

	private FsmString ExplainType;

	private FsmEvent NextEvent;

	private FsmArray OriginalArray;

	private int currentIndex;

	private string ListNameAnswerKeywords;

	public override void Reset()
	{
		ListSendEventName = null;
		ArrExpKeywords = null;
		AnswerKeywords = null;
		currentIndex = 0;
		c_Index = null;
		loopEvent = null;
		finishedEvent = null;
		ExplainType = null;
		NextEvent = null;
		OriginalArray = null;
		ListNameAnswerKeywords = null;
		ResetArrayIndex = false;
	}

	public override void OnEnter()
	{
		if (ResetArrayIndex.Value)
		{
			nextItemIndex = 0;
			ResetArrayIndex.Value = false;
		}
		ExplainType = base.Fsm.Variables.GetFsmString("ExplainType");
		DoGetNextItem();
		if (nextItemIndex > 0)
		{
			ListNameAnswerKeywords = "ListKeyword_exp" + nextItemIndex;
			AnswerKeywords = base.Fsm.Variables.GetFsmArray(ListNameAnswerKeywords);
		}
		if (AnswerKeywords == null)
		{
			base.Fsm.Event(NextEvent);
			Finish();
		}
		SetEventName();
		if (NextEvent == loopEvent)
		{
			DoSetStringValueFromArray();
			ArrayClone();
		}
		if (NextEvent == finishedEvent)
		{
			ArrayCloneSelected();
		}
		if (NextEvent != null)
		{
			c_Index.Value = currentIndex;
			base.Fsm.Event(NextEvent);
		}
		Finish();
	}

	private void DoGetNextItem()
	{
		if (nextItemIndex >= arrayExplainType.Length)
		{
			nextItemIndex = 0;
			currentIndex = arrayExplainType.Length;
			NextEvent = finishedEvent;
		}
		else
		{
			ExplainType.Value = (string)arrayExplainType.Get(nextItemIndex);
			nextItemIndex++;
			currentIndex = nextItemIndex - 1;
			NextEvent = loopEvent;
		}
	}

	private void DoSetStringValueFromArray()
	{
		for (int i = 0; i < AnswerKeywords.Length; i++)
		{
			string text = i.ToString();
			text = "B_ChkKwsAnswer" + text;
			FsmString fsmString = base.Fsm.Variables.GetFsmString(text);
			object obj = AnswerKeywords.Get(i);
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

	private void SetEventName()
	{
		FsmString fsmString = base.Fsm.Variables.GetFsmString("EventName");
		fsmString.Value = "kw_Fail";
		if (currentIndex < ListSendEventName.Length)
		{
			fsmString.Value = (string)ListSendEventName.Get(currentIndex);
		}
	}

	private void ArrayClone()
	{
		OriginalArray = base.Fsm.Variables.GetFsmArray(ListNameAnswerKeywords);
		ArrExpKeywords.Resize(OriginalArray.Length);
		for (int i = 0; i < OriginalArray.Length; i++)
		{
			ArrExpKeywords.Set(i, OriginalArray.Values[i]);
		}
	}

	private void ArrayCloneSelected()
	{
		OriginalArray = base.Fsm.Variables.GetFsmArray("m_arrSelectedKW");
		ArrExpKeywords.Resize(OriginalArray.Length);
		for (int i = 0; i < OriginalArray.Length; i++)
		{
			ArrExpKeywords.Set(i, OriginalArray.Values[i]);
		}
	}
}
