using HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Csv")]
[Tooltip("Remove All Csv References from runtime Memory")]
public class RemoveAllCsvReference : FsmStateAction
{
	public override void OnEnter()
	{
		CsvData.RemoveAllReferences();
		Finish();
	}
}
