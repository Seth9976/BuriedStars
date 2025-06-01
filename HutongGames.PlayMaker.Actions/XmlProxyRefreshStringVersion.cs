using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Refresh the string version of the xml content of an XmlProxy.Costly operation, and typically only needed to refresh the proxy inspector")]
public class XmlProxyRefreshStringVersion : FsmStateAction
{
	[Tooltip("The DataMaker Xml Proxy component to refresh")]
	[CheckForComponent(typeof(DataMakerXmlProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the DataMaker Xml Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[UIHint(UIHint.Description)]
	public string descriptionArea = "Refreshing content is a costly operation.\nUse this to watch live xml content in the proxy";

	[Tooltip("Is true Only refresh content in editor.")]
	public bool OnlyInEditor;

	public override void Reset()
	{
		gameObject = null;
		reference = null;
		OnlyInEditor = true;
	}

	public override void OnEnter()
	{
		if (!Application.isEditor && OnlyInEditor)
		{
			Finish();
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget != null)
		{
			DataMakerXmlProxy dataMakerXmlProxy = DataMakerCore.GetDataMakerProxyPointer(typeof(DataMakerXmlProxy), ownerDefaultTarget, reference.Value, silent: false) as DataMakerXmlProxy;
			if (dataMakerXmlProxy != null)
			{
				dataMakerXmlProxy.RefreshStringVersion();
			}
		}
		Finish();
	}
}
