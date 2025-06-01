using UnityEngine;
using UnityEngine.EventSystems;

public class CommonCloseButton : MonoBehaviour
{
	public GameObject m_CloseButton;

	public GameObject m_SenderForCallback;

	public string m_ParamForCallback;

	[Header("Disappear Animation...")]
	public GameObject m_MenuRoot;

	public Animator m_DisappearAnimator;

	private bool m_isDisappearing;

	private GameDefine.EventProc m_fpCloseButtonClicked;

	private GameDefine.EventProc m_fpClosed;

	public GameDefine.EventProc onClickedCloseButton
	{
		get
		{
			return m_fpCloseButtonClicked;
		}
		set
		{
			m_fpCloseButtonClicked = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public GameDefine.EventProc onClosed
	{
		get
		{
			return m_fpClosed;
		}
		set
		{
			m_fpClosed = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void Start()
	{
		if (m_CloseButton != null)
		{
			GameGlobalUtil.AddEventTrigger(m_CloseButton, EventTriggerType.PointerClick, OnEventProc_CloseButtonClicked);
		}
	}

	private void OnDestroy()
	{
		m_fpCloseButtonClicked = null;
	}

	private void Update()
	{
		if (!m_isDisappearing)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_DisappearAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			if (m_fpClosed != null)
			{
				m_fpClosed(m_SenderForCallback, m_ParamForCallback);
			}
			m_isDisappearing = false;
			if (m_MenuRoot != null)
			{
				m_MenuRoot.SetActive(value: false);
			}
		}
	}

	private void OnEventProc_CloseButtonClicked(BaseEventData evtData)
	{
		if (m_fpCloseButtonClicked != null)
		{
			m_fpCloseButtonClicked(m_SenderForCallback, m_ParamForCallback);
		}
		if (m_MenuRoot != null && m_DisappearAnimator != null)
		{
			Animator[] componentsInChildren = m_MenuRoot.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				int num = componentsInChildren.Length;
				string stateName = GameDefine.UIAnimationState.disappear.ToString();
				for (int i = 0; i < num; i++)
				{
					componentsInChildren[i].Play(stateName);
				}
				m_isDisappearing = true;
				return;
			}
		}
		if (m_fpClosed != null)
		{
			m_fpClosed(m_SenderForCallback, m_ParamForCallback);
		}
	}
}
