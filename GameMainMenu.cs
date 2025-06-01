using GameData;
using UnityEngine;
using UnityEngine.UI;

public class GameMainMenu : MonoBehaviour
{
	private enum eTutoObj
	{
		SmartWatch,
		Fater
	}

	public enum ePushState
	{
		None = -1,
		OptionMenu,
		SnsMenu,
		EndConsider,
		SmartWatch,
		QuickSave,
		EventKeyword
	}

	[Header("GameMain")]
	public GameMain m_GameMain;

	public Canvas m_Canvas;

	[Header("Quick Save")]
	public Text m_textQuickSave;

	[Header("Animation")]
	private const int m_iCountAnimUI = 6;

	public Animator[] m_animUI = new Animator[6];

	public GameObject[] m_goUI = new GameObject[6];

	private bool[] m_isDisappear = new bool[6];

	private GameDefine.eAnimChangeState[] m_eGameUIChgState = new GameDefine.eAnimChangeState[6];

	public string m_strExamineTuto = "tuto_00010";

	private GameDefine.eAnimChangeState m_eExamineStartState;

	private bool m_isRunExamineTuto;

	private bool m_isTutoKeyLock;

	private bool m_isMenuUIMoveComp;

	[Header("Tuto_GameObj")]
	private static int CNT_TUTO_OBJ = 3;

	private bool[] m_isTutoObj = new bool[CNT_TUTO_OBJ];

	public GameObject[] m_goTutoObj = new GameObject[CNT_TUTO_OBJ];

	[Header("SNS Shortcut")]
	public GameObject m_goNewSns;

	[Header("Mental Gage")]
	public GameObject m_MentalGageObject;

	[Header("Keyword Point Increase Effect")]
	public Animator m_animKeywordPoint;

	public Text m_textGetKeyCnt;

	public Text m_textMaxKeyCnt;

	public Text m_textSlash;

	public GameObject m_goKeywordPointShortCutButton;

	private int m_iBefPoint;

	private int m_iAfterPoint;

	private string m_strPlayAnim;

	private GameDefine.eAnimChangeState m_ePointAnimState;

	[Header("PS Button")]
	public Button m_butOption;

	public Button m_butSNS;

	public Button m_butSmartWatch;

	public Button m_butEndConsider;

	public Button m_butQuickSave;

	public Button m_butKeyOption;

	public Button m_butKeySNS;

	public Button m_butKeySmartWatch;

	public Button m_butKeyEndConsider;

	public Button m_butKeyQuickSave;

	public Button m_butEventKeyword;

	public ePushState m_ePushState = ePushState.None;

	private void Start()
	{
		InitAnimation();
	}

	private void Update()
	{
		CheckDisappear();
		UpdateButton();
	}

	private void OnDestroy()
	{
		m_GameMain = null;
	}

	private void InitAnimation()
	{
		int num = 6;
		for (int i = 0; i < num; i++)
		{
			m_isDisappear[i] = false;
			m_eGameUIChgState[i] = GameDefine.eAnimChangeState.none;
			SetGameMainMenuObjActive(i, isActive: false);
		}
		SNSShortCutButtonPressable(isTrue: false);
	}

	private void SetGameMainMenuObjActive(int iIdx, bool isActive)
	{
		if (m_goUI[iIdx] != null)
		{
			m_goUI[iIdx].SetActive(isActive);
		}
		if (iIdx == 0 && m_butSNS != null)
		{
			m_butSNS.gameObject.SetActive(isActive);
		}
	}

	private void cbFcTutorialExit(object sender, object arg)
	{
		m_isTutoKeyLock = false;
		m_isMenuUIMoveComp = true;
	}

