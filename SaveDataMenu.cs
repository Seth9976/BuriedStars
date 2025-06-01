using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveDataMenu : SystemMenuBase
{
	public enum Type
	{
		Save,
		Load
	}

	public Type m_Type;

	public Text m_TitleText;

	public Text m_NoticeText;

	[Header("Slot Container")]
	public GameObject m_SrcSlotObject;

	public RectTransform m_SlotContainerRT;

	public int m_SlotInterval;

	public bool m_CursorLoopEnable = true;

	public ScrollRect m_ScrollRect;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	public float m_BottomSpacing = 80f;

	private float m_fScrollButtonPusingTime;

	private bool m_isPageScrolling;

	private SaveDataSlot[] m_DataSlots;

	private SaveDataSlot m_OnCursorSlot;

	private int m_LoadableSlotIdx = -1;

	private static Dictionary<int, Sprite> s_SaveImagesBySeqIdx = new Dictionary<int, Sprite>();

	private string m_LocationTextFormat;

	private string m_RealTimeTextFormat;

	private string m_autoSlotNumberTag = string.Empty;

	private const string c_SceneName = "Scene/GameMain";

	protected override void Awake()
	{
		base.Awake();
		m_SrcSlotObject.SetActive(value: false);
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		ClearSlots();
		m_OnCursorSlot = null;
		if (s_SaveImagesBySeqIdx != null)
		{
			s_SaveImagesBySeqIdx.Clear();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_AppearCheckAnimator != null)
		{
			if (m_AppearCheckAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.idle)))
			{
				m_AppearCheckAnimator = null;
			}
		}
		else
		{
			if (m_DisappearCheckAnimator != null)
			{
				return;
			}
			if (m_ScrollHandler.IsScrolling)
			{
				m_ScrollHandler.Update();
				bool flag = true;
				if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorSlot != null)
				{
					ChangeOnCursorContent_byScrollPos();
					Vector2 zero = Vector2.zero;
					if (GamePadInput.GetLStickMove(out zero.x, out zero.y) || GamePadInput.GetLStickMove(out zero.x, out zero.y))
					{
						m_ScrollRect.velocity = Vector2.zero;
						flag = false;
					}
				}
				if (flag)
				{
					return;
				}
			}
			if (base.isInputBlock || PopupDialoguePlus.IsAnyPopupActivated() || ButtonPadInput.IsPlayingButPressAnim())
			{
				return;
			}
			float num = 0f;
			if (m_ScrollHandler.isScrollable)
			{
				bool flag2 = false;
				num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
				if (!GameGlobalUtil.IsAlmostSame(num, 0f))
				{
					m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
					flag2 = true;
				}
				else
				{
					flag2 = !m_ScrollHandler.IsScrolling && m_isPageScrolling;
					if (flag2)
					{
						m_isPageScrolling = false;
					}
					if (!m_ScrollHandler.IsScrolling)
					{
						num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickX);
						if (GameGlobalUtil.IsAlmostSame(num, 0f))
						{
							num = 0f - GamePadInput.GetMouseWheelScrollDelta();
						}
						if (num > 0.9f)
						{
							m_isPageScrolling = ChangeScrollPage(isUpSide: false);
						}
						else if (num < -0.9f)
						{
							m_isPageScrolling = ChangeScrollPage(isUpSide: true);
						}
						if (m_isPageScrolling)
						{
							flag2 = false;
						}
					}
				}
				if (flag2)
				{
					ChangeOnCursorContent_byScrollPos();
				}
			}
			num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickY);
			if (Mathf.Abs(num) > 0.5f)
			{
				if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
				{
					ChangeOnCursorDataSlot(isUpSide: true);
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
				{
					ChangeOnCursorDataSlot(isUpSide: false);
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
				{
					m_fScrollButtonPusingTime += Time.deltaTime;
					if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
					{
						m_fScrollButtonPusingTime = 0f;
						ChangeOnCursorDataSlot(isUpSide: true);
					}
				}
				else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
				{
					m_fScrollButtonPusingTime += Time.deltaTime;
					if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
					{
						m_fScrollButtonPusingTime = 0f;
						ChangeOnCursorDataSlot(isUpSide: false);
					}
				}
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_OK");
				}
				if (m_OnCursorSlot != null && m_OnCursorSlot.m_SelSlotData != null)
				{
					GameGlobalUtil.PlayUIAnimation_WithChidren(m_OnCursorSlot.m_SelSlotData.m_RootObject, "steam_push");
				}
				ProcOnCursorDataSlot();
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				if (m_OutterExitButton != null)
				{
					ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_OutterExitButton, null, null, null, null, isShowAnim: true, isExcuteEvent: false);
				}
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Cancel");
				}
				Close();
			}
		}
	}

	public override void Show(bool isEnableAnimation = true, GameDefine.EventProc fpClosedCB = null, bool isNeedSetCloseCB = true, Button outterExitButton = null)
	{
		m_LocationTextFormat = GameGlobalUtil.GetXlsProgramText("SAVELOAD_MENU_LOCATION_TEXT_FMT");
		m_RealTimeTextFormat = GameGlobalUtil.GetXlsProgramText("REAL_TIME_TEXT_FMT");
		m_autoSlotNumberTag = GameGlobalUtil.GetXlsProgramText("LOAD_MENU_AUTO_SLOT_TEXT");
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_SlotContainerRT);
		if (m_ScrollRect != null)
		{
			m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		}
		InitDataSlots();
		ResetXlsTexts();
		m_LoadableSlotIdx = -1;
		SetOnCursorDataSlot(null);
		object[] etcArgs = new object[3] { fpClosedCB, isNeedSetCloseCB, outterExitButton };
		base.isInputBlock = true;
		MainLoadThing.instance.StartCoroutine(ShowSlots(isEnableAnimation, etcArgs));
	}

	public override void ResetXlsTexts()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
		FontManager.ResetTextFontByCurrentLanguage(m_NoticeText);
		if (m_TitleText != null)
		{
			m_TitleText.text = GameGlobalUtil.GetXlsProgramText((m_Type != Type.Save) ? "LOAD_MENU_TITLE" : "SAVE_MENU_TITLE");
		}
		if (m_NoticeText != null)
		{
			m_NoticeText.text = GameGlobalUtil.GetXlsProgramText((m_Type != Type.Save) ? "LOAD_MENU_NOTICE" : "SAVE_MENU_NOTICE");
		}
	}

	protected override void Closed()
	{
		base.gameObject.SetActive(value: false);
		if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, m_LoadableSlotIdx);
		}
	}

	private void InitDataSlots()
	{
		ClearSlots();
		int num = ((m_Type != Type.Save) ? 16 : SaveLoad.USER_SLOT_CNT);
		if (!(m_SlotContainerRT == null) && num > 0 && !(m_SrcSlotObject == null))
		{
			m_DataSlots = new SaveDataSlot[num];
			float num2 = 0f;
			float num3 = 0f;
			GameObject gameObject = null;
			SaveDataSlot saveDataSlot = null;
			float num4 = 0f;
			for (int i = 0; i < num; i++)
			{
				gameObject = UnityEngine.Object.Instantiate(m_SrcSlotObject);
				gameObject.name = $"Slot_{i}";
				saveDataSlot = gameObject.GetComponent<SaveDataSlot>();
				m_DataSlots[i] = saveDataSlot;
				saveDataSlot.rectTransform.SetParent(m_SlotContainerRT, worldPositionStays: false);
				saveDataSlot.rectTransform.anchoredPosition = new Vector2(saveDataSlot.rectTransform.anchoredPosition.x, num3);
				num2 = saveDataSlot.rectTransform.rect.height * saveDataSlot.rectTransform.localScale.y;
				num3 -= num2 + (float)m_SlotInterval;
				saveDataSlot.slotIndex = i;
				saveDataSlot.slotNumberText = "#" + ((m_Type != Type.Save) ? i : (i + 1));
				saveDataSlot.enableSlotNumberText = true;
				saveDataSlot.enableSlotAutoText = false;
				gameObject.SetActive(value: false);
				num4 += num2;
			}
			float size = 0f;
			if (num > 0)
			{
				num4 += (float)m_SlotInterval * (float)(num - 1);
				num4 += m_BottomSpacing;
				size = Mathf.Max(m_ScrollRect.viewport.rect.height, num4);
			}
			m_SlotContainerRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			m_ScrollHandler.ResetScrollRange();
			if (m_Type == Type.Load && num > 0)
			{
				SaveDataSlot saveDataSlot2 = m_DataSlots[0];
				saveDataSlot2.slotAutoText = m_autoSlotNumberTag;
				saveDataSlot2.enableSlotAutoText = true;
				saveDataSlot2.enableSlotNumberText = false;
			}
		}
	}

	private void ClearSlots()
	{
		if (m_DataSlots == null)
		{
			return;
		}
		int num = m_DataSlots.Length;
		SaveDataSlot saveDataSlot = null;
		for (int i = 0; i < num; i++)
		{
			saveDataSlot = m_DataSlots[i];
			if (!(saveDataSlot == null))
			{
				UnityEngine.Object.Destroy(saveDataSlot.gameObject);
			}
		}
		m_DataSlots = null;
	}

	private IEnumerator ShowSlots(bool isEnableAnimation = true, object[] etcArgs = null)
	{
		if (m_DataSlots != null)
		{
			int count = m_DataSlots.Length;
			SaveDataSlot dataSlot = null;
			for (int i = 0; i < count; i++)
			{
				dataSlot = m_DataSlots[i];
				dataSlot.gameObject.SetActive(value: false);
				dataSlot.isOnCursor = false;
				yield return MainLoadThing.instance.StartCoroutine(SetSlotInfo(dataSlot));
			}
			for (int j = 0; j < count; j++)
			{
				m_DataSlots[j].gameObject.SetActive(value: true);
			}
			if (m_DataSlots != null && m_DataSlots.Length > 0)
			{
				SetOnCursorDataSlot(m_DataSlots[0], isAdjustScrollPos: false);
			}
			if (etcArgs != null && etcArgs.Length >= 3)
			{
				GameDefine.EventProc fpClosedCB = (GameDefine.EventProc)etcArgs[0];
				bool isNeedSetCloseCB = (bool)etcArgs[1];
				Button outterExitButton = (Button)etcArgs[2];
				base.Show(isEnableAnimation, fpClosedCB, isNeedSetCloseCB, outterExitButton);
				m_ScrollHandler.ResetScrollRange();
				m_ScrollHandler.scrollPos = 0f;
				base.isInputBlock = false;
			}
		}
	}

	private IEnumerator SetSlotInfo(SaveDataSlot dataSlot)
	{
		int slotIdx = dataSlot.slotIndex;
		SaveLoad cSaveLoad = SaveLoad.GetInstance();
		SaveLoad.cSaveSlotInfo cSlotInfo = cSaveLoad.GetSlotInfo((m_Type != Type.Save) ? slotIdx : (slotIdx + 1));
		if (cSlotInfo != null && cSlotInfo.m_isSave)
		{
			Xls.TalkCutSetting cutData = Xls.TalkCutSetting.GetData_bySwitchIdx(cSlotInfo.m_iCutIdx);
			Xls.SequenceData seqData = Xls.SequenceData.GetData_bySwitchIdx(cSlotInfo.m_iSeqIdx);
			dataSlot.noDataSlot = false;
			string format = (string.IsNullOrEmpty(m_LocationTextFormat) ? "{0} - {1}" : m_LocationTextFormat);
			string loactionText = GameGlobalUtil.GetXlsTextData(cutData.m_strIDCutName);
			string InGameTimeText = GameGlobalUtil.GetGameTimeString(cSlotInfo.m_fGameTime);
			if (loactionText != null)
			{
				dataSlot.locationText = string.Format(format, loactionText, InGameTimeText);
			}
			format = (string.IsNullOrEmpty(m_RealTimeTextFormat) ? "{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}" : m_RealTimeTextFormat);
			dataSlot.dateText = string.Format(format, cSlotInfo.m_iCurYear, cSlotInfo.m_iCurMonth, cSlotInfo.m_iCurDay, cSlotInfo.m_iCurH, cSlotInfo.m_iCurM);
			Sprite slotImage = null;
			if (!s_SaveImagesBySeqIdx.ContainsKey(cSlotInfo.m_iSeqIdx) || s_SaveImagesBySeqIdx[cSlotInfo.m_iSeqIdx] == null)
			{
				Xls.ImageFile xlsImage = Xls.ImageFile.GetData_byKey(seqData.m_strSaveImg);
				if (xlsImage != null)
				{
					yield return GameGlobalUtil.GetSprRequestFromImgPath(xlsImage.m_strAssetPath);
					slotImage = GameGlobalUtil.m_sprLoadFromImgXls;
					if (slotImage != null)
					{
						if (s_SaveImagesBySeqIdx.ContainsKey(cSlotInfo.m_iSeqIdx))
						{
							s_SaveImagesBySeqIdx[cSlotInfo.m_iSeqIdx] = slotImage;
						}
						else
						{
							s_SaveImagesBySeqIdx.Add(cSlotInfo.m_iSeqIdx, slotImage);
						}
					}
				}
			}
			else
			{
				slotImage = s_SaveImagesBySeqIdx[cSlotInfo.m_iSeqIdx];
			}
			dataSlot.image = slotImage;
		}
		else
		{
			dataSlot.noDataSlot = true;
		}
		yield return null;
	}

	private void SetOnCursorDataSlot(SaveDataSlot dataSlot, bool isAdjustScrollPos = true)
	{
		if (!(m_OnCursorSlot == dataSlot))
		{
			if (m_OnCursorSlot != null)
			{
				m_OnCursorSlot.isOnCursor = false;
			}
			if (dataSlot != null)
			{
				dataSlot.isOnCursor = true;
			}
			m_OnCursorSlot = dataSlot;
			if (isAdjustScrollPos)
			{
				AdjustScrollPos_byOnCursonContent();
			}
		}
	}

	private bool ChangeOnCursorDataSlot(bool isUpSide)
	{
		int num = m_DataSlots.Length;
		if (m_OnCursorSlot == null)
		{
			if (num > 0)
			{
				SetOnCursorDataSlot(m_DataSlots[0]);
			}
			return false;
		}
		int num2 = Array.IndexOf(m_DataSlots, m_OnCursorSlot);
		num2 = ((!isUpSide) ? (num2 + 1) : (num2 - 1));
		if (num2 < 0)
		{
			if (!m_CursorLoopEnable)
			{
				return false;
			}
			num2 = num - 1;
		}
		else if (num2 >= num)
		{
			if (!m_CursorLoopEnable)
			{
				return false;
			}
			num2 = 0;
		}
		SetOnCursorDataSlot(m_DataSlots[num2]);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		return true;
	}

	private bool ChangeScrollPage(bool isUpSide)
	{
		if (!m_ScrollHandler.isScrollable || m_ScrollHandler.IsScrolling)
		{
			return false;
		}
		if ((isUpSide && m_ScrollHandler.scrollPos <= 0f) || (!isUpSide && m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max))
		{
			return false;
		}
		float num = m_ScrollRect.viewport.rect.height - m_BottomSpacing;
		float value = m_ScrollHandler.scrollPos + ((!isUpSide) ? num : (0f - num));
		value = Mathf.Clamp(value, 0f, m_ScrollHandler.scrollPos_Max);
		if (GameGlobalUtil.IsAlmostSame(value, m_ScrollHandler.scrollPos))
		{
			return false;
		}
		m_ScrollHandler.ScrollToTargetPos(0f - value);
		return true;
	}

	private void ChangeOnCursorContent_byScrollPos()
	{
		if (m_OnCursorSlot == null)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_SlotContainerRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		float y = m_OnCursorSlot.rectTransform.offsetMax.y;
		float f2 = m_OnCursorSlot.rectTransform.offsetMax.y - m_OnCursorSlot.rectTransform.rect.height * m_OnCursorSlot.rectTransform.localScale.y;
		int num4 = Array.IndexOf(m_DataSlots, m_OnCursorSlot);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(f2) < num3)
		{
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_DataSlots[num5].rectTransform;
				f2 = rectTransform.offsetMax.y - rectTransform.rect.height * rectTransform.localScale.y;
				if (Mathf.CeilToInt(f2) >= num3)
				{
					SetOnCursorDataSlot(m_DataSlots[num5], isAdjustScrollPos: false);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
		else
		{
			if (Mathf.FloorToInt(y) <= num2)
			{
				return;
			}
			int num6 = m_DataSlots.Length;
			for (int i = num4 + 1; i < num6; i++)
			{
				rectTransform = m_DataSlots[i].rectTransform;
				y = rectTransform.offsetMax.y;
				if (Mathf.FloorToInt(y) <= num2)
				{
					SetOnCursorDataSlot(m_DataSlots[i], isAdjustScrollPos: false);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
	}

	private void AdjustScrollPos_byOnCursonContent()
	{
		if (!(m_OnCursorSlot == null))
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_SlotContainerRT.offsetMax.y;
			float num2 = num - height;
			float y = m_OnCursorSlot.rectTransform.offsetMax.y;
			float num3 = m_OnCursorSlot.rectTransform.offsetMax.y - m_OnCursorSlot.rectTransform.rect.height * m_OnCursorSlot.rectTransform.localScale.y;
			num3 -= m_BottomSpacing;
			if (num3 < num2)
			{
				float fTargetPos = num3 + height;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (y > num)
			{
				float fTargetPos2 = y;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public void OnClick_DataSlot(SaveDataSlot slot)
	{
		SaveDataSlot onCursorSlot = m_OnCursorSlot;
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		SetOnCursorDataSlot(slot);
		if (onCursorSlot != slot)
		{
			GameGlobalUtil.PlayUIAnimation_WithChidren(slot.m_SelSlotData.m_RootObject, "push");
		}
		ProcOnCursorDataSlot();
	}

	private void PlaySlotPressAni()
	{
		Animator componentInChildren = m_OnCursorSlot.m_SelSlotData.m_RootObject.GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			componentInChildren.SetTrigger("Pressed");
		}
	}

	private void ProcOnCursorDataSlot()
	{
		if (m_OnCursorSlot == null || base.isInputBlock)
		{
			return;
		}
		base.isInputBlock = true;
		SaveLoad instance = SaveLoad.GetInstance();
		SaveLoad.cSaveSlotInfo slotInfo = instance.GetSlotInfo((m_Type != Type.Save) ? m_OnCursorSlot.slotIndex : (m_OnCursorSlot.slotIndex + 1));
		if (m_Type == Type.Save)
		{
			if (slotInfo.m_isSave)
			{
				PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("SAVE_MENU_POPUP_OVERWRITE"), PopupResult_OverwriteSaveData);
			}
			else
			{
				SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveGameInfoColl, m_OnCursorSlot.slotIndex + 1, OnGameDataSaveDone);
			}
		}
		else if (!slotInfo.m_isSave)
		{
			base.isInputBlock = false;
		}
		else
		{
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("LOAD_MENU_POPUP_ACCEPT"), PoupResult_LoadGame);
		}
	}

	private void PopupResult_OverwriteSaveData(PopupDialoguePlus.Result result)
	{
		if (result != PopupDialoguePlus.Result.Yes)
		{
			base.isInputBlock = false;
		}
		else
		{
			SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveGameInfoColl, m_OnCursorSlot.slotIndex + 1, OnGameDataSaveDone);
		}
	}

	private void OnGameDataSaveDone(bool isExistErr)
	{
		MainLoadThing.instance.StartCoroutine(SetSlotInfo(m_OnCursorSlot));
		PopupDialoguePlus.ShowPopup_OK(GameGlobalUtil.GetXlsProgramText("SAVE_MENU_POPUP_COMPLETE"), OnClosed_PopupSaveDone);
	}

	private void OnClosed_PopupSaveDone(PopupDialoguePlus.Result result)
	{
		Resources.UnloadUnusedAssets();
		base.isInputBlock = false;
	}

	private void PoupResult_LoadGame(PopupDialoguePlus.Result result)
	{
		if (result != PopupDialoguePlus.Result.Yes)
		{
			base.isInputBlock = false;
			return;
		}
		m_LoadableSlotIdx = m_OnCursorSlot.slotIndex;
		Close();
	}

	public static void LoadGameData(int slotIdx)
	{
		EventEngine instance = EventEngine.GetInstance(isCreate: false);
		if (instance != null && instance.IsEventRunning())
		{
			instance.StopEvent();
		}
		GameSwitch instance2 = GameSwitch.GetInstance();
		instance2.FreeGoToMain();
		instance2.InitGameVal(ConstGameSwitch.eSTARTTYPE.CONTINUE, slotIdx, OnGameDataLoadDone);
	}

	private static void OnGameDataLoadDone(bool isExistErr)
	{
		EventEngine.m_strLoadedLevel = null;
		EventEngine.GetInstance().OnlyFreeResForGoToMain();
		SceneManager.LoadScene("Scene/GameMain");
	}

	public void Touch_DataSlot(SaveDataSlot slot)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_DataSlot(slot);
		}
	}
}
