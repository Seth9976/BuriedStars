using System.Collections.Generic;
using GameEvent;
using UnityEngine;

public class FloatingUIRoot : MonoBehaviour
{
	public enum EventType
	{
		None,
		Disappear,
		Move,
		Rotation,
		Zoom,
		Motion
	}

	public class EventBase
	{
		private EventType m_Type;

		private List<IFloatingUIObject> m_Members = new List<IFloatingUIObject>();

		public List<IFloatingUIObject> members => m_Members;

		public EventBase(EventType type)
		{
			m_Type = type;
		}

		public virtual void Start()
		{
		}

		public virtual bool UpdateEvent()
		{
			return true;
		}

		public void AddMember(IFloatingUIObject foObj)
		{
			if (!m_Members.Contains(foObj))
			{
				m_Members.Add(foObj);
			}
			if (!foObj.foEvents.Contains(this))
			{
				foObj.foEvents.Add(this);
			}
		}

		public void RemoveMember(IFloatingUIObject foObj, bool isRemoveEvent)
		{
			int num = m_Members.IndexOf(foObj);
			if (num >= 0 && isRemoveEvent)
			{
				m_Members.Remove(foObj);
			}
			if (foObj.foEvents.Contains(this))
			{
				foObj.foEvents.Remove(this);
			}
		}
	}

	private class EventDisappear : EventBase
	{
		public float m_SpeedRate = 1f;

		public EventDisappear()
			: base(EventType.Disappear)
		{
		}

		public override void Start()
		{
			int count = base.members.Count;
			if (count > 0)
			{
				EventEngine instance = EventEngine.GetInstance();
				float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
				IFloatingUIObject floatingUIObject = null;
				string strMot = GameDefine.UIAnimationState.disappear.ToString();
				for (int i = 0; i < count; i++)
				{
					floatingUIObject = base.members[i];
					floatingUIObject.foAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(floatingUIObject.foGameObject, strMot, m_SpeedRate * num);
				}
			}
		}

		public override bool UpdateEvent()
		{
			int count = base.members.Count;
			if (count <= 0)
			{
				return true;
			}
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
			IFloatingUIObject floatingUIObject = null;
			string name = GameDefine.UIAnimationState.disappear.ToString();
			for (int i = 0; i < count; i++)
			{
				floatingUIObject = base.members[i];
				if (floatingUIObject != null && !(floatingUIObject.foAnimator == null))
				{
					floatingUIObject.foAnimator.speed = m_SpeedRate * num;
					AnimatorStateInfo currentAnimatorStateInfo = floatingUIObject.foAnimator.GetCurrentAnimatorStateInfo(0);
					if (!currentAnimatorStateInfo.IsName(name) || currentAnimatorStateInfo.normalizedTime < 0.99f)
					{
						return false;
					}
					floatingUIObject.foHandler.DestoryContent(floatingUIObject.foGameObject, needContainCheck: false, removeEvents: false);
					base.members[i] = null;
				}
			}
			return true;
		}
	}

	private class EventMove : EventBase
	{
		public Vector2 m_targetMoveFactor;

		public float m_playTime;

		public int m_SpeedType;

		private float m_distance;

		private float m_remainedTime;

		private Vector2 m_direction;

		private Vector2 m_remainedMoveFactor;

		private float m_accel;

		private float m_speed;

		public EventMove()
			: base(EventType.Move)
		{
		}

		public override void Start()
		{
			m_distance = m_targetMoveFactor.magnitude;
			m_direction = m_targetMoveFactor.normalized;
			m_remainedTime = m_playTime;
			m_remainedMoveFactor = m_targetMoveFactor;
			SetFirstSpeedAccel(m_distance, m_playTime, (SPEED_TYPE)m_SpeedType, out m_speed, out m_accel);
		}

