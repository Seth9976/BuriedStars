using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMainMenu : CommonBGChildBase
{
	public enum Buttons
	{
		None = -1,
		Sound,
		Image,
		Keyword,
		Profile,
		Trophy,
		Exit
	}

	[Serializable]
	public class ButtonData
	{
		public GameObject m_RootObject;

		public Button m_ButtonComponent;

		public GameObject m_NonSelectRootObj;

		public Text m_NonSelectText;

		public Text m_NonSelectCompleteRate;

		public GameObject m_SelectRootObj;

		public Text m_SelectText;

		public Text m_SelectCompleteRate;

		public GameObject m_BannerRootObj;

		public Text m_BannerText;
	}

	public enum PrevMenuType
	{
		Unknown,
		MainMenu,
		SystemMenu
	}

	[Header("Title Members")]
	public Text m_TitleText1;

	public Text m_TitleText2;

	[Header("Comlete Rate Members")]
	public Text m_ComleteRateTitle;

	public Text m_ComleteRateValue;

	[Header("Buttons")]
	public ButtonData m_SoundButton = new ButtonData();

	public ButtonData m_ImageButton = new ButtonData();

	public ButtonData m_KeywordButton = new ButtonData();

	public ButtonData m_ProfileButton = new ButtonData();

	public ButtonData m_TrophyButton = new ButtonData();

	public ButtonData m_ExitButton = new ButtonData();

	private ButtonData[] m_Buttons;

	private Buttons m_curSelectedButton = Buttons.None;

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	public GameObject m_TouchBlockObject;

	private AudioManager m_AudioManager;

	private bool m_isInputByKeyOrPad;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInputBlock;

	private Animator m_CloseAnimator;

	private Buttons m_procedButton = Buttons.None;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_MoveCursor;

	private string m_buttonGuide_Submit;

	private string m_buttonGuide_Exit;

	private static PrevMenuType s_PrevMenuType;

	private int m_prevNewCount_Sound;

	private int m_prevNewCount_Image;

	private int m_prevNewCount_Keyword;

	private int m_prevNewCount_Profile;

	private int m_prevNewCount_Trophy;

	private bool m_isNowMainMenu = true;

	public static PrevMenuType prevMenuType
	{
		get
		{
			return s_PrevMenuType;
		}
		set
		{
			s_PrevMenuType = value;
		}
	}

	public bool isNowMainMenu
	{
		set
		{
			m_isNowMainMenu = value;
		}
	}

	private void Awake()
	{
		int num = Enum.GetValues(typeof(Buttons)).Length - 1;
		m_Buttons = new ButtonData[num];
		m_Buttons[0] = m_SoundButton;
		m_Buttons[1] = m_ImageButton;
		m_Buttons[2] = m_KeywordButton;
		m_Buttons[3] = m_ProfileButton;
		m_Buttons[4] = m_TrophyButton;
		m_Buttons[5] = m_ExitButton;
	}

	private void OnDestroy()
	{
		if (m_Buttons != null)
		{
			int num = m_Buttons.Length;
			for (int i = 0; i < num; i++)
			{
				m_Buttons[i] = null;
			}
			m_Buttons = null;
		}
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_CloseAnimator = null;
	}

	private void Update()
	{
		if (m_CloseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.cStrAnimStateDisappear) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetShow(isShow: false);
				}
				if (base.eventCloseComplete != null)
				{
					base.eventCloseComplete(this, m_procedButton);
				}
				base.gameObject.SetActive(value: false);
				GC.Collect();
			}
		}
		else
		{
			if (m_isInputBlock || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			if (m_curSelectedButton != Buttons.None && ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_Buttons[(int)m_curSelectedButton].m_ButtonComponent))
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_Submit, isActivate: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_OK");
				}
				m_isInputByKeyOrPad = true;
				return;
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_Exit, isActivate: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Menu_Cancel");
				}
				Close(Buttons.Exit);
				return;
			}
			float fAxisX = 0f;
			float fAxisY = 0f;
			if (!GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					num = -1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					num = 1;
					m_fScrollButtonPusingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						num = -1;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					num = 1;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				num2 = -1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				num2 = 1;
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					num2 = -1;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				num2 = 1;
			}
			if (num == 0 && num2 == 0)
			{
				return;
			}
			int num3 = 3;
			int num4 = 2;
			int curSelectedButton = (int)m_curSelectedButton;
			int num5 = curSelectedButton / num3;
			int num6 = curSelectedButton % num3;
			if (num != 0)
			{
				num6 += num;
				if (num6 < 0)
				{
					num6 = num3 - 1;
				}
				else if (num6 >= num3)
				{
					num6 = 0;
				}
			}
			if (num2 != 0)
			{
				num5 += num2;
				if (num5 < 0)
				{
					num5 = num4 - 1;
				}
				else if (num5 >= num4)
				{
					num5 = 0;
				}
			}
			int newSelectButton = num5 * num3 + num6;
			ChangeSelectionButton((Buttons)newSelectButton);
			m_ButtonGuide.SetContentActivate(m_buttonGuide_MoveCursor, isActivate: true);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
		}
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_fScrollButtonPusingTime += Time.deltaTime;
		if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
		{
			m_fScrollButtonPusingTime = 0f;
			return true;
		}
		return false;
	}

	public void Show(Buttons firstSelectButton, bool isBannerCountBackup = false)
	{
		GC.Collect();
		m_CloseAnimator = null;
		base.gameObject.SetActive(value: true);
		m_ComleteRateTitle.gameObject.SetActive(value: false);
		m_ComleteRateValue.gameObject.SetActive(value: false);
		SetButtonBannerValue(Buttons.Sound, 0);
		SetButtonBannerValue(Buttons.Image, 0);
		SetButtonBannerValue(Buttons.Keyword, 0);
		SetButtonBannerValue(Buttons.Profile, 0);
		SetButtonBannerValue(Buttons.Trophy, 0);
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		EnableTouchBlockObject(isEnable: false);
		InitTextMembers();
		InitButtonGuide();
		ButtonData[] buttons = m_Buttons;
		foreach (ButtonData buttonData in buttons)
		{
			if (buttonData != null)
			{
				ChangeButtonState(buttonData, isSelect: false);
			}
		}
		ChangeSelectionButton(firstSelectButton, isIgnoreSame: false);
		StartCoroutine(CalcCompleteRate(isBannerCountBackup));
	}

	public void Close(Buttons procedButton, bool isCheckNewCountChanged = true)
	{
		if (m_isNowMainMenu && procedButton == Buttons.Exit && isCheckNewCountChanged && (m_prevNewCount_Sound != GetButtonBannerValue(Buttons.Sound) || m_prevNewCount_Image != GetButtonBannerValue(Buttons.Image) || m_prevNewCount_Keyword != GetButtonBannerValue(Buttons.Keyword) || m_prevNewCount_Profile != GetButtonBannerValue(Buttons.Profile) || m_prevNewCount_Trophy != GetButtonBannerValue(Buttons.Trophy)))
		{
			EnableTouchBlockObject(isEnable: true);
			SaveLoad.GetInstance().SaveLoadWhat(SaveLoad.eSaveWhat.eSaveColl, 0, CompleteCollectionSave);
			return;
		}
		m_CloseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		m_procedButton = procedButton;
		if (procedButton == Buttons.Exit && base.eventNoticeExit != null)
		{
			base.eventNoticeExit(this, false);
		}
	}

	private void CompleteCollectionSave(bool isExistErr)
	{
		Close(Buttons.Exit, isCheckNewCountChanged: false);
	}

	public void SetButtonBannerValue(Buttons button, int newCount)
	{
		if (button >= Buttons.Sound && (int)button < m_Buttons.Length)
		{
			ButtonData buttonData = m_Buttons[(int)button];
			if (!(buttonData.m_BannerRootObj == null) && !(buttonData.m_BannerText == null))
			{
				buttonData.m_BannerRootObj.SetActive(newCount > 0);
				buttonData.m_BannerText.text = newCount.ToString();
			}
		}
	}

	public int GetButtonBannerValue(Buttons button)
	{
		if (button < Buttons.Sound || (int)button >= m_Buttons.Length)
		{
			return 0;
		}
		ButtonData buttonData = m_Buttons[(int)button];
		if (buttonData.m_BannerRootObj == null || buttonData.m_BannerText == null)
		{
			return 0;
		}
		int result = 0;
		int.TryParse(buttonData.m_BannerText.text, out result);
		return result;
	}

	private void SetButtonCompleteRate(Buttons button, int totalCount, int completeCount)
	{
		if (button < Buttons.Sound || (int)button >= m_Buttons.Length)
		{
			return;
		}
		int num = 0;
		Color color = GameGlobalUtil.HexToColor(482645);
		if (completeCount >= totalCount)
		{
			num = 100;
			color = GameGlobalUtil.HexToColor(8037204);
		}
		else if (completeCount > 0)
		{
			num = (int)((float)completeCount * 100f / (float)totalCount);
			if (num <= 0)
			{
				num = 1;
			}
		}
		string text = $"{num}%";
		ButtonData buttonData = m_Buttons[(int)button];
		if (buttonData.m_NonSelectCompleteRate != null)
		{
			buttonData.m_NonSelectCompleteRate.text = text;
			buttonData.m_NonSelectCompleteRate.color = color;
		}
		if (buttonData.m_SelectCompleteRate != null)
		{
			buttonData.m_SelectCompleteRate.text = text;
		}
	}

	public void InitTextMembers()
	{
		Text[] textComps = new Text[34]
		{
			m_TitleText1, m_TitleText2, m_ComleteRateTitle, m_ComleteRateValue, m_SoundButton.m_SelectText, m_SoundButton.m_NonSelectText, m_SoundButton.m_SelectCompleteRate, m_SoundButton.m_NonSelectCompleteRate, m_SoundButton.m_BannerText, m_ImageButton.m_SelectText,
			m_ImageButton.m_NonSelectText, m_ImageButton.m_SelectCompleteRate, m_ImageButton.m_NonSelectCompleteRate, m_ImageButton.m_BannerText, m_KeywordButton.m_SelectText, m_KeywordButton.m_NonSelectText, m_KeywordButton.m_SelectCompleteRate, m_KeywordButton.m_NonSelectCompleteRate, m_KeywordButton.m_BannerText, m_ProfileButton.m_SelectText,
			m_ProfileButton.m_NonSelectText, m_ProfileButton.m_SelectCompleteRate, m_ProfileButton.m_NonSelectCompleteRate, m_ProfileButton.m_BannerText, m_TrophyButton.m_SelectText, m_TrophyButton.m_NonSelectText, m_TrophyButton.m_SelectCompleteRate, m_TrophyButton.m_NonSelectCompleteRate, m_TrophyButton.m_BannerText, m_ExitButton.m_SelectText,
			m_ExitButton.m_NonSelectText, m_ExitButton.m_SelectCompleteRate, m_ExitButton.m_NonSelectCompleteRate, m_ExitButton.m_BannerText
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_SOUND");
		m_SoundButton.m_NonSelectText.text = xlsProgramText;
		m_SoundButton.m_SelectText.text = xlsProgramText;
		xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_IMAGE");
		m_ImageButton.m_NonSelectText.text = xlsProgramText;
		m_ImageButton.m_SelectText.text = xlsProgramText;
		xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_KEYWORD");
		m_KeywordButton.m_NonSelectText.text = xlsProgramText;
		m_KeywordButton.m_SelectText.text = xlsProgramText;
		xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_PROFILE");
		m_ProfileButton.m_NonSelectText.text = xlsProgramText;
		m_ProfileButton.m_SelectText.text = xlsProgramText;
		xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_TROPHY");
		m_TrophyButton.m_NonSelectText.text = xlsProgramText;
		m_TrophyButton.m_SelectText.text = xlsProgramText;
		xlsProgramText = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_EXIT");
		m_ExitButton.m_NonSelectText.text = xlsProgramText;
		m_ExitButton.m_SelectText.text = xlsProgramText;
		m_TitleText1.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_TITLE_1");
		m_TitleText2.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_TITLE_2");
		m_ComleteRateTitle.text = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_COMPLETE_RATE");
	}

	private void InitButtonGuide()
	{
		if (m_ButtonGuide == null)
		{
			m_ButtonGuide = GameGlobalUtil.GetCommonButtonGuide();
			if (m_ButtonGuide == null)
			{
				return;
			}
		}
		if (!m_isInitializedButtonGuidText)
		{
			m_buttonGuide_MoveCursor = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_BTNGUIDE_MOVE_CURSOR");
			m_buttonGuide_Submit = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_BTNGUIDE_SUBMIT");
			m_buttonGuide_Exit = GameGlobalUtil.GetXlsProgramText("COLLECTION_MENU_MAIN_BTNGUIDE_EXIT");
			m_isInitializedButtonGuidText = true;
		}
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_MoveCursor, PadInput.GameInput.LStickX, isIngoreAxis: true);
		m_ButtonGuide.AddContent(m_buttonGuide_Submit, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_Exit, PadInput.GameInput.CrossButton);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private void ChangeSelectionButton(Buttons newSelectButton, bool isIgnoreSame = true)
	{
		if (m_curSelectedButton != newSelectButton || !isIgnoreSame)
		{
			if (m_curSelectedButton != Buttons.None)
			{
				ChangeButtonState(m_curSelectedButton, isSelect: false);
			}
			if (newSelectButton != Buttons.None)
			{
				ChangeButtonState(newSelectButton, isSelect: true);
			}
			m_curSelectedButton = newSelectButton;
		}
	}

	private void ChangeButtonState(Buttons button, bool isSelect)
	{
		if (button != Buttons.None)
		{
			ChangeButtonState(m_Buttons[(int)button], isSelect);
		}
	}

	private void ChangeButtonState(ButtonData buttonData, bool isSelect)
	{
		buttonData.m_NonSelectRootObj.SetActive(!isSelect);
		buttonData.m_SelectRootObj.SetActive(isSelect);
	}

	private IEnumerator CalcCompleteRate(bool isBannerCountBackup)
	{
		int eachCount = 0;
		int eachCompleteCount = 0;
		int totalCount = 0;
		int toralCompleteCount = 0;
		int count = 0;
		int completeCount = 0;
		int newCountSound = 0;
		int newCountImage = 0;
		int newCountKeyword = 0;
		int newCountProfile = 0;
		int newCountTrophy = 0;
		sbyte switchState = 0;
		sbyte switchStateOn = 1;
		sbyte switchStateRead = 2;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		GetCollectionCompleteCount_Sound(out eachCount, out eachCompleteCount, out newCountSound);
		totalCount += eachCount;
		toralCompleteCount += eachCompleteCount;
		SetButtonCompleteRate(Buttons.Sound, eachCount, eachCompleteCount);
		yield return null;
		GetCollectionCompleteCount_Image(out eachCount, out eachCompleteCount, out newCountImage);
		totalCount += eachCount;
		toralCompleteCount += eachCompleteCount;
		SetButtonCompleteRate(Buttons.Image, eachCount, eachCompleteCount);
		yield return null;
		GetCollectionCompleteCount_Keyword(out eachCount, out eachCompleteCount, out newCountKeyword);
		totalCount += eachCount;
		toralCompleteCount += eachCompleteCount;
		SetButtonCompleteRate(Buttons.Keyword, eachCount, eachCompleteCount);
		yield return null;
		GetCollectionCompleteCount_Profile(out eachCount, out eachCompleteCount, out newCountProfile);
		totalCount += eachCount;
		toralCompleteCount += eachCompleteCount;
		SetButtonCompleteRate(Buttons.Profile, eachCount, eachCompleteCount);
		yield return null;
		GetCollectionCompleteCount_Trophy(out eachCount, out eachCompleteCount, out newCountTrophy);
		totalCount += eachCount;
		toralCompleteCount += eachCompleteCount;
		SetButtonCompleteRate(Buttons.Trophy, eachCount, eachCompleteCount);
		float completeRate = 0f;
		if (toralCompleteCount >= totalCount)
		{
			completeRate = 100f;
		}
		else if (toralCompleteCount > 0)
		{
			completeRate = (float)toralCompleteCount / (float)totalCount;
			completeRate = Mathf.Clamp01(completeRate);
			completeRate *= 100f;
			if (completeRate < 0.01f)
			{
				completeRate = 0.01f;
			}
		}
		m_ComleteRateTitle.gameObject.SetActive(value: true);
		m_ComleteRateValue.text = $"{completeRate:#0.00}%";
		m_ComleteRateValue.gameObject.SetActive(value: true);
		SetButtonBannerValue(Buttons.Sound, newCountSound);
		SetButtonBannerValue(Buttons.Image, newCountImage);
		SetButtonBannerValue(Buttons.Keyword, newCountKeyword);
		SetButtonBannerValue(Buttons.Profile, newCountProfile);
		SetButtonBannerValue(Buttons.Trophy, newCountTrophy);
		if (isBannerCountBackup)
		{
			m_prevNewCount_Sound = newCountSound;
			m_prevNewCount_Image = newCountImage;
			m_prevNewCount_Keyword = newCountKeyword;
			m_prevNewCount_Profile = newCountProfile;
			m_prevNewCount_Trophy = newCountTrophy;
		}
	}

	public static void GetCollectionCompleteCount_Sound(out int totalCount, out int completeCount, out int newCount)
	{
		totalCount = 0;
		completeCount = 0;
		newCount = 0;
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		GameSwitch instance = GameSwitch.GetInstance();
		List<Xls.CollSounds> datas = Xls.CollSounds.datas;
		int count = datas.Count;
		Xls.CollSounds collSounds = null;
		int num = 5;
		for (int i = 0; i < count; i++)
		{
			collSounds = datas[i];
			if (collSounds.m_iCategory != num)
			{
				b = instance.GetCollSound(collSounds.m_iIdx);
				newCount += ((b == b2) ? 1 : 0);
				completeCount += ((b == b2 || b == b3) ? 1 : 0);
				totalCount++;
			}
		}
	}

	public static void GetCollectionCompleteCount_Image(out int totalCount, out int completeCount, out int newCount)
	{
		totalCount = 0;
		completeCount = 0;
		newCount = 0;
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		GameSwitch instance = GameSwitch.GetInstance();
		List<Xls.CollImages> datas = Xls.CollImages.datas;
		int count = datas.Count;
		completeCount = 0;
		Xls.CollImages collImages = null;
		for (int i = 0; i < count; i++)
		{
			collImages = datas[i];
			b = instance.GetCollImage(collImages.m_iIdx);
			newCount += ((b == b2) ? 1 : 0);
			completeCount += ((b == b2 || b == b3) ? 1 : 0);
		}
		totalCount = count;
	}

	public static void GetCollectionCompleteCount_Keyword(out int totalCount, out int completeCount, out int newCount)
	{
		totalCount = 0;
		completeCount = 0;
		newCount = 0;
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		GameSwitch instance = GameSwitch.GetInstance();
		List<Xls.CollKeyword> datas = Xls.CollKeyword.datas;
		int count = datas.Count;
		completeCount = 0;
		Xls.CollKeyword collKeyword = null;
		for (int i = 0; i < count; i++)
		{
			collKeyword = datas[i];
			b = instance.GetCollKeyword(collKeyword.m_strKey);
			newCount += ((b == b2) ? 1 : 0);
			completeCount += ((b == b2 || b == b3) ? 1 : 0);
		}
		totalCount = count;
	}

	public static void GetCollectionCompleteCount_Profile(out int totalCount, out int completeCount, out int newCount)
	{
		totalCount = 0;
		completeCount = 0;
		newCount = 0;
		sbyte b = 0;
		sbyte b2 = 1;
		sbyte b3 = 2;
		GameSwitch instance = GameSwitch.GetInstance();
		List<Xls.Profiles> datas = Xls.Profiles.datas;
		int count = datas.Count;
		completeCount = 0;
		Xls.Profiles profiles = null;
		for (int i = 0; i < count; i++)
		{
			profiles = datas[i];
			b = instance.GetCollProfile(profiles.m_strKey);
			newCount += ((b == b2) ? 1 : 0);
			completeCount += ((b == b2 || b == b3) ? 1 : 0);
		}
		totalCount = count;
	}

	public static void GetCollectionCompleteCount_Trophy(out int totalCount, out int completeCount, out int newCount)
	{
		totalCount = 0;
		completeCount = 0;
		newCount = 0;
		sbyte b = 1;
		GameSwitch instance = GameSwitch.GetInstance();
		List<Xls.Trophys> datas = Xls.Trophys.datas;
		int count = datas.Count;
		completeCount = 0;
		int num = 0;
		Xls.Trophys trophys = null;
		for (int i = 0; i < count; i++)
		{
			trophys = datas[i];
			num = instance.GetTrophyCnt(trophys.m_iIndex);
			completeCount += ((num >= trophys.m_iMax) ? 1 : 0);
			newCount += ((instance.GetTrophyNew(trophys.m_iIndex) == b) ? 1 : 0);
		}
		totalCount = count;
	}

	public void OnClickCloseButton()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		Close(Buttons.Exit);
	}

	public void OnClickButton(int buttonID)
	{
		if (!m_isInputByKeyOrPad && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		m_isInputByKeyOrPad = false;
		Close((Buttons)buttonID);
	}

	public void TouchCloseButton()
	{
		OnClickCloseButton();
	}

	public void TouchButton(int buttonID)
	{
		ChangeSelectionButton((Buttons)buttonID);
		OnClickButton(buttonID);
	}

	private void EnableTouchBlockObject(bool isEnable)
	{
		m_isInputBlock = isEnable;
		if (m_TouchBlockObject != null)
		{
			m_TouchBlockObject.SetActive(isEnable);
		}
	}
}
