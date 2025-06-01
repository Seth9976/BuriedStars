using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Get a Csv Header and save it in an array of string. Use ReadCsv first.")]
public class GetCsvHeader : FsmStateAction
{
	[Tooltip("The csv reference defined in ReadCsv action")]
	public FsmString reference;

	[ActionSection("Result")]
	[Tooltip("All header values the csv reference")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	[UIHint(UIHint.Variable)]
	public FsmArray header;

	[Tooltip("Event sent if an error ocurred")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		reference = null;
		header = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		DoGetCsvHeader();
		Finish();
	}

	private void DoGetCsvHeader()
	{
		CsvData csvData = CsvData.GetReference(reference.Value);
		if (csvData == null)
		{
			base.Fsm.Event(errorEvent);
			header.Resize(0);
		}
		else if (!csvData.HasHeader)
		{
			LogError("Csv Data '" + reference.Value + "' has no header");
			base.Fsm.Event(errorEvent);
			header.Resize(0);
		}
		else
		{
			header.stringValues = csvData.HeaderKeys.ToArray();
			header.SaveChanges();
		}
	}
}
