using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

public class ButtonAttribute : PropertyAttribute
{
	public string methodName;

	public string buttonName;

	public bool useValue;

	public BindingFlags flags;

	public ButtonAttribute(string methodName, string buttonName, bool useValue, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
	{
		this.methodName = methodName;
		this.buttonName = buttonName;
		this.useValue = useValue;
		this.flags = flags;
	}

	public ButtonAttribute(string methodName, bool useValue, BindingFlags flags)
		: this(methodName, methodName, useValue, flags)
	{
	}

	public ButtonAttribute(string methodName, bool useValue)
		: this(methodName, methodName, useValue)
	{
	}

	public ButtonAttribute(string methodName, string buttonName, BindingFlags flags)
		: this(methodName, buttonName, useValue: false, flags)
	{
	}

	public ButtonAttribute(string methodName, string buttonName)
		: this(methodName, buttonName, useValue: false)
	{
	}

	public ButtonAttribute(string methodName, BindingFlags flags)
		: this(methodName, methodName, useValue: false, flags)
	{
	}

	public ButtonAttribute(string methodName)
		: this(methodName, methodName, useValue: false)
	{
	}
}
