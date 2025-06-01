using System.Xml;

namespace HutongGames.PlayMaker.Actions;

public abstract class DataMakerXmlActions : FsmStateAction
{
	public static string GetNodeProperty(XmlNode node, string property)
	{
		if (property.StartsWith("@"))
		{
			property = property.Remove(0, 1);
			XmlAttribute xmlAttribute = node.Attributes[property];
			if (xmlAttribute != null)
			{
				return xmlAttribute.InnerText;
			}
		}
		else
		{
			if (!property.StartsWith("/") && !property.StartsWith("."))
			{
				XmlNode xmlNode = node[property];
				if (xmlNode != null)
				{
					return xmlNode.InnerText;
				}
				return node.InnerText;
			}
			if (property.StartsWith("/"))
			{
				property = "." + property;
			}
			XmlNode xmlNode2 = node.SelectSingleNode(property);
			if (xmlNode2 != null)
			{
				return xmlNode2.InnerText;
			}
		}
		return string.Empty;
	}

	public static void SetNodeProperty(XmlNode node, string property, string propertyValue)
	{
		if (property.StartsWith("@"))
		{
			property = property.Remove(0, 1);
			XmlAttribute xmlAttribute = node.Attributes[property];
			if (xmlAttribute == null)
			{
				xmlAttribute = node.OwnerDocument.CreateAttribute(property);
				node.Attributes.Append(xmlAttribute);
			}
			xmlAttribute.InnerText = propertyValue;
		}
		else if (property.StartsWith("/") || property.StartsWith("."))
		{
			if (property.StartsWith("/"))
			{
				property = "." + property;
			}
			XmlNode xmlNode = node.SelectSingleNode(property);
			if (xmlNode != null)
			{
				xmlNode.InnerText = propertyValue;
			}
		}
		else
		{
			XmlNode xmlNode2 = node[property];
			if (xmlNode2 != null)
			{
				xmlNode2.InnerText = propertyValue;
			}
		}
	}
}
