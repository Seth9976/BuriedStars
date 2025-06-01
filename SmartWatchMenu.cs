using System;
using UnityEngine;
using UnityEngine.UI;

public class SmartWatchMenu : MonoBehaviour
{
	public enum Modes
	{
		Unknown = -1,
		Home,
		SNS,
		Messenger,
		Rank,
		Memo,
		Address,
		ToDo,
		Phone,
		Record,
		Config,
		Count
	}

	public enum SpecialAniState
	{
		move_left_start,
		move_left_appear,
		move_left_idle,
		move_left_return
	}

	public enum State
	{
		Hided,
		Idle,
		Appear,
		Disappear,
		IdleLeftSide,
		MoveToLeftSide,
		MoveToCenter
	}

	public enum ModeState
	{
		Unknown,
		Idle,
		Appear,
		Disappear,
		WaitForNextMode
	}

	[Serializable]
	public class HomeMenuButton
	{
		public GameObject m_ButtonObj;

		public Text m_ButtonText;

		public GameObject m_BannerObj;

		public Text m_BannerText;

		public GameObject m_LinkedMenuRoot;
	}

	public GameObject m_Root;

	public GameObject m_BackGound;

	public GameObject m_Title;

	public Text m_TitleText;

	public GameObject m_BackButton;

	public GameObject m_InputBlockObj;

	[Header("Watch Contents")]
	public GameObject m_WatchRoot;

	public GameObject[] m_ModeRoot = new GameObject[10];

	[Header("Home Menu")]
	public Text m_TimeText;

	public HomeMenuButton[] m_Buttons = new HomeMenuButton[9];

	private const string c_XlsDataName_TitleText = "SMART_WATCH_TITLE";

	private const string c_XlsDataName_TimeFormat = "SMART_WATCH_TIME_FORMAT";

	private const string c_XlsDataName_ButtonTextSNS = "SMART_WATCH_BUTTON_SNS";

	private const string c_XlsDataName_ButtonTextMSG = "SMART_WATCH_BUTTON_MSG";

	private const string c_XlsDataName_ButtonTextRank = "SMART_WATCH_BUTTON_RANK";

	private const string c_XlsDataName_ButtonTextMeno = "SMART_WATCH_BUTTON_MEMO";

	private const string c_XlsDataName_ButtonTextAddress = "SMART_WATCH_BUTTON_ADDRESS";

	private const string c_XlsDataName_ButtonTextToDo = "SMART_WATCH_BUTTON_TODO";

	private const string c_XlsDataName_ButtonTextPhone = "SMART_WATCH_BUTTON_PHONE";

	private const string c_XlsDataName_ButtonTextRecord = "SMART_WATCH_BUTTON_RECORD";

	private const string c_XlsDataName_ButtonTextConfig = "SMART_WATCH_BUTTON_CONFIG";

	private State m_curState;

	private ModeState m_curModeState;

	private Modes m_curMode = Modes.Unknown;

	private Modes m_nextMode = Modes.Unknown;

	private bool m_isChangeMode_EnableAnimation;

	private Animator m_AnimatorWatchSelf;

	private Animator m_AnimatorModeStateCheck;

