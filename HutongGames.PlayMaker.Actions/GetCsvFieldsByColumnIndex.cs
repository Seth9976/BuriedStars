using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv Fields by Column index and save it in an array of string. Use ReadCsv first.")]
public class GetCsvFieldsByColumnIndex : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[Tooltip("The column Index")]
	public FsmInt column;

	[Tooltip("if true, first item is index 0, else first item is index 1")]
	public bool zeroBasedIndexing;

	[ActionSection("Result")]
	[Tooltip("All fields at column the csv reference")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	[UIHint(UIHint.Variable)]
	public FsmArray result;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		column = null;
		result = null;
		zeroBasedIndexing = true;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		DoGetCsvColumn();
		Finish();
	}

	private void DoGetCsvColumn()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			result.Resize(0);
			return;
		}
		int num = ((!zeroBasedIndexing) ? (column.Value - 1) : column.Value);
		if (csvData.ColumnCount <= num)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + num + " columns, only " + csvData.ColumnCount);
			base.Fsm.Event(errorEvent);
			result.Resize(0);
		}
		else
		{
			result.Resize(csvData.RecordCount);
			for (int i = 0; i < csvData.RecordCount; i++)
			{
				result.Set(i, csvData.GetFieldAt(i, num, logErrors: false));
			}
			result.SaveChanges();
		}
	}
}
