using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv field by index. Use ReadCsv first.")]
public class GetCsvFieldByIndex : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[Tooltip("If the csv first line is a header check this, it will allow you to use keys to access columns instead of indexes")]
	public FsmInt record;

	[Tooltip("If the csv first line is a header check this, it will allow you to use keys to access columns instead of indexes")]
	public FsmInt column;

	[Tooltip("if true, indexing starts at 0, else first index is 1")]
	public bool zeroBasedIndexing;

	[ActionSection("Result")]
	[Tooltip("The field at row and column for the csv reference")]
	[UIHint(UIHint.Variable)]
	public FsmString field;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		record = null;
		column = null;
		field = null;
		zeroBasedIndexing = true;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		GetCsvEntry();
		Finish();
	}

	private void GetCsvEntry()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
			return;
		}
		int num = ((!zeroBasedIndexing) ? (record.Value - 1) : record.Value);
		int num2 = ((!zeroBasedIndexing) ? (column.Value - 1) : column.Value);
		if (csvData.RecordCount <= num)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num + 1) + " records, only " + csvData.RecordCount);
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
		}
		else if (csvData.ColumnCount <= num2)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num2 + 1) + " columns, only " + csvData.ColumnCount);
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
		}
		else
		{
			field.Value = csvData.GetFieldAt(num, num2);
		}
	}
}
