using System.Xml;
using HutongGames.PlayMaker;
using UnityEngine;

public class FsmXmlSource : FsmStateAction
{
	public string[] sourceTypes = new string[5] { "Plain Text", "Text Asset", "Use Variable", "Use Proxy", "In Memory" };

	public int sourceSelection;

	public TextAsset sourcetextAsset;

	public FsmString sourceString;

	public FsmGameObject sourceProxyGameObject;

	public FsmString sourceProxyReference;

	public FsmString inMemoryReference;

	public bool _minimized;

	public Vector2 _scroll;

	public bool _sourcePreview = true;

	public bool _sourceEdit = true;

	public XmlNode Value
	{
		get
		{
			switch (sourceSelection)
			{
			case 0:
			case 2:
				return GetXmlNodeFromString(sourceString.Value);
			case 1:
				if (sourcetextAsset == null)
				{
					return null;
				}
				return GetXmlNodeFromString(sourcetextAsset.text);
			case 3:
			{
				DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), sourceProxyGameObject.Value, sourceProxyReference.Value, silent: false) as DataMakerXmlProxy;
				if (dataMakerXmlProxy != null)
				{
					return dataMakerXmlProxy.xmlNode;
				}
				break;
			}
			case 4:
				return DataMakerXmlUtils.XmlRetrieveNode(inMemoryReference.Value);
			}
			return null;
		}
	}

	private XmlNode GetXmlNodeFromString(string source)
	{
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.LoadXml(source);
		}
		catch (XmlException)
		{
			return null;
		}
		return xmlDocument.DocumentElement;
	}
}
