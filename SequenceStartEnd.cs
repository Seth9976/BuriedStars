using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SequenceStartEnd : MonoBehaviour
{
	public GameObject m_RootObject;

	[Header("Start Members")]
	public GameObject m_StartRootObj;

	public Text m_SequenceNameText;

	public Text m_SequenceTimeText;

	public float m_StartWaitTime;

	[Header("End Members")]
	public GameObject m_EndRootObj;

	public float m_EndWaitTime;

	public Text m_textToBeContinued;

	public Text m_textToBeContinued_01;

	private GameDefine.EventProc m_fpClosed;

	private Animator m_BaseAnimator;

	private float m_RemainWaitTime;

	private void OnEnable()
	{
		Text[] textComps = new Text[2] { m_textToBeContinued, m_textToBeContinued_01 };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("SEQUENCE_START_TO_BE_CONTINUED");
		if (m_textToBeContinued != null)
		{
			m_textToBeContinued.text = xlsProgramText;
		}
		if (m_textToBeContinued_01 != null)
		{
			m_textToBeContinued_01.text = xlsProgramText;
		}
	}

	private void OnDestroy()
	{
		m_fpClosed = null;
		m_BaseAnimator = null;
	}

	private void Update()
	{
		if (m_BaseAnimator == null)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		m_BaseAnimator.speed = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		AnimatorStateInfo currentAnimatorStateInfo = m_BaseAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()))
		{
			if (currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Closed();
			}
		}
		else if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString()) && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				Close();
			}
			else if (m_RemainWaitTime <= 0f)
			{
				Close();
			}
			else
			{
				m_RemainWaitTime -= num;
			}
		}
	}

	public void ShowStartSequence(string xlsSequenceKey, GameDefine.EventProc fpClosed = null)
	{
		ShowStartSequence(Xls.SequenceData.GetData_byKey(xlsSequenceKey), fpClosed);
	}

	public void ShowStartSequence(int iSequenceIdx, GameDefine.EventProc fpClosed = null)
	{
		ShowStartSequence(Xls.SequenceData.GetData_bySwitchIdx(iSequenceIdx), fpClosed);
	}

	public void ShowStartSequence(Xls.SequenceData xlsSequenceData, GameDefine.EventProc fpClosed = null)
	{
		m_fpClosed = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_StartRootObj.SetActive(value: true);
		m_EndRootObj.SetActive(value: false);
		FontManager.ResetTextFontByCurrentLanguage(m_SequenceNameText);
		FontManager.ResetTextFontByCurrentLanguage(m_SequenceTimeText);
		Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsSequenceData.m_strIDName);
		m_SequenceNameText.text = data_byKey.m_strTxt;
		m_SequenceTimeText.text = GameGlobalUtil.GetCurrentGameTimeString();
		m_RemainWaitTime = m_StartWaitTime;
		m_BaseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		AudioManager audioManager = GameGlobalUtil.GetAudioManager();
		if (audioManager != null)
		{
			audioManager.PlayUISound("View_SeqInout");
		}
	}

	public void ShowEndSequence(GameDefine.EventProc fpClosed = null)
	{
		m_fpClosed = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_StartRootObj.SetActive(value: false);
		m_EndRootObj.SetActive(value: true);
		m_RemainWaitTime = m_EndWaitTime;
		m_BaseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		AudioManager audioManager = GameGlobalUtil.GetAudioManager();
		if (audioManager != null)
		{
			audioManager.PlayUISound("View_SeqInout");
		}
	}

	public void Close()
	{
		m_BaseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
	}

	private void Closed()
	{
		m_RootObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		Resources.UnloadUnusedAssets();
		if (m_fpClosed != null)
		{
			m_fpClosed(this, null);
		}
	}

	public void OnClick_FullScreen()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_BaseAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()) && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			Close();
		}
	}
}
