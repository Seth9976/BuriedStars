using GameEvent;
using UnityEngine;

public class CommonCoverEffect : MonoBehaviour
{
	public Animator m_Animator;

	public string m_StartStateName = string.Empty;

	public string m_EndStateName = string.Empty;

	[Range(0f, 1f)]
	public float m_EndStateExitTime;

	public string m_ParamName = string.Empty;

	private float m_speedRateOri = 1f;

	private GameDefine.EventProc m_fpCompleteCB;

	private void OnDestroy()
	{
		m_fpCompleteCB = null;
	}

	public void Play(int param, float fSpeedRate = 1f, GameDefine.EventProc fpCB = null)
	{
		if (!(m_Animator == null))
		{
			m_speedRateOri = fSpeedRate;
			m_fpCompleteCB = ((fpCB == null) ? null : new GameDefine.EventProc(fpCB.Invoke));
			base.gameObject.SetActive(value: true);
			m_Animator.Rebind();
			if (!string.IsNullOrEmpty(m_ParamName))
			{
				m_Animator.SetInteger(m_ParamName, param);
			}
			m_Animator.speed = fSpeedRate;
			m_Animator.Play(m_StartStateName);
		}
	}

	private void Update()
	{
		if (m_Animator == null)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((instance == null || !instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		m_Animator.speed = m_speedRateOri * num;
		AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(m_EndStateName) && currentAnimatorStateInfo.normalizedTime >= m_EndStateExitTime)
		{
			m_Animator.Rebind();
			base.gameObject.SetActive(value: false);
			if (m_fpCompleteCB != null)
			{
				m_fpCompleteCB(this, null);
			}
		}
	}
}
