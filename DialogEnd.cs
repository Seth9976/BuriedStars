using GameData;
using GameEvent;
using UnityEngine;

public class DialogEnd : MonoBehaviour
{
	private enum eDialogEndState
	{
		None,
		Appear,
		Appear2,
		Idle,
		Idle2,
		Push,
		Disappear
	}

	public Animator m_animDialogEndCircleBut;

	public Animator m_animDialogEndCrossBut;

	public Animator m_animDialogEndXboxB;

	public Animator m_animDialogEndXboxA;

	public Animator m_animDialogKeyboard;

	public Animator m_animDialogBut;

	private GameDefine.eAnimChangeState m_eAnimState;

	private eDialogEndState m_eDialogEndState;

	private float AUTO_DELAY_TIME = -1f;

	private float m_fAutoDelayTime;

	private void OnEnable()
	{
		AUTO_DELAY_TIME = GameSwitch.GetInstance().GetAutoDelayTime() + 1f;
		SetState(eDialogEndState.Appear);
	}

	private void OnDisable()
	{
		SetState(eDialogEndState.None);
	}

	private void Update()
	{
		ProcSkip();
		ProcInputButton();
		ProcAnimation();
	}

	private void ProcAnimation()
	{
		string text = null;
		eDialogEndState state = eDialogEndState.None;
		switch (m_eDialogEndState)
		{
		case eDialogEndState.Appear:
			text = GameDefine.UIAnimationState.appear.ToString();
			state = eDialogEndState.Idle;
			break;
		case eDialogEndState.Appear2:
			text = "appear_auto";
			state = eDialogEndState.Idle2;
			break;
		case eDialogEndState.Disappear:
			text = GameDefine.UIAnimationState.disappear.ToString();
			state = eDialogEndState.None;
			break;
		case eDialogEndState.Idle2:
			m_fAutoDelayTime += Time.deltaTime;
			if (m_fAutoDelayTime >= AUTO_DELAY_TIME)
			{
				SetState(eDialogEndState.Disappear);
			}
			break;
		case eDialogEndState.Push:
			text = "steam_push";
			state = eDialogEndState.Disappear;
			break;
		}
		if (text != null && GameGlobalUtil.CheckPlayEndUIAnimation(m_animDialogBut, text, ref m_eAnimState) && m_eAnimState == GameDefine.eAnimChangeState.play_end)
		{
			m_eAnimState = GameDefine.eAnimChangeState.none;
			SetState(state);
		}
	}

	private void ProcSkip()
	{
		if (m_eDialogEndState == eDialogEndState.Idle || m_eDialogEndState == eDialogEndState.Idle2)
		{
			EventEngine instance = EventEngine.GetInstance();
			if ((m_eDialogEndState == eDialogEndState.Idle || m_eDialogEndState == eDialogEndState.Idle2) && (instance == null || (instance != null && instance.GetSkip())))
			{
				SetState(eDialogEndState.Disappear);
			}
		}
	}

	private void ProcInputButton()
	{
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			PressDialogEnd();
		}
	}

	public void PressDialogEnd()
	{
		if (m_eDialogEndState == eDialogEndState.Idle)
		{
			SetState(eDialogEndState.Push);
		}
	}

	private void SetState(eDialogEndState eState)
	{
		switch (eState)
		{
		case eDialogEndState.Appear:
		{
			m_fAutoDelayTime = 0f;
			GameSwitch instance = GameSwitch.GetInstance();
			EventEngine instance2 = EventEngine.GetInstance();
			bool flag = false;
			bool flag2 = false;
			if (instance2 != null)
			{
				flag2 = instance2.GetAuto();
			}
			GameSwitch.eUIButType uIButType = GameSwitch.GetInstance().GetUIButType();
			if (instance != null && instance.GetOXType() == 0)
			{
				flag = true;
			}
			switch (uIButType)
			{
			case GameSwitch.eUIButType.PS4:
				m_animDialogBut = ((!flag) ? m_animDialogEndCrossBut : m_animDialogEndCircleBut);
				break;
			case GameSwitch.eUIButType.XBOX:
				m_animDialogBut = ((!flag) ? m_animDialogEndXboxA : m_animDialogEndXboxB);
				break;
			default:
				m_animDialogBut = m_animDialogKeyboard;
				break;
			}
			m_animDialogBut.gameObject.SetActive(value: true);
			m_animDialogBut.SetBool("m_isAuto", flag2);
			m_eAnimState = GameDefine.eAnimChangeState.none;
			GameGlobalUtil.PlayUIAnimation(m_animDialogBut, (!flag2) ? "appear" : "appear_auto", ref m_eAnimState);
			if (flag2)
			{
				m_eDialogEndState = eDialogEndState.Appear2;
				return;
			}
			break;
		}
		case eDialogEndState.Disappear:
			m_eAnimState = GameDefine.eAnimChangeState.none;
			GameGlobalUtil.PlayUIAnimation(m_animDialogBut, GameDefine.UIAnimationState.disappear, ref m_eAnimState);
			break;
		case eDialogEndState.Push:
			AudioManager.instance.PlayUISound("Push_NextBTN");
			m_eAnimState = GameDefine.eAnimChangeState.none;
			GameGlobalUtil.PlayUIAnimation(m_animDialogBut, "steam_push", ref m_eAnimState);
			break;
		case eDialogEndState.None:
			m_animDialogBut.gameObject.SetActive(value: false);
			break;
		}
		m_eDialogEndState = eState;
	}

	public bool IsProcDialogEnd()
	{
		return m_eDialogEndState == eDialogEndState.None;
	}

	public void TouchDialogEnd()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			PressDialogEnd();
		}
	}
}
