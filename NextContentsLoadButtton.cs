using UnityEngine;
using UnityEngine.UI;

public class NextContentsLoadButtton : MonoBehaviour
{
	public enum State
	{
		NotSelected,
		Selected,
		Loading
	}

	[Header("Not Selected Members")]
	public GameObject m_NotSelectedRoot;

	public Text m_NotSelectedText;

	[Header("Selected Members")]
	public GameObject m_SelectedRoot;

	public Text m_SelectedText;

	public Button m_PadIconButton;

	[Header("Loading Members")]
	public GameObject m_LoadingRoot;

	public Text m_LoadingText;

	public RectTransform m_LoadingWheelRT;

	public float m_LoadingWheelSpace = -20f;

	private State m_curState;

	private RectTransform m_rt;

	public State curState => m_curState;

	public string nextPageText
	{
		set
		{
			Text notSelectedText = m_NotSelectedText;
			m_SelectedText.text = value;
			notSelectedText.text = value;
		}
	}

	public string loadingText
	{
		set
		{
			m_LoadingText.text = value;
			AdjustLoadingWheel();
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			if (m_rt == null)
			{
				m_rt = base.gameObject.GetComponent<RectTransform>();
			}
			return m_rt;
		}
	}

	private void OnDestroy()
	{
		m_rt = null;
	}

	public void SetState(State newState, bool isIgnoreSame = true, bool enableAppearAni = false)
	{
		if (m_curState != newState || !isIgnoreSame)
		{
			GameObject obj = null;
			switch (newState)
			{
			case State.NotSelected:
				m_NotSelectedRoot.SetActive(value: true);
				m_SelectedRoot.SetActive(value: false);
				m_LoadingRoot.SetActive(value: false);
				obj = m_NotSelectedRoot;
				break;
			case State.Selected:
				m_NotSelectedRoot.SetActive(value: false);
				m_SelectedRoot.SetActive(value: true);
				m_LoadingRoot.SetActive(value: false);
				obj = m_SelectedRoot;
				break;
			case State.Loading:
				m_NotSelectedRoot.SetActive(value: false);
				m_SelectedRoot.SetActive(value: false);
				m_LoadingRoot.SetActive(value: true);
				obj = m_LoadingRoot;
				break;
			}
			GameGlobalUtil.PlayUIAnimation_WithChidren(obj, (!enableAppearAni) ? GameDefine.UIAnimationState.idle.ToString() : GameDefine.UIAnimationState.appear.ToString());
			m_curState = newState;
		}
	}

	private void AdjustLoadingWheel()
	{
		if (!(m_LoadingWheelRT == null))
		{
			RectTransform component = m_LoadingRoot.GetComponent<RectTransform>();
			float num = (component.rect.width - m_LoadingText.preferredWidth) * 0.5f;
			num -= m_LoadingWheelRT.rect.width * 0.5f;
			num -= m_LoadingWheelSpace;
			m_LoadingWheelRT.anchoredPosition = new Vector2(num, m_LoadingWheelRT.anchoredPosition.y);
		}
	}
}
