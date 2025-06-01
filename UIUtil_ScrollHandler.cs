using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class UIUtil_ScrollHandler
{
	public enum ScrollType
	{
		Horizontal,
		Vertical
	}

	public enum ScrollButton
	{
		Prev,
		Next,
		ToFirst,
		ToLast
	}

	public enum ButtonEvent
	{
		None,
		Click,
		Down,
		Up,
		Enter,
		Exit
	}

	public enum ScrollEvent
	{
		None,
		Click,
		Pushing,
		PrePushing,
		PushingOut,
		IngnoreClick,
		JumpingToTarget
	}

	public bool m_EnableButtonPushing = true;

	public float m_BoundPushingTime = 0.2f;

	public float m_Speed = 1500f;

	protected ScrollType m_ScrollType;

	protected ScrollRect m_ScrollRect;

	protected RectTransform m_rtContainer;

	protected Button[] m_ScrollButtons;

	private bool m_isScrollable;

	private float m_ScrollPos_Max;

	private float m_ScrollPos_Target;

	private float m_PageLength = 1f;

	private int m_curScrollButtonIdx = -1;

	private ScrollEvent m_curScrollEvent;

	private double m_dbButtonDownTime;

	private const float c_fJumpDuration = 0.2f;

	private const float c_fMinJumpSpeed = 100f;

	private float m_fJumpTarget;

	private float m_fJumpSpeed;

	private float m_jumpAccel;

	private float m_jumpDistance;

	private int m_jumpDirection;

	private int m_jumpSpeedType;

	private float m_jumpPlayTime;

	private float m_jumpRemainedTime;

	private float m_prevScrollPos;

	private GameDefine.EventProc m_onScrollButtonEvent;

	public bool isScrollable => m_isScrollable;

	public float scrollPos_Max => m_ScrollPos_Max;

	public float scrollPos_Target => m_ScrollPos_Target;

	public float pageLength
	{
		get
		{
			return m_PageLength;
		}
		set
		{
			m_PageLength = value;
		}
	}

	public float jumpTarget => m_fJumpTarget;

	public float scrollPos
	{
		get
		{
			float value = ((m_rtContainer == null) ? 0f : ((m_ScrollType != ScrollType.Horizontal) ? m_rtContainer.anchoredPosition.y : (0f - m_rtContainer.anchoredPosition.x)));
			return Mathf.Clamp(value, 0f, m_ScrollPos_Max);
		}
		set
		{
			if (!(m_rtContainer == null))
			{
				float num = Mathf.Clamp(value, 0f, m_ScrollPos_Max);
				m_rtContainer.anchoredPosition = ((m_ScrollType != ScrollType.Horizontal) ? new Vector2(m_rtContainer.anchoredPosition.x, num) : new Vector2(0f - num, m_rtContainer.anchoredPosition.y));
			}
		}
	}

	public bool IsScrolling => m_curScrollEvent != ScrollEvent.None || m_prevScrollPos != scrollPos;

	public ScrollEvent CurScrollEvent => m_curScrollEvent;

	public GameDefine.EventProc onScrollButtonEvent
	{
		get
		{
			return m_onScrollButtonEvent;
		}
		set
		{
			m_onScrollButtonEvent = value;
		}
	}

	public void Init(ScrollType _scrollType, ScrollRect _scrollRect, RectTransform _rtContainer, Button _sbPrev = null, Button _sbNext = null, Button _sbFirst = null, Button _sbLast = null)
	{
		m_ScrollType = _scrollType;
		m_ScrollRect = _scrollRect;
		m_rtContainer = _rtContainer;
		m_ScrollRect.scrollSensitivity = 0f;
		int length = Enum.GetValues(typeof(ScrollButton)).Length;
		m_ScrollButtons = new Button[length];
		m_ScrollButtons[0] = _sbPrev;
		m_ScrollButtons[1] = _sbNext;
		m_ScrollButtons[2] = _sbFirst;
		m_ScrollButtons[3] = _sbLast;
		InitScrollButtonEventTrigger();
		ResetScrollRange();
		if (m_ScrollRect != null)
		{
			m_ScrollRect.inertia = false;
		}
	}

	public void ReleaseScroll()
	{
		m_rtContainer = null;
		m_ScrollButtons = null;
	}

	public void ResetScrollRange()
	{
		float num = ((m_ScrollType != ScrollType.Horizontal) ? m_ScrollRect.viewport.rect.height : m_ScrollRect.viewport.rect.width);
		float num2 = ((m_ScrollType != ScrollType.Horizontal) ? m_rtContainer.rect.height : m_rtContainer.rect.width);
		m_isScrollable = num2 > num;
		m_ScrollPos_Max = ((!m_isScrollable) ? 0f : (num2 - num));
		m_ScrollPos_Target = 0f;
		m_fJumpTarget = 0f;
		scrollPos = 0f;
		m_prevScrollPos = 0f;
	}

	private void InitScrollButtonEventTrigger()
	{
		Button button = m_ScrollButtons[0];
		if (button != null)
		{
			EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = button.gameObject.AddComponent<EventTrigger>();
			}
			eventTrigger.triggers.Clear();
			GameGlobalUtil.AddEventTrigger(eventTrigger, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_Prev, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger, EventTriggerType.PointerDown, OnEventProc_ScrollButtonDown_Prev, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, OnEventProc_ScrollButtonUp_Prev, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, OnEventProc_ScrollButtonEnter_Prev, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, OnEventProc_ScrollButtonExit_Prev, isCheckOldEvent: false);
		}
		button = m_ScrollButtons[1];
		if (button != null)
		{
			EventTrigger eventTrigger2 = button.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger2 == null)
			{
				eventTrigger2 = button.gameObject.AddComponent<EventTrigger>();
			}
			eventTrigger2.triggers.Clear();
			GameGlobalUtil.AddEventTrigger(eventTrigger2, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_Next, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger2, EventTriggerType.PointerDown, OnEventProc_ScrollButtonDown_Next, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger2, EventTriggerType.PointerUp, OnEventProc_ScrollButtonUp_Next, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger2, EventTriggerType.PointerEnter, OnEventProc_ScrollButtonEnter_Next, isCheckOldEvent: false);
			GameGlobalUtil.AddEventTrigger(eventTrigger2, EventTriggerType.PointerExit, OnEventProc_ScrollButtonExit_Next, isCheckOldEvent: false);
		}
		button = m_ScrollButtons[2];
		if (button != null)
		{
			GameGlobalUtil.AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_ToFirst);
		}
		button = m_ScrollButtons[3];
		if (button != null)
		{
			GameGlobalUtil.AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_ToLast);
		}
		GameGlobalUtil.AddEventTrigger(m_ScrollRect.gameObject, EventTriggerType.PointerDown, OnPointDown);
	}

	private void OnEventProc_ScrollButtonClick_Prev(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Prev, ButtonEvent.Click);
	}

	private void OnEventProc_ScrollButtonDown_Prev(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Prev, ButtonEvent.Down);
	}

	private void OnEventProc_ScrollButtonUp_Prev(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Prev, ButtonEvent.Up);
	}

	private void OnEventProc_ScrollButtonEnter_Prev(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Prev, ButtonEvent.Enter);
	}

	private void OnEventProc_ScrollButtonExit_Prev(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Prev, ButtonEvent.Exit);
	}

	private void OnEventProc_ScrollButtonClick_Next(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Next, ButtonEvent.Click);
	}

	private void OnEventProc_ScrollButtonDown_Next(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Next, ButtonEvent.Down);
	}

	private void OnEventProc_ScrollButtonUp_Next(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Next, ButtonEvent.Up);
	}

	private void OnEventProc_ScrollButtonEnter_Next(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Next, ButtonEvent.Enter);
	}

	private void OnEventProc_ScrollButtonExit_Next(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.Next, ButtonEvent.Exit);
	}

	private void OnEventProc_ScrollButton(ScrollButton button, ButtonEvent eventType)
	{
		if (!m_isScrollable)
		{
			return;
		}
		switch (eventType)
		{
		case ButtonEvent.Click:
			if (m_curScrollEvent != ScrollEvent.IngnoreClick)
			{
				m_curScrollButtonIdx = (int)button;
				float num = scrollPos;
				m_ScrollPos_Target = ((m_curScrollButtonIdx == 0) ? (num - m_PageLength) : ((m_curScrollButtonIdx != 1) ? num : (num + m_PageLength)));
				m_ScrollPos_Target = Mathf.Clamp(m_ScrollPos_Target, 0f, m_ScrollPos_Max);
				m_curScrollEvent = ((num != m_ScrollPos_Target) ? ScrollEvent.Click : ScrollEvent.None);
			}
			else
			{
				m_curScrollEvent = ScrollEvent.None;
			}
			break;
		case ButtonEvent.Down:
			if (m_EnableButtonPushing)
			{
				m_curScrollButtonIdx = (int)button;
				m_curScrollEvent = ScrollEvent.PrePushing;
				m_dbButtonDownTime = 0.0;
			}
			break;
		case ButtonEvent.Up:
			if (m_EnableButtonPushing)
			{
				m_curScrollButtonIdx = -1;
				m_curScrollEvent = ((m_curScrollEvent == ScrollEvent.Pushing) ? ScrollEvent.IngnoreClick : ScrollEvent.None);
			}
			break;
		case ButtonEvent.Enter:
			if (m_EnableButtonPushing && m_curScrollButtonIdx == (int)button && m_curScrollEvent == ScrollEvent.PushingOut)
			{
				m_curScrollEvent = ScrollEvent.PrePushing;
				m_dbButtonDownTime = 0.0;
			}
			break;
		case ButtonEvent.Exit:
			if (m_EnableButtonPushing && m_curScrollButtonIdx == (int)button && (m_curScrollEvent == ScrollEvent.PrePushing || m_curScrollEvent == ScrollEvent.Pushing))
			{
				m_curScrollEvent = ScrollEvent.PushingOut;
			}
			break;
		}
		if (m_onScrollButtonEvent != null)
		{
			m_onScrollButtonEvent(button, eventType);
		}
	}

	private void OnEventProc_ScrollButtonClick_ToFirst(BaseEventData eventData)
	{
		if (m_isScrollable)
		{
			float fLeft = scrollPos;
			if (!GameGlobalUtil.IsAlmostSame(fLeft, m_fJumpTarget))
			{
				ScrollToTargetPos(0f);
				m_curScrollButtonIdx = 2;
			}
		}
	}

	private void OnEventProc_ScrollButtonClick_ToLast(BaseEventData eventData)
	{
		if (m_isScrollable)
		{
			float fLeft = scrollPos;
			if (!GameGlobalUtil.IsAlmostSame(fLeft, m_fJumpTarget))
			{
				ScrollToTargetPos(m_ScrollPos_Max);
				m_curScrollButtonIdx = 3;
			}
		}
	}

	public void OnPointDown(BaseEventData eventData)
	{
		if (m_curScrollEvent != ScrollEvent.None)
		{
			m_curScrollEvent = ScrollEvent.None;
			m_curScrollButtonIdx = -1;
			m_fJumpTarget = scrollPos;
		}
	}

	public void Update()
	{
		m_prevScrollPos = scrollPos;
		ProcScrollEvent();
	}

	private void ProcScrollEvent()
	{
		if (!m_isScrollable)
		{
			return;
		}
		switch (m_curScrollEvent)
		{
		case ScrollEvent.Click:
		{
			float num3 = scrollPos;
			float num4 = m_Speed * Time.deltaTime;
			if (m_ScrollPos_Target > num3)
			{
				num3 += num4;
				if (num3 >= m_ScrollPos_Target)
				{
					num3 = m_ScrollPos_Target;
					m_curScrollEvent = ScrollEvent.None;
				}
				scrollPos = num3;
			}
			else if (m_ScrollPos_Target < num3)
			{
				num3 -= num4;
				if (num3 <= m_ScrollPos_Target)
				{
					num3 = m_ScrollPos_Target;
					m_curScrollEvent = ScrollEvent.None;
				}
				scrollPos = num3;
			}
			else
			{
				m_curScrollEvent = ScrollEvent.None;
			}
			break;
		}
		case ScrollEvent.PrePushing:
			m_dbButtonDownTime += Time.deltaTime;
			if (m_dbButtonDownTime >= (double)m_BoundPushingTime)
			{
				m_dbButtonDownTime = 0.0;
				m_curScrollEvent = ScrollEvent.Pushing;
			}
			break;
		case ScrollEvent.Pushing:
		{
			float num = scrollPos;
			float num2 = m_Speed * Time.deltaTime;
			num = ((m_curScrollButtonIdx != 0) ? (num + num2) : (num - num2));
			num = Mathf.Clamp(num, 0f, m_ScrollPos_Max);
			scrollPos = num;
			break;
		}
		case ScrollEvent.JumpingToTarget:
			ProcScrollEvent_JumpingToTarget();
			break;
		case ScrollEvent.PushingOut:
		case ScrollEvent.IngnoreClick:
			break;
		}
	}

	private void ProcScrollEvent_JumpingToTarget()
	{
		float deltaTime = Time.deltaTime;
		m_jumpRemainedTime -= deltaTime;
		if (m_jumpRemainedTime <= 0f)
		{
			scrollPos = m_fJumpTarget;
			m_curScrollButtonIdx = -1;
			m_curScrollEvent = ScrollEvent.None;
			return;
		}
		float num = 0f;
		if (m_jumpSpeedType == 3 && m_jumpRemainedTime <= m_jumpPlayTime * 0.5f && m_jumpAccel > 0f)
		{
			float num2 = m_jumpPlayTime * 0.5f - m_jumpRemainedTime;
			float num3 = deltaTime - num2;
			if (!GameGlobalUtil.IsAlmostSame(num3, 0f))
			{
				m_fJumpSpeed += m_jumpAccel * num3;
				num = (float)m_jumpDirection * (m_fJumpSpeed * num3);
				scrollPos += num;
			}
			m_jumpAccel = 0f - m_jumpAccel;
			if (!GameGlobalUtil.IsAlmostSame(num2, 0f))
			{
				m_fJumpSpeed += m_jumpAccel * num2;
				num = (float)m_jumpDirection * (m_fJumpSpeed * num2);
				scrollPos += num;
			}
		}
		else
		{
			num = (float)m_jumpDirection * (m_fJumpSpeed * deltaTime);
			scrollPos += num;
			m_fJumpSpeed += m_jumpAccel * deltaTime;
			if (m_fJumpSpeed <= 0f)
			{
				m_fJumpSpeed = 0.01f;
			}
		}
	}

	public void ScrollToTargetPos(float fTargetPos, float duration = 0.2f, int speedType = 0)
	{
		if (!m_isScrollable)
		{
			return;
		}
		fTargetPos = Mathf.Clamp(0f - fTargetPos, 0f, m_ScrollPos_Max);
		float num = scrollPos;
		if (GameGlobalUtil.IsAlmostSame(fTargetPos, num))
		{
			return;
		}
		m_fJumpTarget = fTargetPos;
		if (GameGlobalUtil.IsAlmostSame(num, m_fJumpTarget))
		{
			return;
		}
		m_curScrollButtonIdx = -1;
		m_curScrollEvent = ScrollEvent.JumpingToTarget;
		m_jumpSpeedType = speedType;
		m_jumpDistance = m_fJumpTarget - scrollPos;
		m_jumpDirection = ((m_jumpDistance >= 0f) ? 1 : (-1));
		m_jumpDistance = Mathf.Abs(m_jumpDistance);
		m_jumpPlayTime = duration;
		m_jumpRemainedTime = duration;
		if (!(m_jumpPlayTime < 0f) && !GameGlobalUtil.IsAlmostSame(m_jumpPlayTime, 0f))
		{
			switch (m_jumpSpeedType)
			{
			case 1:
				m_jumpAccel = m_jumpDistance * 2f / (m_jumpPlayTime * m_jumpPlayTime);
				m_fJumpSpeed = 0f;
				break;
			case 2:
				m_jumpAccel = 0f - m_jumpDistance * 2f / (m_jumpPlayTime * m_jumpPlayTime);
				m_fJumpSpeed = m_jumpDistance * 2f / m_jumpPlayTime;
				break;
			case 3:
				m_jumpAccel = m_jumpDistance * 2f / (m_jumpPlayTime * m_jumpPlayTime) * 2f;
				m_fJumpSpeed = 0f;
				break;
			default:
				m_jumpAccel = 0f;
				m_fJumpSpeed = m_jumpDistance / m_jumpPlayTime;
				break;
			}
		}
	}

	public void ScrollByDirection(bool isToNext, float fMoveFactor = 1f)
	{
		float num = scrollPos;
		float num2 = m_Speed * Time.deltaTime * fMoveFactor;
		num = (isToNext ? (num + num2) : (num - num2));
		num = Mathf.Clamp(num, 0f, m_ScrollPos_Max);
		scrollPos = num;
		m_curScrollEvent = ScrollEvent.None;
	}

	public void SetScrollPos(float _scrollPos)
	{
		scrollPos = _scrollPos;
		m_prevScrollPos = scrollPos;
		m_curScrollEvent = ScrollEvent.None;
	}
}
