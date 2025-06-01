using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_RecordMenu : MonoBehaviour
{
	public Text m_TitleText;

	public RectTransform m_ContentContainer;

	public GameObject m_ContentSrcObject;

	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private int c_ContentCount = 4;

	private List<SWSub_RecordContent> m_Contents = new List<SWSub_RecordContent>();

	private bool m_isStarted;

	private object m_eventSender;

	private object m_eventArg;

	private void Awake()
	{
		m_ContentSrcObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		if (m_TitleText != null)
		{
			m_TitleText.gameObject.SetActive(!m_isSimpleView);
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_TITLE");
		}
		if (m_ContentContainer != null)
		{
			m_ContentContainer.gameObject.SetActive(!m_isSimpleView);
		}
		if (m_SimpleIconRoot != null)
		{
			m_SimpleIconRoot.SetActive(m_isSimpleView);
		}
	}

	private void OnDisable()
	{
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			m_Contents[i].gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		ClearContent();
		m_eventSender = null;
		m_eventArg = null;
	}

	private void Start()
	{
		if (!m_isSimpleView && !m_isStarted)
		{
			ClearContent();
			FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
			GameObject gameObject = null;
			SWSub_RecordContent sWSub_RecordContent = null;
			float num = 0f;
			for (int i = 0; i < c_ContentCount; i++)
			{
				gameObject = Object.Instantiate(m_ContentSrcObject);
				sWSub_RecordContent = gameObject.GetComponent<SWSub_RecordContent>();
				m_Contents.Add(sWSub_RecordContent);
				sWSub_RecordContent.rectTransform.SetParent(m_ContentContainer, worldPositionStays: false);
				sWSub_RecordContent.rectTransform.anchoredPosition = new Vector2(sWSub_RecordContent.rectTransform.anchoredPosition.x, num);
				gameObject.SetActive(value: false);
				num -= sWSub_RecordContent.rectTransform.rect.height * sWSub_RecordContent.rectTransform.localScale.y;
				FontManager.ResetTextFontByCurrentLanguage(sWSub_RecordContent.m_ContentText);
			}
			m_isStarted = true;
			if (m_eventSender != null)
			{
				OnProc_ChnagedSelectContent(m_eventSender, m_eventArg);
			}
		}
	}

	private void ClearContent()
	{
		int count = m_Contents.Count;
		SWSub_RecordContent sWSub_RecordContent = null;
		for (int i = 0; i < count; i++)
		{
			sWSub_RecordContent = m_Contents[i];
			Object.Destroy(sWSub_RecordContent.gameObject);
		}
		m_Contents.Clear();
	}

	public void OnProc_ChnagedSelectContent(object sender, object arg)
	{
		if (m_isSimpleView)
		{
			return;
		}
		if (!m_isStarted)
		{
			m_eventSender = sender;
			m_eventArg = arg;
			return;
		}
		RecordMenuPlus recordMenuPlus = sender as RecordMenuPlus;
		if (!(recordMenuPlus == null))
		{
			RecordContentPlus recordContentPlus = arg as RecordContentPlus;
			if (!(recordContentPlus == null))
			{
				int num = 0;
				RecordContentPlus recordContentPlus2 = null;
				List<RecordContentPlus> list = new List<RecordContentPlus>();
				do
				{
					recordContentPlus2 = ((num != 0) ? recordMenuPlus.GetContentNearBy(recordContentPlus, num) : recordContentPlus);
					if (recordContentPlus2 == null)
					{
						if (num < 0)
						{
							break;
						}
						num = -1;
					}
					else if (num >= 0)
					{
						list.Add(recordContentPlus2);
						num++;
					}
					else
					{
						list.Insert(0, recordContentPlus2);
						num--;
					}
				}
				while (list.Count < m_Contents.Count);
				int i;
				for (i = 0; i < list.Count; i++)
				{
					m_Contents[i].contentText = list[i].title;
					m_Contents[i].gameObject.SetActive(value: true);
				}
				for (; i < m_Contents.Count; i++)
				{
					m_Contents[i].gameObject.SetActive(value: false);
				}
				return;
			}
		}
		for (int j = 0; j < m_Contents.Count; j++)
		{
			m_Contents[j].gameObject.SetActive(value: false);
		}
	}
}
