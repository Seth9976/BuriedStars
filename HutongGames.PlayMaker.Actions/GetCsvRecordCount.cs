using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv Records count. Use ReadCsv first.")]
public class GetCsvRecordCount : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[ActionSection("Result")]
	[Tooltip("The number of records")]
	[UIHint(UIHint.Variable)]
	public FsmInt recordCount;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		recordCount = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		DoGetCsvRecordCount();
		Finish();
	}

	private void DoGetCsvRecordCount()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			recordCount.Value = 0;
		}
		else
		{
			recordCount.Value = csvData.RecordCount;
		}
	}
}
