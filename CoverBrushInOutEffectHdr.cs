using GameEvent;
using UnityEngine;

public class CoverBrushInOutEffectHdr : MonoBehaviour
{
	public delegate void CompleteCB();

	public Animator LinkedAnimator;

	public string StartStateName = string.Empty;

	public string EndStateName = string.Empty;

	private bool isPlaying;

	private float m_speedRateOri = 1f;

	private CompleteCB m_fpCompleteCB;

	private void OnDestroy()
	{
		m_fpCompleteCB = null;
	}

	public void Play(CompleteCB fpCB = null, float fSpeedRate = 1f)
	{
		if (!(LinkedAnimator == null))
		{
			if (fpCB != null)
			{
				m_fpCompleteCB = fpCB.Invoke;
			}
			isPlaying = true;
			m_speedRateOri = fSpeedRate;
			LinkedAnimator.Rebind();
			LinkedAnimator.SetBool("Play", value: true);
			LinkedAnimator.speed = fSpeedRate;
			LinkedAnimator.Play(StartStateName, -1, 0f);
		}
	}

	private void Update()
	{
		if (LinkedAnimator == null || !isPlaying)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		LinkedAnimator.speed = num * m_speedRateOri;
		if (LinkedAnimator.GetCurrentAnimatorStateInfo(0).IsName(EndStateName))
		{
			LinkedAnimator.Rebind();
			LinkedAnimator.SetBool("Play", value: false);
			isPlaying = false;
			if (m_fpCompleteCB != null)
			{
				m_fpCompleteCB();
			}
		}
	}
}
