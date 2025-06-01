using System;
using System.Text.RegularExpressions;
using HutongGames.PlayMaker;
using UnityEngine;

public class PlayMakerUtils
{
	public static PlayMakerFSM FsmEventSender;

	public static Quaternion ConvertToQuaternion(object value, Quaternion defaultValue = default(Quaternion))
	{
		if (value is Quaternion)
		{
			return (Quaternion)value;
		}
		return defaultValue;
	}

	public static Rect ConvertToRect(object value, Rect defaultValue = default(Rect))
	{
		if (value is Rect)
		{
			return (Rect)value;
		}
		return defaultValue;
	}

	public static Color ConvertToColor(object value, Color defaultValue = default(Color))
	{
		if (value is Color)
		{
			return (Color)value;
		}
		return defaultValue;
	}

	public static Vector3 ConvertToVector3(object value, Vector3 defaultValue = default(Vector3))
	{
		if (value is Vector3)
		{
			return (Vector3)value;
		}
		return defaultValue;
	}

	public static Vector2 ConvertToVector2(object value, Vector2 defaultValue = default(Vector2))
	{
		if (value is Vector2)
		{
			return (Vector2)value;
		}
		return defaultValue;
	}

	public static Vector4 ConvertToVector2(object value, Vector4 defaultValue = default(Vector4))
	{
		if (value is Vector4)
		{
			return (Vector4)value;
		}
		return defaultValue;
	}

	public static PlayMakerFSM GetFsmEventSender()
	{
		if (FsmEventSender == null)
		{
			FsmEventSender = new GameObject("PlayMaker Send Event Proxy").AddComponent<PlayMakerFSM>();
			FsmEventSender.FsmName = "Send Event Proxy";
			FsmEventSender.FsmDescription = "This Fsm was created at runtime, because a script or component is willing to send a PlayMaker event";
		}
		return FsmEventSender;
	}

	public static void CreateIfNeededGlobalEvent(string globalEventName)
	{
		if (!FsmEvent.IsEventGlobal(globalEventName))
		{
			FsmEvent fsmEvent = new FsmEvent(globalEventName);
			fsmEvent.IsGlobal = true;
			FsmEvent.AddFsmEvent(fsmEvent);
		}
	}

