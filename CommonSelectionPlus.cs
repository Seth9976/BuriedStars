using System;
using UnityEngine;
using UnityEngine.UI;

public class CommonSelectionPlus : MonoBehaviour
{
	[Serializable]
	public class SelectButton
	{
		public GameObject m_RootObject;

		public GameObject m_SelectedSkin;

		public Text m_SeletedText;

		public Text m_SeletedSubText;

		public GameObject m_NotSelectedSkin;

		public Text m_NotSeletedText;

		public Text m_NotSeletedSubText;
	}

	public enum Buttons
	{
		Left,
		Right,
		Exit
	}

	public enum State
	{
		Normal,
		LeftSelection,
		RightSelection,
		CancelExit
	}

	public GameObject m_RootObject;

	public SelectButton m_LeftButton = new SelectButton();

	public SelectButton m_RightButton = new SelectButton();

	[Header("Timer Params")]
	public GameObject m_TimerRoot;

	public Text m_TimerText;

	public GameObject m_NonTimerRoot;

	[Header("Dialog Area Params")]
	public GameObject m_DialogAreaRoot;

	public TagText m_DialogText;

	[Header("Exit Button Params")]
	public GameObject m_ExitButtonRoot;

	public Text m_ExitText;

	public Button m_ExitButton;

	public Button m_ExitIconButton;

	public GameObject m_goTouchLock;

	private State m_curState;

	private Animator m_DisappearCheckAnimator;

	private Animator m_DisappearCheckAnimator2;

	private GameDefine.EventProc m_fpResultCB;

	private CommonButtonGuide m_CommonButtonGuide;

	private bool m_isInitailizedButtonGuideText;

	private string m_ButtonGuide_SelAnswer = string.Empty;

	private string m_ButtonGuide_Submit = string.Empty;

	private string m_ButtonGuide_Exit = string.Empty;

	private AudioManager m_AudioManager;

	private const string c_aniStateName_SelectedDisappear = "select_disappear";

	public string leftText
	{
		get
		{
			return m_LeftButton.m_NotSeletedText.text;
		}
		set
		{
			m_LeftButton.m_NotSeletedText.text = value;
			m_LeftButton.m_SeletedText.text = value;
		}
	}

	public string leftSubText
	{
		get
		{
			return m_LeftButton.m_NotSeletedSubText.text;
		}
		set
		{
			m_LeftButton.m_NotSeletedSubText.text = value;
			m_LeftButton.m_SeletedSubText.text = value;
		}
	}

	public string rightText
	{
		get
		{
			return m_RightButton.m_NotSeletedText.text;
		}
		set
		{
			m_RightButton.m_NotSeletedText.text = value;
			m_RightButton.m_SeletedText.text = value;
		}
	}

	public string rightSubText
	{
		get
		{
			return m_RightButton.m_NotSeletedSubText.text;
		}
		set
		{
			m_RightButton.m_NotSeletedSubText.text = value;
			m_RightButton.m_SeletedSubText.text = value;
		}
	}