		public override bool UpdateEvent()
		{
			int count = base.members.Count;
			if (count <= 0)
			{
				return true;
			}
			if (m_targetMoveFactor == Vector2.zero)
			{
				return true;
			}
			IFloatingUIObject floatingUIObject = null;
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
			if (m_playTime <= 0f)
			{
				for (int i = 0; i < count; i++)
				{
					floatingUIObject = base.members[i];
					floatingUIObject.foRectTransform.anchoredPosition += m_targetMoveFactor;
				}
				return true;
			}
			m_remainedTime -= num;
			if (m_remainedTime <= 0f)
			{
				for (int j = 0; j < count; j++)
				{
					floatingUIObject = base.members[j];
					floatingUIObject.foRectTransform.anchoredPosition += m_remainedMoveFactor;
				}
				return true;
			}
			Vector2 vector;
			if (m_SpeedType == 3 && m_remainedTime <= m_playTime * 0.5f && m_accel > 0f)
			{
				float num2 = m_playTime * 0.5f - m_remainedTime;
				float num3 = num - num2;
				if (!GameGlobalUtil.IsAlmostSame(num3, 0f))
				{
					m_speed += m_accel * num3;
					vector = m_direction * (m_speed * num3);
					for (int k = 0; k < count; k++)
					{
						floatingUIObject = base.members[k];
						floatingUIObject.foRectTransform.anchoredPosition += vector;
					}
					m_remainedMoveFactor -= vector;
				}
				m_accel = 0f - m_accel;
				if (!GameGlobalUtil.IsAlmostSame(num2, 0f))
				{
					m_speed += m_accel * num2;
					vector = m_direction * (m_speed * num2);
					for (int l = 0; l < count; l++)
					{
						floatingUIObject = base.members[l];
						floatingUIObject.foRectTransform.anchoredPosition += vector;
					}
					m_remainedMoveFactor -= vector;
				}
				return false;
			}
			vector = m_direction * (m_speed * num);
			for (int m = 0; m < count; m++)
			{
				floatingUIObject = base.members[m];
				floatingUIObject.foRectTransform.anchoredPosition += vector;
			}
			m_remainedMoveFactor -= vector;
			m_speed += m_accel * num;
			if (m_speed <= 0f)
			{
				m_speed = 0.01f;
			}
			return false;
		}
	}

	private class EventRotate : EventBase
	{
		public Vector3 m_targetRotateFactor;

		public float m_playTime;

		public int m_SpeedType;

		private float m_distance;

		private float m_remainedTime;

		private Vector3 m_direction;

		private Vector3 m_remainedRotateFactor;

		private float m_accel;

		private float m_speed;

		public EventRotate()
			: base(EventType.Rotation)
		{
		}

		public override void Start()
		{
			m_distance = m_targetRotateFactor.magnitude;
			m_direction = m_targetRotateFactor.normalized;
			m_remainedTime = m_playTime;
			m_remainedRotateFactor = m_targetRotateFactor;
			SetFirstSpeedAccel(m_distance, m_playTime, (SPEED_TYPE)m_SpeedType, out m_speed, out m_accel);
		}

