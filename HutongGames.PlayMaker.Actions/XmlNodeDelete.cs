using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Deletes a node.")]
public class XmlNodeDelete : DataMakerXmlActions
{
	[Tooltip("The Reference of the node to delete.")]
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
			xmlNode.ParentNode.RemoveChild(xmlNode);
		}
		else
		{
			base.Fsm.Event(failureEvent);
		}
		Finish();
	}
}
