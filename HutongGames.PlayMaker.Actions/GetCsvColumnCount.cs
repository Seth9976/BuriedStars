using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv Columns count. Use ReadCsv first.")]
public class GetCsvColumnCount : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[ActionSection("Result")]
	[Tooltip("The number of columns")]
	[UIHint(UIHint.Variable)]
	public FsmInt columnCount;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		columnCount = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		DoGetCsvColumnCount();
		Finish();
	}

	private void DoGetCsvColumnCount()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			columnCount.Value = 0;
		}
		else
		{
			columnCount.Value = csvData.ColumnCount;
		}
	}
}