		public override bool UpdateEvent()
		{
			int count = base.members.Count;
			if (count <= 0)
			{
				return true;
			}
			if (m_targetRotateFactor == Vector3.zero)
			{
				return true;
			}
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
			IFloatingUIObject floatingUIObject = null;
			if (m_playTime <= 0f)
			{
				for (int i = 0; i < count; i++)
				{
					floatingUIObject = base.members[i];
					floatingUIObject.foRotateAngle += m_targetRotateFactor;
					floatingUIObject.foRectTransform.localRotation = Quaternion.Euler(floatingUIObject.foRotateAngle);
				}
				return true;
			}
			m_remainedTime -= num;
			if (m_remainedTime <= 0f)
			{
				for (int j = 0; j < count; j++)
				{
					floatingUIObject = base.members[j];
					floatingUIObject.foRotateAngle += m_remainedRotateFactor;
					floatingUIObject.foRectTransform.localRotation = Quaternion.Euler(floatingUIObject.foRotateAngle);
				}
				return true;
			}
			Vector3 vector;
			if (m_SpeedType == 3 && m_remainedTime <= m_playTime * 0.5f && m_accel > 0f)
			{
				float num2 = m_playTime * 0.5f - m_remainedTime;
				float num3 = num - num2;
				if (!GameGlobalUtil.IsAlmostSame(num3, 0f))
				{
					m_speed += m_accel * num3;
					vector = m_direction * (m_speed * num3);
					for (int k = 0; k < count; k++)
					{
						floatingUIObject = base.members[k];
						floatingUIObject.foRotateAngle += vector;
						floatingUIObject.foRectTransform.localRotation = Quaternion.Euler(floatingUIObject.foRotateAngle);
					}
					m_remainedRotateFactor -= vector;
				}
				m_accel = 0f - m_accel;
				if (!GameGlobalUtil.IsAlmostSame(num2, 0f))
				{
					m_speed += m_accel * num2;
					vector = m_direction * (m_speed * num3);
					for (int l = 0; l < count; l++)
					{
						floatingUIObject = base.members[l];
						floatingUIObject.foRotateAngle += vector;
						floatingUIObject.foRectTransform.localRotation = Quaternion.Euler(floatingUIObject.foRotateAngle);
					}
					m_remainedRotateFactor -= vector;
				}
				return false;
			}
			vector = m_direction * (m_speed * num);
			for (int m = 0; m < count; m++)
			{
				floatingUIObject = base.members[m];
				floatingUIObject.foRotateAngle += vector;
				floatingUIObject.foRectTransform.localRotation = Quaternion.Euler(floatingUIObject.foRotateAngle);
			}
			m_remainedRotateFactor -= vector;
			m_speed += m_accel * num;
			if (m_speed <= 0f)
			{
				m_speed = 0.01f;
			}
			return false;
		}
	}

	public class ScalingParams
	{
		public float m_curScaleFactor = 1f;

		public float m_accel;

		public float m_speed;

		public Vector3 m_baseScale;
	}

	private class EventScaling : EventBase
	{
		public float m_targetScaleFactor = 1f;

		public float m_playTime;

		public int m_SpeedType;

		private float m_remainedTime;

		private bool m_isBoundedAcDecel;

		public EventScaling()
			: base(EventType.Zoom)
		{
		}

		public override void Start()
		{
			m_remainedTime = m_playTime;
			int count = base.members.Count;
			IFloatingUIObject floatingUIObject = null;
			ScalingParams scalingParams = null;
			for (int i = 0; i < count; i++)
			{
				floatingUIObject = base.members[i];
				scalingParams = floatingUIObject.foScalingParmas;
				SetFirstSpeedAccel(m_targetScaleFactor - scalingParams.m_curScaleFactor, m_playTime, (SPEED_TYPE)m_SpeedType, out scalingParams.m_speed, out scalingParams.m_accel);
			}
		}

