using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Remove a Csv Reference from runtime Memory")]
public class RemoveCsvReference : FsmStateAction
{
	[Tooltip("The csv string")]
	[RequiredField]
	public FsmString reference;

	[ActionSection("Result")]
	[Tooltip("True if the reference was succesfully removed")]
	[UIHint(UIHint.Variable)]
	public FsmBool success;

	[Tooltip("Event sent if the reference was succesfully removed")]
	public FsmEvent successEvent;

	[Tooltip("Event sent if the reference failed to be removed")]
	public FsmEvent failureEvent;

	public override void Reset()
	{
		reference = null;
		success = null;
		successEvent = null;
		failureEvent = null;
	}

	public override void OnEnter()
	{
		RemoveReference();
		Finish();
	}

	private void RemoveReference()
	{
		bool flag = CsvData.RemoveReference(reference.Value);
		if (!success.IsNone)
		{
			success.Value = flag;
		}
		base.Fsm.Event((!flag) ? failureEvent : successEvent);
	}
}
