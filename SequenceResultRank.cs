using System;
using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SequenceResultRank : MonoBehaviour
{
	[Serializable]
	public class SlotInfo
	{
		public GameObject m_SlotRootObject;

		public Text m_NameText;

		public Text m_ScoreText;

		public Image m_FaceImage;

		public Text m_DeadText;

		public Text m_RankingText;
	}

	public GameObject m_RootObject;

	public Animator m_BasedAnimator;

	public Text m_SequenceNameText;

	public Text m_TitleText;

	public float m_AutoDelayTime = 5f;

	[Header("Rank Slot")]
	public Text m_RankNoticeText;

	public SlotInfo m_Slot1st = new SlotInfo();

	public SlotInfo m_Slot2nd = new SlotInfo();

	public SlotInfo m_Slot3rd = new SlotInfo();

	public SlotInfo m_Slot4th = new SlotInfo();

	public SlotInfo m_Slot5th = new SlotInfo();

	private const int c_SlotCount = 5;

	private SlotInfo[] m_SlotInfos = new SlotInfo[5];

	[Header("Under Bar")]
	public Text m_ContinueText;

	public Button m_PadIconButton;

	private GameDefine.EventProc m_fpClosed;

	private string m_strCharState_Dead = string.Empty;

	private AudioManager m_AudioManager;

	private bool m_isPrevButtonGuideView;

	private float m_RemainAutoDelayTime;

	private const string c_characterImageBundleName = "image/smartwatch_rank";

	private ContentThumbnailManager m_characterImageMgr = new ContentThumbnailManager("image/smartwatch_rank");

	private void OnDestroy()
	{
		m_fpClosed = null;
		m_AudioManager = null;
	}

	private void Update()
	{
		if (m_BasedAnimator == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_BasedAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()))
		{
			if (currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Closed();
			}
		}
		else
		{
			if (!currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString()) || PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_PadIconButton);
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_PadIconButton);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_OK");
				}
				Close();
			}
			if (EventEngine.GetInstance().GetAuto())
			{
				m_RemainAutoDelayTime -= Time.deltaTime;
				if (m_RemainAutoDelayTime <= 0f)
				{
					Close();
				}
			}
		}
	}

	public IEnumerator Show(GameDefine.EventProc fpClosed = null)
	{
		m_SlotInfos[0] = m_Slot1st;
		m_SlotInfos[1] = m_Slot2nd;
		m_SlotInfos[2] = m_Slot3rd;
		m_SlotInfos[3] = m_Slot4th;
		m_SlotInfos[4] = m_Slot5th;
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		yield return GameMain.instance.StartCoroutine(m_characterImageMgr.LoadAssetsAll(GameMain.instance));
		yield return null;
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_fpClosed = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_RemainAutoDelayTime = m_AutoDelayTime;
		Text[] textComps = new Text[4] { m_SequenceNameText, m_TitleText, m_RankNoticeText, m_ContinueText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		int iIdx = 1;
		SlotInfo[] slotInfos = m_SlotInfos;
		foreach (SlotInfo slotInfo in slotInfos)
		{
			FontManager.ResetTextFontByCurrentLanguage(slotInfo.m_DeadText);
			FontManager.ResetTextFontByCurrentLanguage(slotInfo.m_NameText);
			FontManager.ResetTextFontByCurrentLanguage(slotInfo.m_ScoreText);
			FontManager.ResetTextFontByCurrentLanguage(slotInfo.m_RankingText);
			slotInfo.m_RankingText.text = iIdx.ToString();
			iIdx++;
		}
		CommonButtonGuide btnGuide = GameGlobalUtil.GetCommonButtonGuide();
		if (btnGuide != null)
		{
			m_isPrevButtonGuideView = btnGuide.IsShow();
			btnGuide.SetShow(isShow: false);
		}
		EventEngine.GetInstance(isCreate: false)?.SetShowSkipBut(isShowSkipBut: false);
		int curSequenceIdx = GameSwitch.GetInstance().GetCurSequence();
		if (curSequenceIdx >= 0)
		{
			Xls.SequenceData data_bySwitchIdx = Xls.SequenceData.GetData_bySwitchIdx(curSequenceIdx);
			Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_bySwitchIdx.m_strIDName);
			m_SequenceNameText.text = data_byKey.m_strTxt;
		}
		m_TitleText.text = GameGlobalUtil.GetXlsProgramText("SEQUENCE_RESULT_RANK_TITLE");
		m_RankNoticeText.text = GameGlobalUtil.GetXlsProgramText("SEQUENCE_RESULT_RANK_SLOT_TITLE");
		if (m_ContinueText != null)
		{
			m_ContinueText.text = GameGlobalUtil.GetXlsProgramText("SEQUENCE_RESULT_CONTINUE");
		}
		m_strCharState_Dead = GameGlobalUtil.GetXlsProgramText("SEQUENCE_RESULT_RANK_STATE_DEAD");
		for (int j = 0; j < 5; j++)
		{
			InitSlot(j);
		}
		yield return null;
	}

	private void InitSlot(int slotIdx)
	{
		SlotInfo slotInfo = m_SlotInfos[slotIdx];
		GameSwitch instance = GameSwitch.GetInstance();
		GameSwitch.VoteRank voteRank = instance.GetVoteRank(slotIdx);
		if (voteRank != null)
		{
			slotInfo.m_SlotRootObject.SetActive(value: true);
			Xls.CharData charData = null;
			Xls.TextData textData = null;
			Xls.ImageFile imageFile = null;
			try
			{
				charData = Xls.CharData.GetData_bySwitchIdx(voteRank.m_iCharIdx);
				textData = Xls.TextData.GetData_byKey(charData.m_strNameKey);
				imageFile = Xls.ImageFile.GetData_byKey(charData.m_strRankImg);
			}
			catch (Exception)
			{
			}
			if (charData != null)
			{
				slotInfo.m_NameText.text = textData.m_strTxt;
				slotInfo.m_ScoreText.text = voteRank.m_iVoteCnt.ToString("0,0");
				if (instance.GetCharParty(voteRank.m_iCharIdx) == 2)
				{
					string imagePath = $"{imageFile.m_strAssetPath}_dead";
					slotInfo.m_FaceImage.sprite = m_characterImageMgr.GetThumbnailImageInCache(imagePath);
					slotInfo.m_DeadText.gameObject.SetActive(value: true);
					slotInfo.m_DeadText.text = m_strCharState_Dead;
				}
				else
				{
					string strAssetPath = imageFile.m_strAssetPath;
					slotInfo.m_FaceImage.sprite = m_characterImageMgr.GetThumbnailImageInCache(strAssetPath);
					slotInfo.m_DeadText.gameObject.SetActive(value: false);
				}
				return;
			}
		}
		slotInfo.m_SlotRootObject.SetActive(value: false);
	}

	public void Close()
	{
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
	}

	public void Closed()
	{
		base.gameObject.SetActive(value: false);
		m_RootObject.SetActive(value: false);
		if (m_characterImageMgr != null)
		{
			m_characterImageMgr.ClearThumbnailCaches();
			m_characterImageMgr.UnloadThumbnailBundle();
		}
		if (m_isPrevButtonGuideView)
		{
			CommonButtonGuide commonButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
			if (commonButtonGuide != null)
			{
				commonButtonGuide.SetShow(isShow: true);
			}
			EventEngine.GetInstance(isCreate: false)?.SetShowSkipBut(isShowSkipBut: true);
		}
		if (m_fpClosed != null)
		{
			m_fpClosed(this, null);
		}
	}

	public void OnClickNextBut()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			Close();
		}
	}
}
