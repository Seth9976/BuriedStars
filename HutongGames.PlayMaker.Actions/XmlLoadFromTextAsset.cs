using System.Xml;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Load an xml string from a textAsset")]
public class XmlLoadFromTextAsset : DataMakerXmlNodeActions
{
	[Tooltip("The xml text")]
	public TextAsset source;

	[ActionSection("Result")]
	[Tooltip("Save as xml reference")]
	public FsmString storeReference;

	[Tooltip("Save in DataMaker Xml Proxy component")]
	[CheckForComponent(typeof(DataMakerXmlProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the DataMaker Xml Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[Tooltip("Save as string")]
	public FsmString xmlString;

	public FsmEvent errorEvent;

	public override void Reset()
	{
		source = null;
		storeReference = new FsmString
		{
			UseVariable = true
		};
		gameObject = null;
		reference = new FsmString
		{
			UseVariable = true
		};
		xmlString = new FsmString
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		LoadFromText();
		Finish();
	}

	private void LoadFromText()
	{
		XmlNode xmlNode = DataMakerXmlUtils.StringToXmlNode(source.text);
		if (xmlNode == null)
		{
			Fsm.EventData.StringData = DataMakerXmlUtils.lastError;
			base.Fsm.Event(errorEvent);
			return;
		}
		if (!storeReference.IsNone)
		{
			DataMakerXmlUtils.XmlStoreNode(xmlNode, storeReference.Value);
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), ownerDefaultTarget, reference.Value, silent: false) as DataMakerXmlProxy;
		if (dataMakerXmlProxy != null)
		{
			dataMakerXmlProxy.InjectXmlNode(xmlNode);
		}
		if (!xmlString.IsNone)
		{
			xmlString.Value = source.text;
		}
		Finish();
	}
}
