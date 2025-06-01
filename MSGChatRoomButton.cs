using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSGChatRoomButton : MonoBehaviour
{
	[Serializable]
	public class ContentInfo
	{
		public GameObject m_RootObject;

		public GameObject m_SelctionCursor;

		public Text m_Text;

		public Image m_Image;

		public GameObject m_EmptyMask;

		public GameObject m_Banner;

		public GameObject m_EmptyBanner;
	}

	public ContentInfo m_SelContentInfo = new ContentInfo();

	public ContentInfo m_NotSelContentInfo = new ContentInfo();

	private ContentInfo m_CurContentInfo;

	private GameDefine.EventProc m_EventProc_Clicked;

	private RectTransform m_RectTransform;

	private Xls.MessengerChatroomData m_XlsData;

	private List<Xls.MessengerTalkData> m_XlsTalkDatas = new List<Xls.MessengerTalkData>();

	public string text
	{
		get
		{
			return m_SelContentInfo.m_Text.text;
		}
		set
		{
			m_SelContentInfo.m_Text.text = value;
			m_NotSelContentInfo.m_Text.text = value;
		}
	}

	public Sprite image
	{
		get
		{
			return m_SelContentInfo.m_Image.sprite;
		}
		set
		{
			m_SelContentInfo.m_Image.sprite = value;
			m_NotSelContentInfo.m_Image.sprite = value;
		}
	}

	public bool selected
	{
		get
		{
			return m_CurContentInfo == m_SelContentInfo;
		}
		set
		{
			m_CurContentInfo = ((!value) ? m_NotSelContentInfo : m_SelContentInfo);
			m_SelContentInfo.m_RootObject.SetActive(m_CurContentInfo == m_SelContentInfo);
			m_NotSelContentInfo.m_RootObject.SetActive(m_CurContentInfo == m_NotSelContentInfo);
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_SelContentInfo.m_RootObject, strMot);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_NotSelContentInfo.m_RootObject, strMot);
		}
	}

	public bool activeBanner
	{
		get
		{
			return m_SelContentInfo.m_Banner.activeSelf;
		}
		set
		{
			m_SelContentInfo.m_Banner.SetActive(value);
			m_NotSelContentInfo.m_Banner.SetActive(value);
		}
	}

	public bool activeEmptyIcon
	{
		get
		{
			return m_SelContentInfo.m_EmptyBanner.activeSelf;
		}
		set
		{
			m_SelContentInfo.m_EmptyBanner.SetActive(value);
			m_SelContentInfo.m_EmptyMask.SetActive(value);
			m_NotSelContentInfo.m_EmptyBanner.SetActive(value);
			m_NotSelContentInfo.m_EmptyMask.SetActive(value);
		}
	}

	public bool acitveOnCursor
	{
		get
		{
			return m_SelContentInfo.m_SelctionCursor.activeSelf;
		}
		set
		{
			m_SelContentInfo.m_SelctionCursor.SetActive(value);
		}
	}

	public GameDefine.EventProc onClickedProc
	{
		get
		{
			return m_EventProc_Clicked;
		}
		set
		{
			m_EventProc_Clicked = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public Xls.MessengerChatroomData xlsChatroomData => m_XlsData;

	public List<Xls.MessengerTalkData> xlsTalkDatas => m_XlsTalkDatas;

	private void OnDestroy()
	{
		m_CurContentInfo = null;
		m_EventProc_Clicked = null;
		m_RectTransform = null;
		m_XlsData = null;
		if (m_XlsTalkDatas != null)
		{
			m_XlsTalkDatas.Clear();
		}
	}

	public void OnClick()
	{
		if (m_EventProc_Clicked != null)
		{
			m_EventProc_Clicked(this, this);
		}
	}

	public IEnumerator InitValue(Xls.MessengerChatroomData xlsData)
	{
		m_XlsData = xlsData;
		FontManager.ResetTextFontByCurrentLanguage(m_SelContentInfo.m_Text);
		FontManager.ResetTextFontByCurrentLanguage(m_NotSelContentInfo.m_Text);
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsData.m_strTextID);
		text = data_byKey.m_strTxt;
		Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(xlsData.m_strProfileImagePath);
		string strAssetPath = data_byKey2.m_strAssetPath;
		image = MainLoadThing.instance.faterProfileImageManager.GetThumbnailImageInCache(strAssetPath);
		m_CurContentInfo = m_NotSelContentInfo;
		m_NotSelContentInfo.m_RootObject.SetActive(value: true);
		m_SelContentInfo.m_RootObject.SetActive(value: false);
		yield break;
	}

	public List<Xls.MessengerTalkData> GetTalkData_bySequence(string seqKey)
	{
		int count = m_XlsTalkDatas.Count;
		if (count <= 0)
		{
			return null;
		}
		Xls.MessengerTalkData messengerTalkData = null;
		List<Xls.MessengerTalkData> list = new List<Xls.MessengerTalkData>();
		for (int i = 0; i < count; i++)
		{
			messengerTalkData = m_XlsTalkDatas[i];
			if (messengerTalkData.m_strIDSeq == seqKey)
			{
				list.Add(messengerTalkData);
			}
		}
		return list;
	}
}
