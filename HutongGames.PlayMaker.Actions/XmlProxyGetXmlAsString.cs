using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Gets the xml content of an XmlProxy as a string")]
public class XmlProxyGetXmlAsString : FsmStateAction
{
	[Tooltip("The DataMaker Xml Proxy component to refresh")]
	[CheckForComponent(typeof(DataMakerXmlProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the DataMaker Xml Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[Tooltip("Force the xml to be parsed as string. This is a costly operation")]
	public FsmBool refreshContentFirst;

	[ActionSection("Result")]
	[Tooltip("The Xml content as string")]
	[RequiredField]
	public FsmString xmlString;

	public override void Reset()
	{
		gameObject = null;
		refreshContentFirst = true;
		reference = null;
		xmlString = null;
	}

	public override void OnEnter()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget != null)
		{
			DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), ownerDefaultTarget, reference.Value, silent: false) as DataMakerXmlProxy;
			if (dataMakerXmlProxy != null)
			{
				if (refreshContentFirst.Value)
				{
					dataMakerXmlProxy.RefreshStringVersion();
				}
				xmlString.Value = dataMakerXmlProxy.content;
			}
		}
		Finish();
	}
}
