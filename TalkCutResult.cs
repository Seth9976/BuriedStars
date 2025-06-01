using System;
using System.Collections;
using System.Globalization;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class TalkCutResult : MonoBehaviour
{
	[Serializable]
	public class SlotInfo
	{
		public int m_CharacterIndex;

		public GameObject m_SlotRootObj;

		public Image m_CharacterImage;

		public Image m_OverlayerImage;

		public Text m_CharacterNameText;

		public GameObject m_MentalRootObj;

		public Image m_MentalArrawImage;

		public Image m_MentalIconImage;

		public Text m_MentalText;

		public GameObject m_ConditionRootObj;

		public Image m_ConditionIconImage;

		public Text m_ConditionText;

		public Text m_DeadText;
	}

	public enum Mode
	{
		TalkCutResult,
		SurvivorResult
	}

	public GameObject m_RootObject;

	public Animator m_BasedAnimator;

	public Text m_TitleSequenceText;

	public Text m_TitleText;

	public Text m_TitleResultText;

	public float m_AutoDelayTime = 5f;

	[Header("Result Infos")]
	public GameObject m_ResultInfoRoot;

	public Text m_KeywordGetTitle;

	public Text m_KeywordGetCount;

	public Text m_ProfileGetTitle;

	public Text m_ProfileGetCount;

	[Header("Under Bar")]
	public Text m_ContinueText;

	public Button m_PadIconButton;

	[Header("Slot Infos")]
	public SlotInfo m_SlotInfo0 = new SlotInfo();

	public SlotInfo m_SlotInfo1 = new SlotInfo();

	public SlotInfo m_SlotInfo2 = new SlotInfo();

	public SlotInfo m_SlotInfo3 = new SlotInfo();

	public SlotInfo m_SlotInfo4 = new SlotInfo();

	public SlotInfo m_SlotInfo5 = new SlotInfo();

	private const int c_SlotCount = 6;

	private SlotInfo[] m_SlotInfos = new SlotInfo[6];

	[Header("Over Layer Sprite")]
	public Sprite m_OverLayerNormal;

	public Sprite m_OverLayerUp;

	public Sprite m_OverLayerDown;

	public Sprite m_OverLayerDead;

	[Header("Talk Cut State Icons")]
	public Sprite m_HeartIconNormal;

	public Sprite m_HeartIconUp;

	public Sprite m_HeartIconDown;

	public Sprite m_ArrowIconUp1;

	public Sprite m_ArrowIconUp2;

	public Sprite m_ArrowIconUp3;

	public Sprite m_ArrowIconDown1;

	public Sprite m_ArrowIconDown2;

	public Sprite m_ArrowIconDown3;

	[Header("Survivor State Icons")]
	public Sprite m_StateIconNormal;

	public Sprite m_StateIconBreak;

	public Sprite m_StateIconMoreLess;

	public Sprite m_StateIconLess;

	public Sprite m_StateIconBeyond;

	public Sprite m_StateIconPartyOut;

	[Header("Text Color Infos")]
	public Color m_TextColorNormal = Color.black;

	public Color m_TextColorGood = Color.black;

	public Color m_TextColorBad = Color.black;

	private Mode m_Mode;

	private GameDefine.EventProc m_fpClosed;

	private string m_strMentalDelta_NotChanged = string.Empty;

	private string m_strMentalDelta_Down = string.Empty;

	private string m_strMentalDelta_Up = string.Empty;

	private string m_strRelationDelta_NotChanged = string.Empty;

	private string m_strRelationDelta_Down = string.Empty;

	private string m_strRelationDelta_Up = string.Empty;

	private string m_strCharState_PartyOut = string.Empty;

	private string m_strCharState_Dead = string.Empty;

	private string m_strCharState_Relation_Break = string.Empty;

	private string m_strCharState_Relation_Distrust = string.Empty;

	private string m_strCharState_Relation_Normal = string.Empty;

	private string m_strCharState_Relation_Trust = string.Empty;

	private string m_strCharState_Relation_Soulmate = string.Empty;

	private string m_strCharState_Mental_Break = string.Empty;

	private string m_strCharState_Mental_MoreLess = string.Empty;

	private string m_strCharState_Mental_Less = string.Empty;

	private string m_strCharState_Mental_Normal = string.Empty;

	private string m_strCharState_Mental_Beyond = string.Empty;

	private float m_MentalBound_Less;

	private float m_MentalBound_MoreLess;

	private float m_DeltaBound_Small;

	private float m_DeltaBound_Midium;

	private float m_RelationDeltaBound_Small;

	private float m_RelationDeltaBound_Midium;

	private AudioManager m_AudioManager;

	private bool m_isPrevButtonGuideView;

	private float m_RemainedAutoDelayTime;

	private const string c_characterImageBundleName = "image/sequence_cut";

	private ContentThumbnailManager m_characterImageMgr = new ContentThumbnailManager("image/sequence_cut");

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
			if (PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString()))
			{
				if (EventEngine.GetInstance().GetAuto())
				{
					m_RemainedAutoDelayTime -= Time.deltaTime;
					if (m_RemainedAutoDelayTime <= 0f)
					{
						Close();
					}
				}
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
				{
					GoNextState();
				}
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				GoNextState();
			}
		}
	}

	public void GoNextState()
	{
		AnimatorStateInfo currentAnimatorStateInfo = m_BasedAnimator.GetCurrentAnimatorStateInfo(0);
		if (PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString()))
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_PadIconButton);
			ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_PadIconButton);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			Close();
		}
		else
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_PadIconButton);
			ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_PadIconButton);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.idle.ToString());
		}
	}

	public IEnumerator Show(Mode mode, GameDefine.EventProc fpClosed = null)
	{
		Text[] textComps = new Text[8] { m_TitleSequenceText, m_TitleText, m_TitleResultText, m_KeywordGetTitle, m_KeywordGetCount, m_ProfileGetTitle, m_ProfileGetCount, m_ContinueText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_SlotInfos[0] = m_SlotInfo0;
		m_SlotInfos[1] = m_SlotInfo1;
		m_SlotInfos[2] = m_SlotInfo2;
		m_SlotInfos[3] = m_SlotInfo3;
		m_SlotInfos[4] = m_SlotInfo4;
		m_SlotInfos[5] = m_SlotInfo5;
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		yield return GameMain.instance.StartCoroutine(m_characterImageMgr.LoadAssetsAll(GameMain.instance));
		yield return null;
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_Mode = mode;
		m_fpClosed = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_RemainedAutoDelayTime = m_AutoDelayTime;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		int curTalkCutIndex = GameSwitch.GetInstance().GetCurCutIdx();
		if (curTalkCutIndex >= 0)
		{
			Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(curTalkCutIndex);
			m_TitleSequenceText.text = GameGlobalUtil.GetXlsTextData(data_bySwitchIdx.m_strIDCutName);
		}
		m_ContinueText.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_CONTINUE");
		CommonButtonGuide btnGuide = GameGlobalUtil.GetCommonButtonGuide();
		if (btnGuide != null)
		{
			m_isPrevButtonGuideView = btnGuide.IsShow();
			btnGuide.SetShow(isShow: false);
		}
		EventEngine.GetInstance(isCreate: false)?.SetShowSkipBut(isShowSkipBut: false);
		m_strMentalDelta_NotChanged = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_MENTAL_NOT_CHANGED");
		m_strMentalDelta_Down = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_MENTAL_DOWN");
		m_strMentalDelta_Up = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_MENTAL_UP");
		m_strRelationDelta_NotChanged = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_RELATION_NOT_CHANGED");
		m_strRelationDelta_Down = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_RELATION_DOWN");
		m_strRelationDelta_Up = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_RELATION_UP");
		m_strCharState_PartyOut = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_PARTYOUT");
		m_strCharState_Dead = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_DEAD");
		m_strCharState_Relation_Break = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_RELATION_BREAK");
		m_strCharState_Relation_Distrust = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_RELATION_DISTRUST");
		m_strCharState_Relation_Normal = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_RELATION_NORMAL");
		m_strCharState_Relation_Trust = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_RELATION_TRUST");
		m_strCharState_Relation_Soulmate = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_RELATION_SOULMATE");
		m_strCharState_Mental_Break = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_MENTAL_BREAK");
		m_strCharState_Mental_MoreLess = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_MENTAL_MORELESS");
		m_strCharState_Mental_Less = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_MENTAL_LESS");
		m_strCharState_Mental_Normal = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_MENTAL_NORMAL");
		m_strCharState_Mental_Beyond = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_STATE_MENTAL_BEYOND");
		Xls.ProgramDefineStr xlsData = null;
		xlsData = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_LESS_BOUND");
		m_MentalBound_Less = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		m_MentalBound_Less *= ConstGameSwitch.MAX_MENTAL_POINT;
		xlsData = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_MORELESS_BOUND");
		m_MentalBound_MoreLess = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		m_MentalBound_MoreLess *= ConstGameSwitch.MAX_MENTAL_POINT;
		xlsData = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_DELTA_SMALL_BOUND");
		m_DeltaBound_Small = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		xlsData = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_DELTA_MEDIUM_BOUND");
		m_DeltaBound_Midium = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		xlsData = Xls.ProgramDefineStr.GetData_byKey("RELATION_DELTA_SMALL_BOUND");
		m_RelationDeltaBound_Small = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		xlsData = Xls.ProgramDefineStr.GetData_byKey("RELATION_DELTA_MIDIUM_BOUND");
		m_RelationDeltaBound_Midium = float.Parse(xlsData.m_strTxt, CultureInfo.InvariantCulture);
		switch (m_Mode)
		{
		case Mode.TalkCutResult:
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_TITLE_NORMAL");
			m_TitleResultText.gameObject.SetActive(value: true);
			m_TitleResultText.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_TEXT");
			m_ResultInfoRoot.gameObject.SetActive(value: true);
			m_KeywordGetTitle.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_NEW_KEYWORD");
			m_KeywordGetCount.text = gameSwitch.GetCutKeywordCnt().ToString();
			m_ProfileGetTitle.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_NEW_PROFILE");
			m_ProfileGetCount.text = gameSwitch.GetCutProfileCnt().ToString();
			break;
		case Mode.SurvivorResult:
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText("TALK_CUT_RESULT_TITLE_SURVIVOR");
			m_TitleResultText.gameObject.SetActive(value: false);
			m_ResultInfoRoot.gameObject.SetActive(value: false);
			break;
		}
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		SlotInfo[] slotInfos = m_SlotInfos;
		foreach (SlotInfo slotInfo in slotInfos)
		{
			InitSlot(slotInfo);
		}
		yield return null;
	}

	private void InitSlot(SlotInfo slotInfo)
	{
		Text[] textComps = new Text[4] { slotInfo.m_CharacterNameText, slotInfo.m_ConditionText, slotInfo.m_DeadText, slotInfo.m_MentalText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		Xls.CharData charData = null;
		Xls.TextData textData = null;
		Xls.ImageFile imageFile = null;
		try
		{
			charData = Xls.CharData.GetData_bySwitchIdx(slotInfo.m_CharacterIndex);
			textData = Xls.TextData.GetData_byKey(charData.m_strNameKey);
			imageFile = Xls.ImageFile.GetData_byKey(charData.m_strResultSlotImage);
		}
		catch (Exception)
		{
		}
		if (charData == null)
		{
			slotInfo.m_SlotRootObj.SetActive(value: false);
			return;
		}
		GameSwitch instance = GameSwitch.GetInstance();
		int charParty = instance.GetCharParty(slotInfo.m_CharacterIndex);
		slotInfo.m_SlotRootObj.SetActive(value: true);
		if (charParty == 2)
		{
			slotInfo.m_CharacterNameText.gameObject.SetActive(value: false);
			string imagePath = $"{imageFile.m_strAssetPath}_dead";
			slotInfo.m_CharacterImage.sprite = m_characterImageMgr.GetThumbnailImageInCache(imagePath);
			slotInfo.m_DeadText.gameObject.SetActive(value: true);
			slotInfo.m_DeadText.text = m_strCharState_Dead;
			slotInfo.m_OverlayerImage.sprite = m_OverLayerDead;
			slotInfo.m_MentalRootObj.SetActive(value: false);
			slotInfo.m_ConditionRootObj.SetActive(value: false);
			return;
		}
		slotInfo.m_CharacterNameText.gameObject.SetActive(value: true);
		slotInfo.m_CharacterNameText.text = textData.m_strTxt;
		slotInfo.m_CharacterImage.sprite = m_characterImageMgr.GetThumbnailImageInCache(imageFile.m_strAssetPath);
		slotInfo.m_DeadText.gameObject.SetActive(value: false);
		if (charParty == 0)
		{
			slotInfo.m_MentalRootObj.SetActive(value: false);
			slotInfo.m_ConditionRootObj.SetActive(value: true);
			slotInfo.m_OverlayerImage.sprite = m_OverLayerNormal;
			slotInfo.m_ConditionIconImage.sprite = m_StateIconPartyOut;
			slotInfo.m_ConditionText.text = m_strCharState_PartyOut;
			slotInfo.m_ConditionText.color = m_TextColorNormal;
			return;
		}
		switch (m_Mode)
		{
		case Mode.TalkCutResult:
		{
			slotInfo.m_MentalRootObj.SetActive(value: true);
			slotInfo.m_ConditionRootObj.SetActive(value: false);
			int num = ((slotInfo.m_CharacterIndex != 0) ? instance.GetCutStartRelation(slotInfo.m_CharacterIndex) : instance.GetCutStartMental());
			int num2 = ((slotInfo.m_CharacterIndex != 0) ? instance.GetCharRelation(slotInfo.m_CharacterIndex) : instance.GetMental());
			float num3 = ((slotInfo.m_CharacterIndex != 0) ? m_RelationDeltaBound_Small : m_DeltaBound_Small);
			float num4 = ((slotInfo.m_CharacterIndex != 0) ? m_RelationDeltaBound_Midium : m_DeltaBound_Midium);
			if (num2 == num)
			{
				slotInfo.m_OverlayerImage.sprite = m_OverLayerNormal;
				slotInfo.m_MentalArrawImage.gameObject.SetActive(value: false);
				slotInfo.m_MentalIconImage.sprite = m_HeartIconNormal;
				slotInfo.m_MentalText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strRelationDelta_NotChanged : m_strMentalDelta_NotChanged);
				slotInfo.m_MentalText.color = m_TextColorNormal;
			}
			else if (num2 > num)
			{
				slotInfo.m_OverlayerImage.sprite = m_OverLayerUp;
				slotInfo.m_MentalArrawImage.gameObject.SetActive(value: true);
				slotInfo.m_MentalIconImage.sprite = m_HeartIconUp;
				slotInfo.m_MentalText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strRelationDelta_Up : m_strMentalDelta_Up);
				slotInfo.m_MentalText.color = m_TextColorGood;
				float num5 = num2 - num;
				if (num5 > num4)
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconUp3;
				}
				else if (num5 > num3)
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconUp2;
				}
				else
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconUp1;
				}
			}
			else
			{
				slotInfo.m_OverlayerImage.sprite = m_OverLayerDown;
				slotInfo.m_MentalArrawImage.gameObject.SetActive(value: true);
				slotInfo.m_MentalIconImage.sprite = m_HeartIconDown;
				slotInfo.m_MentalText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strRelationDelta_Down : m_strMentalDelta_Down);
				slotInfo.m_MentalText.color = m_TextColorBad;
				float num6 = num - num2;
				if (num6 > num4)
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconDown3;
				}
				else if (num6 > num3)
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconDown2;
				}
				else
				{
					slotInfo.m_MentalArrawImage.sprite = m_ArrowIconDown1;
				}
			}
			break;
		}
		case Mode.SurvivorResult:
			slotInfo.m_MentalRootObj.SetActive(value: false);
			slotInfo.m_ConditionRootObj.SetActive(value: true);
			slotInfo.m_ConditionIconImage.sprite = IconCondition.instance.GetConditionSprite(charData.m_strKey);
			switch (instance.GetCharIconState(charData.m_strKey))
			{
			case GameSwitch.eChrIconState.Full:
				slotInfo.m_OverlayerImage.sprite = m_OverLayerUp;
				slotInfo.m_ConditionText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strCharState_Relation_Soulmate : m_strCharState_Mental_Beyond);
				slotInfo.m_ConditionText.color = m_TextColorGood;
				break;
			case GameSwitch.eChrIconState.High:
				slotInfo.m_OverlayerImage.sprite = m_OverLayerUp;
				slotInfo.m_ConditionText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strCharState_Relation_Trust : m_strCharState_Mental_Normal);
				slotInfo.m_ConditionText.color = m_TextColorGood;
				break;
			case GameSwitch.eChrIconState.Medium:
				slotInfo.m_OverlayerImage.sprite = m_OverLayerNormal;
				slotInfo.m_ConditionText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strCharState_Relation_Normal : m_strCharState_Mental_Less);
				slotInfo.m_ConditionText.color = m_TextColorNormal;
				break;
			case GameSwitch.eChrIconState.Low:
				slotInfo.m_OverlayerImage.sprite = m_OverLayerDown;
				slotInfo.m_ConditionText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strCharState_Relation_Distrust : m_strCharState_Mental_MoreLess);
				slotInfo.m_ConditionText.color = m_TextColorBad;
				break;
			case GameSwitch.eChrIconState.Zero:
				slotInfo.m_OverlayerImage.sprite = m_OverLayerDown;
				slotInfo.m_ConditionText.text = ((slotInfo.m_CharacterIndex != 0) ? m_strCharState_Relation_Break : m_strCharState_Mental_Break);
				slotInfo.m_ConditionText.color = m_TextColorBad;
				break;
			}
			break;
		}
	}

	public void Close()
	{
		GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
	}

	public void Closed()
	{
		base.gameObject.SetActive(value: false);
		m_RootObject.SetActive(value: false);
		SlotInfo[] slotInfos = m_SlotInfos;
		foreach (SlotInfo slotInfo in slotInfos)
		{
			if (slotInfo != null && !(slotInfo.m_CharacterImage == null))
			{
				slotInfo.m_CharacterImage.sprite = null;
			}
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
		if (m_characterImageMgr != null)
		{
			m_characterImageMgr.ClearThumbnailCaches();
			m_characterImageMgr.UnloadThumbnailBundle();
		}
		Resources.UnloadUnusedAssets();
		if (m_fpClosed != null)
		{
			m_fpClosed(this, null);
		}
	}

	public void TouchGoNextState()
	{
		if (!m_BasedAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.disappear.ToString()) && MainLoadThing.instance.IsTouchableState())
		{
			GoNextState();
		}
	}
}
