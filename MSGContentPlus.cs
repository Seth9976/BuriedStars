using System;
using System.Collections;
using System.Collections.Generic;
using GameEvent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MSGContentPlus : MonoBehaviour, IFloatingUIObject
{
	public enum MSGContentType
	{
		Unknown,
		Me,
		MeFrist,
		Partner,
		PartnerFirst
	}

	public enum SpectialAniState
	{
		input_appear,
		input_idle,
		input_disappear
	}

	[Serializable]
	public class ContentMainObj
	{
		public GameObject m_RootObject;

		public Text m_ContentText;
	}

	[Header("Content Area")]
	public ContentMainObj m_NoImageRoot = new ContentMainObj();

	public ContentMainObj m_ImageRoot = new ContentMainObj();

	public GameObject m_BoxTail;

	public GameObject m_SelectionCursor;

	public GameObject m_ContentImageRoot;

	public Image m_ContentImage;

	public Button m_ImageViewButton;

	public Button m_PsIconButton;

	[Header("Profile Area")]
	public Image m_ProfileImage;

	public Text m_ProfileText;

	private Xls.MessengerTalkData m_XlsData;

	private bool m_isExistImage;

	private MSGMenuPlus m_msgMenu;

	private bool m_isInitailized;

	private bool m_isOnCursor;

	private MSGContentType m_Type;

	private string m_strProfileText = string.Empty;

	private bool m_isMyTalk;

	private RectTransform m_RectTransform;

	private int m_indexInContentInfos = -1;

	private bool m_isOnlyInputDir;

	private float m_inputDirTime;

	private float m_inputDirTime_Remained;

	private Animator m_inputDirAnimator;

	private GameDefine.EventProc m_fpFinishedOnlyInputDir;

	private RectTransform m_rt;

	private Animator m_foAnimator;

	private FloatingUIHandler m_foHandler;

	private List<FloatingUIRoot.EventBase> m_foEvents;

	private Vector3 m_foRotateAngle;

	private FloatingUIRoot.ScalingParams m_foScalingParams;

	private int m_foMotionLoopCount;

	private bool m_foIsMotionComplete;

	private string m_foTag = string.Empty;

	public const string c_AssetName_MSGContent_Partner = "Prefabs/InGame/Menu/MSGContent_Partner";

	private static UnityEngine.Object s_MSGContent_Partner;

	public const string c_AssetName_MSGContent_PartnerFirst = "Prefabs/InGame/Menu/MSGContent_PartnerFirst";

	private static UnityEngine.Object s_MSGContent_PartnerFirst;

	public const string c_AssetName_MSGContent_Me = "Prefabs/InGame/Menu/MSGContent_Me";

	private static UnityEngine.Object s_MSGContent_Me;

	public const string c_MyAccountID = "acc_00000";

	private static bool s_initializedPrefabs;

	public Xls.MessengerTalkData xlsData => m_XlsData;

	public bool isExistImage => m_isExistImage;

	public MSGMenuPlus linkedMsgMenu
	{
		set
		{
			m_msgMenu = value;
		}
	}

	public bool isInitailized => m_isInitailized;

	public bool onCursor
	{
		get
		{
			return m_isOnCursor;
		}
		set
		{
			m_isOnCursor = value;
			if (m_SelectionCursor != null)
			{
				m_SelectionCursor.SetActive(m_isOnCursor);
			}
			if (m_ImageViewButton != null)
			{
				m_ImageViewButton.gameObject.SetActive(m_isOnCursor);
			}
		}
	}

	public GameObject contentRoot => (!m_isExistImage) ? m_NoImageRoot.m_RootObject : m_ImageRoot.m_RootObject;

	public Text contentTextComp => (!m_isExistImage) ? m_NoImageRoot.m_ContentText : m_ImageRoot.m_ContentText;

	public MSGContentType type
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}

	public string text
	{
		get
		{
			return contentTextComp.text;
		}
		set
		{
			contentTextComp.text = TagText.TransTagTextToUnityText(value, isIgnoreHideTag: true);
			contentTextComp.CalculateLayoutInputVertical();
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
			return m_strProfileText;
		}
		set
		{
			m_strProfileText = value;
			if (m_ProfileText != null)
			{
				m_ProfileText.text = value;
			}
		}
	}

	public Sprite profileThumbnail
	{
		get
		{
			return (!(m_ProfileImage != null)) ? null : m_ProfileImage.sprite;
		}
		set
		{
			if (!(m_ProfileImage == null))
			{
				m_ProfileImage.sprite = value;
				m_ProfileImage.gameObject.SetActive(value != null);
			}
		}
	}

	public bool isMyTalk => m_isMyTalk;

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

	public Button foPsIconButton => m_PsIconButton;

	public static bool initializedPrefabs => s_initializedPrefabs;

	private void Awake()
	{
		onCursor = false;
		if (m_NoImageRoot.m_ContentText != null)
		{
			m_NoImageRoot.m_ContentText.verticalOverflow = VerticalWrapMode.Overflow;
		}
		if (m_ImageRoot.m_ContentText != null)
		{
			m_ImageRoot.m_ContentText.verticalOverflow = VerticalWrapMode.Overflow;
		}
	}

	private void Start()
	{
		RectTransform component = contentTextComp.gameObject.GetComponent<RectTransform>();
		float b = 0f;
		if (m_isExistImage)
		{
			RectTransform component2 = m_ContentImageRoot.GetComponent<RectTransform>();
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

	private void OnDestroy()
	{
		m_msgMenu = null;
		m_inputDirAnimator = null;
		m_fpFinishedOnlyInputDir = null;
		m_RectTransform = null;
		m_rt = null;
		m_foAnimator = null;
		m_foHandler = null;
		if (m_foEvents != null)
		{
			m_foEvents.Clear();
		}
		m_foScalingParams = null;
	}

	private void Update()
	{
		if (m_inputDirAnimator == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_inputDirAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(SpectialAniState.input_idle.ToString()))
		{
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? 1f : instance.GetLerpSkipValue());
			m_inputDirTime_Remained -= Time.deltaTime * num;
			if (m_inputDirTime_Remained <= 0f)
			{
				float speedRate = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
				m_inputDirAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, SpectialAniState.input_disappear.ToString(), speedRate);
			}
		}
		else
		{
			if (!currentAnimatorStateInfo.IsName(SpectialAniState.input_disappear.ToString()) || !(currentAnimatorStateInfo.normalizedTime >= 0.99f))
			{
				return;
			}
			if (!m_isOnlyInputDir)
			{
				if (m_PsIconButton != null)
				{
					m_PsIconButton.gameObject.SetActive(value: true);
				}
				EventEngine instance2 = EventEngine.GetInstance();
				float speedRate2 = ((!instance2.GetSkip()) ? 1f : instance2.GetAnimatorSkipValue());
				GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear), speedRate2);
				AudioManager audioManager = GameGlobalUtil.GetAudioManager();
				if (audioManager != null)
				{
					audioManager.PlayUISound((!isMyTalk) ? "MSG_Other" : "MSG_Mine");
				}
			}
			else if (m_fpFinishedOnlyInputDir != null)
			{
				m_fpFinishedOnlyInputDir(this, null);
			}
		}
	}

	private void OnClick_ThumbnailImage(BaseEventData evtData)
	{
		if (!(m_msgMenu == null))
		{
			m_msgMenu.StartCoroutine(m_msgMenu.OnProc_ViewImageDetail(this));
		}
	}

	public void SetContentData(Xls.MessengerTalkData xlsData, float posY = 0f, float height = 0f)
	{
		m_XlsData = xlsData;
		if (m_XlsData == null)
		{
			return;
		}
		m_isMyTalk = m_Type == MSGContentType.Me || m_Type == MSGContentType.MeFrist;
		if (m_BoxTail != null)
		{
			m_BoxTail.SetActive(m_Type == MSGContentType.MeFrist || m_Type == MSGContentType.PartnerFirst);
		}
		if (m_Type == MSGContentType.PartnerFirst)
		{
			Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(xlsData.m_strIDAcc);
			if (data_byKey != null)
			{
				Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_nicknameID);
				if (data_byKey2 != null)
				{
					profileText = data_byKey2.m_strTitle;
				}
				Xls.ImageFile data_byKey3 = Xls.ImageFile.GetData_byKey(data_byKey.m_mespicID);
				if (data_byKey3 != null)
				{
					profileThumbnail = MainLoadThing.instance.faterProfileImageManager.GetThumbnailImageInCache(data_byKey3.m_strAssetPath);
				}
			}
		}
		m_isExistImage = !string.IsNullOrEmpty(xlsData.m_strIDImg);
		m_NoImageRoot.m_RootObject.SetActive(!m_isExistImage);
		m_ImageRoot.m_RootObject.SetActive(m_isExistImage);
		if (m_isExistImage)
		{
			Sprite sprite = null;
			Xls.CollImages data_byKey4 = Xls.CollImages.GetData_byKey(xlsData.m_strIDImg);
			if (data_byKey4 != null)
			{
				Xls.ImageFile data_byKey5 = Xls.ImageFile.GetData_byKey(data_byKey4.m_strIDImg);
				if (data_byKey5 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey5.m_strAssetPath_Thumbnail);
				}
			}
			imageThumbnail = sprite;
		}
		Xls.TextData data_byKey6 = Xls.TextData.GetData_byKey(xlsData.m_strIDText);
		text = ((data_byKey6 == null) ? string.Empty : data_byKey6.m_strTxt);
		RectTransform rectTransform = foRectTransform;
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY);
		if (height > 0f)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

	public void SetInputDirectionParams(float inputTime, bool isOnlyInputDir, GameDefine.EventProc fpFinishedDir = null)
	{
		if (!(inputTime <= 0.0001f))
		{
			EventEngine instance = EventEngine.GetInstance();
			float speedRate = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
			m_isOnlyInputDir = isOnlyInputDir;
			m_inputDirTime = inputTime;
			m_inputDirTime_Remained = m_inputDirTime;
			m_inputDirAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, SpectialAniState.input_appear.ToString(), speedRate);
			m_foAnimator = m_inputDirAnimator;
			m_fpFinishedOnlyInputDir = fpFinishedDir;
			if (m_PsIconButton != null)
			{
				m_PsIconButton.gameObject.SetActive(value: false);
			}
		}
	}

	public static GameObject CreateMSGContentObject(Xls.MessengerTalkData msgTalkData, bool isFirstTalk, bool isForEvent = false, MSGMenuPlus msgMenu = null)
	{
		GameObject gameObject = null;
		MSGContentPlus mSGContentPlus = null;
		bool flag = msgTalkData.m_strIDAcc.Equals("acc_00000");
		if (flag)
		{
			if (s_MSGContent_Me == null)
			{
				s_MSGContent_Me = MainLoadThing.instance.m_prefabMSGContent_Me;
			}
			gameObject = UnityEngine.Object.Instantiate(s_MSGContent_Me) as GameObject;
			mSGContentPlus = gameObject.GetComponent<MSGContentPlus>();
			if (mSGContentPlus.m_BoxTail != null)
			{
				mSGContentPlus.m_BoxTail.SetActive(isFirstTalk);
			}
			mSGContentPlus.m_Type = ((!isFirstTalk) ? MSGContentType.Me : MSGContentType.MeFrist);
		}
		else if (isFirstTalk)
		{
			s_MSGContent_PartnerFirst = MainLoadThing.instance.m_prefabMSGContent_PartnerFirst;
			gameObject = UnityEngine.Object.Instantiate(s_MSGContent_PartnerFirst) as GameObject;
			mSGContentPlus = gameObject.GetComponent<MSGContentPlus>();
			if (mSGContentPlus.m_BoxTail != null)
			{
				mSGContentPlus.m_BoxTail.SetActive(value: true);
			}
			mSGContentPlus.m_Type = MSGContentType.PartnerFirst;
		}
		else
		{
			s_MSGContent_Partner = MainLoadThing.instance.m_prefabMSGContent_Partner;
			gameObject = UnityEngine.Object.Instantiate(s_MSGContent_Partner) as GameObject;
			mSGContentPlus = gameObject.GetComponent<MSGContentPlus>();
			if (mSGContentPlus.m_BoxTail != null)
			{
				mSGContentPlus.m_BoxTail.SetActive(value: false);
			}
			mSGContentPlus.m_Type = MSGContentType.Partner;
		}
		mSGContentPlus.m_XlsData = msgTalkData;
		mSGContentPlus.m_msgMenu = msgMenu;
		mSGContentPlus.m_isMyTalk = flag;
		Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(msgTalkData.m_strIDAcc);
		if (data_byKey != null)
		{
			Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_nicknameID);
			if (data_byKey2 != null)
			{
				mSGContentPlus.profileText = data_byKey2.m_strTitle;
			}
			if (!flag && isFirstTalk)
			{
				Xls.ImageFile data_byKey3 = Xls.ImageFile.GetData_byKey(data_byKey.m_mespicID);
				if (data_byKey3 != null)
				{
					mSGContentPlus.profileThumbnail = MainLoadThing.instance.faterProfileImageManager.GetThumbnailImageInCache(data_byKey3.m_strAssetPath);
				}
			}
		}
		mSGContentPlus.m_isExistImage = !string.IsNullOrEmpty(msgTalkData.m_strIDImg);
		mSGContentPlus.m_NoImageRoot.m_RootObject.SetActive(!mSGContentPlus.m_isExistImage);
		mSGContentPlus.m_ImageRoot.m_RootObject.SetActive(mSGContentPlus.m_isExistImage);
		if (mSGContentPlus.m_isExistImage)
		{
			Sprite sprite = null;
			Xls.CollImages data_byKey4 = Xls.CollImages.GetData_byKey(msgTalkData.m_strIDImg);
			if (data_byKey4 != null)
			{
				Xls.ImageFile data_byKey5 = Xls.ImageFile.GetData_byKey(data_byKey4.m_strIDImg);
				if (data_byKey5 != null)
				{
					sprite = MainLoadThing.instance.colImageThumbnailManager.GetThumbnailImageInCache(data_byKey5.m_strAssetPath_Thumbnail);
				}
			}
			mSGContentPlus.imageThumbnail = sprite;
		}
		if (mSGContentPlus.m_ImageViewButton != null)
		{
			mSGContentPlus.m_ImageViewButton.gameObject.SetActive(value: false);
		}
		Text[] textComps = new Text[3]
		{
			mSGContentPlus.m_ImageRoot.m_ContentText,
			mSGContentPlus.m_NoImageRoot.m_ContentText,
			mSGContentPlus.m_ProfileText
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		Xls.TextData data_byKey6 = Xls.TextData.GetData_byKey(msgTalkData.m_strIDText);
		mSGContentPlus.text = ((data_byKey6 == null) ? string.Empty : data_byKey6.m_strTxt);
		return gameObject;
	}

	public static MSGContentPlus CreateMSGContentSlot(MSGContentType contentType)
	{
		GameObject gameObject = null;
		UnityEngine.Object obj = null;
		switch (contentType)
		{
		case MSGContentType.Me:
		case MSGContentType.MeFrist:
			s_MSGContent_Me = MainLoadThing.instance.m_prefabMSGContent_Me;
			obj = s_MSGContent_Me;
			break;
		case MSGContentType.Partner:
			s_MSGContent_Partner = MainLoadThing.instance.m_prefabMSGContent_Partner;
			obj = s_MSGContent_Partner;
			break;
		case MSGContentType.PartnerFirst:
			s_MSGContent_PartnerFirst = MainLoadThing.instance.m_prefabMSGContent_PartnerFirst;
			obj = s_MSGContent_PartnerFirst;
			break;
		}
		if (obj == null)
		{
			return null;
		}
		gameObject = UnityEngine.Object.Instantiate(obj) as GameObject;
		MSGContentPlus component = gameObject.GetComponent<MSGContentPlus>();
		Text[] textComps = new Text[3]
		{
			component.m_ImageRoot.m_ContentText,
			component.m_NoImageRoot.m_ContentText,
			component.m_ProfileText
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		return component;
	}

	public static IEnumerator LoadMsgContentPrefabs()
	{
		s_initializedPrefabs = false;
		if (s_MSGContent_Me == null)
		{
			s_MSGContent_Me = MainLoadThing.instance.m_prefabMSGContent_Me;
		}
		if (s_MSGContent_Partner == null)
		{
			s_MSGContent_Partner = MainLoadThing.instance.m_prefabMSGContent_Partner;
		}
		if (s_MSGContent_PartnerFirst == null)
		{
			s_MSGContent_PartnerFirst = MainLoadThing.instance.m_prefabMSGContent_PartnerFirst;
		}
		s_initializedPrefabs = true;
		yield break;
	}

	public static IEnumerator LoadMsgContentPrefabs(MonoBehaviour parentBehaviour)
	{
		s_initializedPrefabs = false;
		if (s_MSGContent_Me == null)
		{
			s_MSGContent_Me = MainLoadThing.instance.m_prefabMSGContent_Me;
		}
		if (s_MSGContent_Partner == null)
		{
			s_MSGContent_Partner = MainLoadThing.instance.m_prefabMSGContent_Partner;
		}
		if (s_MSGContent_PartnerFirst == null)
		{
			s_MSGContent_PartnerFirst = MainLoadThing.instance.m_prefabMSGContent_PartnerFirst;
		}
		s_initializedPrefabs = true;
		yield break;
	}

	public static void UnloadMsgContentBundle()
	{
		s_initializedPrefabs = false;
		s_MSGContent_Me = null;
		s_MSGContent_Partner = null;
		s_MSGContent_PartnerFirst = null;
	}

	public void OnClick_Content()
	{
		if (!(m_msgMenu == null))
		{
			m_msgMenu.OnClick_Content(this);
		}
	}
}
