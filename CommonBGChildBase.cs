using UnityEngine;

public class CommonBGChildBase : MonoBehaviour
{
	private GameDefine.EventProc m_fpCloseComplete;

	private GameDefine.EventProc m_fpNoticeExitProc;

	public GameDefine.EventProc eventCloseComplete
	{
		get
		{
			return m_fpCloseComplete;
		}
		set
		{
			m_fpCloseComplete = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public GameDefine.EventProc eventNoticeExit
	{
		get
		{
			return m_fpNoticeExitProc;
		}
		set
		{
			m_fpNoticeExitProc = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}
}