	private void cbFcActionPopupExit(PopupDialoguePlus.Result result)
	{
		SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveColl, 0, cbCompSave);
	}

	private void cbCompSave(bool isExistErr)
	{
		bool flag = false;
		if (m_isRunExamineTuto)
		{
			flag = TutorialPopup.isShowAble(m_strExamineTuto);
			if (flag)
			{
				GameMain.instance.StartCoroutine(TutorialPopup.Show(m_strExamineTuto, cbFcTutorialExit, m_Canvas));
			}
		}
		if (!flag)
		{
			m_isTutoKeyLock = false;
			m_isMenuUIMoveComp = true;
		}
	}

	private void CheckDisappear()
	{
		int num = 1;
		if (m_goUI[num].activeInHierarchy && m_eGameUIChgState[num] != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_animUI[num], GameDefine.UIAnimationState.appear, ref m_eGameUIChgState[num]))
		{
			m_eGameUIChgState[num] = GameDefine.eAnimChangeState.none;
			m_isMenuUIMoveComp = true;
			if (m_GameMain.m_GameSwitch.GetCutConsider() && !m_GameMain.m_GameSwitch.GetAllKeywordPopByCut())
			{
				m_isMenuUIMoveComp = false;
			}
		}
		if (m_goUI[3].activeInHierarchy && m_eExamineStartState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_animUI[3], GameDefine.UIAnimationState.appear, ref m_eExamineStartState))
		{
			m_eExamineStartState = GameDefine.eAnimChangeState.none;
			if (m_GameMain.m_GameSwitch.GetCutConsider() && !m_GameMain.m_GameSwitch.GetAllKeywordPopByCut())
			{
				m_isTutoKeyLock = true;
				m_GameMain.m_GameSwitch.SetAllKeywordPopByCut(isShowPop: true);
				cbCompSave(isExistErr: false);
			}
		}
		Animator animator = null;
		int num2 = 6;
		for (int i = 0; i < num2; i++)
		{
			if (m_isDisappear[i] && m_eGameUIChgState[i] != GameDefine.eAnimChangeState.none)
			{
				animator = m_animUI[i];
				if (animator != null && GameGlobalUtil.CheckPlayEndUIAnimation(animator, GameDefine.UIAnimationState.disappear, ref m_eGameUIChgState[i]) && m_eGameUIChgState[i] == GameDefine.eAnimChangeState.play_end)
				{
					SetGameMainMenuObjActive(i, isActive: false);
					m_isDisappear[i] = false;
					m_eGameUIChgState[i] = GameDefine.eAnimChangeState.none;
				}
			}
		}
	}

	public void ShowAnimation(bool isShow, int iUIIdx, bool isForceSet = false)
	{
		if (!isForceSet && iUIIdx == 3 && !m_GameMain.m_GameSwitch.GetCutConsider())
		{
			return;
		}
		if (isShow)
		{
			switch (iUIIdx)
			{
			case 0:
				SetGameMainMenuObjActive(iUIIdx, m_isTutoObj[0]);
				m_goTutoObj[1].SetActive(m_isTutoObj[1]);
				break;
			case 4:
			{
				SetGameMainMenuObjActive(iUIIdx, isActive: true);
				SNSShortCutButtonPressable(isTrue: true);
				m_animUI[iUIIdx].speed = 1f;
				if (m_animUI[iUIIdx].gameObject.activeInHierarchy)
				{
					m_animUI[iUIIdx].SetBool("isFull", m_GameMain.m_GameSwitch.GetMustGetKeywordCnt() == m_GameMain.m_GameSwitch.GetMustMaxKeywordCnt());
				}
				SetKeywordPointText(m_GameMain.m_GameSwitch.GetMustGetKeywordCnt(), m_GameMain.m_GameSwitch.GetMustMaxKeywordCnt());
				Text[] textComps = new Text[3] { m_textGetKeyCnt, m_textMaxKeyCnt, m_textSlash };
				FontManager.ResetTextFontByCurrentLanguage(textComps);
				break;
			}
			case 5:
				SetGameMainMenuObjActive(iUIIdx, isActive: true);
				FontManager.ResetTextFontByCurrentLanguage(m_textQuickSave);
				m_textQuickSave.text = GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_TEXT_QUICKSAVE_SMALL");
				break;
			default:
				SetGameMainMenuObjActive(iUIIdx, isActive: true);
				break;
			}
		}
		m_isDisappear[iUIIdx] = !isShow;
		if (m_goUI[iUIIdx] != null && !m_goUI[iUIIdx].activeSelf)
		{
			return;
		}
		if (iUIIdx == 0)
		{
			SetNewSnsShortCut(m_GameMain.m_GameSwitch.IsSNSButtonBright());
		}
		if (isShow)
		{
			m_eGameUIChgState[iUIIdx] = GameDefine.eAnimChangeState.none;
			switch (iUIIdx)
			{
			case 3:
				m_isRunExamineTuto = GameSwitch.GetInstance().GetTutorial(m_strExamineTuto) == 0;
				m_eExamineStartState = GameDefine.eAnimChangeState.none;
				GameGlobalUtil.PlayUIAnimation(m_animUI[iUIIdx], GameDefine.UIAnimationState.appear, ref m_eExamineStartState);
				if (!m_GameMain.m_GameSwitch.GetAllKeywordPopByCut())
				{
					AudioManager.instance.PlayUISound("View_EndBTN");
				}
				break;
			case 2:
			{
				MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
				if (s_Instance != null)
				{
					s_Instance.SetMentalLevel_byCurrentMentalPoint();
				}
				break;
			}
			default:
				GameGlobalUtil.PlayUIAnimation(m_animUI[iUIIdx], GameDefine.UIAnimationState.appear);
				if (iUIIdx == 1)
				{
					m_isMenuUIMoveComp = false;
					m_eGameUIChgState[iUIIdx] = GameDefine.eAnimChangeState.changing;
				}
				break;
			}
		}
		else if (iUIIdx == 2)
		{
			MentalGageRenewal s_Instance2 = MentalGageRenewal.s_Instance;
			if (s_Instance2 != null)
			{
				s_Instance2.Hide();
			}
		}
		else if (m_animUI[iUIIdx].gameObject.activeInHierarchy)
		{
			GameGlobalUtil.PlayUIAnimation(m_animUI[iUIIdx], GameDefine.UIAnimationState.disappear, ref m_eGameUIChgState[iUIIdx]);
		}
	}

	public bool CheckMenuClickable()
	{
		bool flag = true;
		for (int i = 0; i < 6; i++)
		{
			if (m_eGameUIChgState[i] != GameDefine.eAnimChangeState.none)
			{
				flag = false;
			}
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	public void SetTutoObj(int iTutoState)
	{
		for (int i = 0; i < CNT_TUTO_OBJ; i++)
		{
			m_isTutoObj[i] = i < iTutoState;
		}
	}

	public void OnClick_SmartWatchButton()
	{
		if (!(m_GameMain == null) && !m_GameMain.GetIsZoomingMenu())
		{
			m_GameMain.m_EventEngine.m_TalkChar.SetDisalbeAllTalkIcon();
			m_GameMain.ShowEnterMenuZoom(GameMain.eGoMenu.SmartWatchMenu);
		}
	}

	public void OnClick_MenuButton()
	{
		if (m_GameMain != null)
		{
			StartCoroutine(m_GameMain.ShowSystemMenu());
		}
	}

	public void SetNewSnsShortCut(bool isNew)
	{
		m_goNewSns.SetActive(isNew);
	}

	public void OnClickSnsShortCutButton()
	{
		if (m_GameMain != null)
		{
			m_GameMain.StartCoroutine(m_GameMain.ShowSNSMenu(isShow: true, SNSMenuPlus.Mode.Keyword, m_GameMain.CBCloseSNSMenu));
		}
	}

	public void OnClickQuickSave()
	{
		if (m_GameMain != null)
		{
			StartCoroutine(m_GameMain.ShowQuickSaveMenu());
		}
	}

	public void OnClickEventKeyword()
	{
		if (m_GameMain != null)
		{
			m_GameMain.ShowEnterMenuZoom(GameMain.eGoMenu.EventKeywordMenu);
		}
	}

	public void OnClickExitButton()
	{
		PopupDialoguePlus.ShowActionPopup_YesNo(GameGlobalUtil.GetXlsProgramText("EXIT_BUTTON_Q"), CB_ExipPopupReslut);
	}

	private void CB_ExipPopupReslut(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes && m_GameMain != null)
		{
			m_GameMain.m_EventEngine.RunKeywordGameOver();
		}
	}

	public void OnClick_ActPointAddButton()
	{
	}

	public bool IsCompleteMentalGageChange()
	{
		MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
		return !(s_Instance != null) || s_Instance.IsCompleteChangeMentalGage();
	}

	public void SetMentalPoint(int iCurPoint, int iPrevPoint, bool isEnableTrans = true, GameDefine.EventProc fpComleteTrans = null)
	{
		MentalGageRenewal s_Instance = MentalGageRenewal.s_Instance;
		if (!(s_Instance == null))
		{
			if (!isEnableTrans)
			{
				s_Instance.SetMentalLevel_byPoint(iCurPoint, isShow: false);
			}
			else if (iCurPoint != iPrevPoint)
			{
				s_Instance.ShowChangedState_byMentalPoint(iCurPoint, iPrevPoint, fpComleteTrans);
			}
			else
			{
				s_Instance.SetMentalLevel_byPoint(iCurPoint);
			}
		}
	}

	private string SetPointDigit(int iIdx)
	{
		return $"{iIdx:D2}";
	}

	private void SetKeywordPointText(int iLeftIdx, int iRightIdx)
	{
		m_textGetKeyCnt.text = SetPointDigit(iLeftIdx);
		m_textMaxKeyCnt.text = SetPointDigit(iRightIdx);
		m_textSlash.text = GameGlobalUtil.GetXlsProgramText("STR_SLASH");
	}

	public void PlayAddPointAnim(int iBeforePoint, int iAfterPoint)
	{
		m_iBefPoint = iBeforePoint;
		m_iAfterPoint = iAfterPoint;
		m_animKeywordPoint.gameObject.SetActive(value: true);
		SetKeywordPointText(iBeforePoint, m_GameMain.m_GameSwitch.GetMustMaxKeywordCnt());
		m_strPlayAnim = "appear";
		GameGlobalUtil.PlayUIAnimation(m_animKeywordPoint, m_strPlayAnim, ref m_ePointAnimState);
		SNSShortCutButtonPressable(isTrue: false);
	}

	public void SNSShortCutButtonPressable(bool isTrue)
	{
		m_butEventKeyword.gameObject.SetActive(isTrue);
	}

	public bool ProcAddPointAnim()
	{
		bool flag = false;
		string text = null;
		m_animKeywordPoint.speed = m_GameMain.m_EventEngine.GetAnimatorSkipValue();
		if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animKeywordPoint, m_strPlayAnim, ref m_ePointAnimState))
		{
			if (m_strPlayAnim == "appear")
			{
				text = "plus1";
			}
			else if (m_strPlayAnim == "plus1")
			{
				SetKeywordPointText(m_iAfterPoint, m_GameMain.m_GameSwitch.GetMustMaxKeywordCnt());
				text = "plus2";
			}
			else if (m_strPlayAnim == "plus2")
			{
				text = "disappear";
			}
			else if (m_strPlayAnim == "disappear")
			{
				m_animKeywordPoint.gameObject.SetActive(value: false);
				text = (m_strPlayAnim = null);
			}
			m_ePointAnimState = GameDefine.eAnimChangeState.none;
			if (text != null)
			{
				GameGlobalUtil.PlayUIAnimation(m_animKeywordPoint, text, ref m_ePointAnimState);
				m_strPlayAnim = text;
			}
		}
		if (!flag && m_strPlayAnim == null && text == null)
		{
			flag = true;
		}
		return flag;
	}

	public bool IsGameMainMenuKeyLock()
	{
		bool result = false;
		if (PopupDialoguePlus.IsAnyPopupActivated() || GameMain.IsActiveBackLogMenu() || !CheckMenuClickable() || m_isTutoKeyLock || !m_isMenuUIMoveComp || (m_GameMain != null && m_GameMain.m_EventEngine != null && m_GameMain.m_EventEngine.IsEventRunning()) || !m_GameMain.IsGameMainStateDef() || m_GameMain.GetIsZoomingMenu())
		{
			result = true;
		}
		return result;
	}

	private void UpdateButton()
	{
		if (m_butOption == null || IsGameMainMenuKeyLock() || (m_GameMain != null && m_GameMain.IsPressTalkOkButton()) || m_ePushState != ePushState.None)
		{
			return;
		}
		bool flag = true;
		flag = false;
		if (ButtonPadInput.PressInputButton(PadInput.GameInput.OptionButton, m_butOption, m_butKeyOption, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.OptionMenu;
			AudioManager.instance.PlayUISound("Menu_System");
		}
		else if (ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_butSNS, m_butKeySNS, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.SnsMenu;
			AudioManager.instance.PlayUISound("Push_Maker");
		}
		else if (ButtonPadInput.PressInputButton(PadInput.GameInput.R1Button, m_butEndConsider, m_butKeyEndConsider, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.EndConsider;
			AudioManager.instance.PlayUISound("Push_EndBTN");
		}
		else if (ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_butSmartWatch, m_butKeySmartWatch, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.SmartWatch;
			if (AudioManager.instance != null)
			{
				AudioManager.instance.PlayUISound("Watch_In");
			}
		}
		else if (ButtonPadInput.PressInputButton(PadInput.GameInput.TouchPadButton, m_butQuickSave, m_butKeyQuickSave, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.QuickSave;
			AudioManager.instance.PlayUISound("Menu_System");
		}
		else if (ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_butEventKeyword, null, null, null, EndPushGameMainMenuButton, isShowAnim: true, flag))
		{
			m_ePushState = ePushState.EventKeyword;
			AudioManager.instance.PlayUISound("Menu_System");
		}
	}

	private void EndPushGameMainMenuButton()
	{
		switch (m_ePushState)
		{
		case ePushState.OptionMenu:
			OnClick_MenuButton();
			break;
		case ePushState.EndConsider:
			OnClickExitButton();
			break;
		case ePushState.SnsMenu:
			OnClickSnsShortCutButton();
			break;
		case ePushState.SmartWatch:
			OnClick_SmartWatchButton();
			break;
		case ePushState.EventKeyword:
			OnClickEventKeyword();
			break;
		case ePushState.QuickSave:
			OnClickQuickSave();
			break;
		}
		m_ePushState = ePushState.None;
	}

	public void TouchExitButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Push_EndBTN");
			OnClickExitButton();
		}
	}

	public void TouchEventKeyword()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Menu_System");
			OnClickEventKeyword();
		}
	}

	public void TouchMenuButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Menu_System");
			OnClick_MenuButton();
		}
	}

	public void TouchQuickSave()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Menu_System");
			OnClickQuickSave();
		}
	}

	public void TouchSmartWatchButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Watch_In");
			OnClick_SmartWatchButton();
		}
	}

	public void TouchSnsShortCutButton()
	{
		if (MainLoadThing.instance.IsTouchableState() && !IsGameMainMenuKeyLock())
		{
			AudioManager.instance.PlayUISound("Push_Maker");
			OnClickSnsShortCutButton();
		}
	}
}
