using UnityEngine;
using UnityEngine.UI;

public class SystemMenuBase : MonoBehaviour
{
	public GameObject m_RootObject;

	public GameObject m_TitleRootObject;

	protected GameDefine.EventProc m_fpClosedCB;

	protected Animator m_AppearCheckAnimator;

	protected Animator m_DisappearCheckAnimator;

	protected AudioManager m_AudioManager;

	protected bool m_isInputBlock;

	protected Button m_OutterExitButton;

	protected bool m_isInFromMainMenu;

	protected GameDefine.EventProc m_fpCBChangeValueClose;

	public bool isInputBlock
	{
		get
		{
			return m_isInputBlock;
		}
		set
		{
			m_isInputBlock = value;
		}
	}

	public bool isInFormMainMenu
	{
		set
		{
			m_isInFromMainMenu = value;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
	}

	protected virtual void OnDestroy()
	{
		m_fpClosedCB = null;
		m_DisappearCheckAnimator = null;
		m_AudioManager = null;
		m_OutterExitButton = null;
	}

	protected virtual void Update()
	{
		if (!(m_DisappearCheckAnimator == null))
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_DisappearCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Closed();
			}
		}
	}

	public virtual void Show(bool isEnableAnimation = true, GameDefine.EventProc fpClosedCB = null, bool isNeedSetCloseCB = true, Button outterExitButton = null)
	{
		if (isNeedSetCloseCB)
		{
			m_fpClosedCB = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		}
		m_DisappearCheckAnimator = null;
		m_OutterExitButton = outterExitButton;
		base.gameObject.SetActive(value: true);
		if (m_RootObject != null)
		{
			m_RootObject.SetActive(value: true);
		}
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		m_isInputBlock = false;
		string strMot = ((!isEnableAnimation) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
		m_AppearCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, strMot);
	}

	public virtual void Close(bool isEnableAnimation = true)
	{
		if (m_fpCBChangeValueClose != null)
		{
			m_fpCBChangeValueClose(this, null);
			m_fpCBChangeValueClose = null;
			return;
		}
		if (isEnableAnimation)
		{
			m_DisappearCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
			if (m_DisappearCheckAnimator != null)
			{
				return;
			}
		}
		Closed();
	}

	protected virtual void Closed()
	{
		base.gameObject.SetActive(value: false);
		if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, null);
		}
	}

	public virtual void ResetXlsTexts()
	{
	}

	public virtual void ShowTitle(bool isShow)
	{
		if (!(m_TitleRootObject == null))
		{
			m_TitleRootObject.SetActive(isShow);
		}
	}

	public virtual bool CheckExistChangeValue(GameDefine.EventProc fpCB)
	{
		return false;
	}
}
