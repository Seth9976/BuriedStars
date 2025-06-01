using UnityEngine;
using UnityEngine.UI;

public class KeywordSlotPlus : MonoBehaviour
{
	public KeywordMenuPlus m_KeywordMenu;

	public GameObject m_goSelObj;

	public GameObject m_goNSelObj;

	public GameObject m_goShowButton;

	public GameObject m_goSelImgGroup;

	public GameObject m_goNewButton;

	public GameObject m_goLockedGroup;

	public GameObject m_goLockedSlot;

	public GameObject m_goLockedSelSlot;

	public GameObject m_goEmptySlot;

	public RectTransform m_rtfDragImgGroup;

	public Image m_imgSelCircle;

	public Image m_imgNSelCircle;

	public Image m_imgSelSlot;

	public Image m_imgNSelSlot;

	public Text m_textSelTitle;

	public Text m_textNSelTitle;

	public Text m_textMaskSelTitle;

	public Text m_textMaskNSelTitle;

	public Text m_textButton;

	public Animator m_animSuggestBut;

	[Header("Arrange")]
	public GameObject m_goMarkedAnswerIcon;

	public Animator m_animMarkedAnswerIcon;

	[Header("O Button")]
	public Button m_butConfirm;

	[Header("Animator")]
	public Animator m_AnimKeywordSlot;

	public Animator m_AnimButton;

	[HideInInspector]
	public string m_strKeywordKey;

	private bool m_isMarkedAnswerIcon;

	private bool m_isMaskOn;

	private Button m_btnComp;

	private RectTransform m_RectTransform;

	private Xls.CollKeyword m_xlsData;

	private bool m_isValid = true;

	private bool m_isEnableNewMark = true;

	private Image m_slotBGImage;

	private GameDefine.EventProc m_onNoticeClicked;

