using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Check Selected Keyword contains array")]
public class ArrayContainsEach : FsmStateAction
{
	[ActionSection("Input")]
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[Tooltip("The 1st Array Variable to test.")]
	public FsmArray array1;

	[UIHint(UIHint.Variable)]
	[Tooltip("The 2nd Array Variable to test.")]
	public FsmArray array2;

	[UIHint(UIHint.Variable)]
	[Tooltip("The 3rd Array Variable to test.")]
	public FsmArray array3;

	[UIHint(UIHint.Variable)]
	[Tooltip("The 4th Array Variable to test.")]
	public FsmArray array4;

	public FsmString SelectedKeyword;

	[UIHint(UIHint.Variable)]
	[ActionSection("Result")]
	[Tooltip("Event sent if the array 1 contains that keyword")]
	public FsmEvent isContainedEvent1;

	[Tooltip("Event sent if the array 2 contains that keyword")]
	public FsmEvent isContainedEvent2;

	[Tooltip("Event sent if the array 3 contains that keyword")]
	public FsmEvent isContainedEvent3;

	[Tooltip("Event sent if the array 4 contains that keyword")]
	public FsmEvent isContainedEvent4;

	[Tooltip("Event sent if any array do not contains that keyword")]
	public FsmEvent isNotContainedEvent;

	private bool isEventSent;

	public override void Reset()
	{
		array1 = null;
		array2 = null;
		array3 = null;
		SelectedKeyword = null;
		isContainedEvent1 = null;
		isContainedEvent2 = null;
		isContainedEvent3 = null;
		isContainedEvent4 = null;
	}

	public override void OnEnter()
	{
		string text = SelectedKeyword.Value.Replace(",", string.Empty);
		if (array1 != null)
		{
			isEventSent = DoCheckContainsValue(array1, text, isContainedEvent1);
		}
		if (array2 != null && !isEventSent)
		{
			isEventSent = DoCheckContainsValue(array2, text, isContainedEvent2);
		}
		if (array3 != null && !isEventSent)
		{
			isEventSent = DoCheckContainsValue(array3, text, isContainedEvent3);
		}
		if (array4 != null && !isEventSent)
		{
			isEventSent = DoCheckContainsValue(array4, text, isContainedEvent4);
		}
		if (!isEventSent)
		{
			base.Fsm.Event(isNotContainedEvent);
		}
		Finish();
	}

	private bool DoCheckContainsValue(FsmArray array, FsmString value, FsmEvent fsmEvent)
	{
		int num = Array.IndexOf(array.Values, value.Value);
		bool flag = num != -1;
		if (flag)
		{
			base.Fsm.Event(fsmEvent);
		}
		return flag;
	}
}
