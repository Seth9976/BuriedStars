using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Removes all the child nodes and/or attributes of the targeted node.")]
public class XmlNodeRemoveAll : DataMakerXmlActions
{
	[Tooltip("The Reference of the node.")]
	public FsmString xmlReference;

	[Tooltip("Event Fired if the node reference is null")]
	public FsmEvent failureEvent;

	public override void Reset()
	{
		xmlReference = null;
		failureEvent = null;
	}

	public override void OnEnter()
	{
		XmlNode xmlNode = DataMakerXmlUtils.XmlRetrieveNode(xmlReference.Value);
		if (xmlNode != null)
		{
			xmlNode.RemoveAll();
		}
		else
		{
			base.Fsm.Event(failureEvent);
		}
		Finish();
	}
}
