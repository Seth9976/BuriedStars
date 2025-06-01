using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Create a node. Use an xml reference to store it.")]
public class XmlCreateNode : DataMakerXmlActions
{
	[Tooltip("The parent node")]
	public FsmString parentNodeReference;

	[ActionSection("XML Node")]
	[RequiredField]
	public FsmString nodeName;

	public FsmString nodeInnerText;

	[CompoundArray("Node Attributes", "Attribute", "Value")]
	public FsmString[] attributes;

	public FsmString[] attributesValues;

	[ActionSection("Store Reference")]
	public FsmString storeReference;

	[ActionSection("Feedback")]
	public FsmEvent errorEvent;

	private XmlNode _node;

	public override void Reset()
	{
		parentNodeReference = null;
		nodeName = null;
		nodeInnerText = null;
		attributes = null;
		attributesValues = null;
		storeReference = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		CreateNode();
		Finish();
	}

	private void CreateNode()
	{
		XmlNode xmlNode = DataMakerXmlUtils.XmlRetrieveNode(parentNodeReference.Value);
		if (xmlNode == null)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		_node = xmlNode.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName.Value, null);
		if (_node == null)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		if (!string.IsNullOrEmpty(storeReference.Value))
		{
			DataMakerXmlUtils.XmlStoreNode(_node, storeReference.Value);
		}
		SetAttributes();
		xmlNode.AppendChild(_node);
		_node.InnerText = nodeInnerText.Value;
	}

	private void SetAttributes()
	{
		int num = 0;
		FsmString[] array = attributes;
		foreach (FsmString fsmString in array)
		{
			DataMakerXmlActions.SetNodeProperty(_node, fsmString.Value, attributesValues[num].Value);
			num++;
		}
	}
}
