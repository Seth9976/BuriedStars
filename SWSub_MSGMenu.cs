using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_MSGMenu : MonoBehaviour
{
	public Text m_TitleText;

	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContainerRT;

	public GameObject m_ChatroomSelMode;

	public Text m_ChatroomModeText;

	[Header("Src Content Object")]
	public GameObject m_SrcObjMe;

	public GameObject m_SrcObjPartner;

	public GameObject m_SrcObjPartnerF;

	[Header("Simple Mode")]
	public bool m_isSimpleView;

	public GameObject m_SimpleIconRoot;

	private List<SWSub_MSGContent> m_Contents_Me = new List<SWSub_MSGContent>();

	private List<SWSub_MSGContent> m_Contents_Partner = new List<SWSub_MSGContent>();

	private List<SWSub_MSGContent> m_Contents_PartnerF = new List<SWSub_MSGContent>();

	private List<SWSub_MSGContent> m_ActivatedContents = new List<SWSub_MSGContent>();

	private MSGMenuPlus.Mode m_curMode;

	private void Awake()
	{
		m_SrcObjMe.SetActive(value: false);
		m_SrcObjPartner.SetActive(value: false);
		m_SrcObjPartnerF.SetActive(value: false);
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
			if (m_ChatroomSelMode != null)
			{
				m_ChatroomSelMode.gameObject.SetActive(value: false);
			}
			if (m_ChatroomModeText != null)
			{
				m_ChatroomModeText.gameObject.SetActive(value: false);
			}
			if (m_SimpleIconRoot != null)
			{
				m_SimpleIconRoot.gameObject.SetActive(value: true);
			}
		}
		else
		{
			FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
			FontManager.ResetTextFontByCurrentLanguage(m_ChatroomModeText);
			if (m_TitleText != null)
			{
				m_TitleText.gameObject.SetActive(value: true);
				m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_SUB_MSG_TITLE");
			}
			if (m_ScrollRect != null)
			{
				m_ScrollRect.gameObject.SetActive(value: true);
			}
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: true);
			}
			if (m_ChatroomSelMode != null)
			{
				m_ChatroomSelMode.gameObject.SetActive(value: true);
			}
			if (m_ChatroomModeText != null)
			{
				m_ChatroomModeText.gameObject.SetActive(value: true);
				m_ChatroomModeText.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_SUB_MSG_CHATROOM");
			}
			ChangeMode(m_curMode);
			if (m_SimpleIconRoot != null)
			{
				m_SimpleIconRoot.gameObject.SetActive(value: false);
			}
		}
		DeactivateContents();
	}

	private void OnDestory()
	{
		ClearContents();
	}

	private void ClearContents()
	{
		int count = m_Contents_Me.Count;
		for (int i = 0; i < count; i++)
		{
			Object.Destroy(m_Contents_Me[i].gameObject);
		}
		m_Contents_Me.Clear();
		count = m_Contents_Partner.Count;
		for (int j = 0; j < count; j++)
		{
			Object.Destroy(m_Contents_Partner[j].gameObject);
		}
		m_Contents_Partner.Clear();
		count = m_Contents_PartnerF.Count;
		for (int k = 0; k < count; k++)
		{
			Object.Destroy(m_Contents_PartnerF[k].gameObject);
		}
		m_Contents_PartnerF.Clear();
		m_ActivatedContents.Clear();
	}

	private void DeactivateContents()
	{
		int count = m_Contents_Me.Count;
		for (int i = 0; i < count; i++)
		{
			m_Contents_Me[i].gameObject.SetActive(value: false);
		}
		count = m_Contents_Partner.Count;
		for (int j = 0; j < count; j++)
		{
			m_Contents_Partner[j].gameObject.SetActive(value: false);
		}
		count = m_Contents_PartnerF.Count;
		for (int k = 0; k < count; k++)
		{
			m_Contents_PartnerF[k].gameObject.SetActive(value: false);
		}
		m_ActivatedContents.Clear();
	}

	public void OnProc_ChangedSelectContent(object sender, object arg)
	{
		if (m_isSimpleView)
		{
			return;
		}
		DeactivateContents();
		MSGMenuPlus mSGMenuPlus = sender as MSGMenuPlus;
		if (mSGMenuPlus == null)
		{
			return;
		}
		int num = (int)arg;
		if (num < 0)
		{
			return;
		}
		float num2 = m_ScrollRect.viewport.rect.height * m_ScrollRect.viewport.localScale.y;
		float num3 = 0f;
		int num4 = 0;
		MSGMenuPlus.ContentInfo contentInfo = null;
		MSGMenuPlus.ContentInfo talkDataNearBy = mSGMenuPlus.GetTalkDataNearBy(num, 0);
		SWSub_MSGContent sWSub_MSGContent = null;
		do
		{
			contentInfo = ((num4 != 0) ? mSGMenuPlus.GetTalkDataNearBy(num, num4) : talkDataNearBy);
			if (contentInfo == null)
			{
				if (num4 >= 0)
				{
					num4 = -1;
					continue;
				}
				break;
			}
			sWSub_MSGContent = GetEmptyContentObject(contentInfo);
			if (num4 >= 0)
			{
				m_ActivatedContents.Add(sWSub_MSGContent);
				num4++;
			}
			else
			{
				m_ActivatedContents.Insert(0, sWSub_MSGContent);
				num4--;
			}
			num3 += sWSub_MSGContent.rectTransform.rect.height;
			sWSub_MSGContent.gameObject.SetActive(value: true);
		}
		while (num3 < num2);
		int count = m_ActivatedContents.Count;
		if (count > 0)
		{
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			float num5 = ((!(num3 > num2) || num4 >= -1) ? 0f : (num3 - num2));
			for (int i = 0; i < count; i++)
			{
				sWSub_MSGContent = m_ActivatedContents[i];
				sWSub_MSGContent.rectTransform.SetParent(m_ContentContainerRT, worldPositionStays: false);
				sWSub_MSGContent.rectTransform.anchoredPosition = new Vector2(sWSub_MSGContent.rectTransform.anchoredPosition.x, num5);
				GameGlobalUtil.PlayUIAnimation_WithChidren(sWSub_MSGContent.gameObject, strMot);
				num5 -= sWSub_MSGContent.rectTransform.rect.height;
			}
		}
	}

	private SWSub_MSGContent GetEmptyContentObject(MSGMenuPlus.ContentInfo contentInfo)
	{
		List<SWSub_MSGContent> list = null;
		switch (contentInfo.contentType)
		{
		case MSGContentPlus.MSGContentType.Me:
		case MSGContentPlus.MSGContentType.MeFrist:
			list = m_Contents_Me;
			break;
		case MSGContentPlus.MSGContentType.Partner:
			list = m_Contents_Partner;
			break;
		case MSGContentPlus.MSGContentType.PartnerFirst:
			list = m_Contents_PartnerF;
			break;
		}
		int count = list.Count;
		SWSub_MSGContent sWSub_MSGContent = null;
		for (int i = 0; i < count; i++)
		{
			if (!list[i].gameObject.activeSelf)
			{
				sWSub_MSGContent = list[i];
				break;
			}
		}
		if (sWSub_MSGContent == null)
		{
			GameObject gameObject = null;
			switch (contentInfo.contentType)
			{
			case MSGContentPlus.MSGContentType.Me:
			case MSGContentPlus.MSGContentType.MeFrist:
				gameObject = Object.Instantiate(m_SrcObjMe);
				break;
			case MSGContentPlus.MSGContentType.Partner:
				gameObject = Object.Instantiate(m_SrcObjPartner);
				break;
			case MSGContentPlus.MSGContentType.PartnerFirst:
				gameObject = Object.Instantiate(m_SrcObjPartnerF);
				break;
			}
			sWSub_MSGContent = gameObject.GetComponent<SWSub_MSGContent>();
			list.Add(sWSub_MSGContent);
		}
		FontManager.ResetTextFontByCurrentLanguage(sWSub_MSGContent.m_ContentText);
		FontManager.ResetTextFontByCurrentLanguage(sWSub_MSGContent.m_ProfileText);
		sWSub_MSGContent.SetValue(contentInfo);
		return sWSub_MSGContent;
	}

	public void OnNotice_ChangedMessengerMode(MSGMenuPlus.Mode mode)
	{
		if (!m_isSimpleView)
		{
			ChangeMode(mode);
		}
	}

	private void ChangeMode(MSGMenuPlus.Mode mode)
	{
		switch (mode)
		{
		case MSGMenuPlus.Mode.SelChatRoom:
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: false);
			}
			if (m_ChatroomSelMode != null)
			{
				m_ChatroomSelMode.SetActive(value: true);
			}
			break;
		case MSGMenuPlus.Mode.InChatRoom:
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: true);
			}
			if (m_ChatroomSelMode != null)
			{
				m_ChatroomSelMode.SetActive(value: false);
			}
			break;
		default:
			if (m_ContentContainerRT != null)
			{
				m_ContentContainerRT.gameObject.SetActive(value: false);
			}
			if (m_ChatroomSelMode != null)
			{
				m_ChatroomSelMode.SetActive(value: false);
			}
			break;
		}
		m_curMode = mode;
	}
}
