using System;
using System.Collections;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class SystemMenuPlus : CommonBGChildBase
{
	public enum Mode
	{
		None = -1,
		Save,
		Load,
		Collection,
		Sound,
		Config,
		QuitGame,
		Count
	}

	[Serializable]
	public class ButtonData
	{
		public Toggle m_ButtonObj;

		public GameObject m_NotSelectedObj;

		public GameObject m_SelectedObj;

		public Text m_NotSelectText;

		public Text m_SelectText;

		public Image m_SelectImage;

		public Button m_SelectButtonIcon;

		[NonSerialized]
		public string m_prefabBundleName;

		[NonSerialized]
		public AssetBundleObjectHandler m_assetBundleHdr;

		[NonSerialized]
		public SystemMenuBase m_LinkedMenu;

		[NonSerialized]
		public int m_iType;
	}

	public enum ExitType
	{
		InGame,
		LoadGame,
		Collection,
		MainMenu
	}

	[Header("Mode Datas")]
	public bool m_CursorLoopEnable = true;

	public GameObject m_ButtonsRoot;

	public ButtonData m_SaveButton = new ButtonData();

	public ButtonData m_LoadButton = new ButtonData();

	public ButtonData m_CollectionButton = new ButtonData();

	public ButtonData m_SoundButton = new ButtonData();

	public ButtonData m_ConfigButton = new ButtonData();

	public ButtonData m_QuitGameButton = new ButtonData();

	private ButtonData[] m_ButtonDatas;

	[Header("Button Sprites")]
	public Sprite m_OnCursorSprite;

	public Sprite m_SelectedSprite;

	[Header("QuickSave Title")]
	public GameObject m_QuickSaveTitleRoot;

	public Text m_QuickSaveBigText;

	public Text m_QuickSaveSmallText;

	[Header("Exit")]
	public Button m_ExitButtonIcon;

	public Text m_ExitGuideText;

	private ButtonData m_OnCurButton;

	private ExitType m_ExitType;

	private int m_loadableSlotIdx = -1;

	private Animator m_DisappearCheckAnimator;

	private AudioManager m_AudioManager;

	private bool m_isInputBlock;

	private bool m_isQuickSaveMode;

	private static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();

	private bool m_isSubMenuChanging;

	private GameObject m_InputBlockPanel;

	public int loadableSlotIdx => m_loadableSlotIdx;

	private void Awake()
	{
		m_SaveButton.m_prefabBundleName = "prefabs/ingame/menu/ui_savemenu";
		m_LoadButton.m_prefabBundleName = "prefabs/ingame/menu/ui_loadmenu";
		m_SoundButton.m_prefabBundleName = "prefabs/ingame/menu/ui_soundmenu";
		m_ConfigButton.m_prefabBundleName = "prefabs/ingame/menu/ui_configmenu";
		m_SaveButton.m_iType = 0;
		m_LoadButton.m_iType = 1;
		m_SoundButton.m_iType = 3;
		m_ConfigButton.m_iType = 4;
		m_ButtonDatas = new ButtonData[6];
		m_ButtonDatas[0] = m_SaveButton;
		m_ButtonDatas[1] = m_LoadButton;
		m_ButtonDatas[2] = m_CollectionButton;
		m_ButtonDatas[3] = m_SoundButton;
		m_ButtonDatas[4] = m_ConfigButton;
		m_ButtonDatas[5] = m_QuitGameButton;
	}

	private void Start()
	{
		CreateInputBlockPanel();
	}

	private void OnDestroy()
	{
		m_OnCurButton = null;
		m_DisappearCheckAnimator = null;
		m_AudioManager = null;
	}

	private void Update()
	{
		if (m_DisappearCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_DisappearCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Closed();
			}
		}
		else
		{
			if (m_DisappearCheckAnimator != null || m_isInputBlock || PopupDialoguePlus.IsAnyPopupActivated() || (m_isQuickSaveMode && m_SaveButton.m_LinkedMenu != null && m_SaveButton.m_LinkedMenu.isInputBlock))
			{
				return;
			}
			if (!m_isQuickSaveMode)
			{
				if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
				{
					ChangeOnCursorButton(isUpSide: true);
					return;
				}
				if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
				{
					ChangeOnCursorButton(isUpSide: false);
					return;
				}
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
				{
					if (m_OnCurButton != null)
					{
						ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCurButton.m_SelectButtonIcon);
					}
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_OK");
					}
					StartCoroutine(ChangeMode_ByOnCursorButton());
					return;
				}
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_ExitButtonIcon, null, null, null, null, isShowAnim: true, isExcuteEvent: false);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Cancel");
				}
				Close();
			}
		}
	}

	private void fpCBAfterSaveClosePopup(object sender, object arg)
	{
		Close();
	}

	public void Show(Mode firstSelMode = Mode.Save)
	{
		base.gameObject.SetActive(value: true);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		m_DisappearCheckAnimator = null;
		m_isInputBlock = false;
		m_ExitType = ExitType.InGame;
		m_loadableSlotIdx = -1;
		m_isQuickSaveMode = false;
		if (m_ButtonsRoot != null)
		{
			m_ButtonsRoot.SetActive(value: true);
		}
		if (m_QuickSaveTitleRoot != null)
		{
			m_QuickSaveTitleRoot.SetActive(value: false);
		}
		ResetXlsTexts();
		int num = m_ButtonDatas.Length;
		for (int i = 0; i < num; i++)
		{
			ButtonData buttonData = m_ButtonDatas[i];
			buttonData.m_SelectImage.sprite = m_OnCursorSprite;
			buttonData.m_NotSelectedObj.SetActive(value: true);
			buttonData.m_SelectedObj.SetActive(value: false);
		}
		SetOnCursorButton(null);
		int num2 = Mathf.Clamp((int)firstSelMode, 0, m_ButtonDatas.Length - 1);
		SetOnCursorButton(m_ButtonDatas[num2]);
	}

	public IEnumerator ShowQuickSaveMode()
	{
		base.gameObject.SetActive(value: true);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		FontManager.ResetTextFontByCurrentLanguage(m_QuickSaveBigText);
		FontManager.ResetTextFontByCurrentLanguage(m_QuickSaveSmallText);
		FontManager.ResetTextFontByCurrentLanguage(m_ExitGuideText);
		m_DisappearCheckAnimator = null;
		m_isInputBlock = false;
		m_ExitType = ExitType.InGame;
		m_loadableSlotIdx = -1;
		m_isQuickSaveMode = true;
		if (m_ButtonsRoot != null)
		{
			m_ButtonsRoot.SetActive(value: false);
		}
		if (m_QuickSaveTitleRoot != null)
		{
			m_QuickSaveTitleRoot.SetActive(value: true);
		}
		if (m_QuickSaveBigText != null)
		{
			m_QuickSaveBigText.text = GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_TEXT_QUICKSAVE_BIG");
		}
		if (m_QuickSaveSmallText != null)
		{
			m_QuickSaveSmallText.text = GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_TEXT_QUICKSAVE_SMALL");
		}
		if (m_ExitGuideText != null)
		{
			m_ExitGuideText.text = GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_EXIT");
		}
		if (m_SaveButton.m_LinkedMenu == null)
		{
			m_isInputBlock = true;
			GameObject gameObject = UnityEngine.Object.Instantiate(MainLoadThing.instance.m_prefabSystemSaveMenu) as GameObject;
			m_SaveButton.m_LinkedMenu = gameObject.GetComponent<SystemMenuBase>();
			Canvas componentInChildren = base.gameObject.GetComponentInChildren<Canvas>();
			Canvas componentInChildren2 = gameObject.GetComponentInChildren<Canvas>();
			componentInChildren2.sortingOrder = componentInChildren.sortingOrder + 1;
			m_isInputBlock = false;
		}
		m_SaveButton.m_LinkedMenu.ShowTitle(isShow: true);
		m_SaveButton.m_LinkedMenu.Show();
		yield break;
	}

	public void Close(bool isEnableAnimation = true)
	{
		if (base.eventNoticeExit != null)
		{
			base.eventNoticeExit(this, m_ExitType);
		}
		if (m_OnCurButton != null && m_OnCurButton.m_LinkedMenu != null && m_OnCurButton.m_LinkedMenu.gameObject.activeInHierarchy)
		{
			if (isEnableAnimation)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(m_OnCurButton.m_LinkedMenu.gameObject, GameDefine.UIAnimationState.disappear.ToString());
			}
			else
			{
				m_OnCurButton.m_LinkedMenu.gameObject.SetActive(value: false);
			}
		}
		if (m_isQuickSaveMode && m_SaveButton.m_LinkedMenu != null)
		{
			m_SaveButton.m_LinkedMenu.isInputBlock = true;
		}
		if (isEnableAnimation)
		{
			m_DisappearCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
			if (m_DisappearCheckAnimator == null)
			{
				Closed();
			}
		}
		else
		{
			Closed();
		}
	}

	private void Closed()
	{
		ClosedComplete();
		SetOnCursorButton(null);
		base.gameObject.SetActive(value: false);
		if (base.eventCloseComplete != null)
		{
			base.eventCloseComplete(this, m_ExitType);
		}
	}

	public void ClosedComplete()
	{
		int num = m_ButtonDatas.Length;
		ButtonData buttonData = null;
		for (int i = 0; i < num; i++)
		{
			buttonData = m_ButtonDatas[i];
			if (!(buttonData.m_LinkedMenu == null))
			{
				UnityEngine.Object.Destroy(buttonData.m_LinkedMenu.gameObject);
				buttonData.m_LinkedMenu = null;
			}
		}
	}

	public void ResetXlsTexts()
	{
		string[] array = new string[6] { "SYSTEM_MENU_TEXT_SAVE", "SYSTEM_MENU_TEXT_LOAD", "SYSTEM_MENU_TEXT_COLLECTION", "SYSTEM_MENU_TEXT_SOUND", "SYSTEM_MENU_TEXT_CONFIG", "SYSTEM_MENU_TEXT_QUITGAME" };
		ButtonData buttonData = null;
		string empty = string.Empty;
		int num = Mathf.Min(array.Length, m_ButtonDatas.Length);
		for (int i = 0; i < num; i++)
		{
			buttonData = m_ButtonDatas[i];
			empty = GameGlobalUtil.GetXlsProgramText(array[i]);
			if (buttonData.m_NotSelectText != null)
			{
				FontManager.ResetTextFontByCurrentLanguage(buttonData.m_NotSelectText);
				buttonData.m_NotSelectText.text = empty;
			}
			if (buttonData.m_SelectText != null)
			{
				FontManager.ResetTextFontByCurrentLanguage(buttonData.m_SelectText);
				buttonData.m_SelectText.text = empty;
			}
		}
		if (m_ExitGuideText != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_ExitGuideText);
			m_ExitGuideText.text = GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_EXIT");
		}
	}

	private void SetOnCursorButton(ButtonData btnData)
	{
		if (m_OnCurButton == btnData)
		{
			return;
		}
		if (m_OnCurButton != null)
		{
			SetToggleValue(m_OnCurButton.m_ButtonObj, isValue: false);
			if (m_OnCurButton.m_SelectButtonIcon != null)
			{
				m_OnCurButton.m_SelectButtonIcon.gameObject.SetActive(value: false);
			}
			m_OnCurButton.m_NotSelectedObj.SetActive(value: true);
			m_OnCurButton.m_SelectedObj.SetActive(value: false);
		}
		if (btnData != null)
		{
			SetToggleValue(btnData.m_ButtonObj, isValue: true);
			if (btnData.m_SelectButtonIcon != null)
			{
				btnData.m_SelectButtonIcon.gameObject.SetActive(value: true);
			}
			btnData.m_NotSelectedObj.SetActive(value: false);
			btnData.m_SelectedObj.SetActive(value: true);
			btnData.m_SelectImage.sprite = m_OnCursorSprite;
		}
		m_OnCurButton = btnData;
	}

	public static void SetToggleValue(Toggle toggle, bool isValue)
	{
		if (!(toggle == null))
		{
			Toggle.ToggleEvent onValueChanged = toggle.onValueChanged;
			toggle.onValueChanged = emptyToggleEvent;
			toggle.isOn = isValue;
			toggle.onValueChanged = onValueChanged;
		}
	}

	private bool ChangeOnCursorButton(bool isUpSide)
	{
		int num = m_ButtonDatas.Length;
		if (m_OnCurButton == null)
		{
			if (num > 0)
			{
				SetOnCursorButton(m_ButtonDatas[0]);
			}
			return false;
		}
		int num2 = Array.IndexOf(m_ButtonDatas, m_OnCurButton);
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
		SetOnCursorButton(m_ButtonDatas[num2]);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		return true;
	}

	private void CreateInputBlockPanel()
	{
		if (m_InputBlockPanel == null)
		{
			m_InputBlockPanel = new GameObject("InputBlockPanel");
			RawImage rawImage = m_InputBlockPanel.AddComponent<RawImage>();
			rawImage.color = new Color(0f, 0f, 0f, 0f);
			rawImage.raycastTarget = true;
		}
		Canvas componentInChildren = base.gameObject.GetComponentInChildren<Canvas>();
		if (componentInChildren != null)
		{
			RectTransform component = componentInChildren.gameObject.GetComponent<RectTransform>();
			RectTransform component2 = m_InputBlockPanel.GetComponent<RectTransform>();
			component2.SetParent(component, worldPositionStays: false);
			component2.SetAsLastSibling();
			component2.anchorMin = Vector2.zero;
			component2.anchorMax = Vector2.zero;
			component2.anchoredPosition = new Vector2(component.rect.width * 0.5f, component.rect.height * 0.5f);
			component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rect.width);
			component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component.rect.height);
		}
		m_InputBlockPanel.SetActive(value: false);
	}

	public void OnClickBackButton()
	{
		if (!m_isSubMenuChanging && MainLoadThing.instance.IsTouchableState())
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Cancel");
			}
			if ((m_OnCurButton != m_SoundButton && m_OnCurButton != m_ConfigButton) || !(m_OnCurButton.m_LinkedMenu != null) || !m_OnCurButton.m_LinkedMenu.CheckExistChangeValue(fpCBAfterSaveClosePopup))
			{
				Close();
			}
		}
	}

	public void TouchOnCursorButton(int iIdx)
	{
		if (iIdx < 0 || iIdx >= m_ButtonDatas.Length || m_isSubMenuChanging || !MainLoadThing.instance.IsTouchableState())
		{
			return;
		}
		int iSize = m_ButtonDatas.Length;
		if (BitCalc.CheckArrayIdx(iIdx, iSize) && m_ButtonDatas[iIdx].m_ButtonObj.isOn && (m_ButtonDatas[iIdx] != m_OnCurButton || !(m_OnCurButton.m_LinkedMenu != null) || !m_OnCurButton.m_LinkedMenu.gameObject.activeSelf))
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_OK");
			}
			if (m_OnCurButton.m_LinkedMenu != null && m_OnCurButton.m_LinkedMenu.gameObject.activeSelf)
			{
				StartCoroutine(ChangeMode_ClosePrevMode(iIdx));
				return;
			}
			SetOnCursorButton(m_ButtonDatas[iIdx]);
			StartCoroutine(ChangeMode_ByOnCursorButton());
		}
	}

	private void fpCBAfterSaveChangePopup(object sender, object arg)
	{
		StartCoroutine(ChangeMode_ByOnCursorButton(isCheckChangeValue: false));
	}

	private IEnumerator ChangeMode_ClosePrevMode(int newModeIdx)
	{
		m_isSubMenuChanging = true;
		if (m_InputBlockPanel != null)
		{
			m_InputBlockPanel.SetActive(value: true);
		}
		ButtonData m_prevButtonData = m_OnCurButton;
		ButtonData newButtonData = m_ButtonDatas[newModeIdx];
		SetOnCursorButton(m_ButtonDatas[newModeIdx]);
		if (m_prevButtonData != null && m_prevButtonData != newButtonData && !(m_prevButtonData.m_LinkedMenu == null) && m_prevButtonData.m_LinkedMenu.gameObject.activeSelf)
		{
			m_prevButtonData.m_LinkedMenu.Close(isEnableAnimation: false);
			while ((bool)m_prevButtonData.m_LinkedMenu && m_prevButtonData.m_LinkedMenu.gameObject.activeSelf)
			{
				yield return null;
			}
			yield return null;
		}
		yield return StartCoroutine(ChangeMode_ByOnCursorButton());
		m_isSubMenuChanging = false;
		if (m_InputBlockPanel != null)
		{
			m_InputBlockPanel.SetActive(value: false);
		}
	}

	private UnityEngine.Object GetPrefabSystemMenu(int iType)
	{
		UnityEngine.Object result = null;
		switch (iType)
		{
		case 0:
			result = MainLoadThing.instance.m_prefabSystemSaveMenu;
			break;
		case 1:
			result = MainLoadThing.instance.m_prefabSystemLoadMenu;
			break;
		case 3:
			result = MainLoadThing.instance.m_prefabSystemSoundMenu;
			break;
		case 4:
			result = MainLoadThing.instance.m_prefabSystemConfigMenu;
			break;
		}
		return result;
	}

	private IEnumerator ChangeMode_ByOnCursorButton(bool isCheckChangeValue = true)
	{
		if (m_OnCurButton == null)
		{
			yield break;
		}
		m_isInputBlock = true;
		if (m_OnCurButton.m_SelectImage != null)
		{
			m_OnCurButton.m_SelectImage.sprite = m_SelectedSprite;
		}
		if (m_OnCurButton.m_SelectButtonIcon != null)
		{
			m_OnCurButton.m_SelectButtonIcon.gameObject.SetActive(value: false);
		}
		ButtonData[] buttonDatas = m_ButtonDatas;
		foreach (ButtonData buttonData in buttonDatas)
		{
			if (buttonData != null && !(buttonData.m_LinkedMenu == null))
			{
				buttonData.m_LinkedMenu.ShowTitle(isShow: false);
				if (buttonData.m_LinkedMenu.gameObject.activeInHierarchy && (buttonData == m_SoundButton || buttonData == m_ConfigButton) && isCheckChangeValue && buttonData.m_LinkedMenu.CheckExistChangeValue(fpCBAfterSaveChangePopup))
				{
					yield break;
				}
				buttonData.m_LinkedMenu.gameObject.SetActive(value: false);
			}
		}
		if (m_OnCurButton == m_QuitGameButton)
		{
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("SYSTEM_MENU_POPUP_QUITGAME"), OnPopupResult_QuitGame);
		}
		else if (m_OnCurButton == m_CollectionButton)
		{
			m_ExitType = ExitType.Collection;
			Close();
		}
		else
		{
			if (m_OnCurButton.m_prefabBundleName == null)
			{
				yield break;
			}
			if (m_OnCurButton.m_LinkedMenu == null)
			{
				m_isSubMenuChanging = true;
				if (m_InputBlockPanel != null)
				{
					m_InputBlockPanel.SetActive(value: true);
				}
				UnityEngine.Object prefabSystemMenu = GetPrefabSystemMenu(m_OnCurButton.m_iType);
				GameObject gameObject = null;
				if (prefabSystemMenu != null)
				{
					gameObject = UnityEngine.Object.Instantiate(prefabSystemMenu) as GameObject;
				}
				m_OnCurButton.m_LinkedMenu = gameObject.GetComponent<SystemMenuBase>();
				Canvas componentInChildren = base.gameObject.GetComponentInChildren<Canvas>();
				Canvas componentInChildren2 = gameObject.GetComponentInChildren<Canvas>();
				componentInChildren2.sortingOrder = componentInChildren.sortingOrder + 1;
			}
			m_OnCurButton.m_LinkedMenu.ShowTitle(isShow: true);
			m_OnCurButton.m_LinkedMenu.isInFormMainMenu = false;
			m_OnCurButton.m_LinkedMenu.Show(isEnableAnimation: true, OnProc_ClosedSubMenu, isNeedSetCloseCB: true, m_ExitButtonIcon);
			while (!m_OnCurButton.m_LinkedMenu.gameObject.activeSelf)
			{
				yield return null;
			}
			m_isSubMenuChanging = false;
			if (m_InputBlockPanel != null)
			{
				yield return new WaitForSeconds(0.25f);
				m_InputBlockPanel.SetActive(value: false);
			}
		}
	}

	public void OnPopupResult_QuitGame(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			m_ExitType = ExitType.MainMenu;
			Close();
		}
		else
		{
			OnProc_ClosedSubMenu(null, null);
		}
	}

	private void OnProc_ClosedSubMenu(object sender, object arg)
	{
		if (m_OnCurButton == null)
		{
			return;
		}
		if (sender is SaveDataMenu && arg is int num)
		{
			if (num >= 0)
			{
				m_loadableSlotIdx = num;
				m_ExitType = ExitType.LoadGame;
				Close();
				return;
			}
		}
		else if (sender is ConfigMenuPlus && (sender as ConfigMenuPlus).IsChangedLanguage)
		{
			ResetXlsTexts();
		}
		if ((bool)m_OnCurButton.m_SelectImage)
		{
			m_OnCurButton.m_SelectImage.sprite = m_OnCursorSprite;
		}
		if ((bool)m_OnCurButton.m_SelectButtonIcon)
		{
			m_OnCurButton.m_SelectButtonIcon.gameObject.SetActive(value: true);
		}
		m_isInputBlock = false;
	}
}