		public override bool UpdateEvent()
		{
			int count = base.members.Count;
			if (count <= 0)
			{
				return true;
			}
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
			IFloatingUIObject floatingUIObject = null;
			ScalingParams scalingParams = null;
			if (m_playTime <= 0f)
			{
				for (int i = 0; i < count; i++)
				{
					floatingUIObject = base.members[i];
					scalingParams = floatingUIObject.foScalingParmas;
					scalingParams.m_curScaleFactor = m_targetScaleFactor;
					floatingUIObject.foRectTransform.localScale = scalingParams.m_baseScale * m_targetScaleFactor;
				}
				return true;
			}
			m_remainedTime -= num;
			if (m_remainedTime <= 0f)
			{
				for (int j = 0; j < count; j++)
				{
					floatingUIObject = base.members[j];
					scalingParams = floatingUIObject.foScalingParmas;
					scalingParams.m_curScaleFactor = m_targetScaleFactor;
					floatingUIObject.foRectTransform.localScale = scalingParams.m_baseScale * m_targetScaleFactor;
				}
				return true;
			}
			if (m_SpeedType == 3 && m_remainedTime <= m_playTime * 0.5f && !m_isBoundedAcDecel)
			{
				float num2 = m_playTime * 0.5f - m_remainedTime;
				float num3 = num - num2;
				for (int k = 0; k < count; k++)
				{
					floatingUIObject = base.members[k];
					scalingParams = floatingUIObject.foScalingParmas;
					if (!GameGlobalUtil.IsAlmostSame(num3, 0f))
					{
						scalingParams.m_speed += scalingParams.m_accel * num3;
						scalingParams.m_curScaleFactor += scalingParams.m_speed * num3;
						floatingUIObject.foRectTransform.localScale = scalingParams.m_baseScale * scalingParams.m_curScaleFactor;
					}
					scalingParams.m_accel = 0f - scalingParams.m_accel;
					if (!GameGlobalUtil.IsAlmostSame(num2, 0f))
					{
						scalingParams.m_speed += scalingParams.m_accel * num2;
						scalingParams.m_curScaleFactor += scalingParams.m_speed * num2;
						floatingUIObject.foRectTransform.localScale = scalingParams.m_baseScale * scalingParams.m_curScaleFactor;
					}
				}
				m_isBoundedAcDecel = true;
				return false;
			}
			for (int l = 0; l < count; l++)
			{
				floatingUIObject = base.members[l];
				scalingParams = floatingUIObject.foScalingParmas;
				scalingParams.m_curScaleFactor += scalingParams.m_speed * num;
				scalingParams.m_speed += scalingParams.m_accel * num;
				floatingUIObject.foRectTransform.localScale = scalingParams.m_baseScale * scalingParams.m_curScaleFactor;
			}
			return false;
		}
	}

	private class EventMotion : EventBase
	{
		public string m_motionName = string.Empty;

		public float m_speedRate = 1f;

		public int m_loopCount = 1;

		public EventMotion()
			: base(EventType.Motion)
		{
		}

		public override void Start()
		{
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
			int count = base.members.Count;
			IFloatingUIObject floatingUIObject = null;
			for (int i = 0; i < count; i++)
			{
				floatingUIObject = base.members[i];
				floatingUIObject.foAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(floatingUIObject.foGameObject, m_motionName, m_speedRate * num);
				floatingUIObject.foMotionLoopCount = 0;
				floatingUIObject.foIsMotionComplete = false;
			}
		}

		public override bool UpdateEvent()
		{
			int count = base.members.Count;
			if (count <= 0)
			{
				return true;
			}
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
			string strMot = GameDefine.UIAnimationState.idle.ToString();
			bool result = true;
			IFloatingUIObject floatingUIObject = null;
			for (int i = 0; i < count; i++)
			{
				floatingUIObject = base.members[i];
				if (floatingUIObject == null || floatingUIObject.foAnimator == null || floatingUIObject.foIsMotionComplete)
				{
					continue;
				}
				floatingUIObject.foAnimator.speed = m_speedRate * num;
				AnimatorStateInfo currentAnimatorStateInfo = floatingUIObject.foAnimator.GetCurrentAnimatorStateInfo(0);
				if (!currentAnimatorStateInfo.IsName(m_motionName) || currentAnimatorStateInfo.normalizedTime < 0.99f)
				{
					result = false;
					continue;
				}
				floatingUIObject.foMotionLoopCount++;
				if (floatingUIObject.foMotionLoopCount < m_loopCount)
				{
					floatingUIObject.foAnimator.Rebind();
					floatingUIObject.foAnimator.Play(m_motionName);
					floatingUIObject.foAnimator.speed = m_speedRate;
					result = false;
				}
				else
				{
					floatingUIObject.foIsMotionComplete = true;
					GameGlobalUtil.PlayUIAnimation_WithChidren(floatingUIObject.foGameObject, strMot);
				}
			}
			return result;
		}
	}