	private GameDefine.EventProc m_fpClosedCallBack;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		m_AnimatorWatchSelf = null;
		m_AnimatorModeStateCheck = null;
		m_fpClosedCallBack = null;
	}

	private void Update()
	{
		if ((bool)m_AnimatorModeStateCheck)
		{
			switch (m_curModeState)
			{
			case ModeState.Appear:
				if (m_AnimatorModeStateCheck.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					m_curModeState = ModeState.Idle;
				}
				break;
			case ModeState.Disappear:
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_AnimatorModeStateCheck.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
				{
					m_curModeState = ((m_curMode != m_nextMode) ? ModeState.WaitForNextMode : ModeState.Unknown);
				}
				break;
			}
			case ModeState.WaitForNextMode:
				ChangeNextMode();
				break;
			}
		}
		if (!(m_AnimatorWatchSelf != null))
		{
			return;
		}
		switch (m_curState)
		{
		case State.Idle:
		case State.IdleLeftSide:
			break;
		case State.Appear:
			if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
			{
				ChangeState(State.Idle);
			}
			break;
		case State.Disappear:
		{
			AnimatorStateInfo currentAnimatorStateInfo2 = m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo2.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo2.normalizedTime >= 0.99f)
			{
				m_Root.SetActive(value: false);
				if (m_fpClosedCallBack != null)
				{
					m_fpClosedCallBack(this, null);
				}
			}
			break;
		}
		case State.MoveToLeftSide:
			if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(SpecialAniState.move_left_idle.ToString()))
			{
				ChangeState(State.IdleLeftSide);
			}
			break;
		case State.MoveToCenter:
			if (m_AnimatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
			{
				ChangeState(State.Idle);
			}
			break;
		}
	}

	public void Show(bool isShow, GameDefine.EventProc fpClosedCB = null)
	{
		if (!isShow)
		{
			ChangeState(State.Disappear);
			ChangeMode(Modes.Unknown, isEnableAnimation: true);
			ShowCommonMembers(isShow: false);
			return;
		}
		m_fpClosedCallBack = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		m_Root.SetActive(value: true);
		InitTextComponents();
		ChangeState(State.Appear);
		ChangeMode(Modes.Home, isEnableAnimation: true);
		ShowCommonMembers(isShow: true);
	}

	public void InitTextComponents()
	{
		Xls.ProgramText programText = null;
		if (m_TitleText != null)
		{
			programText = Xls.ProgramText.GetData_byKey("SMART_WATCH_TITLE");
			if (programText != null)
			{
				m_TitleText.text = programText.m_strTxt;
			}
		}
		string[] array = new string[9] { "SMART_WATCH_BUTTON_SNS", "SMART_WATCH_BUTTON_MSG", "SMART_WATCH_BUTTON_RANK", "SMART_WATCH_BUTTON_MEMO", "SMART_WATCH_BUTTON_ADDRESS", "SMART_WATCH_BUTTON_TODO", "SMART_WATCH_BUTTON_PHONE", "SMART_WATCH_BUTTON_RECORD", "SMART_WATCH_BUTTON_CONFIG" };
		HomeMenuButton homeMenuButton = null;
		int num = Mathf.Min(m_Buttons.Length, array.Length);
		for (int i = 0; i < num; i++)
		{
			homeMenuButton = m_Buttons[i];
			if (!(homeMenuButton.m_ButtonText == null))
			{
				programText = Xls.ProgramText.GetData_byKey(array[i]);
				homeMenuButton.m_ButtonText.text = ((programText == null) ? string.Empty : programText.m_strTxt);
			}
		}
	}

	public void OnClick_HomeMenuButton(string buttonName)
	{
		Modes modes = (Modes)Enum.Parse(typeof(Modes), buttonName, ignoreCase: true);
		HomeMenuButton homeMenuButton = GatModeHomeButtonInfo(modes);
		if (homeMenuButton != null && homeMenuButton.m_LinkedMenuRoot != null)
		{
			if (modes != Modes.Memo)
			{
				homeMenuButton.m_LinkedMenuRoot.SetActive(value: true);
			}
			switch (modes)
			{
			case Modes.Rank:
				StartCoroutine(RankMenu.ShowRankMenu_FormAssetBundle(OnClosed_InnerMenu));
				break;
			}
		}
		ChangeMode(modes, isEnableAnimation: true);
		ChangeState(State.MoveToLeftSide);
		ShowCommonMembers(isShow: false);
	}

	public void OnClick_BackButton()
	{
		Show(isShow: false);
	}

	private void OnClosed_InnerMenu(object sender, object arg)
	{
		HomeMenuButton homeMenuButton = GatModeHomeButtonInfo(m_curMode);
		if (homeMenuButton != null && homeMenuButton.m_LinkedMenuRoot != null)
		{
			homeMenuButton.m_LinkedMenuRoot.SetActive(value: false);
		}
		ChangeState(State.MoveToCenter);
		ChangeMode(Modes.Home, isEnableAnimation: true);
		ShowCommonMembers(isShow: true);
	}

	private void ChangeMode(Modes nextMode, bool isEnableAnimation)
	{
		if (m_curMode != nextMode)
		{
			m_nextMode = nextMode;
			m_isChangeMode_EnableAnimation = isEnableAnimation;
			GameObject modeRootObject = GetModeRootObject(m_curMode);
			if (!isEnableAnimation || m_curMode == Modes.Unknown || modeRootObject == null)
			{
				ChangeNextMode();
				return;
			}
			m_AnimatorModeStateCheck = GameGlobalUtil.PlayUIAnimation_WithChidren(modeRootObject, GameDefine.UIAnimationState.disappear.ToString());
			m_curModeState = ModeState.Disappear;
		}
	}

	private void ChangeNextMode()
	{
		GameObject modeRootObject = GetModeRootObject(m_curMode);
		if (modeRootObject != null)
		{
			modeRootObject.SetActive(value: false);
		}
		m_curMode = m_nextMode;
		modeRootObject = GetModeRootObject(m_curMode);
		if (modeRootObject == null)
		{
			m_curModeState = ModeState.Idle;
			return;
		}
		modeRootObject.SetActive(value: true);
		string strMot = ((!m_isChangeMode_EnableAnimation) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
		Animator animator = GameGlobalUtil.PlayUIAnimation_WithChidren(modeRootObject, strMot);
		m_AnimatorModeStateCheck = ((!m_isChangeMode_EnableAnimation) ? null : animator);
		m_curModeState = ((!m_isChangeMode_EnableAnimation) ? ModeState.Idle : ModeState.Appear);
	}

	private void ChangeState(State nextState)
	{
		if (m_curState != nextState)
		{
			string text = string.Empty;
			switch (nextState)
			{
			case State.Idle:
				text = GameDefine.UIAnimationState.idle.ToString();
				break;
			case State.Appear:
				text = GameDefine.UIAnimationState.appear.ToString();
				break;
			case State.Disappear:
				text = GameDefine.UIAnimationState.disappear.ToString();
				break;
			case State.IdleLeftSide:
				text = SpecialAniState.move_left_idle.ToString();
				break;
			case State.MoveToLeftSide:
				text = ((m_curState != State.Hided) ? SpecialAniState.move_left_start.ToString() : SpecialAniState.move_left_appear.ToString());
				break;
			case State.MoveToCenter:
				text = SpecialAniState.move_left_return.ToString();
				break;
			}
			if (m_AnimatorWatchSelf == null)
			{
				m_AnimatorWatchSelf = ((!(m_WatchRoot != null)) ? null : m_WatchRoot.GetComponentInChildren<Animator>());
			}
			if (!string.IsNullOrEmpty(text) && m_AnimatorWatchSelf != null)
			{
				GameGlobalUtil.PlayUIAnimation(m_AnimatorWatchSelf, text);
			}
			m_curState = nextState;
			if (m_InputBlockObj != null)
			{
				m_InputBlockObj.SetActive(m_curState != State.Idle);
			}
		}
	}

	private void ShowCommonMembers(bool isShow, bool isEnableAnimation = true)
	{
		if (isShow)
		{
			if (m_BackGound != null)
			{
				m_BackGound.SetActive(value: true);
			}
			if (m_Title != null)
			{
				m_Title.SetActive(value: true);
			}
			if (m_BackButton != null)
			{
				m_BackButton.SetActive(value: true);
			}
			if (isEnableAnimation)
			{
				string strMot = ((!isEnableAnimation) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot);
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_Title, strMot);
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButton, strMot);
			}
		}
		else if (isEnableAnimation)
		{
			string strMot2 = GameDefine.UIAnimationState.disappear.ToString();
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackGound, strMot2);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_Title, strMot2);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_BackButton, strMot2);
		}
		else
		{
			if (m_BackGound != null)
			{
				m_BackGound.SetActive(value: false);
			}
			if (m_Title != null)
			{
				m_Title.SetActive(value: false);
			}
			if (m_BackButton != null)
			{
				m_BackButton.SetActive(value: false);
			}
		}
	}

	private GameObject GetModeRootObject(Modes mode)
	{
		return (mode < Modes.Home || (int)mode >= m_ModeRoot.Length) ? null : m_ModeRoot[(int)mode];
	}

	private HomeMenuButton GatModeHomeButtonInfo(Modes mode)
	{
		int num = (int)(mode - 1);
		return (num < 0 || num >= m_Buttons.Length) ? null : m_Buttons[num];
	}
}
