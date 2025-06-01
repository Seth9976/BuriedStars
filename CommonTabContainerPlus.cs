using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonTabContainerPlus : MonoBehaviour
{
	private enum PushingInputType
	{
		None,
		L1,
		Left_LS,
		Left_RS,
		R1,
		Right_LS,
		Right_RS
	}

	public class TabButtonInfo
	{
		private GameObject m_GameObject;

		private RectTransform m_RectTransform;

		private CommonTabButtonPlus m_TabButtonComp;

		private object m_Tag;

		public GameObject gameObject => m_GameObject;

		public RectTransform rectTransform => m_RectTransform;

		public CommonTabButtonPlus tabButtonComp => m_TabButtonComp;

		public object tag
		{
			get
			{
				return m_Tag;
			}
			set
			{
				m_Tag = value;
			}
		}

		public TabButtonInfo(GameObject gameObj, object _tag = null)
		{
			m_GameObject = gameObj;
			m_RectTransform = m_GameObject.GetComponent<RectTransform>();
			m_TabButtonComp = m_GameObject.GetComponent<CommonTabButtonPlus>();
			m_Tag = _tag;
			FontManager.ResetTextFontByCurrentLanguage(m_TabButtonComp.m_SelButton.m_Text);
			FontManager.ResetTextFontByCurrentLanguage(m_TabButtonComp.m_NotSelButton.m_Text);
			FontManager.ResetTextFontByCurrentLanguage(m_TabButtonComp.m_Banner.m_Text);
		}
	}

	public class TabCreateInfo
	{
		public string m_Text = string.Empty;

		public int m_BannerNum;

		public object m_Tag;
	}

	public delegate List<TabCreateInfo> GetTabCreateInfos();

	public ScrollRect m_ScrollRect;

	public RectTransform m_ContainerRect;

	public Object m_ButtonSrcObject;

	public float m_ButtonInterval;

	public bool m_AutoBuild = true;

	public bool m_SkipDisableButton = true;

	[Header("SelectionCursor/PadButton")]
	public Button m_SelectionIconButton;

	private RectTransform m_SelectionIconRT;

	[Header("Scroll Members")]
	public Button m_ScrollButtonPrev;

	public Image m_ImageScrollBtnPrev;

	public Sprite m_SpriteEnableScrollBtnPrev;

	public Sprite m_SpriteDisableScrollBtnPrev;

	public Button m_ScrollButtonNext;

	public Image m_ImageScrollBtnNext;

	public Sprite m_SpriteEnableScrollBtnNext;

	public Sprite m_SpriteDisableScrollBtnNext;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	private float m_ScrollRepeatTime = 0.3f;

	private PushingInputType m_PusingInputType;

	private float m_fButtonPusingTime;

	private bool m_isInputBlock;

	private AudioManager m_AudioManager;

	private List<TabButtonInfo> m_TabButtonInfos = new List<TabButtonInfo>();

	private TabButtonInfo m_CurSelectedTabInfo;

	private bool m_isChangingTab;

	private TabButtonInfo m_CurChangingTabInfo;

	private bool m_isNeedButtonAlign;

	private float m_fContentTotalWidth;

	private float m_fContentUnitWidth;

	private GetTabCreateInfos m_fpGetTabCreateInfo;

	private GameDefine.EventProc m_fpChangedSelectTab;

	private GameDefine.EventProc m_fpChangingSelectTab;

	private GameDefine.EventProc m_fpPressTabButton;

	public bool isInputBlock
	{
		get
		{
			return m_isInputBlock;
		}
		set
		{
			m_isInputBlock = value;
		}
	}

	public List<TabButtonInfo> tabButtonInfos => m_TabButtonInfos;

	public TabButtonInfo selectedTabInfo => m_CurSelectedTabInfo;

	public bool isChaningTab => m_isChangingTab;

	public GetTabCreateInfos getTabCreateInfoFP
	{
		get
		{
			return m_fpGetTabCreateInfo;
		}
		set
		{
			m_fpGetTabCreateInfo = ((value == null) ? null : new GetTabCreateInfos(value.Invoke));
		}
	}

	public GameDefine.EventProc onChangedSelectTab
	{
		get
		{
			return m_fpChangedSelectTab;
		}
		set
		{
			m_fpChangedSelectTab = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public GameDefine.EventProc onChangingSelectTab
	{
		get
		{
			return m_fpChangingSelectTab;
		}
		set
		{
			m_fpChangingSelectTab = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	public GameDefine.EventProc onPressTabButton
	{
		get
		{
			return m_fpPressTabButton;
		}
		set
		{
			m_fpPressTabButton = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void Awake()
	{
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Horizontal, m_ScrollRect, m_ContainerRect, m_ScrollButtonPrev, m_ScrollButtonNext);
		m_ScrollHandler.onScrollButtonEvent = OnScrollButtonProc;
		if (m_ButtonSrcObject is GameObject)
		{
			(m_ButtonSrcObject as GameObject).SetActive(value: false);
		}
		if (m_SelectionIconButton != null)
		{
			m_SelectionIconRT = m_SelectionIconButton.GetComponent<RectTransform>();
			m_SelectionIconButton.gameObject.SetActive(value: false);
		}
		m_AudioManager = GameGlobalUtil.GetAudioManager();
	}

	private void OnDestroy()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_AudioManager = null;
		if (m_TabButtonInfos != null)
		{
			m_TabButtonInfos.Clear();
		}
		m_CurSelectedTabInfo = null;
		m_CurChangingTabInfo = null;
		m_fpGetTabCreateInfo = null;
		m_fpChangedSelectTab = null;
		m_fpChangingSelectTab = null;
		m_fpPressTabButton = null;
	}

	private void Update()
	{
		if (m_isNeedButtonAlign)
		{
			AlignTavButtonObjects();
			m_isNeedButtonAlign = false;
			return;
		}
		UpdateSelectionCursor();
		if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
		}
		else
		{
			if (m_TabButtonInfos.Count <= 1 || m_isInputBlock || PopupDialoguePlus.IsAnyPopupActivated() || !TutorialPopup.IsTutorialPopupEnd())
			{
				return;
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.L1Button))
			{
				m_PusingInputType = PushingInputType.L1;
				ButtonPadInput.PushingInputButton(PadInput.GameInput.L1Button, m_ScrollButtonPrev);
				ChangeSelectingTab(isToNext: false);
				m_fButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.R1Button))
			{
				m_PusingInputType = PushingInputType.R1;
				ButtonPadInput.PushingInputButton(PadInput.GameInput.R1Button, m_ScrollButtonNext);
				ChangeSelectingTab(isToNext: true);
				m_fButtonPusingTime = 0f;
			}
			else
			{
				if (!m_isChangingTab)
				{
					return;
				}
				float axisValue = GamePadInput.GetAxisValue(PadInput.GameInput.LStickX);
				float axisValue2 = GamePadInput.GetAxisValue(PadInput.GameInput.RStickX);
				if (GamePadInput.IsButtonState_Pushing(PadInput.GameInput.L1Button))
				{
					ProcPusingMoveInput(PushingInputType.L1, moveRight: false);
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing) && axisValue <= -0.3f)
				{
					ProcPusingMoveInput(PushingInputType.Left_LS, moveRight: false);
				}
				else if (GamePadInput.IsRStickState_Left(PadInput.ButtonState.Pushing) && axisValue2 <= -0.3f)
				{
					ProcPusingMoveInput(PushingInputType.Left_RS, moveRight: false);
				}
				else if (GamePadInput.IsButtonState_Pushing(PadInput.GameInput.R1Button))
				{
					ProcPusingMoveInput(PushingInputType.R1, moveRight: true);
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && axisValue >= 0.3f)
				{
					ProcPusingMoveInput(PushingInputType.Right_LS, moveRight: true);
				}
				else if (GamePadInput.IsRStickState_Right(PadInput.ButtonState.Pushing) && axisValue2 >= 0.3f)
				{
					ProcPusingMoveInput(PushingInputType.Right_RS, moveRight: true);
				}
				else
				{
					m_PusingInputType = PushingInputType.None;
					m_fButtonPusingTime = 0f;
				}
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) && m_SelectionIconButton.gameObject.activeSelf)
				{
					ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_SelectionIconButton);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_OK");
					}
					CallBack_PressSubmitButton(PadInput.GameInput.CircleButton);
				}
				else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
				{
					CancelChangingState();
				}
			}
		}
	}

	private void ProcPusingMoveInput(PushingInputType pushingInputType, bool moveRight)
	{
		if (m_PusingInputType != pushingInputType)
		{
			m_PusingInputType = pushingInputType;
			ChangeSelectingTab(moveRight);
			m_fButtonPusingTime = 0f;
			return;
		}
		m_fButtonPusingTime += Time.deltaTime;
		if (m_fButtonPusingTime >= m_ScrollRepeatTime)
		{
			m_fButtonPusingTime = 0f;
			ChangeSelectingTab(moveRight);
		}
	}

	public void CancelChangingState()
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Cancel");
		}
		CallBack_PressSubmitButton(PadInput.GameInput.CrossButton);
		CancelSelectingTab();
	}

	public void BuildTabButtonObjects()
	{
		if (m_fpGetTabCreateInfo == null)
		{
			return;
		}
		ClearTabButtonObjects();
		List<TabCreateInfo> list = m_fpGetTabCreateInfo();
		if (list == null || list.Count <= 0)
		{
			return;
		}
		int count = list.Count;
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		TabButtonInfo tabButtonInfo = null;
		TabCreateInfo tabCreateInfo = null;
		m_fContentUnitWidth = 0f;
		m_fContentTotalWidth = 0f;
		for (int i = 0; i < count; i++)
		{
			tabCreateInfo = list[i];
			GameObject gameObject = Object.Instantiate(m_ButtonSrcObject, zero, identity) as GameObject;
			gameObject.name = $"TabButton_{i}";
			tabButtonInfo = new TabButtonInfo(gameObject, i);
			m_TabButtonInfos.Add(tabButtonInfo);
			tabButtonInfo.rectTransform.SetParent(m_ContainerRect, worldPositionStays: false);
			tabButtonInfo.tabButtonComp.onSelectedProc = OnSelected_TabButton;
			tabButtonInfo.tabButtonComp.text = tabCreateInfo.m_Text;
			tabButtonInfo.tabButtonComp.bannerNum = list[i].m_BannerNum;
			tabButtonInfo.tag = tabCreateInfo.m_Tag;
			tabButtonInfo.tabButtonComp.curState = CommonTabButtonPlus.State.NotSelected;
			m_fContentUnitWidth = tabButtonInfo.rectTransform.rect.width;
			m_fContentTotalWidth += tabButtonInfo.rectTransform.rect.width;
		}
		if (count > 0 && m_ButtonInterval != 0f)
		{
			m_fContentTotalWidth += m_ButtonInterval * (float)(count - 1);
		}
		if (count > 1)
		{
			if (m_ScrollButtonPrev != null)
			{
				m_ScrollButtonPrev.enabled = true;
			}
			if (m_ScrollButtonNext != null)
			{
				m_ScrollButtonNext.enabled = true;
			}
			if (m_ImageScrollBtnPrev != null && m_SpriteEnableScrollBtnPrev != null)
			{
				m_ImageScrollBtnPrev.sprite = m_SpriteEnableScrollBtnPrev;
			}
			if (m_ImageScrollBtnNext != null && m_SpriteEnableScrollBtnNext != null)
			{
				m_ImageScrollBtnNext.sprite = m_SpriteEnableScrollBtnNext;
			}
		}
		else
		{
			if (m_ScrollButtonPrev != null)
			{
				m_ScrollButtonPrev.enabled = false;
			}
			if (m_ScrollButtonNext != null)
			{
				m_ScrollButtonNext.enabled = true;
			}
			if (m_ImageScrollBtnPrev != null && m_SpriteDisableScrollBtnPrev != null)
			{
				m_ImageScrollBtnPrev.sprite = m_SpriteDisableScrollBtnPrev;
			}
			if (m_ImageScrollBtnNext != null && m_SpriteDisableScrollBtnNext != null)
			{
				m_ImageScrollBtnNext.sprite = m_SpriteDisableScrollBtnNext;
			}
		}
		m_isNeedButtonAlign = true;
	}

	public void ClearTabButtonObjects()
	{
		int count = m_TabButtonInfos.Count;
		TabButtonInfo tabButtonInfo = null;
		for (int i = 0; i < count; i++)
		{
			tabButtonInfo = m_TabButtonInfos[i];
			Object.Destroy(tabButtonInfo.gameObject);
		}
		m_TabButtonInfos.Clear();
		m_CurSelectedTabInfo = null;
		m_CurChangingTabInfo = null;
	}

	private void AlignTavButtonObjects()
	{
		float num = 0f - (m_fContentTotalWidth - m_fContentUnitWidth) * 0.5f;
		int count = m_TabButtonInfos.Count;
		TabButtonInfo tabButtonInfo = null;
		for (int i = 0; i < count; i++)
		{
			tabButtonInfo = m_TabButtonInfos[i];
			tabButtonInfo.rectTransform.anchoredPosition = new Vector2(num, tabButtonInfo.rectTransform.anchoredPosition.y);
			num += m_fContentUnitWidth + m_ButtonInterval;
			tabButtonInfo.gameObject.SetActive(value: true);
		}
		RectTransform viewport = m_ScrollRect.viewport;
		float size = Mathf.Max(viewport.rect.width, m_fContentTotalWidth);
		m_ContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		m_ScrollHandler.pageLength = m_fContentUnitWidth;
		m_ScrollHandler.ResetScrollRange();
	}

	public void OnSelected_TabButton(object sender, object arg)
	{
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_OK");
		}
		SetSelectedTabButton(arg as CommonTabButtonPlus);
	}

	private void SetSelectedTabButton(CommonTabButtonPlus tabButtonComp)
	{
		TabButtonInfo tabButtonInfo = null;
		if (tabButtonComp != null)
		{
			int count = m_TabButtonInfos.Count;
			TabButtonInfo tabButtonInfo2 = null;
			for (int i = 0; i < count; i++)
			{
				tabButtonInfo2 = m_TabButtonInfos[i];
				if (tabButtonInfo2.tabButtonComp == tabButtonComp)
				{
					tabButtonInfo = tabButtonInfo2;
					break;
				}
			}
		}
		SetSelectedTabButtonInfo(tabButtonInfo, isAdjustTabButton: true);
	}

	private void SetSelectedTabButtonInfo(TabButtonInfo tabButtonInfo, bool isAdjustTabButton = false, bool fireEvent = true)
	{
		m_isChangingTab = false;
		m_PusingInputType = PushingInputType.None;
		if (m_CurChangingTabInfo != null)
		{
			m_CurChangingTabInfo.tabButtonComp.curState = CommonTabButtonPlus.State.NotSelected;
			m_CurChangingTabInfo = null;
		}
		m_PusingInputType = PushingInputType.None;
		m_fButtonPusingTime = 0f;
		if (m_CurSelectedTabInfo != tabButtonInfo)
		{
			if (m_CurSelectedTabInfo != null)
			{
				m_CurSelectedTabInfo.tabButtonComp.curState = CommonTabButtonPlus.State.NotSelected;
			}
			if (tabButtonInfo != null)
			{
				tabButtonInfo.tabButtonComp.curState = CommonTabButtonPlus.State.Selected;
			}
			m_CurSelectedTabInfo = tabButtonInfo;
		}
		else if (m_CurSelectedTabInfo != null)
		{
			m_CurSelectedTabInfo.tabButtonComp.curState = CommonTabButtonPlus.State.Selected;
		}
		if (isAdjustTabButton)
		{
			AdjustTabButton(m_CurSelectedTabInfo);
		}
		if (fireEvent)
		{
			if (m_fpChangingSelectTab != null)
			{
				m_fpChangingSelectTab(m_CurSelectedTabInfo, m_isChangingTab);
			}
			if (m_fpChangedSelectTab != null)
			{
				m_fpChangedSelectTab(m_CurSelectedTabInfo, null);
			}
		}
	}

	public bool SetSelectedTab(string strText, bool isAdjustTabButton = true)
	{
		int count = m_TabButtonInfos.Count;
		TabButtonInfo tabButtonInfo = null;
		for (int i = 0; i < count; i++)
		{
			tabButtonInfo = m_TabButtonInfos[i];
			if (tabButtonInfo.tabButtonComp.text.Equals(strText))
			{
				SetSelectedTabButtonInfo(tabButtonInfo, isAdjustTabButton);
				return true;
			}
		}
		return false;
	}

	public bool SetSelectedTab_byIdx(int idx, bool isAdjustTabButton = true)
	{
		if (idx < 0 || idx >= m_TabButtonInfos.Count)
		{
			return false;
		}
		SetSelectedTabButtonInfo(m_TabButtonInfos[idx], isAdjustTabButton);
		return true;
	}

	public bool SetSelectedTab_byObject(TabButtonInfo tabBtnInfoObject, bool isAdjustTabButton = true)
	{
		if (!m_TabButtonInfos.Contains(tabBtnInfoObject))
		{
			return false;
		}
		SetSelectedTabButtonInfo(tabBtnInfoObject, isAdjustTabButton);
		return true;
	}

	public CommonTabButtonPlus GetTabButton(int idx)
	{
		if (idx < 0 || idx >= m_TabButtonInfos.Count)
		{
			return null;
		}
		return m_TabButtonInfos[idx].tabButtonComp;
	}

	public CommonTabButtonPlus GetTabButton(object tag)
	{
		List<TabButtonInfo>.Enumerator enumerator = m_TabButtonInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TabButtonInfo current = enumerator.Current;
			if (current == null || current.tag == null || !current.tag.Equals(tag))
			{
				continue;
			}
			return current.tabButtonComp;
		}
		return null;
	}

	private bool ChangeSelectingTab(bool isToNext)
	{
		if (m_TabButtonInfos.Count <= 1)
		{
			return false;
		}
		if (m_SkipDisableButton)
		{
			int num = 0;
			int count = m_TabButtonInfos.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_TabButtonInfos[i].tabButtonComp.isEnableButton)
				{
					num++;
				}
			}
			if (num <= 1)
			{
				return false;
			}
		}
		TabButtonInfo tabButtonInfo = ((m_CurChangingTabInfo == null) ? m_CurSelectedTabInfo : m_CurChangingTabInfo);
		TabButtonInfo tabButtonInfo2 = null;
		int num2 = 0;
		if (tabButtonInfo != null)
		{
			int num3 = m_TabButtonInfos.IndexOf(tabButtonInfo);
			if (!m_SkipDisableButton)
			{
				num2 = ((!isToNext) ? (num3 - 1) : (num3 + 1));
				if (num2 >= m_TabButtonInfos.Count)
				{
					num2 = ((!m_ScrollLoopEnable) ? (m_TabButtonInfos.Count - 1) : 0);
				}
				else if (num2 < 0)
				{
					num2 = (m_ScrollLoopEnable ? (m_TabButtonInfos.Count - 1) : 0);
				}
			}
			else
			{
				int num4 = num3;
				do
				{
					num2 = ((!isToNext) ? (num4 - 1) : (num4 + 1));
					if (num2 >= m_TabButtonInfos.Count)
					{
						num2 = ((!m_ScrollLoopEnable) ? (m_TabButtonInfos.Count - 1) : 0);
					}
					else if (num2 < 0)
					{
						num2 = (m_ScrollLoopEnable ? (m_TabButtonInfos.Count - 1) : 0);
					}
					if (num4 == num2)
					{
						break;
					}
					tabButtonInfo2 = m_TabButtonInfos[num2];
					if (tabButtonInfo2.tabButtonComp.isEnableButton)
					{
						break;
					}
					num4 = num2;
				}
				while (num2 != num3);
			}
		}
		tabButtonInfo2 = m_TabButtonInfos[num2];
		m_isChangingTab = true;
		m_CurChangingTabInfo = tabButtonInfo2;
		UpdateSelectionCursor();
		if (tabButtonInfo2 != tabButtonInfo)
		{
			if (tabButtonInfo2 != null)
			{
				tabButtonInfo2.tabButtonComp.curState = ((!m_isChangingTab) ? CommonTabButtonPlus.State.Selected : CommonTabButtonPlus.State.Selecting);
			}
			if (tabButtonInfo != null)
			{
				tabButtonInfo.tabButtonComp.curState = CommonTabButtonPlus.State.NotSelected;
			}
			if (m_fpChangingSelectTab != null)
			{
				m_fpChangingSelectTab(tabButtonInfo2, m_isChangingTab);
			}
			PadInput.GameInput gameInputType = PadInput.GameInput.None;
			switch (m_PusingInputType)
			{
			case PushingInputType.L1:
				gameInputType = PadInput.GameInput.L1Button;
				break;
			case PushingInputType.Left_LS:
				gameInputType = PadInput.GameInput.L1Button;
				break;
			case PushingInputType.Left_RS:
				gameInputType = PadInput.GameInput.L1Button;
				break;
			case PushingInputType.R1:
				gameInputType = PadInput.GameInput.R1Button;
				break;
			case PushingInputType.Right_LS:
				gameInputType = PadInput.GameInput.R1Button;
				break;
			case PushingInputType.Right_RS:
				gameInputType = PadInput.GameInput.R1Button;
				break;
			}
			CallBack_PressSubmitButton(gameInputType);
			AdjustTabButton(m_CurChangingTabInfo);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_CTG");
			}
			return true;
		}
		return false;
	}

	private void CancelSelectingTab()
	{
		SetSelectedTabButtonInfo(m_CurSelectedTabInfo, isAdjustTabButton: true, fireEvent: false);
		if (m_fpChangingSelectTab != null)
		{
			m_fpChangingSelectTab(m_CurSelectedTabInfo, false);
		}
	}

	private void AdjustTabButton(TabButtonInfo tabButtonInfo)
	{
		if (tabButtonInfo != null)
		{
			float width = m_ScrollRect.viewport.rect.width;
			float width2 = m_ContainerRect.rect.width;
			float num = width2 * tabButtonInfo.rectTransform.anchorMin.x;
			float num2 = tabButtonInfo.rectTransform.offsetMin.x + num;
			float num3 = tabButtonInfo.rectTransform.offsetMax.x + num;
			float num4 = 0f - m_ContainerRect.offsetMin.x;
			float num5 = num4 + width;
			if (num2 < num4)
			{
				float fTargetPos = 0f - num2;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (num3 > num5)
			{
				float fTargetPos2 = width - num3;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	private void UpdateSelectionCursor()
	{
		if (!(m_SelectionIconRT == null))
		{
			if (!m_isChangingTab || m_CurChangingTabInfo == null || m_ScrollHandler.IsScrolling)
			{
				m_SelectionIconRT.gameObject.SetActive(value: false);
				return;
			}
			Vector3[] array = new Vector3[4];
			m_CurChangingTabInfo.rectTransform.GetWorldCorners(array);
			Vector3 position = new Vector3(array[2].x, (array[2].y + array[3].y) * 0.5f, 0f);
			m_SelectionIconRT.gameObject.SetActive(value: true);
			m_SelectionIconRT.position = position;
		}
	}

	public void OnClick_TabSelectButton()
	{
		if (m_isChangingTab)
		{
			if (m_CurChangingTabInfo != m_CurSelectedTabInfo)
			{
				SetSelectedTabButtonInfo(m_CurChangingTabInfo);
			}
			else
			{
				CancelSelectingTab();
			}
		}
	}

	private void CallBack_PressSubmitButton(PadInput.GameInput gameInputType)
	{
		if (m_fpPressTabButton != null)
		{
			m_fpPressTabButton(this, gameInputType);
		}
	}

	private void OnScrollButtonProc(object sender, object args)
	{
		if (args is UIUtil_ScrollHandler.ButtonEvent buttonEvent && buttonEvent == UIUtil_ScrollHandler.ButtonEvent.Click && m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_CTG");
		}
	}

	public void OnClickLeftRightTabButton(bool isRight)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			ChangeSelectingTab(isRight);
			SetSelectedTabButtonInfo(m_CurChangingTabInfo);
		}
	}
}
