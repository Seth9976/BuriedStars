using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/HashTable")]
[Tooltip("Parse a XmlNode properties and attributes inside a HashTable")]
public class HashTableGetXmlNodeProperties : HashTableActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
	[CheckForComponent(typeof(PlayMakerHashTableProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[ActionSection("XML Source")]
	public FsmXmlSource xmlSource;

	public FsmXmlPropertiesTypes propertiesTypes;

	[ActionSection("Feedback")]
	public FsmEvent successEvent;

	public FsmEvent failureEvent;

	public override void Reset()
	{
		gameObject = null;
		reference = null;
		xmlSource = null;
		propertiesTypes = new FsmXmlPropertiesTypes();
		successEvent = null;
		failureEvent = null;
	}

	public override void OnEnter()
	{
		if (SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			if (ParseNode())
			{
				base.Fsm.Event(successEvent);
			}
			else
			{
				base.Fsm.Event(failureEvent);
			}
		}
		Finish();
	}

	public bool ParseNode()
	{
		if (!isProxyValid())
		{
			return false;
		}
		if (xmlSource.Value == null)
		{
			return false;
		}
		XmlAttributeCollection attributes = xmlSource.Value.Attributes;
		foreach (XmlAttribute item in attributes)
		{
			if (!string.IsNullOrEmpty(item.InnerText))
			{
				proxy.hashTable["@" + item.Name] = PlayMakerUtils.ParseValueFromString(item.InnerText, propertiesTypes.GetPropertyType("@" + item.Name));
			}
		}
		foreach (XmlNode childNode in xmlSource.Value.ChildNodes)
		{
			if (!string.IsNullOrEmpty(childNode.InnerText))
			{
				proxy.hashTable[childNode.Name] = PlayMakerUtils.ParseValueFromString(childNode.InnerText, propertiesTypes.GetPropertyType(childNode.Name));
			}
		}
		return true;
	}
}
