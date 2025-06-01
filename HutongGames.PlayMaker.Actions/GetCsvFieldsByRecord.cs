using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get Csv Fields by Record index and save it in an array of string. Use ReadCsv first.")]
public class GetCsvFieldsByRecord : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[Tooltip("The record index.")]
	public FsmInt record;

	[Tooltip("if true, indexing starts at 0, else first index is 1")]
	public bool zeroBasedIndexing;

	[ActionSection("Result")]
	[Tooltip("All fields at record index the csv reference")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	[UIHint(UIHint.Variable)]
	public FsmArray fields;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		record = null;
		fields = null;
		zeroBasedIndexing = true;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		DoGetCsvFields();
		Finish();
	}

	private void DoGetCsvFields()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			fields.Resize(0);
			return;
		}
		int num = ((!zeroBasedIndexing) ? (record.Value - 1) : record.Value);
		if (csvData.RecordCount <= num)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num + 1) + " records, only " + csvData.RecordCount);
			base.Fsm.Event(errorEvent);
			fields.Resize(0);
		}
		else
		{
			fields.stringValues = csvData.GetRecordAt(num);
			fields.SaveChanges();
		}
	}
}