	private void OnDestroy()
	{
		m_DisappearCheckAnimator = null;
		m_DisappearCheckAnimator2 = null;
		m_fpResultCB = null;
		m_CommonButtonGuide = null;
		m_AudioManager = null;
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (m_DisappearCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_DisappearCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				bool flag = true;
				if (m_DisappearCheckAnimator2 != null)
				{
					currentAnimatorStateInfo = m_DisappearCheckAnimator2.GetCurrentAnimatorStateInfo(0);
					flag = currentAnimatorStateInfo.IsName("select_disappear") && currentAnimatorStateInfo.normalizedTime >= 0.99f;
				}
				if (flag)
				{
					Cloesed();
					m_DisappearCheckAnimator = null;
					m_DisappearCheckAnimator2 = null;
				}
			}
		}
		else
		{
			if (PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			float axisValue = GamePadInput.GetAxisValue(PadInput.GameInput.LStickX);
			if (!GameGlobalUtil.IsAlmostSame(axisValue, 0f))
			{
				if (axisValue < 0f && m_curState != State.LeftSelection)
				{
					SelectionProc(Buttons.Left);
					if (m_CommonButtonGuide != null)
					{
						m_CommonButtonGuide.SetContentActivate(m_ButtonGuide_SelAnswer, isActivate: true);
					}
				}
				else if (axisValue > 0f && m_curState != State.RightSelection)
				{
					SelectionProc(Buttons.Right);
					if (m_CommonButtonGuide != null)
					{
						m_CommonButtonGuide.SetContentActivate(m_ButtonGuide_SelAnswer, isActivate: true);
					}
				}
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) && (m_curState == State.LeftSelection || m_curState == State.RightSelection))
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("SEL_OK");
				}
				Close();
				if (m_CommonButtonGuide != null)
				{
					m_CommonButtonGuide.SetContentActivate(m_ButtonGuide_Submit, isActivate: true);
				}
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Cancel");
				}
				ChangeState(State.CancelExit);
				Close();
				if (m_CommonButtonGuide != null)
				{
					m_CommonButtonGuide.SetContentActivate(m_ButtonGuide_Exit, isActivate: true);
				}
			}
		}
	}

	public void Show(Xls.SNSReplyData xlsSNSReplyData, GameDefine.EventProc fpResultCB = null)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		try
		{
			Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsSNSReplyData.m_leftPostID);
			if (data_byKey != null)
			{
				text = data_byKey.m_strTxt;
			}
			data_byKey = Xls.TextData.GetData_byKey(xlsSNSReplyData.m_leftTextSub);
			if (data_byKey != null)
			{
				text2 = data_byKey.m_strTxt;
			}
			data_byKey = Xls.TextData.GetData_byKey(xlsSNSReplyData.m_rightPostID);
			if (data_byKey != null)
			{
				text3 = data_byKey.m_strTxt;
			}
			data_byKey = Xls.TextData.GetData_byKey(xlsSNSReplyData.m_rightTextSub);
			if (data_byKey != null)
			{
				text4 = data_byKey.m_strTxt;
			}
		}
		catch (Exception)
		{
		}
		Show(text, text2, text3, text4, fpResultCB);
	}

	public void Show(string _leftText, string _leftSubText, string _rightText, string _rightSubText, GameDefine.EventProc fpResultCB = null)
	{
		Text[] textComps = new Text[9] { m_LeftButton.m_NotSeletedText, m_LeftButton.m_NotSeletedSubText, m_LeftButton.m_SeletedText, m_LeftButton.m_SeletedSubText, m_RightButton.m_NotSeletedText, m_RightButton.m_NotSeletedSubText, m_RightButton.m_SeletedText, m_RightButton.m_SeletedSubText, m_TimerText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		FontManager.ResetTagTextFontByCurrentLanguage(m_DialogText);
		m_DisappearCheckAnimator = null;
		m_DisappearCheckAnimator2 = null;
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		leftText = _leftText;
		leftSubText = _leftSubText;
		rightText = _rightText;
		rightSubText = _rightSubText;
		if (m_ExitText != null)
		{
			m_ExitText.text = GameGlobalUtil.GetXlsProgramText("SELECT_BOT_MENU_EXIT");
		}
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
			m_AudioManager.PlayUISound("View_Select");
		}
		m_TimerRoot.SetActive(value: false);
		m_NonTimerRoot.SetActive(value: true);
		m_DialogAreaRoot.SetActive(value: false);
		if (m_goTouchLock != null)
		{
			m_goTouchLock.SetActive(value: false);
		}
		m_fpResultCB = ((fpResultCB == null) ? null : new GameDefine.EventProc(fpResultCB.Invoke));
		InitButtonGuide();
		ChangeState(State.Normal);
	}

	private void InitButtonGuide()
	{
		m_CommonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		if (!(m_CommonButtonGuide == null))
		{
			if (!m_isInitailizedButtonGuideText)
			{
				m_ButtonGuide_SelAnswer = GameGlobalUtil.GetXlsProgramText("SELECT_BOT_MENU_CURSOR");
				m_ButtonGuide_Submit = GameGlobalUtil.GetXlsProgramText("SELECT_BOT_MENU_SELECT");
				m_ButtonGuide_Exit = GameGlobalUtil.GetXlsProgramText("SELECT_BOT_MENU_EXIT");
				m_isInitailizedButtonGuideText = true;
			}
			m_CommonButtonGuide.ClearContents();
			m_CommonButtonGuide.SetCanvasSortOrder(100);
			m_CommonButtonGuide.AddContent(m_ButtonGuide_SelAnswer, PadInput.GameInput.LStickX);
			m_CommonButtonGuide.AddContent(m_ButtonGuide_Submit, PadInput.GameInput.CircleButton);
			m_CommonButtonGuide.AddContent(m_ButtonGuide_Exit, PadInput.GameInput.CrossButton);
			m_CommonButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
			m_CommonButtonGuide.SetShow(isShow: true);
		}
	}

	public void OnClick_LeftButton()
	{
		SelectionProc(Buttons.Left);
	}

	public void OnClick_RightButton()
	{
		SelectionProc(Buttons.Right);
	}

	public void OnClick_ExitButton()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		ChangeState(State.CancelExit);
		Close();
		if (m_CommonButtonGuide != null)
		{
			m_CommonButtonGuide.SetContentActivate(m_ButtonGuide_Exit, isActivate: true);
		}
	}

	private void SelectionProc(Buttons button)
	{
		switch (button)
		{
		case Buttons.Left:
		{
			if (m_curState == State.LeftSelection)
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("SEL_OK");
				}
				Close();
				break;
			}
			ChangeState(State.LeftSelection);
			string strMot2 = GameDefine.UIAnimationState.idle.ToString();
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_LeftButton.m_SelectedSkin, strMot2);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_RightButton.m_NotSelectedSkin, strMot2);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("SEL_SELECT");
			}
			break;
		}
		case Buttons.Right:
		{
			if (m_curState == State.RightSelection)
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("SEL_OK");
				}
				Close();
				break;
			}
			ChangeState(State.RightSelection);
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_LeftButton.m_NotSelectedSkin, strMot);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_RightButton.m_SelectedSkin, strMot);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("SEL_SELECT");
			}
			break;
		}
		}
	}

	private void ChangeState(State newState)
	{
		if (m_curState != newState)
		{
			switch (newState)
			{
			case State.Normal:
			case State.CancelExit:
				m_LeftButton.m_NotSelectedSkin.SetActive(value: true);
				m_LeftButton.m_SelectedSkin.SetActive(value: false);
				m_RightButton.m_NotSelectedSkin.SetActive(value: true);
				m_RightButton.m_SelectedSkin.SetActive(value: false);
				break;
			case State.LeftSelection:
				m_LeftButton.m_NotSelectedSkin.SetActive(value: false);
				m_LeftButton.m_SelectedSkin.SetActive(value: true);
				m_RightButton.m_NotSelectedSkin.SetActive(value: true);
				m_RightButton.m_SelectedSkin.SetActive(value: false);
				break;
			case State.RightSelection:
				m_LeftButton.m_NotSelectedSkin.SetActive(value: true);
				m_LeftButton.m_SelectedSkin.SetActive(value: false);
				m_RightButton.m_NotSelectedSkin.SetActive(value: false);
				m_RightButton.m_SelectedSkin.SetActive(value: true);
				break;
			}
			m_curState = newState;
		}
	}

	private void Close()
	{
		m_DisappearCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_DisappearCheckAnimator == null)
		{
			Cloesed();
			return;
		}
		GameObject gameObject = null;
		switch (m_curState)
		{
		case State.Normal:
			gameObject = null;
			break;
		case State.LeftSelection:
			gameObject = m_LeftButton.m_SelectedSkin;
			break;
		case State.RightSelection:
			gameObject = m_RightButton.m_SelectedSkin;
			break;
		}
		if (gameObject != null)
		{
			m_DisappearCheckAnimator2 = GameGlobalUtil.PlayUIAnimation_WithChidren(gameObject, "select_disappear");
		}
		if (m_goTouchLock != null)
		{
			m_goTouchLock.SetActive(value: true);
		}
	}

	private void Cloesed()
	{
		if (m_CommonButtonGuide != null)
		{
			m_CommonButtonGuide.SetShow(isShow: false);
		}
		if (m_fpResultCB != null)
		{
			Buttons buttons = Buttons.Exit;
			switch (m_curState)
			{
			case State.LeftSelection:
				buttons = Buttons.Left;
				break;
			case State.RightSelection:
				buttons = Buttons.Right;
				break;
			}
			m_fpResultCB(this, buttons);
		}
		m_RootObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
	}
}
