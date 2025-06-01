using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[AttributeUsage(AttributeTargets.All)]
public class ExpectComponent : Attribute
{
	public readonly Type expectedComponentType;

	public ExpectComponent(Type type)
	{
		expectedComponentType = type;
	}

	public static Type GetTypeFromString(string typeString)
	{
		return Type.GetType(typeString);
	}

	public static string GetTypeAsString(Type type)
	{
		return type.AssemblyQualifiedName;
	}
}