	public static void SendEventToTarget(PlayMakerFSM fromFsm, FsmEventTarget target, string fsmEvent, FsmEventData eventData)
	{
		if (fromFsm == null)
		{
			fromFsm = GetFsmEventSender();
		}
		if (eventData != null)
		{
			Fsm.EventData = eventData;
		}
		if (!(fromFsm == null))
		{
			fromFsm.Fsm.Event(target, fsmEvent);
		}
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren, null);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren: false, null);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, FsmEventData eventData)
	{
		SendEventToGameObject(fromFsm, target, fsmEvent, includeChildren: false, eventData);
	}

	public static void SendEventToGameObject(PlayMakerFSM fromFsm, GameObject target, string fsmEvent, bool includeChildren, FsmEventData eventData)
	{
		if (fromFsm == null)
		{
			fromFsm = GetFsmEventSender();
		}
		if (eventData != null)
		{
			Fsm.EventData = eventData;
		}
		if (!(fromFsm == null))
		{
			FsmEventTarget fsmEventTarget = new FsmEventTarget();
			fsmEventTarget.excludeSelf = false;
			fsmEventTarget.sendToChildren = includeChildren;
			fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
			FsmOwnerDefault fsmOwnerDefault = new FsmOwnerDefault();
			fsmOwnerDefault.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
			fsmOwnerDefault.GameObject = new FsmGameObject();
			fsmOwnerDefault.GameObject.Value = target;
			fsmEventTarget.gameObject = fsmOwnerDefault;
			fromFsm.Fsm.Event(fsmEventTarget, fsmEvent);
		}
	}

	public static bool DoesTargetImplementsEvent(FsmEventTarget target, string eventName)
	{
		if (target.target == FsmEventTarget.EventTarget.BroadcastAll)
		{
			return FsmEvent.IsEventGlobal(eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.FSMComponent)
		{
			return DoesFsmImplementsEvent(target.fsmComponent, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObject)
		{
			return DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.GameObjectFSM)
		{
			return DoesGameObjectImplementsEvent(target.gameObject.GameObject.Value, target.fsmName.Value, eventName);
		}
		if (target.target == FsmEventTarget.EventTarget.Self)
		{
		}
		if (target.target == FsmEventTarget.EventTarget.SubFSMs)
		{
		}
		if (target.target == FsmEventTarget.EventTarget.HostFSM)
		{
		}
		return false;
	}

	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmEvent, bool includeChildren = false)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		if (includeChildren)
		{
			PlayMakerFSM[] componentsInChildren = go.GetComponentsInChildren<PlayMakerFSM>();
			foreach (PlayMakerFSM fsm in componentsInChildren)
			{
				if (DoesFsmImplementsEvent(fsm, fsmEvent))
				{
					return true;
				}
			}
		}
		else
		{
			PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
			foreach (PlayMakerFSM fsm2 in components)
			{
				if (DoesFsmImplementsEvent(fsm2, fsmEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool DoesGameObjectImplementsEvent(GameObject go, string fsmName, string fsmEvent)
	{
		if (go == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(fsmName);
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if (flag && object.Equals(playMakerFSM, fsmName) && DoesFsmImplementsEvent(playMakerFSM, fsmEvent))
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesFsmImplementsEvent(PlayMakerFSM fsm, string fsmEvent)
	{
		if (fsm == null || string.IsNullOrEmpty(fsmEvent))
		{
			return false;
		}
		FsmTransition[] fsmGlobalTransitions = fsm.FsmGlobalTransitions;
		foreach (FsmTransition fsmTransition in fsmGlobalTransitions)
		{
			if (fsmTransition.EventName.Equals(fsmEvent))
			{
				return true;
			}
		}
		FsmState[] fsmStates = fsm.FsmStates;
		foreach (FsmState fsmState in fsmStates)
		{
			FsmTransition[] transitions = fsmState.Transitions;
			foreach (FsmTransition fsmTransition2 in transitions)
			{
				if (fsmTransition2.EventName.Equals(fsmEvent))
				{
					return true;
				}
			}
		}
		return false;
	}

	public FsmEvent CreateGlobalEvent(string EventName)
	{
		bool ExistsAlready;
		return CreateGlobalEvent(EventName, out ExistsAlready);
	}

	public FsmEvent CreateGlobalEvent(string EventName, out bool ExistsAlready)
	{
		FsmEvent fsmEvent = FsmEvent.GetFsmEvent(EventName);
		ExistsAlready = FsmEvent.EventListContains(EventName);
		if (ExistsAlready)
		{
			if (fsmEvent != null && fsmEvent.IsGlobal)
			{
				fsmEvent.IsGlobal = true;
			}
			return fsmEvent;
		}
		fsmEvent = new FsmEvent(EventName);
		fsmEvent.IsGlobal = true;
		FsmEvent.AddFsmEvent(fsmEvent);
		return fsmEvent;
	}

	public static PlayMakerFSM FindFsmOnGameObject(GameObject go, string fsmName)
	{
		if (go == null || string.IsNullOrEmpty(fsmName))
		{
			return null;
		}
		PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
		foreach (PlayMakerFSM playMakerFSM in components)
		{
			if (string.Equals(playMakerFSM.FsmName, fsmName))
			{
				return playMakerFSM;
			}
		}
		return null;
	}

	public static void RefreshValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm != null && fsmVar != null && fsmVar.useVariable)
		{
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmInt(fsmVar.variableName));
				break;
			case VariableType.Float:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmFloat(fsmVar.variableName));
				break;
			case VariableType.Bool:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmBool(fsmVar.variableName));
				break;
			case VariableType.Color:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmColor(fsmVar.variableName));
				break;
			case VariableType.Quaternion:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName));
				break;
			case VariableType.Rect:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmRect(fsmVar.variableName));
				break;
			case VariableType.Vector2:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector2(fsmVar.variableName));
				break;
			case VariableType.Vector3:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
				break;
			case VariableType.Texture:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmVector3(fsmVar.variableName));
				break;
			case VariableType.Material:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmMaterial(fsmVar.variableName));
				break;
			case VariableType.String:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmString(fsmVar.variableName));
				break;
			case VariableType.GameObject:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmGameObject(fsmVar.variableName));
				break;
			case VariableType.Enum:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmEnum(fsmVar.variableName));
				break;
			case VariableType.Array:
				fsmVar.GetValueFrom(fromFsm.Variables.GetFsmArray(fsmVar.variableName));
				break;
			case VariableType.Object:
				break;
			}
		}
	}

	public static object GetValueFromFsmVar(Fsm fromFsm, FsmVar fsmVar)
	{
		if (fromFsm == null)
		{
			return null;
		}
		if (fsmVar == null)
		{
			return null;
		}
		if (fsmVar.useVariable)
		{
			string variableName = fsmVar.variableName;
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				return fromFsm.Variables.GetFsmInt(variableName).Value;
			case VariableType.Float:
				return fromFsm.Variables.GetFsmFloat(variableName).Value;
			case VariableType.Bool:
				return fromFsm.Variables.GetFsmBool(variableName).Value;
			case VariableType.Color:
				return fromFsm.Variables.GetFsmColor(variableName).Value;
			case VariableType.Quaternion:
				return fromFsm.Variables.GetFsmQuaternion(variableName).Value;
			case VariableType.Rect:
				return fromFsm.Variables.GetFsmRect(variableName).Value;
			case VariableType.Vector2:
				return fromFsm.Variables.GetFsmVector2(variableName).Value;
			case VariableType.Vector3:
				return fromFsm.Variables.GetFsmVector3(variableName).Value;
			case VariableType.Texture:
				return fromFsm.Variables.GetFsmTexture(variableName).Value;
			case VariableType.Material:
				return fromFsm.Variables.GetFsmMaterial(variableName).Value;
			case VariableType.String:
				return fromFsm.Variables.GetFsmString(variableName).Value;
			case VariableType.GameObject:
				return fromFsm.Variables.GetFsmGameObject(variableName).Value;
			case VariableType.Object:
				return fromFsm.Variables.GetFsmObject(variableName).Value;
			case VariableType.Enum:
				return fromFsm.Variables.GetFsmEnum(variableName).Value;
			case VariableType.Array:
				return fromFsm.Variables.GetFsmArray(variableName).Values;
			}
		}
		else
		{
			switch (fsmVar.Type)
			{
			case VariableType.Int:
				return fsmVar.intValue;
			case VariableType.Float:
				return fsmVar.floatValue;
			case VariableType.Bool:
				return fsmVar.boolValue;
			case VariableType.Color:
				return fsmVar.colorValue;
			case VariableType.Quaternion:
				return fsmVar.quaternionValue;
			case VariableType.Rect:
				return fsmVar.rectValue;
			case VariableType.Vector2:
				return fsmVar.vector2Value;
			case VariableType.Vector3:
				return fsmVar.vector3Value;
			case VariableType.Texture:
				return fsmVar.textureValue;
			case VariableType.Material:
				return fsmVar.materialValue;
			case VariableType.String:
				return fsmVar.stringValue;
			case VariableType.GameObject:
				return fsmVar.gameObjectValue;
			case VariableType.Object:
				return fsmVar.objectReference;
			case VariableType.Enum:
				return fsmVar.EnumValue;
			case VariableType.Array:
				return fsmVar.arrayValue;
			}
		}
		return null;
	}

	public static bool ApplyValueToFsmVar(Fsm fromFsm, FsmVar fsmVar, object[] value)
	{
		if (fromFsm == null)
		{
			return false;
		}
		if (fsmVar == null)
		{
			return false;
		}
		FsmArray fsmArray;
		if (value == null || value.Length == 0)
		{
			if (fsmVar.Type == VariableType.Array)
			{
				fsmArray = fromFsm.Variables.GetFsmArray(fsmVar.variableName);
				fsmArray.Reset();
			}
			return true;
		}
		if (fsmVar.Type != VariableType.Array)
		{
			return false;
		}
		fsmArray = fromFsm.Variables.GetFsmArray(fsmVar.variableName);
		fsmArray.Values = value;
		return true;
	}

	public static bool ApplyValueToFsmVar(Fsm fromFsm, FsmVar fsmVar, object value)
	{
		if (fromFsm == null)
		{
			return false;
		}
		if (fsmVar == null)
		{
			return false;
		}
		if (value == null)
		{
			if (fsmVar.Type == VariableType.Bool)
			{
				FsmBool fsmBool = fromFsm.Variables.GetFsmBool(fsmVar.variableName);
				fsmBool.Value = false;
			}
			else if (fsmVar.Type == VariableType.Color)
			{
				FsmColor fsmColor = fromFsm.Variables.GetFsmColor(fsmVar.variableName);
				fsmColor.Value = Color.black;
			}
			else if (fsmVar.Type == VariableType.Int)
			{
				FsmInt fsmInt = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
				fsmInt.Value = 0;
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				FsmFloat fsmFloat = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
				fsmFloat.Value = 0f;
			}
			else if (fsmVar.Type == VariableType.GameObject)
			{
				FsmGameObject fsmGameObject = fromFsm.Variables.GetFsmGameObject(fsmVar.variableName);
				fsmGameObject.Value = null;
			}
			else if (fsmVar.Type == VariableType.Material)
			{
				FsmMaterial fsmMaterial = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
				fsmMaterial.Value = null;
			}
			else if (fsmVar.Type == VariableType.Object)
			{
				FsmObject fsmObject = fromFsm.Variables.GetFsmObject(fsmVar.variableName);
				fsmObject.Value = null;
			}
			else if (fsmVar.Type == VariableType.Quaternion)
			{
				FsmQuaternion fsmQuaternion = fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName);
				fsmQuaternion.Value = Quaternion.identity;
			}
			else if (fsmVar.Type == VariableType.Rect)
			{
				FsmRect fsmRect = fromFsm.Variables.GetFsmRect(fsmVar.variableName);
				fsmRect.Value = new Rect(0f, 0f, 0f, 0f);
			}
			else if (fsmVar.Type == VariableType.String)
			{
				FsmString fsmString = fromFsm.Variables.GetFsmString(fsmVar.variableName);
				fsmString.Value = string.Empty;
			}
			else if (fsmVar.Type == VariableType.String)
			{
				FsmTexture fsmTexture = fromFsm.Variables.GetFsmTexture(fsmVar.variableName);
				fsmTexture.Value = null;
			}
			else if (fsmVar.Type == VariableType.Vector2)
			{
				FsmVector2 fsmVector = fromFsm.Variables.GetFsmVector2(fsmVar.variableName);
				fsmVector.Value = Vector2.zero;
			}
			else if (fsmVar.Type == VariableType.Vector3)
			{
				FsmVector3 fsmVector2 = fromFsm.Variables.GetFsmVector3(fsmVar.variableName);
				fsmVector2.Value = Vector3.zero;
			}
			else if (fsmVar.Type == VariableType.Enum)
			{
				FsmEnum fsmEnum = fromFsm.Variables.GetFsmEnum(fsmVar.variableName);
				fsmEnum.ResetValue();
			}
			else if (fsmVar.Type == VariableType.Array)
			{
				FsmArray fsmArray = fromFsm.Variables.GetFsmArray(fsmVar.variableName);
				fsmArray.Reset();
			}
			return true;
		}
		Type type = value.GetType();
		Type type2 = null;
		switch (fsmVar.Type)
		{
		case VariableType.Int:
			type2 = typeof(int);
			break;
		case VariableType.Float:
			type2 = typeof(float);
			break;
		case VariableType.Bool:
			type2 = typeof(bool);
			break;
		case VariableType.Color:
			type2 = typeof(Color);
			break;
		case VariableType.GameObject:
			type2 = typeof(GameObject);
			break;
		case VariableType.Quaternion:
			type2 = typeof(Quaternion);
			break;
		case VariableType.Rect:
			type2 = typeof(Rect);
			break;
		case VariableType.String:
			type2 = typeof(string);
			break;
		case VariableType.Texture:
			type2 = typeof(Texture2D);
			break;
		case VariableType.Vector2:
			type2 = typeof(Vector2);
			break;
		case VariableType.Vector3:
			type2 = typeof(Vector3);
			break;
		case VariableType.Object:
			type2 = typeof(UnityEngine.Object);
			break;
		case VariableType.Material:
			type2 = typeof(Material);
			break;
		case VariableType.Enum:
			type2 = typeof(Enum);
			break;
		case VariableType.Array:
			type2 = typeof(Array);
			break;
		}
		bool flag = true;
		if (!type2.Equals(type))
		{
			flag = false;
			if (type2.Equals(typeof(UnityEngine.Object)))
			{
				flag = true;
			}
			if (type2.Equals(typeof(Enum)))
			{
				flag = true;
			}
			if (!flag)
			{
				if (type.Equals(typeof(ProceduralMaterial)))
				{
					flag = true;
				}
				if (type.Equals(typeof(double)))
				{
					flag = true;
				}
				if (type.Equals(typeof(long)))
				{
					flag = true;
				}
				if (type.Equals(typeof(byte)))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		if (type == typeof(bool))
		{
			FsmBool fsmBool2 = fromFsm.Variables.GetFsmBool(fsmVar.variableName);
			fsmBool2.Value = (bool)value;
		}
		else if (type == typeof(Color))
		{
			FsmColor fsmColor2 = fromFsm.Variables.GetFsmColor(fsmVar.variableName);
			fsmColor2.Value = (Color)value;
		}
		else if (type == typeof(int))
		{
			FsmInt fsmInt2 = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
			fsmInt2.Value = Convert.ToInt32(value);
		}
		else if (type == typeof(byte))
		{
			FsmInt fsmInt3 = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
			fsmInt3.Value = Convert.ToInt32(value);
		}
		else if (type == typeof(long))
		{
			if (fsmVar.Type == VariableType.Int)
			{
				FsmInt fsmInt4 = fromFsm.Variables.GetFsmInt(fsmVar.variableName);
				fsmInt4.Value = Convert.ToInt32(value);
			}
			else if (fsmVar.Type == VariableType.Float)
			{
				FsmFloat fsmFloat2 = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
				fsmFloat2.Value = Convert.ToSingle(value);
			}
		}
		else if (type == typeof(float))
		{
			FsmFloat fsmFloat3 = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
			fsmFloat3.Value = (float)value;
		}
		else if (type == typeof(double))
		{
			FsmFloat fsmFloat4 = fromFsm.Variables.GetFsmFloat(fsmVar.variableName);
			fsmFloat4.Value = Convert.ToSingle(value);
		}
		else if (type == typeof(GameObject))
		{
			FsmGameObject fsmGameObject2 = fromFsm.Variables.GetFsmGameObject(fsmVar.variableName);
			fsmGameObject2.Value = (GameObject)value;
		}
		else if (type == typeof(Material))
		{
			FsmMaterial fsmMaterial2 = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
			fsmMaterial2.Value = (Material)value;
		}
		else if (type == typeof(ProceduralMaterial))
		{
			FsmMaterial fsmMaterial3 = fromFsm.Variables.GetFsmMaterial(fsmVar.variableName);
			fsmMaterial3.Value = (ProceduralMaterial)value;
		}
		else if (type == typeof(UnityEngine.Object) || type2 == typeof(UnityEngine.Object))
		{
			FsmObject fsmObject2 = fromFsm.Variables.GetFsmObject(fsmVar.variableName);
			fsmObject2.Value = (UnityEngine.Object)value;
		}
		else if (type == typeof(Quaternion))
		{
			FsmQuaternion fsmQuaternion2 = fromFsm.Variables.GetFsmQuaternion(fsmVar.variableName);
			fsmQuaternion2.Value = (Quaternion)value;
		}
		else if (type == typeof(Rect))
		{
			FsmRect fsmRect2 = fromFsm.Variables.GetFsmRect(fsmVar.variableName);
			fsmRect2.Value = (Rect)value;
		}
		else if (type == typeof(string))
		{
			FsmString fsmString2 = fromFsm.Variables.GetFsmString(fsmVar.variableName);
			fsmString2.Value = (string)value;
		}
		else if (type == typeof(Texture2D))
		{
			FsmTexture fsmTexture2 = fromFsm.Variables.GetFsmTexture(fsmVar.variableName);
			fsmTexture2.Value = (Texture2D)value;
		}
		else if (type == typeof(Vector2))
		{
			FsmVector2 fsmVector3 = fromFsm.Variables.GetFsmVector2(fsmVar.variableName);
			fsmVector3.Value = (Vector2)value;
		}
		else if (type == typeof(Vector3))
		{
			FsmVector3 fsmVector4 = fromFsm.Variables.GetFsmVector3(fsmVar.variableName);
			fsmVector4.Value = (Vector3)value;
		}
		else if (value is Enum)
		{
			FsmEnum fsmEnum2 = fromFsm.Variables.GetFsmEnum(fsmVar.variableName);
			fsmEnum2.Value = (Enum)value;
		}
		return true;
	}

	public static float GetFloatFromObject(object _obj, VariableType targetType, bool fastProcessingIfPossible)
	{
		switch (targetType)
		{
		case VariableType.Float:
		case VariableType.Int:
			return Convert.ToSingle(_obj);
		case VariableType.Vector2:
		{
			Vector2 vector = (Vector2)_obj;
			if (vector != Vector2.zero)
			{
				return (!fastProcessingIfPossible) ? vector.magnitude : vector.sqrMagnitude;
			}
			break;
		}
		}
		if (targetType == VariableType.Vector3)
		{
			Vector3 vector2 = (Vector3)_obj;
			if (vector2 != Vector3.zero)
			{
				return (!fastProcessingIfPossible) ? vector2.magnitude : vector2.sqrMagnitude;
			}
		}
		if (targetType == VariableType.GameObject)
		{
			GameObject gameObject = (GameObject)_obj;
			if (gameObject != null)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					return component.bounds.size.x * component.bounds.size.y * component.bounds.size.z;
				}
			}
		}
		switch (targetType)
		{
		case VariableType.Rect:
		{
			Rect rect = (Rect)_obj;
			return rect.width * rect.height;
		}
		case VariableType.String:
		{
			string text = (string)_obj;
			if (text != null)
			{
				return float.Parse(text);
			}
			break;
		}
		}
		return 0f;
	}

	public static string ParseFsmVarToString(Fsm fsm, FsmVar fsmVar)
	{
		if (fsmVar == null)
		{
			return string.Empty;
		}
		object valueFromFsmVar = GetValueFromFsmVar(fsm, fsmVar);
		if (valueFromFsmVar == null)
		{
			return string.Empty;
		}
		if (fsmVar.Type == VariableType.String)
		{
			return (string)valueFromFsmVar;
		}
		if (fsmVar.Type == VariableType.Bool)
		{
			return (!(bool)valueFromFsmVar) ? "0" : "1";
		}
		if (fsmVar.Type == VariableType.Float)
		{
			return float.Parse(valueFromFsmVar.ToString()).ToString();
		}
		if (fsmVar.Type == VariableType.Int)
		{
			return int.Parse(valueFromFsmVar.ToString()).ToString();
		}
		if (fsmVar.Type == VariableType.Vector2)
		{
			Vector2 vector = (Vector2)valueFromFsmVar;
			return vector.x + "," + vector.y;
		}
		if (fsmVar.Type == VariableType.Vector3)
		{
			Vector3 vector2 = (Vector3)valueFromFsmVar;
			return vector2.x + "," + vector2.y + "," + vector2.z;
		}
		if (fsmVar.Type == VariableType.Quaternion)
		{
			Quaternion quaternion = (Quaternion)valueFromFsmVar;
			return quaternion.x + "," + quaternion.y + "," + quaternion.z + "," + quaternion.w;
		}
		if (fsmVar.Type == VariableType.Rect)
		{
			Rect rect = (Rect)valueFromFsmVar;
			return rect.x + "," + rect.y + "," + rect.width + "," + rect.height;
		}
		if (fsmVar.Type == VariableType.Color)
		{
			Color color = (Color)valueFromFsmVar;
			return color.r + "," + color.g + "," + color.b + "," + color.a;
		}
		if (fsmVar.Type == VariableType.GameObject)
		{
			GameObject gameObject = (GameObject)valueFromFsmVar;
			return gameObject.name;
		}
		if (fsmVar.Type == VariableType.Material)
		{
			Material material = (Material)valueFromFsmVar;
			return material.name;
		}
		if (fsmVar.Type == VariableType.Texture)
		{
			Texture2D texture2D = (Texture2D)valueFromFsmVar;
			return texture2D.name;
		}
		return string.Concat("<", fsmVar.Type, "> not supported");
	}

	public static string ParseValueToString(object item, bool useBytes)
	{
		return string.Empty;
	}

	public static string ParseValueToString(object item)
	{
		if (item == null)
		{
			return string.Empty;
		}
		if (item.GetType() == typeof(string))
		{
			return "string(" + item.ToString() + ")";
		}
		if (item.GetType() == typeof(bool))
		{
			int num = (((bool)item) ? 1 : 0);
			return "bool(" + num + ")";
		}
		if (item.GetType() == typeof(float))
		{
			float num2 = float.Parse(item.ToString());
			return "float(" + num2 + ")";
		}
		if (item.GetType() == typeof(int))
		{
			int num3 = int.Parse(item.ToString());
			return "int(" + num3 + ")";
		}
		if (item.GetType() == typeof(Vector2))
		{
			Vector2 vector = (Vector2)item;
			return "vector2(" + vector.x + "," + vector.y + ")";
		}
		if (item.GetType() == typeof(Vector3))
		{
			Vector3 vector2 = (Vector3)item;
			return "vector3(" + vector2.x + "," + vector2.y + "," + vector2.z + ")";
		}
		if (item.GetType() == typeof(Vector4))
		{
			Vector4 vector3 = (Vector4)item;
			return "vector4(" + vector3.x + "," + vector3.y + "," + vector3.z + "," + vector3.w + ")";
		}
		if (item.GetType() == typeof(Quaternion))
		{
			Quaternion quaternion = (Quaternion)item;
			return "quaternion(" + quaternion.x + "," + quaternion.y + "," + quaternion.z + "," + quaternion.w + ")";
		}
		if (item.GetType() == typeof(Rect))
		{
			Rect rect = (Rect)item;
			return "rect(" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + ")";
		}
		if (item.GetType() == typeof(Color))
		{
			Color color = (Color)item;
			return "color(" + color.r + "," + color.g + "," + color.b + "," + color.a + ")";
		}
		if (item.GetType() == typeof(Texture2D))
		{
			Texture2D tex = (Texture2D)item;
			byte[] inArray = tex.EncodeToPNG();
			return "texture(" + Convert.ToBase64String(inArray) + ")";
		}
		if (item.GetType() == typeof(GameObject))
		{
			GameObject gameObject = (GameObject)item;
			return "gameObject(" + gameObject.name + ")";
		}
		return string.Concat("<", item.GetType(), "> not supported");
	}

	public static object ParseValueFromString(string source, bool useBytes)
	{
		return null;
	}

	public static object ParseValueFromString(string source, VariableType type)
	{
		Type typeFromHandle = typeof(string);
		switch (type)
		{
		case VariableType.Bool:
			typeFromHandle = typeof(bool);
			break;
		case VariableType.Color:
			typeFromHandle = typeof(Color);
			break;
		case VariableType.Float:
			typeFromHandle = typeof(float);
			break;
		case VariableType.GameObject:
			typeFromHandle = typeof(GameObject);
			break;
		case VariableType.Int:
			typeFromHandle = typeof(int);
			break;
		case VariableType.Quaternion:
			typeFromHandle = typeof(Quaternion);
			break;
		case VariableType.Rect:
			typeFromHandle = typeof(Rect);
			break;
		case VariableType.Vector2:
			typeFromHandle = typeof(Vector2);
			break;
		case VariableType.Vector3:
			typeFromHandle = typeof(Vector3);
			break;
		case VariableType.Unknown:
			return ParseValueFromString(source);
		}
		return ParseValueFromString(source, typeFromHandle);
	}

	public static object ParseValueFromString(string source, Type type)
	{
		if (source == null)
		{
			return null;
		}
		if (type == typeof(string))
		{
			return source;
		}
		if (type == typeof(bool))
		{
			if (string.Equals(source, "true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(source, "false", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			bool flag = ((int.Parse(source) != 0) ? true : false);
			return flag;
		}
		if (type == typeof(int))
		{
			int num = int.Parse(source);
			return num;
		}
		if (type == typeof(float))
		{
			float num2 = float.Parse(source);
			return num2;
		}
		if (type == typeof(Vector2))
		{
			string text = "vector2\\([x],[y]\\)";
			string text2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + text2 + ")");
			text = text.Replace("[y]", "(?<y>" + text2 + ")");
			text = "^\\s*" + text;
			Regex regex = new Regex(text);
			Match match = regex.Match(source);
			if (match.Groups["x"].Value != string.Empty && match.Groups["y"].Value != string.Empty)
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		if (type == typeof(Vector3))
		{
			string text3 = "vector3\\([x],[y],[z]\\)";
			string text4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + text4 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + text4 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + text4 + ")");
			text3 = "^\\s*" + text3;
			Regex regex2 = new Regex(text3);
			Match match2 = regex2.Match(source);
			if (match2.Groups["x"].Value != string.Empty && match2.Groups["y"].Value != string.Empty && match2.Groups["z"].Value != string.Empty)
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		if (type == typeof(Vector4))
		{
			string text5 = "vector4\\([x],[y],[z],[w]\\)";
			string text6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + text6 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + text6 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + text6 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + text6 + ")");
			text5 = "^\\s*" + text5;
			Regex regex3 = new Regex(text5);
			Match match3 = regex3.Match(source);
			if (match3.Groups["x"].Value != string.Empty && match3.Groups["y"].Value != string.Empty && match3.Groups["z"].Value != string.Empty && match3.Groups["z"].Value != string.Empty)
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		if (type == typeof(Rect))
		{
			string text7 = "rect\\([x],[y],[w],[h]\\)";
			string text8 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text7 = text7.Replace("[x]", "(?<x>" + text8 + ")");
			text7 = text7.Replace("[y]", "(?<y>" + text8 + ")");
			text7 = text7.Replace("[w]", "(?<w>" + text8 + ")");
			text7 = text7.Replace("[h]", "(?<h>" + text8 + ")");
			text7 = "^\\s*" + text7;
			Regex regex4 = new Regex(text7);
			Match match4 = regex4.Match(source);
			if (match4.Groups["x"].Value != string.Empty && match4.Groups["y"].Value != string.Empty && match4.Groups["w"].Value != string.Empty && match4.Groups["h"].Value != string.Empty)
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (type == typeof(Quaternion))
		{
			string text9 = "quaternion\\([x],[y],[z],[w]\\)";
			string text10 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text9 = text9.Replace("[x]", "(?<x>" + text10 + ")");
			text9 = text9.Replace("[y]", "(?<y>" + text10 + ")");
			text9 = text9.Replace("[z]", "(?<z>" + text10 + ")");
			text9 = text9.Replace("[w]", "(?<w>" + text10 + ")");
			text9 = "^\\s*" + text9;
			Regex regex5 = new Regex(text9);
			Match match5 = regex5.Match(source);
			if (match5.Groups["x"].Value != string.Empty && match5.Groups["y"].Value != string.Empty && match5.Groups["z"].Value != string.Empty && match5.Groups["z"].Value != string.Empty)
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		if (type == typeof(Color))
		{
			string text11 = "color\\([r],[g],[b],[a]\\)";
			string text12 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text11 = text11.Replace("[r]", "(?<r>" + text12 + ")");
			text11 = text11.Replace("[g]", "(?<g>" + text12 + ")");
			text11 = text11.Replace("[b]", "(?<b>" + text12 + ")");
			text11 = text11.Replace("[a]", "(?<a>" + text12 + ")");
			text11 = "^\\s*" + text11;
			Regex regex6 = new Regex(text11);
			Match match6 = regex6.Match(source);
			if (match6.Groups["r"].Value != string.Empty && match6.Groups["g"].Value != string.Empty && match6.Groups["b"].Value != string.Empty && match6.Groups["a"].Value != string.Empty)
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		if (type == typeof(GameObject))
		{
			source = source.Substring(11, source.Length - 12);
			return GameObject.Find(source);
		}
		return null;
	}

	public static object ParseValueFromString(string source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.StartsWith("string("))
		{
			source = source.Substring(7, source.Length - 8);
			return source;
		}
		if (source.StartsWith("bool("))
		{
			source = source.Substring(5, source.Length - 6);
			bool flag = int.Parse(source) == 1;
			return flag;
		}
		if (source.StartsWith("int("))
		{
			source = source.Substring(4, source.Length - 5);
			int num = int.Parse(source);
			return num;
		}
		if (source.StartsWith("float("))
		{
			source = source.Substring(6, source.Length - 7);
			float num2 = float.Parse(source);
			return num2;
		}
		if (source.StartsWith("vector2("))
		{
			string text = "vector2\\([x],[y]\\)";
			string text2 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text = text.Replace("[x]", "(?<x>" + text2 + ")");
			text = text.Replace("[y]", "(?<y>" + text2 + ")");
			text = "^\\s*" + text;
			Regex regex = new Regex(text);
			Match match = regex.Match(source);
			if (match.Groups["x"].Value != string.Empty && match.Groups["y"].Value != string.Empty)
			{
				return new Vector2(float.Parse(match.Groups["x"].Value), float.Parse(match.Groups["y"].Value));
			}
			return Vector2.zero;
		}
		if (source.StartsWith("vector3("))
		{
			string text3 = "vector3\\([x],[y],[z]\\)";
			string text4 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text3 = text3.Replace("[x]", "(?<x>" + text4 + ")");
			text3 = text3.Replace("[y]", "(?<y>" + text4 + ")");
			text3 = text3.Replace("[z]", "(?<z>" + text4 + ")");
			text3 = "^\\s*" + text3;
			Regex regex2 = new Regex(text3);
			Match match2 = regex2.Match(source);
			if (match2.Groups["x"].Value != string.Empty && match2.Groups["y"].Value != string.Empty && match2.Groups["z"].Value != string.Empty)
			{
				return new Vector3(float.Parse(match2.Groups["x"].Value), float.Parse(match2.Groups["y"].Value), float.Parse(match2.Groups["z"].Value));
			}
			return Vector3.zero;
		}
		if (source.StartsWith("vector4("))
		{
			string text5 = "vector4\\([x],[y],[z],[w]\\)";
			string text6 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text5 = text5.Replace("[x]", "(?<x>" + text6 + ")");
			text5 = text5.Replace("[y]", "(?<y>" + text6 + ")");
			text5 = text5.Replace("[z]", "(?<z>" + text6 + ")");
			text5 = text5.Replace("[w]", "(?<w>" + text6 + ")");
			text5 = "^\\s*" + text5;
			Regex regex3 = new Regex(text5);
			Match match3 = regex3.Match(source);
			if (match3.Groups["x"].Value != string.Empty && match3.Groups["y"].Value != string.Empty && match3.Groups["z"].Value != string.Empty && match3.Groups["z"].Value != string.Empty)
			{
				return new Vector4(float.Parse(match3.Groups["x"].Value), float.Parse(match3.Groups["y"].Value), float.Parse(match3.Groups["z"].Value), float.Parse(match3.Groups["w"].Value));
			}
			return Vector4.zero;
		}
		if (source.StartsWith("rect("))
		{
			string text7 = "rect\\([x],[y],[w],[h]\\)";
			string text8 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text7 = text7.Replace("[x]", "(?<x>" + text8 + ")");
			text7 = text7.Replace("[y]", "(?<y>" + text8 + ")");
			text7 = text7.Replace("[w]", "(?<w>" + text8 + ")");
			text7 = text7.Replace("[h]", "(?<h>" + text8 + ")");
			text7 = "^\\s*" + text7;
			Regex regex4 = new Regex(text7);
			Match match4 = regex4.Match(source);
			if (match4.Groups["x"].Value != string.Empty && match4.Groups["y"].Value != string.Empty && match4.Groups["w"].Value != string.Empty && match4.Groups["h"].Value != string.Empty)
			{
				return new Rect(float.Parse(match4.Groups["x"].Value), float.Parse(match4.Groups["y"].Value), float.Parse(match4.Groups["w"].Value), float.Parse(match4.Groups["h"].Value));
			}
			return new Rect(0f, 0f, 0f, 0f);
		}
		if (source.StartsWith("quaternion("))
		{
			string text9 = "quaternion\\([x],[y],[z],[w]\\)";
			string text10 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text9 = text9.Replace("[x]", "(?<x>" + text10 + ")");
			text9 = text9.Replace("[y]", "(?<y>" + text10 + ")");
			text9 = text9.Replace("[z]", "(?<z>" + text10 + ")");
			text9 = text9.Replace("[w]", "(?<w>" + text10 + ")");
			text9 = "^\\s*" + text9;
			Regex regex5 = new Regex(text9);
			Match match5 = regex5.Match(source);
			if (match5.Groups["x"].Value != string.Empty && match5.Groups["y"].Value != string.Empty && match5.Groups["z"].Value != string.Empty && match5.Groups["z"].Value != string.Empty)
			{
				return new Quaternion(float.Parse(match5.Groups["x"].Value), float.Parse(match5.Groups["y"].Value), float.Parse(match5.Groups["z"].Value), float.Parse(match5.Groups["w"].Value));
			}
			return Quaternion.identity;
		}
		if (source.StartsWith("color("))
		{
			string text11 = "color\\([r],[g],[b],[a]\\)";
			string text12 = "[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?";
			text11 = text11.Replace("[r]", "(?<r>" + text12 + ")");
			text11 = text11.Replace("[g]", "(?<g>" + text12 + ")");
			text11 = text11.Replace("[b]", "(?<b>" + text12 + ")");
			text11 = text11.Replace("[a]", "(?<a>" + text12 + ")");
			text11 = "^\\s*" + text11;
			Regex regex6 = new Regex(text11);
			Match match6 = regex6.Match(source);
			if (match6.Groups["r"].Value != string.Empty && match6.Groups["g"].Value != string.Empty && match6.Groups["b"].Value != string.Empty && match6.Groups["a"].Value != string.Empty)
			{
				return new Color(float.Parse(match6.Groups["r"].Value), float.Parse(match6.Groups["g"].Value), float.Parse(match6.Groups["b"].Value), float.Parse(match6.Groups["a"].Value));
			}
			return Color.black;
		}
		if (source.StartsWith("texture("))
		{
			source = source.Substring(8, source.Length - 9);
			byte[] data = Convert.FromBase64String(source);
			Texture2D texture2D = new Texture2D(16, 16);
			texture2D.LoadImage(data);
			return texture2D;
		}
		if (source.StartsWith("gameObject("))
		{
			source = source.Substring(11, source.Length - 12);
			return GameObject.Find(source);
		}
		return null;
	}
}
