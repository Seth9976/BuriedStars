using System.Collections.Generic;
using System.IO;
using System.Xml;

public class DataMakerXmlUtils
{
	private static Dictionary<string, XmlNode> xmlNodeLUT;

	private static Dictionary<string, XmlNodeList> xmlNodeListLUT;

	public static string lastError = string.Empty;

	public static void XmlStoreNode(XmlNode node, string reference)
	{
		if (string.IsNullOrEmpty(reference))
		{
		}
		if (xmlNodeLUT == null)
		{
			xmlNodeLUT = new Dictionary<string, XmlNode>();
		}
		xmlNodeLUT[reference] = node;
	}

	public static XmlNode XmlRetrieveNode(string reference)
	{
		if (string.IsNullOrEmpty(reference))
		{
		}
		if (xmlNodeLUT == null)
		{
			return null;
		}
		if (!xmlNodeLUT.ContainsKey(reference))
		{
			return null;
		}
		return xmlNodeLUT[reference];
	}

	public static void XmlStoreNodeList(XmlNodeList nodeList, string reference)
	{
		if (string.IsNullOrEmpty(reference))
		{
		}
		if (xmlNodeListLUT == null)
		{
			xmlNodeListLUT = new Dictionary<string, XmlNodeList>();
		}
		xmlNodeListLUT[reference] = nodeList;
	}

	public static XmlNodeList XmlRetrieveNodeList(string reference)
	{
		if (string.IsNullOrEmpty(reference))
		{
		}
		return xmlNodeListLUT[reference];
	}

	public static XmlNode StringToXmlNode(string content)
	{
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.LoadXml(content);
		}
		catch (XmlException ex)
		{
			lastError = ex.Message;
			return null;
		}
		return xmlDocument.DocumentElement;
	}

	public static string XmlNodeListToString(XmlNodeList nodeList)
	{
		return XmlNodeListToString(nodeList, 2);
	}

	public static string XmlNodeListToString(XmlNodeList nodeList, int indentation)
	{
		if (nodeList == null)
		{
			return "-- NULL --";
		}
		using StringWriter stringWriter = new StringWriter();
		using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
		{
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = indentation;
			xmlTextWriter.WriteRaw("<result>");
			foreach (XmlNode node in nodeList)
			{
				node.WriteTo(xmlTextWriter);
			}
			xmlTextWriter.WriteRaw("</result>");
		}
		return stringWriter.ToString();
	}

	public static string XmlNodeToString(XmlNode node)
	{
		return XmlNodeToString(node, 2);
	}

	public static string XmlNodeToString(XmlNode node, int indentation)
	{
		if (node == null)
		{
			return "-- NULL --";
		}
		using StringWriter stringWriter = new StringWriter();
		using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
		{
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = indentation;
			node.WriteTo(xmlTextWriter);
		}
		return stringWriter.ToString();
	}
}
