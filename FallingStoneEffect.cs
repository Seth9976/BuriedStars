using GameEvent;
using UnityEngine;

public class FallingStoneEffect : MonoBehaviour
{
	public Animator m_Animator;

	public string[] m_AniStateNames;

	private int m_curStateIdx = -1;

	private float m_curAniSpeedRate = 1f;

	public bool Show(int stateIdx, float aniSpeedRate = 1f)
	{
		if (m_Animator == null)
		{
			return false;
		}
		if (m_AniStateNames == null || m_AniStateNames.Length <= 0)
		{
			return false;
		}
		if (stateIdx < 0 || stateIdx >= m_AniStateNames.Length)
		{
			return false;
		}
		bool flag = m_curStateIdx == stateIdx;
		bool flag2 = GameGlobalUtil.IsAlmostSame(aniSpeedRate, m_curAniSpeedRate);
		if (flag && flag2)
		{
			return true;
		}
		base.gameObject.SetActive(value: true);
		if (!flag)
		{
			string stateName = m_AniStateNames[stateIdx];
			if (!GameGlobalUtil.HasStateInAnimator(m_Animator, stateName))
			{
				base.gameObject.SetActive(value: false);
				return false;
			}
			m_Animator.Play(stateName);
		}
		m_Animator.speed = aniSpeedRate;
		m_curAniSpeedRate = aniSpeedRate;
		m_curStateIdx = stateIdx;
		return true;
	}

	public void Update()
	{
		if (!(m_Animator == null))
		{
			EventEngine instance = EventEngine.GetInstance();
			float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
			m_Animator.speed = num * m_curAniSpeedRate;
		}
	}

	public void Hide()
	{
		m_curStateIdx = -1;
		base.gameObject.SetActive(value: false);
	}
}
