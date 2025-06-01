using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[AttributeUsage(AttributeTargets.All)]
public class FsmVariableType : Attribute
{
	public VariableType variableType;

	public FsmVariableType(VariableType variableType)
	{
		this.variableType = variableType;
	}
}
