using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

public class CsvData
{
	public static Dictionary<string, CsvData> References;

	public List<List<string>> Data = new List<List<string>>();

	public List<string> HeaderKeys = new List<string>();

	public int RecordCount;

	public int ColumnCount;

	public bool HasHeader;

	private bool headerDone;

	public CsvData(bool hasHeader = false)
	{
		HasHeader = hasHeader;
	}

	public static void RemoveAllReferences()
	{
		References = null;
	}

	public static void AddReference(CsvData data, string reference)
	{
		if (References == null)
		{
			References = new Dictionary<string, CsvData>();
		}
		References[reference] = data;
	}

	public static bool RemoveReference(string reference)
	{
		if (References != null)
		{
			return References.Remove(reference);
		}
		return false;
	}

	public static bool HasReference(string reference)
	{
		return References != null && References.ContainsKey(reference);
	}

	public static CsvData GetReference(string reference)
	{
		if (References != null && References.ContainsKey(reference))
		{
			return References[reference];
		}
		return null;
	}

	public void AddRecord(List<string> items)
	{
		ColumnCount = Mathf.Max(ColumnCount, items.Count);
		if (HasHeader && !headerDone)
		{
			headerDone = true;
			HeaderKeys = new List<string>(items);
		}
		else
		{
			Data.Add(new List<string>(items));
		}
		RecordCount = Data.Count;
	}

	public void OnParseEnded()
	{
		foreach (List<string> datum in Data)
		{
			if (datum.Count < ColumnCount)
			{
				datum.AddRange(Enumerable.Repeat(string.Empty, ColumnCount - datum.Count));
			}
		}
	}

	public string GetFieldAt(int record, int column, bool logErrors = true)
	{
		if (Data.Count <= record)
		{
			return string.Empty;
		}
		if (Data[record].Count <= column)
		{
			return string.Empty;
		}
		return Data[record][column];
	}

	public string GetFieldAt(int record, string key)
	{
		if (!HasHeader)
		{
			return string.Empty;
		}
		if (Data.Count <= record)
		{
			return string.Empty;
		}
		int num = HeaderKeys.IndexOf(key);
		if (Data[record].Count <= num)
		{
			return string.Empty;
		}
		return Data[record][num];
	}

	public string[] GetRecordAt(int record)
	{
		if (Data.Count <= record)
		{
			return new string[ColumnCount];
		}
		return Data[record].ToArray();
	}
}
