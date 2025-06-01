using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Get the next node properties in a nodelist. \nEach time this action is called it gets the next node.This lets you quickly loop through all the nodelist to perform actions per nodes.")]
public class XmlGetNextNodeListProperties : DataMakerXmlActions
{
	[ActionSection("XML Source")]
	public FsmString nodeListReference;

	[ActionSection("Set up")]
	[Tooltip("Set to true to force iterating from the value of the index variable. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the first item.")]
	[UIHint(UIHint.Variable)]
	public FsmBool reset;

	[Tooltip("Event to send for looping.")]
	public FsmEvent loopEvent;

	[Tooltip("Event to send when there are no more nodes.")]
	public FsmEvent finishedEvent;

	[ActionSection("Result")]
	[Tooltip("Save the node into memory")]
	public FsmString indexedNodeReference;

	[Tooltip("The index into the node List")]
	[UIHint(UIHint.Variable)]
	public FsmInt index;

	public FsmXmlPropertiesStorage storeProperties;

	[ActionSection("Properties Storage")]
	public FsmXmlProperty[] storeNodeProperties;

	private int nextItemIndex;

	private bool noMoreItems;

	private XmlNodeList _nodeList;

	public override void Reset()
	{
		nodeListReference = null;
		storeProperties = null;
		storeNodeProperties = null;
		reset = null;
		finishedEvent = null;
		loopEvent = null;
		indexedNodeReference = new FsmString
		{
			UseVariable = true
		};
		index = null;
	}

	public override void OnEnter()
	{
		if (reset.Value)
		{
			reset.Value = false;
			nextItemIndex = index.Value;
			_nodeList = null;
		}
		if (_nodeList == null)
		{
			_nodeList = DataMakerXmlUtils.XmlRetrieveNodeList(nodeListReference.Value);
			if (_nodeList == null)
			{
				base.Fsm.Event(finishedEvent);
				return;
			}
		}
		DoGetNextNode();
		Finish();
	}

	private void DoGetNextNode()
	{
		int count = _nodeList.Count;
		if (nextItemIndex >= count)
		{
			nextItemIndex = 0;
			base.Fsm.Event(finishedEvent);
			return;
		}
		if (!string.IsNullOrEmpty(indexedNodeReference.Value))
		{
			DataMakerXmlUtils.XmlStoreNode(_nodeList[nextItemIndex], indexedNodeReference.Value);
		}
		index.Value = nextItemIndex;
		if (storeNodeProperties.Length > 0)
		{
			FsmXmlProperty.StoreNodeProperties(base.Fsm, _nodeList[nextItemIndex], storeNodeProperties);
		}
		else
		{
			storeProperties.StoreNodeProperties(base.Fsm, _nodeList[nextItemIndex]);
		}
		if (nextItemIndex >= count)
		{
			base.Fsm.Event(finishedEvent);
			nextItemIndex = 0;
			return;
		}
		nextItemIndex++;
		if (loopEvent != null)
		{
			base.Fsm.Event(loopEvent);
		}
	}
}
