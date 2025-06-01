using GameEvent;
using UnityEngine;

public class MarkerTouch : MonoBehaviour
{
	private EventEngine m_EventEngine;

	private void Start()
	{
		m_EventEngine = EventEngine.GetInstance();
	}

	private void OnDestroy()
	{
		m_EventEngine = null;
	}

	private bool AvailableTouch(GameObject goAni)
	{
		bool result = false;
		Animator componentInChildren = goAni.GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("ANI_IDLE");
			if (xlsProgramDefineStr != null)
			{
				result = componentInChildren.GetCurrentAnimatorStateInfo(0).IsName(xlsProgramDefineStr);
			}
		}
		return result;
	}

	public void TouchFindIcon()
	{
		if (MainLoadThing.instance.IsTouchableState() && AvailableTouch(base.transform.parent.gameObject) && m_EventEngine != null)
		{
			m_EventEngine.m_EventObject.TouchFindMarker(base.gameObject);
		}
	}

	public void TouchTalkIcon()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_EventEngine != null)
		{
			string strCharKey = base.gameObject.GetComponent<MarkerTalk>().m_strCharKey;
			m_EventEngine.m_TalkChar.TouchTalkIcon(strCharKey);
		}
	}
}
