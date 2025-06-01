using System.Xml;
using UnityEngine;

public class FsmXmlNodeList : Object
{
	private XmlNodeList _xmlNodeList;

	public XmlNodeList Value
	{
		get
		{
			return _xmlNodeList;
		}
		set
		{
			_xmlNodeList = value;
		}
	}

	public override string ToString()
	{
		return "FsmXmlNodeList";
	}
}
