using System.Xml;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

public class FsmXmlProperty : FsmStateAction
{
	public FsmString property;

	[UIHint(UIHint.Variable)]
	public FsmVar variable;

	public static void StoreNodeProperties(Fsm fsm, XmlNode node, FsmXmlProperty[] properties)
	{
		int num = 0;
		foreach (FsmXmlProperty fsmXmlProperty in properties)
		{
			string nodeProperty = DataMakerXmlActions.GetNodeProperty(node, fsmXmlProperty.property.Value);
			PlayMakerUtils.ApplyValueToFsmVar(fsm, fsmXmlProperty.variable, PlayMakerUtils.ParseValueFromString(nodeProperty, fsmXmlProperty.variable.Type));
			num++;
		}
	}
}
