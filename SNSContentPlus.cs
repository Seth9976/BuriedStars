using System;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SNSContentPlus : MonoBehaviour, IFloatingUIObject
{
	public enum SNSContentType
	{
		Normal,
		Reply,
		Shared
	}

	[Serializable]
	public class ContentMainObj
	{
		public GameObject m_RootObject;

		public Text m_ContentText;
	}

	[Serializable]
	public class ButtonGroup
	{
		public enum State
		{
			Hide,
			Normal,
			OnCursor,
			Disable
		}

		public Button m_Normal;

		public Button m_OnCursor;

		public Button m_Disable;

		private State m_State;

		public State state => m_State;

		public Button activedButton => m_State switch
		{
			State.Hide => null, 
			State.Normal => (!(m_Normal != null) || !m_Normal.gameObject.activeSelf) ? null : m_Normal, 
			State.OnCursor => (m_OnCursor != null && m_OnCursor.gameObject.activeSelf) ? m_OnCursor : ((!(m_Normal != null) || !m_Normal.gameObject.activeSelf) ? null : m_Normal), 
			State.Disable => (!(m_Disable != null) || !m_Disable.gameObject.activeSelf) ? null : m_Disable, 
			_ => null, 
		};

		public void SetState(State state)
		{
			switch (state)
			{
			case State.Hide:
				if (m_Normal != null)
				{
					m_Normal.gameObject.SetActive(value: false);
				}
				if (m_OnCursor != null)
				{
					m_OnCursor.gameObject.SetActive(value: false);
				}
				if (m_Disable != null)
				{
					m_Disable.gameObject.SetActive(value: false);
				}
				break;
			case State.Normal:
				if (m_Normal != null)
				{
					m_Normal.gameObject.SetActive(value: true);
				}
				if (m_OnCursor != null)
				{
					m_OnCursor.gameObject.SetActive(value: false);
				}
				if (m_Disable != null)
				{
					m_Disable.gameObject.SetActive(value: false);
				}
				break;
			case State.OnCursor:
				if (m_Normal != null)
				{
					m_Normal.gameObject.SetActive(value: false);
				}
				if (m_OnCursor != null)
				{
					m_OnCursor.gameObject.SetActive(value: true);
				}
				else if (m_Normal != null)
				{
					m_Normal.gameObject.SetActive(value: true);
				}
				if (m_Disable != null)
				{
					m_Disable.gameObject.SetActive(value: false);
				}
				break;
			case State.Disable:
				if (m_Normal != null)
				{
					m_Normal.gameObject.SetActive(value: false);
				}
				if (m_OnCursor != null)
				{
					m_OnCursor.gameObject.SetActive(value: false);
				}
				if (m_Disable != null)
				{
					m_Disable.gameObject.SetActive(value: true);
				}
				break;
			}
			m_State = state;
		}
	}

	private const char c_AccountTextBeginSymbol = '[';

	private const char c_AccountTextEndSymbol = ']';

	private const char c_AccountTextBoundNickName = '@';

	private const string c_xlsDataName_SharedPostProfileFormat = "SNS_SHARED_POST_PROFILE_TEXT";

	private const string c_xlsDataName_SharedPostPAccountFormat = "SNS_SHARED_POST_ACCOUNT_TEXT";

	private const string c_xlsDataName_NormalPostProfileFormat = "SNS_NORMAL_POST_PROFILE_TEXT";

	public SNSContentType m_Type;

	public GameObject m_ForEventBG;

	[Header("Content Area")]
	public ContentMainObj m_NoImageRoot = new ContentMainObj();

	public ContentMainObj m_ImageRoot = new ContentMainObj();

	public GameObject m_SelectionCursor;

	public GameObject m_ContentImageRoot;

	public Image m_ContentImage;

	public Button m_ImageViewButton;

	public Text m_TextPromotedBy;

	public Text m_TextTime;

	[Header("Profile Area")]
	public Image m_ProfileImage;

	public Text m_ProfileText;

	public GameObject m_RetweetRoot;

	public Text m_RetweetText;

	public GameObject m_RetweetMineRoot;

	public Text m_RetweetMineText;

	[Header("Buttons")]
	public ButtonGroup m_KeywordButton;

	public ButtonGroup m_ReplyButton;

	public ButtonGroup m_AdViewButton;

	public Button m_SelectionIconButton;

	public Text m_ButtonStateText;

	[Header("Shared Post Special")]
	public Text m_OriginAccountText;

	private Xls.SNSPostData m_XlsData;

	private bool m_isExistImage;

	private SNSMenuPlus m_snsMenu;

	private bool m_isInitailized;

	private ButtonGroup m_curActivedButtonGroup;

	private bool m_isOnCursor;

	private bool m_isRetweeted;

	private GameObject m_activedRetweetRoot;

	private Text m_activedRetweetText;

	private int m_indexInContentInfos = -1;

	private RectTransform m_rt;

	private Animator m_foAnimator;

	private FloatingUIHandler m_foHandler;

	private List<FloatingUIRoot.EventBase> m_foEvents;

	private Vector3 m_foRotateAngle;

	private FloatingUIRoot.ScalingParams m_foScalingParams;

	private int m_foMotionLoopCount;

	private bool m_foIsMotionComplete;

	private string m_foTag = string.Empty;

	public const string c_AssetName_SNSContent_Normal = "Prefabs/InGame/Menu/SNSContent_Normal";

	private static UnityEngine.Object s_SNSContent_Normal_SrcObj;

	public const string c_AssetName_SNSContent_Reply = "Prefabs/InGame/Menu/SNSContent_Reply";

	private static UnityEngine.Object s_SNSContent_Reply_SrcObj;

	public const string c_AssetName_SNSContent_Shared = "Prefabs/InGame/Menu/SNSContent_Shared";

	private static UnityEngine.Object s_SNSContent_Shared_SrcObj;

	private static bool s_initializedPrefabs;

	public Xls.SNSPostData xlsData => m_XlsData;

	public bool isExistImage => m_isExistImage;

	public SNSMenuPlus linkedSnsMenu
	{
		set
		{
			m_snsMenu = value;
		}
	}

	public bool isInitailized => m_isInitailized;

	public ButtonGroup curActivedButtonGroup => m_curActivedButtonGroup;

	public Button curActivedButton => (m_curActivedButtonGroup == null) ? null : m_curActivedButtonGroup.activedButton;

	public bool isKeywordContained => m_curActivedButtonGroup != null && m_curActivedButtonGroup == m_KeywordButton;

	public bool isReplyContained => m_curActivedButtonGroup != null && m_curActivedButtonGroup == m_ReplyButton;

	public bool isOnCursor => m_isOnCursor;

	public GameObject contentRoot => (!m_isExistImage) ? m_NoImageRoot.m_RootObject : m_ImageRoot.m_RootObject;

	public Text contentTextComp => (!m_isExistImage) ? m_NoImageRoot.m_ContentText : m_ImageRoot.m_ContentText;

	public SNSContentType type => m_Type;

	public string text
	{
		get
		{
			return contentTextComp.text;
		}
		set
		{
			SetContentText(value);
		}
	}

	public Sprite imageThumbnail
	{
		get
		{
			return m_ContentImage.sprite;
		}
		set
		{
			m_ContentImage.sprite = value;
			if (value != null && m_ContentImageRoot != null)
			{
				m_ContentImage.color = Color.white;
				GameGlobalUtil.AddEventTrigger(m_ContentImageRoot, EventTriggerType.PointerClick, OnClick_ThumbnailImage);
			}
		}
	}

	public string profileText
	{
		get
		{
			return m_ProfileText.text;
		}
		set
		{
			m_ProfileText.text = value;
		}
	}

	public Sprite profileThumbnail
	{
		get
		{
			return m_ProfileImage.sprite;
		}
		set
		{
			m_ProfileImage.sprite = value;
			m_ProfileImage.gameObject.SetActive(value != null);
		}
	}

	public int retweetCount
	{
		get
		{
			return int.Parse(m_activedRetweetText.text);
		}
		set
		{
			m_activedRetweetText.text = value.ToString();
		}
	}

	public string retweetCountText => m_activedRetweetText.text;

	public string timeText
	{
		get
		{
			return m_TextTime.text;
		}
		set
		{
			m_TextTime.text = value;
		}
	}

	public string promotionText
	{
		get
		{
			return (!(m_TextPromotedBy != null)) ? string.Empty : m_TextPromotedBy.text;
		}
		set
		{
			if (!(m_TextPromotedBy == null))
			{
				m_TextPromotedBy.text = value;
				m_TextPromotedBy.gameObject.SetActive(!string.IsNullOrEmpty(value));
			}
		}
	}

	public bool isReadable
	{
		get
		{
			if (m_XlsData == null)
			{
				return false;
			}
			GameSwitch instance = GameSwitch.GetInstance();
			Xls.SequenceData data_byKey = Xls.SequenceData.GetData_byKey(m_XlsData.m_strIDSeq);
			return instance.GetCurSequence() == data_byKey.m_iIdx && instance.GetCurPhase() == m_XlsData.m_iPhase;
		}
	}

	public bool isRetweeted => m_isRetweeted;

	public int indexInContentInfos
	{
		get
		{
			return m_indexInContentInfos;
		}
		set
		{
			m_indexInContentInfos = value;
		}
	}

	public GameObject foGameObject => base.gameObject;

	public RectTransform foRectTransform
	{
		get
		{
			if (m_rt == null)
			{
				m_rt = base.gameObject.GetComponent<RectTransform>();
			}
			return m_rt;
		}
	}

	public Animator foAnimator
	{
		get
		{
			return m_foAnimator;
		}
		set
		{
			m_foAnimator = value;
		}
	}

	public FloatingUIHandler foHandler
	{
		get
		{
			return m_foHandler;
		}
		set
		{
			m_foHandler = value;
		}
	}

	public List<FloatingUIRoot.EventBase> foEvents
	{
		get
		{
			if (m_foEvents == null)
			{
				m_foEvents = new List<FloatingUIRoot.EventBase>();
			}
			return m_foEvents;
		}
	}

	public Vector3 foRotateAngle
	{
		get
		{
			return m_foRotateAngle;
		}
		set
		{
			m_foRotateAngle = value;
		}
	}

	public FloatingUIRoot.ScalingParams foScalingParmas
	{
		get
		{
			if (m_foScalingParams == null)
			{
				m_foScalingParams = new FloatingUIRoot.ScalingParams();
				m_foScalingParams.m_baseScale = foRectTransform.localScale;
			}
			return m_foScalingParams;
		}
	}

	public int foMotionLoopCount
	{
		get
		{
			return m_foMotionLoopCount;
		}
		set
		{
			m_foMotionLoopCount = value;
		}
	}

	public bool foIsMotionComplete
	{
		get
		{
			return m_foIsMotionComplete;
		}
		set
		{
			m_foIsMotionComplete = value;
		}
	}

	public string foTag
	{
		get
		{
			return m_foTag;
		}
		set
		{
			m_foTag = value;
		}
	}

	public Button foPsIconButton => m_SelectionIconButton;

	public static bool initializedPrefabs => s_initializedPrefabs;

	private void Awake()
	{
		if (m_SelectionIconButton != null)
		{
			m_SelectionIconButton.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		m_XlsData = null;
		m_snsMenu = null;
		m_curActivedButtonGroup = null;
		m_activedRetweetRoot = null;
		m_activedRetweetText = null;
		m_rt = null;
		m_foAnimator = null;
		if (m_foEvents != null)
		{
			m_foEvents.Clear();
		}
		m_foEvents = null;
		m_foScalingParams = null;
	}

	private void Start()
	{
		RectTransform component = contentTextComp.gameObject.GetComponent<RectTransform>();
		float b = 0f;
		if (m_isExistImage)
		{
			RectTransform component2 = m_ContentImageRoot.gameObject.GetComponent<RectTransform>();
			b = component2.rect.height;
		}
		float height = component.rect.height;
		float num = Mathf.Max(contentTextComp.preferredHeight, b);
		if (!GameGlobalUtil.IsAlmostSame(height, num))
		{
			float num2 = num - height;
			RectTransform component3 = base.gameObject.GetComponent<RectTransform>();
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component3.rect.height + num2);
		}
		m_isInitailized = true;
	}

	private void Update()
	{
	}

	private void ReflashTextMembers()
	{
		Text[] textComps = new Text[9] { m_NoImageRoot.m_ContentText, m_ImageRoot.m_ContentText, m_TextPromotedBy, m_TextTime, m_ProfileText, m_RetweetText, m_RetweetMineText, m_ButtonStateText, m_OriginAccountText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void InitProfileInfo()
	{
		string empty = string.Empty;
		Sprite sprite = null;
		if (m_XlsData != null)
		{
			Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(m_XlsData.m_strIDAcc);
			if (data_byKey != null)
			{
				empty = GetProfileText(m_XlsData);
				Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(data_byKey.m_snspicID);
				if (data_byKey2 != null)
				{
					sprite = MainLoadThing.instance.faterProfileImageManager.GetThumbnailImageInCache(data_byKey2.m_strAssetPath);
				}
			}
		}
		profileText = empty;
		profileThumbnail = sprite;
	}

	private void SetContentText(string strText)
	{
		GameSwitch instance = GameSwitch.GetInstance();
		bool flag = !string.IsNullOrEmpty(m_XlsData.m_strIDKeyword) && instance.GetKeywordAllState(m_XlsData.m_strIDKeyword) == 0;
		contentTextComp.text = TagText.TransTagTextToUnityText(strText, !flag);
		contentTextComp.CalculateLayoutInputVertical();
	}

	public void ResetContentText()
	{
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(m_XlsData.m_strIDText);
		ParseContentText((data_byKey == null) ? string.Empty : data_byKey.m_strTxt);
	}

	private void InitButtons(SNSMenuPlus.Mode menuMode, bool isForEvent)
	{
		HideButtons();
		if (m_XlsData == null || isForEvent)
		{
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		Xls.SequenceData data_byKey = Xls.SequenceData.GetData_byKey(m_XlsData.m_strIDSeq);
		if (data_byKey.m_iIdx == instance.GetCurSequence() && xlsData.m_iPhase == instance.GetCurPhase())
		{
			if (menuMode != SNSMenuPlus.Mode.WatchMenu)
			{
				if (!string.IsNullOrEmpty(m_XlsData.m_strIDReply) && xlsData.m_iIsSelPost == 0)
				{
					sbyte postReply = instance.GetPostReply(m_XlsData.m_strID);
					if (postReply == 0 && (m_XlsData.m_iReplyGroup < 0 || instance.GetReplyGroup(m_XlsData.m_iReplyGroup) == 0))
					{
						m_curActivedButtonGroup = m_ReplyButton;
					}
				}
				else if (!string.IsNullOrEmpty(m_XlsData.m_strIDKeyword) && instance.GetKeywordAllState(m_XlsData.m_strIDKeyword) == 0 && (m_XlsData.m_iGroupKeyword < 0 || instance.GetKeywordGroup(m_XlsData.m_iGroupKeyword) == 0))
				{
					m_curActivedButtonGroup = m_KeywordButton;
				}
			}
			if (m_curActivedButtonGroup == null && !string.IsNullOrEmpty(m_XlsData.m_strIDAds))
			{
				m_curActivedButtonGroup = m_AdViewButton;
			}
		}
		m_KeywordButton.SetState((m_curActivedButtonGroup == m_KeywordButton) ? ButtonGroup.State.Normal : ButtonGroup.State.Hide);
		m_ReplyButton.SetState((m_curActivedButtonGroup == m_ReplyButton) ? ButtonGroup.State.Normal : ButtonGroup.State.Hide);
		m_AdViewButton.SetState((m_curActivedButtonGroup == m_AdViewButton) ? ButtonGroup.State.Normal : ButtonGroup.State.Hide);
	}

	public void CheckValid_KeywordButton()
	{
		if (m_curActivedButtonGroup == m_KeywordButton)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			if (string.IsNullOrEmpty(m_XlsData.m_strIDKeyword) || instance.GetKeywordAllState(m_XlsData.m_strIDKeyword) != 0 || (m_XlsData.m_iGroupKeyword >= 0 && instance.GetKeywordGroup(m_XlsData.m_iGroupKeyword) != 0))
			{
				m_curActivedButtonGroup.SetState(ButtonGroup.State.Hide);
				m_curActivedButtonGroup = null;
			}
		}
	}

	public void CheckValid_ReplyButton()
	{
		if (m_curActivedButtonGroup == m_ReplyButton)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			if (string.IsNullOrEmpty(m_XlsData.m_strIDReply) || instance.GetPostReply(m_XlsData.m_strID) != 0 || (m_XlsData.m_iReplyGroup >= 0 && instance.GetReplyGroup(m_XlsData.m_iReplyGroup) != 0))
			{
				m_curActivedButtonGroup.SetState(ButtonGroup.State.Hide);
				m_curActivedButtonGroup = null;
			}
		}
	}

	public virtual void HideButtons()
	{
		m_KeywordButton.SetState(ButtonGroup.State.Hide);
		m_ReplyButton.SetState(ButtonGroup.State.Hide);
		m_AdViewButton.SetState(ButtonGroup.State.Hide);
		m_curActivedButtonGroup = null;
	}

	public bool OnProc_ButtonClickEvent()
	{
		if (m_curActivedButtonGroup == null)
		{
			return false;
		}
		if (m_curActivedButtonGroup == m_KeywordButton)
		{
			OnClick_KeywordButton();
			return true;
		}
		if (m_curActivedButtonGroup == m_ReplyButton)
		{
			OnClick_ReplyButton();
			return true;
		}
		if (m_curActivedButtonGroup == m_AdViewButton)
		{
			OnClick_AdViewButton();
			return true;
		}
		return false;
	}

	public void OnClick_KeywordButton()
	{
		if (!(m_snsMenu == null) && m_curActivedButtonGroup != null && m_curActivedButtonGroup.state != ButtonGroup.State.Hide && m_curActivedButtonGroup.state != ButtonGroup.State.Disable)
		{
			m_snsMenu.OnProc_KeywordGetButton(this);
		}
	}

	public void OnClick_ReplyButton()
	{
		if (!(m_snsMenu == null))
		{
			m_snsMenu.OnProc_ReplyButton(this);
		}
	}

	public void OnClick_AdViewButton()
	{
		if (!(m_snsMenu == null))
		{
			m_snsMenu.OnProc_ViewAdButton(this);
		}
	}

	public void OnClick_ThumbnailImage(BaseEventData evtData)
	{
		if (!(m_snsMenu == null))
		{
			m_snsMenu.StartCoroutine(m_snsMenu.OnProc_ViewImageDetail(this));
		}
	}

	private void ParseContentText(string contentText)
	{
		ParseContentText(contentText, m_XlsData.m_iPostType, out var outText, out var outOriginAccount);
		text = outText;
		if (m_OriginAccountText != null)
		{
			m_OriginAccountText.text = outOriginAccount;
		}
	}

	public void SetState(bool _isOnCursor, bool isEnabled = true, string buttonStateText = "")
	{
		m_isOnCursor = _isOnCursor;
		if (m_SelectionCursor != null)
		{
			m_SelectionCursor.SetActive(_isOnCursor);
		}
		if (isExistImage && m_ImageViewButton != null)
		{
			m_ImageViewButton.gameObject.SetActive(_isOnCursor);
		}
		if (m_curActivedButtonGroup != null)
		{
			m_curActivedButtonGroup.SetState((!_isOnCursor) ? ButtonGroup.State.Normal : ButtonGroup.State.OnCursor);
		}
		if (m_ButtonStateText != null && !string.IsNullOrEmpty(buttonStateText))
		{
			m_ButtonStateText.text = buttonStateText;
		}
	}

	public void SetContentData(Xls.SNSPostData xlsData, float posY = 0f, float height = 0f, SNSMenuPlus.Mode menuMode = SNSMenuPlus.Mode.WatchMenu)
	{
		m_XlsData = xlsData;
		if (m_XlsData == null)
		{
			return;
		}
		m_isRetweeted = GameSwitch.GetInstance().GetPostRetweet(m_XlsData.m_iIdx) != 0;
		m_RetweetRoot.SetActive(!m_isRetweeted);
		m_RetweetMineRoot.SetActive(m_isRetweeted);
		m_activedRetweetRoot = ((!m_isRetweeted) ? m_RetweetRoot : m_RetweetMineRoot);
		m_activedRetweetText = ((!m_isRetweeted) ? m_RetweetText : m_RetweetMineText);
		m_isExistImage = !string.IsNullOrEmpty(m_XlsData.m_strIDImg);
		m_NoImageRoot.m_RootObject.SetActive(!m_isExistImage);
		m_ImageRoot.m_RootObject.SetActive(m_isExistImage);
		if (m_isExistImage)
		{
			Sprite sprite = null;
			Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(xlsData.m_strIDImg);
			if (data_byKey != null)
			{
				Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(data_byKey.m_strIDImg);
				if (data_byKey2 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey2.m_strAssetPath_Thumbnail);
				}
			}
			imageThumbnail = sprite;
		}
		Xls.TextData data_byKey3 = Xls.TextData.GetData_byKey(m_XlsData.m_strIDText);
		ParseContentText((data_byKey3 == null) ? string.Empty : data_byKey3.m_strTxt);
		InitProfileInfo();
		retweetCount = GetRetweetCount(m_XlsData);
		promotionText = string.Empty;
		timeText = m_XlsData.m_strPostTime;
		InitButtons(menuMode, isForEvent: false);
		m_ForEventBG.SetActive(value: false);
		RectTransform rectTransform = foRectTransform;
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY);
		if (height > 0f)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

	public static GameObject CreateSNSContentObject(Xls.SNSPostData snsPostData, SNSMenuPlus.Mode menuMode, bool isForEvent = false, SNSMenuPlus snsMenu = null)
	{
		GameObject gameObject = null;
		SNSContentPlus sNSContentPlus = null;
		switch (snsPostData.m_iPostType)
		{
		case 0:
			s_SNSContent_Normal_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Normal;
			gameObject = UnityEngine.Object.Instantiate(s_SNSContent_Normal_SrcObj) as GameObject;
			break;
		case 1:
			s_SNSContent_Reply_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Reply;
			gameObject = UnityEngine.Object.Instantiate(s_SNSContent_Reply_SrcObj) as GameObject;
			break;
		case 2:
			s_SNSContent_Shared_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Shared;
			gameObject = UnityEngine.Object.Instantiate(s_SNSContent_Shared_SrcObj) as GameObject;
			break;
		default:
			return null;
		}
		sNSContentPlus = gameObject.GetComponent<SNSContentPlus>();
		sNSContentPlus.ReflashTextMembers();
		sNSContentPlus.m_snsMenu = snsMenu;
		sNSContentPlus.m_XlsData = snsPostData;
		sNSContentPlus.m_isRetweeted = GameSwitch.GetInstance().GetPostRetweet(snsPostData.m_iIdx) != 0;
		sNSContentPlus.m_RetweetRoot.SetActive(!sNSContentPlus.m_isRetweeted);
		sNSContentPlus.m_RetweetMineRoot.SetActive(sNSContentPlus.m_isRetweeted);
		sNSContentPlus.m_activedRetweetRoot = ((!sNSContentPlus.m_isRetweeted) ? sNSContentPlus.m_RetweetRoot : sNSContentPlus.m_RetweetMineRoot);
		sNSContentPlus.m_activedRetweetText = ((!sNSContentPlus.m_isRetweeted) ? sNSContentPlus.m_RetweetText : sNSContentPlus.m_RetweetMineText);
		sNSContentPlus.m_isExistImage = !string.IsNullOrEmpty(snsPostData.m_strIDImg);
		sNSContentPlus.m_NoImageRoot.m_RootObject.SetActive(!sNSContentPlus.m_isExistImage);
		sNSContentPlus.m_ImageRoot.m_RootObject.SetActive(sNSContentPlus.m_isExistImage);
		if (sNSContentPlus.m_isExistImage)
		{
			Sprite sprite = null;
			Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(snsPostData.m_strIDImg);
			if (data_byKey != null)
			{
				Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(data_byKey.m_strIDImg);
				if (data_byKey2 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey2.m_strAssetPath_Thumbnail);
				}
			}
			sNSContentPlus.imageThumbnail = sprite;
		}
		if (sNSContentPlus.m_ImageViewButton != null)
		{
			sNSContentPlus.m_ImageViewButton.gameObject.SetActive(value: false);
		}
		Xls.TextData data_byKey3 = Xls.TextData.GetData_byKey(snsPostData.m_strIDText);
		sNSContentPlus.ParseContentText((data_byKey3 == null) ? string.Empty : data_byKey3.m_strTxt);
		sNSContentPlus.InitProfileInfo();
		sNSContentPlus.retweetCount = GameSwitch.GetInstance().GetPostRT(snsPostData.m_strID);
		sNSContentPlus.promotionText = string.Empty;
		sNSContentPlus.timeText = snsPostData.m_strPostTime;
		sNSContentPlus.InitButtons(menuMode, isForEvent);
		if (sNSContentPlus.m_ForEventBG != null)
		{
			sNSContentPlus.m_ForEventBG.SetActive(isForEvent);
		}
		return gameObject;
	}

	public static SNSContentPlus CreateSNSContentSlot(SNSContentType contentType)
	{
		GameObject gameObject = null;
		UnityEngine.Object original = null;
		switch (contentType)
		{
		case SNSContentType.Normal:
			s_SNSContent_Normal_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Normal;
			original = s_SNSContent_Normal_SrcObj;
			break;
		case SNSContentType.Reply:
			s_SNSContent_Reply_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Reply;
			original = s_SNSContent_Reply_SrcObj;
			break;
		case SNSContentType.Shared:
			s_SNSContent_Shared_SrcObj = MainLoadThing.instance.m_prefabSNSContent_Shared;
			original = s_SNSContent_Shared_SrcObj;
			break;
		}
		gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		SNSContentPlus component = gameObject.GetComponent<SNSContentPlus>();
		component.ReflashTextMembers();
		return component;
	}

	public static void ParseContentText(string contentText, int contentType, out string outText, out string outOriginAccount)
	{
		outText = contentText;
		outOriginAccount = string.Empty;
		if (contentType != 2 || string.IsNullOrEmpty(contentText))
		{
			return;
		}
		contentText = contentText.Trim();
		if (contentText[0] != '[')
		{
			return;
		}
		int num = contentText.IndexOf(']', 1);
		if (num > 0)
		{
			string text = contentText.Substring(1, num - 1);
			int num2 = text.IndexOf('@');
			Xls.ProgramText data_byKey = Xls.ProgramText.GetData_byKey("SNS_SHARED_POST_ACCOUNT_TEXT");
			if (num2 > 0 && data_byKey != null)
			{
				string arg = text.Substring(num2);
				text = text.Substring(0, num2);
				outOriginAccount = string.Format(data_byKey.m_strTxt, text, arg);
			}
			else
			{
				outOriginAccount = text;
			}
			contentText = contentText.Substring(num + 1).Trim();
			outText = contentText;
		}
	}

	public static string GetProfileText(Xls.SNSPostData postData)
	{
		if (postData == null)
		{
			return string.Empty;
		}
		Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(postData.m_strIDAcc);
		if (data_byKey == null)
		{
			return string.Empty;
		}
		Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_nicknameID);
		if (data_byKey2 == null)
		{
			return string.Empty;
		}
		Xls.ProgramText data_byKey3 = Xls.ProgramText.GetData_byKey((postData.m_iPostType == 2) ? "SNS_SHARED_POST_PROFILE_TEXT" : "SNS_NORMAL_POST_PROFILE_TEXT");
		if (data_byKey3 != null)
		{
			return (postData.m_iPostType == 2) ? string.Format(data_byKey3.m_strTxt, data_byKey2.m_strTitle) : string.Format(data_byKey3.m_strTxt, data_byKey2.m_strTitle, data_byKey2.m_strText);
		}
		return $"{data_byKey2.m_strTitle} @{data_byKey2.m_strText}";
	}

	public static string GetContentText(Xls.SNSPostData postData)
	{
		if (postData == null)
		{
			return string.Empty;
		}
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(postData.m_strIDText);
		if (data_byKey == null)
		{
			return string.Empty;
		}
		ParseContentText(data_byKey.m_strTxt, postData.m_iPostType, out var outText, out var _);
		bool flag = !string.IsNullOrEmpty(postData.m_strIDKeyword) && GameSwitch.GetInstance().GetKeywordAllState(postData.m_strIDKeyword) == 0;
		return TagText.TransTagTextToUnityText(outText, !flag);
	}

	public static int GetRetweetCount(Xls.SNSPostData postData)
	{
		return (postData != null) ? GameSwitch.GetInstance().GetPostRT(postData.m_strID) : 0;
	}

	public void TouchKeywordButton()
	{
		OnClick_KeywordButton();
	}

	public void TouchReplyButton()
	{
		OnClick_ReplyButton();
	}

	public void TouchContent()
	{
		m_snsMenu.TouchContent(this);
	}
}
