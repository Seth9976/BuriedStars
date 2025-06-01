using System.Xml;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

public class FsmXmlPropertiesStorage : FsmStateAction
{
	public FsmString[] properties;

	public FsmVar[] propertiesVariables;

	public void StoreNodeProperties(Fsm fsm, XmlNode node)
	{
		int num = 0;
		FsmString[] array = properties;
		foreach (FsmString fsmString in array)
		{
			string nodeProperty = DataMakerXmlActions.GetNodeProperty(node, fsmString.Value);
			PlayMakerUtils.ApplyValueToFsmVar(fsm, propertiesVariables[num], PlayMakerUtils.ParseValueFromString(nodeProperty, propertiesVariables[num].Type));
			num++;
		}
	}
}
