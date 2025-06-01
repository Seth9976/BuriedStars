using System.Collections.Generic;

public class MultiSortedDictionary<Key, Value>
{
	private SortedDictionary<Key, List<Value>> m_Dic;

	public List<Value> this[Key key]
	{
		get
		{
			List<Value> value = null;
			if (!m_Dic.TryGetValue(key, out value))
			{
				value = new List<Value>();
				m_Dic.Add(key, value);
			}
			return value;
		}
	}

	public IEnumerable<Key> keys => m_Dic.Keys;

	public MultiSortedDictionary()
	{
		m_Dic = new SortedDictionary<Key, List<Value>>();
	}

	public MultiSortedDictionary(IComparer<Key> comparer)
	{
		m_Dic = new SortedDictionary<Key, List<Value>>(comparer);
	}

	public void Add(Key key, Value value)
	{
		List<Value> value2 = null;
		if (m_Dic.TryGetValue(key, out value2))
		{
			value2.Add(value);
			return;
		}
		value2 = new List<Value>();
		value2.Add(value);
		m_Dic.Add(key, value2);
	}

	public bool ContainsKey(Key key)
	{
		return m_Dic.ContainsKey(key);
	}
}
