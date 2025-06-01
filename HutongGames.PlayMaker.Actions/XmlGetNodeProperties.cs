using System.Xml;
using System.Xml.XPath;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Gets a node attributes and Properties, Properties are referenced from the node itself, so a '.' is prepended if you use xpath within the property string like ")]
public class XmlGetNodeProperties : DataMakerXmlActions
{
	[ActionSection("XML Source")]
	public FsmXmlSource xmlSource;

	[ActionSection("Result")]
	public FsmXmlPropertiesStorage storeProperties;

	[ActionSection("Result")]
	public FsmXmlProperty[] storeNodeProperties;

	[ActionSection("Feedback")]
	[UIHint(UIHint.Variable)]
	public FsmBool found;

	public FsmEvent foundEvent;

	public FsmEvent notFoundEvent;

	public FsmEvent errorEvent;

	public override void Reset()
	{
		xmlSource = null;
		storeProperties = new FsmXmlPropertiesStorage();
		storeNodeProperties = null;
		found = null;
		foundEvent = null;
		notFoundEvent = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		GetNodeProps();
		Finish();
	}

	private void GetNodeProps()
	{
		XmlNode xmlNode = null;
		try
		{
			xmlNode = xmlSource.Value;
		}
		catch (XPathException)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		if (xmlNode != null)
		{
			if (storeNodeProperties.Length > 0)
			{
				FsmXmlProperty.StoreNodeProperties(base.Fsm, xmlNode, storeNodeProperties);
			}
			else
			{
				storeProperties.StoreNodeProperties(base.Fsm, xmlNode);
			}
			found.Value = true;
			base.Fsm.Event(foundEvent);
		}
		else
		{
			found.Value = false;
			base.Fsm.Event(notFoundEvent);
		}
		Finish();
	}
}
