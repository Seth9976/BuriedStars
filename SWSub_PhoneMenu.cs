using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SWSub_PhoneMenu : MonoBehaviour
{
	[Serializable]
	public class ContactSlot
	{
		public GameObject m_RootObject;

		public GameObject m_SelectionCursor;

		public Button m_PadIconButton;

		public Image m_FaceImage;

		public Text m_NameText;

		[NonSerialized]
		public Xls.CharData m_xlsData;

		public bool select
		{
			get
			{
				return m_SelectionCursor != null && m_SelectionCursor.activeSelf;
			}
			set
			{
				if (m_SelectionCursor != null)
				{
					m_SelectionCursor.SetActive(value);
				}
				if (m_PadIconButton != null)
				{
					m_PadIconButton.gameObject.SetActive(value);
				}
			}
		}

		public bool active
		{
			get
			{
				return m_RootObject != null && m_RootObject.activeSelf;
			}
			set
			{
				if (m_RootObject != null)
				{
					m_RootObject.SetActive(value);
				}
			}
		}
	}

	private enum Mode
	{
		Unknown,
		Selection,
		Engaged,
		EngagedByConti
	}

	public enum EngageState
	{
		Unknown,
		Sending,
		SendingSmall,
		Engaged,
		Finish,
		Ringing,
		EngagedShow,
		EngagedHide
	}

	public Image m_WallpaperImage;

	[Header("Selection Mode")]
	public GameObject[] m_SelectionRootObjs;

	public Text m_SelectionTitle;

	private const int c_SlotMaxCount = 9;

	private const int c_SlotColCount = 3;

	private const int c_SlotRawCount = 3;

	public ContactSlot m_Slot1 = new ContactSlot();

	public ContactSlot m_Slot2 = new ContactSlot();

	public ContactSlot m_Slot3 = new ContactSlot();

	public ContactSlot m_Slot4 = new ContactSlot();

	public ContactSlot m_Slot5 = new ContactSlot();

	public ContactSlot m_Slot6 = new ContactSlot();

	public ContactSlot m_Slot7 = new ContactSlot();

	public ContactSlot m_Slot8 = new ContactSlot();

	public ContactSlot m_Slot9 = new ContactSlot();

	private ContactSlot[] m_Slots = new ContactSlot[9];

	private ContactSlot m_OnCursorSlot;

	[Header("Engaged Mode")]
	public GameObject[] m_EngagedRootObjs;

	public Image m_EngagedFaceImage;

	public Text m_EngagedNameText;

	public Text m_EngagedStateText;

	public Animator m_TalkingAnimator;

	public Animator m_CharFaceAnimator;

	public float m_SendingTimeMin = 3f;

	public float m_SendingTimeMax = 10f;

	public float m_FinishWaitTime = 3f;

	private float m_CurWaittingTime;

	private Mode m_curMode;

	private EngageState m_curEngageState;

	private SWatchMenuPlus m_SWatchMenu;

	private GameObject m_inputBlockPanel;

	private Xls.CharData m_EngagedXlsData;

	private AudioManager m_AudioManager;

	private const int c_RingSoundChannel = 6;

	private bool m_isInputBlock;

	private bool m_isTutorialActivated;

	private float m_fScrollButtonPusingTime;

	private GameDefine.EventProc m_fpClosedProc;

	private Animator m_CheckAnimator;

	private bool m_isDisappearing;

	private string m_xlsText_Title = string.Empty;

	private string m_xlsText_EngageState_Send = string.Empty;

	private string m_xlsText_EngageState_Engaged = string.Empty;

	private string m_xlsText_EngageState_Finish = string.Empty;

	private string m_xlsText_EngageState_Ringing = string.Empty;

	private CommonButtonGuide m_ButtonGuide;

	private string m_buttonGuideText_MoveCursor = string.Empty;

	private string m_buttonGuideText_Submit = string.Empty;

	private string m_buttonGuideText_Exit = string.Empty;

	private GameDefine.EventProc m_fpChangeStateComplete;

	public EngageState curEngageState => m_curEngageState;

	public GameDefine.EventProc onChangeStateComplete
	{
		set
		{
			m_fpChangeStateComplete = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private bool InputBlocked
	{
		get
		{
			return m_isInputBlock || (m_inputBlockPanel != null && m_inputBlockPanel.activeInHierarchy);
		}
		set
		{
			m_isInputBlock = value;
			if (m_inputBlockPanel != null)
			{
				m_inputBlockPanel.SetActive(value);
			}
		}
	}

	private void OnEnable()
	{
	}

	private void OnDestroy()
	{
		m_OnCursorSlot = null;
		m_SWatchMenu = null;
		m_inputBlockPanel = null;
		m_EngagedXlsData = null;
		m_AudioManager = null;
		m_fpClosedProc = null;
		m_CheckAnimator = null;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.ClearContents();
		}
		m_ButtonGuide = null;
		m_fpChangeStateComplete = null;
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (m_CheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (m_isDisappearing && currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Closed();
			}
			return;
		}
		switch (m_curMode)
		{
		case Mode.Selection:
			Update_SelectionMode();
			break;
		case Mode.Engaged:
		case Mode.EngagedByConti:
			Update_EngageMode();
			break;
		}
	}

	private void Update_SelectionMode()
	{
		if (m_isTutorialActivated || InputBlocked || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		float fAxisX = 0f;
		float fAxisY = 0f;
		if (GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
		{
			int deltaX = 0;
			int deltaY = 0;
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					deltaX = -1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					deltaX = 1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						deltaX = -1;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					deltaX = 1;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				deltaY = -1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				deltaY = 1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					deltaY = -1;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				deltaY = 1;
			}
			ChangeOnCursorSlot(deltaX, deltaY);
		}
		if (m_OnCursorSlot != null && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCursorSlot.m_PadIconButton);
			OnProc_SelectedSlot(m_OnCursorSlot);
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuideText_Exit, isActivate: true);
			}
			Close();
		}
	}

	private void Update_EngageMode()
	{
		switch (m_curEngageState)
		{
		case EngageState.Sending:
			break;
		case EngageState.SendingSmall:
			break;
		case EngageState.Engaged:
			break;
		case EngageState.Finish:
			break;
		case EngageState.Ringing:
			break;
		case EngageState.EngagedShow:
			if (m_fpChangeStateComplete != null && m_SWatchMenu.animatorWatchSelf != null && m_SWatchMenu.animatorWatchSelf.GetCurrentAnimatorStateInfo(0).IsName(SWatchMenuPlus.SpecialAniState.move_smaller_idle.ToString()))
			{
				m_fpChangeStateComplete(null, null);
				m_fpChangeStateComplete = null;
				m_curEngageState = EngageState.Engaged;
			}
			break;
		case EngageState.EngagedHide:
			if (m_fpChangeStateComplete != null && m_SWatchMenu.animatorWatchSelf != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_SWatchMenu.animatorWatchSelf.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(SWatchMenuPlus.SpecialAniState.move_smaller_disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
				{
					m_fpChangeStateComplete(null, null);
					m_fpChangeStateComplete = null;
				}
			}
			break;
		}
	}

	public void OnClick_FaceButton(int buttonIndex)
	{
		if (buttonIndex >= 0 && buttonIndex < m_Slots.Length)
		{
			ContactSlot contactSlot = m_Slots[buttonIndex];
			if (contactSlot != null && contactSlot.m_RootObject.activeInHierarchy)
			{
				SetOnCursorSlot(contactSlot);
				OnProc_SelectedSlot(contactSlot);
			}
		}
	}

	private void OnProc_SelectedSlot(ContactSlot selectedSlot, bool isPlaySE = true)
	{
		if (isPlaySE && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Push_WatchOK");
		}
		sbyte charCallState = GameSwitch.GetInstance().GetCharCallState(selectedSlot.m_xlsData.m_strKey);
		if (charCallState == 3)
		{
			InputBlocked = true;
			PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_PHONE_DEAD"), OnClosed_CallDieActionPop);
		}
		else if (charCallState == 0)
		{
			InputBlocked = true;
			string xlsDataName = ((!(selectedSlot.m_xlsData.m_strKey == "장세일")) ? "SWATCH_POPUP_TEXT_PHONE_DO_NOT_CALL" : "SWATCH_POPUP_TEXT_CHANG_DO_NOT_CALL");
			PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText(xlsDataName), OnClosed_CallDieActionPop);
		}
		else if (charCallState == 4)
		{
			InputBlocked = true;
			PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("SWATCH_POPUP_TEXT_ALREADY_CALL"), OnClosed_CallDieActionPop);
		}
		else
		{
			m_EngagedXlsData = selectedSlot.m_xlsData;
			StartCoroutine(ChangeMode(Mode.Engaged));
		}
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_fScrollButtonPusingTime += Time.deltaTime;
		if (m_fScrollButtonPusingTime >= m_SWatchMenu.m_ScrollRepeatTime)
		{
			m_fScrollButtonPusingTime = 0f;
			return true;
		}
		return false;
	}

	public IEnumerator ShowSelectionMode(SWatchMenuPlus swatchMenu, GameDefine.EventProc fpCloseProc = null)
	{
		yield return MainLoadThing.instance.StartCoroutine(ShowCommonProc(swatchMenu, fpCloseProc));
		swatchMenu.m_TitleText.text = m_xlsText_Title;
		yield return MainLoadThing.instance.StartCoroutine(ChangeMode(Mode.Selection, isIgnoreSame: false));
		ContactSlot[] slots = new ContactSlot[9] { m_Slot1, m_Slot2, m_Slot3, m_Slot4, m_Slot5, m_Slot6, m_Slot7, m_Slot8, m_Slot9 };
		for (int i = 0; i < 9; i++)
		{
			m_Slots[i] = slots[i];
			m_Slots[i].active = false;
		}
		List<Xls.CharData> xlsCharDatas = Xls.CharData.datas;
		int dataCount = xlsCharDatas.Count;
		Xls.CharData xlsCharData = null;
		Xls.TextData xlsTextData = null;
		Xls.ImageFile xlsImageData = null;
		ContactSlot slot = null;
		for (int j = 0; j < dataCount; j++)
		{
			xlsCharData = xlsCharDatas[j];
			if (xlsCharData.m_iCallListSlot >= 0 && xlsCharData.m_iCallListSlot < 9)
			{
				slot = m_Slots[xlsCharData.m_iCallListSlot];
				slot.m_xlsData = xlsCharData;
				FontManager.ResetTextFontByCurrentLanguage(slot.m_NameText);
				xlsTextData = Xls.TextData.GetData_byKey(xlsCharData.m_strNameKey);
				slot.m_NameText.text = xlsTextData.m_strTxt;
				xlsImageData = Xls.ImageFile.GetData_byKey(xlsCharData.m_strCallListImage);
				string strPath = xlsImageData.m_strAssetPath;
				yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSprRequestFromImgPath(strPath, isOneFileBundle: false));
				slot.m_FaceImage.sprite = GameGlobalUtil.m_sprLoadFromImgXls;
				slot.active = true;
			}
		}
		for (int k = 0; k < 9; k++)
		{
			slot = m_Slots[k];
			if (slot.active)
			{
				SetOnCursorSlot(slot);
				break;
			}
		}
		InputBlocked = false;
	}

	public IEnumerator ShowEngageMode(SWatchMenuPlus swatchMenu, string dataKey, EngageState engageState, GameDefine.EventProc fpCloseProc = null)
	{
		yield return StartCoroutine(ShowCommonProc(swatchMenu, fpCloseProc));
		if (!string.IsNullOrEmpty(dataKey))
		{
			m_EngagedXlsData = Xls.CharData.GetData_byKey(dataKey);
		}
		if (m_EngagedXlsData != null)
		{
			yield return StartCoroutine(ChangeMode(Mode.EngagedByConti, isIgnoreSame: false));
			ChangeEngageState(engageState);
		}
	}

	private IEnumerator ShowCommonProc(SWatchMenuPlus swatchMenu, GameDefine.EventProc fpCloseProc = null)
	{
		GameObject[] selectionRootObjs = m_SelectionRootObjs;
		foreach (GameObject gameObject in selectionRootObjs)
		{
			gameObject.SetActive(value: false);
		}
		GameObject[] engagedRootObjs = m_EngagedRootObjs;
		foreach (GameObject gameObject2 in engagedRootObjs)
		{
			gameObject2.SetActive(value: false);
		}
		m_SWatchMenu = swatchMenu;
		m_inputBlockPanel = m_SWatchMenu.m_InputBlockObject;
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		m_fpClosedProc = ((fpCloseProc == null) ? null : new GameDefine.EventProc(fpCloseProc.Invoke));
		m_CheckAnimator = null;
		m_isDisappearing = false;
		Sprite spr = null;
		yield return MainLoadThing.instance.StartCoroutine(GameGlobalUtil.GetSWatchBGImageSpriteRequest());
		spr = GameGlobalUtil.m_sprLoadFromImgXls;
		if (spr != null && m_WallpaperImage != null)
		{
			m_WallpaperImage.sprite = spr;
		}
		Text[] textComps = new Text[3] { m_SelectionTitle, m_EngagedNameText, m_EngagedStateText };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_xlsText_Title = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_TITLE");
		m_SelectionTitle.text = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_SELECTION_TITLE");
		m_xlsText_EngageState_Send = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_ENGAGE_STATE_SEND");
		m_xlsText_EngageState_Engaged = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_ENGAGE_STATE_ENGAGED");
		m_xlsText_EngageState_Finish = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_ENGAGE_STATE_FINISH");
		m_xlsText_EngageState_Ringing = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_ENGAGE_STATE_RINGING");
		InitButtonGuide();
		yield return null;
	}

	public void Close()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		m_isDisappearing = true;
		InputBlocked = true;
		m_CheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CheckAnimator == null)
		{
			Closed();
		}
	}

	private void Closed()
	{
		base.gameObject.SetActive(value: false);
		InputBlocked = false;
		if (m_fpClosedProc != null)
		{
			m_fpClosedProc(this, null);
		}
		m_curMode = Mode.Unknown;
	}

	private void InitButtonGuide()
	{
		m_ButtonGuide = null;
		if (m_ButtonGuide == null)
		{
			m_ButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
		}
		m_buttonGuideText_MoveCursor = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_BTNGUIDE_SEL_CURSOR");
		m_buttonGuideText_Submit = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_BTNGUIDE_SUBMIT");
		m_buttonGuideText_Exit = GameGlobalUtil.GetXlsProgramText("PHONE_MENU_BTNGUIDE_EXIT");
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuideText_MoveCursor, PadInput.GameInput.LStickY, isIngoreAxis: true);
		m_ButtonGuide.AddContent(m_buttonGuideText_Submit, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuideText_Exit, PadInput.GameInput.CrossButton);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetShow(isShow: true);
	}

	public void ChangeMode_Selection()
	{
		StartCoroutine(ChangeMode(Mode.Selection));
	}

	private IEnumerator ChangeMode(Mode newMode, bool isIgnoreSame = true)
	{
		if (m_curMode == newMode && isIgnoreSame)
		{
			yield break;
		}
		switch (newMode)
		{
		case Mode.Selection:
		{
			GameObject[] selectionRootObjs2 = m_SelectionRootObjs;
			foreach (GameObject gameObject3 in selectionRootObjs2)
			{
				gameObject3.SetActive(value: true);
			}
			GameObject[] engagedRootObjs2 = m_EngagedRootObjs;
			foreach (GameObject gameObject4 in engagedRootObjs2)
			{
				gameObject4.SetActive(value: false);
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetShow(isShow: true);
			}
			StopPhoneSound();
			InputBlocked = false;
			if (m_curMode != Mode.Unknown && m_SWatchMenu != null && m_SWatchMenu.m_BackButtonAnimator != null)
			{
				m_SWatchMenu.m_BackButtonAnimator.gameObject.SetActive(value: true);
				GameGlobalUtil.PlayUIAnimation(m_SWatchMenu.m_BackButtonAnimator, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.appear));
			}
			break;
		}
		case Mode.Engaged:
		case Mode.EngagedByConti:
		{
			GameObject[] selectionRootObjs = m_SelectionRootObjs;
			foreach (GameObject gameObject in selectionRootObjs)
			{
				gameObject.SetActive(value: false);
			}
			GameObject[] engagedRootObjs = m_EngagedRootObjs;
			foreach (GameObject gameObject2 in engagedRootObjs)
			{
				gameObject2.SetActive(value: true);
			}
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetShow(isShow: false);
			}
			m_EngagedStateText.text = string.Empty;
			Xls.TextData xlsText = Xls.TextData.GetData_byKey(m_EngagedXlsData.m_strNameKey);
			m_EngagedNameText.text = xlsText.m_strTxt;
			Xls.ImageFile xlsImage = Xls.ImageFile.GetData_byKey(m_EngagedXlsData.m_strCallListImage);
			string strPath = xlsImage.m_strAssetPath;
			yield return StartCoroutine(GameGlobalUtil.GetSprRequestFromImgPath(strPath, isOneFileBundle: false));
			m_EngagedFaceImage.sprite = GameGlobalUtil.m_sprLoadFromImgXls;
			if (m_curMode == Mode.Selection)
			{
				InputBlocked = true;
				GameSwitch.GetInstance().SetCallChar(m_EngagedXlsData.m_strKey);
				string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("RUN_CALL");
				if (xlsProgramDefineStr != null)
				{
					EventEngine.GetInstance().EnableAndRunObj(xlsProgramDefineStr);
				}
				EventEngine.GetInstance().SetEventBotGuide(isShow: true);
				if (m_SWatchMenu != null && m_SWatchMenu.m_BackButtonAnimator != null)
				{
					GameGlobalUtil.PlayUIAnimation(m_SWatchMenu.m_BackButtonAnimator, GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear));
				}
			}
			break;
		}
		}
		m_curMode = newMode;
	}

	public void ChangeEngageState(EngageState engageState, bool isIgnoreSame = true)
	{
		if (m_curEngageState == engageState && isIgnoreSame)
		{
			return;
		}
		switch (m_curEngageState)
		{
		case EngageState.Sending:
			if (engageState != EngageState.SendingSmall)
			{
				StopPhoneSound();
			}
			switch (engageState)
			{
			case EngageState.Finish:
			{
				Xls.SoundFile data_byKey2 = Xls.SoundFile.GetData_byKey("통화연결종료");
				if (data_byKey2 != null)
				{
					PlayPhoneSound(data_byKey2, isLoop: false);
				}
				break;
			}
			case EngageState.Engaged:
			{
				Xls.SoundFile data_byKey = Xls.SoundFile.GetData_byKey("통화연결성공");
				if (data_byKey != null)
				{
					PlayPhoneSound(data_byKey, isLoop: false);
				}
				break;
			}
			}
			break;
		case EngageState.SendingSmall:
			StopPhoneSound();
			break;
		case EngageState.Finish:
			StopPhoneSound();
			break;
		case EngageState.Ringing:
			StopPhoneSound();
			break;
		}
		switch (engageState)
		{
		case EngageState.Sending:
			m_EngagedStateText.text = m_xlsText_EngageState_Send;
			m_CurWaittingTime = UnityEngine.Random.Range(m_SendingTimeMin, m_SendingTimeMax);
			if (m_AudioManager != null)
			{
				GameSwitch.GetInstance().PlayCharRingingBell(isPlay: true, m_EngagedXlsData.m_strKey);
			}
			break;
		case EngageState.SendingSmall:
			m_EngagedStateText.text = m_xlsText_EngageState_Send;
			if (m_AudioManager != null && m_curEngageState != EngageState.Sending)
			{
				GameSwitch.GetInstance().PlayCharRingingBell(isPlay: true, m_EngagedXlsData.m_strKey);
			}
			PlayAniPhoneEngaging(isPlay: false);
			m_SWatchMenu.ChangeState(SWatchMenuPlus.State.MoveToSmall);
			break;
		case EngageState.Engaged:
			m_EngagedStateText.text = m_xlsText_EngageState_Engaged;
			PlayAniPhoneEngaging(isPlay: false);
			if (m_SWatchMenu.curState != SWatchMenuPlus.State.MoveToSmall && m_SWatchMenu.curState != SWatchMenuPlus.State.IdleSmall)
			{
				m_SWatchMenu.ChangeState(SWatchMenuPlus.State.MoveToSmall);
			}
			if (m_CharFaceAnimator != null)
			{
				m_CharFaceAnimator.Play("call_connect");
			}
			break;
		case EngageState.Finish:
			InputBlocked = false;
			m_EngagedStateText.text = m_xlsText_EngageState_Finish;
			m_CurWaittingTime = m_FinishWaitTime;
			PlayAniPhoneEngaging(isPlay: false);
			m_SWatchMenu.ChangeState(SWatchMenuPlus.State.MoveToNormal);
			if (m_AudioManager != null)
			{
				Xls.SoundFile data_byKey4 = Xls.SoundFile.GetData_byKey("통화연결종료");
				if (data_byKey4 != null)
				{
					PlayPhoneSound(data_byKey4, isLoop: false);
				}
			}
			break;
		case EngageState.Ringing:
		{
			m_EngagedStateText.text = m_xlsText_EngageState_Ringing;
			if (!(m_AudioManager != null))
			{
				break;
			}
			int swRingtone = GameSwitch.GetInstance().GetSwRingtone();
			Xls.CollSounds data_bySwitchIdx = Xls.CollSounds.GetData_bySwitchIdx(swRingtone);
			if (data_bySwitchIdx != null)
			{
				Xls.SoundFile data_byKey3 = Xls.SoundFile.GetData_byKey(data_bySwitchIdx.m_strIDSnd);
				if (data_byKey3 != null)
				{
					PlayPhoneSound(data_byKey3);
				}
			}
			break;
		}
		case EngageState.EngagedShow:
			m_SWatchMenu.ChangeState(SWatchMenuPlus.State.AppearSmall);
			break;
		case EngageState.EngagedHide:
			m_SWatchMenu.ChangeState(SWatchMenuPlus.State.DisappearSmall);
			break;
		}
		m_curEngageState = engageState;
	}

	public void PlayAniPhoneEngaging(bool isPlay)
	{
		if (isPlay)
		{
			m_TalkingAnimator.Rebind();
			m_TalkingAnimator.Play("playing");
		}
		else
		{
			m_TalkingAnimator.Play(GameDefine.UIAnimationState.idle.ToString());
		}
	}

	public string GetCurSelChar()
	{
		string result = null;
		if (m_EngagedXlsData != null)
		{
			result = m_EngagedXlsData.m_strKey;
		}
		return result;
	}

	private void SetOnCursorSlot(ContactSlot slot)
	{
		if (m_OnCursorSlot != null)
		{
			m_OnCursorSlot.select = false;
		}
		if (slot != null)
		{
			slot.select = true;
		}
		m_OnCursorSlot = slot;
	}

	private void ChangeOnCursorSlot(int deltaX, int deltaY)
	{
		if (deltaX == 0 && deltaY == 0)
		{
			return;
		}
		int num = ((m_OnCursorSlot != null) ? Array.IndexOf(m_Slots, m_OnCursorSlot) : 0);
		int num2 = num / 3;
		int num3 = num % 3;
		int num4 = num;
		int num5 = num2;
		int num6 = num3;
		do
		{
			if (deltaX != 0)
			{
				num6 += ((deltaX > 0) ? 1 : (-1));
				if (num6 < 0)
				{
					num6 = 2;
				}
				else if (num6 >= 3)
				{
					num6 = 0;
				}
			}
			if (deltaY != 0)
			{
				num5 += ((deltaY > 0) ? 1 : (-1));
				if (num5 < 0)
				{
					num5 = 2;
				}
				else if (num5 >= 3)
				{
					num5 = 0;
				}
			}
			num4 = num5 * 3 + num6;
		}
		while (!m_Slots[num4].active && num4 != num);
		SetOnCursorSlot(m_Slots[num4]);
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentActivate(m_buttonGuideText_MoveCursor, isActivate: true);
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
	}

	private void PlayPhoneSound(Xls.SoundFile xlsSoundFile, bool isLoop = true)
	{
		if (!(m_AudioManager == null))
		{
			StartCoroutine(m_AudioManager.PlayFileName(6, xlsSoundFile.m_strFileName, isSetVol: false, 0f, 0f, isLoop));
		}
	}

	private void StopPhoneSound()
	{
		if (!(m_AudioManager == null))
		{
			m_AudioManager.Stop(6);
		}
	}

	private void ShowTutorialPopup()
	{
		string strTutorialKey = "tuto_00024";
		m_isTutorialActivated = TutorialPopup.isShowAble(strTutorialKey);
		if (m_isTutorialActivated)
		{
			StartCoroutine(TutorialPopup.Show(strTutorialKey, OnClosed_TutorialPopup, base.gameObject.GetComponentInParent<Canvas>()));
		}
		if (m_isTutorialActivated && m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
	}

	private void OnClosed_TutorialPopup(object sender, object arg)
	{
		m_isTutorialActivated = false;
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: true);
		}
	}

	private void OnClosed_CallDieActionPop(PopupDialoguePlus.Result result)
	{
		InputBlocked = false;
	}
}