	public Button ButtonComp
	{
		get
		{
			if (m_btnComp == null)
			{
				m_btnComp = base.gameObject.GetComponent<Button>();
			}
			return m_btnComp;
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

	public Xls.CollKeyword xlsData => m_xlsData;

	public bool isValid => m_isValid;

	public bool isEnableNewMark
	{
		get
		{
			return m_isEnableNewMark;
		}
		set
		{
			m_isEnableNewMark = value;
			SetNewMark(m_isEnableNewMark);
		}
	}

	public GameDefine.EventProc OnNoticeClicked
	{
		set
		{
			m_onNoticeClicked = value;
		}
	}

	private void OnEnable()
	{
		SetMaskWidth();
		Text[] textComps = new Text[5] { m_textSelTitle, m_textNSelTitle, m_textMaskSelTitle, m_textMaskNSelTitle, m_textButton };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void SetMaskWidth()
	{
		if (KeywordMenuPlus.m_fSlotTextMaskWidth == -1f)
		{
			KeywordMenuPlus.m_fSlotTextMaskWidth = m_textMaskSelTitle.transform.parent.GetComponent<RectTransform>().rect.width;
		}
	}

	private void OnDestroy()
	{
		m_RectTransform = null;
		m_xlsData = null;
		m_slotBGImage = null;
		m_onNoticeClicked = null;
	}

	public void InitSlotState(bool isDragSlot = false, bool isPlaySlotAppearMot = true)
	{
		if (m_slotBGImage != null)
		{
			m_slotBGImage.enabled = true;
		}
		if (isDragSlot)
		{
			m_imgNSelCircle.enabled = false;
			m_imgSelCircle.enabled = false;
		}
		m_goSelImgGroup.SetActive(value: false);
		m_goNSelObj.SetActive(value: false);
		m_goShowButton.SetActive(value: false);
		m_goNewButton.SetActive(value: false);
		m_goLockedGroup.SetActive(value: false);
		m_goLockedSelSlot.SetActive(value: false);
		m_goLockedSlot.SetActive(value: false);
		m_goEmptySlot.SetActive(value: false);
		m_goSelObj.SetActive(value: false);
		m_goMarkedAnswerIcon.SetActive(value: false);
		if (isPlaySlotAppearMot)
		{
			GameGlobalUtil.PlayUIAnimation(m_AnimKeywordSlot, GameDefine.UIAnimationState.appear);
		}
		m_isMarkedAnswerIcon = false;
	}

	public void DisappearSlot(bool isSel, bool isShowButtonPressAnim = false)
	{
		if (isShowButtonPressAnim)
		{
			if (m_AnimButton.gameObject.activeInHierarchy)
			{
				GameGlobalUtil.PlayUIAnimation(m_AnimButton, GameDefine.UIAnimationState.disappear);
			}
		}
		else
		{
			GameGlobalUtil.PlayUIAnimation(m_AnimButton, GameDefine.UIAnimationState.disappear);
		}
		GameGlobalUtil.PlayUIAnimation(m_AnimKeywordSlot, GameDefine.UIAnimationState.disappear);
		if (m_animMarkedAnswerIcon.gameObject.activeInHierarchy)
		{
			GameGlobalUtil.PlayUIAnimation(m_animMarkedAnswerIcon, GameDefine.UIAnimationState.disappear);
		}
	}

	public void SetSlotButtonText(string strText)
	{
		m_textButton.text = strText;
	}

	public void SetSlotButtonTextAndColor(string strText, Color color)
	{
		m_textButton.text = strText;
		m_textButton.color = color;
	}

	public void SetSlotImage(Sprite sprSel, Sprite sprNSel)
	{
		m_imgSelSlot.sprite = sprSel;
		m_imgNSelSlot.sprite = sprNSel;
	}

	public Sprite GetSelImg()
	{
		return m_imgSelSlot.sprite;
	}

	public void SelSlot(bool isSel, bool isSetShow = false, bool isShow = false, bool isSetEmptySlot = false, bool isEmptyState = false)
	{
		if (m_goLockedGroup.activeSelf || (isSetEmptySlot && isEmptyState))
		{
			m_goLockedSelSlot.SetActive(isSel);
			m_goLockedSlot.SetActive(!isSel);
			m_goSelObj.SetActive(value: false);
		}
		else
		{
			m_goNSelObj.SetActive(!isSel);
			m_goShowButton.SetActive((!isSetShow) ? isSel : isShow);
			m_goSelImgGroup.SetActive(isSel);
			m_goSelObj.SetActive(isSel);
		}
		if (isSel && m_isMaskOn)
		{
			base.gameObject.GetComponent<Animator>().Play("idle", -1, 0f);
		}
	}

	public void SetLockSlot()
	{
		InitSlotState();
		m_goLockedGroup.SetActive(value: true);
		m_goLockedSlot.SetActive(value: true);
	}

	public void SetEmptySlot()
	{
		InitSlotState();
		m_goEmptySlot.SetActive(value: true);
	}

	public void SetNewMark(bool isOn)
	{
		m_goNewButton.SetActive(isOn);
	}

	public void PressSuggestBut()
	{
		GameGlobalUtil.PlayUIAnimation(m_animSuggestBut, "steam_push");
	}

	public void SetArrangeSelMark(bool isOn)
	{
		m_isMarkedAnswerIcon = isOn;
		m_goMarkedAnswerIcon.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation(m_animMarkedAnswerIcon, (!isOn) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
	}

	public bool GetArrangeSelMark()
	{
		return m_isMarkedAnswerIcon;
	}

	public bool IsUsableSlot()
	{
		return !m_goEmptySlot.activeInHierarchy && !m_goLockedGroup.activeInHierarchy;
	}

	public bool IsSelectableSlot()
	{
		return !m_goEmptySlot.activeInHierarchy;
	}

	public bool IsEmptySlot()
	{
		return m_goEmptySlot.activeInHierarchy;
	}

	public void ClickSlot()
	{
		if (KeywordMenuPlus.m_isKewordMenuPlusOn)
		{
			m_KeywordMenu.SelKeywordSlot(KeywordMenuPlus.eKeywordTouch.Click, this);
		}
		else if (m_onNoticeClicked != null)
		{
			m_onNoticeClicked(this, null);
		}
	}

	public void ClickShowButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && KeywordMenuPlus.m_isKewordMenuPlusOn)
		{
			AudioManager.instance.PlayUISound("Push_SubmissBTN");
			if (KeywordMenuPlus.m_eKeywordState == KeywordMenuPlus.KeywordState.ARRANGE_MULTI)
			{
				ClickSlot();
			}
			else
			{
				m_KeywordMenu.SelKeywordSlot(KeywordMenuPlus.eKeywordTouch.RunEvent, this);
			}
		}
	}

	public void TouchSlot()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			ClickSlot();
		}
	}

