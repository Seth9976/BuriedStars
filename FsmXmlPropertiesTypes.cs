using System.Collections.Generic;
using HutongGames.PlayMaker;

public class FsmXmlPropertiesTypes : FsmStateAction
{
	public FsmString[] properties;

	public VariableType[] propertiesTypes;

	private Dictionary<string, VariableType> _cache;

	public void cacheTypes()
	{
		_cache = new Dictionary<string, VariableType>();
		int num = 0;
		FsmString[] array = properties;
		foreach (FsmString fsmString in array)
		{
			_cache.Add(fsmString.Value, propertiesTypes[num]);
			num++;
		}
	}

	public VariableType GetPropertyType(string property)
	{
		if (_cache == null)
		{
			cacheTypes();
		}
		if (_cache.ContainsKey(property))
		{
			return _cache[property];
		}
		return VariableType.Unknown;
	}
}
