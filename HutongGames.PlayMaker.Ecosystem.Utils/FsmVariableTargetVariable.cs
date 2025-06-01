using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[AttributeUsage(AttributeTargets.All)]
public class FsmVariableTargetVariable : Attribute
{
	public string variable;

	public FsmVariableTargetVariable(string variable)
	{
		this.variable = variable;
	}
}