	public void PressConfirmButton()
	{
		ButtonPadInput.AddAndShowPressAnim(m_butConfirm, PadInput.ButtonState.Down, null, null, null, isCallEndFunc: false);
	}

	private void Awake()
	{
		m_slotBGImage = base.gameObject.GetComponent<Image>();
	}

	public void SetMastTextOrNotMaskText(string strText)
	{
		Text textSelTitle = m_textSelTitle;
		string text = strText;
		m_textNSelTitle.text = text;
		textSelTitle.text = text;
		bool flag = false;
		Text textMaskSelTitle = m_textMaskSelTitle;
		text = strText;
		m_textMaskNSelTitle.text = text;
		textMaskSelTitle.text = text;
		SetMaskWidth();
		RectTransform component = m_textMaskSelTitle.transform.parent.GetComponent<RectTransform>();
		if (m_textMaskSelTitle.preferredWidth > KeywordMenuPlus.m_fSlotTextMaskWidth)
		{
			flag = true;
		}
		m_textMaskNSelTitle.gameObject.SetActive(flag);
		m_textMaskSelTitle.gameObject.SetActive(flag);
		m_textNSelTitle.gameObject.SetActive(!flag);
		m_textSelTitle.gameObject.SetActive(!flag);
		m_isMaskOn = flag;
	}

	public void InitCollectionContent(Xls.CollKeyword _xlsData, bool _isValid = true, bool enableNewTag = false, bool isIgnoreSame = true)
	{
		if (m_xlsData == _xlsData && isIgnoreSame)
		{
			return;
		}
		m_xlsData = _xlsData;
		m_goSelImgGroup.SetActive(value: false);
		m_goNSelObj.SetActive(value: false);
		m_goShowButton.SetActive(value: false);
		m_goNewButton.SetActive(value: false);
		m_goLockedGroup.SetActive(value: false);
		m_goLockedSlot.SetActive(value: false);
		m_goEmptySlot.SetActive(value: false);
		FontManager.ResetTextFontByCurrentLanguage(m_textSelTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_textNSelTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_textMaskNSelTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_textMaskSelTitle);
		FontManager.ResetTextFontByCurrentLanguage(m_textButton);
		if (m_slotBGImage != null)
		{
			m_slotBGImage.enabled = true;
		}
		string mastTextOrNotMaskText = string.Empty;
		if (m_xlsData == null)
		{
			SetEmptySlot();
			m_isValid = false;
			m_isEnableNewMark = false;
		}
		else
		{
			m_goEmptySlot.SetActive(value: false);
			m_isValid = _isValid;
			if (!m_isValid)
			{
				SetLockSlot();
			}
			else
			{
				m_isEnableNewMark = enableNewTag;
				SetNewMark(enableNewTag);
				Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(m_xlsData.m_strIconImgID);
				if (data_byKey != null)
				{
					string strAssetPath = data_byKey.m_strAssetPath;
					m_imgSelSlot.sprite = MainLoadThing.instance.keywordIconImageManager.GetThumbnailImageInCache(strAssetPath + "_s");
					m_imgNSelSlot.sprite = MainLoadThing.instance.keywordIconImageManager.GetThumbnailImageInCache(strAssetPath);
				}
				Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(m_xlsData.m_strTitleID);
				if (data_byKey2 != null)
				{
					mastTextOrNotMaskText = data_byKey2.m_strTitle;
				}
			}
		}
		SetMastTextOrNotMaskText(mastTextOrNotMaskText);
		SetSelect_forCollectionContent(_isSelect: false);
	}

	public void SetSelect_forCollectionContent(bool _isSelect)
	{
		if (m_xlsData == null)
		{
			SetEmptySlot();
		}
		else
		{
			SelSlot(_isSelect, isSetShow: true);
		}
	}

	public void HideCollectionContent()
	{
		SetEmptySlot();
		if (m_slotBGImage != null)
		{
			m_slotBGImage.enabled = false;
		}
	}

	public void ShowCollectionContent()
	{
		InitCollectionContent(m_xlsData, m_isValid, m_isEnableNewMark, isIgnoreSame: false);
	}
}
