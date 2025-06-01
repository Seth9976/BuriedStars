using GameData;
using UnityEngine;
using UnityEngine.UI;

public class EventEngine_SelectPlus : MonoBehaviour
{
	private enum SELECT_OBJ
	{
		L_PARENT,
		R_PARENT,
		L_SELECTED,
		R_SELECTED,
		L_N_SELECTED,
		R_N_SELECTED
	}

	private enum eBottomGuide
	{
		Cursor,
		Select,
		Count
	}

	private static int SELECT_OBJ_CNT = 6;

	private ConstGameSwitch.eSELECT m_eSelectedVal;

	private int m_iCurSelectIdx;

	private bool m_isKeyLock;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private GameDefine.eAnimChangeState m_eSelectMotState;

	private Animator m_animSelect;

	private PMEventScript m_pmFSM;

	public GameObject m_goSelectWindow;

	public GameObject[] m_goSelect = new GameObject[SELECT_OBJ_CNT];

	public GameObject m_goCenterCircle;

	public GameObject m_goTime;

	public Slider m_uislTime;

	public Text m_txtTime;

	public TagText m_SelDialogTextData;

	public Text m_txtLSelSub;

	public Text m_txtLNSelSub;

	public Text m_txtLSelAns;

	public Text m_txtLNSelAns;

	public Text m_txtRSelSub;

	public Text m_txtRNSelSub;

	public Text m_txtRSelAns;

	public Text m_txtRNSelAns;

	public Animator m_animBG;

	public Animator m_animTimeCount;

	public Animator m_animDialogUIBG;

	public Animator m_animSelL;

	public Animator m_animNSelL;

	public Animator m_animSelR;

	public Animator m_animNSelR;

	public Animator m_animCenterCircle;

	private bool m_isQuitApplication;

	private bool m_isCompTyping;

	private float PRINT_AUTO_DELAY_TIME;

	private float m_fForceDelayTime;

