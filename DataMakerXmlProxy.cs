using System;
using System.Xml;
using UnityEngine;

public class DataMakerXmlProxy : DataMakerProxyBase
{
	public static bool delegationActive = true;

	public string storeInMemory = string.Empty;

	public bool useSource;

	public TextAsset XmlTextAsset;

	private XmlNode _xmlNode;

	[NonSerialized]
	[HideInInspector]
	public bool isDirty;

	[NonSerialized]
	[HideInInspector]
	public string content;

	public PlayMakerFSM FsmEventTarget;

	[HideInInspector]
	public XmlNode xmlNode
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

	private void Awake()
	{
		if (useSource && XmlTextAsset != null)
		{
			InjectXmlString(XmlTextAsset.text);
		}
		RegisterEventHandlers();
	}

	public void RefreshStringVersion()
	{
		content = DataMakerXmlUtils.XmlNodeToString(xmlNode);
		isDirty = true;
	}

	public void InjectXmlNode(XmlNode node)
	{
		xmlNode = node;
		RegisterEventHandlers();
	}

	public void InjectXmlNodeList(XmlNodeList nodeList)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlNode = xmlDocument.CreateElement("root");
		foreach (XmlNode node in nodeList)
		{
			xmlNode.AppendChild(node);
		}
		RegisterEventHandlers();
	}

	public void InjectXmlString(string source)
	{
		xmlNode = DataMakerXmlUtils.StringToXmlNode(source);
		RegisterEventHandlers();
	}

	private void UnregisterEventHandlers()
	{
	}

	private void RegisterEventHandlers()
	{
		if (xmlNode != null)
		{
			xmlNode.OwnerDocument.NodeChanged += NodeTouchedHandler;
			xmlNode.OwnerDocument.NodeInserted += NodeTouchedHandler;
			xmlNode.OwnerDocument.NodeRemoved += NodeTouchedHandler;
		}
	}

	private void NodeTouchedHandler(object src, XmlNodeChangedEventArgs args)
	{
		if (!(FsmEventTarget == null) && delegationActive)
		{
			if (args.Action == XmlNodeChangedAction.Insert)
			{
				PlayMakerUtils.SendEventToGameObject(FsmEventTarget, FsmEventTarget.gameObject, "XML / INSERTED");
			}
			else if (args.Action == XmlNodeChangedAction.Change)
			{
				PlayMakerUtils.SendEventToGameObject(FsmEventTarget, FsmEventTarget.gameObject, "XML / CHANGED");
			}
			else if (args.Action == XmlNodeChangedAction.Remove)
			{
				PlayMakerUtils.SendEventToGameObject(FsmEventTarget, FsmEventTarget.gameObject, "XML / REMOVED");
			}
		}
	}
}
