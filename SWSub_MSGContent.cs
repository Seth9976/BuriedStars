using UnityEngine;
using UnityEngine.UI;

public class SWSub_MSGContent : MonoBehaviour
{
	public Text m_ContentText;

	public GameObject m_BoxTailObj;

	public Image m_ProfileImage;

	public Text m_ProfileText;

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

	private void OnDestroy()
	{
		m_rectTransform = null;
	}

	public void SetValue(MSGMenuPlus.ContentInfo contentInfo)
	{
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(contentInfo.xlsData.m_strIDText);
		m_ContentText.text = ((data_byKey == null) ? string.Empty : data_byKey.m_strTxt);
		if (m_BoxTailObj != null)
		{
			switch (contentInfo.contentType)
			{
			case MSGContentPlus.MSGContentType.Me:
			case MSGContentPlus.MSGContentType.Partner:
				m_BoxTailObj.SetActive(value: false);
				break;
			case MSGContentPlus.MSGContentType.MeFrist:
			case MSGContentPlus.MSGContentType.PartnerFirst:
				m_BoxTailObj.SetActive(value: true);
				break;
			}
		}
		Xls.AccountData data_byKey2 = Xls.AccountData.GetData_byKey(contentInfo.xlsData.m_strIDAcc);
		if (data_byKey2 == null)
		{
			return;
		}
		if (m_ProfileText != null)
		{
			Xls.TextListData data_byKey3 = Xls.TextListData.GetData_byKey(data_byKey2.m_nicknameID);
			m_ProfileText.text = ((data_byKey3 == null) ? string.Empty : data_byKey3.m_strTitle);
		}
		if (m_ProfileImage != null)
		{
			Sprite sprite = null;
			Xls.ImageFile data_byKey4 = Xls.ImageFile.GetData_byKey(data_byKey2.m_snspicID);
			if (data_byKey4 != null && MainLoadThing.instance != null && MainLoadThing.instance.faterProfileImageManager != null)
			{
				sprite = MainLoadThing.instance.faterProfileImageManager.GetThumbnailImageInCache(data_byKey4.m_strAssetPath);
			}
			m_ProfileImage.sprite = sprite;
		}
	}
}
