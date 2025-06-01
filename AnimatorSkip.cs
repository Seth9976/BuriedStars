using GameEvent;
using UnityEngine;

public class AnimatorSkip : MonoBehaviour
{
	private Animator m_animSkip;

	private void OnEnable()
	{
		m_animSkip = GetComponent<Animator>();
	}

	private void Update()
	{
		if (m_animSkip != null)
		{
			m_animSkip.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
	}
}
