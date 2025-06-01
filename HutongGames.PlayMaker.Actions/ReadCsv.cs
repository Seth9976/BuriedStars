using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Read a Csv String into accessible data, use GetCsvXXX actions to use it")]
public class ReadCsv : FsmStateAction
{
	[Tooltip("The csv string")]
	[RequiredField]
	public FsmString csvSource;

	[Tooltip("If the csv first line is a headerm check this, it will allow you to use keys to access columns instead of indexes")]
	public FsmBool hasHeader;

	[Tooltip("Save as csv reference")]
	[RequiredField]
	public FsmString storeReference;

	[ActionSection("Result")]
	[Tooltip("The number of records")]
	[UIHint(UIHint.Variable)]
	public FsmInt recordCount;

	[Tooltip("The number of columns")]
	[UIHint(UIHint.Variable)]
	public FsmInt columnCount;

	public FsmEvent errorEvent;

	public override void Reset()
	{
		csvSource = null;
		hasHeader = null;
		recordCount = null;
		columnCount = null;
		storeReference = new FsmString
		{
			UseVariable = true
		};
		errorEvent = null;
	}

	public override void OnEnter()
	{
		ParseCsv();
		Finish();
	}

	private void ParseCsv()
	{
		CsvData csvData = CsvReader.LoadFromString(csvSource.Value, hasHeader.Value);
		CsvData.AddReference(csvData, storeReference.Value);
		if (!recordCount.IsNone)
		{
			recordCount.Value = csvData.RecordCount;
		}
		if (!columnCount.IsNone)
		{
			columnCount.Value = csvData.ColumnCount;
		}
		Finish();
	}
}
