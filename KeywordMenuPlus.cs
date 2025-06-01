using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeywordMenuPlus : MonoBehaviour
{
	public enum KeywordState
	{
		MEMO,
		TALK,
		ARRANGE_MULTI,
		ARRANGE_ONE,
		EVENT_KEYWORD,
		COUNT
	}

	private enum SlotPage
	{
		Unknown = -1,
		Prev,
		Current,
		Next
	}

	private class SelectAnswer
	{
		public string[] m_strArrUsingSelList = new string[MAX_USING_SEL_CNT];

		public string m_strQuestID;
	}

	private class clSlotImgRes
	{
		public string m_strSelImgName;

		public Sprite m_sprSel;

		public Sprite m_sprNSel;
	}

	private class clDetailImgRes
	{
		public string m_strImgName;

		public Sprite m_sprDetail;
	}

	private class PageMarks
	{
		public bool m_isSelMark;

		public bool m_isMakeObj;

		public GameObject m_goMark;
	}

	private class SelUsingAns
	{
		private enum eState
		{
			Sel,
			NSel,
			Empty,
			Count
		}

		private KeywordMenuPlus m_KeywordMenu;

		public GameObject m_goSelObj;

		public bool m_isEmpty;

		public bool m_isSelecting;

		private Sprite m_sprDefIcon;

		public string m_strKeywordKey;

		private float[] m_iSelNSelH = new float[3];

		private GameObject[] m_goSelNSel = new GameObject[3];

		private Text[] m_textSelNSel = new Text[3];

		private Image[] m_imgSelNSel = new Image[3];

		private Animator[] m_animSelNSel = new Animator[3];

		private GameObject[] m_goTag = new GameObject[3];

		private Text[] m_textTag = new Text[3];

		private GameObject m_goFocus;

		public SelUsingAns(GameObject goUnit, GameObject goParent, bool isEmpty, KeywordMenuPlus keywordMenu)
		{
			m_KeywordMenu = keywordMenu;
			m_goSelObj = goUnit;
			m_isEmpty = isEmpty;
			int num = -1;
			if (!isEmpty)
			{
				KeywordAnswerSlot component = m_goSelObj.GetComponent<KeywordAnswerSlot>();
				if (component == null)
				{
					return;
				}
				for (int i = 0; i < 2; i++)
				{
					component.m_rtfSlot[i].position = Vector3.zero;
					m_iSelNSelH[i] = component.m_rtfSlot[i].rect.height;
					m_goSelNSel[i] = component.m_goSlot[i];
					m_textSelNSel[i] = component.m_textKeywordName[i];
					FontManager.ResetTextFontByCurrentLanguage(m_textSelNSel[i]);
					m_imgSelNSel[i] = component.m_imgKeywordIcon[i];
					m_animSelNSel[i] = component.m_animSlot[i];
					m_goTag[i] = component.m_goTag[i];
					m_textTag[i] = component.m_textTagOrder[i];
					FontManager.ResetTextFontByCurrentLanguage(m_textTag[i]);
				}
				m_sprDefIcon = component.m_sprDefKeywordIcon;
				m_goFocus = component.m_goFocus;
				m_goFocus.SetActive(value: false);
				m_goSelNSel[0].SetActive(value: false);
				m_goSelNSel[1].SetActive(value: true);
			}
			else
			{
				num = 2;
				m_iSelNSelH[num] = m_goSelObj.transform.GetComponent<RectTransform>().rect.height;
				m_animSelNSel[num] = m_goSelObj.GetComponent<Animator>();
			}
			m_goSelObj.SetActive(value: true);
			m_goSelObj.transform.SetParent(goParent.transform, worldPositionStays: false);
		}

		public void DestroySprite()
		{
			m_sprDefIcon = null;
			int num = 3;
			for (int i = 0; i < num; i++)
			{
				if (m_imgSelNSel[i] != null)
				{
					m_imgSelNSel[i].sprite = null;
				}
			}
		}

		public void PlayDisappear()
		{
			for (int i = 0; i < 3; i++)
			{
				if (m_animSelNSel[i] != null && m_animSelNSel[i].gameObject.activeInHierarchy)
				{
					GameGlobalUtil.PlayUIAnimation(m_animSelNSel[i], GameDefine.UIAnimationState.disappear);
				}
			}
		}

		public void CopyAns(SelUsingAns oriAns = null)
		{
			if (oriAns == null)
			{
				m_strKeywordKey = null;
				return;
			}
			m_isEmpty = oriAns.m_isEmpty;
			m_strKeywordKey = oriAns.m_strKeywordKey;
		}

		public void SetTagOrder(bool isShow, int iIdx = 0)
		{
			for (int i = 0; i < 2; i++)
			{
				if (m_textTag[i] != null && isShow)
				{
					m_textTag[i].text = iIdx.ToString();
				}
				if (m_goTag[i] != null)
				{
					m_goTag[i].SetActive(isShow);
					m_imgSelNSel[i].gameObject.SetActive(!isShow);
				}
			}
		}

		public void SetAnsFocus(bool isFocusOn, KeywordSlotPlus ksTemp)
		{
			if (!(m_goSelNSel[1] == null) && m_goSelNSel[1].activeInHierarchy)
			{
				m_textSelNSel[1].color = ((!isFocusOn) ? GameGlobalUtil.HexToColor(11183747) : GameGlobalUtil.HexToColor(16777215));
				m_goFocus.SetActive(isFocusOn);
			}
		}

		public float SetAnsDetail(float fY, bool isSel, KeywordSlotPlus ksTemp = null, bool isSetKeyword = false, string strKeywordKey = null)
		{
			if (isSetKeyword)
			{
				m_strKeywordKey = strKeywordKey;
			}
			int num = ((!isSel) ? 1 : 0);
			int num2 = (isSel ? 1 : 0);
			Vector3 position = m_goSelObj.GetComponent<RectTransform>().position;
			m_goSelObj.GetComponent<RectTransform>().position = new Vector3(position.x, fY, position.z);
			RectTransform component = m_goSelObj.GetComponent<RectTransform>();
			if (m_isEmpty)
			{
				component.localPosition = new Vector3(component.localPosition.x, fY, component.localPosition.z);
			}
			else
			{
				component.offsetMax = new Vector2(component.offsetMax.x, fY);
			}
			float num3 = 0f;
			if (!m_isEmpty)
			{
				m_goSelNSel[num2].SetActive(value: false);
				m_goSelNSel[num].SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation(m_animSelNSel[num], GameDefine.UIAnimationState.idle);
				string text = null;
				m_isSelecting = isSel;
				if (isSel)
				{
					text = m_KeywordMenu.m_strArrangeSelecting;
					num3 = m_iSelNSelH[0];
				}
				else
				{
					if (m_strKeywordKey == null)
					{
						text = string.Empty;
					}
					else
					{
						Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(m_strKeywordKey);
						if (data_byKey != null)
						{
							text = m_KeywordMenu.GetTextList(data_byKey.m_strTitleID, isTitle: true);
						}
					}
					num3 = m_iSelNSelH[1];
				}
				if (text != null && m_textSelNSel[num] != null)
				{
					m_textSelNSel[num].text = text;
				}
				if (isSel || (!isSel && m_strKeywordKey == null))
				{
					m_imgSelNSel[num].sprite = m_sprDefIcon;
				}
				else if (ksTemp != null)
				{
					m_imgSelNSel[num].sprite = ksTemp.m_imgSelSlot.sprite;
				}
			}
			else
			{
				num3 = m_iSelNSelH[2];
			}
			return fY - num3;
		}
	}

	private enum eBottomGuide
	{
		Cursor,
		Suggest,
		SuggCancel,
		Exit,
		ShowImage,
		TabMove,
		TabSelect,
		TabCancel,
		Confirm,
		KeywordRecord,
		InitAllArrangeKeyword,
		Count
	}

	public enum ReturnCheckAns
	{
		WRONG_QUEST_ID = 0,
		ANS_0 = 1,
		ANS_ALL_WRONG = 11
	}

	public enum eKeywordTouch
	{
		Down,
		Up,
		Click,
		Drag,
		BeginDrag,
		EndDrag,
		RunEvent
	}

	public static bool m_isKewordMenuPlusOn;

	public static KeywordState m_eKeywordState;

	private GameSwitch m_GameSwitch;

	private EventEngine m_EventEngine;

	private GameMain m_GameMain;

	[Header("Keyword Menu")]
	public GameObject m_KeywordMenu;

	[Header("Canvas")]
	public Canvas m_canvas;

	[Header("Back Overay Color")]
	public GameObject[] m_goBackOverlay = new GameObject[5];

	[Header("BG Multiply")]
	public GameObject m_goMemoMultiply;

	public GameObject m_goNotMemoMultiply;

	public GameObject m_goMultiType3;

	public Animator m_animMemoMultiply;

	public Animator m_animNotMemoMultiply;

	public Animator m_animMultiType3;

	[Header("Title Name")]
	public GameObject[] m_goTitle = new GameObject[5];

	public Text[] m_textTitle = new Text[5];

	public Text m_textSubTitle;

	[Header("Talk Balloon")]
	public GameObject m_goTalkBalloon;

	public Text m_textTalkBalloon;

	[Header("Left Condition")]
	public GameObject m_goCondition;

	public Text m_textCondition;

	public Image m_imgCondition;

	[Header("Keyword Item Group")]
	public GameObject m_goKeywordItemGroup;

	public Transform m_tfKeywordItemGroup;

	public GameObject m_goKeywordGroupViewBox;

	public RectTransform m_rtfKeywordViewBox;

	public RectTransform m_rtfKeywordGroupContent;

	public GridLayoutGroup m_gridKeywordSlots;

	[Header("Keyword Item Arrows Group")]
	public GameObject m_goKeywordArrows;

	[Header("Page Show")]
	public GameObject m_goLeftArrow;

	public GameObject m_goRightArrow;

	public GameObject m_goPageGroup;

	public GameObject m_goPageSel;

	public GameObject m_goPageNotSel;

	public GameObject m_goRightStickUI;

	[Header("Keyword Detail Group")]
	public GameObject m_goKeyDetailGroup;

	public Image m_imgKeyDetailBack;

	private Sprite m_sprDetailBack;

	public Button m_butLogTouchPad;

	public GameObject m_goLogIcon;

	public Text m_textLogIcon;

	public Image m_imgDetailFace;

	public Image m_imgDetailMental;

	public GameObject m_goCutKeywordMark;

	[Header("Keyword Detail")]
	public Text m_textKeyTitle;

	public GameObject m_goKeyCategory;

	public Text m_textKeyCategory;

	public GameObject m_goKeyOnlyText;

	public Text m_textKeyOnlyText;

	[Header("Keyword Detail BG Image")]
	public Sprite m_sprDetailBG_Memo;

	public Sprite m_sprDetailBG_Commu;

	public Sprite m_sprDetailBG_Event;

	[Header("Keyword Detail With Image")]
	public GameObject m_goKeyImgGroup;

	public Text m_textKeyImgText;

	public Image m_imgKeyImage;

	[Header("Keyword Detail Image Spread")]
	public ShowImageOriginSize m_ShowImageOriginSize;

	[Header("Slot Item")]
	public GameObject m_prefabKeywordSlot;

	private const int cPreMakeSlotCnt = 24;

	public GameObject[] m_goPreMakeSlot = new GameObject[24];

	[Header("Slot Drag")]
	public GameObject m_goSlotDragParent;

	public RectTransform m_rtfSlotDragParent;

	[Header("Up Tab")]
	public GameObject m_goUpTab;

	public CommonTabContainerPlus m_TabContainer;

	[Header("Arrange_AnswerList")]
	public GameObject m_goAnswerGroup;

	public Text m_txtAnswerSelCnt;

	public GameObject m_goAnsFocus;

	public GameObject m_goAnsSlotEmpty;

	public GameObject m_goAnsSlotNEmpty;

	public GameObject m_goAnsSlotSel;

	public Image m_goAnsSlotSelIcon;

	public Text m_textAnsSlotSelName;

	public GameObject m_goAnsSlotNSel;

	public Image m_goAnsSlotNSelIcon;

	public Text m_textAnsSlotNSelName;

	public GameObject m_goAnsParent;

	[Header("Arrange_AnswerConfirm")]
	public GameObject m_goAnswerConfirm;

	public Text m_textAnswerConfirm;

	public Button m_butAnswerConfirm;

	[Header("Arrange_Question")]
	public GameObject m_goQuestionGroup;

	public Text m_textQuestion;

	public GameObject m_goQuestionIcon;

	[Header("Arrange_Cancel")]
	public GameObject m_goArrangeCancel;

	public Text m_textArrangeCancel;

	public Button m_butArrangeCancel;

	[Header("Help Window")]
	public GameObject m_goHelpWindow;

	public GameObject m_goHWArrow;

	public GameObject m_goHWCloseButton;

	public GameObject m_goHWDirectionButton;

	public GameObject m_goHWMemoCloseButton;

	public Text m_textHWCloseButton;

	public Text m_textHWDirectionButton;

	public Text m_textHWMemoCloseButton;

	public Text m_textHWLog;

	public Button m_butHWCloseOButton;

	public Button m_butHWCloseXButton;

	public Button m_butHWDirectionButton;

	public Button m_butHWMemoCloseOButton;

	public Button m_butHWMemoCloseXButton;

	public Text m_textHWContentTitle;

	public GameObject m_goHWNoImgObj;

	public GameObject m_goHWImgObj;

	public Text m_textHWNoImgContentTxt;

	public Text m_textHWImgContentTxt;

	public Image m_imgHWDetailImg;

	public GameObject[] m_goHWResultEachObj = new GameObject[5];

	public Button m_butHWLeftArrow;

	public Button m_butHWRightArrow;

	public Text m_textUseRecordNotExist;

	public Image m_imgHWKeyCardImg;

	public Text m_textTitleRelation;

	public Text m_textTitleGetKeyword;

	[Header("DANGER MARK")]
	public GameObject m_goVignetting;

	public Animator m_animVignetting;

	private int m_iCrisisLevel;

	private SWSub_MemoMenu m_subMemoMenu;

	[Header("KeywordEventMenu")]
	public Text m_textEventKeywordTitle;

	public Text m_textCurGameTime;

	public Text m_textEventKeywordCutName;

	public Text m_textEventKeywordGetCnt;

	public Text m_textEventKeywordSlash;

	public Text m_textEventKeywordMaxCnt;

	[Header("Animator ")]
	public Animator m_animTalkBalloon;

	public Animator m_animSlotBGPanel;

	public Animator m_animScrollButton;

	public Animator m_animKeywordDetailBox;

	public Animator m_animHelpWindow;

	public Animator m_animDotReaction;

	private Animator m_animUIBGOverColor;

	public Animator[] m_animUIBGOveray = new Animator[5];

	public Animator m_animMenuTitle;

	public Animator[] m_animMenuEachTitle = new Animator[5];

	public Animator m_animQuestion;

	public Animator m_animArrangeAnswerConfirm;

	public Animator m_animAnswerList;

	public Animator m_animConditionIcon;

	[Header("HelpWindow Animator")]
	public Animator m_animHWEtc;

	public Animator m_animHWKeywordContext;

	public Animator m_animHWHistoryWindow;

	public Animator[] m_animHWReaction = new Animator[5];

	public Animator m_animHWArrow;

	[Header("LoadingIcon")]
	public GameObject m_goLoadingIconPosition;

	private LoadingSWatchIcon m_LoadingIcon;

	[Header("ChangingTabCover")]
	public GameObject m_goChangingTabCover;

	[Header("ArrangeOpeningEffect")]
	public GameObject m_goArrangeOpeningEffect;

	public Animator m_aniArrangeOpeningEffect;

	private GameDefine.eAnimChangeState m_eArrangeOpeningEffectMotState;

	private bool m_isArrangeOpeningEffect;

	[Header("Touch BackClose Button")]
	public GameObject m_goBackButton;

	public GameObject m_goCloseButton;

	[Header("Touch HelpWindow Close Button")]
	public GameObject m_goHelpWindowCloseButton;

	[Header("Touch Input Block")]
	public GameObject m_goTouchInputBlock;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private GameDefine.eAnimChangeState m_eStartMotState;

	private GameDefine.eAnimChangeState m_eHWEndMotState;

	[HideInInspector]
	private static string m_strCharKey;

	private static bool m_isShowBackButton;

	private static int MAX_USING_SEL_CNT = 10;

	private List<SelectAnswer> m_listSelectAnswer = new List<SelectAnswer>();

	private string m_strKeywordUsedID;

	private int m_iKeywordUseSelCnt;

	private int m_iCurUsingIdx;

	private bool m_isReUsingSelList;

	[HideInInspector]
	public string m_strArrangeSelecting = string.Empty;

	private bool m_isTutoKeyLock;

	private bool m_isTabSelKeyLock;

	private bool m_isKeyLock;

	private int m_iShowKeywordCnt;

	private float m_fFirstContentX;

	private float m_fContentW = -1f;

	private float m_fTotalMoveT = 0.3f;

	private float m_fPassMoveT;

	private float m_fDestContentX;

	private bool m_isDrag;

	private static int NONE_CATE_IDX = -99;

	private int m_iCurCategory = NONE_CATE_IDX;

	[HideInInspector]
	public bool m_isFromKeywordEvt;

	private int m_iSlotCurPage;

	private int m_iSlotTotalPage;

	private int m_iCurSequence;

	private Dictionary<int, CategoryInfo<Xls.CollKeyword>> m_CategoryInfos = new Dictionary<int, CategoryInfo<Xls.CollKeyword>>();

	private string STR_CATEGORY_HIDDEN;

	private List<int> m_iListTabOrder;

	private string m_strFirstSetTab;

	private static int LINE_CNT = 2;

	private int[] m_iLineMinIdx = new int[LINE_CNT];

	private int[] m_iLineMaxIdx = new int[LINE_CNT];

	private int m_iSlotCntPerLine;

	[HideInInspector]
	public string m_strRunKeywordKey;

	[HideInInspector]
	public string m_strSelKeywordKey;

	private KeywordState m_eBefRunKeywordState;

	private static bool m_isFromRunKeyword;

	private static int m_iRunKeywordPage;

	private bool m_isShowTutorial;

	private const string m_strTalkTutorialID = "";

	private const string m_strMemoTutorialID = "";

	private string m_strTutorialID;

	private List<clSlotImgRes> m_listSlotImgRes;

	private List<clDetailImgRes> m_listDetailRes;

	[Header("CommonCloseButton Button")]
	public CommonCloseButton m_closeButton;

	private const int USE_KEYWORD_CHR_CNT = 5;

	private Sprite[] m_sprDotFace = new Sprite[5];

	private const int MENTAL_STATE_CNT = 4;

	private Sprite[] m_sprMental = new Sprite[4];

	private KeywordContainer m_keywordContainer = new KeywordContainer();

	private List<Xls.CollKeyword> m_validXlsDatas = new List<Xls.CollKeyword>();

	private string m_strMemoSelKeywordKey;

	private KeywordContainer.EventArg_OnCursorChanged m_MemoSelCursor;

	private List<KeywordUseResultSlot> m_listKeyUseResultSlot;

	private bool m_isShowHelpWindow;

	private const int c_pageSlotRawCount = 2;

	private const int c_pageSlotColCount = 4;

	private const int c_pageSlotTotalCount = 8;

	private const int c_pageEdgePlusCnt = 4;

	private const int c_maxSlotObjectCount = 24;

	public float m_InputRepeatTimeBound = 0.2f;

	private float m_PadStickPushingTime;

	private bool m_isSetFirstAnchoredPosition;

	private Vector2 m_vecGridAnchoredPosition;

	private bool m_isQuitApplication;

	private bool m_isSetMenuCreating;

	private const string c_characterIconBundleName = "image/keyword_ui";

	private ContentThumbnailManager m_characterIconManager = new ContentThumbnailManager("image/keyword_ui");

	private int m_iMaxSlotCnt;

	private int m_iCurSelSlotIdx;

	private bool m_isCurSlotShowImg;

	private List<KeywordSlotPlus> m_listKeySlot;

	private List<Xls.CollKeyword> m_listXlsMemo;

	private List<PageMarks> m_listPageMarks;

	[HideInInspector]
	public static float m_fSlotTextMaskWidth = -1f;

	private List<SelUsingAns> m_listUsingAns;

	private bool[] m_isButton = new bool[11];

	private bool[] m_isTempSaveButton = new bool[11];

	private static int USING_KEYWORD_ARG_CNT = 10;

	private bool m_isBackUp;

	private clSlotImgRes m_slotImgRes;

	private clDetailImgRes m_clDetailImgRes;

	private int m_iPlusCnt;

	private KeywordSlotPlus m_slotMadeSlot;

	private bool m_isEndTypeRunEvent;

	private KeywordSlotPlus m_SelKeywordSlot;

	private AssetBundleObjectHandler m_showingImageAssetObjHdr;

	private void OnEnable()
	{
		m_isKewordMenuPlusOn = true;
		Text[] textComps = new Text[27]
		{
			m_textSubTitle, m_textTalkBalloon, m_textCondition, m_textLogIcon, m_textHWLog, m_textKeyTitle, m_textKeyOnlyText, m_textKeyImgText, m_txtAnswerSelCnt, m_textAnswerConfirm,
			m_textQuestion, m_textArrangeCancel, m_textHWCloseButton, m_textHWDirectionButton, m_textHWContentTitle, m_textHWNoImgContentTxt, m_textHWImgContentTxt, m_textUseRecordNotExist, m_textTitleRelation, m_textTitleGetKeyword,
			m_textEventKeywordTitle, m_textCurGameTime, m_textEventKeywordCutName, m_textEventKeywordGetCnt, m_textEventKeywordSlash, m_textEventKeywordMaxCnt, m_textHWMemoCloseButton
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		FontManager.ResetTextFontByCurrentLanguage(m_textTitle);
		m_GameMain.m_CommonButtonGuide.ClearContents();
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Cursor), PadInput.GameInput.LStickY, isIngoreAxis: true);
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Exit), PadInput.GameInput.CrossButton);
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.ShowImage), PadInput.GameInput.SquareButton);
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.TabMove), PadInput.GameInput.L1Button);
		if (m_eKeywordState == KeywordState.MEMO)
		{
			m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.TabSelect), PadInput.GameInput.CircleButton);
			m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.TabCancel), PadInput.GameInput.CrossButton);
		}
		else if (m_eKeywordState != KeywordState.EVENT_KEYWORD)
		{
			m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide((m_eKeywordState != KeywordState.ARRANGE_MULTI) ? eBottomGuide.Suggest : eBottomGuide.SuggCancel), PadInput.GameInput.CircleButton);
		}
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.TabMove), PadInput.GameInput.R1Button);
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Confirm), PadInput.GameInput.TriangleButton);
		if (m_eKeywordState != KeywordState.EVENT_KEYWORD)
		{
			m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.KeywordRecord), PadInput.GameInput.TouchPadButton);
		}
		m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.InitAllArrangeKeyword), PadInput.GameInput.CrossButton);
		m_GameMain.m_CommonButtonGuide.BuildContents((m_eKeywordState != KeywordState.MEMO) ? CommonButtonGuide.AlignType.Center : CommonButtonGuide.AlignType.Left);
		m_GameMain.m_CommonButtonGuide.SetShow(isShow: true);
		BitCalc.InitArray(m_isButton);
		Invoke("FirstSetBottomGuide", 0.5f);
		InitGameObjectState();
		m_strTutorialID = null;
		switch (m_eKeywordState)
		{
		case KeywordState.MEMO:
			m_goUpTab.SetActive(value: true);
			if (m_TabContainer != null)
			{
				m_TabContainer.getTabCreateInfoFP = SetTabCreateInfos;
				m_TabContainer.onChangedSelectTab = OnChangedSelectTab;
				m_TabContainer.onChangingSelectTab = OnChangingSelectTab;
				m_TabContainer.onPressTabButton = OnProc_PressedTabButtons;
				m_TabContainer.m_AutoBuild = false;
			}
			m_strTutorialID = string.Empty;
			m_keywordContainer.InitContainer(m_gridKeywordSlots, m_prefabKeywordSlot, 4, 2, KeywordContainer.MenuType.Memo);
			m_keywordContainer.OnCursorChanged = OnProc_OnCursorChanged;
			m_keywordContainer.ChangedCurrentPage = OnProc_ChangedCurrentPage;
			m_keywordContainer.KeyInputedScrollPage = OnProc_KeyInputedScrollPage;
			m_keywordContainer.KeyInputedMoveCursor = OnProc_KeyInputedMoveCursor;
			break;
		case KeywordState.TALK:
			m_strTutorialID = string.Empty;
			break;
		case KeywordState.ARRANGE_MULTI:
		case KeywordState.ARRANGE_ONE:
			m_strArrangeSelecting = GameGlobalUtil.GetXlsProgramText("KEYWORD_ANS_SELETING");
			break;
		}
		GameGlobalUtil.PlayUIAnimation(m_animSlotBGPanel, GameDefine.UIAnimationState.appear);
		m_eStartMotState = GameDefine.eAnimChangeState.none;
		GameGlobalUtil.PlayUIAnimation(m_animUIBGOverColor, GameDefine.UIAnimationState.appear, ref m_eStartMotState);
		Init();
		m_isShowTutorial = m_strTutorialID != null && m_GameSwitch.GetTutorial(m_strTutorialID) == 0;
		if (m_isShowTutorial)
		{
			OnBottomCloseHelp(isShow: true);
		}
		AudioManager.instance.PlayUISound("View_KeyTalk_Title");
	}

	private void FirstSetBottomGuide()
	{
		OnBottomGuide((m_eKeywordState != KeywordState.ARRANGE_MULTI) ? eBottomGuide.Suggest : eBottomGuide.SuggCancel, m_eKeywordState != KeywordState.MEMO);
		OnBottomGuide(eBottomGuide.TabMove, m_eKeywordState == KeywordState.MEMO);
		OnBottomGuide(eBottomGuide.Exit, m_eKeywordState != KeywordState.ARRANGE_MULTI && m_eKeywordState != KeywordState.ARRANGE_ONE);
		OnBottomGuide(eBottomGuide.TabSelect, isShow: false);
		OnBottomGuide(eBottomGuide.TabCancel, isShow: false);
		OnBottomGuide(eBottomGuide.InitAllArrangeKeyword, m_eKeywordState == KeywordState.ARRANGE_MULTI);
	}

	private void OnBottomGuide(eBottomGuide eGuide, bool isShow)
	{
		if (!(m_GameMain == null) && !(m_GameMain.m_CommonButtonGuide == null))
		{
			string strBottomGuide = GetStrBottomGuide(eGuide);
			m_GameMain.m_CommonButtonGuide.SetContentEnable(strBottomGuide, isShow);
			m_isButton[(int)eGuide] = isShow;
		}
	}

	private void OnBottomCloseHelp(bool isShow)
	{
		if (isShow)
		{
			for (eBottomGuide eBottomGuide = eBottomGuide.Cursor; eBottomGuide < eBottomGuide.Count; eBottomGuide++)
			{
				int num = (int)eBottomGuide;
				m_isTempSaveButton[num] = m_isButton[num];
				OnBottomGuide(eBottomGuide, isShow: false);
			}
			return;
		}
		for (eBottomGuide eBottomGuide2 = eBottomGuide.Cursor; eBottomGuide2 < eBottomGuide.Count; eBottomGuide2++)
		{
			if (m_isTempSaveButton[(int)eBottomGuide2])
			{
				OnBottomGuide(eBottomGuide2, isShow: true);
			}
		}
	}

	private void ActionBottomGuide(eBottomGuide eGuide)
	{
		string strBottomGuide = GetStrBottomGuide(eGuide);
		m_GameMain.m_CommonButtonGuide.SetContentActivate(strBottomGuide, isActivate: true);
	}

	private string GetStrBottomGuide(eBottomGuide eGuide)
	{
		string xlsDataName = null;
		switch (eGuide)
		{
		case eBottomGuide.Cursor:
			xlsDataName = "KEYWORD_BOT_MENU_CURSOR";
			break;
		case eBottomGuide.Suggest:
			xlsDataName = "KEYWORD_BOT_MENU_SUGGEST";
			break;
		case eBottomGuide.SuggCancel:
			xlsDataName = "KEYWORD_BOT_MENU_SUGGCANCEL";
			break;
		case eBottomGuide.Exit:
			xlsDataName = "KEYWORD_BOT_MENU_EXIT";
			break;
		case eBottomGuide.ShowImage:
			xlsDataName = "KEYWORD_BOT_MENU_SHOW_IMAGE";
			break;
		case eBottomGuide.TabMove:
			xlsDataName = "KEYWORD_BOT_MENU_TAB";
			break;
		case eBottomGuide.TabSelect:
			xlsDataName = "KEYWORD_BOT_MENU_TAB_SEL";
			break;
		case eBottomGuide.TabCancel:
			xlsDataName = "KEYWORD_BOT_MENU_TAB_CANCEL";
			break;
		case eBottomGuide.Confirm:
			xlsDataName = "KEYWORD_BOT_MENU_CONFIRM";
			break;
		case eBottomGuide.KeywordRecord:
			xlsDataName = "KEYWORD_BOT_MENU_LOG";
			break;
		case eBottomGuide.InitAllArrangeKeyword:
			xlsDataName = "KEYWORD_RESET_BUTTON";
			break;
		}
		return GameGlobalUtil.GetXlsProgramText(xlsDataName);
	}

	private void Update()
	{
		if (m_eKeywordState == KeywordState.MEMO && m_keywordContainer.IsPageScrolling())
		{
			m_keywordContainer.UpdatePageScroll();
		}
		else if (m_eEndMotState != GameDefine.eAnimChangeState.play_end)
		{
			if (m_eKeywordState != KeywordState.MEMO)
			{
				ProcDragSlotGroup();
			}
			ProcInputButton();
		}
	}

	private void KeywordUseOneQuestPop(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			SelUsingKeywordOne(GetSlotPlusByIdx(m_iCurSelSlotIdx));
		}
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_PadStickPushingTime += Time.deltaTime;
		if (m_PadStickPushingTime >= m_InputRepeatTimeBound)
		{
			m_PadStickPushingTime = 0f;
			return true;
		}
		return false;
	}

	private GamePadInput.StickDir GetMoveDir()
	{
		GamePadInput.StickDir result = GamePadInput.StickDir.None;
		float fAxisX = 0f;
		float fAxisY = 0f;
		if (GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
		{
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					result = GamePadInput.StickDir.Left;
					m_PadStickPushingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					result = GamePadInput.StickDir.Right;
					m_PadStickPushingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						result = GamePadInput.StickDir.Left;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					result = GamePadInput.StickDir.Right;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				result = GamePadInput.StickDir.Up;
				m_PadStickPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				result = GamePadInput.StickDir.Down;
				m_PadStickPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					result = GamePadInput.StickDir.Up;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				result = GamePadInput.StickDir.Down;
			}
		}
		return result;
	}

	private void ProcInputButton()
	{
		if (m_goChangingTabCover.activeInHierarchy || m_isTutoKeyLock || m_isTabSelKeyLock || m_isKeyLock || PopupDialoguePlus.IsAnyPopupActivated() || !m_isSetMenuCreating || (m_LoadingIcon != null && m_LoadingIcon.gameObject.activeInHierarchy))
		{
			return;
		}
		if (m_eKeywordState == KeywordState.MEMO && !m_isShowHelpWindow && m_iMaxSlotCnt > 0)
		{
			m_keywordContainer.ProcKeyInput();
		}
		if (ProcEndHWWindow() || ProcEndGame() || ProcArrangeEffectMot())
		{
			return;
		}
		GamePadInput.StickDir stickDir = GetMoveDir();
		if (m_isShowHelpWindow && stickDir == GamePadInput.StickDir.None)
		{
			stickDir = GameGlobalUtil.GetMouseWheelAxis();
		}
		if (stickDir != GamePadInput.StickDir.None)
		{
			if (m_isShowHelpWindow)
			{
				if (m_eKeywordState != KeywordState.MEMO && (stickDir == GamePadInput.StickDir.Left || stickDir == GamePadInput.StickDir.Right))
				{
					ButtonPadInput.AddAndShowPressAnim(m_butHWDirectionButton, PadInput.ButtonState.Down, (stickDir != GamePadInput.StickDir.Left) ? m_butHWRightArrow : m_butHWLeftArrow);
					PadInputDir(stickDir);
					AudioManager.instance.PlayUISound("Menu_Select");
					SetHelpWindow(isShow: true);
				}
			}
			else if (!PopupDialoguePlus.IsAnyPopupActivated())
			{
				PadInputDir(stickDir);
			}
			return;
		}
		float axisValue = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
		stickDir = GameGlobalUtil.GetMouseWheelAxis();
		if (!GameGlobalUtil.IsAlmostSame(axisValue, 0f) || stickDir != GamePadInput.StickDir.None)
		{
			if (m_iSlotTotalPage > 1)
			{
				bool isLeft = axisValue > 0f || stickDir == GamePadInput.StickDir.Left;
				DragSlotGroup(isLeft, isPlaySound: true);
			}
		}
		else if (m_isShowHelpWindow)
		{
			bool flag = m_eKeywordState == KeywordState.MEMO;
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				ButtonPadInput.AddAndShowPressAnim((!flag) ? m_butHWCloseOButton : m_butHWMemoCloseOButton);
				OnClickCloseHWWindow();
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				ButtonPadInput.AddAndShowPressAnim((!flag) ? m_butHWMemoCloseXButton : m_butHWCloseXButton);
				OnClickCloseHWWindow();
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			OnClickSlot();
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_eKeywordState == KeywordState.ARRANGE_MULTI)
			{
				if (m_isButton[10])
				{
					AllDeselect();
					ActionBottomGuide(eBottomGuide.InitAllArrangeKeyword);
					ButtonPadInput.AddAndShowPressAnim(m_butArrangeCancel);
				}
			}
			else if (m_eKeywordState != KeywordState.ARRANGE_ONE && m_isButton[3])
			{
				PressBackButton();
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
		{
			if (m_eKeywordState == KeywordState.ARRANGE_MULTI && m_iKeywordUseSelCnt == m_iCurUsingIdx)
			{
				AudioManager.instance.PlayUISound("Push_SubmissBTN");
				CompKeywordUsing();
				ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_butAnswerConfirm);
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
		{
			if (m_isCurSlotShowImg)
			{
				AudioManager.instance.PlayUISound("Menu_Detail");
				StartCoroutine(OnClickDetailImage(isShow: true));
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TouchPadButton) && m_isButton[9])
		{
			OnClickRecordButton();
		}
	}

	public void OnClickRecordButton()
	{
		AudioManager.instance.PlayUISound("Push_KeyTalk_Popup");
		ButtonPadInput.AddAndShowPressAnim(m_butLogTouchPad);
		ActionBottomGuide(eBottomGuide.KeywordRecord);
		SetHelpWindow(isShow: true);
	}

	public void OnClickSlot()
	{
		if (m_iCurSelSlotIdx == -1)
		{
			return;
		}
		if (m_eKeywordState == KeywordState.TALK)
		{
			if (m_isButton[1])
			{
				KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(m_iCurSelSlotIdx);
				if (slotPlusByIdx != null)
				{
					slotPlusByIdx.PressSuggestBut();
				}
				AudioManager.instance.PlayUISound("Push_KeyTalk_OK");
				ActionBottomGuide(eBottomGuide.Suggest);
				RunEvent(GetSlotPlusByIdx(m_iCurSelSlotIdx));
			}
		}
		else if (m_eKeywordState == KeywordState.ARRANGE_MULTI)
		{
			if (m_isButton[2])
			{
				KeywordSlotPlus slotPlusByIdx2 = GetSlotPlusByIdx(m_iCurSelSlotIdx);
				slotPlusByIdx2.PressConfirmButton();
				ActionBottomGuide(eBottomGuide.SuggCancel);
				SelUsingKeyword(GetSlotPlusByIdx(m_iCurSelSlotIdx));
			}
		}
		else if (m_eKeywordState == KeywordState.ARRANGE_ONE && m_isButton[1] && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			ActionBottomGuide(eBottomGuide.Suggest);
			KeywordSlotPlus slotPlusByIdx3 = GetSlotPlusByIdx(m_iCurSelSlotIdx);
			if (slotPlusByIdx3 != null)
			{
				slotPlusByIdx3.PressSuggestBut();
			}
			AudioManager.instance.PlayUISound("Push_SubmissBTN");
			SelUsingKeywordOne(GetSlotPlusByIdx(m_iCurSelSlotIdx));
		}
	}

	public void PressBackButton()
	{
		if (m_eKeywordState == KeywordState.MEMO)
		{
			AudioManager.instance.PlayUISound("Watch_Out");
		}
		else
		{
			AudioManager.instance.PlayUISound("Push_KeyTalk_Back");
		}
		ActionBottomGuide(eBottomGuide.Exit);
		BackToGame();
	}

	private void Init()
	{
		int num = 0;
		if (m_iCrisisLevel > 0 && (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE))
		{
			num = m_iCrisisLevel;
		}
		StartCoroutine(EventCameraEffect.Instance.Activate_KeywordMenuBG(num));
		m_goVignetting.SetActive(num > 0);
		if (num > 0)
		{
			GameGlobalUtil.PlayUIAnimation(m_animVignetting, num.ToString());
		}
		m_GameSwitch = GameSwitch.GetInstance();
		m_EventEngine = EventEngine.GetInstance();
		m_subMemoMenu = m_GameMain.GetSmartWatchSubMemoMenu();
		if (m_eBefRunKeywordState == m_eKeywordState)
		{
			m_strSelKeywordKey = null;
		}
		m_GameSwitch.SetRunKeywordCharKey(m_strCharKey);
		if (m_eKeywordState == KeywordState.MEMO)
		{
			InitCategoryTab();
		}
		StartCoroutine(SetMenu());
		SetHelpWindow(isShow: false);
	}

	private void InitGameObjectState()
	{
		m_txtAnswerSelCnt.text = string.Empty;
		m_goAnswerGroup.SetActive(value: false);
		m_goQuestionGroup.SetActive(value: false);
		m_goArrangeCancel.SetActive(value: false);
		m_goUpTab.SetActive(value: false);
		for (int i = 0; i < 5; i++)
		{
			if (m_goTitle[i] != null)
			{
				m_goTitle[i].SetActive(value: false);
			}
			if (m_goBackOverlay[i] != null)
			{
				m_goBackOverlay[i].SetActive(value: false);
			}
		}
		m_goTalkBalloon.SetActive(value: false);
		m_goKeyDetailGroup.SetActive(value: false);
		m_goMemoMultiply.SetActive(value: false);
		m_goNotMemoMultiply.SetActive(value: false);
		m_goMultiType3.SetActive(value: false);
		m_TabContainer.getTabCreateInfoFP = null;
		m_TabContainer.onChangedSelectTab = null;
		for (eBottomGuide eBottomGuide = eBottomGuide.Cursor; eBottomGuide < eBottomGuide.Count; eBottomGuide++)
		{
			OnBottomGuide(eBottomGuide, isShow: false);
		}
		m_goRightStickUI.SetActive(value: false);
		m_goAnswerConfirm.SetActive(value: false);
		m_goCondition.SetActive(value: false);
		m_goBackButton.SetActive(value: false);
		m_goCloseButton.SetActive(value: false);
		if (m_goTouchInputBlock != null)
		{
			m_goTouchInputBlock.SetActive(value: false);
		}
	}

	private void OnApplicationQuit()
	{
		m_isQuitApplication = true;
	}

	private void OnDisable()
	{
		ButtonSpriteSwap.DeleteAllButton();
		if (m_eKeywordState == KeywordState.TALK && m_strCharKey == null)
		{
			if (m_EventEngine != null && m_EventEngine.m_TalkChar != null)
			{
				m_EventEngine.m_TalkChar.RevertPartyCharKeywordSet(null);
			}
		}
		else if (m_eKeywordState == KeywordState.MEMO)
		{
			m_keywordContainer.ClearSlotObjects();
		}
		m_iCurCategory = NONE_CATE_IDX;
		DeleteRes();
		EventCameraEffect.Instance.Deactivate_KeywordMenuBG();
		InitGameObjectState();
		if (!m_isQuitApplication)
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
			m_GameMain.m_CommonButtonGuide.SetShow(isShow: false);
		}
		if (m_GameMain.GetGameMainState() == GameMain.eGameMainState.RunEvent && m_GameMain != null && m_GameMain.m_EventEngine != null)
		{
			m_GameMain.m_EventEngine.SetEventBotGuide(isShow: true);
		}
		if (!m_isQuitApplication && m_closeButton != null && m_closeButton.onClosed != null)
		{
			m_closeButton.onClosed(null, null);
		}
		if (m_eKeywordState == KeywordState.TALK && m_strRunKeywordKey != string.Empty && m_GameSwitch != null)
		{
			m_GameSwitch.TempSaveCurCharRelation(m_strRunKeywordKey, m_strCharKey);
		}
		if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			FreeUsingRes();
		}
		m_isKewordMenuPlusOn = false;
		if (m_CategoryInfos != null)
		{
			m_CategoryInfos.Clear();
		}
		if (m_iListTabOrder != null)
		{
			m_iListTabOrder.Clear();
		}
		m_iListTabOrder = null;
		if (m_validXlsDatas != null)
		{
			m_validXlsDatas.Clear();
		}
		if (m_listKeyUseResultSlot != null)
		{
			m_listKeyUseResultSlot.Clear();
		}
		m_listKeyUseResultSlot = null;
		m_subMemoMenu = null;
		m_GameMain = null;
		m_EventEngine = null;
		m_GameSwitch = null;
	}

	public void OnDestroy()
	{
		if (m_LoadingIcon != null)
		{
			UnityEngine.Object.Destroy(m_LoadingIcon.gameObject);
			m_LoadingIcon = null;
		}
		if (m_keywordContainer != null)
		{
			m_keywordContainer.Release();
		}
		FreeSelectAnswer();
	}

	public void FreeKeywordMenu()
	{
		m_isEndTypeRunEvent = false;
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
		m_GameSwitch.SetRunKeywordEvt(isRun: false);
		m_strRunKeywordKey = null;
		m_iRunKeywordPage = 0;
	}

	public void SetMenuState(KeywordState eKeywordState, string strCharKey = null, GameMain clGameMain = null, bool isFromKeywordEvt = false, bool isShowBackButton = true, bool isFinalSeq = false)
	{
		m_eKeywordState = eKeywordState;
		if (eKeywordState != KeywordState.MEMO)
		{
			SetClosedFunc();
		}
		m_strCharKey = strCharKey;
		m_GameMain = clGameMain;
		m_isFromKeywordEvt = isFromKeywordEvt;
		m_isShowBackButton = isShowBackButton;
		m_isFromRunKeyword = false;
	}

	public void SetUseKeyword(string strKeywordUseID, int iSelCnt, GameMain clGameMain = null)
	{
		m_iKeywordUseSelCnt = iSelCnt;
		m_eKeywordState = ((iSelCnt != 1) ? KeywordState.ARRANGE_MULTI : KeywordState.ARRANGE_ONE);
		if (m_eKeywordState != KeywordState.MEMO)
		{
			SetClosedFunc();
		}
		m_strKeywordUsedID = strKeywordUseID;
		m_GameMain = clGameMain;
		m_isReUsingSelList = false;
		m_isArrangeOpeningEffect = true;
		m_eArrangeOpeningEffectMotState = GameDefine.eAnimChangeState.none;
	}

	public void SetReUseKeyword(GameMain clGameMain = null)
	{
		m_GameMain = clGameMain;
		m_isReUsingSelList = true;
		m_isArrangeOpeningEffect = false;
		m_eArrangeOpeningEffectMotState = GameDefine.eAnimChangeState.none;
	}

	public void SetClosedFunc(GameDefine.EventProc fpClosedCB = null)
	{
		if (m_closeButton != null)
		{
			m_closeButton.onClickedCloseButton = null;
			m_closeButton.onClosed = fpClosedCB;
		}
	}

	public void SetFromEndRunKeyword(GameMain clGameMain = null)
	{
		m_eKeywordState = KeywordState.TALK;
		m_GameMain = clGameMain;
		m_isFromKeywordEvt = true;
		m_isFromRunKeyword = true;
	}

	private IEnumerator SetMenu()
	{
		m_isSetMenuCreating = false;
		m_iCurSequence = m_GameSwitch.GetCurSequence();
		m_isEndTypeRunEvent = false;
		string strTitleKey = null;
		int iKeywordIdx = (int)m_eKeywordState;
		Sprite sprDetailBG = null;
		int iTitleColor = 0;
		bool isSetArrangeQuestion = false;
		switch (m_eKeywordState)
		{
		case KeywordState.MEMO:
			strTitleKey = "COLL_KEYWORD_MENU_TITLE";
			break;
		case KeywordState.TALK:
			strTitleKey = "TALK_MENU_TITLE";
			m_EventEngine.m_TalkChar.ChangeCharMot(m_strCharKey, m_GameSwitch.GetCharPartyMotion(m_strCharKey), m_GameSwitch.GetCharPartyDir(m_strCharKey), isSaveCurMot: false);
			break;
		case KeywordState.ARRANGE_MULTI:
			m_goAnswerGroup.SetActive(value: true);
			isSetArrangeQuestion = true;
			m_textAnswerConfirm.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_USE_CONFIRM_BUTTON");
			m_goArrangeCancel.SetActive(value: true);
			m_textArrangeCancel.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_RESET_BUTTON");
			break;
		case KeywordState.ARRANGE_ONE:
			isSetArrangeQuestion = true;
			break;
		}
		if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			Xls.KeywordUsing data_byKey = Xls.KeywordUsing.GetData_byKey(m_strKeywordUsedID);
			int num = 0;
			if (data_byKey != null)
			{
				num = data_byKey.m_iTitleTextType;
			}
			strTitleKey = num switch
			{
				0 => "QUESTION_MENU_TITLE", 
				1 => "ARRANGE_MENU_TITLE", 
				2 => "DETECTIVE_MENU_TITLE", 
				_ => "ORDER_ARRANGE_MENU_TITLE", 
			};
		}
		Text textLogIcon = m_textLogIcon;
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("KEYWORD_LOG");
		m_textHWLog.text = xlsProgramText;
		textLogIcon.text = xlsProgramText;
		if (isSetArrangeQuestion)
		{
			m_goQuestionGroup.SetActive(value: true);
			Xls.KeywordUsing data_byKey2 = Xls.KeywordUsing.GetData_byKey(m_strKeywordUsedID);
			if (data_byKey2 != null)
			{
				string xlsTextData = GameGlobalUtil.GetXlsTextData(data_byKey2.m_strQuestTextID);
				if (xlsTextData != null)
				{
					m_textQuestion.text = xlsTextData;
				}
			}
			RectTransform component = m_goQuestionIcon.GetComponent<RectTransform>();
			component.localPosition = new Vector3(0f - (m_textQuestion.preferredWidth / 2f + component.rect.width), component.localPosition.y, component.localPosition.z);
		}
		if (m_eKeywordState == KeywordState.MEMO)
		{
			sprDetailBG = m_sprDetailBG_Memo;
			iTitleColor = 12437695;
			m_goMemoMultiply.SetActive(value: true);
		}
		else if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
		{
			sprDetailBG = m_sprDetailBG_Event;
			m_goMultiType3.SetActive(value: true);
			m_textEventKeywordTitle.text = GameGlobalUtil.GetXlsProgramText("TITLE_EVENT_KEYWORD_MENU");
			m_textCurGameTime.text = GameGlobalUtil.GetCurrentGameTimeString();
			Xls.TalkCutSetting data_byIdx = Xls.TalkCutSetting.GetData_byIdx(m_GameSwitch.GetCurCutIdx());
			if (data_byIdx != null)
			{
				m_textEventKeywordCutName.text = GameGlobalUtil.GetXlsTextData(data_byIdx.m_strIDCutName);
			}
			m_textEventKeywordGetCnt.text = $"{m_GameSwitch.GetMustGetKeywordCnt():D2}";
			m_textEventKeywordMaxCnt.text = $"{m_GameSwitch.GetMustMaxKeywordCnt():D2}";
			m_textEventKeywordSlash.text = GameGlobalUtil.GetXlsProgramText("STR_SLASH");
			iTitleColor = 4078387;
		}
		else
		{
			Xls.TalkCutSetting data_byIdx2 = Xls.TalkCutSetting.GetData_byIdx(m_GameSwitch.GetCurCutIdx());
			if (data_byIdx2 != null)
			{
				string xlsTextData2 = GameGlobalUtil.GetXlsTextData(data_byIdx2.m_strIDCutName);
				if (xlsTextData2 != null)
				{
					m_textSubTitle.text = xlsTextData2;
				}
			}
			sprDetailBG = m_sprDetailBG_Commu;
			iTitleColor = 4066099;
			m_goNotMemoMultiply.SetActive(value: true);
		}
		int iXlsChrCnt = Xls.CharData.GetDataCount();
		for (int i = 0; i < iXlsChrCnt; i++)
		{
			Xls.CharData xlsCharData = Xls.CharData.GetData_bySwitchIdx(i);
			if (xlsCharData != null)
			{
				int iIdx = xlsCharData.m_iUseIdx;
				if (BitCalc.CheckArrayIdx(iIdx, 5) && !(m_sprDotFace[iIdx] != null))
				{
					string strPath = xlsCharData.m_strDotIconImage;
					strPath = GameGlobalUtil.GetPathStrFromImageXls(strPath);
					m_sprDotFace[iIdx] = MainLoadThing.instance.keywordUIImageManager.GetThumbnailImageInCache(strPath);
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			string strPath = $"keyword_mental_state_{j}";
			strPath = GameGlobalUtil.GetPathStrFromImageXls(strPath);
			m_sprMental[j] = MainLoadThing.instance.keywordUIImageManager.GetThumbnailImageInCache(strPath);
		}
		m_animUIBGOverColor = m_animUIBGOveray[iKeywordIdx];
		m_animMenuTitle = m_animMenuEachTitle[iKeywordIdx];
		if (m_goTitle[iKeywordIdx] != null)
		{
			m_goTitle[iKeywordIdx].SetActive(value: true);
		}
		if (m_goBackOverlay[iKeywordIdx] != null)
		{
			m_goBackOverlay[iKeywordIdx].SetActive(value: true);
		}
		m_imgKeyDetailBack.sprite = sprDetailBG;
		m_textKeyTitle.color = GameGlobalUtil.HexToColor(iTitleColor);
		if (strTitleKey != null && m_textTitle[iKeywordIdx] != null)
		{
			m_textTitle[iKeywordIdx].text = GameGlobalUtil.GetXlsProgramText(strTitleKey);
		}
		string strClose = GameGlobalUtil.GetXlsProgramText("KEYWORD_HELP_WINDOW_CLOSE");
		m_textHWCloseButton.text = strClose;
		if (m_textHWMemoCloseButton != null)
		{
			m_textHWMemoCloseButton.text = strClose;
		}
		string strHelpWindowChangeKeyword = GameGlobalUtil.GetXlsProgramText("KEYWORD_HELP_WINDOW_CHANGE_KEYWORD");
		if (strHelpWindowChangeKeyword != null)
		{
			m_textHWDirectionButton.text = strHelpWindowChangeKeyword;
		}
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		if (m_eKeywordState != KeywordState.MEMO)
		{
			yield return StartCoroutine(SetSlot(0, isFirst: true));
		}
		else
		{
			SetBeforeTab();
			SetTab(isFirst: true);
		}
		if ((m_eKeywordState == KeywordState.ARRANGE_ONE || m_eKeywordState == KeywordState.ARRANGE_MULTI) && m_isArrangeOpeningEffect)
		{
			m_goArrangeOpeningEffect.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation(m_aniArrangeOpeningEffect, GameDefine.UIAnimationState.appear, ref m_eArrangeOpeningEffectMotState);
		}
		if (m_eKeywordState == KeywordState.TALK && m_iMaxSlotCnt == 0)
		{
			PopupDialoguePlus.ShowPopup_OK(GameGlobalUtil.GetXlsProgramText("HAVE_NO_USABLE_KEYWORD"), CB_PopupExit);
		}
		m_isSetMenuCreating = true;
		if (m_eKeywordState != KeywordState.ARRANGE_MULTI && m_eKeywordState != KeywordState.ARRANGE_ONE)
		{
			bool flag = m_eKeywordState == KeywordState.TALK;
			m_goBackButton.SetActive(!flag);
			m_goCloseButton.SetActive(flag);
		}
	}

	private void CB_PopupExit(PopupDialoguePlus.Result result)
	{
		BackToGame();
	}

	private int SeparateMultiKeyword(string strKeyword, ref string[] strSaveArr)
	{
		char[] array = strKeyword.ToCharArray();
		char[] array2 = new char[20];
		int num = array.Length;
		int num2 = strSaveArr.Length;
		int num3 = 0;
		int length = 0;
		bool flag = false;
		BitCalc.InitArray(strSaveArr);
		for (int i = 0; i < num; i++)
		{
			if (array[i] == ',' || array[i] == '\0')
			{
				flag = true;
			}
			else if (i == num - 1)
			{
				array2[length++] = array[i];
				flag = true;
			}
			else if (array[i] != ' ')
			{
				array2[length++] = array[i];
			}
			if (flag)
			{
				strSaveArr[num3] = new string(array2);
				strSaveArr[num3] = strSaveArr[num3].Substring(0, length);
				num3++;
				length = 0;
				BitCalc.InitArray(array2);
				flag = false;
				if (num3 >= num2)
				{
					break;
				}
			}
		}
		return num3;
	}

	public void FreeKeywordMenuForFinishPM()
	{
		m_strRunKeywordKey = null;
		FreeUsingRes();
		FreeSelectAnswer();
	}

	private void FreeSelectAnswer()
	{
		if (m_listSelectAnswer != null)
		{
			int count = m_listSelectAnswer.Count;
			for (int i = 0; i < count; i++)
			{
				m_listSelectAnswer[i].m_strQuestID = null;
				m_listSelectAnswer[i].m_strArrUsingSelList = null;
			}
			m_listSelectAnswer.Clear();
		}
	}

	private SelectAnswer InitUsingSelList()
	{
		SelectAnswer selectAnswer = FindSelectAnswer(m_strKeywordUsedID);
		if (selectAnswer == null)
		{
			selectAnswer = new SelectAnswer();
			selectAnswer.m_strQuestID = m_strKeywordUsedID;
			BitCalc.InitArray(selectAnswer.m_strArrUsingSelList);
			m_listSelectAnswer.Add(selectAnswer);
		}
		return selectAnswer;
	}

	private SelectAnswer FindSelectAnswer(string strQuestID)
	{
		SelectAnswer result = null;
		int count = m_listSelectAnswer.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listSelectAnswer[i].m_strQuestID == strQuestID)
			{
				result = m_listSelectAnswer[i];
				break;
			}
		}
		return result;
	}

	private void FreeUsingRes()
	{
		if (m_listUsingAns != null && m_listUsingAns.Count > 0)
		{
			for (int i = 0; i < MAX_USING_SEL_CNT; i++)
			{
				m_listUsingAns[i].DestroySprite();
				UnityEngine.Object.Destroy(m_listUsingAns[i].m_goSelObj);
			}
			m_listUsingAns.Clear();
			m_listUsingAns = null;
		}
	}

	private void SetUsingSelIdx()
	{
		if (m_listUsingAns == null)
		{
			m_listUsingAns = new List<SelUsingAns>();
		}
		FreeUsingRes();
		SelectAnswer selectAnswer = InitUsingSelList();
		m_iCurUsingIdx = 0;
		if (m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			return;
		}
		for (int i = 0; i < MAX_USING_SEL_CNT; i++)
		{
			if (m_isReUsingSelList && selectAnswer.m_strArrUsingSelList[i] != null)
			{
				m_iCurUsingIdx = m_iKeywordUseSelCnt;
				break;
			}
		}
	}

	private void SetUsingSelList()
	{
		if (m_listUsingAns == null)
		{
			m_listUsingAns = new List<SelUsingAns>();
		}
		FreeUsingRes();
		SelectAnswer selectAnswer = InitUsingSelList();
		if (m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			return;
		}
		float fY = 0f;
		m_iCurUsingIdx = 0;
		Xls.KeywordUsing data_byKey = Xls.KeywordUsing.GetData_byKey(m_strKeywordUsedID);
		int num = -1;
		if (data_byKey != null)
		{
			num = data_byKey.m_iTitleTextType;
		}
		for (int i = 0; i < MAX_USING_SEL_CNT; i++)
		{
			bool flag = i >= m_iKeywordUseSelCnt;
			GameObject gameObject = UnityEngine.Object.Instantiate((!flag) ? m_goAnsSlotNEmpty : m_goAnsSlotEmpty);
			gameObject.name = "using_" + i;
			SelUsingAns selUsingAns = new SelUsingAns(gameObject, m_goAnsParent, flag, this);
			if (m_isReUsingSelList && selectAnswer.m_strArrUsingSelList[i] != null)
			{
				KeywordSlotPlus slotPlusByKeywordKey = GetSlotPlusByKeywordKey(selectAnswer.m_strArrUsingSelList[i]);
				slotPlusByKeywordKey.SetArrangeSelMark(isOn: true);
				fY = selUsingAns.SetAnsDetail(fY, isSel: false, slotPlusByKeywordKey, isSetKeyword: true, selectAnswer.m_strArrUsingSelList[i]);
				m_iCurUsingIdx = m_iKeywordUseSelCnt;
			}
			else
			{
				fY = selUsingAns.SetAnsDetail(fY, i == m_iCurUsingIdx);
			}
			if (num == 3)
			{
				selUsingAns.SetTagOrder(!flag, i + 1);
			}
			else
			{
				selUsingAns.SetTagOrder(isShow: false);
			}
			m_listUsingAns.Add(selUsingAns);
		}
		m_txtAnswerSelCnt.text = $"{m_iCurUsingIdx} / {m_iKeywordUseSelCnt}";
		bool flag2 = m_iCurUsingIdx == m_iKeywordUseSelCnt;
		m_goAnswerConfirm.SetActive(flag2);
		if (flag2)
		{
			AudioManager.instance.PlayUISound("View_SubmissBTN");
		}
	}

	private void SetHelpWindow(bool isShow)
	{
		if (m_listKeyUseResultSlot == null)
		{
			m_listKeyUseResultSlot = new List<KeywordUseResultSlot>();
		}
		int num = 0;
		if (isShow)
		{
			bool flag = true;
			string empty = string.Empty;
			bool flag2 = m_eKeywordState == KeywordState.MEMO;
			if (flag2)
			{
				empty = m_strMemoSelKeywordKey;
				flag = m_GameSwitch.GetKeywordAllState(empty) != 0;
			}
			else
			{
				empty = GetSlotPlusByIdx(m_iCurSelSlotIdx).m_strKeywordKey;
			}
			m_goHWCloseButton.SetActive(!flag2);
			m_goHWDirectionButton.SetActive(!flag2);
			m_goHWMemoCloseButton.SetActive(flag2);
			Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(empty);
			if (!flag2)
			{
				m_goHWArrow.SetActive(m_iMaxSlotCnt > 1);
			}
			else
			{
				m_goHWArrow.SetActive(value: false);
			}
			m_textHWContentTitle.text = ((!flag) ? "-" : GetTextList(data_byKey.m_strTitleID, isTitle: true));
			string text = ((!flag) ? "-" : GetTextList(data_byKey.m_strTitleID, isTitle: false));
			Text textHWImgContentTxt = m_textHWImgContentTxt;
			string text2 = text;
			m_textHWNoImgContentTxt.text = text2;
			textHWImgContentTxt.text = text2;
			m_imgHWDetailImg.sprite = m_imgKeyImage.sprite;
			m_textUseRecordNotExist.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_USE_RECORD_NOT_EXIST");
			m_textTitleRelation.text = GameGlobalUtil.GetXlsProgramText("KEYWORDMENU_HELP_WINDOW_RELATION");
			m_textTitleGetKeyword.text = GameGlobalUtil.GetXlsProgramText("KEYWORDMENU_HELP_WINDOW_GET_KEYWORD");
			byte[] array = new byte[5];
			bool flag3 = false;
			if (flag)
			{
				for (int i = 0; i < 5; i++)
				{
					if (data_byKey != null)
					{
						flag3 = m_GameSwitch.GetCollSelKeywod(i, empty) == 1;
						array[i] = (byte)((!flag3) ? m_GameSwitch.GetCollKeywordUse(i, empty) : 4);
						if (array[i] != 0)
						{
							m_goHWResultEachObj[num].SetActive(value: true);
							KeywordUseResultSlot component = m_goHWResultEachObj[num].GetComponent<KeywordUseResultSlot>();
							component.SetFaceImage(m_sprDotFace[i]);
							component.SetMentalImage(array[i]);
							component.SetCutKeyword(m_GameSwitch.GetCollCutKeyword(i, empty) == 1);
							m_listKeyUseResultSlot.Add(component);
							num++;
						}
					}
				}
			}
			m_textUseRecordNotExist.gameObject.SetActive(num == 0);
			for (int j = num; j < 5; j++)
			{
				m_goHWResultEachObj[j].SetActive(value: false);
			}
			m_eHWEndMotState = GameDefine.eAnimChangeState.none;
			KeywordSlotPlus keywordSlotPlus = null;
			keywordSlotPlus = ((m_eKeywordState != KeywordState.MEMO) ? GetSlotPlusByKeywordKey(empty) : m_MemoSelCursor.m_curOnCursorSlot);
			m_imgHWKeyCardImg.gameObject.SetActive(flag);
			if (flag)
			{
				m_imgHWKeyCardImg.sprite = keywordSlotPlus.GetSelImg();
			}
		}
		if (m_isShowHelpWindow != isShow)
		{
			OnBottomCloseHelp(isShow);
		}
		m_isShowHelpWindow = isShow;
		KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(m_iCurSelSlotIdx);
		if (slotPlusByIdx != null)
		{
			string strKeywordKey = slotPlusByIdx.m_strKeywordKey;
			bool flag4 = false;
			flag4 = ((m_eKeywordState != KeywordState.EVENT_KEYWORD) ? slotPlusByIdx.IsUsableSlot() : (m_GameSwitch.GetKeywordAllState(strKeywordKey) >= 1));
			StartCoroutine(ShowKeywordSlotDetail(strKeywordKey, flag4));
		}
		m_goHelpWindow.SetActive(isShow);
		m_TabContainer.isInputBlock = isShow;
	}

	private void SelUsingKeywordOne(KeywordSlotPlus ksSlot)
	{
		m_iCurUsingIdx = 1;
		CompKeywordUsing(ksSlot.m_strKeywordKey);
	}

	public void AllDeselect()
	{
		int iCurUsingIdx = m_iCurUsingIdx;
		for (int i = 0; i < MAX_USING_SEL_CNT; i++)
		{
			if (m_listUsingAns[i].m_strKeywordKey != null)
			{
				KeywordSlotPlus slotPlusByKeywordKey = GetSlotPlusByKeywordKey(m_listUsingAns[i].m_strKeywordKey);
				SelUsingKeyword(slotPlusByKeywordKey, isForceDel: true);
			}
		}
		for (int j = 0; j < MAX_USING_SEL_CNT; j++)
		{
			m_listUsingAns[j].SetAnsFocus(isFocusOn: false, null);
		}
	}

	private void SelUsingKeyword(KeywordSlotPlus ksSlot, bool isForceDel = false)
	{
		int num = -1;
		bool flag = false;
		flag = !isForceDel && !ksSlot.GetArrangeSelMark();
		float fY = 0f;
		int num2 = m_iCurUsingIdx;
		bool flag2 = false;
		bool flag3 = false;
		if (flag && m_iCurUsingIdx < m_iKeywordUseSelCnt)
		{
			flag2 = true;
			m_iCurUsingIdx++;
			for (int i = 0; i < m_iKeywordUseSelCnt; i++)
			{
				if (m_listUsingAns[i].m_strKeywordKey == null)
				{
					num2 = i;
					break;
				}
			}
			GameSwitch.CheckMinMax(ref m_iCurUsingIdx, 0, m_iKeywordUseSelCnt);
			ksSlot.SetArrangeSelMark(isOn: true);
		}
		if (!flag && m_iCurUsingIdx > 0)
		{
			ksSlot.SetArrangeSelMark(isOn: false);
			m_iCurUsingIdx--;
			flag3 = true;
			GameSwitch.CheckMinMax(ref m_iCurUsingIdx, 0, m_iKeywordUseSelCnt);
			int num3 = -1;
			for (int j = 0; j < MAX_USING_SEL_CNT; j++)
			{
				if (m_listUsingAns[j].m_strKeywordKey == ksSlot.m_strKeywordKey)
				{
					m_listUsingAns[j].SetAnsFocus(isFocusOn: false, ksSlot);
					num3 = j;
					break;
				}
			}
			if (num3 != -1)
			{
				m_listUsingAns[num3].CopyAns();
				num = num3;
				int num4 = -1;
				for (int k = 0; k < m_iKeywordUseSelCnt; k++)
				{
					if (m_listUsingAns[k].m_strKeywordKey == null)
					{
						num4 = k;
						break;
					}
				}
				if (num4 < num)
				{
					num = num4;
				}
			}
		}
		int num5 = 2;
		if (num == -1 && m_iCurUsingIdx < m_iKeywordUseSelCnt)
		{
			num5 = (flag2 ? 2 : 0);
			for (int l = 0; l < m_iKeywordUseSelCnt; l++)
			{
				if (m_listUsingAns[l].m_strKeywordKey == null)
				{
					if (flag2)
					{
						num5--;
					}
					if (num5 == 0)
					{
						num = l;
						break;
					}
				}
			}
		}
		int count = m_listUsingAns.Count;
		for (int m = 0; m < MAX_USING_SEL_CNT && m < count && m_listUsingAns[m] != null; m++)
		{
			if (flag2 && m == num2)
			{
				fY = m_listUsingAns[m].SetAnsDetail(fY, isSel: false, ksSlot, isSetKeyword: true, ksSlot.m_strKeywordKey);
				m_listUsingAns[m].SetAnsFocus(isFocusOn: true, ksSlot);
			}
			else
			{
				fY = m_listUsingAns[m].SetAnsDetail(fY, m == num);
			}
		}
		m_txtAnswerSelCnt.text = $"{m_iCurUsingIdx} / {m_iKeywordUseSelCnt}";
		if (flag2 && m_iCurUsingIdx == m_iKeywordUseSelCnt)
		{
			m_goAnswerConfirm.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation(m_animArrangeAnswerConfirm, GameDefine.UIAnimationState.appear);
		}
		else if (flag3 && m_iCurUsingIdx == m_iKeywordUseSelCnt - 1)
		{
			GameGlobalUtil.PlayUIAnimation(m_animArrangeAnswerConfirm, GameDefine.UIAnimationState.disappear);
		}
		if (flag2)
		{
			AudioManager.instance.PlayUISound("Push_KeyTalk_OK");
		}
		if (flag3)
		{
			AudioManager.instance.PlayUISound("Push_KeyUse_Cancel");
		}
		SetCurMarkBySelKeyword();
		OnBottomGuide(eBottomGuide.Confirm, m_iCurUsingIdx == m_iKeywordUseSelCnt);
	}

	private void PlayUsingListDisappear()
	{
		if (m_listUsingAns != null)
		{
			int count = m_listUsingAns.Count;
			for (int i = 0; i < count; i++)
			{
				m_listUsingAns[i].PlayDisappear();
			}
		}
	}

	private void SetCurMarkBySelKeyword(int iBefSel = -1, int iCurSel = -1)
	{
		int num = -1;
		int num2 = -1;
		KeywordSlotPlus keywordSlotPlus = null;
		KeywordSlotPlus keywordSlotPlus2 = null;
		if (iBefSel != iCurSel)
		{
			keywordSlotPlus = GetSlotPlusByIdx(iBefSel);
			keywordSlotPlus2 = GetSlotPlusByIdx(iCurSel);
			if (keywordSlotPlus2.IsEmptySlot())
			{
				keywordSlotPlus2 = keywordSlotPlus;
				keywordSlotPlus = null;
			}
		}
		else
		{
			iCurSel = m_iCurSelSlotIdx;
			keywordSlotPlus2 = GetSlotPlusByIdx(iCurSel);
		}
		int count = m_listUsingAns.Count;
		for (int i = 0; i < count; i++)
		{
			if (keywordSlotPlus != null && m_listUsingAns[i].m_strKeywordKey == keywordSlotPlus.m_strKeywordKey)
			{
				num = i;
			}
			if (keywordSlotPlus2 != null && m_listUsingAns[i].m_strKeywordKey == keywordSlotPlus2.m_strKeywordKey)
			{
				num2 = i;
			}
		}
		if (num != -1 && m_listUsingAns[num] != null)
		{
			m_listUsingAns[num].SetAnsFocus(isFocusOn: false, keywordSlotPlus);
		}
		if (num2 != -1)
		{
			if (m_listUsingAns[num2] != null)
			{
				m_listUsingAns[num2].SetAnsFocus(isFocusOn: true, keywordSlotPlus2);
			}
			if (keywordSlotPlus2 != null)
			{
				bool arrangeSelMark = keywordSlotPlus2.GetArrangeSelMark();
				string xlsDataName = ((!arrangeSelMark) ? "KEYWORD_SLOT_BUTTON_SUGGEST" : "KEYWORD_SLOT_BUTTON_CANCEL");
				int hexVal = ((!arrangeSelMark) ? 1778211 : 10367265);
				keywordSlotPlus2.SetSlotButtonTextAndColor(GameGlobalUtil.GetXlsProgramText(xlsDataName), GameGlobalUtil.HexToColor(hexVal));
			}
		}
		else
		{
			keywordSlotPlus2.SetSlotButtonTextAndColor(GameGlobalUtil.GetXlsProgramText("KEYWORD_SLOT_BUTTON_SUGGEST"), GameGlobalUtil.HexToColor(1778211));
		}
	}

	private void CompKeywordUsing(string strKeywordKeyForOne = null)
	{
		SelectAnswer selectAnswer = FindSelectAnswer(m_strKeywordUsedID);
		if (selectAnswer != null)
		{
			if (m_eKeywordState == KeywordState.ARRANGE_MULTI)
			{
				for (int i = 0; i < MAX_USING_SEL_CNT; i++)
				{
					selectAnswer.m_strArrUsingSelList[i] = m_listUsingAns[i].m_strKeywordKey;
				}
			}
			else if (m_eKeywordState == KeywordState.ARRANGE_ONE && strKeywordKeyForOne != null)
			{
				selectAnswer.m_strArrUsingSelList[0] = strKeywordKeyForOne;
			}
		}
		OnClickBackToGame();
	}

	public bool CheckLastQuestID(string strQuestID)
	{
		string text = null;
		if (m_listSelectAnswer != null)
		{
			int count = m_listSelectAnswer.Count;
			text = m_listSelectAnswer[count - 1].m_strQuestID;
		}
		return text != null && text == strQuestID;
	}

	public ReturnCheckAns CheckKeywordUsingEach(string strQuestID, string strAns0, string strAns1, string strAns2, string strAns3, string strAns4, string strAns5, string strAns6, string strAns7, string strAns8, string strAns9)
	{
		SelectAnswer selectAnswer = FindSelectAnswer(strQuestID);
		if (selectAnswer == null)
		{
			return ReturnCheckAns.WRONG_QUEST_ID;
		}
		string[] strArrAns = null;
		SetCheckUsingKeywordArr(ref strArrAns, strAns0, strAns1, strAns2, strAns3, strAns4, strAns5, strAns6, strAns7, strAns8, strAns9);
		int num = -1;
		int num2 = 10;
		string[] strSaveArr = new string[num2];
		for (int i = 0; i < USING_KEYWORD_ARG_CNT; i++)
		{
			if (strArrAns[i] == string.Empty)
			{
				continue;
			}
			int num3 = SeparateMultiKeyword(strArrAns[i], ref strSaveArr);
			if (num3 == 0)
			{
				continue;
			}
			if (num3 == 1)
			{
				for (int j = 0; j < MAX_USING_SEL_CNT; j++)
				{
					if (selectAnswer.m_strArrUsingSelList[j] != null && selectAnswer.m_strArrUsingSelList[j] == strSaveArr[0])
					{
						num = i;
						break;
					}
				}
			}
			else if (num3 > 1)
			{
				for (int k = 0; k < MAX_USING_SEL_CNT; k++)
				{
					if (selectAnswer.m_strArrUsingSelList[k] == null)
					{
						continue;
					}
					for (int l = 0; l < num3; l++)
					{
						if (selectAnswer.m_strArrUsingSelList[k] == strSaveArr[l])
						{
							num = i;
							break;
						}
					}
					if (num != -1)
					{
						break;
					}
				}
			}
			if (num != -1)
			{
				break;
			}
		}
		ReturnCheckAns result = ReturnCheckAns.ANS_ALL_WRONG;
		if (num != -1 && num >= 0 && num < MAX_USING_SEL_CNT)
		{
			result = (ReturnCheckAns)(1 + num);
		}
		return result;
	}

	public string[] GetKeywordUseAnswer(string strQuestID)
	{
		string[] array = new string[USING_KEYWORD_ARG_CNT];
		int count = m_listSelectAnswer.Count;
		SelectAnswer selectAnswer = null;
		for (int i = 0; i < count; i++)
		{
			if (m_listSelectAnswer[i].m_strQuestID == strQuestID)
			{
				selectAnswer = m_listSelectAnswer[i];
				break;
			}
		}
		return selectAnswer?.m_strArrUsingSelList;
	}

	public bool CheckKeywordUsing(string strQuestID, string strAns0, string strAns1, string strAns2, string strAns3, string strAns4, string strAns5, string strAns6, string strAns7, string strAns8, string strAns9)
	{
		SelectAnswer selectAnswer = FindSelectAnswer(strQuestID);
		if (selectAnswer == null)
		{
			return false;
		}
		string[] strArrAns = null;
		SetCheckUsingKeywordArr(ref strArrAns, strAns0, strAns1, strAns2, strAns3, strAns4, strAns5, strAns6, strAns7, strAns8, strAns9);
		bool result = true;
		int num = 10;
		string[] strSaveArr = new string[num];
		bool flag = false;
		for (int i = 0; i < USING_KEYWORD_ARG_CNT; i++)
		{
			if (strArrAns[i] == string.Empty)
			{
				continue;
			}
			int num2 = SeparateMultiKeyword(strArrAns[i], ref strSaveArr);
			if (num2 == 0)
			{
				continue;
			}
			flag = false;
			if (num2 == 1)
			{
				for (int j = 0; j < MAX_USING_SEL_CNT; j++)
				{
					if (selectAnswer.m_strArrUsingSelList[j] != null && selectAnswer.m_strArrUsingSelList[j] == strSaveArr[0])
					{
						flag = true;
						break;
					}
				}
			}
			else if (num2 > 1)
			{
				for (int k = 0; k < MAX_USING_SEL_CNT; k++)
				{
					if (selectAnswer.m_strArrUsingSelList[k] == null)
					{
						continue;
					}
					for (int l = 0; l < num2; l++)
					{
						if (selectAnswer.m_strArrUsingSelList[k] == strSaveArr[l])
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			if (!flag)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private void SetCheckUsingKeywordArr(ref string[] strArrAns, string strAns0, string strAns1, string strAns2, string strAns3, string strAns4, string strAns5, string strAns6, string strAns7, string strAns8, string strAns9)
	{
		if (strArrAns != null)
		{
			strArrAns = null;
		}
		strArrAns = new string[USING_KEYWORD_ARG_CNT];
		strArrAns[0] = strAns0;
		strArrAns[1] = strAns1;
		strArrAns[2] = strAns2;
		strArrAns[3] = strAns3;
		strArrAns[4] = strAns4;
		strArrAns[5] = strAns5;
		strArrAns[6] = strAns6;
		strArrAns[7] = strAns7;
		strArrAns[8] = strAns8;
		strArrAns[9] = strAns9;
	}

	private void SetTab(bool isFirst)
	{
		if (m_eKeywordState != KeywordState.MEMO)
		{
			return;
		}
		int count = m_CategoryInfos.Count;
		CommonTabContainerPlus.TabButtonInfo tabButtonInfo = null;
		int num = 0;
		CategoryInfo<Xls.CollKeyword> categoryInfo = null;
		int num2 = 0;
		int num3 = 0;
		if (isFirst && m_TabContainer != null)
		{
			m_TabContainer.BuildTabButtonObjects();
		}
		if (m_strFirstSetTab != null)
		{
			m_TabContainer.SetSelectedTab(m_strFirstSetTab);
			m_strFirstSetTab = null;
		}
		for (int i = 0; i < count; i++)
		{
			tabButtonInfo = m_TabContainer.tabButtonInfos[i];
			num = (int)tabButtonInfo.tag;
			if (m_CategoryInfos.ContainsKey(num))
			{
				categoryInfo = m_CategoryInfos[num];
				num2 = categoryInfo.m_ValidContentCount;
				tabButtonInfo.tabButtonComp.enabled = num2 > 0;
				tabButtonInfo.tabButtonComp.SetVisibleNewSymbol(categoryInfo.m_NewContentCount > 0);
				if (isFirst)
				{
					num3 += num2;
				}
			}
		}
	}

	private void InitCategoryTab()
	{
		m_CategoryInfos.Clear();
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("COLLECTION_KEYWORD_ORDER");
		if (string.IsNullOrEmpty(xlsProgramDefineStr))
		{
			return;
		}
		string[] array = xlsProgramDefineStr.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (m_iListTabOrder == null)
		{
			m_iListTabOrder = new List<int>();
		}
		m_iListTabOrder.Clear();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrEmpty(text))
			{
				int result = -1;
				if (int.TryParse(text, out result) && !m_iListTabOrder.Contains(result))
				{
					m_iListTabOrder.Add(result);
				}
			}
		}
		CategoryInfo<Xls.CollKeyword> categoryInfo = null;
		foreach (int item in m_iListTabOrder)
		{
			string xlsDataName = $"KEYWORD_COLLECTION_CTG_{item:D2}";
			string xlsProgramText = GameGlobalUtil.GetXlsProgramText(xlsDataName);
			categoryInfo = new CategoryInfo<Xls.CollKeyword>(item, xlsProgramText);
			m_CategoryInfos.Add(item, categoryInfo);
		}
		STR_CATEGORY_HIDDEN = GameGlobalUtil.GetXlsProgramText("KEYWORD_COLLECTION_CTG_HIDDEN");
	}

	private List<CommonTabContainerPlus.TabCreateInfo> SetTabCreateInfos()
	{
		List<CommonTabContainerPlus.TabCreateInfo> list = new List<CommonTabContainerPlus.TabCreateInfo>();
		List<GameSwitch.SequenceInfo> sequencceList = m_GameSwitch.GetSequencceList();
		m_strFirstSetTab = null;
		foreach (CategoryInfo<Xls.CollKeyword> value in m_CategoryInfos.Values)
		{
			CommonTabContainerPlus.TabCreateInfo tabCreateInfo = new CommonTabContainerPlus.TabCreateInfo();
			if (value.m_ValidContentCount > 0)
			{
				tabCreateInfo.m_Text = value.Name;
				if (m_strFirstSetTab == null)
				{
					m_strFirstSetTab = value.Name;
				}
			}
			else
			{
				tabCreateInfo.m_Text = STR_CATEGORY_HIDDEN;
			}
			tabCreateInfo.m_Tag = value.ID;
			list.Add(tabCreateInfo);
		}
		return list;
	}

	private void OnChangedSelectTab(object sender, object arg)
	{
		if (sender is CommonTabContainerPlus.TabButtonInfo { tag: not null } tabButtonInfo)
		{
			int num = (int)tabButtonInfo.tag;
			StartCoroutine(SetSlot(num));
			SetActiveTabCover(isOn: false, num);
		}
	}

	private void SetActiveTabCover(bool isOn, int iSel = -1, bool isSelecting = false)
	{
		if (m_goChangingTabCover != null)
		{
			m_goChangingTabCover.SetActive(isOn);
			OnBottomGuide(eBottomGuide.Cursor, !isOn);
			OnBottomGuide(eBottomGuide.Exit, !isOn);
			if (isSelecting)
			{
				OnBottomGuide(eBottomGuide.KeywordRecord, isShow: false);
			}
			else
			{
				OnBottomGuide(eBottomGuide.KeywordRecord, !isOn);
			}
		}
	}

	private void OnChangingSelectTab(object sender, object arg)
	{
		CommonTabContainerPlus.TabButtonInfo tabButtonInfo = sender as CommonTabContainerPlus.TabButtonInfo;
		if (m_eKeywordState != KeywordState.MEMO)
		{
			return;
		}
		bool flag = (bool)arg;
		if (!m_isBackUp && flag)
		{
			OnBottomCloseHelp(isShow: true);
			m_isBackUp = true;
		}
		if (!flag)
		{
			if ((tabButtonInfo != null && m_iCurCategory == (int)tabButtonInfo.tag) || tabButtonInfo == null)
			{
				OnBottomCloseHelp(flag);
			}
			m_isBackUp = false;
		}
		OnBottomGuide(eBottomGuide.TabSelect, flag);
		OnBottomGuide(eBottomGuide.TabCancel, flag);
		m_isTabSelKeyLock = (bool)arg;
		if (m_goChangingTabCover != null)
		{
			m_goChangingTabCover.SetActive((bool)arg);
		}
	}

	private void OnProc_PressedTabButtons(object sender, object arg)
	{
		switch ((PadInput.GameInput)arg)
		{
		case PadInput.GameInput.CircleButton:
			ActionBottomGuide(eBottomGuide.TabSelect);
			break;
		case PadInput.GameInput.CrossButton:
			ActionBottomGuide(eBottomGuide.TabCancel);
			break;
		case PadInput.GameInput.L1Button:
		case PadInput.GameInput.R1Button:
			ActionBottomGuide(eBottomGuide.TabMove);
			break;
		case PadInput.GameInput.TouchPadButton:
			ActionBottomGuide(eBottomGuide.KeywordRecord);
			break;
		case PadInput.GameInput.TriangleButton:
		case PadInput.GameInput.SquareButton:
		case PadInput.GameInput.OptionButton:
			break;
		}
	}

	private void GetSlotImageRes(string strImgName)
	{
		m_slotImgRes = null;
		if (m_listSlotImgRes == null)
		{
			m_listSlotImgRes = new List<clSlotImgRes>();
		}
		int count = m_listSlotImgRes.Count;
		for (int i = 0; i < count; i++)
		{
			clSlotImgRes clSlotImgRes = m_listSlotImgRes[i];
			if (clSlotImgRes.m_strSelImgName.Equals(strImgName))
			{
				m_slotImgRes = clSlotImgRes;
				break;
			}
		}
		if (m_slotImgRes == null)
		{
			Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(strImgName);
			if (data_byKey != null)
			{
				m_slotImgRes = new clSlotImgRes();
				string strAssetPath = data_byKey.m_strAssetPath;
				m_slotImgRes.m_sprSel = MainLoadThing.instance.keywordIconImageManager.GetThumbnailImageInCache(strAssetPath + "_s");
				m_slotImgRes.m_sprNSel = MainLoadThing.instance.keywordIconImageManager.GetThumbnailImageInCache(strAssetPath);
				m_slotImgRes.m_strSelImgName = strImgName;
				m_listSlotImgRes.Add(m_slotImgRes);
			}
		}
	}

	private IEnumerator GetDetailImageRes(string strDetailName)
	{
		m_clDetailImgRes = null;
		if (m_listDetailRes == null)
		{
			m_listDetailRes = new List<clDetailImgRes>();
		}
		int iListCnt = m_listDetailRes.Count;
		for (int i = 0; i < iListCnt; i++)
		{
			clDetailImgRes tmpDetailRes = m_listDetailRes[i];
			if (tmpDetailRes.m_strImgName.Equals(strDetailName))
			{
				m_clDetailImgRes = tmpDetailRes;
				break;
			}
		}
		if (m_clDetailImgRes == null)
		{
			m_clDetailImgRes = new clDetailImgRes();
			Xls.CollImages xlsCollImages = Xls.CollImages.GetData_byKey(strDetailName);
			if (xlsCollImages != null)
			{
				string strImgID = xlsCollImages.m_strIDImg;
				yield return StartCoroutine(GameGlobalUtil.GetSprRequestFromImageXls(strImgID));
				m_clDetailImgRes.m_sprDetail = GameGlobalUtil.m_sprLoadFromImgXls;
				m_clDetailImgRes.m_strImgName = strDetailName;
				m_listDetailRes.Add(m_clDetailImgRes);
			}
		}
	}

	private void SetLoadingIconOn(bool isOn, bool isInputLock = false)
	{
		if (m_LoadingIcon == null)
		{
			m_LoadingIcon = LoadingSWatchIcon.Create(m_goLoadingIconPosition);
		}
		if (m_LoadingIcon != null)
		{
			m_LoadingIcon.gameObject.SetActive(isOn);
		}
		m_goKeywordItemGroup.SetActive(!isOn);
		if (isInputLock)
		{
			m_TabContainer.isInputBlock = isOn;
		}
		if (m_goTouchInputBlock != null)
		{
			m_goTouchInputBlock.SetActive(isOn);
		}
	}

	private void SetBeforeTab()
	{
		int dataCount = Xls.CollKeyword.GetDataCount();
		sbyte b = -1;
		for (int i = 0; i < dataCount; i++)
		{
			Xls.CollKeyword data_byIdx = Xls.CollKeyword.GetData_byIdx(i);
			b = m_GameSwitch.GetKeywordAllState(data_byIdx.m_iIndex);
			if (b != 0)
			{
				GameSwitch.KeywordSet keywordSet = new GameSwitch.KeywordSet();
				keywordSet.m_strIDKeyword = data_byIdx.m_strKey;
				if (m_CategoryInfos.ContainsKey(data_byIdx.m_Sequence))
				{
					m_CategoryInfos[data_byIdx.m_Sequence].m_ValidContentCount++;
				}
			}
		}
	}

	private IEnumerator SetSlot(int iCategory, bool isFirst = false)
	{
		if (m_iCurCategory == iCategory)
		{
			yield break;
		}
		SetLoadingIconOn(isOn: true, isInputLock: true);
		while (!MainLoadThing.instance.isCompleteKeywordIconImageLoad)
		{
			yield return null;
		}
		m_iCurCategory = iCategory;
		m_SelKeywordSlot = null;
		HideKeywordSlotDetail();
		DeleteCurSlot();
		DeletePage();
		m_validXlsDatas.Clear();
		if (m_eKeywordState == KeywordState.MEMO && m_keywordContainer != null)
		{
			m_keywordContainer.ClearSlotObjects();
		}
		if (!m_isSetFirstAnchoredPosition)
		{
			m_vecGridAnchoredPosition = m_rtfKeywordGroupContent.anchoredPosition;
			m_isSetFirstAnchoredPosition = true;
		}
		m_rtfKeywordGroupContent.anchoredPosition = m_vecGridAnchoredPosition;
		m_iSlotTotalPage = 0;
		m_fDestContentX = 0f;
		KeywordMenuPlus keywordMenuPlus = this;
		int iRunKeywordPage = 0;
		keywordMenuPlus.m_iSlotCurPage = 0;
		m_iRunKeywordPage = iRunKeywordPage;
		SetDrag(isOn: false);
		bool isSetBeforeState = false;
		int iCateKeywordsCnt = 0;
		int iCntKeywordList = 0;
		GameSwitch.SequenceInfo sqInfo = null;
		List<GameSwitch.KeywordSet> listKeywordSet = null;
		if (m_eKeywordState != KeywordState.MEMO)
		{
			sqInfo = m_GameSwitch.GetSeqInfo(m_iCurSequence);
			listKeywordSet = sqInfo.m_clKeyword;
		}
		if (m_eKeywordState == KeywordState.MEMO)
		{
			listKeywordSet = new List<GameSwitch.KeywordSet>();
			iCntKeywordList = Xls.CollKeyword.GetDataCount();
			foreach (CategoryInfo<Xls.CollKeyword> value in m_CategoryInfos.Values)
			{
				value.m_NewContentCount = 0;
			}
			for (int i = 0; i < iCntKeywordList; i++)
			{
				Xls.CollKeyword keyword = Xls.CollKeyword.GetData_byIdx(i);
				sbyte keywordAllState = m_GameSwitch.GetKeywordAllState(keyword.m_iIndex);
				if (keywordAllState == 0)
				{
					continue;
				}
				GameSwitch.KeywordSet keywordSet = new GameSwitch.KeywordSet();
				keywordSet.m_strIDKeyword = keyword.m_strKey;
				listKeywordSet.Add(keywordSet);
				if (keyword.m_Sequence == iCategory)
				{
					iCateKeywordsCnt++;
				}
				if (m_CategoryInfos.ContainsKey(keyword.m_Sequence))
				{
					m_CategoryInfos[keyword.m_Sequence].m_ValidContentCount++;
					if (keywordAllState == 1)
					{
						m_CategoryInfos[keyword.m_Sequence].m_NewContentCount++;
					}
				}
			}
		}
		else if (m_eKeywordState == KeywordState.TALK)
		{
			iCntKeywordList = (iRunKeywordPage = m_GameSwitch.GetShowKeywordCntBySeq(m_iCurSequence));
			iCateKeywordsCnt = iRunKeywordPage;
		}
		else if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			string text = string.Empty;
			Xls.KeywordUsing data_byKey = Xls.KeywordUsing.GetData_byKey(m_strKeywordUsedID);
			if (data_byKey != null)
			{
				text = data_byKey.m_strKeywordSet;
			}
			if (text == string.Empty)
			{
				sqInfo = m_GameSwitch.GetSeqInfo(m_iCurSequence);
				listKeywordSet = sqInfo.m_clKeyword;
				iCntKeywordList = (iRunKeywordPage = m_GameSwitch.GetShowKeywordCntBySeq(m_iCurSequence, isUsedCheck: false));
				iCateKeywordsCnt = iRunKeywordPage;
			}
			else
			{
				listKeywordSet = new List<GameSwitch.KeywordSet>();
				string[] strSaveArr = new string[100];
				SeparateMultiKeyword(text, ref strSaveArr);
				for (int j = 0; strSaveArr[j] != null; j++)
				{
					GameSwitch.KeywordSet keywordSet2 = new GameSwitch.KeywordSet();
					keywordSet2.m_strIDKeyword = strSaveArr[j].ToString();
					listKeywordSet.Add(keywordSet2);
				}
				iCateKeywordsCnt = listKeywordSet.Count;
			}
		}
		else if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
		{
			string[] cutKeywordList = m_GameSwitch.GetCutKeywordList();
			int num = cutKeywordList.Length;
			listKeywordSet = new List<GameSwitch.KeywordSet>();
			for (int k = 0; k < num; k++)
			{
				if (cutKeywordList[k] != null)
				{
					GameSwitch.KeywordSet keywordSet3 = new GameSwitch.KeywordSet();
					keywordSet3.m_strIDKeyword = cutKeywordList[k];
					listKeywordSet.Add(keywordSet3);
				}
			}
			iCateKeywordsCnt = m_GameSwitch.GetShowEventKeywordCntByCut();
		}
		m_iMaxSlotCnt = iCateKeywordsCnt;
		if (iCateKeywordsCnt > 0)
		{
			OnBottomGuide(eBottomGuide.Cursor, iCateKeywordsCnt >= 1);
			if (m_listKeySlot == null)
			{
				m_listKeySlot = new List<KeywordSlotPlus>();
			}
			int iRestDivisionEight = iCateKeywordsCnt % 8;
			m_iShowKeywordCnt = ((iRestDivisionEight <= 0) ? iCateKeywordsCnt : (iCateKeywordsCnt + (8 - iRestDivisionEight)));
			int iHalfShowKeyword = m_iShowKeywordCnt >> 1;
			m_iSlotTotalPage = m_iShowKeywordCnt / 8;
			string strKeywordPreName = GameGlobalUtil.GetXlsProgramDefineStr("KEYWORDS_SLOT_NAME");
			string strKeywordName = string.Empty;
			m_iPlusCnt = 0;
			if (m_eKeywordState == KeywordState.MEMO)
			{
				KeywordSlotPlus slotComponent = null;
				int iSlotObjectCount = m_listKeySlot.Count;
				for (int l = 0; l < iSlotObjectCount; l++)
				{
					m_listKeySlot[l].rectTransform.SetSiblingIndex(l);
					slotComponent = m_listKeySlot[l];
					if (!(slotComponent == null))
					{
						slotComponent.InitCollectionContent(null, _isValid: false);
					}
				}
				yield return null;
				m_gridKeywordSlots.gameObject.SetActive(value: false);
				m_gridKeywordSlots.gameObject.SetActive(value: true);
				yield return null;
				m_gridKeywordSlots.padding.left = 0;
				m_gridKeywordSlots.enabled = false;
				m_gridKeywordSlots.enabled = true;
				yield return null;
				m_gridKeywordSlots.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				m_gridKeywordSlots.constraintCount = 12;
				yield return null;
				yield return MainLoadThing.instance.StartCoroutine(m_keywordContainer.CreateSlotObjects());
				int iPlusCnt = 0;
				for (int m = 0; m < iCntKeywordList; m++)
				{
					string strKeywordKey = null;
					Xls.CollKeyword keyword = Xls.CollKeyword.GetData_byIdx(m);
					if (m_GameSwitch.GetKeywordAllState(keyword.m_iIndex) == 0 || iPlusCnt >= iCateKeywordsCnt)
					{
						continue;
					}
					strKeywordKey = keyword.m_strKey;
					if (keyword.m_Sequence == iCategory)
					{
						Xls.CollKeyword data_byKey2 = Xls.CollKeyword.GetData_byKey(strKeywordKey);
						if (data_byKey2 != null)
						{
							m_validXlsDatas.Add(data_byKey2);
						}
					}
				}
				if (!isFirst)
				{
					m_keywordContainer.SetOnCursorKeywordData(-1);
					m_keywordContainer.SetKeywordDatas(m_validXlsDatas, isCopyList: true);
				}
			}
			else
			{
				m_gridKeywordSlots.constraintCount = 2;
				m_gridKeywordSlots.constraint = GridLayoutGroup.Constraint.FixedRowCount;
				m_gridKeywordSlots.padding.left = 0;
				if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
				{
					int count = listKeywordSet.Count;
					for (int n = 0; n < count; n++)
					{
						string strKeywordKey = null;
						strKeywordName = string.Empty;
						if (m_iPlusCnt >= iCateKeywordsCnt)
						{
							continue;
						}
						strKeywordKey = listKeywordSet[n].m_strIDKeyword;
						if (strKeywordKey == null)
						{
							continue;
						}
						Xls.CollKeyword data_byKey3 = Xls.CollKeyword.GetData_byKey(strKeywordKey);
						if (m_eKeywordState == KeywordState.EVENT_KEYWORD && data_byKey3.m_iCtg == 0 && data_byKey3 != null)
						{
							MakeSlot(strShowText: GetTextList(data_byKey3.m_strTitleID, isTitle: true), strGoName: strKeywordPreName + strKeywordKey, strKeywordKey: strKeywordKey, isSetEmpty: false, isShowUsedMark: true);
							KeywordSlotPlus slotMadeSlot = m_slotMadeSlot;
							if (m_GameSwitch.GetKeywordAllState(strKeywordKey) == 0)
							{
								slotMadeSlot.SetLockSlot();
							}
						}
					}
					m_SelKeywordSlot = null;
					SelSlot(0, isFirstSet: true);
				}
				else if (m_eKeywordState == KeywordState.TALK)
				{
					List<int> lstKewordByOrder = m_GameSwitch.GetLstKewordByOrder();
					int count2 = lstKewordByOrder.Count;
					for (int num2 = 0; num2 < count2; num2++)
					{
						Xls.CollKeyword data_bySwitchIdx = Xls.CollKeyword.GetData_bySwitchIdx(lstKewordByOrder[num2]);
						if (data_bySwitchIdx != null)
						{
							string strKeywordKey = data_bySwitchIdx.m_strKey;
							strKeywordName = string.Empty;
							if (m_iPlusCnt < iCateKeywordsCnt && m_GameSwitch.GetKeywordSeqUsed(m_iCurSequence, strKeywordKey) == 0)
							{
								MakeSlot(strShowText: GetTextList(data_bySwitchIdx.m_strTitleID, isTitle: true), strGoName: strKeywordPreName + strKeywordKey, strKeywordKey: strKeywordKey, isSetEmpty: false, isShowUsedMark: true);
							}
						}
					}
				}
				else
				{
					int count3 = listKeywordSet.Count;
					for (int num3 = 0; num3 < count3; num3++)
					{
						string strKeywordKey = null;
						strKeywordName = string.Empty;
						if (m_iPlusCnt >= iCateKeywordsCnt)
						{
							continue;
						}
						strKeywordKey = listKeywordSet[num3].m_strIDKeyword;
						if (strKeywordKey != null)
						{
							if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
							{
								SetUsingSelIdx();
							}
							Xls.CollKeyword data_byKey4 = Xls.CollKeyword.GetData_byKey(strKeywordKey);
							if ((m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE) && m_GameSwitch.GetKeywordAllState(strKeywordKey) > 0 && data_byKey4 != null)
							{
								MakeSlot(strShowText: GetTextList(data_byKey4.m_strTitleID, isTitle: true), strGoName: strKeywordPreName + strKeywordKey, strKeywordKey: strKeywordKey, isSetEmpty: false, isShowUsedMark: true);
								_ = m_slotMadeSlot;
							}
							if (m_isFromRunKeyword && strKeywordKey == m_strRunKeywordKey)
							{
								isSetBeforeState = true;
							}
						}
					}
				}
			}
			if (m_eKeywordState == KeywordState.MEMO)
			{
				m_rtfKeywordGroupContent.anchoredPosition = new Vector2(0f, m_rtfKeywordGroupContent.anchoredPosition.y);
			}
			else
			{
				if (m_iPlusCnt > 0 && ((m_eKeywordState != KeywordState.ARRANGE_ONE && m_eKeywordState != KeywordState.ARRANGE_MULTI) || m_isReUsingSelList))
				{
					SelSlot(0, isFirstSet: true);
				}
				for (int num4 = m_iPlusCnt; num4 < m_iShowKeywordCnt; num4++)
				{
					MakeSlot("!_" + num4, null, string.Empty, isSetEmpty: true, isShowUsedMark: true);
				}
				for (int num5 = 0; num5 < LINE_CNT; num5++)
				{
					m_iLineMinIdx[num5] = -1;
					m_iLineMaxIdx[num5] = -1;
				}
				m_iSlotCntPerLine = 4 * m_iSlotTotalPage;
				m_iLineMinIdx[0] = 0;
				m_iLineMinIdx[1] = m_iSlotCntPerLine;
				for (int num6 = 0; num6 < m_iShowKeywordCnt; num6++)
				{
					int siblingIndex;
					if (num6 / 4 % 2 == 0)
					{
						siblingIndex = num6 / 8 * 4 + num6 % 4;
					}
					else
					{
						int num7 = num6 % 4;
						siblingIndex = iHalfShowKeyword + (num6 - num7 - 4) / 2 + num7;
					}
					m_listKeySlot[num6].transform.GetSiblingIndex();
					m_listKeySlot[num6].transform.SetSiblingIndex(siblingIndex);
				}
				for (int num8 = 0; num8 < m_iShowKeywordCnt; num8++)
				{
					KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(num8);
					bool flag = slotPlusByIdx == null || slotPlusByIdx.IsEmptySlot() || slotPlusByIdx.m_strKeywordKey == null;
					if (m_iLineMaxIdx[0] == -1 && num8 < m_iSlotCntPerLine)
					{
						if (flag)
						{
							m_iLineMaxIdx[0] = num8 - 1;
						}
						else if (num8 == m_iSlotCntPerLine - 1)
						{
							m_iLineMaxIdx[0] = num8;
						}
					}
					else if (m_iLineMaxIdx[1] == -1 && num8 >= m_iSlotCntPerLine)
					{
						if (flag)
						{
							m_iLineMaxIdx[1] = num8 - 1;
						}
						else if (num8 == m_iShowKeywordCnt - 1)
						{
							m_iLineMaxIdx[1] = num8;
						}
					}
				}
				if (m_fContentW == -1f)
				{
					m_fContentW = (m_gridKeywordSlots.cellSize.x + m_gridKeywordSlots.spacing.x) * 4f;
					m_fFirstContentX = 0f;
				}
				m_fDestContentX = m_fFirstContentX;
				int iIdx = (isSetBeforeState ? ((m_iRunKeywordPage >= m_iSlotTotalPage) ? (m_iSlotTotalPage - 1) : m_iRunKeywordPage) : 0);
				SetCurPageIdx(iIdx);
				m_fDestContentX -= m_fContentW * (float)m_iSlotCurPage;
				m_rtfKeywordGroupContent.anchoredPosition = new Vector2(m_fDestContentX, m_rtfKeywordGroupContent.anchoredPosition.y);
				m_rtfKeywordGroupContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_fContentW * (float)m_iSlotTotalPage);
			}
			m_goRightStickUI.SetActive(m_iSlotTotalPage > 1);
			if (m_iSlotTotalPage <= 1)
			{
				m_goKeywordArrows.SetActive(value: false);
			}
			else
			{
				m_listPageMarks = new List<PageMarks>();
				m_goKeywordArrows.SetActive(value: true);
				m_goLeftArrow.SetActive(value: true);
				m_goRightArrow.SetActive(value: true);
				m_goPageSel.SetActive(value: true);
				m_goPageNotSel.SetActive(value: true);
				for (int num9 = 0; num9 < m_iSlotTotalPage; num9++)
				{
					PageMarks pageMarks = new PageMarks();
					pageMarks.m_isSelMark = false;
					switch (num9)
					{
					case 0:
						pageMarks.m_isSelMark = true;
						pageMarks.m_goMark = m_goPageSel;
						pageMarks.m_goMark.transform.SetSiblingIndex(0);
						break;
					case 1:
						pageMarks.m_goMark = m_goPageNotSel;
						break;
					default:
						pageMarks.m_isMakeObj = true;
						pageMarks.m_goMark = UnityEngine.Object.Instantiate(m_goPageNotSel);
						pageMarks.m_goMark.name = "n_" + num9;
						pageMarks.m_goMark.transform.SetParent(m_goPageGroup.transform, worldPositionStays: false);
						break;
					}
					m_listPageMarks.Add(pageMarks);
				}
			}
			if (m_eKeywordState == KeywordState.MEMO && !isFirst)
			{
				m_keywordContainer.SetCurrentPage(0);
				m_keywordContainer.SetOnCursorKeywordData(0);
			}
			if (m_isFromRunKeyword)
			{
				string text2 = ((!m_GameSwitch.GetRunLowRelationEvt()) ? m_strSelKeywordKey : m_strRunKeywordKey);
				int count4 = m_listKeySlot.Count;
				KeywordSlotPlus keywordSlotPlus = null;
				if (text2 != null)
				{
					for (int num10 = 0; num10 < count4; num10++)
					{
						KeywordSlotPlus keywordSlotPlus2 = m_listKeySlot[num10];
						if (keywordSlotPlus2 != null && keywordSlotPlus2.m_strKeywordKey == text2)
						{
							keywordSlotPlus = keywordSlotPlus2;
							break;
						}
					}
					if (keywordSlotPlus != null)
					{
						SelSlot(keywordSlotPlus.gameObject.transform.GetSiblingIndex(), isFirstSet: false, isDirectMove: true);
					}
					else
					{
						SelSlot(0, isFirstSet: false, isDirectMove: true);
					}
				}
				m_GameSwitch.SetRunLowRealtionEvt(isSet: false);
			}
			if (isSetBeforeState)
			{
				m_isFromRunKeyword = false;
				m_strRunKeywordKey = null;
				m_iRunKeywordPage = 0;
			}
		}
		m_goRightStickUI.SetActive(m_iSlotTotalPage > 1);
		SetTab(isFirst);
		if (isFirst && (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE))
		{
			SetUsingSelList();
		}
		if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			SetCurMarkBySelKeyword(-1, 0);
		}
		else if (m_eKeywordState == KeywordState.MEMO && m_iMaxSlotCnt == 0)
		{
			m_subMemoMenu.SetNone();
		}
		if (iCateKeywordsCnt > 0)
		{
			int count5 = m_listKeySlot.Count;
			for (int num11 = 0; num11 < count5; num11++)
			{
				if (m_listKeySlot[num11] != null)
				{
					m_listKeySlot[num11].gameObject.SetActive(value: true);
				}
			}
		}
		SetLoadingIconOn(isOn: false, isInputLock: true);
	}

	private void MakeSlot(string strGoName, string strKeywordKey, string strShowText, bool isSetEmpty, bool isShowUsedMark)
	{
		m_slotMadeSlot = null;
		GameObject gameObject = null;
		bool flag = false;
		gameObject = UnityEngine.Object.Instantiate(m_prefabKeywordSlot);
		m_slotMadeSlot = gameObject.GetComponent<KeywordSlotPlus>();
		m_slotMadeSlot.InitSlotState();
		if (!flag)
		{
			gameObject.name = strGoName;
		}
		if (isSetEmpty)
		{
			m_slotMadeSlot.SetEmptySlot();
		}
		else
		{
			sbyte b = (sbyte)((m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE) ? 2 : ((m_eKeywordState != KeywordState.MEMO) ? m_GameSwitch.GetKeywordSeqState(m_iCurSequence, strKeywordKey) : m_GameSwitch.GetKeywordAllState(strKeywordKey)));
			if (b == 0)
			{
				m_slotMadeSlot.SetLockSlot();
			}
			else
			{
				m_slotMadeSlot.SelSlot(isSel: false);
				if (b == 1)
				{
					m_slotMadeSlot.SetNewMark(isOn: true);
				}
				else
				{
					m_slotMadeSlot.SetNewMark(isOn: false);
				}
				Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
				clSlotImgRes clSlotImgRes = null;
				if (data_byKey != null)
				{
					GetSlotImageRes(data_byKey.m_strIconImgID);
					clSlotImgRes = m_slotImgRes;
				}
				if (clSlotImgRes != null)
				{
					m_slotMadeSlot.SetSlotImage(clSlotImgRes.m_sprSel, clSlotImgRes.m_sprNSel);
				}
			}
		}
		gameObject.SetActive(value: false);
		gameObject.transform.SetParent(m_tfKeywordItemGroup, worldPositionStays: false);
		gameObject.transform.SetSiblingIndex(m_iPlusCnt);
		m_slotMadeSlot.m_strKeywordKey = strKeywordKey;
		m_slotMadeSlot.SetMastTextOrNotMaskText(strShowText);
		if (m_eKeywordState == KeywordState.TALK)
		{
			m_slotMadeSlot.SetSlotButtonTextAndColor(GameGlobalUtil.GetXlsProgramText("KEYWORD_SLOT_BUTTON_SUGGEST"), GameGlobalUtil.HexToColor(1778211));
		}
		m_listKeySlot.Add(m_slotMadeSlot);
		if (m_iPlusCnt == 0)
		{
			m_iCurSelSlotIdx = 0;
			if (m_eKeywordState != KeywordState.ARRANGE_ONE && m_eKeywordState != KeywordState.ARRANGE_MULTI)
			{
				SelSlot(m_slotMadeSlot, isFirstSet: true);
			}
		}
		m_iPlusCnt++;
	}

	private void DeleteRes()
	{
		DeletePage();
		DeleteCurSlot();
		DeleteSpriteRes();
		DeleteDetailImg();
		m_sprDetailBack = null;
	}

	private void DeleteDetailImg()
	{
		if (m_listDetailRes == null)
		{
			return;
		}
		int count = m_listDetailRes.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listDetailRes[i].m_sprDetail != null)
			{
				UnityEngine.Object.Destroy(m_listDetailRes[i].m_sprDetail);
			}
			m_listDetailRes[i].m_sprDetail = null;
		}
		m_listDetailRes.Clear();
		m_listDetailRes = null;
	}

	private void DeleteSpriteRes()
	{
		if (m_listSlotImgRes != null)
		{
			int count = m_listSlotImgRes.Count;
			for (int i = 0; i < count; i++)
			{
				clSlotImgRes clSlotImgRes = m_listSlotImgRes[i];
				clSlotImgRes.m_sprNSel = null;
				clSlotImgRes.m_sprSel = null;
			}
			m_listSlotImgRes.Clear();
			m_listSlotImgRes = null;
		}
		for (int j = 0; j < 5; j++)
		{
			m_sprDotFace[j] = null;
		}
		for (int k = 0; k < 4; k++)
		{
			m_sprMental[k] = null;
		}
	}

	private void DeletePage()
	{
		m_goKeywordArrows.SetActive(value: false);
		m_goLeftArrow.SetActive(value: false);
		m_goRightArrow.SetActive(value: false);
		if (m_listPageMarks == null)
		{
			return;
		}
		int count = m_listPageMarks.Count;
		for (int i = 0; i < count; i++)
		{
			PageMarks pageMarks = m_listPageMarks[i];
			if (pageMarks.m_isMakeObj)
			{
				UnityEngine.Object.Destroy(pageMarks.m_goMark);
			}
		}
		m_listPageMarks.Clear();
		m_listPageMarks = null;
	}

	private void MovePage(int iCurPage)
	{
		if (m_listPageMarks == null)
		{
			return;
		}
		int count = m_listPageMarks.Count;
		for (int i = 0; i < count; i++)
		{
			PageMarks pageMarks = m_listPageMarks[i];
			if (pageMarks.m_isSelMark)
			{
				pageMarks.m_goMark.transform.SetSiblingIndex(iCurPage);
				break;
			}
		}
	}

	private void DeleteCurSlot()
	{
		if (m_listKeySlot != null)
		{
			int count = m_listKeySlot.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = m_listKeySlot[i].gameObject;
				gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(gameObject);
			}
			m_listKeySlot.Clear();
			m_listKeySlot = null;
		}
		if (m_listXlsMemo != null)
		{
			m_listXlsMemo.Clear();
			m_listXlsMemo = null;
		}
		m_iCurSelSlotIdx = -1;
	}

	public void OnClickCloseHWWindow()
	{
		AudioManager.instance.PlayUISound("Menu_Cancel");
		GameGlobalUtil.PlayUIAnimation(m_animHWHistoryWindow, GameDefine.UIAnimationState.disappear, ref m_eHWEndMotState);
		GameGlobalUtil.PlayUIAnimation(m_animHWEtc, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animHWKeywordContext, GameDefine.UIAnimationState.disappear);
		for (int i = 0; i < 5; i++)
		{
			GameGlobalUtil.PlayUIAnimation(m_animHWReaction[i], GameDefine.UIAnimationState.disappear);
		}
		GameGlobalUtil.PlayUIAnimation(m_animHWArrow, GameDefine.UIAnimationState.disappear);
	}

	private bool ProcEndHWWindow()
	{
		if (m_eHWEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			return false;
		}
		if (m_eHWEndMotState == GameDefine.eAnimChangeState.none)
		{
			return false;
		}
		if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animHWHistoryWindow, GameDefine.UIAnimationState.disappear, ref m_eHWEndMotState) && m_eHWEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			SetHelpWindow(isShow: false);
		}
		return true;
	}

	private bool ProcArrangeEffectMot()
	{
		if (m_eArrangeOpeningEffectMotState == GameDefine.eAnimChangeState.none || m_eArrangeOpeningEffectMotState == GameDefine.eAnimChangeState.play_end)
		{
			return false;
		}
		if (m_aniArrangeOpeningEffect != null && m_eArrangeOpeningEffectMotState != GameDefine.eAnimChangeState.none && m_eArrangeOpeningEffectMotState != GameDefine.eAnimChangeState.play_end)
		{
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_aniArrangeOpeningEffect, GameDefine.UIAnimationState.appear, ref m_eArrangeOpeningEffectMotState) && m_eArrangeOpeningEffectMotState == GameDefine.eAnimChangeState.play_end)
			{
				SelSlot(0, isFirstSet: true);
				m_eArrangeOpeningEffectMotState = GameDefine.eAnimChangeState.none;
				m_goArrangeOpeningEffect.SetActive(value: false);
				return false;
			}
			return true;
		}
		return false;
	}

	private bool ProcEndGame()
	{
		if (m_eEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			return false;
		}
		if (m_eEndMotState == GameDefine.eAnimChangeState.none)
		{
			if (m_eStartMotState != GameDefine.eAnimChangeState.none && m_eStartMotState != GameDefine.eAnimChangeState.play_end && m_animUIBGOverColor != null && GameGlobalUtil.CheckPlayEndUIAnimation(m_animUIBGOverColor, GameDefine.UIAnimationState.appear, ref m_eStartMotState))
			{
				if (m_eStartMotState == GameDefine.eAnimChangeState.play_end && m_isShowTutorial)
				{
					m_isTutoKeyLock = TutorialPopup.isShowAble(m_strTutorialID);
					if (m_isTutoKeyLock)
					{
						StartCoroutine(TutorialPopup.Show(m_strTutorialID, cbFcTutorialExit, m_canvas));
					}
				}
				return true;
			}
			return false;
		}
		if (m_animUIBGOverColor != null && GameGlobalUtil.CheckPlayEndUIAnimation(m_animUIBGOverColor, GameDefine.UIAnimationState.disappear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			BackToGame();
		}
		return true;
	}

	private void cbFcTutorialExit(object sender, object arg)
	{
		m_isTutoKeyLock = false;
		OnBottomCloseHelp(isShow: false);
	}

	private void BackToGame()
	{
		int childCount = m_rtfSlotDragParent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UnityEngine.Object.Destroy(m_rtfSlotDragParent.GetChild(i).gameObject);
		}
		if (m_isEndTypeRunEvent)
		{
			int curSequence = m_GameSwitch.GetCurSequence();
			if (!m_GameSwitch.GetRunLowRelationEvt() && m_GameSwitch.GetKeywordSeqUsed(curSequence, m_strRunKeywordKey) == 0)
			{
				m_GameSwitch.SetKeywordSeqUsed(curSequence, m_strRunKeywordKey, 1);
			}
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("RUN_KEYWORD_OBJ_NAME");
			if (xlsProgramDefineStr != null)
			{
				m_EventEngine.EnableAndRunObj(xlsProgramDefineStr);
			}
		}
		else if (m_eKeywordState != KeywordState.ARRANGE_MULTI && m_eKeywordState != KeywordState.ARRANGE_ONE)
		{
			if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
			{
				m_GameMain.EndEnterMenuZoomBack();
			}
			else
			{
				m_GameMain.SetGameMainState((m_eKeywordState != KeywordState.MEMO) ? GameMain.eGameMainState.LoadDefScene : GameMain.eGameMainState.SmartWatchMenu);
			}
		}
		m_KeywordMenu.SetActive(value: false);
	}

	public void OnClickBackToGame(bool isRunEvent = false)
	{
		if (m_goTalkBalloon.activeInHierarchy)
		{
			GameGlobalUtil.PlayUIAnimation(m_animTalkBalloon, GameDefine.UIAnimationState.disappear);
		}
		GameGlobalUtil.PlayUIAnimation(m_animSlotBGPanel, GameDefine.UIAnimationState.disappear);
		if (m_iSlotTotalPage > 1)
		{
			GameGlobalUtil.PlayUIAnimation(m_animScrollButton, GameDefine.UIAnimationState.disappear);
		}
		GameGlobalUtil.PlayUIAnimation(m_animKeywordDetailBox, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animHelpWindow, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animDotReaction, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animUIBGOverColor, GameDefine.UIAnimationState.disappear, ref m_eEndMotState);
		for (int i = 0; i < 5; i++)
		{
			if (m_animUIBGOveray[i] != null && m_animUIBGOveray[i].gameObject.activeInHierarchy)
			{
				GameGlobalUtil.PlayUIAnimation(m_animUIBGOveray[i], GameDefine.UIAnimationState.disappear);
			}
			if (m_animMenuEachTitle[i] != null && m_animMenuEachTitle[i].gameObject.activeInHierarchy)
			{
				GameGlobalUtil.PlayUIAnimation(m_animMenuEachTitle[i], GameDefine.UIAnimationState.disappear);
			}
		}
		GameGlobalUtil.PlayUIAnimation(m_animMenuTitle, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animQuestion, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animArrangeAnswerConfirm, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animMemoMultiply, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animNotMemoMultiply, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animMultiType3, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animConditionIcon, GameDefine.UIAnimationState.disappear);
		GameGlobalUtil.PlayUIAnimation(m_animAnswerList, GameDefine.UIAnimationState.disappear);
		PlayUsingListDisappear();
		if (m_listKeySlot != null)
		{
			int count = m_listKeySlot.Count;
			for (int j = 0; j < count; j++)
			{
				m_listKeySlot[j].DisappearSlot(j == m_iCurSelSlotIdx, m_eKeywordState == KeywordState.TALK || m_eKeywordState == KeywordState.ARRANGE_ONE);
			}
		}
	}

	public void SelKeywordSlot(eKeywordTouch eTouchEvt, KeywordSlotPlus goSlot, PointerEventData pointerData = null)
	{
		switch (eTouchEvt)
		{
		case eKeywordTouch.Click:
			if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
			{
				SetCurMarkBySelKeyword(m_iCurSelSlotIdx, goSlot.transform.GetSiblingIndex());
			}
			if (SelSlot(goSlot))
			{
				AudioManager.instance.PlayUISound("Menu_Select");
			}
			break;
		case eKeywordTouch.RunEvent:
			ClickShowButton(goSlot);
			break;
		}
	}

	public void HereDrag_OnPointerEnter()
	{
	}

	public void HereDrag_OnPointerExit()
	{
	}

	private string GetTalkString(string strCharKey, string strKeywordKey)
	{
		string result = string.Empty;
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
		switch (strCharKey)
		{
		case "민주영":
			result = data_byKey.m_strTalkMin;
			break;
		case "서혜성":
			result = data_byKey.m_strTalkSeo;
			break;
		case "오인하":
			result = data_byKey.m_strTalkOh;
			break;
		case "이규혁":
			result = data_byKey.m_strTalkLee;
			break;
		case "장세일":
			result = data_byKey.m_strTalkChang;
			break;
		case "하수창":
			result = data_byKey.m_strTalkHa;
			break;
		}
		return result;
	}

	private int GetCalcIdxInputDir(GamePadInput.StickDir dir, bool isHelpWindowMove = false)
	{
		if (!isHelpWindowMove)
		{
			isHelpWindowMove = m_isShowHelpWindow;
		}
		int num = 0;
		if (isHelpWindowMove)
		{
			bool flag = true;
			num = m_iCurSelSlotIdx;
			int num2 = m_iLineMinIdx[1];
			int num3 = num2 - 4;
			int num4 = m_iSlotTotalPage * 4;
			if (dir == GamePadInput.StickDir.Right)
			{
				int num5 = num % num4 / 4;
				if (num == m_iLineMaxIdx[0])
				{
					flag = false;
					num = 0;
					if (m_iLineMinIdx[1] <= m_iLineMaxIdx[1])
					{
						int num6 = m_iLineMaxIdx[1] % num4 / 4;
						if (num5 <= num6)
						{
							num = m_iLineMinIdx[1] + num6 * 4;
						}
					}
				}
				else if (num == m_iLineMaxIdx[1])
				{
					flag = false;
					num = 0;
					int num7 = m_iLineMaxIdx[0] % num4 / 4;
					if (num5 < num7)
					{
						num = m_iLineMinIdx[0] + (num5 + 1) * 4;
					}
				}
				else if (num % 4 == 3)
				{
					num = ((num > m_iLineMaxIdx[0]) ? (num - num2) : (num + num3));
				}
			}
			else if (num == 0)
			{
				int num8 = m_iLineMaxIdx[0] % num4;
				int num9 = m_iLineMaxIdx[1] % num4;
				int num10 = num8 / 4;
				int num11 = num9 / 4;
				if (num10 > num11 || (num10 == num11 && m_iLineMinIdx[1] > m_iLineMaxIdx[1]))
				{
					num = m_iLineMaxIdx[0];
					flag = false;
				}
				else
				{
					num = m_iLineMaxIdx[1];
					flag = false;
				}
			}
			else if (num % 4 == 0)
			{
				num = ((num <= m_iLineMaxIdx[0]) ? (num + num2) : (num - num3));
			}
			if (flag)
			{
				int num12 = 0;
				switch (dir)
				{
				case GamePadInput.StickDir.Right:
					num12 = 1;
					break;
				case GamePadInput.StickDir.Left:
					num12 = -1;
					break;
				}
				num += num12;
			}
		}
		else
		{
			int num13 = 0;
			int num14 = m_iCurSelSlotIdx / m_iSlotCntPerLine;
			bool flag2 = false;
			int num15 = m_iCurSelSlotIdx / (8 * m_iSlotTotalPage);
			switch (dir)
			{
			case GamePadInput.StickDir.Right:
				num13 = 1;
				flag2 = true;
				break;
			case GamePadInput.StickDir.Left:
				num13 = -1;
				flag2 = true;
				break;
			case GamePadInput.StickDir.Up:
			case GamePadInput.StickDir.Down:
				num13 = ((num14 != 0) ? (-m_iSlotCntPerLine) : m_iSlotCntPerLine);
				break;
			}
			num = m_iCurSelSlotIdx + num13;
			if (flag2)
			{
				if (num < m_iLineMinIdx[num14])
				{
					num = m_iLineMaxIdx[num14];
				}
				if (num > m_iLineMaxIdx[num14])
				{
					num = m_iLineMinIdx[num14];
				}
				int num16 = num / m_iSlotCntPerLine;
				if (num14 != num16)
				{
					return -1;
				}
			}
		}
		return num;
	}

	private void PadInputDir(GamePadInput.StickDir dir)
	{
		if (m_isDrag || m_iSlotTotalPage == 0)
		{
			return;
		}
		if (m_eKeywordState == KeywordState.MEMO)
		{
			if (!m_isShowHelpWindow)
			{
			}
			return;
		}
		int iCurSelSlotIdx = m_iCurSelSlotIdx;
		int calcIdxInputDir = GetCalcIdxInputDir(dir);
		if (calcIdxInputDir == -1)
		{
			return;
		}
		if (m_eKeywordState == KeywordState.ARRANGE_MULTI || m_eKeywordState == KeywordState.ARRANGE_ONE)
		{
			SetCurMarkBySelKeyword(m_iCurSelSlotIdx, calcIdxInputDir);
		}
		if (iCurSelSlotIdx != calcIdxInputDir || dir == GamePadInput.StickDir.None)
		{
			SelSlot(calcIdxInputDir);
			if (iCurSelSlotIdx != m_iCurSelSlotIdx || dir == GamePadInput.StickDir.None)
			{
				AudioManager.instance.PlayUISound("KeyTalk_Selet");
				ActionBottomGuide(eBottomGuide.Cursor);
			}
		}
	}

	private KeywordSlotPlus GetSlotPlusByIdx(int iIdx)
	{
		KeywordSlotPlus result = null;
		int count = m_listKeySlot.Count;
		if (BitCalc.CheckArrayIdx(iIdx, count))
		{
			for (int i = 0; i < count; i++)
			{
				if (m_listKeySlot[i].transform.GetSiblingIndex() == iIdx)
				{
					result = m_listKeySlot[i];
					break;
				}
			}
		}
		return result;
	}

	private KeywordSlotPlus GetSlotPlusByKeywordKey(string strKeywordKey)
	{
		KeywordSlotPlus result = null;
		KeywordSlotPlus keywordSlotPlus = null;
		int count = m_listKeySlot.Count;
		for (int i = 0; i < count; i++)
		{
			keywordSlotPlus = m_listKeySlot[i];
			if (keywordSlotPlus.m_strKeywordKey == strKeywordKey)
			{
				result = keywordSlotPlus;
				break;
			}
		}
		return result;
	}

	private void SelSlot(int iSiblingIdx, bool isFirstSet = false, bool isDirectMove = false)
	{
		KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(iSiblingIdx);
		if (!(slotPlusByIdx != null))
		{
			return;
		}
		if (SelSlot(slotPlusByIdx, isFirstSet))
		{
			m_iCurSelSlotIdx = iSiblingIdx;
			int num = ((m_iCurSelSlotIdx >= m_iSlotCntPerLine) ? (m_iCurSelSlotIdx - m_iSlotCntPerLine) : m_iCurSelSlotIdx);
			int num2 = num / 4;
			if (m_iSlotCurPage != num2)
			{
				SetPageIdx(num2, isDirectMove);
			}
		}
		slotPlusByIdx = GetSlotPlusByIdx(m_iCurSelSlotIdx);
		if (m_eKeywordState == KeywordState.MEMO)
		{
			string text = null;
			int dataIndexInKeywordSlot = m_keywordContainer.GetDataIndexInKeywordSlot(slotPlusByIdx);
			Xls.CollKeyword collKeyword = m_keywordContainer.KeywordDatas[dataIndexInKeywordSlot];
			if (collKeyword != null)
			{
				text = collKeyword.m_strKey;
			}
			if (text != null)
			{
				m_subMemoMenu.SetKeywordKey(text, slotPlusByIdx.IsUsableSlot());
			}
		}
	}

	private bool SelSlot(KeywordSlotPlus goSlot = null, bool isFirstSet = false)
	{
		if (goSlot == null)
		{
			return false;
		}
		if (goSlot.Equals(m_SelKeywordSlot) && !isFirstSet)
		{
			OnClickSlot();
		}
		if (goSlot.IsEmptySlot())
		{
			return false;
		}
		if (goSlot != null && m_SelKeywordSlot != null)
		{
			if (m_SelKeywordSlot.m_strKeywordKey == null)
			{
				m_SelKeywordSlot.SetEmptySlot();
			}
			else
			{
				m_SelKeywordSlot.SelSlot(isSel: false);
			}
		}
		if (goSlot != null)
		{
			string text = null;
			if (m_eKeywordState == KeywordState.MEMO)
			{
				int dataIndexInKeywordSlot = m_keywordContainer.GetDataIndexInKeywordSlot(goSlot);
				Xls.CollKeyword collKeyword = m_keywordContainer.KeywordDatas[dataIndexInKeywordSlot];
				if (collKeyword != null)
				{
					text = collKeyword.m_strKey;
				}
				m_keywordContainer.SetOnCursorKeywordData(dataIndexInKeywordSlot);
			}
			else
			{
				text = goSlot.m_strKeywordKey;
			}
			bool flag = false;
			flag = ((m_eKeywordState != KeywordState.EVENT_KEYWORD) ? goSlot.IsUsableSlot() : (m_GameSwitch.GetKeywordAllState(text) >= 1));
			StartCoroutine(ShowKeywordSlotDetail(text, flag));
			if (m_eKeywordState == KeywordState.ARRANGE_MULTI)
			{
				bool isShow = true;
				int count = m_listUsingAns.Count;
				SelectAnswer selectAnswer = InitUsingSelList();
				if (m_iCurUsingIdx >= m_iKeywordUseSelCnt)
				{
					isShow = false;
					for (int i = 0; i < m_iCurUsingIdx; i++)
					{
						if (!m_isReUsingSelList)
						{
							if (i < count && m_listUsingAns[i].m_strKeywordKey == goSlot.m_strKeywordKey)
							{
								isShow = true;
							}
						}
						else if (selectAnswer.m_strArrUsingSelList[i] != null && selectAnswer.m_strArrUsingSelList[i] == goSlot.m_strKeywordKey)
						{
							isShow = true;
						}
					}
				}
				goSlot.SelSlot(isSel: true, isSetShow: true, isShow);
			}
			else if (m_eKeywordState == KeywordState.ARRANGE_ONE)
			{
				goSlot.SelSlot(isSel: true, isSetShow: true, isShow: true);
			}
			else if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
			{
				goSlot.SelSlot(isSel: true, isSetShow: false, isShow: false, isSetEmptySlot: true, !flag);
			}
			else
			{
				goSlot.SelSlot(isSel: true);
			}
			if (m_eKeywordState == KeywordState.MEMO)
			{
				sbyte keywordAllState = m_GameSwitch.GetKeywordAllState(text);
				m_GameSwitch.SetKeywordAllState(text, 2);
				goSlot.SetNewMark(isOn: false);
				goSlot.isEnableNewMark = false;
				int tabIdx = GetTabIdx(m_iCurCategory);
				int count2 = m_TabContainer.tabButtonInfos.Count;
				if (keywordAllState == 1 && tabIdx < count2)
				{
					CommonTabContainerPlus.TabButtonInfo tabButtonInfo = m_TabContainer.tabButtonInfos[tabIdx];
					int key = (int)tabButtonInfo.tag;
					if (m_CategoryInfos.ContainsKey(key) && --m_CategoryInfos[key].m_NewContentCount <= 0)
					{
						tabButtonInfo.tabButtonComp.SetVisibleNewSymbol(visible: false);
					}
				}
			}
			else if (flag)
			{
				m_GameSwitch.SetKeywordSeqState(m_iCurSequence, text, 2);
				goSlot.SetNewMark(isOn: false);
			}
			bool active = false;
			if (m_eKeywordState == KeywordState.TALK)
			{
				if (text != null)
				{
					goSlot.m_goShowButton.SetActive(value: true);
					string talkString = GetTalkString(m_strCharKey, text);
					bool flag2 = !talkString.Equals(string.Empty);
					m_goTalkBalloon.SetActive(flag2);
					string xlsTextData = GameGlobalUtil.GetXlsTextData(talkString);
					if (flag2 && xlsTextData != null)
					{
						m_textTalkBalloon.text = xlsTextData;
					}
					active = true;
					m_imgCondition.sprite = IconCondition.instance.GetConditionSprite(m_strCharKey);
					m_textCondition.text = GameGlobalUtil.GetXlsProgramText("KEYWORD_CONDITION");
				}
			}
			else if (m_eKeywordState != KeywordState.ARRANGE_MULTI && m_eKeywordState != KeywordState.ARRANGE_ONE)
			{
				goSlot.m_goShowButton.SetActive(value: false);
			}
			m_goCondition.SetActive(active);
			if (m_eKeywordState == KeywordState.TALK)
			{
				string charKeywordMot = GetCharKeywordMot(m_strCharKey, text);
				m_EventEngine.m_TalkChar.ChangeCharMot(m_strCharKey, charKeywordMot, m_GameSwitch.GetCharPartyDir(m_strCharKey), isSaveCurMot: false);
			}
		}
		m_SelKeywordSlot = goSlot;
		m_iCurSelSlotIdx = goSlot.gameObject.transform.GetSiblingIndex();
		if (m_eKeywordState == KeywordState.MEMO)
		{
			string strKeywordKey = null;
			int dataIndexInKeywordSlot2 = m_keywordContainer.GetDataIndexInKeywordSlot(goSlot);
			Xls.CollKeyword collKeyword2 = m_keywordContainer.KeywordDatas[dataIndexInKeywordSlot2];
			if (collKeyword2 != null)
			{
				strKeywordKey = collKeyword2.m_strKey;
			}
			m_subMemoMenu.SetKeywordKey(strKeywordKey, goSlot.IsUsableSlot());
		}
		return true;
	}

	private string GetCharKeywordMot(string strCharKey, string strKeywordKey)
	{
		string text = string.Empty;
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
		if (data_byKey != null)
		{
			switch (strCharKey)
			{
			case "민주영":
				text = data_byKey.m_strFMotionMin;
				break;
			case "서혜성":
				text = data_byKey.m_strFMotionSeo;
				break;
			case "오인하":
				text = data_byKey.m_strFMotionOh;
				break;
			case "이규혁":
				text = data_byKey.m_strFMotionLee;
				break;
			case "장세일":
				text = data_byKey.m_strFMotionChang;
				break;
			case "하수창":
				text = data_byKey.m_strFMotionHa;
				break;
			}
		}
		if (text == string.Empty)
		{
			text = m_GameSwitch.GetCharPartyMotion(strCharKey);
		}
		return text;
	}

	private void ClickShowButton(KeywordSlotPlus goSlot)
	{
		if (!(goSlot == null) && m_SelKeywordSlot == goSlot)
		{
			RunEvent(goSlot);
		}
	}

	private void RunEvent(KeywordSlotPlus goSlot)
	{
		bool flag = m_GameSwitch.IsCheckRunLowRelationEvt(goSlot.m_strKeywordKey, m_strCharKey);
		m_GameSwitch.SetRunLowRealtionEvt(flag);
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("KEYWORD_USE_LOW_RELATION");
		string text = ((!flag || xlsProgramDefineStr == null) ? goSlot.m_strKeywordKey : xlsProgramDefineStr);
		text = text + "_" + m_strCharKey;
		if (ContiDataHandler.IsExistConti(text))
		{
			m_GameSwitch.SetRunKeyword(text);
		}
		else
		{
			string xlsProgramDefineStr2 = GameGlobalUtil.GetXlsProgramDefineStr("KEYWORD_NOUSE_PRE_NAME");
			if (xlsProgramDefineStr2 != null)
			{
				m_GameSwitch.SetRunKeyword(xlsProgramDefineStr2 + m_strCharKey);
			}
		}
		m_isEndTypeRunEvent = true;
		m_GameSwitch.SetRunKeywordEvt(isRun: true);
		m_strRunKeywordKey = goSlot.m_strKeywordKey;
		m_iRunKeywordPage = m_iSlotCurPage;
		OnClickBackToGame(isRunEvent: true);
		m_GameSwitch.SetRunKeywordGameOver(isRun: false);
		m_EventEngine.m_TalkChar.SetDefMotion(m_strCharKey, m_GameSwitch.GetCharPartyMotion(m_strCharKey), m_GameSwitch.GetCharPartyDir(m_strCharKey));
		int siblingIndex = m_SelKeywordSlot.transform.GetSiblingIndex();
		bool flag2 = false;
		bool flag3 = siblingIndex <= m_iLineMaxIdx[0];
		int num = m_iSlotTotalPage - 1;
		if (m_iSlotCurPage == num)
		{
			if (flag3)
			{
				int num2 = (m_iLineMaxIdx[1] - m_iLineMinIdx[1]) / 4;
				if (num2 < num)
				{
					flag2 = siblingIndex == m_iLineMaxIdx[0];
				}
			}
			else
			{
				flag2 = siblingIndex == m_iMaxSlotCnt - 1;
			}
		}
		int iIdx = ((!flag2) ? GetCalcIdxInputDir(GamePadInput.StickDir.Right, isHelpWindowMove: true) : GetCalcIdxInputDir(GamePadInput.StickDir.Left, isHelpWindowMove: true));
		KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(iIdx);
		m_strSelKeywordKey = slotPlusByIdx.m_strKeywordKey;
		m_eBefRunKeywordState = m_eKeywordState;
		m_SelKeywordSlot = null;
	}

	private string GetTextList(string strKey, bool isTitle)
	{
		string result = null;
		Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(strKey);
		if (data_byKey != null)
		{
			result = ((!isTitle) ? data_byKey.m_strText : data_byKey.m_strTitle);
		}
		return result;
	}

	private IEnumerator ShowKeywordSlotDetail(string strKeywordKey, bool isUsable)
	{
		m_goKeyDetailGroup.SetActive(value: true);
		string strKeywordTitle = null;
		string strKeywordBody = null;
		string strImgID = null;
		Xls.CollKeyword collKey = null;
		bool isShowFaceIcon = false;
		if (strKeywordKey == null || !isUsable)
		{
			if (m_eKeywordState == KeywordState.EVENT_KEYWORD)
			{
				collKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
				_ = string.Empty;
				if (collKey != null)
				{
					string strRequiredHint = collKey.m_strRequiredHint;
					strKeywordTitle = GetTextList(strRequiredHint, isTitle: true);
					string strRequiredkw = collKey.m_strRequiredkw;
					bool flag = false;
					if (!string.IsNullOrEmpty(strRequiredkw))
					{
						if (m_GameSwitch.GetKeywordSeqState(m_iCurSequence, strRequiredkw) >= 1)
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					strKeywordBody = ((!flag) ? GameGlobalUtil.GetXlsProgramText("KEYWORD_EVENT_FIND_CLUE") : GetTextList(strRequiredHint, isTitle: false));
				}
			}
			else
			{
				strKeywordTitle = GameGlobalUtil.GetXlsProgramText("NONE_KEYWORD_DETAIL_TITLE");
				strKeywordBody = GameGlobalUtil.GetXlsProgramText("NONE_KEYWORD_DETAIL_BODY");
			}
		}
		else
		{
			collKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
			if (collKey != null)
			{
				strKeywordTitle = GetTextList(collKey.m_strTitleID, isTitle: true);
				strKeywordBody = GetTextList(collKey.m_strTitleID, isTitle: false);
				if (!collKey.m_strImgID.Equals(string.Empty))
				{
					strImgID = collKey.m_strImgID;
				}
			}
			if (m_eKeywordState == KeywordState.TALK)
			{
				if (m_GameSwitch.GetCollSelKeyword(strKeywordKey) == 1)
				{
					isShowFaceIcon = true;
				}
				else if (m_GameSwitch.GetCollKeywordUse(m_strCharKey, strKeywordKey) != 0)
				{
					isShowFaceIcon = true;
				}
			}
		}
		bool isShowLogIcon = false;
		if (m_eKeywordState == KeywordState.MEMO || m_eKeywordState == KeywordState.TALK)
		{
			collKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
			if (collKey != null)
			{
				isShowLogIcon = collKey.m_iCtg == 1;
			}
		}
		m_goLogIcon.SetActive(isShowLogIcon);
		bool isCutKeywordOn = false;
		if (m_eKeywordState != KeywordState.ARRANGE_MULTI && m_eKeywordState != KeywordState.ARRANGE_ONE)
		{
			isCutKeywordOn = m_GameSwitch.GetCollCutKeyword(m_strCharKey, strKeywordKey) == 1;
		}
		int iCharCnt = ConstGameSwitch.COUNT_KEYWORD_USE_CHAR_CNT;
		m_goCutKeywordMark.SetActive(isCutKeywordOn);
		if (!m_isShowHelpWindow)
		{
			OnBottomGuide(eBottomGuide.KeywordRecord, isShowLogIcon);
		}
		m_imgDetailFace.gameObject.SetActive(value: false);
		m_imgDetailMental.gameObject.SetActive(isShowFaceIcon);
		if (isShowFaceIcon)
		{
			byte b = (byte)((m_GameSwitch.GetCollSelKeyword(strKeywordKey) != 1) ? m_GameSwitch.GetCollKeywordUse(m_strCharKey, strKeywordKey) : 4);
			if (b != 0 && b != 0)
			{
				m_imgDetailMental.sprite = m_sprMental[b - 1];
			}
		}
		if (strKeywordTitle != null)
		{
			m_textKeyTitle.gameObject.SetActive(value: true);
			m_textKeyTitle.text = strKeywordTitle;
		}
		bool isImgIDExist = strImgID != null;
		m_goKeyOnlyText.SetActive(!isImgIDExist);
		m_goKeyImgGroup.SetActive(isImgIDExist);
		if (strKeywordBody != null)
		{
			Text textKeyOnlyText = m_textKeyOnlyText;
			string text = strKeywordBody;
			m_textKeyImgText.text = text;
			textKeyOnlyText.text = text;
		}
		if (!m_isShowHelpWindow)
		{
			OnBottomGuide(eBottomGuide.ShowImage, isImgIDExist);
		}
		if (m_eKeywordState == KeywordState.MEMO)
		{
			m_goKeyCategory.SetActive(value: false);
		}
		m_isCurSlotShowImg = isImgIDExist;
		if (isImgIDExist && collKey != null)
		{
			yield return StartCoroutine(GetDetailImageRes(collKey.m_strImgID));
			clDetailImgRes detailRes = m_clDetailImgRes;
			if (detailRes != null)
			{
				m_imgKeyImage.sprite = detailRes.m_sprDetail;
			}
		}
	}

	private void HideKeywordSlotDetail()
	{
		m_goLogIcon.SetActive(value: false);
		if (m_imgDetailFace != null)
		{
			m_imgDetailFace.gameObject.SetActive(value: false);
		}
		m_imgDetailMental.gameObject.SetActive(value: false);
		m_goCutKeywordMark.SetActive(value: false);
		m_textKeyTitle.gameObject.SetActive(value: false);
		m_goKeyOnlyText.SetActive(value: false);
		m_goKeyImgGroup.SetActive(value: false);
	}

	private void OnProc_ClosedViewImageDetail(object sender, object arg)
	{
		OnBottomCloseHelp(isShow: false);
		m_TabContainer.isInputBlock = false;
		m_isKeyLock = false;
	}

	public IEnumerator OnClickDetailImage(bool isShow)
	{
		m_TabContainer.isInputBlock = true;
		m_isKeyLock = true;
		string strKeywordKey = null;
		if (m_eKeywordState == KeywordState.MEMO)
		{
			strKeywordKey = m_strMemoSelKeywordKey;
		}
		else
		{
			KeywordSlotPlus slotPlusByIdx = GetSlotPlusByIdx(m_iCurSelSlotIdx);
			if (slotPlusByIdx == null)
			{
				goto IL_02a3;
			}
			strKeywordKey = slotPlusByIdx.m_strKeywordKey;
		}
		Xls.CollKeyword collKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
		if (collKey != null)
		{
			Xls.CollImages xlsImageData = Xls.CollImages.GetData_byKey(collKey.m_strImgID);
			if (xlsImageData != null && m_imgKeyImage.sprite != null)
			{
				ImageDetailViewer imageDetailViewer = MainLoadThing.instance.imageDetailViewer;
				if (!(imageDetailViewer == null))
				{
					imageDetailViewer.SetCanvasOrder(9999);
					if (!string.IsNullOrEmpty(xlsImageData.m_strIDColImageDest))
					{
						imageDetailViewer.ShowImage(m_imgKeyImage.sprite, xlsImageData.m_strIDColImageDest, OnProc_ClosedViewImageDetail);
						goto IL_0292;
					}
					if (!(m_ShowImageOriginSize == null))
					{
						Xls.ImageFile xlsImageFile = Xls.ImageFile.GetData_byKey(collKey.m_strImgID);
						if (xlsImageFile != null)
						{
							m_showingImageAssetObjHdr = new AssetBundleObjectHandler(xlsImageFile.m_strAssetPath);
							yield return StartCoroutine(m_showingImageAssetObjHdr.LoadAssetBundle());
							Sprite spr = m_showingImageAssetObjHdr.GetLoadedAsset_ToSprite();
							if (!(spr == null))
							{
								m_ShowImageOriginSize.gameObject.SetActive(value: true);
								m_ShowImageOriginSize.ShowImage(isShow: true, spr, OnProc_ClosedViewImageDetail);
								goto IL_0292;
							}
							OnProc_ClosedViewImageDetail(null, null);
						}
					}
				}
			}
		}
		goto IL_02a3;
		IL_0292:
		OnBottomCloseHelp(isShow: true);
		yield break;
		IL_02a3:
		m_TabContainer.isInputBlock = false;
		m_isKeyLock = false;
	}

	public void OnClickArrow(bool isRight)
	{
		DragSlotGroup(isRight);
	}

	private void SetMemoSlot()
	{
	}

	private void SetDrag(bool isOn)
	{
		if (!isOn || m_eKeywordState != KeywordState.MEMO)
		{
			m_isDrag = isOn;
		}
	}

	private void SetPageIdx(int iIdx, bool isDirectMove = false)
	{
		if (!m_isDrag && m_iSlotCurPage != iIdx)
		{
			int num = iIdx - m_iSlotCurPage;
			m_fDestContentX -= (float)num * m_fContentW;
			MovePage(iIdx);
			SetCurPageIdx(iIdx);
			if (!isDirectMove)
			{
				SetDrag(isOn: true);
				m_fPassMoveT = 0f;
			}
			else
			{
				SetDrag(isOn: false);
				m_rtfKeywordGroupContent.anchoredPosition = new Vector2(m_fDestContentX, m_rtfKeywordGroupContent.anchoredPosition.y);
			}
		}
	}

	private void SetCurPageIdx(int iIdx, bool isIgnoreSame = true)
	{
		if (m_iSlotCurPage != iIdx || !isIgnoreSame)
		{
			m_iSlotCurPage = iIdx;
		}
	}

	private void DragSlotGroup(bool isLeft, bool isPlaySound = false)
	{
		if (m_isDrag || m_iSlotTotalPage <= 1)
		{
			return;
		}
		if (m_eKeywordState == KeywordState.MEMO)
		{
			m_keywordContainer.StartPageScroll(isLeft ? KeywordContainer.SlotPage.Next : KeywordContainer.SlotPage.Prev, isOnCursorAtFirstSlot: true);
			return;
		}
		if (isPlaySound)
		{
			AudioManager.instance.PlayUISound("Scroll_Page");
		}
		if (isLeft)
		{
			if (m_iSlotCurPage < m_iSlotTotalPage - 1)
			{
				m_fDestContentX -= m_fContentW;
				m_iSlotCurPage++;
			}
			else
			{
				m_fDestContentX = m_fFirstContentX;
				SetCurPageIdx(0);
			}
		}
		else if (m_iSlotCurPage > 0)
		{
			m_fDestContentX += m_fContentW;
			m_iSlotCurPage--;
		}
		else
		{
			int num = m_iSlotTotalPage - 1;
			m_fDestContentX = m_fFirstContentX - m_fContentW * (float)num;
			SetCurPageIdx(num);
		}
		SetDrag(isOn: true);
		MovePage(m_iSlotCurPage);
		m_fPassMoveT = 0f;
		SelSlot(m_iSlotCurPage * 4);
	}

	public void ProcDragSlotGroup()
	{
		if (m_isDrag)
		{
			float x = m_rtfKeywordGroupContent.anchoredPosition.x;
			float y = m_rtfKeywordGroupContent.anchoredPosition.y;
			Vector2 vector = new Vector2(m_fDestContentX, y);
			m_fPassMoveT += Time.deltaTime;
			if (m_fPassMoveT < m_fTotalMoveT)
			{
				m_rtfKeywordGroupContent.anchoredPosition = Vector2.Lerp(m_rtfKeywordGroupContent.anchoredPosition, vector, m_fPassMoveT / m_fTotalMoveT);
				return;
			}
			m_rtfKeywordGroupContent.anchoredPosition = vector;
			SetDrag(isOn: false);
		}
	}

	private void OnProc_OnCursorChanged(object sender, object args)
	{
		if (!(args is KeywordContainer.EventArg_OnCursorChanged { m_curOnCursorDataIndex: >=0 } eventArg_OnCursorChanged) || eventArg_OnCursorChanged.m_curOnCursorDataIndex >= m_validXlsDatas.Count)
		{
			return;
		}
		Xls.CollKeyword collKeyword = m_validXlsDatas[eventArg_OnCursorChanged.m_curOnCursorDataIndex];
		KeywordSlotPlus keywordSlotPlus = (m_SelKeywordSlot = eventArg_OnCursorChanged.m_curOnCursorSlot);
		keywordSlotPlus.SetNewMark(isOn: false);
		keywordSlotPlus.isEnableNewMark = false;
		sbyte keywordAllState = m_GameSwitch.GetKeywordAllState(collKeyword.m_strKey);
		if (keywordAllState == 1)
		{
			CommonTabContainerPlus.TabButtonInfo selectedTabInfo = m_TabContainer.selectedTabInfo;
			if (m_CategoryInfos.ContainsKey(m_iCurCategory) && --m_CategoryInfos[m_iCurCategory].m_NewContentCount <= 0)
			{
				selectedTabInfo.tabButtonComp.SetVisibleNewSymbol(visible: false);
			}
		}
		if (collKeyword != null)
		{
			m_strMemoSelKeywordKey = collKeyword.m_strKey;
			m_GameSwitch.SetKeywordAllState(m_strMemoSelKeywordKey, 2);
			bool flag = m_GameSwitch.GetKeywordAllState(collKeyword.m_strKey) >= 1;
			m_MemoSelCursor = eventArg_OnCursorChanged;
			StartCoroutine(ShowKeywordSlotDetail(collKeyword.m_strKey, flag));
			m_subMemoMenu.SetKeywordKey(m_strMemoSelKeywordKey, flag);
		}
	}

	private void OnProc_ChangedCurrentPage(object sender, object args)
	{
		if (args is KeywordContainer.EventArg_ChangedCurrentPage eventArg_ChangedCurrentPage)
		{
			MovePage(eventArg_ChangedCurrentPage.m_currentPageIndex);
		}
	}

	private void OnProc_KeyInputedScrollPage(object sender, object args)
	{
	}

	private void OnProc_KeyInputedMoveCursor(object sender, object args)
	{
		ActionBottomGuide(eBottomGuide.Cursor);
	}

	public void SetCrisisLevel(int iLevel)
	{
		if (iLevel >= 0 && iLevel <= 3)
		{
			m_iCrisisLevel = iLevel;
		}
	}

	public int GetTabIdx(int iIdx)
	{
		int result = -1;
		int count = m_iListTabOrder.Count;
		for (int i = 0; i < count; i++)
		{
			if (iIdx == m_iListTabOrder[i])
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public static void SetIsShowBackButton(bool isShow)
	{
		m_isShowBackButton = isShow;
	}

	public static bool GetIsShowBackButton()
	{
		return m_isShowBackButton;
	}

	public static void SetCharKey(int iSwitchIdx)
	{
		if (iSwitchIdx != -1)
		{
			m_strCharKey = Xls.CharData.GetData_bySwitchIdx(iSwitchIdx).m_strKey;
		}
		else
		{
			m_strCharKey = null;
		}
	}

	public static int GetCharKeyIdx()
	{
		int result = -1;
		if (m_strCharKey != null)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_strCharKey);
			if (data_byKey != null)
			{
				result = data_byKey.m_iIdx;
			}
		}
		return result;
	}

	public static string GetCharKey()
	{
		return m_strCharKey;
	}

	public static void SetRunKeywordPage(int iVal)
	{
		m_iRunKeywordPage = iVal;
	}

	public static int GetRunKeywordPage()
	{
		return m_iRunKeywordPage;
	}

	public void TouchBackButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			PressBackButton();
		}
	}

	public void OnClickLeftRightHelpMenu(bool isLeft)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			PadInputDir((!isLeft) ? GamePadInput.StickDir.Right : GamePadInput.StickDir.Left);
			AudioManager.instance.PlayUISound("Menu_Select");
			SetHelpWindow(isShow: true);
		}
	}

	public void TouchAllDeselect()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AllDeselect();
		}
	}

	public void TouchCloseHWWindow()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClickCloseHWWindow();
		}
	}

	public void TouchShowHelpWindow()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			SetHelpWindow(isShow: true);
		}
	}

	public void TouchDetailImage(bool isShow)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			if (isShow)
			{
				AudioManager.instance.PlayUISound("Menu_Detail");
			}
			StartCoroutine(OnClickDetailImage(isShow));
		}
	}

	public void ClickKeywordUsingComp()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Push_SubmissBTN");
			CompKeywordUsing();
		}
	}

	public void TouchRecordButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClickRecordButton();
		}
	}

	public void TouchArrow(bool isRight)
	{
		OnClickArrow(isRight);
	}
}
