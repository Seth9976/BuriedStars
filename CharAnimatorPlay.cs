using GameEvent;
using UnityEngine;

public class CharAnimatorPlay : MonoBehaviour
{
	public Animator m_HeadAnimator;

	public Animator m_BodyAnimator;

	public void PlayAnimation(string strMotion, bool isTalk)
	{
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("CHR_ANI_MOT_TALK");
		string strMot = strMotion + ((xlsProgramDefineStr == null || !isTalk) ? string.Empty : xlsProgramDefineStr);
		GameGlobalUtil.PlayUIAnimation(m_HeadAnimator, strMot);
		if (!GameGlobalUtil.IsCheckPlayAnimation(m_BodyAnimator, strMotion))
		{
			GameGlobalUtil.PlayUIAnimation(m_BodyAnimator, strMotion);
		}
	}

	public void Update()
	{
		if (m_HeadAnimator != null)
		{
			m_HeadAnimator.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
		if (m_BodyAnimator != null)
		{
			m_BodyAnimator.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
	}
}
