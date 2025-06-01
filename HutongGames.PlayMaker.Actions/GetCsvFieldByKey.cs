using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv field by key. Use ReadCsv first with the hasKey option set to true.")]
public class GetCsvFieldByKey : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[Tooltip("The record index")]
	public FsmInt record;

	[Tooltip("The column key")]
	public FsmString key;

	[Tooltip("if true, indexing starts at 0, else first index is 1")]
	public bool zeroBasedIndexing;

	[ActionSection("Result")]
	[Tooltip("The field at record index and key for the csv reference")]
	[UIHint(UIHint.Variable)]
	public FsmString field;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		record = null;
		key = null;
		field = null;
		zeroBasedIndexing = true;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		GetCsvFields();
		Finish();
	}

	private void GetCsvFields()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
			return;
		}
		if (!csvData.HasHeader)
		{
			LogError("Csv Data (" + reference.Value + ") has no header");
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
			return;
		}
		int num = ((!zeroBasedIndexing) ? (record.Value - 1) : record.Value);
		if (csvData.RecordCount <= num)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num + 1) + " records, only " + csvData.RecordCount);
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
			return;
		}
		int num2 = csvData.HeaderKeys.IndexOf(key.Value);
		if (csvData.ColumnCount <= num2)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num2 + 1) + " columns based on key " + key.Value + ", only " + csvData.ColumnCount);
			base.Fsm.Event(errorEvent);
			field.Value = string.Empty;
		}
		else
		{
			field.Value = csvData.GetFieldAt(record.Value, key.Value);
		}
	}
}
