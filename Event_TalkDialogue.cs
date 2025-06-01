using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class Event_TalkDialogue : MonoBehaviour
{
	private enum eEndButtonType
	{
		PS_O,
		PS_X,
		XBox_B,
		XBox_A,
		KeyBoard
	}

	private enum ePressMot
	{
		none,
		press,
		disappear,
		done
	}

	private enum eDialogType
	{
		Narration,
		Dialog
	}

	private class DialogText
	{
		private string m_strChar;

		private string m_strText;

		public void SetTalkText(string strText, string strChar = "")
		{
			m_strChar = strChar;
			m_strText = strText;
		}

		public string GetChar()
		{
			return m_strChar;
		}

		public string GetText()
		{
			return m_strText;
		}
	}

	private enum NarOnceState
	{
		None,
		Enter,
		Wait,
		Exit,
		End
	}

	public GameObject m_goTalkWindow;

	public GameObject m_goDialog;

	public GameObject m_goNarration;

	public GameObject m_goSkip;

	public GameObject m_goHide;

	public GameObject m_goHideFull;

	public GameObject m_goAutoIcon;

	public Transform m_tfDialogText;

	public GameObject m_goCharNameDialog;

	public TagText m_tagtCharName;

	public GameObject m_goDialogButtons;

	public GameObject m_goBacklogButton;

	public GameObject m_goDialogTextData;

	public GameObject m_goNarrationTextData;

	public TagText m_tagtDialog;

	public TagText m_tagtNarration;

	[Header("Animation")]
	public Animator m_aniDialogButton;

	public Animator m_aniSkip;

	private const int iEndType = 5;

	public Animator[] m_aniDialogCircleCrossButton = new Animator[5];

	public GameObject m_goConversationSignCanvas;

	public GameObject m_goConversationTop;

	public GameObject m_goEventSystem;

	[HideInInspector]
	public Animator m_aniDialogEndButton;

	[Header("Button sprite Set")]
	public Button m_butHideUI;

	public Button[] m_butBackLog = new Button[3];

	public Button m_butAuto;

	public Button m_butSkip;

	[HideInInspector]
	public GameObject m_TEXTDATA;

	[HideInInspector]
	public TagText m_tagtCur;

	private EventEngine m_EventEngine;

	private TalkChar m_TalkChar;

	private GameSwitch m_GameSwitch;

	private float FORCE_PRINT_DELAY_TIME = -1f;

	private float PRINT_AUTO_DELAY_TIME = -1f;

	private bool m_isTouchableFunc = true;

	private float m_fForceDelayTime;

	private bool m_isHideUI;

	private bool m_isBefShowSkipState;

	private bool m_isCompletePrintTalk;

	private bool m_isTouchPrintTalk;

	private bool m_isAutoCheckEnd;

	private int m_iTotalPageCount;

	private int m_iCurPage;

	private string m_strCurTalkChar;

	private bool m_isCompPlayVoice;

	private bool m_isCompTyping;

	private bool m_isCallTalkEndMot;

	private bool m_isPlayTalkMot;

	private bool m_isBefHideTalkMot;

	private bool m_isBackLogShow;

	private int[] m_iTypingSpeed = new int[5];

	private bool m_isKeyLock;

	private bool m_isOpeningBacklog;

	private bool m_isNotTypingEndMot;

	private float m_fProcEndMotTime;

	private const float cENDMOT_TIME = 0.5f;

	private string m_strCharName = string.Empty;

	private string m_strCharNameColor = string.Empty;

	private ePressMot m_ePressMot;

	private GameDefine.eAnimChangeState m_ePrintEndMot;

	private eDialogType m_eDialogType;

	private static Event_TalkDialogue s_Instance;

	private Queue<DialogText> m_queueDialogText = new Queue<DialogText>();

	private bool m_isBefDialogBotState = true;

	private float m_fNarOnceEnterT;

	private float m_fNarOnceWaitT;

	private float m_fNarOnceExitT;

	private float m_fNarOncePassT;

	private float m_fNarOnceMaxT;

	private float m_fMinAlpha;

	private float m_fMaxAlpha;

	private NarOnceState m_eNarOnceState;

	private bool m_isBefAuto;

	public bool m_isOpenPrintWindow { get; private set; }

	public static Event_TalkDialogue instance => s_Instance;

	private void Start()
	{
		s_Instance = this;
	}

	private void Update()
	{
		if (m_isNotTypingEndMot)
		{
			m_fProcEndMotTime -= Time.deltaTime;
			if (m_fProcEndMotTime < 0f)
			{
				SetCharTalkEndMot();
			}
		}
		CheckUnlockKeylock();
	}

	private void OnEnable()
	{
		s_Instance = this;
		m_goEventSystem.SetActive(value: true);
		TagText[] tagTextComps = new TagText[4] { m_tagtCharName, m_tagtDialog, m_tagtNarration, m_tagtCur };
		FontManager.ResetTagTextFontByCurrentLanguage(tagTextComps);
		int oXType = GameSwitch.GetInstance().GetOXType();
		int num = 0;
		switch (GameSwitch.GetInstance().GetUIButType())
		{
		case GameSwitch.eUIButType.PS4:
			num = oXType;
			break;
		case GameSwitch.eUIButType.XBOX:
			num = 2 + oXType;
			break;
		case GameSwitch.eUIButType.KEYMOUSE:
			num = 4;
			break;
		}
		m_aniDialogEndButton = m_aniDialogCircleCrossButton[num];
		RectTransform component = m_aniDialogEndButton.gameObject.GetComponent<RectTransform>();
		if (component != null)
		{
			component.pivot = Vector2.zero;
			if (component.rect.height < 0f)
			{
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f - component.rect.height);
			}
		}
		m_goHide.SetActive(value: true);
		m_goHideFull.SetActive(value: false);
	}

	private void OnDisable()
	{
		m_strCurTalkChar = null;
		m_strCharName = null;
		m_strCharNameColor = null;
		m_goEventSystem.SetActive(value: false);
	}

	private void OnDestroy()
	{
		m_EventEngine = null;
		m_TalkChar = null;
		m_GameSwitch = null;
		if (m_queueDialogText != null)
		{
			m_queueDialogText.Clear();
		}
		m_queueDialogText = null;
		s_Instance = null;
	}

	public void AddTalkText(string strTalkKey, string strCharKey = "")
	{
		DialogText dialogText = new DialogText();
		dialogText.SetTalkText(strTalkKey, strCharKey);
		m_queueDialogText.Enqueue(dialogText);
	}

	public void SetNextQueueText()
	{
		if (m_queueDialogText.Count > 0)
		{
			DialogText dialogText = m_queueDialogText.Dequeue();
			string text = dialogText.GetChar();
			bool isTalkMot = text != null && text != string.Empty;
			SetTalkWindow(dialogText.GetChar(), dialogText.GetText(), isTalkMot);
		}
	}

	private void FreeDialogTalkText()
	{
		int count = m_queueDialogText.Count;
		for (int i = 0; i < count; i++)
		{
			DialogText dialogText = m_queueDialogText.Dequeue();
			dialogText = null;
		}
		m_queueDialogText.Clear();
	}

	private void InitMemberClass()
	{
		if (m_EventEngine == null)
		{
			m_EventEngine = EventEngine.GetInstance();
		}
		if (m_TalkChar == null)
		{
			m_TalkChar = m_EventEngine.m_TalkChar;
		}
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
	}

	private void InitValue()
	{
		InitMemberClass();
		SetCompletePrintTalk(isComplete: false);
		m_aniDialogEndButton.gameObject.SetActive(value: false);
		m_isOpenPrintWindow = false;
		m_fForceDelayTime = 0f;
		m_isHideUI = false;
		FORCE_PRINT_DELAY_TIME = m_GameSwitch.GetAutoDelayTime();
		PRINT_AUTO_DELAY_TIME = m_GameSwitch.GetAutoDelayTime() + 1f;
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("TYPING_SPEED_0");
		if (xlsProgramDefineStr != null)
		{
			m_iTypingSpeed[0] = int.Parse(xlsProgramDefineStr);
		}
		xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("TYPING_SPEED_1");
		if (xlsProgramDefineStr != null)
		{
			m_iTypingSpeed[1] = int.Parse(xlsProgramDefineStr);
		}
		xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("TYPING_SPEED_2");
		if (xlsProgramDefineStr != null)
		{
			m_iTypingSpeed[2] = int.Parse(xlsProgramDefineStr);
		}
		xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("TYPING_SPEED_3");
		if (xlsProgramDefineStr != null)
		{
			m_iTypingSpeed[3] = int.Parse(xlsProgramDefineStr);
		}
		xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("TYPING_SPEED_4");
		if (xlsProgramDefineStr != null)
		{
			m_iTypingSpeed[4] = int.Parse(xlsProgramDefineStr);
		}
	}

	public void InitTalkWindow()
	{
		if (m_goTalkWindow != null)
		{
			FreeDialogTalkText();
			m_goTalkWindow.SetActive(value: false);
			m_isOpenPrintWindow = false;
		}
	}

	private void SetTagTextObj(string strText, string strAlign = "", bool isSetTyping = true, bool isNarration = false)
	{
		byte typingEff = m_GameSwitch.GetTypingEff();
		bool flag = false;
		bool flag2 = typingEff != 4;
		flag = !isNarration && !flag2;
		SetCompletePrintTalk(flag);
		SetTouchPrintTalk(isTouch: false);
		m_isAutoCheckEnd = false;
		if ((bool)m_TEXTDATA)
		{
			m_tagtCur.m_cbCompleteTypingEffect = CompleteTypingCB;
			m_tagtCur.tagText = strText;
			if (isNarration)
			{
				m_tagtCur.useTypingEffect = isSetTyping;
				if (isSetTyping)
				{
					m_tagtCur.typingCPS = m_iTypingSpeed[typingEff];
				}
			}
			else
			{
				m_tagtCur.useTypingEffect = flag2;
				if (flag2)
				{
					m_tagtCur.typingCPS = m_iTypingSpeed[typingEff];
				}
				else if (m_isCompPlayVoice)
				{
					m_isNotTypingEndMot = true;
					m_fProcEndMotTime = 0.5f;
				}
			}
			if (!flag2)
			{
				m_isCompTyping = true;
			}
			SetAlign(strAlign);
			m_tagtCur.BuildTextData();
			m_iTotalPageCount = m_tagtCur.pageCount;
			m_iCurPage = m_tagtCur.currentPage;
		}
		if (m_EventEngine.GetSkip())
		{
			m_tagtCur.SkipPlayTypingEffect();
		}
	}

	private void SetAlign(string strAlign)
	{
		if (!strAlign.Equals(string.Empty))
		{
			int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strAlign);
			TextAnchor alignment = TextAnchor.UpperLeft;
			switch ((TALK_ALIGN)xlsScriptKeyValue)
			{
			case TALK_ALIGN.T_ALIGN_TL:
				alignment = TextAnchor.UpperLeft;
				break;
			case TALK_ALIGN.T_ALIGN_TC:
				alignment = TextAnchor.UpperCenter;
				break;
			case TALK_ALIGN.T_ALIGN_TR:
				alignment = TextAnchor.UpperRight;
				break;
			case TALK_ALIGN.T_ALIGN_CL:
				alignment = TextAnchor.MiddleLeft;
				break;
			case TALK_ALIGN.T_ALIGN_CC:
				alignment = TextAnchor.MiddleCenter;
				break;
			case TALK_ALIGN.T_ALIGN_CR:
				alignment = TextAnchor.MiddleRight;
				break;
			case TALK_ALIGN.T_ALIGN_BL:
				alignment = TextAnchor.LowerLeft;
				break;
			case TALK_ALIGN.T_ALIGN_BC:
				alignment = TextAnchor.LowerCenter;
				break;
			case TALK_ALIGN.T_ALIGN_BR:
				alignment = TextAnchor.LowerRight;
				break;
			}
			m_tagtCur.alignment = alignment;
		}
	}

	private void SetUI(bool isDialog, bool isNarration, bool isSkip)
	{
		m_goDialog.SetActive(isDialog);
		m_goNarration.SetActive(isNarration);
		m_goHide.SetActive(value: true);
		m_goAutoIcon.SetActive(value: true);
		SetActiveButtons(!m_EventEngine.GetSkip());
	}

	private void SetActiveButtons(bool isOn)
	{
		m_goHide.SetActive(isOn);
		m_goDialogButtons.SetActive(isOn);
		if (m_goBacklogButton != null)
		{
			m_goBacklogButton.SetActive(isOn);
		}
		if (isOn && m_aniDialogButton.gameObject.activeInHierarchy)
		{
			if (!m_isBefDialogBotState)
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogButton, GameDefine.UIAnimationState.appear);
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogButton, (!m_EventEngine.GetAuto()) ? GameDefine.UIAnimationState.idle : GameDefine.UIAnimationState.idle2);
			}
			GameGlobalUtil.PlayUIAnimation(m_aniSkip, GameDefine.UIAnimationState.idle);
			if (m_aniDialogButton.gameObject.activeInHierarchy)
			{
				m_aniDialogButton.SetBool("m_isAutoOn", m_EventEngine.GetAuto());
			}
		}
	}

	public void SetTalkWindow(string strCharKey, string strTalkKey, bool isTalkMot, bool isTouchableFunc = true)
	{
		InitValue();
		m_isTouchableFunc = isTouchableFunc;
		m_eDialogType = eDialogType.Dialog;
		m_isBefDialogBotState = m_goDialogButtons.activeInHierarchy;
		if (m_goTalkWindow != null)
		{
			m_goTalkWindow.SetActive(value: true);
			m_isOpenPrintWindow = true;
		}
		SetUI(isDialog: true, isNarration: false, isSkip: true);
		m_TEXTDATA = m_goDialogTextData;
		m_strCurTalkChar = strCharKey;
		Xls.ScriptSpeechData data_byKey = Xls.ScriptSpeechData.GetData_byKey(strTalkKey);
		string strTxt = data_byKey.m_strTxt;
		string text = string.Empty;
		string strSound = data_byKey.m_strSound;
		m_isCallTalkEndMot = (m_isCompPlayVoice = (m_isCompTyping = false));
		if (strSound != string.Empty)
		{
			m_isCompPlayVoice = false;
			if (!m_EventEngine.GetSkip())
			{
				AudioManager.instance.PlayVoice(strSound, isSetVol: false, 0f, 0f, isLoop: false, PlayEndVoiceSound);
			}
		}
		else
		{
			m_isCompPlayVoice = true;
		}
		string text2 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_NAME_DEF_COLOR");
		if (m_goCharNameDialog != null)
		{
			if (!strCharKey.Equals(string.Empty))
			{
				string xlsProgramText = GameGlobalUtil.GetXlsProgramText("CHR_UNKNOWN");
				Xls.CharData data_byKey2 = Xls.CharData.GetData_byKey(strCharKey);
				if (data_byKey2 != null)
				{
					text2 = data_byKey2.m_strTalkColor;
					Xls.TextData data_byKey3 = Xls.TextData.GetData_byKey(data_byKey2.m_strNameKey);
					if (data_byKey3 != null)
					{
						text = ((!m_GameSwitch.GetCharAnonymous(strCharKey)) ? data_byKey3.m_strTxt : xlsProgramText);
					}
				}
			}
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_NAME_START");
			string xlsProgramDefineStr2 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_NAME_END");
			string xlsProgramDefineStr3 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_END");
			if (xlsProgramDefineStr != null && xlsProgramDefineStr3 != null)
			{
				m_tagtCharName.tagText = xlsProgramDefineStr + text2 + xlsProgramDefineStr2 + text + xlsProgramDefineStr3;
				m_tagtCharName.BuildTextData();
			}
		}
		string strText = string.Empty;
		bool flag = strCharKey != string.Empty;
		string xlsProgramDefineStr4 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_END");
		if (strCharKey.Equals(string.Empty))
		{
			string xlsProgramDefineStr5 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_NONECHR_START");
			if (xlsProgramDefineStr5 != null && xlsProgramDefineStr4 != null)
			{
				strText = xlsProgramDefineStr5 + strTxt + xlsProgramDefineStr4;
			}
		}
		else
		{
			string xlsProgramDefineStr6 = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_DIALOGUE_START");
			if (xlsProgramDefineStr6 != null && xlsProgramDefineStr4 != null)
			{
				strText = xlsProgramDefineStr6 + strTxt + xlsProgramDefineStr4;
			}
		}
		m_tagtCur = m_tagtDialog;
		SetTagTextObj(strText, string.Empty);
		m_strCharName = text;
		m_strCharNameColor = text2;
		BacklogDataManager.AddBacklogData(m_tagtCur.GetCurrentPageText(), text, text2, strSound, m_tagtCur.IsExistNextPage());
		if (!m_EventEngine.GetSkip() && flag)
		{
			PlayTalkMotion(strCharKey, isTalkMot);
			GameMain gameMain = GameMain.instance;
			if (gameMain != null && gameMain.m_SmartWatchRoot != null && gameMain.m_SmartWatchRoot.GetCurrentPhoneState() != SWSub_PhoneMenu.EngageState.Unknown)
			{
				gameMain.m_SmartWatchRoot.PlayAniPhoneEngaging(strCharKey != "한도윤");
			}
		}
		m_ePressMot = ePressMot.none;
	}

	public void SetNarrationWindow(string strTalkKey, string strAlign, bool isTouchableFunc = true, bool isTyping = true)
	{
		InitValue();
		m_isTouchableFunc = isTouchableFunc;
		m_eDialogType = eDialogType.Narration;
		if (m_goTalkWindow != null)
		{
			m_goTalkWindow.SetActive(value: true);
			m_isOpenPrintWindow = true;
		}
		SetUI(isDialog: false, isNarration: true, isSkip: true);
		m_TEXTDATA = m_goNarrationTextData;
		m_TEXTDATA.SetActive(value: true);
		Xls.ScriptSpeechData data_byKey = Xls.ScriptSpeechData.GetData_byKey(strTalkKey);
		if (data_byKey != null)
		{
			string strTxt = data_byKey.m_strTxt;
			m_tagtCur = m_tagtNarration;
			SetTagTextObj(strTxt, strAlign, isTyping, isNarration: true);
			BacklogDataManager.AddBacklogData(m_tagtCur.GetCurrentPageText(), m_tagtCur.IsExistNextPage());
		}
	}

	public void SetNarrationOnce(string strTalkKey, string strAlign, float fEnterTime, float fWaitTime, float fExitTime)
	{
		SetNarrationWindow(strTalkKey, strAlign, isTouchableFunc: true, isTyping: false);
		m_fNarOnceEnterT = fEnterTime;
		m_fNarOnceWaitT = fWaitTime;
		m_fNarOnceExitT = fExitTime;
		SetNarState(NarOnceState.Enter);
	}

	public bool ProcNarrationOnce()
	{
		bool result = false;
		if (m_isKeyLock)
		{
			return false;
		}
		if (GameMain.IsActiveBackLogMenu())
		{
			return false;
		}
		if (ProcButton())
		{
			return false;
		}
		if (m_isOpeningBacklog)
		{
			return false;
		}
		if (m_isHideUI)
		{
			return result;
		}
		if (m_EventEngine.GetSkip())
		{
			result = !SetNarState(NarOnceState.End);
		}
		else
		{
			if (m_eNarOnceState == NarOnceState.End || m_eNarOnceState == NarOnceState.None)
			{
				return true;
			}
			m_fNarOncePassT += Time.deltaTime;
			float alpha = Mathf.Lerp(m_fMinAlpha, m_fMaxAlpha, m_fNarOncePassT / m_fNarOnceMaxT);
			m_tagtCur.SetAlpha(alpha);
			if (m_fNarOncePassT >= m_fNarOnceMaxT)
			{
				result = !SetNarState(m_eNarOnceState + 1);
			}
		}
		return result;
	}

	private bool SetNarState(NarOnceState eState)
	{
		bool flag = true;
		while (true)
		{
			switch (eState)
			{
			case NarOnceState.Enter:
				m_tagtCur.SetAlpha(0f);
				m_fNarOnceMaxT = m_fNarOnceEnterT;
				m_fMinAlpha = 0f;
				m_fMaxAlpha = 1f;
				break;
			case NarOnceState.Wait:
				m_tagtCur.SetAlpha(1f);
				m_fNarOnceMaxT = m_fNarOnceWaitT;
				m_fMinAlpha = 1f;
				m_fMaxAlpha = 1f;
				break;
			case NarOnceState.Exit:
				m_tagtCur.SetAlpha(1f);
				m_fNarOnceMaxT = m_fNarOnceExitT;
				m_fMinAlpha = 1f;
				m_fMaxAlpha = 0f;
				break;
			case NarOnceState.End:
				m_tagtCur.SetAlpha(1f);
				flag = m_tagtCur.ToNextPage();
				if (flag)
				{
					BacklogDataManager.AddBacklogData(m_tagtCur.GetCurrentPageText(), m_tagtCur.IsExistNextPage());
				}
				if (!m_EventEngine.GetSkip() && flag)
				{
					goto IL_0117;
				}
				if (!flag)
				{
					m_tagtCur.tagText = string.Empty;
					m_tagtCur.BuildTextData();
				}
				break;
			default:
				flag = false;
				break;
			}
			break;
			IL_0117:
			m_fNarOncePassT = 0f;
			eState = NarOnceState.Enter;
		}
		m_fNarOncePassT = 0f;
		m_eNarOnceState = eState;
		return flag;
	}

	public void SetCompletePrintTalk(bool isComplete)
	{
		m_isCompletePrintTalk = isComplete;
	}

	private bool SetNextText()
	{
		bool result = false;
		if (!m_tagtCur.ToNextPage())
		{
			AudioManager.instance.StopVoice();
			int count = m_queueDialogText.Count;
			if (count <= 0)
			{
				m_isTouchPrintTalk = true;
				result = true;
			}
			else
			{
				SetNextQueueText();
			}
		}
		else
		{
			BacklogDataManager.AddBacklogData(m_tagtCur.GetCurrentPageText(), m_strCharName, m_strCharNameColor, null, m_tagtCur.IsExistNextPage());
			m_ePressMot = ePressMot.none;
			m_aniDialogEndButton.gameObject.SetActive(value: false);
			m_isCompletePrintTalk = false;
			m_fForceDelayTime = 0f;
			m_iCurPage = m_tagtCur.currentPage;
		}
		return result;
	}

	private void SetTouchPrintTalk(bool isTouch)
	{
		if (isTouch)
		{
			SetCharTalkEndMot();
		}
		if (m_isCompletePrintTalk && isTouch)
		{
			if (m_EventEngine.GetAuto())
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, "disappear", ref m_ePrintEndMot);
				m_ePressMot = ePressMot.disappear;
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, "steam_push", ref m_ePrintEndMot);
				m_ePressMot = ePressMot.press;
			}
		}
		else if (!isTouch)
		{
			m_isTouchPrintTalk = isTouch;
		}
	}

	public void TouchPrintSkip(bool isSkip)
	{
		if (m_EventEngine == null)
		{
			InitMemberClass();
		}
		if (m_EventEngine != null && m_EventEngine.GetAuto() && isSkip)
		{
			SetAuto(isAuto: false);
		}
		SetCharTalkEndMot();
		m_aniDialogEndButton.gameObject.SetActive(value: false);
		if (!isSkip)
		{
			SetHideUI(isHide: false, isForce: true);
		}
	}

	public void SetPrintAuto()
	{
		m_EventEngine.ToggleAuto();
		if (m_aniDialogButton != null)
		{
			m_aniDialogButton.SetBool("m_isAutoOn", m_EventEngine.GetAuto());
		}
		SetToggleAuto(m_EventEngine.GetAuto());
		SetAutoSprImg();
	}

	public void TouchPrintAuto()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			SetPrintAuto();
		}
	}

	public void TouchPrintBacklog()
	{
		SetPrintBacklog();
	}

	public void TouchHideButton(bool isHide)
	{
		SetHideButton(isHide);
	}

	public void TouchPrintTalk()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			SetClickPrintTalk();
		}
	}

	public void SetClickPrintTalk()
	{
		if (m_isTouchableFunc && m_ePrintEndMot == GameDefine.eAnimChangeState.none)
		{
			if (m_tagtCur.IsPlayingTypingEffect())
			{
				m_tagtCur.SkipPlayTypingEffect();
				return;
			}
			if (m_eNarOnceState != NarOnceState.None && m_eNarOnceState != NarOnceState.End)
			{
				SetNarState(m_eNarOnceState + 1);
				return;
			}
			AudioManager.instance.PlayUISound("Push_NextBTN");
			SetTouchPrintTalk(isTouch: true);
		}
	}

	private void SetAuto(bool isAuto)
	{
		m_EventEngine.SetAuto(isAuto);
		if (m_aniDialogButton != null)
		{
			m_aniDialogButton.SetBool("m_isAutoOn", m_EventEngine.GetAuto());
		}
	}

	public void TogglePrintAuto(bool isAuto)
	{
		SetAuto(isAuto);
		SetToggleAuto(isAuto);
		SetAutoSprImg();
	}

	private void SetToggleAuto(bool isAuto)
	{
		AudioManager.instance.PlayUISound("Push_AutoBTN");
		if (m_aniDialogEndButton.gameObject.activeInHierarchy)
		{
			GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, (!isAuto) ? "idle" : "idle2");
		}
	}

	private void SetAutoSprImg()
	{
		bool auto = m_EventEngine.GetAuto();
		m_goAutoIcon.GetComponent<Toggle>().isOn = auto;
		GameGlobalUtil.PlayUIAnimation(m_aniDialogButton, (!auto) ? GameDefine.UIAnimationState.idle : GameDefine.UIAnimationState.idle2);
	}

	public void SetPrintBacklog()
	{
		if (!m_EventEngine.GetSkip())
		{
			m_isBefShowSkipState = m_EventEngine.GetShowSkipBut();
			AudioManager.instance.StopVoice();
			AudioManager.instance.PlayUISound("Push_TextlogBTN");
			m_isBackLogShow = true;
			m_isBefAuto = m_EventEngine.GetAuto();
			if (m_isBefAuto)
			{
				SetAuto(isAuto: false);
			}
			SetHideUI(isHide: true);
			GameMain.ShowBackLogMenu(isShow: true);
		}
		m_isOpeningBacklog = false;
		m_isKeyLock = false;
	}

	public void CloseBacklogCB()
	{
		m_isBackLogShow = false;
		SetHideUI(isHide: false);
		if (m_isBefAuto)
		{
			SetAuto(isAuto: true);
		}
	}

	public void SetHideButton(bool isHide)
	{
		if (isHide)
		{
			m_isBefShowSkipState = m_EventEngine.GetShowSkipBut();
		}
		isHide = m_goHide.activeInHierarchy;
		AudioManager.instance.PlayUISound("Push_WindowBTN");
		SetHideUI(isHide);
		m_isKeyLock = false;
	}

	private void SetHideUI(bool isHide, bool isForce = false)
	{
		m_isHideUI = isHide;
		if (m_isHideUI && m_EventEngine != null)
		{
			SetAuto(isAuto: false);
		}
		if (m_eDialogType == eDialogType.Narration)
		{
			m_goNarration.SetActive(!isHide);
		}
		else
		{
			m_goDialog.SetActive(!isHide);
		}
		if (m_goConversationTop.activeInHierarchy)
		{
			m_goConversationSignCanvas.SetActive(!isHide);
		}
		m_goHide.SetActive(!isHide);
		m_goDialogButtons.SetActive(!isHide);
		if (!isHide)
		{
			GameGlobalUtil.PlayUIAnimation(m_butHideUI.gameObject.GetComponent<Animator>(), GameDefine.UIAnimationState.appear);
			GameGlobalUtil.PlayUIAnimation(m_aniDialogButton, GameDefine.UIAnimationState.appear);
		}
		if (m_EventEngine != null && (isHide || (!isHide && m_isBefShowSkipState)))
		{
			m_EventEngine.SetShowSkipBut(!isHide);
		}
		m_goHideFull.SetActive(isHide);
		if (!isHide && m_isCompletePrintTalk)
		{
			GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, (!m_isBefAuto) ? "idle" : "idle_auto");
		}
	}

	private void CompleteTypingCB()
	{
		SetCompletePrintTalk(isComplete: true);
		m_isCompTyping = true;
		PlayAnimTalkEndMot();
	}

	private void PlayEndVoiceSound(object sender, object arg)
	{
		m_isCompPlayVoice = true;
		PlayAnimTalkEndMot();
		if (m_isHideUI && m_isPlayTalkMot)
		{
			PlayTalkMotion(m_strCurTalkChar, isTalkMot: false, isSaveTalkMot: false);
		}
	}

	private void PlayTalkMotion(string strCharKey, bool isTalkMot, bool isSaveTalkMot = true)
	{
		if (isSaveTalkMot)
		{
			m_isBefHideTalkMot = isTalkMot;
		}
		if ((!isSaveTalkMot && m_isBefHideTalkMot) || isSaveTalkMot)
		{
			m_isPlayTalkMot = isTalkMot;
			m_TalkChar.SetTalkMot(strCharKey, isTalkMot);
		}
	}

	private void PlayAnimTalkEndMot()
	{
		if (m_isCompPlayVoice && m_isCompTyping && !m_isCallTalkEndMot)
		{
			if (m_strCurTalkChar != null)
			{
				PlayTalkMotion(m_strCurTalkChar, isTalkMot: false);
			}
			m_isCallTalkEndMot = true;
			GameMain gameMain = GameMain.instance;
			if (gameMain != null && gameMain.m_SmartWatchRoot != null && gameMain.m_SmartWatchRoot.GetCurrentPhoneState() != SWSub_PhoneMenu.EngageState.Unknown)
			{
				gameMain.m_SmartWatchRoot.PlayAniPhoneEngaging(isPlay: false);
			}
		}
	}

	private void SetCharTalkEndMot()
	{
		if (m_isNotTypingEndMot)
		{
			m_fProcEndMotTime = 0f;
			m_isNotTypingEndMot = false;
		}
		if (m_strCurTalkChar != null)
		{
			PlayTalkMotion(m_strCurTalkChar, isTalkMot: false);
		}
	}

	public bool ProcPrintTalk()
	{
		bool result = false;
		if (GameMain.IsActiveBackLogMenu())
		{
			return false;
		}
		if (m_isKeyLock)
		{
			return false;
		}
		if (m_isOpeningBacklog)
		{
			return false;
		}
		if (m_EventEngine.GetSkip())
		{
			return SetNextText();
		}
		if (ProcButton())
		{
			return false;
		}
		if (m_isHideUI)
		{
			return false;
		}
		ProcTagtEndIcon();
		if (m_isCompletePrintTalk && m_isTouchPrintTalk)
		{
			result = true;
		}
		if (m_isCompletePrintTalk && m_isCompPlayVoice && m_isCompTyping && m_EventEngine.GetAuto())
		{
			m_fForceDelayTime += Time.deltaTime;
			if (m_fForceDelayTime >= PRINT_AUTO_DELAY_TIME && !m_isAutoCheckEnd)
			{
				m_isAutoCheckEnd = true;
				m_fForceDelayTime = 0f;
				SetTouchPrintTalk(isTouch: true);
			}
		}
		return result;
	}

	private bool ProcButton()
	{
		if (m_isBackLogShow)
		{
			return false;
		}
		if (m_isKeyLock)
		{
			return false;
		}
		if (m_EventEngine.GetSkip())
		{
			return false;
		}
		if (m_isHideUI)
		{
			if (GamePadInput.IsButtonState(PadInput.GameInput.CrossButton, PadInput.ButtonState.Down) || GamePadInput.IsButtonState(PadInput.GameInput.CircleButton, PadInput.ButtonState.Down) || GamePadInput.IsButtonState(PadInput.GameInput.TriangleButton, PadInput.ButtonState.Down) || GamePadInput.IsButtonState(PadInput.GameInput.SquareButton, PadInput.ButtonState.Down) || GamePadInput.IsButtonState(PadInput.GameInput.OptionButton, PadInput.ButtonState.Down))
			{
				SetHideButton(isHide: false);
			}
		}
		else
		{
			if (GamePadInput.IsButtonState(PadInput.GameInput.CircleButton, PadInput.ButtonState.Down))
			{
				SetClickPrintTalk();
				return false;
			}
			if (ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_butHideUI))
			{
				m_isKeyLock = true;
				return true;
			}
			if (ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_butAuto, null, null, null, null, isShowAnim: true, isExcuteEvent: false))
			{
				SetPrintAuto();
				return false;
			}
			if (ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_butBackLog[0], m_butBackLog[1]))
			{
				m_isKeyLock = true;
				m_isOpeningBacklog = true;
				return true;
			}
		}
		return false;
	}

	public bool ProcPrintForceTalk()
	{
		if (m_isHideUI)
		{
			return false;
		}
		ProcTagtEndIcon();
		bool flag = false;
		bool flag2 = m_isCompletePrintTalk && m_isCompPlayVoice;
		if (!m_EventEngine.GetSkip() && flag2)
		{
			m_fForceDelayTime += Time.deltaTime;
		}
		bool flag3 = m_fForceDelayTime >= FORCE_PRINT_DELAY_TIME;
		flag = m_iTotalPageCount <= m_iCurPage + 1 && flag3;
		if (flag2 && flag3)
		{
			SetNextText();
		}
		if (m_EventEngine.GetSkip())
		{
			m_isCompletePrintTalk = true;
			m_isCompPlayVoice = true;
			m_fForceDelayTime = FORCE_PRINT_DELAY_TIME;
		}
		return flag;
	}

	private void ProcTagtEndIcon()
	{
		if (m_ePressMot != ePressMot.none)
		{
			ProcNextText();
		}
		else if (m_goDialog.activeInHierarchy && !m_EventEngine.GetSkip() && m_isCompletePrintTalk && m_ePressMot != ePressMot.done && !m_aniDialogEndButton.gameObject.activeInHierarchy)
		{
			m_ePrintEndMot = GameDefine.eAnimChangeState.none;
			m_aniDialogEndButton.gameObject.SetActive(value: true);
			if (m_EventEngine.GetAuto())
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, "appear_auto");
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, "appear");
			}
			m_aniDialogEndButton.gameObject.GetComponent<RectTransform>().anchoredPosition = m_tagtDialog.lastPosition;
		}
	}

	private bool ProcNextText()
	{
		if (m_ePressMot == ePressMot.press && GameGlobalUtil.CheckPlayEndUIAnimation(m_aniDialogEndButton, "steam_push", ref m_ePrintEndMot))
		{
			if (m_ePrintEndMot == GameDefine.eAnimChangeState.play_end)
			{
				m_ePressMot = ePressMot.disappear;
				m_ePrintEndMot = GameDefine.eAnimChangeState.none;
				GameGlobalUtil.PlayUIAnimation(m_aniDialogEndButton, GameDefine.UIAnimationState.disappear, ref m_ePrintEndMot);
			}
		}
		else if (m_ePressMot == ePressMot.disappear && GameGlobalUtil.CheckPlayEndUIAnimation(m_aniDialogEndButton, "disappear", ref m_ePrintEndMot) && m_ePrintEndMot == GameDefine.eAnimChangeState.play_end)
		{
			m_ePressMot = ePressMot.done;
			m_aniDialogEndButton.gameObject.SetActive(value: false);
			m_ePrintEndMot = GameDefine.eAnimChangeState.none;
			SetNextText();
			return true;
		}
		return false;
	}

	private void CheckUnlockKeylock()
	{
		if (m_isKeyLock && !m_isHideUI && !GameMain.IsActiveBackLogMenu())
		{
			m_isKeyLock = false;
		}
	}
}
