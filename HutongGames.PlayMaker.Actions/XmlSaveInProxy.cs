using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Save an xml string into a DataMaker Xml Proxy")]
public class XmlSaveInProxy : DataMakerXmlNodeActions
{
	[Tooltip("The xml reference")]
	public FsmString storeReference;

	[Tooltip("The xml source")]
	public FsmString xmlSource;

	[RequiredField]
	[Tooltip("The gameObject with the DataMaker Xml Proxy component")]
	[CheckForComponent(typeof(DataMakerXmlProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the DataMaker Xml Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[ActionSection("Feedback")]
	public FsmEvent errorEvent;

	public override void Reset()
	{
		storeReference = new FsmString
		{
			UseVariable = true
		};
		xmlSource = new FsmString
		{
			UseVariable = true
		};
		gameObject = null;
		reference = null;
		errorEvent = null;
	}

	public override void OnEnter()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), ownerDefaultTarget, reference.Value, silent: false) as DataMakerXmlProxy;
		if (dataMakerXmlProxy != null)
		{
			if (!storeReference.IsNone)
			{
				dataMakerXmlProxy.InjectXmlNode(DataMakerXmlUtils.XmlRetrieveNode(storeReference.Value));
			}
			else if (!xmlSource.IsNone)
			{
				dataMakerXmlProxy.InjectXmlString(xmlSource.Value);
			}
		}
		else
		{
			base.Fsm.Event(errorEvent);
		}
		Finish();
	}
}
