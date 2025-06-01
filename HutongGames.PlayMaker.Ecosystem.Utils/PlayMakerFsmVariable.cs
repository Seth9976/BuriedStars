using System;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class PlayMakerFsmVariable
{
	public VariableSelectionChoice variableSelectionChoice;

	public VariableType selectedType = VariableType.Unknown;

	public string variableName;

	public string defaultVariableName;

	public bool initialized;

	public bool targetUndefined = true;

	private string variableNameToUse = string.Empty;

	private FsmVariables fsmVariables;

	private NamedVariable _namedVariable;

	private FsmFloat _float;

	private FsmInt _int;

	private FsmBool _bool;

	private FsmGameObject _gameObject;

	private FsmColor _color;

	private FsmMaterial _material;

	private FsmObject _object;

	private FsmQuaternion _quaternion;

	private FsmRect _rect;

	private FsmString _string;

	private FsmTexture _texture;

	private FsmVector2 _vector2;

	private FsmVector3 _vector3;

	private FsmArray _fsmArray;

	private FsmEnum _fsmEnum;

	public NamedVariable namedVariable => _namedVariable;

	public FsmFloat FsmFloat
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Float) || selectedType != VariableType.Float)
			{
				return null;
			}
			if (_float == null && fsmVariables != null && selectedType == VariableType.Float)
			{
				_float = fsmVariables.GetFsmFloat(variableNameToUse);
			}
			return _float;
		}
	}

	public FsmInt FsmInt
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Int) || selectedType != VariableType.Int)
			{
				return null;
			}
			if (_int == null && fsmVariables != null && selectedType == VariableType.Int)
			{
				_int = fsmVariables.GetFsmInt(variableNameToUse);
			}
			return _int;
		}
	}

	public FsmBool FsmBool
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Bool) || selectedType != VariableType.Bool)
			{
				return null;
			}
			if (_bool == null && fsmVariables != null && selectedType == VariableType.Bool)
			{
				_bool = fsmVariables.GetFsmBool(variableNameToUse);
			}
			return _bool;
		}
	}

	public FsmGameObject FsmGameObject
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.GameObject) || selectedType != VariableType.GameObject)
			{
				return null;
			}
			if (_gameObject == null && fsmVariables != null && selectedType == VariableType.GameObject)
			{
				_gameObject = fsmVariables.GetFsmGameObject(variableNameToUse);
			}
			return _gameObject;
		}
	}

	public FsmColor FsmColor
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Color) || selectedType != VariableType.Color)
			{
				return null;
			}
			if (_color == null && fsmVariables != null && selectedType == VariableType.Color)
			{
				_color = fsmVariables.GetFsmColor(variableNameToUse);
			}
			return _color;
		}
	}

	public FsmMaterial FsmMaterial
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Material) || selectedType != VariableType.Material)
			{
				return null;
			}
			if (_material == null && fsmVariables != null && selectedType == VariableType.Material)
			{
				_material = fsmVariables.GetFsmMaterial(variableNameToUse);
			}
			return _material;
		}
	}

	public FsmObject FsmObject
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Object) || selectedType != VariableType.Object)
			{
				return null;
			}
			if (_object == null && fsmVariables != null && selectedType == VariableType.Object)
			{
				_object = fsmVariables.GetFsmObject(variableNameToUse);
			}
			return _object;
		}
	}

	public FsmQuaternion FsmQuaternion
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Quaternion) || selectedType != VariableType.Quaternion)
			{
				return null;
			}
			if (_quaternion == null && fsmVariables != null && selectedType == VariableType.Quaternion)
			{
				_quaternion = fsmVariables.GetFsmQuaternion(variableNameToUse);
			}
			return _quaternion;
		}
	}

	public FsmRect FsmRect
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Rect) || selectedType != VariableType.Rect)
			{
				return null;
			}
			if (_rect == null && fsmVariables != null && selectedType == VariableType.Rect)
			{
				_rect = fsmVariables.GetFsmRect(variableNameToUse);
			}
			return _rect;
		}
	}

	public FsmString FsmString
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.String) || selectedType != VariableType.String)
			{
				return null;
			}
			if (_string == null && fsmVariables != null && selectedType == VariableType.String)
			{
				_string = fsmVariables.GetFsmString(variableNameToUse);
			}
			return _string;
		}
	}

	public FsmTexture FsmTexture
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Texture) || selectedType != VariableType.Texture)
			{
				return null;
			}
			if (_texture == null && fsmVariables != null && selectedType == VariableType.Texture)
			{
				_texture = fsmVariables.GetFsmTexture(variableNameToUse);
			}
			return _texture;
		}
	}

	public FsmVector2 FsmVector2
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Vector2) || selectedType != VariableType.Vector2)
			{
				return null;
			}
			if (_vector2 == null && fsmVariables != null && selectedType == VariableType.Vector2)
			{
				_vector2 = fsmVariables.GetFsmVector2(variableNameToUse);
			}
			return _vector2;
		}
	}

	public FsmVector3 FsmVector3
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Vector3) || selectedType != VariableType.Vector3)
			{
				return null;
			}
			if (_vector3 == null && fsmVariables != null && selectedType == VariableType.Vector3)
			{
				_vector3 = fsmVariables.GetFsmVector3(variableNameToUse);
			}
			return _vector3;
		}
	}

	public FsmArray FsmArray
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Array) || selectedType != VariableType.Array)
			{
				return null;
			}
			if (_fsmArray == null && fsmVariables != null && selectedType == VariableType.Array)
			{
				_fsmArray = fsmVariables.GetFsmArray(variableNameToUse);
			}
			return _fsmArray;
		}
	}

	public FsmEnum FsmEnum
	{
		get
		{
			if ((variableSelectionChoice != VariableSelectionChoice.Any && variableSelectionChoice != VariableSelectionChoice.Enum) || selectedType != VariableType.Enum)
			{
				return null;
			}
			if (_fsmEnum == null && fsmVariables != null && selectedType == VariableType.Enum)
			{
				_fsmEnum = fsmVariables.GetFsmEnum(variableNameToUse);
			}
			return _fsmEnum;
		}
	}

	public PlayMakerFsmVariable()
	{
	}

	public PlayMakerFsmVariable(VariableSelectionChoice variableSelectionChoice)
	{
		this.variableSelectionChoice = variableSelectionChoice;
	}

	public PlayMakerFsmVariable(string defaultVariableName)
	{
		this.defaultVariableName = defaultVariableName;
	}

	public PlayMakerFsmVariable(VariableSelectionChoice variableSelectionChoice, string defaultVariableName)
	{
		this.variableSelectionChoice = variableSelectionChoice;
		this.defaultVariableName = defaultVariableName;
	}

	public bool GetVariable(PlayMakerFsmVariableTarget variableTarget)
	{
		initialized = true;
		targetUndefined = true;
		if (variableTarget.FsmVariables != null)
		{
			targetUndefined = false;
			variableNameToUse = ((!string.IsNullOrEmpty(variableName)) ? variableName : defaultVariableName);
			fsmVariables = variableTarget.FsmVariables;
			_namedVariable = fsmVariables.GetVariable(variableNameToUse);
			if (_namedVariable != null)
			{
				selectedType = _namedVariable.VariableType;
				return true;
			}
		}
		selectedType = VariableType.Unknown;
		return false;
	}

	public override string ToString()
	{
		string text = "<color=blue>" + variableName + "</color>";
		if (string.IsNullOrEmpty(text))
		{
			text = "<color=red>None</color>";
		}
		return string.Format(string.Concat("PlayMaker Variable<{0}>: {1} (", _namedVariable, ")"), variableSelectionChoice, text);
	}

	public static VariableType GetTypeFromChoice(VariableSelectionChoice choice)
	{
		return choice switch
		{
			VariableSelectionChoice.Any => VariableType.Unknown, 
			VariableSelectionChoice.Float => VariableType.Float, 
			VariableSelectionChoice.Int => VariableType.Int, 
			VariableSelectionChoice.Bool => VariableType.Bool, 
			VariableSelectionChoice.GameObject => VariableType.GameObject, 
			VariableSelectionChoice.String => VariableType.String, 
			VariableSelectionChoice.Vector2 => VariableType.Vector2, 
			VariableSelectionChoice.Vector3 => VariableType.Vector3, 
			VariableSelectionChoice.Color => VariableType.Color, 
			VariableSelectionChoice.Rect => VariableType.Rect, 
			VariableSelectionChoice.Material => VariableType.Material, 
			VariableSelectionChoice.Texture => VariableType.Texture, 
			VariableSelectionChoice.Quaternion => VariableType.Quaternion, 
			VariableSelectionChoice.Object => VariableType.Object, 
			VariableSelectionChoice.Array => VariableType.Array, 
			VariableSelectionChoice.Enum => VariableType.Enum, 
			_ => VariableType.Unknown, 
		};
	}
}
