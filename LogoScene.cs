using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
	[Serializable]
	public class Parts
	{
		public GameObject m_PartRoot;

		public Animator m_PartAnimator;
	}

	public Parts[] m_Parts;

	private int m_curPartIdx = -1;

	private Parts m_curPart;

	public const string c_SceneName = "Scene/UI_PS/1003_Logo_NF";

	private void Start()
	{
		if (m_Parts != null && m_Parts.Length > 0)
		{
			Parts[] parts = m_Parts;
			foreach (Parts parts2 in parts)
			{
				if (parts2.m_PartRoot != null)
				{
					parts2.m_PartRoot.SetActive(value: false);
				}
			}
			ChangePart(0);
		}
		else
		{
			StartCoroutine(GotoTitleScene());
		}
	}

	private void OnDisable()
	{
		m_curPart = null;
	}

	private void Update()
	{
		if (m_curPart == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_curPart.m_PartAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			if (m_curPartIdx + 1 < m_Parts.Length)
			{
				ChangePart(m_curPartIdx + 1);
			}
			else
			{
				StartCoroutine(GotoTitleScene());
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
		{
			SetDisappearMot();
		}
	}

	public void SetDisappearMot()
	{
		if (m_curPart.m_PartAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.idle)))
		{
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_curPart.m_PartRoot, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
		}
	}

	public void TouchLogoScene()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			SetDisappearMot();
		}
	}

	private void ChangePart(int partIdx)
	{
		if (m_curPartIdx != partIdx)
		{
			if (m_curPart != null)
			{
				m_curPart.m_PartRoot.SetActive(value: false);
			}
			if (m_Parts == null || partIdx >= m_Parts.Length || partIdx < 0)
			{
				m_curPart = null;
				m_curPartIdx = -1;
				return;
			}
			m_curPart = m_Parts[partIdx];
			m_curPartIdx = partIdx;
			m_curPart.m_PartRoot.SetActive(value: true);
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_curPart.m_PartRoot, GameDefine.UIAnimationState.appear.ToString());
		}
	}

	private IEnumerator GotoTitleScene()
	{
		ChangePart(-1);
		UnloadScene();
		yield return StartCoroutine(TitleScreen.LoadScene());
	}

	public static IEnumerator LoadScene()
	{
		SceneManager.LoadScene("Scene/UI_PS/1003_Logo_NF", LoadSceneMode.Single);
		yield break;
	}

	public static void UnloadScene()
	{
	}
}