	private string GetStrBottomGuide(eBottomGuide eGuide)
	{
		string text = null;
		string result = null;
		switch (eGuide)
		{
		case eBottomGuide.Cursor:
			text = "SELECT_BOT_MENU_CURSOR";
			break;
		case eBottomGuide.Select:
			text = "SELECT_BOT_MENU_SELECT";
			break;
		}
		if (text != null)
		{
			result = GameGlobalUtil.GetXlsProgramText(text);
		}
		return result;
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[9] { m_txtTime, m_txtLSelSub, m_txtLNSelSub, m_txtLSelAns, m_txtLNSelAns, m_txtRSelSub, m_txtRNSelSub, m_txtRSelAns, m_txtRNSelAns };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		FontManager.ResetTagTextFontByCurrentLanguage(m_SelDialogTextData);
		m_isKeyLock = false;
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		m_eSelectMotState = GameDefine.eAnimChangeState.none;
		GameSwitch instance = GameSwitch.GetInstance();
		if (instance != null)
		{
			PRINT_AUTO_DELAY_TIME = instance.GetAutoDelayTime();
		}
		GameGlobalUtil.PlayUIAnimation(m_animBG, GameDefine.UIAnimationState.appear);
		GameGlobalUtil.PlayUIAnimation(m_animTimeCount, GameDefine.UIAnimationState.appear);
		GameGlobalUtil.PlayUIAnimation(m_animDialogUIBG, GameDefine.UIAnimationState.appear);
		GameGlobalUtil.PlayUIAnimation(m_animNSelL, GameDefine.UIAnimationState.appear);
		GameGlobalUtil.PlayUIAnimation(m_animNSelR, GameDefine.UIAnimationState.appear);
		GameGlobalUtil.PlayUIAnimation(m_animCenterCircle, GameDefine.UIAnimationState.appear);
		GameMain instance2 = GameMain.instance;
		if (instance2 != null)
		{
			instance2.m_CommonButtonGuide.ClearContents();
			instance2.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Cursor), PadInput.GameInput.LStickX);
			instance2.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Select), PadInput.GameInput.CircleButton);
			instance2.m_CommonButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
			instance2.m_CommonButtonGuide.SetCanvasSortOrder(99);
			instance2.m_CommonButtonGuide.SetShow(isShow: true);
		}
		Invoke("BuildTextData", 0.1f);
	}

	private void OnDestroy()
	{
		m_animSelect = null;
		m_pmFSM = null;
	}

	private void BuildTextData()
	{
		m_SelDialogTextData.BuildTextData();
	}

	private void Update()
	{
		if (m_eSelectMotState != GameDefine.eAnimChangeState.none)
		{
			if (m_animSelect != null && GameGlobalUtil.CheckPlayEndUIAnimation(m_animSelect, "select_disappear", ref m_eSelectMotState) && m_eSelectMotState == GameDefine.eAnimChangeState.play_end)
			{
				CloseWindow();
			}
		}
		else if (m_eSelectedVal == ConstGameSwitch.eSELECT.TIME_OVER && m_eEndMotState != GameDefine.eAnimChangeState.none)
		{
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animBG, GameDefine.UIAnimationState.disappear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
			{
				CloseWindow();
			}
		}
		else if (m_eEndMotState == GameDefine.eAnimChangeState.none && !m_isKeyLock)
		{
			ProcSelect();
			ProcText();
			ProcInputButton();
		}
	}

	private void OnApplicationQuit()
	{
		m_isQuitApplication = true;
	}

	private void OnDisable()
	{
		m_SelDialogTextData.ClearTextData();
		GameMain instance = GameMain.instance;
		if (instance != null)
		{
			instance.m_CommonButtonGuide.ClearContents();
			instance.m_CommonButtonGuide.SetShow(isShow: false);
			if (!m_isQuitApplication)
			{
				instance.m_EventEngine.SetEventBotGuide(isShow: true);
			}
		}
	}

	public void ProcInputButton()
	{
		if (PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		switch (GamePadInput.GetLStickDownDir())
		{
		case GamePadInput.StickDir.Left:
			MoveLR(isL: true);
			break;
		case GamePadInput.StickDir.Right:
			MoveLR(isL: false);
			break;
		case GamePadInput.StickDir.None:
			if (m_eSelectedVal != ConstGameSwitch.eSELECT.NONE && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				SelectAnswer();
			}
			break;
		}
	}

	public void PlaySelectMotion()
	{
		m_isKeyLock = true;
		if (m_eSelectMotState == GameDefine.eAnimChangeState.none)
		{
			m_animSelect = ((m_eSelectedVal != ConstGameSwitch.eSELECT.LEFT) ? m_animSelR : m_animSelL);
			GameGlobalUtil.PlayUIAnimation(m_animSelect, "select_disappear", ref m_eSelectMotState);
		}
	}

	public void PlayEndMot()
	{
		if (m_eEndMotState == GameDefine.eAnimChangeState.none)
		{
			if (m_eSelectedVal == ConstGameSwitch.eSELECT.RIGHT)
			{
				m_goSelect[5].SetActive(value: false);
			}
			else if (m_eSelectedVal == ConstGameSwitch.eSELECT.LEFT)
			{
				m_goSelect[4].SetActive(value: false);
			}
			if (m_eSelectedVal == ConstGameSwitch.eSELECT.LEFT || m_eSelectedVal == ConstGameSwitch.eSELECT.RIGHT)
			{
				Animator animator = ((m_eSelectedVal != ConstGameSwitch.eSELECT.LEFT) ? m_animNSelL : m_animNSelR);
				m_animSelect = ((m_eSelectedVal != ConstGameSwitch.eSELECT.LEFT) ? m_animSelR : m_animSelL);
				GameGlobalUtil.PlayUIAnimation(m_animSelect, "select_disappear", ref m_eSelectMotState);
				GameGlobalUtil.PlayUIAnimation(animator, GameDefine.UIAnimationState.disappear);
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation(m_animSelL, GameDefine.UIAnimationState.disappear);
				GameGlobalUtil.PlayUIAnimation(m_animSelR, GameDefine.UIAnimationState.disappear);
				GameGlobalUtil.PlayUIAnimation(m_animNSelL, GameDefine.UIAnimationState.disappear);
				GameGlobalUtil.PlayUIAnimation(m_animNSelR, GameDefine.UIAnimationState.disappear);
			}
			GameGlobalUtil.PlayUIAnimation(m_animBG, GameDefine.UIAnimationState.disappear, ref m_eEndMotState);
			GameGlobalUtil.PlayUIAnimation(m_animTimeCount, GameDefine.UIAnimationState.disappear);
			GameGlobalUtil.PlayUIAnimation(m_animDialogUIBG, GameDefine.UIAnimationState.disappear);
			GameGlobalUtil.PlayUIAnimation(m_animCenterCircle, GameDefine.UIAnimationState.disappear);
		}
	}

	public void CloseWindow()
	{
		if (m_pmFSM != null)
		{
			m_pmFSM.SetSelectResult(m_eSelectedVal);
		}
		m_goSelectWindow.SetActive(value: false);
		m_pmFSM = null;
	}

	private string GetTextByKey(string strID)
	{
		string result = null;
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(strID);
		if (data_byKey != null)
		{
			result = data_byKey.m_strTxt;
		}
		return result;
	}

	public void SetSelect(PMEventScript pmScp, string strCurSelectKey, float fTime, ConstGameSwitch.eSELECT_TYPE eType = ConstGameSwitch.eSELECT_TYPE.NORMAL)
	{
		m_goSelectWindow.SetActive(value: true);
		m_pmFSM = pmScp;
		Xls.SelectData data_byKey = Xls.SelectData.GetData_byKey(strCurSelectKey);
		m_iCurSelectIdx = data_byKey.m_iIndex;
		GameSwitch instance = GameSwitch.GetInstance();
		bool flag = false;
		bool flag2 = false;
		if (instance != null && !AutoWrite.IsAutoWriteOn())
		{
			flag = instance.GetBefSelectSwitchOn(m_iCurSelectIdx, ConstGameSwitch.eSELECT.LEFT);
			flag2 = instance.GetBefSelectSwitchOn(m_iCurSelectIdx, ConstGameSwitch.eSELECT.RIGHT);
		}
		string textByKey = GetTextByKey(data_byKey.m_strIDQuest);
		string textByKey2 = GetTextByKey(data_byKey.m_strIDAns0);
		string textByKey3 = GetTextByKey(data_byKey.m_strIDAns1);
		string textByKey4 = GetTextByKey(data_byKey.m_strIDAns0Sub);
		string textByKey5 = GetTextByKey(data_byKey.m_strIDAns1Sub);
		string text = null;
		if (textByKey4 != null)
		{
			text = ((!data_byKey.m_strIDAns0Sub.Equals(string.Empty)) ? textByKey4 : string.Empty);
		}
		string text2 = null;
		if (textByKey5 != null)
		{
			text2 = ((!data_byKey.m_strIDAns1Sub.Equals(string.Empty)) ? textByKey5 : string.Empty);
		}
		if (textByKey != null)
		{
			m_SelDialogTextData.tagText = textByKey;
			m_SelDialogTextData.m_cbCompleteTypingEffect = CompleteTypingCB;
		}
		m_fForceDelayTime = 0f;
		m_isCompTyping = false;
		if (text != null)
		{
			Text txtLSelSub = m_txtLSelSub;
			string text3 = text;
			m_txtLNSelSub.text = text3;
			txtLSelSub.text = text3;
		}
		if (textByKey2 != null)
		{
			Text txtLSelAns = m_txtLSelAns;
			string text3 = textByKey2;
			m_txtLNSelAns.text = text3;
			txtLSelAns.text = text3;
			m_txtLSelAns.color = ((!flag) ? GameGlobalUtil.HexToColor(14342066) : GameGlobalUtil.HexToColor(2330757));
			m_txtLNSelAns.color = ((!flag) ? GameGlobalUtil.HexToColor(6840140) : GameGlobalUtil.HexToColor(4340796));
		}
		if (text2 != null)
		{
			Text txtRSelSub = m_txtRSelSub;
			string text3 = text2;
			m_txtRNSelSub.text = text3;
			txtRSelSub.text = text3;
		}
		if (textByKey3 != null)
		{
			Text txtRSelAns = m_txtRSelAns;
			string text3 = textByKey3;
			m_txtRNSelAns.text = text3;
			txtRSelAns.text = text3;
			m_txtRSelAns.color = ((!flag2) ? GameGlobalUtil.HexToColor(14342066) : GameGlobalUtil.HexToColor(2330757));
			m_txtRNSelAns.color = ((!flag2) ? GameGlobalUtil.HexToColor(6840140) : GameGlobalUtil.HexToColor(4340796));
		}
		if (fTime > 0f)
		{
			m_goCenterCircle.SetActive(value: false);
			m_goTime.SetActive(value: true);
			m_uislTime.minValue = 0f;
			Slider uislTime = m_uislTime;
			m_uislTime.maxValue = fTime;
			uislTime.value = fTime;
		}
		else
		{
			m_goCenterCircle.SetActive(value: true);
			m_goTime.SetActive(value: false);
		}
		m_eSelectedVal = ConstGameSwitch.eSELECT.NONE;
		m_goSelect[4].SetActive(value: true);
		m_goSelect[2].SetActive(value: false);
		m_goSelect[5].SetActive(value: true);
		m_goSelect[3].SetActive(value: false);
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		AudioManager.instance.PlayUISound("View_Select");
	}

	private void CompleteTypingCB()
	{
		m_isCompTyping = true;
	}

	private void NextText()
	{
		m_fForceDelayTime = 0f;
		m_isCompTyping = false;
		m_SelDialogTextData.ToNextPage();
	}

	public void ProcText()
	{
		if (m_isCompTyping)
		{
			m_fForceDelayTime += Time.deltaTime;
			if (m_fForceDelayTime >= PRINT_AUTO_DELAY_TIME)
			{
				NextText();
			}
		}
	}

	public void ProcSelect()
	{
		if (m_goTime.activeSelf)
		{
			m_uislTime.value -= Time.deltaTime;
			float num = Mathf.Floor(m_uislTime.value);
			num += 1f;
			m_txtTime.text = ((!(num < 10f)) ? string.Empty : "0") + num;
			if (m_uislTime.value <= 0f)
			{
				m_eSelectedVal = ConstGameSwitch.eSELECT.TIME_OVER;
				GameSwitch.GetInstance().SetSelectSwitch(m_iCurSelectIdx, (byte)m_eSelectedVal);
				PlayEndMot();
				m_isKeyLock = true;
			}
		}
	}

	private void SetSelectMot(ConstGameSwitch.eSELECT eSel)
	{
		if (m_eSelectedVal != eSel)
		{
			switch (eSel)
			{
			case ConstGameSwitch.eSELECT.LEFT:
				m_goSelect[2].SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation(m_animSelL, GameDefine.UIAnimationState.appear);
				m_goSelect[3].SetActive(value: false);
				break;
			case ConstGameSwitch.eSELECT.RIGHT:
				m_goSelect[3].SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation(m_animSelR, GameDefine.UIAnimationState.appear);
				m_goSelect[2].SetActive(value: false);
				break;
			}
			m_eSelectedVal = eSel;
		}
	}

	public void TouchLeft()
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_isKeyLock)
		{
			ConstGameSwitch.eSELECT eSELECT = ConstGameSwitch.eSELECT.LEFT;
			if (m_eSelectedVal != eSELECT)
			{
				MoveLR(isL: true);
			}
			else
			{
				SelectAnswer();
			}
			m_eSelectedVal = eSELECT;
		}
	}

	public void TouchRight()
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_isKeyLock)
		{
			ConstGameSwitch.eSELECT eSELECT = ConstGameSwitch.eSELECT.RIGHT;
			if (m_eSelectedVal != eSELECT)
			{
				MoveLR(isL: false);
			}
			else
			{
				SelectAnswer();
			}
			m_eSelectedVal = eSELECT;
		}
	}

	public void MoveLR(bool isL)
	{
		m_pmFSM.m_AudioManager.PlayUISound("SEL_SELECT");
		GameMain instance = GameMain.instance;
		if (instance != null)
		{
			instance.m_CommonButtonGuide.SetContentActivate(GetStrBottomGuide(eBottomGuide.Cursor), isActivate: true);
		}
		SetSelectMot(isL ? ConstGameSwitch.eSELECT.LEFT : ConstGameSwitch.eSELECT.RIGHT);
	}

	public void SelectAnswer()
	{
		m_pmFSM.m_AudioManager.PlayUISound("SEL_OK");
		GameMain instance = GameMain.instance;
		if (instance != null)
		{
			instance.m_CommonButtonGuide.SetContentActivate(GetStrBottomGuide(eBottomGuide.Select), isActivate: true);
		}
		GameSwitch.GetInstance().SetSelectSwitch(m_iCurSelectIdx, (byte)m_eSelectedVal);
		PlayEndMot();
		m_isKeyLock = true;
	}
}
