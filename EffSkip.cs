using UnityEngine;

public class EffSkip : MonoBehaviour
{
	public GameObject m_goTopObj;

	public Animator m_animSkip;

	private GameDefine.eAnimChangeState m_eAnimState;

	private bool m_isSkip;

	public void SetSkip(bool isSkip)
	{
		if (m_isSkip == isSkip)
		{
			return;
		}
		m_goTopObj.SetActive(value: true);
		m_isSkip = isSkip;
		if (isSkip)
		{
			if (!m_animSkip.gameObject.activeInHierarchy)
			{
				m_animSkip.gameObject.SetActive(value: true);
			}
			GameGlobalUtil.PlayUIAnimation(m_animSkip, GameDefine.UIAnimationState.appear);
		}
		else if (m_eAnimState == GameDefine.eAnimChangeState.none)
		{
			GameGlobalUtil.PlayUIAnimation(m_animSkip, GameDefine.UIAnimationState.disappear, ref m_eAnimState);
		}
	}

	private void Update()
	{
		if (!m_isSkip && GameGlobalUtil.CheckPlayEndUIAnimation(m_animSkip, GameDefine.UIAnimationState.disappear, ref m_eAnimState))
		{
			m_goTopObj.SetActive(value: false);
			m_animSkip.gameObject.SetActive(value: false);
			m_eAnimState = GameDefine.eAnimChangeState.none;
		}
	}
}
