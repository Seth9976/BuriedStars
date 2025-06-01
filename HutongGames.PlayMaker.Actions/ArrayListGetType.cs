using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/ArrayList")]
[Tooltip("Gets the type at index from a PlayMaker ArrayList Proxy component")]
public class ArrayListGetType : ArrayListActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
	[CheckForComponent(typeof(PlayMakerArrayListProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[UIHint(UIHint.FsmInt)]
	[Tooltip("The index to retrieve the item from")]
	public FsmInt atIndex;

	[ActionSection("Result")]
	[RequiredField]
	[UIHint(UIHint.Variable)]
	[ObjectType(typeof(PlayMakerCollectionProxy.VariableEnum))]
	public FsmEnum type;

	[UIHint(UIHint.FsmEvent)]
	[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
	public FsmEvent failureEvent;

	public override void Reset()
	{
		atIndex = null;
		gameObject = null;
		failureEvent = null;
		type = null;
	}

	public override void OnEnter()
	{
		if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			GetTypeAtIndex();
		}
		Finish();
	}

	public void GetTypeAtIndex()
	{
		if (!isProxyValid())
		{
			return;
		}
		if (type.IsNone)
		{
			base.Fsm.Event(failureEvent);
			return;
		}
		try
		{
			if (proxy.arrayList[atIndex.Value] == null)
			{
				type.Value = proxy.preFillType;
			}
			else
			{
				type.Value = PlayMakerCollectionProxy.GetObjectVariableType(proxy.arrayList[atIndex.Value]);
			}
		}
		catch (Exception)
		{
			base.Fsm.Event(failureEvent);
		}
	}
}
