using System.Collections.Generic;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class StaffRoll : MonoBehaviour
{
	public class clStaffText
	{
		public GameObject m_goText;

		public RectTransform m_rtfPos;

		public Text m_textData;

		public float m_fH;
	}

	public GameObject m_goStaffRoll;

	public GameObject m_goText;

	public RectTransform m_rtfParent;

	public Text m_textStaffRoll;

	public Canvas m_canvas;

	private List<clStaffText> m_lstStaffText;

	private float m_fCanvasW;

	private float m_fCanvasH;

	private float m_fFirstY;

	public float m_fSpeed;

	public float m_fSkipSpeed;

	private EventEngine m_EventEngine;

	private bool m_isStart;

	private static StaffRoll s_Instance;

	public static StaffRoll instance => s_Instance;

	private void Start()
	{
		s_Instance = this;
	}

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textStaffRoll);
	}

	private void OnDestroy()
	{
		ClearStaffRoll();
		m_EventEngine = null;
		s_Instance = null;
	}

	public void SetStaffRoll(string strCate)
	{
		m_goStaffRoll.SetActive(value: true);
		m_rtfParent.gameObject.SetActive(value: false);
		ClearStaffRoll();
		m_EventEngine = EventEngine.GetInstance();
		float num = 0f;
		float num2 = 0f;
		num = GameGlobalUtil.GetXlsProgramDefineStrToFloat("STAFFROLL_SPEED_Y");
		num2 = GameGlobalUtil.GetXlsProgramDefineStrToFloat("STAFFROLL_SKIP_SPEED_Y");
		m_fCanvasW = m_canvas.GetComponent<RectTransform>().rect.width;
		m_fCanvasH = m_canvas.GetComponent<RectTransform>().rect.height;
		m_fSpeed = m_fCanvasH * num / 100f;
		m_fSkipSpeed = m_fCanvasH * num2 / 100f;
		m_fFirstY = 0f - m_fCanvasH;
		m_lstStaffText = new List<clStaffText>();
		int dataCount = Xls.StaffRoll.GetDataCount();
		GameObject gameObject = null;
		clStaffText clStaffText = null;
		m_goText.SetActive(value: false);
		float num3 = m_fFirstY;
		float num4 = 0f;
		for (int i = 0; i < dataCount; i++)
		{
			Xls.StaffRoll data_byIdx = Xls.StaffRoll.GetData_byIdx(i);
			if (data_byIdx != null && data_byIdx.m_strCtg == strCate)
			{
				clStaffText = new clStaffText();
				gameObject = Object.Instantiate(m_goText);
				gameObject.SetActive(value: false);
				if (data_byIdx.m_strTxtKey == string.Empty)
				{
					string empty = string.Empty;
				}
				else
				{
					Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_byIdx.m_strTxtKey);
					string empty = ((data_byKey != null) ? data_byKey.m_strTxt : string.Empty);
					clStaffText.m_textData = gameObject.GetComponent<Text>();
					clStaffText.m_textData.color = Color.white;
					clStaffText.m_textData.alignment = TextAnchor.UpperCenter;
					FontManager.ResetTextFontByCurrentLanguage(clStaffText.m_textData);
					clStaffText.m_textData.text = empty;
					clStaffText.m_textData.fontSize = data_byIdx.m_iSize;
					gameObject.name = "Text_" + i;
				}
				clStaffText.m_goText = gameObject;
				clStaffText.m_rtfPos = gameObject.GetComponent<RectTransform>();
				clStaffText.m_rtfPos.SetParent(m_rtfParent, worldPositionStays: false);
				clStaffText.m_fH = clStaffText.m_textData.preferredHeight;
				clStaffText.m_rtfPos.sizeDelta = new Vector2(clStaffText.m_rtfPos.sizeDelta.x, clStaffText.m_fH);
				float num5 = -1f + 2f * data_byIdx.m_fXPos;
				num4 = m_fCanvasW * num5;
				Vector3 vector = clStaffText.m_rtfPos.anchoredPosition;
				clStaffText.m_rtfPos.anchoredPosition = new Vector3(num4, num3, vector.z);
				clStaffText.m_rtfPos.localScale = new Vector3(1f, 1f, 1f);
				clStaffText.m_rtfPos.offsetMax = new Vector2(0f, clStaffText.m_rtfPos.offsetMax.y);
				m_lstStaffText.Add(clStaffText);
				num3 -= clStaffText.m_fH;
			}
		}
		m_rtfParent.gameObject.SetActive(value: true);
		m_isStart = true;
	}

	private void Update()
	{
		if (m_lstStaffText != null && m_EventEngine != null)
		{
			int count = m_lstStaffText.Count;
			bool flag = true;
			bool flag2 = false;
			float num = ((!m_EventEngine.GetSkip()) ? m_fSpeed : m_fSkipSpeed);
			num = Time.deltaTime * num;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = m_lstStaffText[i].m_rtfPos.anchoredPosition;
				m_lstStaffText[i].m_rtfPos.anchoredPosition = new Vector3(vector.x, vector.y + num, vector.z);
				float y = m_lstStaffText[i].m_rtfPos.anchoredPosition.y;
				float fH = m_lstStaffText[i].m_fH;
				flag = y - fH <= 0f && y >= 0f - m_fCanvasH;
				m_lstStaffText[i].m_goText.SetActive(flag);
				flag2 = flag2 || flag;
			}
			if (m_isStart && !flag2)
			{
				ExitStaffRoll();
			}
		}
	}

	public bool IsProcStaffRoll()
	{
		return !m_goStaffRoll.activeInHierarchy;
	}

	private void ExitStaffRoll()
	{
		ClearStaffRoll();
		m_goStaffRoll.SetActive(value: false);
	}

	private void ClearStaffRoll()
	{
		if (m_lstStaffText != null)
		{
			int count = m_lstStaffText.Count;
			for (int i = 0; i < count; i++)
			{
				Object.Destroy(m_lstStaffText[i].m_goText);
			}
			m_lstStaffText.Clear();
			m_lstStaffText = null;
		}
	}
}
