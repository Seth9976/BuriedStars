using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_ToDoMenu : MonoBehaviour
{
	public Text m_textTitle;

	public GameObject m_goSlot;

	public Transform m_tfParent;

	private List<SWSub_ToDoMenuSlot> m_listTodoSlot;

	private const float m_fSlotGapH = 158f;

	public static int m_iSlotCnt = 4;

	private GameSwitch m_GameSwitch;

	private void OnEnable()
	{
		m_textTitle.text = string.Empty;
	}

	private void OnDestroy()
	{
		if (m_listTodoSlot != null)
		{
			m_listTodoSlot.Clear();
		}
		m_listTodoSlot = null;
		m_GameSwitch = null;
	}

	public void MakeSlot()
	{
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
		FontManager.ResetTextFontByCurrentLanguage(m_textTitle);
		m_listTodoSlot = new List<SWSub_ToDoMenuSlot>();
		GameObject gameObject = null;
		float num = 0f;
		for (int i = 0; i < m_iSlotCnt; i++)
		{
			gameObject = Object.Instantiate(m_goSlot);
			gameObject.transform.SetParent(m_tfParent, worldPositionStays: false);
			gameObject.name = "slot_" + i;
			gameObject.SetActive(value: true);
			SWSub_ToDoMenuSlot component = gameObject.GetComponent<SWSub_ToDoMenuSlot>();
			m_listTodoSlot.Add(component);
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			Vector3 localPosition = component2.localPosition;
			if (i == 0)
			{
				num = localPosition.y;
			}
			component2.localPosition = new Vector3(localPosition.x, num, localPosition.z);
			num -= 158f;
			component.InitSlot();
		}
	}

	public void SetSlotKey(string[] strKey)
	{
		for (int i = 0; i < m_iSlotCnt; i++)
		{
			Xls.Trophys data_byKey = Xls.Trophys.GetData_byKey(strKey[i]);
			Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_strName);
			if (data_byKey2 != null)
			{
				m_listTodoSlot[i].SetTitle(data_byKey2.m_strTitle);
				m_listTodoSlot[i].SetTodoCount(m_GameSwitch.GetTrophyCnt(strKey[i]), data_byKey.m_iMax);
			}
		}
	}
}
