using UnityEngine;

[AddComponentMenu("Largo Custom/Animation Event Handler", 0)]
public class AnimationEventHandler : MonoBehaviour
{
	private static AudioManager s_AudioManager;

	private GameDefine.EventProc m_fpEventCB;

	public GameDefine.EventProc fpEventCB
	{
		get
		{
			return m_fpEventCB;
		}
		set
		{
			m_fpEventCB = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void OnDestroy()
	{
		s_AudioManager = null;
	}

	private void OnAniEvent_PlayUISound(string uiSoundKey)
	{
		if (s_AudioManager == null)
		{
			s_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (!(s_AudioManager == null))
		{
			s_AudioManager.PlayUISound(uiSoundKey);
		}
	}

	private void OnAniEvent_CallEventCB(string _tag)
	{
		if (m_fpEventCB != null)
		{
			m_fpEventCB(this, _tag);
		}
	}
}
