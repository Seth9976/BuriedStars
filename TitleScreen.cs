using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
	public Text m_textVersion;

	public Image m_LacalLogoImage;

	public Text m_PressAnyKeyText;

	public Text m_ButtonRightsText;

	public Text m_textBuriedStarsNationalLang;

	public Animator[] m_ContralAnimator;

	public float m_BGMPlayDelay = 3f;

	private float m_RemainBGMDelay;

	private bool m_isNeedPlayBGM = true;

	private bool m_isDisappearing;

	private bool m_isLoadingMainMenu;

	private AudioManager m_AudioManager;

	private readonly string c_aniStateName_Appear = GameDefine.UIAnimationState.appear.ToString();

	private readonly string c_aniStateName_Idle = GameDefine.UIAnimationState.idle.ToString();

	private readonly string c_aniStateName_Disappear = GameDefine.UIAnimationState.disappear.ToString();

	public const string c_SceneName = "Scene/UI_PS/0044_ui_Title_screen";

	private void Start()
	{
		GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, c_aniStateName_Appear);
		m_isDisappearing = false;
		m_isLoadingMainMenu = false;
		CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		if (commonButtonGuide != null)
		{
			commonButtonGuide.SetShow(isShow: false);
		}
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		m_RemainBGMDelay = m_BGMPlayDelay;
		m_isNeedPlayBGM = true;
		FontManager.ResetTextFontByCurrentLanguage(m_PressAnyKeyText);
		FontManager.ResetTextFontByCurrentLanguage(m_ButtonRightsText);
		FontManager.ResetTextFontByCurrentLanguage(m_textBuriedStarsNationalLang);
		m_PressAnyKeyText.text = GameGlobalUtil.GetXlsProgramText("TITLE_SCREEN_PRESS_BUTTON_TEXT");
		m_ButtonRightsText.text = GameGlobalUtil.GetXlsProgramText("TITLE_SCREEN_BOTTOM_RIGHTS_TEXT");
		m_textBuriedStarsNationalLang.text = GameGlobalUtil.GetXlsProgramText("TITLE_BURIED_STARS");
		m_textVersion.text = string.Empty;
	}

	private void OnDestroy()
	{
		m_AudioManager = null;
	}

	private void Update()
	{
		if (m_isNeedPlayBGM)
		{
			m_RemainBGMDelay -= Time.deltaTime;
			if (m_RemainBGMDelay <= 0f)
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayKey(0, "테마", isSetVol: true, 0f, 1f, isLoop: true);
				}
				m_isNeedPlayBGM = false;
			}
		}
		if (m_isLoadingMainMenu)
		{
			return;
		}
		if (m_isDisappearing)
		{
			bool flag = true;
			Animator[] contralAnimator = m_ContralAnimator;
			foreach (Animator animator in contralAnimator)
			{
				if (!(animator == null))
				{
					AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					if (!currentAnimatorStateInfo.IsName(c_aniStateName_Disappear) || currentAnimatorStateInfo.normalizedTime < 0.99f)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				StartCoroutine(LoadingMainMenu());
			}
		}
		else
		{
			PressAnyKey();
		}
	}

	public void PressAnyKey()
	{
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.OptionButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TouchPadButton))
		{
			GoNextState();
		}
	}

	private void GoNextState()
	{
		int num = 2;
		if (m_ContralAnimator != null && m_ContralAnimator.Length > 0)
		{
			Animator[] contralAnimator = m_ContralAnimator;
			foreach (Animator animator in contralAnimator)
			{
				if (!(animator == null))
				{
					num = (int)GameGlobalUtil.GetAnimationState(animator);
					break;
				}
			}
		}
		if (num == 3 || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		switch (num)
		{
		case 2:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, c_aniStateName_Idle);
			break;
		case 1:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, c_aniStateName_Disappear);
			m_isDisappearing = true;
			if (m_AudioManager != null)
			{
				m_AudioManager.SetVol(0, 0f, 0.2f);
				m_AudioManager.PlayUISound("Push_IntoMain");
			}
			break;
		}
	}

	private IEnumerator LoadingMainMenu()
	{
		m_isLoadingMainMenu = true;
		LoadingScreen.Show();
		UnloadScene();
		AsyncOperation asyncOp = MainMenuCommon.LoadScene(isAsync: true);
		MainLoadThing.instance.StartCoroutine(MainMenuCommon.ChangeMode(MainMenuCommon.Mode.MainMenu));
		MainMenuCommon.SetBGType(MainMenuCommon.BGType.ImageBG);
		if (asyncOp != null)
		{
			while (!asyncOp.isDone)
			{
				yield return null;
			}
		}
	}

	public static IEnumerator LoadScene()
	{
		SceneManager.LoadScene("Scene/UI_PS/0044_ui_Title_screen", LoadSceneMode.Single);
		yield break;
	}

	public static void UnloadScene()
	{
	}

	public void TouchAnyPoint()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			GoNextState();
		}
	}
}