	private List<IFloatingUIObject> m_allFloatingObjs = new List<IFloatingUIObject>();

	private SortedDictionary<string, List<IFloatingUIObject>> m_dicTagedFloatingObjs = new SortedDictionary<string, List<IFloatingUIObject>>();

	private List<EventBase> m_Events = new List<EventBase>();

	private void OnDestroy()
	{
		if (m_allFloatingObjs != null)
		{
			m_allFloatingObjs.Clear();
		}
		if (m_dicTagedFloatingObjs != null)
		{
			m_dicTagedFloatingObjs.Clear();
		}
		if (m_Events != null)
		{
			m_Events.Clear();
		}
	}

	public void AddTagedObject(GameObject obj, string tag, FloatingUIHandler handler)
	{
		if (tag == null)
		{
			tag = string.Empty;
		}
		IFloatingUIObject component = obj.GetComponent<IFloatingUIObject>();
		if (!m_allFloatingObjs.Contains(component))
		{
			m_allFloatingObjs.Add(component);
		}
		component.foTag = tag;
		component.foHandler = handler;
		component.foRotateAngle = component.foRectTransform.localRotation.eulerAngles;
		if (!string.IsNullOrEmpty(tag))
		{
			if (!m_dicTagedFloatingObjs.ContainsKey(tag))
			{
				m_dicTagedFloatingObjs.Add(tag, new List<IFloatingUIObject>());
			}
			List<IFloatingUIObject> list = m_dicTagedFloatingObjs[tag];
			if (!list.Contains(component))
			{
				list.Add(component);
			}
		}
	}

	public void RemoveTagedObject(GameObject obj, bool isRemoveEvent = true)
	{
		IFloatingUIObject component = obj.GetComponent<IFloatingUIObject>();
		if (component.foEvents != null && component.foEvents.Count > 0)
		{
			int count = component.foEvents.Count;
			int num = count;
			while (num > 0)
			{
				num--;
				component.foEvents[num].RemoveMember(component, isRemoveEvent);
			}
		}
		if (m_allFloatingObjs.Contains(component))
		{
			m_allFloatingObjs.Remove(component);
		}
		if (m_dicTagedFloatingObjs.ContainsKey(component.foTag))
		{
			List<IFloatingUIObject> list = m_dicTagedFloatingObjs[component.foTag];
			list.Remove(component);
			if (list.Count <= 0)
			{
				m_dicTagedFloatingObjs.Remove(component.foTag);
			}
		}
	}

	public List<IFloatingUIObject> GetTagedObjectList(string tag)
	{
		if (string.IsNullOrEmpty(tag))
		{
			return m_allFloatingObjs;
		}
		List<IFloatingUIObject> value = null;
		if (!m_dicTagedFloatingObjs.TryGetValue(tag, out value))
		{
			return null;
		}
		return value;
	}

	private void Update()
	{
		int count = m_Events.Count;
		if (count <= 0)
		{
			return;
		}
		EventBase eventBase = null;
		int num = count;
		while (num > 0)
		{
			num--;
			eventBase = m_Events[num];
			if (eventBase == null || eventBase.members.Count <= 0 || eventBase.UpdateEvent())
			{
				m_Events.Remove(eventBase);
			}
		}
	}

	public bool IsExistRunningEvent()
	{
		return m_Events.Count > 0;
	}

