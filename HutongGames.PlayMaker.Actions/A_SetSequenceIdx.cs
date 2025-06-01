using System;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Application)]
public class A_SetSequenceIdx : FsmStateAction
{
	[ObjectType(typeof(MonoBehaviour))]
	[Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
	public FsmObject behaviour;

	[Tooltip("Name of the method to call on the component")]
	public FsmString methodName;

	[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
	public FsmVar[] parameters;

	[ActionSection("Store Result")]
	[UIHint(UIHint.Variable)]
	[Tooltip("Store the result of the method call.")]
	public FsmVar storeResult;

	[Tooltip("Repeat every frame.")]
	public bool everyFrame;

	[Tooltip("Use the old manual editor UI.")]
	public bool manualUI;

	private FsmObject cachedBehaviour;

	private FsmString cachedMethodName;

	private Type cachedType;

	private MethodInfo cachedMethodInfo;

	private ParameterInfo[] cachedParameterInfo;

	private object[] parametersArray;

	private string errorString;

	public override void Reset()
	{
		behaviour = null;
		parameters = null;
		storeResult = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		parametersArray = new object[parameters.Length];
		DoMethodCall();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoMethodCall();
	}

	private void DoMethodCall()
	{
		if (NeedToUpdateCache() && !DoCache())
		{
			Finish();
			return;
		}
		object obj;
		if (cachedParameterInfo.Length == 0)
		{
			obj = cachedMethodInfo.Invoke(cachedBehaviour.Value, null);
		}
		else
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				FsmVar fsmVar = parameters[i];
				fsmVar.UpdateValue();
				if (fsmVar.Type == VariableType.Array)
				{
					fsmVar.UpdateValue();
					object[] array = fsmVar.GetValue() as object[];
					Type elementType = cachedParameterInfo[i].ParameterType.GetElementType();
					Array array2 = Array.CreateInstance(elementType, array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						array2.SetValue(array[j], j);
					}
					parametersArray[i] = array2;
				}
				else
				{
					fsmVar.UpdateValue();
					parametersArray[i] = fsmVar.GetValue();
				}
			}
			obj = cachedMethodInfo.Invoke(cachedBehaviour.Value, parametersArray);
		}
		if (storeResult != null && !storeResult.IsNone && storeResult.Type != VariableType.Unknown && obj != null)
		{
			storeResult.SetValue(obj);
		}
	}

	private bool NeedToUpdateCache()
	{
		return cachedBehaviour == null || cachedMethodName == null || cachedBehaviour.Value != behaviour.Value || cachedBehaviour.Name != behaviour.Name || cachedMethodName.Value != methodName.Value || cachedMethodName.Name != methodName.Name;
	}

	private void ClearCache()
	{
		cachedBehaviour = null;
		cachedMethodName = null;
		cachedType = null;
		cachedMethodInfo = null;
		cachedParameterInfo = null;
	}

	private bool DoCache()
	{
		ClearCache();
		errorString = string.Empty;
		if (behaviour.Value == null)
		{
			behaviour = GameObject.Find("PMEventScript").GetComponent<PMEventScript>();
			cachedBehaviour = behaviour;
		}
		cachedBehaviour = behaviour.Value as MonoBehaviour;
		if (cachedBehaviour == null)
		{
			errorString += "Behaviour is invalid!\n";
			Finish();
			return false;
		}
		cachedType = behaviour.Value.GetType();
		methodName.Value = "A_SetSequenceIdx";
		cachedMethodName = methodName;
		cachedMethodInfo = cachedType.GetMethod(methodName.Value);
		if (cachedMethodInfo == null)
		{
			errorString = errorString + "Method Name is invalid: " + methodName.Value + "\n";
			Finish();
			return false;
		}
		cachedParameterInfo = cachedMethodInfo.GetParameters();
		return true;
	}

	public override string ErrorCheck()
	{
		errorString = string.Empty;
		if (Application.isPlaying)
		{
			return errorString;
		}
		if (!DoCache())
		{
			return errorString;
		}
		if (cachedParameterInfo.Length != parameters.Length)
		{
			parameters = new FsmVar[cachedParameterInfo.Length];
			if (parameters.Length != cachedParameterInfo.Length)
			{
				return "Parameter count does not match method.\nMethod has " + cachedParameterInfo.Length + " parameters.\nYou specified " + parameters.Length + " paramaters.";
			}
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			Type parameterType = cachedParameterInfo[i].ParameterType;
			if (parameterType == typeof(float))
			{
				parameters[i].Type = VariableType.Float;
			}
			else if (parameterType == typeof(int))
			{
				parameters[i].Type = VariableType.Int;
			}
			else if (parameterType == typeof(bool))
			{
				parameters[i].Type = VariableType.Bool;
			}
			else if (parameterType == typeof(GameObject))
			{
				parameters[i].Type = VariableType.GameObject;
			}
			else if (parameterType == typeof(string))
			{
				parameters[i].Type = VariableType.String;
			}
			else if (parameterType == typeof(Vector2))
			{
				parameters[i].Type = VariableType.Vector2;
			}
			else if (parameterType == typeof(Vector3))
			{
				parameters[i].Type = VariableType.Vector3;
			}
			else if (parameterType == typeof(Color))
			{
				parameters[i].Type = VariableType.Color;
			}
			else if (parameterType == typeof(Rect))
			{
				parameters[i].Type = VariableType.Rect;
			}
			else if (parameterType == typeof(Material))
			{
				parameters[i].Type = VariableType.Material;
			}
			else if (parameterType == typeof(Texture))
			{
				parameters[i].Type = VariableType.Texture;
			}
			else if (parameterType == typeof(Quaternion))
			{
				parameters[i].Type = VariableType.Quaternion;
			}
			else if (parameterType == typeof(UnityEngine.Object))
			{
				parameters[i].Type = VariableType.Object;
			}
			else if (parameterType == typeof(object))
			{
				parameters[i].Type = VariableType.String;
			}
		}
		for (int j = 0; j < parameters.Length; j++)
		{
			FsmVar fsmVar = parameters[j];
			Type realType = fsmVar.RealType;
			Type parameterType2 = cachedParameterInfo[j].ParameterType;
			if (!object.ReferenceEquals(realType, parameterType2))
			{
				return string.Concat("Parameters do not match method signature.\nParameter ", j + 1, " (", realType, ") should be of type: ", parameterType2);
			}
		}
		if (object.ReferenceEquals(cachedMethodInfo.ReturnType, typeof(void)))
		{
			if (!string.IsNullOrEmpty(storeResult.variableName))
			{
				return "Method does not have return.\nSpecify 'none' in Store Result.";
			}
		}
		else if (!object.ReferenceEquals(cachedMethodInfo.ReturnType, storeResult.RealType))
		{
			return "Store Result is of the wrong type.\nIt should be of type: " + cachedMethodInfo.ReturnType;
		}
		return string.Empty;
	}
}
