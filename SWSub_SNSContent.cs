using System;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_SNSContent : MonoBehaviour
{
	public enum ContentType
	{
		Normal,
		Retweeted
	}

	[Serializable]
	public class ContentInfo
	{
		public GameObject m_ContentRoot;

		public Text m_UserNameText;

		public Text m_RetweetCountText;

		public Text m_ContentText;
	}

	public ContentInfo m_NormalContent = new ContentInfo();

	public ContentInfo m_RetweetedContent = new ContentInfo();

	private ContentInfo m_CurContentInfo;

	private bool m_isInitailized;

	private RectTransform m_rectTransform;

	public bool isInitailized => m_isInitailized;

	public RectTransform rectTransform
	{
		get
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			return m_rectTransform;
		}
	}

	private void Start()
	{
		m_isInitailized = true;
	}

	public void OnDestroy()
	{
		m_CurContentInfo = null;
		m_rectTransform = null;
	}

	public void SetValue(Xls.SNSPostData postData)
	{
		ContentType contentType = ((postData.m_iPostType == 2) ? ContentType.Retweeted : ContentType.Normal);
		m_CurContentInfo = ((contentType != ContentType.Normal) ? m_RetweetedContent : m_NormalContent);
		m_NormalContent.m_ContentRoot.SetActive(m_CurContentInfo == m_NormalContent);
		m_RetweetedContent.m_ContentRoot.SetActive(m_CurContentInfo == m_RetweetedContent);
		m_CurContentInfo.m_UserNameText.text = SNSContentPlus.GetProfileText(postData);
		m_CurContentInfo.m_RetweetCountText.text = SNSContentPlus.GetRetweetCount(postData).ToString();
		m_CurContentInfo.m_ContentText.text = SNSContentPlus.GetContentText(postData);
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_CurContentInfo.m_ContentRoot, GameDefine.UIAnimationState.idle.ToString());
	}
}
