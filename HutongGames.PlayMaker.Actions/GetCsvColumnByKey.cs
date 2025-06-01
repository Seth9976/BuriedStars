using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv Column by key and save it in an array of string. Use ReadCsv first.")]
public class GetCsvColumnByKey : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[Tooltip("The column Key")]
	public FsmString key;

	[ActionSection("Result")]
	[Tooltip("All values at column the csv reference")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	[UIHint(UIHint.Variable)]
	public FsmArray fields;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		key = null;
		fields = null;
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
			fields.Resize(0);
			return;
		}
		if (!csvData.HasHeader)
		{
			LogError("Csv Data '" + reference.Value + "' has no header");
			base.Fsm.Event(errorEvent);
			fields.Resize(0);
			return;
		}
		int num = csvData.HeaderKeys.IndexOf(key.Value);
		if (csvData.ColumnCount <= num)
		{
			LogError("Csv Data '" + reference.Value + "' doesn't have " + (num + 1) + " columns based on key " + key.Value + ", only " + csvData.ColumnCount);
			base.Fsm.Event(errorEvent);
			fields.Resize(0);
		}
		else
		{
			fields.Resize(csvData.RecordCount);
			for (int i = 0; i < csvData.RecordCount; i++)
			{
				fields.Set(i, csvData.GetFieldAt(i, num, logErrors: false));
			}
			fields.SaveChanges();
		}
	}
}
