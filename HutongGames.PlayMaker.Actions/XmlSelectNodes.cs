using System.Xml;
using System.Xml.XPath;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Gets nodes a xml text asset and an xpath query. Properties are referenced from the node itself, so a '.' is prepended if you use xpath within the property string like ")]
public class XmlSelectNodes : DataMakerXmlActions
{
	[ActionSection("XML Source")]
	public FsmXmlSource xmlSource;

	[ActionSection("xPath Query")]
	public FsmXpathQuery xPath;

	[ActionSection("Result")]
	[Tooltip("The result of the xPathQuery, wrapped into a 'result' node, so that it's resuable and a valid xml")]
	[UIHint(UIHint.Variable)]
	public FsmString xmlResult;

	[Tooltip("The result of the xPathQuery stored in memory. More efficient if you want to process the result further")]
	public FsmString storeReference;

	[Tooltip("The number of entries found for the xPathQuery")]
	[UIHint(UIHint.Variable)]
	public FsmInt nodeCount;

	[ActionSection("Feedback")]
	[UIHint(UIHint.Variable)]
	public FsmBool found;

	public FsmEvent foundEvent;

	public FsmEvent notFoundEvent;

	public FsmEvent errorEvent;

	public override void Reset()
	{
		xmlSource = null;
		xPath = new FsmXpathQuery();
		nodeCount = null;
		xmlResult = null;
		found = null;
		foundEvent = null;
		notFoundEvent = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		SelectNodeList();
		Finish();
	}

	private void SelectNodeList()
	{
		nodeCount.Value = 0;
		if (xmlSource.Value == null)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		string xpath = xPath.ParseXpathQuery(base.Fsm);
		XmlNodeList xmlNodeList = null;
		try
		{
			xmlNodeList = xmlSource.Value.SelectNodes(xpath);
		}
		catch (XPathException)
		{
			base.Fsm.Event(errorEvent);
			return;
		}
		if (xmlNodeList != null)
		{
			nodeCount.Value = xmlNodeList.Count;
			if (xmlNodeList.Count == 0)
			{
				found.Value = false;
				base.Fsm.Event(notFoundEvent);
				return;
			}
			if (!xmlResult.IsNone)
			{
				xmlResult.Value = DataMakerXmlUtils.XmlNodeListToString(xmlNodeList);
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
			DataMakerXmlUtils.XmlStoreNodeList(xmlNodeList, storeReference.Value);
		}
	}
}
