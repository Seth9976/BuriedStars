using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class TalkCutStart : MonoBehaviour
{
	public GameObject m_RootObject;

	public Text m_SequenceNameText;

	public Text m_TalkStartText;

	public Text m_CurTimeText;

	public float m_WaitTime = 10f;

	private float m_RemainTime;

	private GameDefine.EventProc m_fpClosed;

	private Animator m_BasedAnimator;

	private void OnDestroy()
	{
		m_fpClosed = null;
		m_BasedAnimator = null;
	}

	private void Update()
	{
		if (m_BasedAnimator == null)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		m_BasedAnimator.speed = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		AnimatorStateInfo currentAnimatorStateInfo = m_BasedAnimator.GetCurrentAnimatorStateInfo(0);
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
			else if (m_RemainTime <= 0f)
			{
				Close();
			}
			else
			{
				m_RemainTime -= num;
			}
		}
	}

	public void Show(GameDefine.EventProc fpClosed = null)
	{
		m_fpClosed = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		Text[] textComps = new Text[3] { m_SequenceNameText, m_TalkStartText, m_CurTimeText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		int curCutIdx = GameSwitch.GetInstance().GetCurCutIdx();
		Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(curCutIdx);
		m_SequenceNameText.text = GameGlobalUtil.GetXlsTextData(data_bySwitchIdx.m_strIDCutName);
		string xlsDataName = ((GameMain.GetDefaultSceneType() != GameMain.eDefType.Talk) ? "TALK_CUT_START_TEXT_INVEST" : "TALK_CUT_START_TEXT");
		m_TalkStartText.text = GameGlobalUtil.GetXlsProgramText(xlsDataName);
		m_CurTimeText.text = GameGlobalUtil.GetCurrentGameTimeString();
		m_RemainTime = m_WaitTime;
		m_BasedAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		AudioManager audioManager = GameGlobalUtil.GetAudioManager();
		if (audioManager != null)
		{
			audioManager.PlayUISound("View_CutStart");
		}
	}

	public void Close()
	{
		if (m_BasedAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()) && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			m_BasedAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		}
	}

	private void Closed()
	{
		base.gameObject.SetActive(value: false);
		m_RootObject.SetActive(value: false);
		Resources.UnloadUnusedAssets();
		if (m_fpClosed != null)
		{
			m_fpClosed(this, null);
		}
	}

	public void TouchClose()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			Close();
		}
	}
}
