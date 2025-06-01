using System.Xml;
using UnityEngine;

public class FsmXmlNode : Object
{
	private XmlNode _xmlNode;

	public XmlNode Value
	{
		get
		{
			return _xmlNode;
		}
		set
		{
			_xmlNode = value;
		}
	}

	public override string ToString()
	{
		return "FsmXmlNode";
	}
}
