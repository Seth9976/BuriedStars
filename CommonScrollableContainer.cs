using UnityEngine;
using UnityEngine.EventSystems;

public class CommonScrollableContainer : MonoBehaviour
{
	public enum ScrollType
	{
		Horizontal,
		Vertical
	}

	private enum ScrollButton
	{
		None,
		LeftUp,
		RightDown,
		ToFirst,
		ToLast
	}

	private enum ButtonEvent
	{
		None,
		Click,
		Down,
		Up,
		Enter,
		Exit
	}

	private enum ScrollEvent
	{
		None,
		Click,
		Pushing,
		PrePushing,
		PushginOut,
		IngnoreClick,
		JumpingToTarget
	}

	public GameObject m_Viewport;

	public GameObject m_Container;

	public ScrollType m_Type;

	public GameObject m_ButtonLeftUp;

	public GameObject m_ButtonRightDown;

	public GameObject m_ButtonFirst;

	public GameObject m_ButtonLast;

	public bool m_EnableButtonPushing = true;

	public float m_BoundPushingTime = 0.2f;

	public float m_Speed = 1500f;

	public float m_PageSize = 300f;

	public bool m_SelfInitailize = true;

	private RectTransform m_trContainer;

	private bool m_isScrollable;

	private float m_fScrollPos_Max;

	private float m_fScrollPos_Target;

	private ScrollButton m_curScrollButton;

	private ScrollEvent m_curScrollEvent;

	private double m_dbButtonDownTime;

	private const float c_fJumpDuration = 0.2f;

	private float m_fJumpTarget;

	private float m_fJumpSpeed;

	public bool isScrollable => m_isScrollable;

	public float scrollPosMax => m_fScrollPos_Max;

	public float scrollPos
	{
		get
		{
			return (m_trContainer == null) ? 0f : ((m_Type != ScrollType.Horizontal) ? m_trContainer.anchoredPosition.y : (0f - m_trContainer.anchoredPosition.x));
		}
		set
		{
			if (!(m_trContainer == null))
			{
				float num = Mathf.Clamp(value, 0f, m_fScrollPos_Max);
				m_trContainer.anchoredPosition = ((m_Type != ScrollType.Horizontal) ? new Vector2(m_trContainer.anchoredPosition.x, num) : new Vector2(0f - num, m_trContainer.anchoredPosition.y));
			}
		}
	}

	private void Start()
	{
		InitScrollButtonEventTrigger();
		if (m_SelfInitailize)
		{
			InitScrollableContanier();
		}
	}

