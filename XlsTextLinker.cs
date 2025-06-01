using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class XlsTextLinker : MonoBehaviour
{
	public Text m_LinkedText;

	public TagText m_LinkedTagText;

	public string m_ClassName = string.Empty;

	public string m_PropertyName = string.Empty;

	public string m_KeyOrIndex = string.Empty;

	private void Start()
	{
		Proc();
	}

	private void Proc()
	{
		Type type = Type.GetType("Xls/" + m_ClassName);
		if (type == null)
		{
			return;
		}
		FieldInfo field = type.GetField("IsUseKey");
		if (field == null || field.FieldType != typeof(bool))
		{
			return;
		}
		object obj = null;
		MethodInfo methodInfo = null;
		if ((bool)field.GetValue(null))
		{
			FieldInfo field2 = type.GetField("KeyType");
			if (field2 == null || !(field2.GetValue(null) is Type type2))
			{
				return;
			}
			if (type2 == typeof(string))
			{
				obj = m_KeyOrIndex;
			}
			else if (type2 == typeof(int))
			{
				int result = 0;
				if (!int.TryParse(m_KeyOrIndex, out result))
				{
					return;
				}
				obj = result;
			}
			else if (type2 == typeof(float))
			{
				float result2 = 0f;
				if (!float.TryParse(m_KeyOrIndex, NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
				{
					return;
				}
				obj = result2;
			}
			if (obj == null)
			{
				return;
			}
			methodInfo = type.GetMethod("GetData_byKey");
		}
		else
		{
			int result3 = 0;
			if (!int.TryParse(m_KeyOrIndex, out result3))
			{
				return;
			}
			obj = result3;
			methodInfo = type.GetMethod("GetData_byIdx");
		}
		if (methodInfo == null)
		{
			return;
		}
		object obj2 = methodInfo.Invoke(null, new object[1] { obj });
		if (obj2 == null)
		{
			return;
		}
		PropertyInfo property = type.GetProperty(m_PropertyName);
		if (property != null)
		{
			object value = property.GetValue(obj2, null);
			if (m_LinkedText != null)
			{
				m_LinkedText.text = value.ToString();
			}
			if (m_LinkedTagText != null)
			{
				m_LinkedTagText.text = value.ToString();
				m_LinkedTagText.BuildTextData();
			}
		}
	}
}
