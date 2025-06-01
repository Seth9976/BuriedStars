using GameEvent;
using UnityEngine;

public class BackgroundStreamHdr : MonoBehaviour
{
	public delegate void CompleteCB();

	private enum State
	{
		Unknown,
		Appear,
		Enabled,
		Disappear
	}

	public Animator LinkedAnimator;

	public string AppearStateName = string.Empty;

	public string EnabledStateName = string.Empty;

	public string DisappearStateName = string.Empty;

	private CompleteCB m_fpCompleteCB;

	private float m_speedRateOri = 1f;

	private State m_CurState;

	public void Appear(CompleteCB fpCB = null, float fSpeedRate = 1f)
	{
		if (!(LinkedAnimator == null))
		{
			if (fpCB != null)
			{
				m_fpCompleteCB = fpCB.Invoke;
			}
			else
			{
				m_fpCompleteCB = null;
			}
			m_speedRateOri = fSpeedRate;
			LinkedAnimator.Rebind();
			LinkedAnimator.SetBool("Disappear", value: false);
			LinkedAnimator.speed = fSpeedRate;
			LinkedAnimator.Play(AppearStateName, -1, 0f);
			m_CurState = State.Appear;
		}
	}

	public void Disappear(CompleteCB fpCB = null, float fSpeedRate = 1f)
	{
		if (!(LinkedAnimator == null))
		{
			if (fpCB != null)
			{
				m_fpCompleteCB = fpCB.Invoke;
			}
			else
			{
				m_fpCompleteCB = null;
			}
			m_speedRateOri = fSpeedRate;
			LinkedAnimator.SetBool("Disappear", value: true);
			LinkedAnimator.speed = fSpeedRate;
			m_CurState = State.Disappear;
		}
	}

	public void Enable()
	{
		if (!(LinkedAnimator == null))
		{
			LinkedAnimator.Rebind();
			LinkedAnimator.SetBool("Disappear", value: false);
			LinkedAnimator.speed = 1f;
			LinkedAnimator.Play(EnabledStateName, -1, 0f);
			m_CurState = State.Enabled;
		}
	}

	private void Update()
	{
		if (LinkedAnimator == null)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		LinkedAnimator.speed = m_speedRateOri * num;
		switch (m_CurState)
		{
		case State.Appear:
			if (LinkedAnimator.GetCurrentAnimatorStateInfo(0).IsName(EnabledStateName))
			{
				m_CurState = State.Enabled;
				if (m_fpCompleteCB != null)
				{
					m_fpCompleteCB();
				}
			}
			break;
		case State.Disappear:
		{
			AnimatorStateInfo currentAnimatorStateInfo = LinkedAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(DisappearStateName) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				m_CurState = State.Unknown;
				LinkedAnimator.Rebind();
				if (m_fpCompleteCB != null)
				{
					m_fpCompleteCB();
				}
			}
			break;
		}
		}
	}

	private void OnDestroy()
	{
		m_fpCompleteCB = null;
	}
}