	private EventBase SetEvent_Common(EventType type, string tag)
	{
		List<IFloatingUIObject> tagedObjectList = GetTagedObjectList(tag);
		if (tagedObjectList == null)
		{
			return null;
		}
		EventBase eventBase = null;
		eventBase = type switch
		{
			EventType.Disappear => new EventDisappear(), 
			EventType.Move => new EventMove(), 
			EventType.Rotation => new EventRotate(), 
			EventType.Zoom => new EventScaling(), 
			EventType.Motion => new EventMotion(), 
			_ => new EventBase(type), 
		};
		if (eventBase == null)
		{
			return null;
		}
		eventBase.members.AddRange(tagedObjectList);
		foreach (IFloatingUIObject item in tagedObjectList)
		{
			if (item != null && item.foEvents != null)
			{
				item.foEvents.Add(eventBase);
			}
		}
		m_Events.Add(eventBase);
		return eventBase;
	}

	public void SetEvent_Disappear(string tag, float speedRate)
	{
		EventBase eventBase = SetEvent_Common(EventType.Disappear, tag);
		if (eventBase != null)
		{
			EventDisappear eventDisappear = eventBase as EventDisappear;
			eventDisappear.m_SpeedRate = speedRate;
			eventBase.Start();
		}
	}

	public void SetEvent_Move(string tag, float time, Vector2 moveFactor, int moveType)
	{
		EventBase eventBase = SetEvent_Common(EventType.Move, tag);
		if (eventBase != null)
		{
			EventMove eventMove = eventBase as EventMove;
			eventMove.m_targetMoveFactor = moveFactor;
			eventMove.m_playTime = time;
			eventMove.m_SpeedType = moveType;
			eventBase.Start();
		}
	}

	public void SetEvent_Rotate(string tag, float time, Vector3 rotateFactor, int moveType)
	{
		EventBase eventBase = SetEvent_Common(EventType.Rotation, tag);
		if (eventBase != null)
		{
			EventRotate eventRotate = eventBase as EventRotate;
			eventRotate.m_targetRotateFactor = rotateFactor;
			eventRotate.m_playTime = time;
			eventRotate.m_SpeedType = moveType;
			eventRotate.Start();
		}
	}

	public void SetEvent_Zoom(string tag, float time, float zoomFactor, int moveType)
	{
		EventBase eventBase = SetEvent_Common(EventType.Zoom, tag);
		if (eventBase != null)
		{
			EventScaling eventScaling = eventBase as EventScaling;
			eventScaling.m_targetScaleFactor = zoomFactor;
			eventScaling.m_playTime = time;
			eventScaling.m_SpeedType = moveType;
			eventScaling.Start();
		}
	}

	public void SetEvent_Motion(string tag, string motionName, float speedRate, int loopCount)
	{
		EventBase eventBase = SetEvent_Common(EventType.Motion, tag);
		if (eventBase != null)
		{
			EventMotion eventMotion = eventBase as EventMotion;
			eventMotion.m_motionName = motionName;
			eventMotion.m_speedRate = ((!(speedRate <= 0f)) ? speedRate : 0.01f);
			eventMotion.m_loopCount = ((loopCount < 1) ? 1 : loopCount);
			eventMotion.Start();
		}
	}

	private static void SetFirstSpeedAccel(float distance, float time, SPEED_TYPE speedType, out float speed, out float accel)
	{
		speed = 0f;
		accel = 0f;
		if (!GameGlobalUtil.IsAlmostSame(distance, 0f) && !(time < 0f) && !GameGlobalUtil.IsAlmostSame(time, 0f))
		{
			switch (speedType)
			{
			case SPEED_TYPE.SPEED_NORMAL:
				accel = 0f;
				speed = distance / time;
				break;
			case SPEED_TYPE.SPEED_ACCEL:
				accel = distance * 2f / (time * time);
				speed = 0f;
				break;
			case SPEED_TYPE.SPEED_DECEL:
				accel = 0f - distance * 2f / (time * time);
				speed = distance * 2f / time;
				break;
			case SPEED_TYPE.SPEED_ACDECEL:
				accel = distance * 2f / (time * time) * 2f;
				speed = 0f;
				break;
			}
		}
	}
}