	private void OnDestroy()
	{
		m_trContainer = null;
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && base.enabled)
		{
			ProcScrollEvent();
		}
	}

	public void InitScrollableContanier()
	{
		if (!(m_Viewport == null) && !(m_Container == null))
		{
			RectTransform component = m_Viewport.GetComponent<RectTransform>();
			RectTransform component2 = m_Container.GetComponent<RectTransform>();
			m_trContainer = component2;
			float num = ((m_Type != ScrollType.Horizontal) ? m_trContainer.rect.height : m_trContainer.rect.width);
			float num2 = ((m_Type != ScrollType.Horizontal) ? component.rect.height : component.rect.width);
			m_isScrollable = num > num2;
			m_fScrollPos_Max = ((!m_isScrollable) ? 0f : (num - num2));
		}
	}

	private void InitScrollButtonEventTrigger()
	{
		if (m_ButtonLeftUp != null)
		{
			EventTrigger component = m_ButtonLeftUp.GetComponent<EventTrigger>();
			if (component == null)
			{
				component = m_ButtonLeftUp.AddComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerClick;
				entry.callback = new EventTrigger.TriggerEvent();
				entry.callback.AddListener(OnEventProc_ScrollButtonClick_Left);
				component.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback = new EventTrigger.TriggerEvent();
				entry.callback.AddListener(OnEventProc_ScrollButtonDown_Left);
				component.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerUp;
				entry.callback = new EventTrigger.TriggerEvent();
				entry.callback.AddListener(OnEventProc_ScrollButtonUp_Left);
				component.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback = new EventTrigger.TriggerEvent();
				entry.callback.AddListener(OnEventProc_ScrollButtonEnter_Left);
				component.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback = new EventTrigger.TriggerEvent();
				entry.callback.AddListener(OnEventProc_ScrollButtonExit_Left);
				component.triggers.Add(entry);
			}
		}
		if (m_ButtonRightDown != null)
		{
			EventTrigger component2 = m_ButtonRightDown.GetComponent<EventTrigger>();
			if (component2 == null)
			{
				component2 = m_ButtonRightDown.AddComponent<EventTrigger>();
				EventTrigger.Entry entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerClick;
				entry2.callback = new EventTrigger.TriggerEvent();
				entry2.callback.AddListener(OnEventProc_ScrollButtonClick_Right);
				component2.triggers.Add(entry2);
				entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerDown;
				entry2.callback = new EventTrigger.TriggerEvent();
				entry2.callback.AddListener(OnEventProc_ScrollButtonDown_Right);
				component2.triggers.Add(entry2);
				entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerUp;
				entry2.callback = new EventTrigger.TriggerEvent();
				entry2.callback.AddListener(OnEventProc_ScrollButtonUp_Right);
				component2.triggers.Add(entry2);
				entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerEnter;
				entry2.callback = new EventTrigger.TriggerEvent();
				entry2.callback.AddListener(OnEventProc_ScrollButtonEnter_Right);
				component2.triggers.Add(entry2);
				entry2 = new EventTrigger.Entry();
				entry2.eventID = EventTriggerType.PointerExit;
				entry2.callback = new EventTrigger.TriggerEvent();
				entry2.callback.AddListener(OnEventProc_ScrollButtonExit_Right);
				component2.triggers.Add(entry2);
			}
		}
		if (m_ButtonFirst != null)
		{
			GameGlobalUtil.AddEventTrigger(m_ButtonFirst, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_ToFirst);
		}
		if (m_ButtonLast != null)
		{
			GameGlobalUtil.AddEventTrigger(m_ButtonLast, EventTriggerType.PointerClick, OnEventProc_ScrollButtonClick_ToLast);
		}
	}

	private void OnEventProc_ScrollButtonClick_Left(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.LeftUp, ButtonEvent.Click);
	}

	private void OnEventProc_ScrollButtonDown_Left(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.LeftUp, ButtonEvent.Down);
	}

	private void OnEventProc_ScrollButtonUp_Left(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.LeftUp, ButtonEvent.Up);
	}

	private void OnEventProc_ScrollButtonEnter_Left(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.LeftUp, ButtonEvent.Enter);
	}

	private void OnEventProc_ScrollButtonExit_Left(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.LeftUp, ButtonEvent.Exit);
	}

	private void OnEventProc_ScrollButtonClick_Right(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.RightDown, ButtonEvent.Click);
	}

	private void OnEventProc_ScrollButtonDown_Right(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.RightDown, ButtonEvent.Down);
	}

	private void OnEventProc_ScrollButtonUp_Right(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.RightDown, ButtonEvent.Up);
	}

	private void OnEventProc_ScrollButtonEnter_Right(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.RightDown, ButtonEvent.Enter);
	}

	private void OnEventProc_ScrollButtonExit_Right(BaseEventData eventData)
	{
		OnEventProc_ScrollButton(ScrollButton.RightDown, ButtonEvent.Exit);
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
				m_curScrollButton = button;
				float num = scrollPos;
				m_fScrollPos_Target = ((m_curScrollButton == ScrollButton.LeftUp) ? (num - m_PageSize) : ((m_curScrollButton != ScrollButton.RightDown) ? num : (num + m_PageSize)));
				m_fScrollPos_Target = Mathf.Clamp(m_fScrollPos_Target, 0f, m_fScrollPos_Max);
				m_curScrollEvent = ((num != m_fScrollPos_Target) ? ScrollEvent.Click : ScrollEvent.None);
			}
			else
			{
				m_curScrollEvent = ScrollEvent.None;
			}
			break;
		case ButtonEvent.Down:
			if (m_EnableButtonPushing)
			{
				m_curScrollButton = button;
				m_curScrollEvent = ScrollEvent.PrePushing;
				m_dbButtonDownTime = 0.0;
			}
			break;
		case ButtonEvent.Up:
			if (m_EnableButtonPushing)
			{
				m_curScrollButton = ScrollButton.None;
				m_curScrollEvent = ((m_curScrollEvent == ScrollEvent.Pushing) ? ScrollEvent.IngnoreClick : ScrollEvent.None);
			}
			break;
		case ButtonEvent.Enter:
			if (m_EnableButtonPushing && m_curScrollButton == button && m_curScrollEvent == ScrollEvent.PushginOut)
			{
				m_curScrollEvent = ScrollEvent.PrePushing;
				m_dbButtonDownTime = 0.0;
			}
			break;
		case ButtonEvent.Exit:
			if (m_EnableButtonPushing && m_curScrollButton == button && (m_curScrollEvent == ScrollEvent.PrePushing || m_curScrollEvent == ScrollEvent.Pushing))
			{
				m_curScrollEvent = ScrollEvent.PushginOut;
			}
			break;
		}
	}

	private void OnEventProc_ScrollButtonClick_ToFirst(BaseEventData eventData)
	{
		if (m_isScrollable)
		{
			m_fJumpTarget = 0f;
			float num = scrollPos;
			if (!GameGlobalUtil.IsAlmostSame(num, m_fJumpTarget))
			{
				m_fJumpSpeed = (m_fJumpTarget - num) / 0.2f;
				m_curScrollButton = ScrollButton.ToFirst;
				m_curScrollEvent = ScrollEvent.JumpingToTarget;
			}
		}
	}

	private void OnEventProc_ScrollButtonClick_ToLast(BaseEventData eventData)
	{
		if (m_isScrollable)
		{
			m_fJumpTarget = m_fScrollPos_Max;
			float fLeft = scrollPos;
			if (!GameGlobalUtil.IsAlmostSame(fLeft, m_fJumpTarget))
			{
				m_fJumpSpeed = (m_fJumpTarget - scrollPos) / 0.2f;
				m_curScrollButton = ScrollButton.ToLast;
				m_curScrollEvent = ScrollEvent.JumpingToTarget;
			}
		}
	}

	private void ProcScrollEvent()
	{
		if (!m_isScrollable || m_curScrollButton == ScrollButton.None)
		{
			return;
		}
		switch (m_curScrollEvent)
		{
		case ScrollEvent.Click:
		{
			float num2 = scrollPos;
			float num3 = m_Speed * Time.deltaTime;
			if (m_fScrollPos_Target > num2)
			{
				num2 += num3;
				if (num2 >= m_fScrollPos_Target)
				{
					num2 = m_fScrollPos_Target;
					m_curScrollEvent = ScrollEvent.None;
				}
				scrollPos = num2;
			}
			else if (m_fScrollPos_Target < num2)
			{
				num2 -= num3;
				if (num2 <= m_fScrollPos_Target)
				{
					num2 = m_fScrollPos_Target;
					m_curScrollEvent = ScrollEvent.None;
				}
				scrollPos = num2;
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
			float num4 = scrollPos;
			float num5 = m_Speed * Time.deltaTime;
			num4 = ((m_curScrollButton != ScrollButton.LeftUp) ? (num4 + num5) : (num4 - num5));
			num4 = Mathf.Clamp(num4, 0f, m_fScrollPos_Max);
			scrollPos = num4;
			break;
		}
		case ScrollEvent.JumpingToTarget:
		{
			float num = scrollPos + m_fJumpSpeed * Time.deltaTime;
			if ((m_fJumpSpeed > 0f && num >= m_fJumpTarget) || (m_fJumpSpeed < 0f && num <= m_fJumpTarget) || GameGlobalUtil.IsAlmostSame(m_fJumpSpeed, 0f))
			{
				scrollPos = m_fJumpTarget;
				m_curScrollButton = ScrollButton.None;
				m_curScrollEvent = ScrollEvent.None;
			}
			else
			{
				scrollPos = num;
			}
			break;
		}
		case ScrollEvent.PushginOut:
		case ScrollEvent.IngnoreClick:
			break;
		}
	}
}
