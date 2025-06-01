using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Sets node properties and attributes. Use an xml reference.")]
public class XmlSetNodeProperties : DataMakerXmlActions
{
	[ActionSection("XML Node")]
	public FsmString xmlReference;

	[CompoundArray("Node Properties", "Properties", "Value")]
	public FsmString[] attributes;

	public FsmString[] attributesValues;

	[ActionSection("Feedback")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		xmlReference = null;
		attributes = null;
		attributesValues = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		SetAttributes();
		Finish();
	}

	private void SetAttributes()
	{
		XmlNode xmlNode = DataMakerXmlUtils.XmlRetrieveNode(xmlReference.Value);
		if (xmlNode == null)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		int num = 0;
		FsmString[] array = attributes;
		foreach (FsmString fsmString in array)
		{
			DataMakerXmlActions.SetNodeProperty(xmlNode, fsmString.Value, attributesValues[num].Value);
			num++;
		}
		Finish();
	}
}
