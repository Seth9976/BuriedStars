using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SkipButtonGuide : MonoBehaviour
{
	private enum State
	{
		Unknown,
		Appear,
		Idle,
		IdleActived,
		Disappear
	}

	private static SkipButtonGuide s_instance;

	public Image m_ButtonImageComp;

	public Sprite m_ButtonSpriteNormal;

	public Sprite m_ButtonSpritePressed;

	public PadIconHandler m_PadIconHandler;

	private State m_curState;

	private bool m_isPressed;

	private float m_buttonPressedEffTime;

	private float m_remianButtonEffTime;

	private bool m_isSkipActived;

	private const string c_aniStateName_SkipActived = "idle_active";

	private Animator m_Animator;

	private GameDefine.EventProc m_fpAniFinished;

	public static SkipButtonGuide instance => s_instance;

	public void Init()
	{
		s_instance = this;
		m_buttonPressedEffTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("BUTTON_PRESS_EFF_TIME");
		if ((bool)m_PadIconHandler)
		{
			m_PadIconHandler.fpNoticeReflashed = OnReflashed_PadIconHandler;
		}
		m_Animator = base.gameObject.GetComponentInChildren<Animator>();
	}

	private void OnDestroy()
	{
		m_Animator = null;
		m_fpAniFinished = null;
		s_instance = null;
	}

	private void OnReflashed_PadIconHandler(object sender, object args)
	{
		if (args is PadIconManager.IconPair iconPair)
		{
			m_ButtonSpriteNormal = iconPair.m_Normal;
			m_ButtonSpritePressed = iconPair.m_Pressed;
			ChangePressState(m_isPressed);
		}
	}

	private void Update()
	{
		if (m_remianButtonEffTime > 0f)
		{
			m_remianButtonEffTime -= Time.deltaTime;
			if (m_remianButtonEffTime <= 0f)
			{
				ChangePressState(isPressed: false);
			}
		}
		if (!(m_Animator != null))
		{
			return;
		}
		switch (m_curState)
		{
		case State.Appear:
		{
			AnimatorStateInfo currentAnimatorStateInfo2 = m_Animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo2.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear)) && currentAnimatorStateInfo2.normalizedTime > 0.99f)
			{
				if (m_fpAniFinished != null)
				{
					m_fpAniFinished(this, null);
					m_fpAniFinished = null;
				}
				ChangeState((!m_isSkipActived) ? State.Idle : State.IdleActived);
			}
			break;
		}
		case State.Idle:
		case State.IdleActived:
			break;
		case State.Disappear:
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime > 0.99f)
			{
				base.gameObject.SetActive(value: false);
				if (m_fpAniFinished != null)
				{
					m_fpAniFinished(this, null);
					m_fpAniFinished = null;
				}
				ChangeState(State.Unknown);
			}
			break;
		}
		}
	}

	private void ChangeState(State newState, bool isIngnoreSame = true)
	{
		if (m_curState != newState || !isIngnoreSame)
		{
			float animationSpeedRate = GetAnimationSpeedRate();
			switch (newState)
			{
			case State.Appear:
				GameGlobalUtil.PlayUIAnimation_WithChidren(s_instance.gameObject, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear), animationSpeedRate);
				break;
			case State.Idle:
				GameGlobalUtil.PlayUIAnimation_WithChidren(s_instance.gameObject, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.idle));
				break;
			case State.IdleActived:
				GameGlobalUtil.PlayUIAnimation_WithChidren(s_instance.gameObject, "idle_active");
				break;
			case State.Disappear:
				GameGlobalUtil.PlayUIAnimation_WithChidren(s_instance.gameObject, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear), animationSpeedRate);
				break;
			}
			m_curState = newState;
		}
	}

	public void ChangePressState(bool isPressed)
	{
		m_isPressed = isPressed;
		if (!(m_ButtonImageComp == null))
		{
			m_ButtonImageComp.sprite = ((!m_isPressed) ? m_ButtonSpriteNormal : m_ButtonSpritePressed);
			m_remianButtonEffTime = ((!m_isPressed) ? 0f : m_buttonPressedEffTime);
		}
	}

	public void ChangeSkipActivateState(bool isActive)
	{
		if (m_isSkipActived != isActive)
		{
			m_isSkipActived = isActive;
			if (m_Animator != null)
			{
				m_Animator.SetBool("IsActive", m_isSkipActived);
			}
			if (m_curState == State.Idle || m_curState == State.IdleActived)
			{
				ChangeState((!m_isSkipActived) ? State.Idle : State.IdleActived);
			}
		}
	}

	public static bool IsVisible()
	{
		if (s_instance == null)
		{
			return false;
		}
		return s_instance.gameObject.activeSelf;
	}

	public static void Show(bool isEnableAni = true, GameDefine.EventProc fpAniFinished = null)
	{
		if (!(s_instance == null) && !s_instance.gameObject.activeSelf)
		{
			s_instance.gameObject.SetActive(value: true);
			s_instance.ChangeState(isEnableAni ? State.Appear : ((!s_instance.m_isSkipActived) ? State.Idle : State.IdleActived));
			s_instance.m_fpAniFinished = ((!isEnableAni) ? null : fpAniFinished);
		}
	}

	public static void Hide(bool isEnableAni = true, GameDefine.EventProc fpAniFinished = null)
	{
		if (!(s_instance == null) && s_instance.gameObject.activeSelf)
		{
			if (!isEnableAni)
			{
				s_instance.ChangeState(State.Unknown);
				s_instance.gameObject.SetActive(value: false);
				s_instance.m_fpAniFinished = null;
			}
			else
			{
				s_instance.ChangeState(State.Disappear);
				s_instance.m_fpAniFinished = fpAniFinished;
			}
		}
	}

	private static float GetAnimationSpeedRate()
	{
		float result = 1f;
		EventEngine eventEngine = EventEngine.GetInstance(isCreate: false);
		if (eventEngine != null && eventEngine.GetSkip())
		{
			result = eventEngine.GetAnimatorSkipValue();
		}
		return result;
	}

	public static void SetPressState(bool isPressed)
	{
		if (IsVisible())
		{
			s_instance.ChangePressState(isPressed);
		}
	}

	public static void SetSkipActivate(bool isActive)
	{
		if (!(s_instance == null))
		{
			s_instance.ChangeSkipActivateState(isActive);
		}
	}

	public void OnClickSkipButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			EventEngine eventEngine = null;
			EventEngine.GetInstance()?.TouchSkipKey();
		}
	}
}
