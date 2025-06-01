using HutongGames.PlayMaker;

public class FsmXpathQuery : FsmStateAction
{
	public FsmString xPathQuery;

	public FsmVar[] xPathVariables;

	public bool _foldout = true;

	public string parsedQuery;

	public string ParseXpathQuery(Fsm fsm)
	{
		parsedQuery = xPathQuery.Value;
		if (xPathVariables != null)
		{
			int num = 0;
			FsmVar[] array = xPathVariables;
			foreach (FsmVar fsmVar in array)
			{
				if (!fsmVar.IsNone)
				{
					parsedQuery = parsedQuery.Replace("_" + num + "_", PlayMakerUtils.ParseFsmVarToString(fsm, fsmVar));
				}
				num++;
			}
		}
		return parsedQuery;
	}
}
