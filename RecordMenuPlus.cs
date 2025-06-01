using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class RecordMenuPlus : MonoBehaviour
{
	public GameObject m_RootObject;

	public Text m_TitleText;

	public Text m_PlayingText;

	[Header("Content Container")]
	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public GameObject m_ContentSrcObject;

	public float m_ContentInterval;

	public float m_BottomSpacing = 80f;

	[Header("Scroll Members")]
	public GameObject m_ScrollbarRoot;

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	private const int c_AudioCannel = 5;

	private AudioManager m_AudioManager;

	private CommonButtonGuide m_ButtonGuide;

	private bool m_isInputBlock;

	private bool m_isPageScrolling;

	private bool m_isTutorialActivated;

	private Animator m_CloseCheckAnimator;

	private GameDefine.EventProc m_fpClosedFP;

	private GameDefine.EventProc m_fpChangedSelectContent;

	private List<RecordContentPlus> m_Contents = new List<RecordContentPlus>();

	private RecordContentPlus m_OnCursorContent;

	private RecordContentPlus m_CurPlayingContent;

	private bool m_isInitializedButtonGuidText;

	private string m_buttonGuide_SelContent;

	private string m_buttonGuide_ExitMenu;

	private string m_buttonGuide_PlayStop;

	private string m_buttonGuide_ScrollToTop;

	private bool m_isRunEvent;

	public Animator m_aniDialogBlackBG;

	private GameDefine.eAnimChangeState m_eDialogAniState;

	private bool m_isDialogBlackDisappear;

	private static RecordMenuPlus s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_recordmenu";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	public static RecordMenuPlus instance => s_activedInstance;

	public static bool IsActivated => s_activedInstance != null && s_activedInstance.gameObject.activeSelf;

	private void Awake()
	{
		if (m_ContentSrcObject != null)
		{
			m_ContentSrcObject.SetActive(value: false);
		}
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContanierRT, null, null, m_ScrollButtonToFirst);
		m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		m_isRunEvent = false;
		m_isDialogBlackDisappear = false;
	}

	private void OnDestroy()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_AudioManager = null;
		m_ButtonGuide = null;
		m_CloseCheckAnimator = null;
		m_fpClosedFP = null;
		m_fpChangedSelectContent = null;
		m_Contents = null;
		m_OnCursorContent = null;
		m_CurPlayingContent = null;
		s_activedInstance = null;
	}

	private void Update()
	{
		if (m_isDialogBlackDisappear && m_eDialogAniState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState))
		{
			m_aniDialogBlackBG.gameObject.SetActive(value: false);
			m_isDialogBlackDisappear = false;
			m_eDialogAniState = GameDefine.eAnimChangeState.none;
		}
		if (m_CloseCheckAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_CloseCheckAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CallClosedCallback();
			}
			return;
		}
		if (!m_isRunEvent && m_CurPlayingContent != null && m_AudioManager != null && !m_AudioManager.IsPlayingChannel(5))
		{
			m_CurPlayingContent.isPlaying = false;
			m_CurPlayingContent = null;
			m_PlayingText.gameObject.SetActive(value: false);
		}
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			bool flag = true;
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorContent != null)
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
		if (m_isTutorialActivated || m_isInputBlock || ButtonPadInput.IsPlayingButPressAnim() || PopupDialoguePlus.IsAnyPopupActivated())
		{
			return;
		}
		float num = 0f;
		if (m_ScrollHandler.isScrollable)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_ScrollButtonToFirst);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_ScrollToTop, isActivate: true);
				}
				OnClick_ContentScrollToTop();
			}
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
		if (m_ScrollHandler.IsScrolling)
		{
			return;
		}
		num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickY);
		if (Mathf.Abs(num) > 0.5f)
		{
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: true);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeOnCursorContent(isUpSide: false);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContent(isUpSide: true);
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeOnCursorContent(isUpSide: false);
				}
			}
		}
		if (m_OnCursorContent != null)
		{
			ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCursorContent.m_PadIconButton, m_OnCursorContent.m_PlayButton, m_OnCursorContent.m_StopButton);
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetContentActivate(m_buttonGuide_PlayStop, isActivate: true);
				}
				OnProc_PlayStopRecord(m_OnCursorContent, !m_OnCursorContent.isPlaying);
			}
		}
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			if (m_ButtonGuide != null)
			{
				m_ButtonGuide.SetContentActivate(m_buttonGuide_ExitMenu, isActivate: true);
			}
			CloseRecordMenu();
		}
	}

	private void FinishConti(object sender, object arg)
	{
		m_CurPlayingContent.isPlaying = false;
		m_CurPlayingContent = null;
		m_PlayingText.gameObject.SetActive(value: false);
		m_isRunEvent = false;
		m_isInputBlock = false;
		GameSwitch.GetInstance().SetRecordFaterConti(null);
		InitButtonGuide();
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		}
		GameGlobalUtil.PlayUIAnimation(m_aniDialogBlackBG, GameDefine.UIAnimationState.disappear, ref m_eDialogAniState);
		m_isDialogBlackDisappear = true;
	}

	public static IEnumerator ShowRecordMenu_FormAssetBundle(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (Object.Instantiate(MainLoadThing.instance.m_prefabRecordMenu) as GameObject).GetComponent<RecordMenuPlus>();
			yield return null;
		}
		s_activedInstance.ShowRecordMenu(fpClosed, fpChangedSelContent);
	}

	public void ShowRecordMenu(GameDefine.EventProc fpClosed = null, GameDefine.EventProc fpChangedSelContent = null)
	{
		m_fpClosedFP = ((fpClosed == null) ? null : new GameDefine.EventProc(fpClosed.Invoke));
		m_fpChangedSelectContent = ((fpChangedSelContent == null) ? null : new GameDefine.EventProc(fpChangedSelContent.Invoke));
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		m_isRunEvent = false;
		m_isDialogBlackDisappear = false;
		FontManager.ResetTextFontByCurrentLanguage(m_TitleText);
		FontManager.ResetTextFontByCurrentLanguage(m_PlayingText);
		RecordContentPlus.InitStaticTextMembers();
		m_TitleText.text = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_TITLE");
		m_PlayingText.text = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_PLAYING");
		m_PlayingText.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		m_ScrollHandler.ResetScrollRange();
		InitButtonGuide();
		StartCoroutine(InitContents());
	}

	public void CloseRecordMenu(bool isEnableAnimation = true)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		m_isInputBlock = true;
		if (m_CurPlayingContent != null)
		{
			OnProc_PlayStopRecord(m_CurPlayingContent, isPlay: false);
		}
		if (!isEnableAnimation)
		{
			CallClosedCallback();
			return;
		}
		m_CloseCheckAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_CloseCheckAnimator == null)
		{
			CallClosedCallback();
		}
	}

	private void CallClosedCallback()
	{
		m_CloseCheckAnimator = null;
		ClearContents();
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetShow(isShow: false);
		}
		Object.Destroy(base.gameObject);
		s_activedInstance = null;
		if (m_fpClosedFP != null)
		{
			m_fpClosedFP(this, null);
		}
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
			m_buttonGuide_SelContent = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_BTNGUIDE_SEL_CONTENT");
			m_buttonGuide_ExitMenu = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_BTNGUIDE_EXIT_MENU");
			m_buttonGuide_PlayStop = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_BTNGUIDE_PLAY_STOP");
			m_buttonGuide_ScrollToTop = GameGlobalUtil.GetXlsProgramText("RECORD_MENU_BTNGUIDE_SCROLL_TO_TOP");
			m_isInitializedButtonGuidText = true;
		}
		m_ButtonGuide.ClearContents();
		m_ButtonGuide.SetCanvasSortOrder(100);
		m_ButtonGuide.AddContent(m_buttonGuide_SelContent, PadInput.GameInput.LStickY);
		m_ButtonGuide.AddContent(m_buttonGuide_PlayStop, PadInput.GameInput.CircleButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ExitMenu, PadInput.GameInput.CrossButton);
		m_ButtonGuide.AddContent(m_buttonGuide_ScrollToTop, PadInput.GameInput.TriangleButton);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, isEnable: false, isNeedAlign: false);
		m_ButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
		m_ButtonGuide.SetShow(isShow: true);
	}

	private IEnumerator InitContents()
	{
		ClearContents();
		float fContanierSize = 0f;
		if (!(m_ContentSrcObject == null))
		{
			List<Xls.CollSounds> xlsColSounds = Xls.CollSounds.datas;
			int count = xlsColSounds.Count;
			if (xlsColSounds.Count > 0)
			{
				Xls.CollSounds xlsColSound = null;
				GameObject contentObject = null;
				RecordContentPlus recordContent = null;
				GameSwitch gameSwitch = GameSwitch.GetInstance();
				sbyte switchState = 0;
				for (int i = 0; i < count; i++)
				{
					xlsColSound = xlsColSounds[i];
					if (xlsColSound.m_iCategory == 1)
					{
						switchState = gameSwitch.GetSoundSwitch(xlsColSound.m_iIdx);
						if (switchState == 1 || switchState == 2)
						{
							contentObject = Object.Instantiate(m_ContentSrcObject);
							recordContent = contentObject.GetComponent<RecordContentPlus>();
							recordContent.InitRecordContent(m_Contents.Count, xlsColSound, this);
							recordContent.activeNewMark = switchState == 1;
							m_Contents.Add(recordContent);
							recordContent.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
							contentObject.SetActive(value: false);
							yield return null;
						}
					}
				}
				float fCurY = 0f;
				float fHeight = 0f;
				float fTotalHeight = 0f;
				count = m_Contents.Count;
				for (int j = 0; j < count; j++)
				{
					recordContent = m_Contents[j];
					recordContent.gameObject.SetActive(value: true);
					recordContent.rectTransform.anchoredPosition = new Vector2(recordContent.rectTransform.anchoredPosition.x, fCurY);
					fHeight = recordContent.rectTransform.rect.height * recordContent.rectTransform.localScale.y;
					fCurY -= fHeight + m_ContentInterval;
					fTotalHeight += fHeight;
				}
				if (count > 1)
				{
					fTotalHeight += m_ContentInterval * (float)(count - 1);
				}
				fContanierSize = Mathf.Max(b: fTotalHeight + m_BottomSpacing, a: m_ScrollRect.viewport.rect.height);
			}
		}
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fContanierSize);
		m_ScrollHandler.ResetScrollRange();
		m_ContentContanierRT.anchoredPosition = new Vector2(m_ContentContanierRT.anchoredPosition.x, 0f);
		m_ButtonGuide.SetContentEnable(m_buttonGuide_ScrollToTop, m_ScrollHandler.isScrollable);
		if (m_Contents.Count > 0)
		{
			SetOnCursorContent(m_Contents[0]);
		}
		m_isInputBlock = false;
	}

	private void ClearContents()
	{
		int count = m_Contents.Count;
		for (int i = 0; i < count; i++)
		{
			Object.Destroy(m_Contents[i].gameObject);
		}
		m_Contents.Clear();
	}

	private void SetOnCursorContent(RecordContentPlus content, bool isAdjustScrollPos = true)
	{
		if (!(m_OnCursorContent == content))
		{
			if (m_OnCursorContent != null)
			{
				m_OnCursorContent.onCursor = false;
				m_OnCursorContent.activeNewMark = false;
			}
			if (content != null)
			{
				content.onCursor = true;
			}
			m_OnCursorContent = content;
			if (m_OnCursorContent.activeNewMark)
			{
				GameSwitch.GetInstance().SetSoundSwitch(m_OnCursorContent.xlsData.m_iIdx, 2);
			}
			if (isAdjustScrollPos)
			{
				AdjustScrollPos_byOnCursonContent();
			}
			if (m_fpChangedSelectContent != null)
			{
				m_fpChangedSelectContent(this, m_OnCursorContent);
			}
		}
	}

	public void TouchContent(RecordContentPlus content)
	{
		SetOnCursorContent(content);
	}

	private bool ChangeOnCursorContent(bool isUpSide, bool isAdjustScrollPos = true)
	{
		int count = m_Contents.Count;
		if (m_OnCursorContent == null)
		{
			if (count > 0)
			{
				SetOnCursorContent(m_Contents[count - 1], isAdjustScrollPos: false);
			}
			return false;
		}
		int num = m_Contents.IndexOf(m_OnCursorContent);
		num = ((!isUpSide) ? (num + 1) : (num - 1));
		if (num < 0)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = count - 1;
		}
		else if (num >= count)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = 0;
		}
		SetOnCursorContent(m_Contents[num], isAdjustScrollPos);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		if (m_ButtonGuide != null)
		{
			m_ButtonGuide.SetContentActivate(m_buttonGuide_SelContent, isActivate: true);
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
		if (m_OnCursorContent == null)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContanierRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		float y = m_OnCursorContent.rectTransform.offsetMax.y;
		float f2 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
		int num4 = m_Contents.IndexOf(m_OnCursorContent);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(f2) < num3)
		{
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_Contents[num5].rectTransform;
				f2 = rectTransform.offsetMax.y - rectTransform.rect.height * rectTransform.localScale.y;
				if (Mathf.CeilToInt(f2) >= num3)
				{
					SetOnCursorContent(m_Contents[num5]);
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
			int count = m_Contents.Count;
			for (int i = num4 + 1; i < count; i++)
			{
				rectTransform = m_Contents[i].rectTransform;
				y = rectTransform.offsetMax.y;
				if (Mathf.FloorToInt(y) <= num2)
				{
					SetOnCursorContent(m_Contents[i]);
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
		if (!(m_OnCursorContent == null))
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContanierRT.offsetMax.y;
			float num2 = num - height;
			float y = m_OnCursorContent.rectTransform.offsetMax.y;
			float num3 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
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

	public void OnClick_ContentScrollToTop()
	{
		if (m_ScrollHandler.isScrollable)
		{
			m_ScrollHandler.ScrollToTargetPos(0f);
			int count = m_Contents.Count;
			if (count > 0)
			{
				SetOnCursorContent(m_Contents[0], isAdjustScrollPos: false);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	public void OnProc_PlayStopRecord(RecordContentPlus recordContent, bool isPlay)
	{
		if (m_AudioManager == null)
		{
			return;
		}
		m_AudioManager.PlayUISound("Push_WatchOK");
		if (isPlay)
		{
			if (m_CurPlayingContent != null)
			{
				m_CurPlayingContent.isPlaying = false;
			}
			string strIDrecord = recordContent.xlsData.m_strIDrecord;
			strIDrecord = strIDrecord.Trim();
			if (strIDrecord != string.Empty)
			{
				bool flag = ContiDataHandler.IsExistConti(strIDrecord);
				if (flag)
				{
					m_CurPlayingContent = recordContent;
					GameSwitch.GetInstance().SetRecordFaterConti(strIDrecord);
				}
				m_aniDialogBlackBG.gameObject.SetActive(value: true);
				m_isDialogBlackDisappear = false;
				m_eDialogAniState = GameDefine.eAnimChangeState.none;
				m_isRunEvent = true;
				m_CurPlayingContent.isPlaying = flag;
				m_isInputBlock = true;
				EventEngine.GetInstance().EnableRecordFaterEventObj("RUN_RECORD", FinishConti);
				if (m_ButtonGuide != null)
				{
					m_ButtonGuide.SetShow(isShow: false);
				}
			}
			else
			{
				m_AudioManager.PlayKey(5, recordContent.xlsData.m_strIDSnd);
				m_CurPlayingContent = recordContent;
				m_CurPlayingContent.isPlaying = true;
			}
		}
		else
		{
			if (m_CurPlayingContent != null)
			{
				m_CurPlayingContent.isPlaying = false;
			}
			m_AudioManager.Stop(5);
			m_CurPlayingContent = null;
		}
		m_PlayingText.gameObject.SetActive(isPlay);
		GameSwitch.GetInstance().AddTrophyCnt("trp_00029");
	}

	public RecordContentPlus GetContentNearBy(RecordContentPlus baseContent, int idxOffset)
	{
		int count = m_Contents.Count;
		if (count <= 0)
		{
			return null;
		}
		if (baseContent == null)
		{
			baseContent = ((!(m_OnCursorContent != null)) ? m_Contents[0] : m_OnCursorContent);
		}
		if (idxOffset == 0)
		{
			return baseContent;
		}
		int num = m_Contents.IndexOf(baseContent);
		int num2 = num + idxOffset;
		return (num2 < 0 || num2 >= count) ? null : m_Contents[num2];
	}

	private void ShowTutorialPopup()
	{
		string strTutorialKey = "tuto_00020";
		m_isTutorialActivated = TutorialPopup.isShowAble(strTutorialKey);
		if (m_isTutorialActivated)
		{
			StartCoroutine(TutorialPopup.Show(strTutorialKey, OnClosed_TutorialPopup, base.gameObject.GetComponentInChildren<Canvas>()));
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

	public void TouchCloseRecordMenu()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			CloseRecordMenu();
		}
	}
}
