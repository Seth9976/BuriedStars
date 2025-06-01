using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuItem : MonoBehaviour
{
	[Serializable]
	public class Members
	{
		public GameObject m_RootObject;

		public Text m_TextName;

		public Text m_TextSub;

		public Text m_Channel;

		public Text m_GameTimeText;

		public Text m_LocationText;

		public Text m_SavedTimeText;
	}

	public Members m_NotSelect = new Members();

	public Members m_Select = new Members();

	private RectTransform m_rt;

	private Animator m_Animator;

	private GameDefine.EventProc m_PushAniCompleteCB;

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

	public string textName
	{
		set
		{
			if (m_NotSelect.m_TextName != null)
			{
				m_NotSelect.m_TextName.text = value;
			}
			if (m_Select.m_TextName != null)
			{
				m_Select.m_TextName.text = value;
			}
		}
	}

	public string textSub
	{
		set
		{
			if (m_NotSelect.m_TextSub != null)
			{
				m_NotSelect.m_TextSub.text = value;
			}
			if (m_Select.m_TextSub != null)
			{
				m_Select.m_TextSub.text = value;
			}
		}
	}

	public string textChannel
	{
		set
		{
			if (m_NotSelect.m_Channel != null)
			{
				m_NotSelect.m_Channel.text = value;
			}
			if (m_Select.m_Channel != null)
			{
				m_Select.m_Channel.text = value;
			}
		}
	}

	public bool select
	{
		get
		{
			return m_Select.m_RootObject.activeSelf;
		}
		set
		{
			m_Select.m_RootObject.SetActive(value);
			m_NotSelect.m_RootObject.SetActive(!value);
		}
	}

	public void PlayPushAnimation(GameDefine.EventProc fpCompleteProc)
	{
		m_PushAniCompleteCB = ((fpCompleteProc == null) ? null : new GameDefine.EventProc(fpCompleteProc.Invoke));
		m_Animator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, "steam_push");
		if (m_Animator == null && m_PushAniCompleteCB != null)
		{
			m_PushAniCompleteCB(this, null);
		}
	}

	private void OnDestroy()
	{
		m_rt = null;
		m_Animator = null;
		m_PushAniCompleteCB = null;
	}

	private void Update()
	{
		if (m_Animator == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName("steam_push") && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.idle.ToString());
			m_Animator = null;
			if (m_PushAniCompleteCB != null)
			{
				m_PushAniCompleteCB(this, null);
			}
		}
	}

	public void SetSaveSlotInfo(SaveLoad.cSaveSlotInfo saveslotInfo)
	{
		if (saveslotInfo != null)
		{
			string gameTimeString = GameGlobalUtil.GetGameTimeString(saveslotInfo.m_fGameTime);
			if (m_NotSelect.m_GameTimeText != null)
			{
				m_NotSelect.m_GameTimeText.horizontalOverflow = HorizontalWrapMode.Overflow;
				m_NotSelect.m_GameTimeText.text = gameTimeString;
				Transform component = m_NotSelect.m_GameTimeText.gameObject.GetComponent<Transform>();
				if (component != null && component.parent != null)
				{
					component.parent.gameObject.SetActive(value: true);
				}
			}
			if (m_Select.m_GameTimeText != null)
			{
				m_Select.m_GameTimeText.horizontalOverflow = HorizontalWrapMode.Overflow;
				m_Select.m_GameTimeText.text = gameTimeString;
				Transform component2 = m_Select.m_GameTimeText.gameObject.GetComponent<Transform>();
				if (component2 != null && component2.parent != null)
				{
					component2.parent.gameObject.SetActive(value: true);
				}
			}
			Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(saveslotInfo.m_iCutIdx);
			string text = ((data_bySwitchIdx == null) ? string.Empty : GameGlobalUtil.GetXlsTextData(data_bySwitchIdx.m_strIDCutName));
			if (text != null)
			{
				if (m_NotSelect.m_LocationText != null)
				{
					m_NotSelect.m_LocationText.text = text;
				}
				if (m_Select.m_LocationText != null)
				{
					m_Select.m_LocationText.text = text;
				}
			}
			string text2 = GameGlobalUtil.GetXlsProgramText("REAL_TIME_TEXT_FMT");
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}";
			}
			string text3 = string.Format(text2, saveslotInfo.m_iCurYear, saveslotInfo.m_iCurMonth, saveslotInfo.m_iCurDay, saveslotInfo.m_iCurH, saveslotInfo.m_iCurM);
			if (m_NotSelect.m_SavedTimeText != null)
			{
				m_NotSelect.m_SavedTimeText.text = text3;
			}
			if (m_Select.m_SavedTimeText != null)
			{
				m_Select.m_SavedTimeText.text = text3;
			}
			return;
		}
		if (m_NotSelect.m_GameTimeText != null)
		{
			Transform component3 = m_NotSelect.m_GameTimeText.gameObject.GetComponent<Transform>();
			if (component3 != null && component3.parent != null)
			{
				component3.parent.gameObject.SetActive(value: false);
			}
		}
		if (m_Select.m_GameTimeText != null)
		{
			Transform component4 = m_Select.m_GameTimeText.gameObject.GetComponent<Transform>();
			if (component4 != null && component4.parent != null)
			{
				component4.parent.gameObject.SetActive(value: false);
			}
		}
	}

	public void ResetFontByCurrentLanguage()
	{
		Text[] textComps = new Text[12]
		{
			m_NotSelect.m_TextName, m_NotSelect.m_TextSub, m_NotSelect.m_GameTimeText, m_NotSelect.m_LocationText, m_NotSelect.m_SavedTimeText, m_Select.m_TextName, m_Select.m_TextSub, m_Select.m_GameTimeText, m_Select.m_LocationText, m_Select.m_SavedTimeText,
			m_NotSelect.m_Channel, m_Select.m_Channel
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}
}
