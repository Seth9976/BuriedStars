using System.Xml;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("DataMaker Xml")]
[Tooltip("Operates Maths on nodelist content.")]
public class XmlNodesMathOperator : DataMakerXmlActions
{
	public enum NodeListOperators
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Min,
		Max
	}

	[ActionSection("XML Source")]
	public FsmString nodeListReference;

	[ActionSection("Set up")]
	[Tooltip("Set to true to force iterating from the value of the index variable. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the first item.")]
	[UIHint(UIHint.Variable)]
	public NodeListOperators operation;

	[ActionSection("Result")]
	[Tooltip("Operation result as int")]
	[UIHint(UIHint.Variable)]
	public FsmInt resultAsInt;

	[Tooltip("Operation result as float")]
	[UIHint(UIHint.Variable)]
	public FsmFloat resultAsFloat;

	[Tooltip("Event to send when likely no nodelist was passed.")]
	public FsmEvent errorEvent;

	private XmlNodeList _nodeList;

	public override void Reset()
	{
		nodeListReference = null;
		operation = NodeListOperators.Add;
		resultAsInt = new FsmInt
		{
			UseVariable = true
		};
		resultAsFloat = new FsmFloat
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		_nodeList = DataMakerXmlUtils.XmlRetrieveNodeList(nodeListReference.Value);
		if (_nodeList == null)
		{
			base.Fsm.Event(errorEvent);
		}
		else
		{
			DoOperation();
		}
		Finish();
	}

	private void DoOperation()
	{
		if (_nodeList == null)
		{
			return;
		}
		float num = 0f;
		if (operation == NodeListOperators.Min)
		{
			num = float.MaxValue;
		}
		else if (operation == NodeListOperators.Max)
		{
			num = float.MinValue;
		}
		foreach (XmlNode node in _nodeList)
		{
			if (!float.TryParse(node.InnerText, out var result))
			{
				continue;
			}
			switch (operation)
			{
			case NodeListOperators.Add:
				num += result;
				break;
			case NodeListOperators.Subtract:
				num -= result;
				break;
			case NodeListOperators.Multiply:
				num *= result;
				break;
			case NodeListOperators.Divide:
				if (result != 0f)
				{
					num /= result;
				}
				break;
			case NodeListOperators.Min:
				if (result < num)
				{
					num = result;
				}
				break;
			case NodeListOperators.Max:
				if (result > num)
				{
					num = result;
				}
				break;
			}
		}
		if (!resultAsInt.IsNone)
		{
			resultAsInt.Value = (int)num;
		}
		if (!resultAsFloat.IsNone)
		{
			resultAsFloat.Value = num;
		}
	}
}
