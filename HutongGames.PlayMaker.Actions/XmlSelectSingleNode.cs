using System.Xml;
using System.Xml.XPath;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Gets a node attributes and cdata from a xml text asset and an xpath query. Properties are referenced from the node itself, so a '.' is prepended if you use xpath within the property string like ")]
public class XmlSelectSingleNode : DataMakerXmlActions
{
	[ActionSection("Xml Source")]
	public FsmXmlSource xmlSource;

	[ActionSection("xPath Query")]
	public FsmXpathQuery xPath;

	[ActionSection("Result")]
	[Tooltip("The result of the xPathQuery as an xml string")]
	[UIHint(UIHint.Variable)]
	public FsmString xmlResult;

	[Tooltip("The result of the xPathQuery stored in memory. More efficient if you want to process the result further")]
	public FsmString storeReference;

	[ActionSection("Properties Storage")]
	public FsmXmlPropertiesStorage storeProperties;

	[ActionSection("Properties Storage")]
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
		xPath = null;
		xmlResult = null;
		storeReference = null;
		storeProperties = new FsmXmlPropertiesStorage();
		storeProperties.Fsm = base.Fsm;
		storeNodeProperties = null;
		found = null;
		foundEvent = null;
		notFoundEvent = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		SelectSingleNode();
		Finish();
	}

	private void SelectSingleNode()
	{
		if (xmlSource.Value == null)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		string xpath = xPath.ParseXpathQuery(base.Fsm);
		XmlNode xmlNode = null;
		try
		{
			xmlNode = xmlSource.Value.SelectSingleNode(xpath);
		}
		catch (XPathException)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		if (xmlNode != null)
		{
			if (!xmlResult.IsNone)
			{
				xmlResult.Value = DataMakerXmlUtils.XmlNodeToString(xmlNode);
			}
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
		if (!string.IsNullOrEmpty(storeReference.Value))
		{
			DataMakerXmlUtils.XmlStoreNode(xmlNode, storeReference.Value);
		}
		Finish();
	}
}
