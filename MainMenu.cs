using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : CommonBGChildBase
{
	public enum Mode
	{
		Normal,
		Load,
		SoundConfig,
		GameConfig
	}

	public enum Items
	{
		Continue,
		LoadGame,
		NewGame,
		NewGame2,
		Collection,
		SoundConfig,
		GameConfig,
		QuitGame
	}

	[Serializable]
	public class TitleMembers
	{
		public GameObject m_TitleRoot;

		public Text m_TitleText1;

		public Text m_TitleText2;
	}

	private class ModeLinkedInfo
	{
		public UnityEngine.Object m_prefabAssetObject;

		public SystemMenuBase m_linkedMenu;
	}

	public delegate void fpCBClose();

	public delegate void fpCBCloseArg(bool isArg);

	public Canvas m_ContentCanvas;

	public GameObject m_ContentRootObj;

	public GameObject m_ExitButtonRoot;

	public Button m_ExitPadButton;

	public Text m_ExitText;

	[Header("Title")]
	public TitleMembers m_MainMenuTitle = new TitleMembers();

	public TitleMembers m_LoadGameTitle = new TitleMembers();

	public TitleMembers m_SoundConfigTitle = new TitleMembers();

	public TitleMembers m_GameConfigTitle = new TitleMembers();

	public TitleMembers m_CrossSaveTitle1 = new TitleMembers();

	public TitleMembers m_CrossSaveTitle2 = new TitleMembers();

	private TitleMembers[] m_TitleMembers;

	public Text m_TotalPlayTime;

	[Header("Items")]
	public MainMenuItem m_ContinueItem;

	public MainMenuItem m_LoadGameItem;

	public MainMenuItem m_NewGameItem;

	public MainMenuItem m_NewGameItem2;

	public MainMenuItem m_CollectionItem;

	public MainMenuItem m_SoundConfigItem;

	public MainMenuItem m_GameConfigItem;

	public MainMenuItem m_QuitGameItem;

	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public int m_ItemInterval;

	public float m_RightMargin = 18f;

	[Header("Scroll Members")]
	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	public RectTransform m_ScrollNodeRT;

	public GameObject m_ScrollNodeSrcObj;

	public Sprite m_SelectNodeImage;

	public Sprite m_NotSelectNodeImage;

	[Header("Scroll Buttons")]
	public GameObject m_ScrollButtonRoot;

	public GameObject m_ScrollButton_Left;

	public GameObject m_ScrollButton_Right;

	[Header("Mode Linked Prefabs")]
	public UnityEngine.Object m_LoadModePrefab;

	public UnityEngine.Object m_SoundConfigModePrefab;

	public UnityEngine.Object m_GameConfigModePrefab;

	[Header("Touch Back Button")]
	public GameObject m_goBackButton;

	private MainMenuItem[] m_Items;

	private List<MainMenuItem> m_validItems = new List<MainMenuItem>();

	private MainMenuItem m_OnCursorItem;

	private List<Image> m_ScrollNodeImages = new List<Image>();

	private int m_curSelectedScrollNodeIdx = -1;

	private Mode m_curMode;

	private Mode m_nextMode;

	private bool m_isWaitingForNextMode;

	private Animator m_disapperCheck_NormalMode;

	private bool m_isInputBlock;

	private bool m_isNeedAlign;

	private bool m_isEnable2ndGameStart;

	private Animator m_CloseAnimator;

	private AudioManager m_AudioManager;

	private int m_firstSelectItem = -1;

	private static float s_backupedScrollPos_forCollectionMain;

	private GameObject m_goTalkWindow;

	private GameObject m_goBackLogMenu;

	private ModeLinkedInfo m_ModeInfo_Load = new ModeLinkedInfo();

	private ModeLinkedInfo m_ModeInfo_SoundConfig = new ModeLinkedInfo();

	private ModeLinkedInfo m_ModeInfo_GameConfig = new ModeLinkedInfo();

	private int m_lastestSaveSlotIdx = -1;

	private int m_loadableSaveSlotIdx = -1;

	private int m_testTrophyIdx = 1;

	private fpCBClose m_fpCBClose;

	private fpCBCloseArg m_fpCBCloseArg;

	private const string cstrUITalkWindowPath = "Prefabs/InGame/Game/UI_Event_Talk_Window";

	private const string cstrBackLogMenuPath = "Prefabs/InGame/Menu/UI_BackLogMenu";

	private Animator m_Animator_ScrollBtnLeft;

	private Animator m_Animator_ScrollBtnRight;

	private GameSwitch.eUIButType m_prevUIButtonType;

	private GameObject m_pointEventObject;

	private bool m_isPusingScrollButton;

	private bool m_isOverTimePushingBound;

	private const float c_pusingBoundTime = 0.3f;

	private float m_pusingTime;

	public int firstSeletItem
	{
		set
		{
			m_firstSelectItem = value;
		}
	}

	public int lastestSaveSlotIdx => m_lastestSaveSlotIdx;

	public int loadableSaveSlotIdx => m_loadableSaveSlotIdx;

	private void Awake()
	{
		int length = Enum.GetValues(typeof(Mode)).Length;
		m_TitleMembers = new TitleMembers[length];
		m_TitleMembers[0] = m_MainMenuTitle;
		m_TitleMembers[1] = m_LoadGameTitle;
		m_TitleMembers[2] = m_SoundConfigTitle;
		m_TitleMembers[3] = m_GameConfigTitle;
		int length2 = Enum.GetValues(typeof(Items)).Length;
		m_Items = new MainMenuItem[length2];
		m_Items[0] = m_ContinueItem;
		m_Items[1] = m_LoadGameItem;
		m_Items[2] = m_NewGameItem;
		m_Items[3] = m_NewGameItem2;
		m_Items[4] = m_CollectionItem;
		m_Items[5] = m_SoundConfigItem;
		m_Items[6] = m_GameConfigItem;
		m_Items[7] = m_QuitGameItem;
		m_ModeInfo_Load.m_prefabAssetObject = m_LoadModePrefab;
		m_ModeInfo_SoundConfig.m_prefabAssetObject = m_SoundConfigModePrefab;
		m_ModeInfo_GameConfig.m_prefabAssetObject = m_GameConfigModePrefab;
		if (m_ScrollNodeSrcObj != null)
		{
			m_ScrollNodeSrcObj.SetActive(value: false);
		}
		if (m_ScrollButtonRoot != null)
		{
			m_ScrollButtonRoot.SetActive(value: true);
		}
		if (m_ScrollButton_Left != null)
		{
			m_ScrollButton_Left.SetActive(value: false);
		}
		if (m_ScrollButton_Right != null)
		{
			m_ScrollButton_Right.SetActive(value: false);
		}
	}

	private IEnumerator Start()
	{
		GameObject gameObject = (m_goTalkWindow = UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabTalkWindow) as GameObject);
		gameObject.name = "UI_Event_Talk_Window";
		gameObject.SetActive(value: true);
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = gameObject.transform.GetChild(i);
			string text = child.name;
			if (text == "MainTalkWindow")
			{
				child = gameObject.transform.GetChild(i);
				child.gameObject.SetActive(value: true);
			}
			else if (text == "ConversationSign")
			{
				child.GetComponent<ConversationSign>().gameObject.SetActive(value: false);
			}
		}
		gameObject = (m_goBackLogMenu = UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabBackLogMenu) as GameObject);
		gameObject.name = "UI_BackLogMenu";
		gameObject.SetActive(value: false);
		gameObject.GetComponent<BackLogMenuPlus>().SetInstance();
		yield return null;
	}

	private void OnEnable()
	{
		if (m_goBackButton != null)
		{
			m_goBackButton.SetActive(value: false);
		}
	}

	private void OnDestory()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_AudioManager = null;
		m_goTalkWindow = null;
		m_goBackLogMenu = null;
		m_disapperCheck_NormalMode = null;
		m_CloseAnimator = null;
		m_Items = null;
		if (m_validItems != null)
		{
			m_validItems.Clear();
		}
		m_OnCursorItem = null;
		if (m_ScrollNodeImages != null)
		{
			m_ScrollNodeImages.Clear();
		}
		if (MainMenuCommon.instance != null && MainMenuCommon.instance.m_EventPrefabObj != null)
		{
			MainMenuCommon.instance.m_EventPrefabObj.SetActive(value: false);
		}
		if (m_goTalkWindow != null)
		{
			UnityEngine.Object.Destroy(m_goTalkWindow);
		}
		if (m_goBackLogMenu != null)
		{
			UnityEngine.Object.Destroy(m_goBackLogMenu);
		}
	}

	public static void UnloadAssetBundles()
	{
		AssetBundleManager.UnloadAssetBundle("Prefabs/InGame/Menu/UI_BackLogMenu");
		AssetBundleManager.UnloadAssetBundle("Prefabs/InGame/Game/UI_Event_Talk_Window");
	}

	private void Update()
	{
		if (m_CloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		if (m_isWaitingForNextMode)
		{
			if ((bool)m_disapperCheck_NormalMode)
			{
				AnimatorStateInfo currentAnimatorStateInfo2 = m_disapperCheck_NormalMode.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo2.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo2.normalizedTime >= 0.99f)
				{
					ActiveNextMode();
					m_disapperCheck_NormalMode = null;
					m_ContentRootObj.SetActive(value: false);
				}
			}
			return;
		}
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			bool flag = true;
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorItem != null)
			{
				ChangeOnCursorItem_byScrollPos();
				Vector2 zero = Vector2.zero;
				if (GamePadInput.GetLStickMove(out zero.x, out zero.y) || GamePadInput.GetLStickMove(out zero.x, out zero.y))
				{
					m_ScrollRect.velocity = Vector2.zero;
					flag = false;
				}
			}
			if (flag)
			{
				return;
			}
		}
		if (Update_SteamSpec() || m_curMode != Mode.Normal || m_isInputBlock || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
		{
			ChangeOnCursorItem(isLeftSide: true);
			m_fScrollButtonPusingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
		{
			ChangeOnCursorItem(isLeftSide: false);
			m_fScrollButtonPusingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
		{
			m_fScrollButtonPusingTime += Time.deltaTime;
			if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
			{
				m_fScrollButtonPusingTime = 0f;
				ChangeOnCursorItem(isLeftSide: true);
			}
		}
		else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing))
		{
			m_fScrollButtonPusingTime += Time.deltaTime;
			if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
			{
				m_fScrollButtonPusingTime = 0f;
				ChangeOnCursorItem(isLeftSide: false);
			}
		}
		if (m_OnCursorItem != null && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			m_isInputBlock = true;
			m_OnCursorItem.PlayPushAnimation(OnProc_ItemSelect);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
		}
	}

	private void LateUpdate()
	{
		if (m_isNeedAlign)
		{
			AlignItems();
		}
	}

	public void Show(int firstSelectItem = -1, bool isPlayBGM = true)
	{
		EventCameraEffect.Instance.Clear();
		GC.Collect();
		m_OnCursorItem = null;
		m_firstSelectItem = firstSelectItem;
		m_isInputBlock = true;
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Horizontal, m_ScrollRect, m_ContentContanierRT);
		if (MainMenuCommon.instance != null && MainMenuCommon.instance.m_EventPrefabObj != null)
		{
			MainMenuCommon.instance.m_EventPrefabObj.SetActive(value: true);
		}
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_AudioManager != null && isPlayBGM)
		{
			m_AudioManager.PlayKey(0, "컷", isSetVol: true, 0f, 1f, isLoop: true);
		}
		CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		if (commonButtonGuide != null)
		{
			commonButtonGuide.SetShow(isShow: false);
		}
		if (m_ExitButtonRoot != null)
		{
			m_ExitButtonRoot.SetActive(value: false);
		}
		m_isEnable2ndGameStart = GameSwitch.GetInstance().GetShowEnding();
		SetXmlTexts();
		TitleMembers[] titleMembers = m_TitleMembers;
		foreach (TitleMembers titleMembers2 in titleMembers)
		{
			titleMembers2.m_TitleRoot.gameObject.SetActive(value: false);
		}
		MainMenuItem[] items = m_Items;
		foreach (MainMenuItem mainMenuItem in items)
		{
			if (!(mainMenuItem == null))
			{
				mainMenuItem.gameObject.SetActive(value: false);
			}
		}
		m_MainMenuTitle.m_TitleRoot.gameObject.SetActive(value: true);
		CheckSaveData();
		m_isNeedAlign = true;
	}

	public void CheckSaveData()
	{
		if (m_TotalPlayTime != null)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			float totalGamePlayTime = instance.GetTotalGamePlayTime();
			int num = (int)(totalGamePlayTime / 3600f);
			totalGamePlayTime -= (float)num * 3600f;
			int num2 = (int)(totalGamePlayTime / 60f);
			totalGamePlayTime -= (float)num2 * 60f;
			int num3 = (int)totalGamePlayTime;
			m_TotalPlayTime.text = $"{num:D2}:{num2:D2}:{num3:D2}";
		}
		m_lastestSaveSlotIdx = -1;
		int num4 = -1;
		SaveLoad.cSaveSlotInfo cSaveSlotInfo = null;
		SaveLoad.cSaveSlotInfo cSaveSlotInfo2 = null;
		int num5 = 16;
		SaveLoad instance2 = SaveLoad.GetInstance();
		for (int i = 0; i < num5; i++)
		{
			uint iReturnValue = 0u;
			if (!instance2.ExistEventSaveFile(SaveLoad.eSaveType.GAME, ref iReturnValue, i))
			{
				continue;
			}
			cSaveSlotInfo2 = instance2.GetSlotInfo(i);
			if (cSaveSlotInfo2.m_isSave)
			{
				if (num4 < 0 || cSaveSlotInfo == null)
				{
					num4 = i;
					cSaveSlotInfo = cSaveSlotInfo2;
				}
				else if (cSaveSlotInfo2.m_iCurYear >= cSaveSlotInfo.m_iCurYear && (cSaveSlotInfo2.m_iCurYear != cSaveSlotInfo.m_iCurYear || (cSaveSlotInfo2.m_iCurMonth >= cSaveSlotInfo.m_iCurMonth && (cSaveSlotInfo2.m_iCurMonth != cSaveSlotInfo.m_iCurMonth || (cSaveSlotInfo2.m_iCurDay >= cSaveSlotInfo.m_iCurDay && (cSaveSlotInfo2.m_iCurDay != cSaveSlotInfo.m_iCurDay || (cSaveSlotInfo2.m_iCurH >= cSaveSlotInfo.m_iCurH && (cSaveSlotInfo2.m_iCurH != cSaveSlotInfo.m_iCurH || cSaveSlotInfo2.m_iCurM >= cSaveSlotInfo.m_iCurM))))))))
				{
					num4 = i;
					cSaveSlotInfo = cSaveSlotInfo2;
				}
			}
		}
		if (m_isEnable2ndGameStart)
		{
			m_ContinueItem.SetSaveSlotInfo(null);
			if (cSaveSlotInfo != null)
			{
				m_lastestSaveSlotIdx = num4;
			}
		}
		else if (cSaveSlotInfo != null)
		{
			m_ContinueItem.SetSaveSlotInfo(cSaveSlotInfo);
			m_lastestSaveSlotIdx = num4;
		}
	}

	private void SetXmlTexts()
	{
		Text[] textComps = new Text[11]
		{
			m_ExitText, m_MainMenuTitle.m_TitleText1, m_MainMenuTitle.m_TitleText2, m_LoadGameTitle.m_TitleText1, m_LoadGameTitle.m_TitleText2, m_SoundConfigTitle.m_TitleText1, m_GameConfigTitle.m_TitleText1, m_CrossSaveTitle1.m_TitleText1, m_CrossSaveTitle1.m_TitleText2, m_CrossSaveTitle2.m_TitleText1,
			m_CrossSaveTitle2.m_TitleText2
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_ExitText != null)
		{
			m_ExitText.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_EXIT_TEXT");
		}
		if (m_MainMenuTitle.m_TitleText1 != null)
		{
			m_MainMenuTitle.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_NORMAL_MODE_TITLE");
		}
		if (m_MainMenuTitle.m_TitleText2 != null)
		{
			m_MainMenuTitle.m_TitleText2.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_NORMAL_MODE_TITLE_SUB");
		}
		if (m_LoadGameTitle.m_TitleText1 != null)
		{
			m_LoadGameTitle.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_LOADGAME_MODE_TITLE");
		}
		if (m_LoadGameTitle.m_TitleText2 != null)
		{
			m_LoadGameTitle.m_TitleText2.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_LOADGAME_MODE_TITLE_SUB");
		}
		if (m_SoundConfigTitle.m_TitleText1 != null)
		{
			m_SoundConfigTitle.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_SOUND_MODE_TITLE");
		}
		if (m_GameConfigTitle.m_TitleText1 != null)
		{
			m_GameConfigTitle.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_CONFIG_MODE_TITLE");
		}
		if (m_CrossSaveTitle1.m_TitleText1 != null)
		{
			m_CrossSaveTitle1.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_CROSSSAVE_MODE1_TITLE");
		}
		if (m_CrossSaveTitle1.m_TitleText2 != null)
		{
			m_CrossSaveTitle1.m_TitleText2.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_CROSSSAVE_MODE1_TITLE_SUB");
		}
		if (m_CrossSaveTitle2.m_TitleText1 != null)
		{
			m_CrossSaveTitle2.m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_CROSSSAVE_MODE2_TITLE");
		}
		if (m_CrossSaveTitle2.m_TitleText2 != null)
		{
			m_CrossSaveTitle2.m_TitleText2.text = GameGlobalUtil.GetXlsProgramText("MAINMENU_CROSSSAVE_MODE2_TITLE_SUB");
		}
		m_ContinueItem.ResetFontByCurrentLanguage();
		m_NewGameItem.ResetFontByCurrentLanguage();
		m_NewGameItem2.ResetFontByCurrentLanguage();
		m_LoadGameItem.ResetFontByCurrentLanguage();
		m_CollectionItem.ResetFontByCurrentLanguage();
		m_SoundConfigItem.ResetFontByCurrentLanguage();
		m_GameConfigItem.ResetFontByCurrentLanguage();
		if (m_QuitGameItem != null)
		{
			m_QuitGameItem.ResetFontByCurrentLanguage();
		}
		m_ContinueItem.textName = GameGlobalUtil.GetXlsProgramText(m_isEnable2ndGameStart ? "MAINMENU_2ND_GAME_BTN_TEXT" : "MAINMENU_CONTINUE_BTN_TEXT");
		m_ContinueItem.textSub = GameGlobalUtil.GetXlsProgramText(m_isEnable2ndGameStart ? "MAINMENU_2ND_GAME_BTN_TEXT_SUB" : "MAINMENU_CONTINUE_BTN_TEXT_SUB");
		m_ContinueItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_02");
		m_NewGameItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_NEWGAME_BTN_TEXT");
		m_NewGameItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_NEWGAME_BTN_TEXT_SUB");
		m_NewGameItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_06");
		m_NewGameItem2.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_NEWGAME2_BTN_TEXT");
		m_NewGameItem2.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_NEWGAME2_BTN_TEXT_SUB");
		m_NewGameItem2.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_06");
		m_LoadGameItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_LOADGAME_BTN_TEXT");
		m_LoadGameItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_LOADGAME_BTN_TEXT_SUB");
		m_LoadGameItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_04");
		m_CollectionItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_COLLECTION_BTN_TEXT");
		m_CollectionItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_COLLECTION_BTN_TEXT_SUB");
		m_CollectionItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_08");
		m_SoundConfigItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_SOUND_BTN_TEXT");
		m_SoundConfigItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_SOUND_BTN_TEXT_SUB");
		m_SoundConfigItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_11");
		m_GameConfigItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_CONFIG_BTN_TEXT");
		m_GameConfigItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_CONFIG_BTN_TEXT_SUB");
		m_GameConfigItem.textChannel = GameGlobalUtil.GetXlsProgramText("MAINMENU_CH_14");
		if (m_QuitGameItem != null)
		{
			m_QuitGameItem.textName = GameGlobalUtil.GetXlsProgramText("MAINMENU_QUITGAME_BTN_TEXT");
			m_QuitGameItem.textSub = GameGlobalUtil.GetXlsProgramText("MAINMENU_QUITGAME_BTN_TEXT_SUB");
		}
	}

	public void Close(bool isEnableAnimation = true, MainMenuCommon.ExitMode exitMode = MainMenuCommon.ExitMode.None)
	{
		m_isInputBlock = true;
		if (base.eventNoticeExit != null)
		{
			base.eventNoticeExit(this, exitMode);
		}
		if (!isEnableAnimation)
		{
			CallClosedCallback();
			return;
		}
		m_CloseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CloseAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		m_CloseAnimator = null;
		base.gameObject.SetActive(value: false);
		if (base.eventCloseComplete != null)
		{
			base.eventCloseComplete(this, (m_OnCursorItem == m_CollectionItem) ? MainMenuCommon.Mode.CollectionMain : MainMenuCommon.Mode.None);
		}
	}

	private void AlignItems()
	{
		if (!m_isNeedAlign || GameGlobalUtil.IsAlmostSame(m_ScrollRect.viewport.rect.width, 0f))
		{
			return;
		}
		bool flag = m_lastestSaveSlotIdx >= 0;
		m_ContinueItem.gameObject.SetActive(flag || m_isEnable2ndGameStart);
		m_NewGameItem.gameObject.SetActive(!flag);
		m_NewGameItem2.gameObject.SetActive(flag);
		m_LoadGameItem.gameObject.SetActive(flag);
		m_CollectionItem.gameObject.SetActive(value: true);
		m_SoundConfigItem.gameObject.SetActive(value: true);
		m_GameConfigItem.gameObject.SetActive(value: true);
		if (m_QuitGameItem != null)
		{
			m_QuitGameItem.gameObject.SetActive(value: true);
		}
		float num = 0f;
		RectTransform rectTransform = null;
		m_validItems.Clear();
		MainMenuItem[] items = m_Items;
		foreach (MainMenuItem mainMenuItem in items)
		{
			if (!(mainMenuItem == null))
			{
				mainMenuItem.select = false;
				if (mainMenuItem.gameObject.activeSelf)
				{
					rectTransform = mainMenuItem.rectTransform;
					rectTransform.anchoredPosition = new Vector2(num, rectTransform.anchoredPosition.y);
					num += rectTransform.rect.width + (float)m_ItemInterval;
					m_validItems.Add(mainMenuItem);
				}
			}
		}
		float b = num - (float)m_ItemInterval + m_RightMargin;
		float size = Mathf.Max(m_ScrollRect.viewport.rect.width, b);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		m_ScrollHandler.ResetScrollRange();
		m_ScrollHandler.scrollPos = 0f;
		BuildScrollNodes();
		bool flag2 = false;
		int num2 = m_Items.Length;
		if (m_firstSelectItem < 0 || m_firstSelectItem >= num2)
		{
			for (int j = 0; j < num2; j++)
			{
				if (!(m_Items[j] == null) && m_Items[j].gameObject.activeSelf)
				{
					m_firstSelectItem = j;
					break;
				}
			}
		}
		else
		{
			flag2 = m_Items[m_firstSelectItem] == m_CollectionItem;
		}
		SetOnCursorItem(m_Items[m_firstSelectItem], !flag2);
		if (flag2)
		{
			m_ScrollHandler.SetScrollPos(s_backupedScrollPos_forCollectionMain);
		}
		m_isInputBlock = false;
		m_isNeedAlign = false;
	}

	public void SetOnCursorItem(MainMenuItem selectItem, bool isAdjustScrollPos = true)
	{
		if (!(m_OnCursorItem == selectItem))
		{
			if (m_OnCursorItem != null)
			{
				m_OnCursorItem.select = false;
			}
			if (selectItem != null)
			{
				selectItem.select = true;
			}
			m_OnCursorItem = selectItem;
			SetSelectedScrollNode((!(m_OnCursorItem != null)) ? (-1) : m_validItems.IndexOf(m_OnCursorItem));
			if (isAdjustScrollPos)
			{
				AdjustScrollPos_byOnCursonItem();
			}
		}
	}

	private bool ChangeOnCursorItem(bool isLeftSide, bool isAdjustScrollPos = true)
	{
		int count = m_validItems.Count;
		if (count < 0)
		{
			return false;
		}
		if (m_OnCursorItem == null)
		{
			SetOnCursorItem(m_validItems[0]);
			return false;
		}
		if (m_validItems.Count <= 1)
		{
			return false;
		}
		int num = m_validItems.IndexOf(m_OnCursorItem);
		num = ((!isLeftSide) ? (num + 1) : (num - 1));
		if (num < 0)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = count - 1;
		}
		else if (num >= count)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = 0;
		}
		SetOnCursorItem(m_validItems[num], isAdjustScrollPos);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		return true;
	}

	private void AdjustScrollPos_byOnCursonItem()
	{
		if (!(m_OnCursorItem == null))
		{
			float width = m_ScrollRect.viewport.rect.width;
			float num = 0f - m_ContentContanierRT.offsetMin.x;
			float num2 = num + width;
			float x = m_OnCursorItem.rectTransform.offsetMin.x;
			float num3 = m_OnCursorItem.rectTransform.offsetMin.x + m_OnCursorItem.rectTransform.rect.width * m_OnCursorItem.rectTransform.localScale.x;
			num3 += m_RightMargin;
			if (x < num)
			{
				float fTargetPos = 0f - x;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (num3 > num2)
			{
				float fTargetPos2 = width - num3;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	private void ChangeOnCursorItem_byScrollPos()
	{
		if (m_OnCursorItem == null)
		{
			return;
		}
		float width = m_ScrollRect.viewport.rect.width;
		float num = 0f - m_ContentContanierRT.offsetMin.x;
		float f = num + width;
		int num2 = Mathf.FloorToInt(num);
		int num3 = Mathf.CeilToInt(f);
		float x = m_OnCursorItem.rectTransform.offsetMin.x;
		float f2 = x + m_OnCursorItem.rectTransform.rect.width * m_OnCursorItem.rectTransform.localScale.x;
		int num4 = m_validItems.IndexOf(m_OnCursorItem);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(x) < num2)
		{
			int count = m_validItems.Count;
			for (int i = num4 + 1; i < count; i++)
			{
				rectTransform = m_validItems[i].rectTransform;
				x = rectTransform.offsetMin.x;
				if (Mathf.CeilToInt(x) >= num2)
				{
					SetOnCursorItem(m_validItems[i]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
		else
		{
			if (Mathf.FloorToInt(f2) <= num3)
			{
				return;
			}
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_validItems[num5].rectTransform;
				f2 = rectTransform.offsetMax.x;
				if (Mathf.FloorToInt(f2) <= num3)
				{
					SetOnCursorItem(m_validItems[num5]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
	}

	private bool Update_SteamSpec()
	{
		if (m_curMode == Mode.Normal && !m_isInputBlock)
		{
			if (!(m_pointEventObject == null))
			{
				if (m_isPusingScrollButton)
				{
					if (m_isOverTimePushingBound)
					{
						if (!m_ScrollHandler.IsScrolling)
						{
							if (m_pointEventObject == m_ScrollButton_Left)
							{
								ScrollPage(isNextPage: false);
							}
							else if (m_pointEventObject == m_ScrollButton_Right)
							{
								ScrollPage(isNextPage: true);
							}
						}
					}
					else
					{
						m_pusingTime += Time.deltaTime;
						if (m_pusingTime >= 0.3f)
						{
							m_isOverTimePushingBound = true;
						}
					}
				}
				return true;
			}
			float mouseWheelScrollDelta = GamePadInput.GetMouseWheelScrollDelta();
			if (!GameGlobalUtil.IsAlmostSame(mouseWheelScrollDelta, 0.0001f))
			{
				ScrollPage(mouseWheelScrollDelta < 0f);
				return true;
			}
			GameSwitch.eUIButType uIButType = GameSwitch.GetInstance().GetUIButType();
			if (m_prevUIButtonType != uIButType)
			{
				if (m_prevUIButtonType == GameSwitch.eUIButType.KEYMOUSE)
				{
					SetScrollButtonVisible_Left(isShow: false);
					SetScrollButtonVisible_Right(isShow: false);
				}
				m_prevUIButtonType = uIButType;
			}
			else if (m_prevUIButtonType == GameSwitch.eUIButType.KEYMOUSE)
			{
				SetScrollButtonVisible_Left(isShow: true);
				SetScrollButtonVisible_Right(isShow: true);
			}
		}
		if (m_Animator_ScrollBtnLeft != null && m_ScrollButton_Left != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator_ScrollBtnLeft.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime >= 0.999f)
			{
				m_ScrollButton_Left.SetActive(value: false);
				m_Animator_ScrollBtnLeft = null;
			}
		}
		if (m_Animator_ScrollBtnRight != null && m_ScrollButton_Right != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo2 = m_Animator_ScrollBtnRight.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo2.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo2.normalizedTime >= 0.999f)
			{
				m_ScrollButton_Right.SetActive(value: false);
				m_Animator_ScrollBtnRight = null;
			}
		}
		return false;
	}

	private void SetScrollButtonVisible_Left(bool isShow)
	{
		if (!(m_ScrollButton_Left == null))
		{
			if (isShow)
			{
				isShow = m_ScrollHandler.scrollPos > 0.0001f;
			}
			SetScrollButtonVisible(isShow, m_ScrollButton_Left, ref m_Animator_ScrollBtnLeft);
		}
	}

	private void SetScrollButtonVisible_Right(bool isShow)
	{
		if (!(m_ScrollButton_Right == null))
		{
			if (isShow)
			{
				isShow = m_ScrollHandler.scrollPos < m_ScrollHandler.scrollPos_Max - 0.0001f;
			}
			SetScrollButtonVisible(isShow, m_ScrollButton_Right, ref m_Animator_ScrollBtnRight);
		}
	}

	private void SetScrollButtonVisible(bool isShow, GameObject sclBtnObject, ref Animator sclBtnAnimator)
	{
		if (sclBtnObject == null)
		{
			return;
		}
		if (isShow)
		{
			if (!sclBtnObject.activeSelf)
			{
				sclBtnObject.SetActive(value: true);
			}
		}
		else if (sclBtnObject.activeSelf)
		{
			Animator componentInChildren = sclBtnObject.GetComponentInChildren<Animator>();
			if (componentInChildren != null && !componentInChildren.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)))
			{
				componentInChildren.Play(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
				sclBtnAnimator = componentInChildren;
			}
		}
	}

	public void OnPointerDown_ScrollButton(GameObject senderObj)
	{
		m_pointEventObject = senderObj;
		m_isPusingScrollButton = true;
		m_isOverTimePushingBound = false;
		m_pusingTime = 0f;
	}

	public void OnPointerUp_ScrollButton(GameObject senderObj)
	{
		m_pointEventObject = null;
		m_isPusingScrollButton = false;
		m_isOverTimePushingBound = false;
		m_pusingTime = 0f;
	}

	public void OnPointerEnter_ScrollButton(GameObject senderObj)
	{
		if (!(m_pointEventObject == null) && !(m_pointEventObject != senderObj))
		{
			m_isPusingScrollButton = true;
			m_isOverTimePushingBound = false;
			m_pusingTime = 0f;
		}
	}

	public void OnPointerExit_ScrollButton(GameObject senderObj)
	{
		if (!(m_pointEventObject == null) && !(m_pointEventObject != senderObj))
		{
			m_isPusingScrollButton = false;
			m_isOverTimePushingBound = false;
			m_pusingTime = 0f;
		}
	}

	public void OnPointerClick_ScrollButton(GameObject senderObj)
	{
		if (!m_isOverTimePushingBound)
		{
			if (senderObj == m_ScrollButton_Left)
			{
				ScrollPage(isNextPage: false);
			}
			else if (senderObj == m_ScrollButton_Right)
			{
				ScrollPage(isNextPage: true);
			}
		}
	}

	private void ScrollPage(bool isNextPage)
	{
		if (!isNextPage)
		{
			if (m_ScrollHandler.scrollPos > 0.0001f)
			{
				float num = m_ScrollHandler.scrollPos - m_ScrollRect.viewport.rect.width;
				m_ScrollHandler.ScrollToTargetPos(0f - num);
			}
		}
		else if (m_ScrollHandler.scrollPos < m_ScrollHandler.scrollPos_Max - 0.0001f)
		{
			float num2 = m_ScrollHandler.scrollPos + m_ScrollRect.viewport.rect.width;
			m_ScrollHandler.ScrollToTargetPos(0f - num2);
		}
	}

	private void BuildScrollNodes()
	{
		if (m_ScrollNodeSrcObj == null || m_ScrollNodeRT == null)
		{
			return;
		}
		ClearScrollNodes();
		int count = m_validItems.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_ScrollNodeSrcObj);
				Image component = gameObject.GetComponent<Image>();
				component.sprite = m_NotSelectNodeImage;
				RectTransform component2 = gameObject.GetComponent<RectTransform>();
				component2.SetParent(m_ScrollNodeRT, worldPositionStays: false);
				m_ScrollNodeImages.Add(component);
				gameObject.SetActive(value: true);
			}
			m_curSelectedScrollNodeIdx = -1;
		}
	}

	private void ClearScrollNodes()
	{
		Image image = null;
		int count = m_ScrollNodeImages.Count;
		for (int i = 0; i < count; i++)
		{
			image = m_ScrollNodeImages[i];
			UnityEngine.Object.Destroy(image.gameObject);
		}
		m_ScrollNodeImages.Clear();
	}

	private void SetSelectedScrollNode(int scrollNodeIdx)
	{
		if (m_curSelectedScrollNodeIdx != scrollNodeIdx)
		{
			int count = m_validItems.Count;
			if (m_curSelectedScrollNodeIdx >= 0 && m_curSelectedScrollNodeIdx < count)
			{
				m_ScrollNodeImages[m_curSelectedScrollNodeIdx].sprite = m_NotSelectNodeImage;
			}
			if (scrollNodeIdx >= 0 && scrollNodeIdx < count)
			{
				m_ScrollNodeImages[scrollNodeIdx].sprite = m_SelectNodeImage;
			}
			m_curSelectedScrollNodeIdx = scrollNodeIdx;
		}
	}

	private void OnProc_ItemSelect(object sender, object arg)
	{
		m_isInputBlock = false;
		MainMenuItem mainMenuItem = sender as MainMenuItem;
		if (mainMenuItem == m_ContinueItem)
		{
			if (!m_isEnable2ndGameStart)
			{
				m_loadableSaveSlotIdx = m_lastestSaveSlotIdx;
				Close(isEnableAnimation: true, MainMenuCommon.ExitMode.LoadGame);
			}
			else
			{
				Close(isEnableAnimation: true, MainMenuCommon.ExitMode.RestartGame);
			}
		}
		else if (mainMenuItem == m_NewGameItem)
		{
			Close(isEnableAnimation: true, MainMenuCommon.ExitMode.NewGame);
		}
		else if (mainMenuItem == m_NewGameItem2)
		{
			m_isInputBlock = true;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("MAINMENU_POPUP_NEWGAME2"), OnPopupResult_NewGame2);
		}
		else if (mainMenuItem == m_LoadGameItem)
		{
			ChangeMode(Mode.Load);
		}
		else if (mainMenuItem == m_CollectionItem)
		{
			s_backupedScrollPos_forCollectionMain = m_ScrollHandler.scrollPos;
			Close();
		}
		else if (mainMenuItem == m_SoundConfigItem)
		{
			ChangeMode(Mode.SoundConfig);
		}
		else if (mainMenuItem == m_GameConfigItem)
		{
			ChangeMode(Mode.GameConfig);
		}
		else if (mainMenuItem == m_QuitGameItem)
		{
			m_isInputBlock = true;
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("MAINMENU_POPUP_QUITGAME"), OnPopupResult_QuitGame);
		}
	}

	private void OnPopupResult_NewGame2(PopupDialoguePlus.Result result)
	{
		m_isInputBlock = false;
		if (result == PopupDialoguePlus.Result.Yes)
		{
			Close(isEnableAnimation: true, MainMenuCommon.ExitMode.RestartGame);
		}
	}

	private void OnPopupResult_QuitGame(PopupDialoguePlus.Result result)
	{
		m_isInputBlock = false;
		if (result == PopupDialoguePlus.Result.Yes)
		{
			Close(isEnableAnimation: true, MainMenuCommon.ExitMode.QuitGame);
		}
	}

	public void ChangeMode(Mode newMode)
	{
		if (!m_isWaitingForNextMode && m_curMode != newMode)
		{
			if (m_curMode == Mode.Normal)
			{
				m_disapperCheck_NormalMode = GameGlobalUtil.PlayUIAnimation_WithChidren(m_ContentRootObj, GameDefine.UIAnimationState.disappear.ToString());
				SetScrollButtonVisible_Left(isShow: false);
				SetScrollButtonVisible_Right(isShow: false);
			}
			if (m_goBackButton != null)
			{
				m_goBackButton.SetActive(newMode != Mode.Normal);
			}
			m_nextMode = newMode;
			m_isWaitingForNextMode = true;
		}
	}

	public void ActiveNextMode()
	{
		if (!m_isWaitingForNextMode)
		{
			return;
		}
		ModeLinkedInfo modeLinkedInfo = null;
		bool active = true;
		switch (m_nextMode)
		{
		case Mode.Normal:
			m_ContentRootObj.SetActive(value: true);
			active = false;
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_ContentRootObj, GameDefine.UIAnimationState.appear.ToString());
			if (m_prevUIButtonType == GameSwitch.eUIButType.KEYMOUSE)
			{
				SetScrollButtonVisible_Left(isShow: true);
				SetScrollButtonVisible_Right(isShow: true);
			}
			break;
		case Mode.Load:
			modeLinkedInfo = m_ModeInfo_Load;
			break;
		case Mode.SoundConfig:
			modeLinkedInfo = m_ModeInfo_SoundConfig;
			break;
		case Mode.GameConfig:
			modeLinkedInfo = m_ModeInfo_GameConfig;
			break;
		}
		if (m_ExitButtonRoot != null)
		{
			m_ExitButtonRoot.SetActive(active);
		}
		m_isWaitingForNextMode = false;
		m_curMode = m_nextMode;
		ChangeActiveTitle(m_curMode);
		if (modeLinkedInfo != null && modeLinkedInfo.m_prefabAssetObject != null)
		{
			if (modeLinkedInfo.m_linkedMenu == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(modeLinkedInfo.m_prefabAssetObject) as GameObject;
				modeLinkedInfo.m_linkedMenu = gameObject.GetComponent<SystemMenuBase>();
				Canvas componentInChildren = gameObject.GetComponentInChildren<Canvas>();
				componentInChildren.sortingOrder = m_ContentCanvas.sortingOrder + 1;
				Transform component = base.gameObject.GetComponent<Transform>();
				Transform component2 = gameObject.GetComponent<Transform>();
				component2.SetParent(component, worldPositionStays: false);
				component2.localPosition = Vector3.zero;
				component2.localRotation = Quaternion.identity;
				component2.localScale = Vector3.one;
			}
			SetCloseArgFunction(modeLinkedInfo.m_linkedMenu.Close);
			modeLinkedInfo.m_linkedMenu.ShowTitle(isShow: false);
			modeLinkedInfo.m_linkedMenu.isInFormMainMenu = true;
			modeLinkedInfo.m_linkedMenu.Show(isEnableAnimation: true, OnProc_ExitLinkedMenu, isNeedSetCloseCB: true, m_ExitPadButton);
		}
		else
		{
			SetCloseArgFunction(null);
		}
	}

	private void OnProc_ExitLinkedMenu(object sender, object arg)
	{
		if (sender is SaveDataMenu && arg is int num)
		{
			if (num >= 0)
			{
				m_loadableSaveSlotIdx = num;
				Close(isEnableAnimation: true, MainMenuCommon.ExitMode.LoadGame);
				return;
			}
		}
		else if (sender is ConfigMenuPlus)
		{
			if (arg is bool && (bool)arg)
			{
				float num2 = m_GameConfigItem.rectTransform.anchoredPosition.x - m_ScrollHandler.scrollPos;
				m_isEnable2ndGameStart = GameSwitch.GetInstance().GetShowEnding();
				m_firstSelectItem = 6;
				SetOnCursorItem(null);
				SetXmlTexts();
				CheckSaveData();
				m_isNeedAlign = true;
				AlignItems();
				m_ScrollHandler.SetScrollPos(m_GameConfigItem.rectTransform.anchoredPosition.x - num2);
			}
			else if ((sender as ConfigMenuPlus).IsChangedLanguage)
			{
				SetXmlTexts();
				CheckSaveData();
			}
		}
		else
		{
			m_isInputBlock = false;
		}
		ChangeMode(Mode.Normal);
		ActiveNextMode();
	}

	public void ChangeActiveTitle(Mode mode)
	{
		int num = m_TitleMembers.Length;
		for (int i = 0; i < num; i++)
		{
			m_TitleMembers[i].m_TitleRoot.SetActive(i == (int)mode);
		}
	}

	private void SetButtonLock(bool isLock)
	{
	}

	public void OnClickNotSelSlot(MainMenuItem mainMenu)
	{
		if (!MainLoadThing.instance.IsTouchableState() || m_isInputBlock)
		{
			return;
		}
		int num = m_validItems.IndexOf(mainMenu);
		int count = m_validItems.Count;
		if (BitCalc.CheckArrayIdx(num, count))
		{
			SetOnCursorItem(m_validItems[num]);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
		}
	}

	public void OnClickSelSlot(MainMenuItem mainMenu)
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_isInputBlock)
		{
			m_isInputBlock = true;
			OnProc_ItemSelect(mainMenu, null);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
		}
	}

	public void OnClickBack()
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_isInputBlock)
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
			if (m_fpCBClose != null)
			{
				m_fpCBClose();
			}
			else if (m_fpCBCloseArg != null)
			{
				m_fpCBCloseArg(isArg: true);
			}
			m_fpCBClose = null;
			m_fpCBCloseArg = null;
		}
	}

	public void SetCloseFunction(fpCBClose fpCBClose)
	{
		m_fpCBClose = fpCBClose;
	}

	public void SetCloseArgFunction(fpCBCloseArg fpCBCloseArg)
	{
		m_fpCBCloseArg = fpCBCloseArg;
	}
}
