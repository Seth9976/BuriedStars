using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_SNSMenu : MonoBehaviour
{
	public Text m_TitleText;

	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContainerRT;

	public GameObject m_SrcContentObject;

	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private const int c_ContentCount = 2;

	private List<SWSub_SNSContent> m_Contents = new List<SWSub_SNSContent>();

	private void Awake()
	{
		m_SrcContentObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		ClearContent();
	}

	private void OnEnable()
	{
		if (m_isSimpleView)
		{
			if (m_TitleText != null)
			{
				m_TitleText.gameObject.SetActive(value: false);
			}
			if (m_ScrollRect != null)
			{
				m_ScrollRect.gameObject.SetActive(value: false);
			}
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: false);
			}
			if (m_SimpleIconRoot != null)
			{
				m_SimpleIconRoot.SetActive(value: true);
			}
		}
		else
		{
			if (m_TitleText != null)
			{
				m_TitleText.gameObject.SetActive(value: true);
				m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_SUB_SNS_TITLE");
			}
			if (m_ScrollRect != null)
			{
				m_ScrollRect.gameObject.SetActive(value: true);
			}
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: true);
			}
			if (m_SimpleIconRoot != null)
			{
				m_SimpleIconRoot.SetActive(value: false);
			}
		}
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			m_Contents[i].gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (!m_isSimpleView && m_Contents.Count <= 1)
		{
			CreateContent();
		}
	}

	public void OnProc_ChnagedSelectContent(object sender, object arg)
	{
		if (m_isSimpleView)
		{
			return;
		}
		SNSMenuPlus sNSMenuPlus = sender as SNSMenuPlus;
		if (!(sNSMenuPlus == null))
		{
			int num = (int)arg;
			if (num >= 0)
			{
				Xls.SNSPostData postDataNearBy = sNSMenuPlus.GetPostDataNearBy(num, 0);
				if (postDataNearBy != null)
				{
					if (m_Contents.Count <= 1)
					{
						CreateContent();
					}
					Xls.SNSPostData postDataNearBy2 = sNSMenuPlus.GetPostDataNearBy(num, 1);
					if (postDataNearBy2 != null)
					{
						m_Contents[0].SetValue(postDataNearBy);
						m_Contents[0].gameObject.SetActive(value: true);
						m_Contents[1].SetValue(postDataNearBy2);
						m_Contents[1].gameObject.SetActive(value: true);
						return;
					}
					postDataNearBy2 = sNSMenuPlus.GetPostDataNearBy(num, -1);
					if (postDataNearBy2 != null)
					{
						m_Contents[0].SetValue(postDataNearBy2);
						m_Contents[0].gameObject.SetActive(value: true);
						m_Contents[1].SetValue(postDataNearBy);
						m_Contents[1].gameObject.SetActive(value: true);
					}
					else
					{
						m_Contents[0].SetValue(postDataNearBy);
						m_Contents[0].gameObject.SetActive(value: true);
						m_Contents[1].gameObject.SetActive(value: false);
					}
					return;
				}
			}
		}
		for (int i = 0; i < 2; i++)
		{
			m_Contents[i].gameObject.SetActive(value: false);
		}
	}

	private void CreateContent()
	{
		if (m_Contents.Count > 0)
		{
			ClearContent();
		}
		FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
		GameObject gameObject = null;
		SWSub_SNSContent sWSub_SNSContent = null;
		float num = 0f;
		for (int i = 0; i < 2; i++)
		{
			gameObject = Object.Instantiate(m_SrcContentObject);
			sWSub_SNSContent = gameObject.GetComponent<SWSub_SNSContent>();
			m_Contents.Add(sWSub_SNSContent);
			sWSub_SNSContent.rectTransform.SetParent(m_ContentContainerRT, worldPositionStays: false);
			sWSub_SNSContent.rectTransform.anchoredPosition = new Vector2(sWSub_SNSContent.rectTransform.anchoredPosition.x, num);
			gameObject.SetActive(value: false);
			num -= sWSub_SNSContent.rectTransform.rect.height * sWSub_SNSContent.rectTransform.localScale.y;
			Text[] textComps = new Text[6]
			{
				sWSub_SNSContent.m_NormalContent.m_ContentText,
				sWSub_SNSContent.m_NormalContent.m_RetweetCountText,
				sWSub_SNSContent.m_NormalContent.m_UserNameText,
				sWSub_SNSContent.m_RetweetedContent.m_ContentText,
				sWSub_SNSContent.m_RetweetedContent.m_RetweetCountText,
				sWSub_SNSContent.m_RetweetedContent.m_UserNameText
			};
			FontManager.ResetTextFontByCurrentLanguage(textComps);
		}
	}

	private void ClearContent()
	{
		int count = m_Contents.Count;
		SWSub_SNSContent sWSub_SNSContent = null;
		for (int i = 0; i < count; i++)
		{
			sWSub_SNSContent = m_Contents[i];
			Object.Destroy(sWSub_SNSContent.gameObject);
		}
		m_Contents.Clear();
	}
}
