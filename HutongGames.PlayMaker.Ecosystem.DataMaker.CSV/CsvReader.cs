using System.Collections.Generic;
using System.Text;

namespace HutongGames.PlayMaker.Ecosystem.DataMaker.CSV;

public class CsvReader
{
	public static CsvData LoadFromString(string file_contents, bool hasHeader = false)
	{
		CsvData csvData = new CsvData(hasHeader);
		int length = file_contents.Length;
		int num = 0;
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		bool flag = false;
		while (num < length)
		{
			char c = file_contents[num++];
			switch (c)
			{
			case '"':
				if (!flag)
				{
					flag = true;
					break;
				}
				if (num == length)
				{
					flag = false;
					goto case '\n';
				}
				if (file_contents[num] == '"')
				{
					stringBuilder.Append("\"");
					num++;
				}
				else
				{
					flag = false;
				}
				break;
			case '\n':
			case ',':
				if (flag)
				{
					stringBuilder.Append(c);
					break;
				}
				list.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
				if (c == '\n' || num == length)
				{
					csvData.AddRecord(list);
					list.Clear();
				}
				break;
			default:
				stringBuilder.Append(c);
				break;
			case '\r':
				break;
			}
		}
		if (stringBuilder.Length != 0)
		{
			list.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
			csvData.AddRecord(list);
			list.Clear();
		}
		csvData.OnParseEnded();
		return csvData;
	}
}
