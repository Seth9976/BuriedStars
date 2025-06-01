using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Insert a node. Use an xml reference to store it.")]
public class XmlInsertNode : DataMakerXmlActions
{
	public enum InsertNodeType
	{
		AppendChild,
		PrependChild,
		BeforeChild,
		AfterChild
	}

	[Tooltip("The parent node")]
	public FsmString parentNodeReference;

	public InsertNodeType insert;

	[Tooltip("The child node to use for insertion rule")]
	public FsmString childNodeReference;

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
		insert = InsertNodeType.PrependChild;
		childNodeReference = null;
		nodeName = null;
		nodeInnerText = null;
		attributes = null;
		attributesValues = null;
		storeReference = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		XmlNode xmlNode = DataMakerXmlUtils.XmlRetrieveNode(parentNodeReference.Value);
		_node = xmlNode.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName.Value, null);
		if (!string.IsNullOrEmpty(storeReference.Value))
		{
			DataMakerXmlUtils.XmlStoreNode(_node, storeReference.Value);
		}
		SetAttributes();
		if (insert == InsertNodeType.AfterChild)
		{
			XmlNode refChild = DataMakerXmlUtils.XmlRetrieveNode(childNodeReference.Value);
			xmlNode.InsertAfter(_node, refChild);
		}
		else if (insert == InsertNodeType.BeforeChild)
		{
			XmlNode refChild2 = DataMakerXmlUtils.XmlRetrieveNode(childNodeReference.Value);
			xmlNode.InsertBefore(_node, refChild2);
		}
		else if (insert == InsertNodeType.PrependChild)
		{
			xmlNode.PrependChild(_node);
		}
		else if (insert == InsertNodeType.AppendChild)
		{
			xmlNode.AppendChild(_node);
		}
		_node.InnerText = nodeInnerText.Value;
		Finish();
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
