using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class KeywordContainer
{
	public enum SlotPage
	{
		Prev,
		Current,
		Next,
		Count
	}

	public enum MenuType
	{
		Memo,
		CollectionKeyword
	}

	public class EventArg_OnCursorChanged
	{
		public int m_prevOnCursorDataIndex = -1;

		public Xls.CollKeyword m_prevOnCursorData;

		public KeywordSlotPlus m_prevOnCursorSlot;

		public int m_curOnCursorDataIndex = -1;

		public Xls.CollKeyword m_curOnCursorData;

		public KeywordSlotPlus m_curOnCursorSlot;
	}

	public class EventArg_ChangedCurrentPage
	{
		public int m_pageCount;

		public int m_prevPageIndex = -1;

		public int m_currentPageIndex = -1;
	}

	private MenuType m_menuType = MenuType.CollectionKeyword;

	private GridLayoutGroup m_GridLayoutGroup;

	private RectTransform m_ContainerRT;

	private int m_slotColumnCount;

	private int m_slotRawCount;

	private int m_slotCountPerPage;

	private int m_totalSlotCount;

	private GameObject m_SrcSlotObject;

	private List<KeywordSlotPlus> m_keywordSlots = new List<KeywordSlotPlus>();

	private List<Xls.CollKeyword> m_keywordDatas;

	private SortedDictionary<int, KeywordSlotPlus> m_dataIndexToSlotMap = new SortedDictionary<int, KeywordSlotPlus>();

	private int m_pageCount;

	private int m_curPageIndex;

	private int m_onCursorDataIndex = -1;

	private float m_slotWidth;

	private float m_pageWidth;

	private AudioManager m_AudioManager;

	private float m_keyPushingTime;

	private float m_keyInputRepeatTimeBound = 0.2f;

	private SlotPage m_pageScrollDir = SlotPage.Current;

	private int m_targetPageIndex = -1;

	private const float c_pageScrollDuration = 0.25f;

	private float m_pageScrollSpeed;

	private float m_remainScrollDistance;

	private float m_checkScrolledDistance;

	private float m_spareScrollDistance;

	private bool m_isOnCursorAtFirstSlot;

	private int m_curScrollingPageIndex = -1;

	private int m_roopedColumnCount;

	private int m_reflashSlotColumnIdx;

	private GameDefine.EventProc m_OnCursorChanged;

	private GameDefine.EventProc m_ChangedCurrentPage;

	private GameDefine.EventProc m_KeyInputedScrollPage;

	private GameDefine.EventProc m_KeyInputedMoveCursor;

	public int SlotCountPerPage => m_slotCountPerPage;

	public int TotalSlotCount => m_totalSlotCount;

	public List<KeywordSlotPlus> KeywordSlots => m_keywordSlots;

	public List<Xls.CollKeyword> KeywordDatas => m_keywordDatas;

	public int PageCount => m_pageCount;

	public int CurrentPageIndex
	{
		get
		{
			return m_curPageIndex;
		}
		set
		{
			SetCurrentPage(value);
		}
	}

	public int OnCursorDataIndex => m_onCursorDataIndex;

	public Xls.CollKeyword OnCursorData
	{
		get
		{
			int num = ((m_keywordDatas != null) ? m_keywordDatas.Count : 0);
			return (m_onCursorDataIndex < 0 || m_onCursorDataIndex >= num) ? null : m_keywordDatas[m_onCursorDataIndex];
		}
	}

	public KeywordSlotPlus OnCursorSlot
	{
		get
		{
			KeywordSlotPlus value = null;
			return (!m_dataIndexToSlotMap.TryGetValue(m_onCursorDataIndex, out value)) ? null : value;
		}
	}

	public float SlotWidth => m_slotWidth;

	public float PageWidth => m_pageWidth;

	public float KeyInputRepeatTimeBound
	{
		get
		{
			return m_keyInputRepeatTimeBound;
		}
		set
		{
			m_keyInputRepeatTimeBound = value;
		}
	}

	public GameDefine.EventProc OnCursorChanged
	{
		set
		{
			m_OnCursorChanged = value;
		}
	}

	public GameDefine.EventProc ChangedCurrentPage
	{
		set
		{
			m_ChangedCurrentPage = value;
		}
	}

	public GameDefine.EventProc KeyInputedScrollPage
	{
		set
		{
			m_KeyInputedScrollPage = value;
		}
	}

	public GameDefine.EventProc KeyInputedMoveCursor
	{
		set
		{
			m_KeyInputedMoveCursor = value;
		}
	}

	public void Release()
	{
		ClearSlotObjects();
		m_GridLayoutGroup = null;
		m_ContainerRT = null;
		m_SrcSlotObject = null;
		if (m_keywordDatas != null)
		{
			m_keywordDatas.Clear();
		}
		if (m_dataIndexToSlotMap != null)
		{
			m_dataIndexToSlotMap.Clear();
		}
		m_AudioManager = null;
		m_OnCursorChanged = null;
		m_ChangedCurrentPage = null;
		m_KeyInputedScrollPage = null;
		m_KeyInputedMoveCursor = null;
	}

	public void InitContainer(GridLayoutGroup gridLayoutGroup, GameObject srcSlotObject, int columnCount = 4, int rawCount = 2, MenuType menuType = MenuType.CollectionKeyword)
	{
		m_menuType = menuType;
		m_GridLayoutGroup = gridLayoutGroup;
		m_SrcSlotObject = srcSlotObject;
		m_ContainerRT = m_GridLayoutGroup.gameObject.GetComponent<RectTransform>();
		m_slotColumnCount = columnCount;
		m_slotRawCount = rawCount;
		m_slotCountPerPage = m_slotColumnCount * m_slotRawCount;
		m_totalSlotCount = m_slotCountPerPage * 3;
		m_GridLayoutGroup.constraintCount = m_slotColumnCount * 3;
		m_AudioManager = GameGlobalUtil.GetAudioManager();
	}

	public IEnumerator CreateSlotObjects(bool isUsePreMadeObj = false, GameObject[] goPreObj = null, GameDefine.EventProc delFinished = null)
	{
		ClearSlotObjects(isUsePreMadeObj);
		bool isSuccess = false;
		if (m_totalSlotCount > 0 && !(m_ContainerRT == null) && !(m_SrcSlotObject == null))
		{
			if (isUsePreMadeObj)
			{
				m_keywordSlots.Clear();
				for (int i = 0; i < m_totalSlotCount; i++)
				{
					GameObject slotObject = goPreObj[i];
					KeywordSlotPlus component = slotObject.GetComponent<KeywordSlotPlus>();
					component.InitSlotState(isDragSlot: false, isPlaySlotAppearMot: false);
					component.OnNoticeClicked = OnNotice_SlotClicked;
					m_keywordSlots.Add(component);
					slotObject.SetActive(value: false);
				}
				for (int j = 0; j < m_totalSlotCount; j++)
				{
					GameObject slotObject = goPreObj[j];
					slotObject.GetComponent<KeywordSlotPlus>().InitCollectionContent(null, _isValid: false);
					slotObject.gameObject.SetActive(value: true);
				}
				yield return null;
			}
			else
			{
				for (int k = 0; k < m_totalSlotCount; k++)
				{
					GameObject slotObject2 = Object.Instantiate(m_SrcSlotObject);
					slotObject2.name = $"KeywordSlot_{k}";
					KeywordSlotPlus keywordSlot = slotObject2.GetComponent<KeywordSlotPlus>();
					keywordSlot.rectTransform.SetParent(m_ContainerRT, worldPositionStays: false);
					keywordSlot.InitSlotState(isDragSlot: false, isPlaySlotAppearMot: false);
					keywordSlot.OnNoticeClicked = OnNotice_SlotClicked;
					m_keywordSlots.Add(keywordSlot);
					slotObject2.SetActive(value: false);
					yield return null;
				}
				List<KeywordSlotPlus>.Enumerator enumerator = m_keywordSlots.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeywordSlotPlus current = enumerator.Current;
					current.InitCollectionContent(null, _isValid: false);
					current.gameObject.SetActive(value: true);
				}
				yield return null;
			}
			KeywordSlotPlus slotFirst = m_keywordSlots[0];
			KeywordSlotPlus slotLast = m_keywordSlots[1];
			m_slotWidth = slotLast.rectTransform.offsetMin.x - slotFirst.rectTransform.offsetMin.x;
			slotLast = m_keywordSlots[m_slotColumnCount];
			m_pageWidth = slotLast.rectTransform.offsetMin.x - slotFirst.rectTransform.offsetMin.x;
			m_GridLayoutGroup.padding.left = -(int)m_pageWidth;
			isSuccess = true;
		}
		delFinished?.Invoke(this, isSuccess);
	}

	public void ClearSlotObjects(bool isPreMadeSlot = false)
	{
		if (m_ContainerRT != null)
		{
			int childCount = m_ContainerRT.childCount;
			int num = childCount;
			while (num > 0)
			{
				num--;
				GameObject gameObject = m_ContainerRT.GetChild(num).gameObject;
				if (isPreMadeSlot)
				{
					gameObject.SetActive(value: false);
				}
				else
				{
					Object.Destroy(gameObject);
				}
			}
		}
		m_keywordSlots.Clear();
	}

	public void SetKeywordDatas(List<Xls.CollKeyword> keywordDatas, bool isCopyList = false)
	{
		SetOnCursorKeywordData(-1);
		m_dataIndexToSlotMap.Clear();
		if (!isCopyList)
		{
			m_keywordDatas = keywordDatas;
		}
		else
		{
			m_keywordDatas = new List<Xls.CollKeyword>();
			m_keywordDatas.AddRange(keywordDatas);
		}
		if (m_keywordDatas != null)
		{
			int count = m_keywordDatas.Count;
			m_pageCount = count / m_slotCountPerPage;
			if (count % m_slotCountPerPage > 0)
			{
				m_pageCount++;
			}
		}
		else
		{
			m_pageCount = 0;
		}
	}

	public void SetCurrentPage(int pageIndex)
	{
		if (pageIndex < 0 || pageIndex >= m_pageCount)
		{
			return;
		}
		EventArg_ChangedCurrentPage eventArg_ChangedCurrentPage = new EventArg_ChangedCurrentPage();
		eventArg_ChangedCurrentPage.m_pageCount = m_pageCount;
		eventArg_ChangedCurrentPage.m_prevPageIndex = m_curPageIndex;
		eventArg_ChangedCurrentPage.m_currentPageIndex = pageIndex;
		m_curPageIndex = pageIndex;
		SetPageContent(pageIndex, SlotPage.Current);
		if (m_pageCount > 1)
		{
			SetPageContent((pageIndex - 1 >= 0) ? (pageIndex - 1) : (m_pageCount - 1), SlotPage.Prev);
			SetPageContent((pageIndex + 1 < m_pageCount) ? (pageIndex + 1) : 0, SlotPage.Next);
			if (m_curPageIndex == 0)
			{
				HidePageContent(SlotPage.Prev);
			}
			if (m_curPageIndex == m_pageCount - 1)
			{
				HidePageContent(SlotPage.Next);
			}
		}
		else
		{
			HidePageContent(SlotPage.Prev);
			HidePageContent(SlotPage.Next);
		}
		if (m_ChangedCurrentPage != null)
		{
			m_ChangedCurrentPage(this, eventArg_ChangedCurrentPage);
		}
	}

	private int GetPageBaseSlotIndex(SlotPage slotPage)
	{
		return slotPage switch
		{
			SlotPage.Prev => 0, 
			SlotPage.Current => m_slotColumnCount, 
			SlotPage.Next => m_slotColumnCount * 2, 
			_ => 0, 
		};
	}

	private void SetPageContent(int pageIndex, SlotPage slotPage)
	{
		if (pageIndex >= 0 && pageIndex < m_pageCount)
		{
			int pageBaseSlotIndex = GetPageBaseSlotIndex(slotPage);
			int num = pageIndex * m_slotCountPerPage;
			for (int i = 0; i < m_slotColumnCount; i++)
			{
				SetSlotColumnContent(pageBaseSlotIndex + i, num + i, slotPage == SlotPage.Current);
			}
		}
	}

	private void SetSlotColumnContent(int baseSlotIndex, int baseDataIndex, bool enableClick)
	{
		if (baseSlotIndex < 0 || baseSlotIndex >= m_GridLayoutGroup.constraintCount)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int childCount = m_ContainerRT.childCount;
		int num3 = ((m_keywordDatas != null) ? m_keywordDatas.Count : 0);
		KeywordSlotPlus keywordSlotPlus = null;
		Xls.CollKeyword collKeyword = null;
		int num4 = -1;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			num = baseSlotIndex + m_GridLayoutGroup.constraintCount * i;
			if (num < 0 || num >= childCount)
			{
				break;
			}
			Transform child = m_ContainerRT.GetChild(num);
			if (child == null)
			{
				break;
			}
			keywordSlotPlus = child.gameObject.GetComponent<KeywordSlotPlus>();
			if (keywordSlotPlus == null)
			{
				break;
			}
			num2 = ((baseDataIndex < 0) ? (-1) : (baseDataIndex + m_slotColumnCount * i));
			collKeyword = ((num2 < 0 || num2 >= num3) ? null : m_keywordDatas[num2]);
			SetSlotContent(keywordSlotPlus, collKeyword);
			num4 = GetDataIndexInKeywordSlot(keywordSlotPlus);
			if (num4 >= 0 && num4 < num3)
			{
				m_dataIndexToSlotMap[num4] = null;
			}
			if (num2 >= 0 && num2 < num3)
			{
				m_dataIndexToSlotMap[num2] = keywordSlotPlus;
			}
			Button buttonComp = keywordSlotPlus.ButtonComp;
			if (buttonComp != null)
			{
				buttonComp.enabled = enableClick && collKeyword != null;
			}
		}
	}

	private void SetSlotContent(KeywordSlotPlus slot, Xls.CollKeyword keywordData)
	{
		if (slot == null)
		{
			return;
		}
		if (keywordData == null)
		{
			slot.InitCollectionContent(null, _isValid: false, enableNewTag: false, isIgnoreSame: false);
			return;
		}
		sbyte b = 0;
		switch (m_menuType)
		{
		case MenuType.Memo:
			b = GameSwitch.GetInstance().GetKeywordAllState(keywordData.m_iIndex);
			break;
		case MenuType.CollectionKeyword:
			b = GameSwitch.GetInstance().GetCollKeyword(keywordData.m_iIndex);
			break;
		}
		bool flag = b == 1;
		bool isValid = flag || b == 2;
		slot.InitCollectionContent(keywordData, isValid, flag, isIgnoreSame: false);
	}

	private void HideColumnContent(int slotColumnIndex)
	{
		if (slotColumnIndex < 0 || slotColumnIndex >= m_GridLayoutGroup.constraintCount)
		{
			return;
		}
		int num = 0;
		Transform transform = null;
		KeywordSlotPlus keywordSlotPlus = null;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			num = i * m_GridLayoutGroup.constraintCount + slotColumnIndex;
			transform = m_ContainerRT.GetChild(num);
			if (!(transform == null))
			{
				keywordSlotPlus = transform.gameObject.GetComponent<KeywordSlotPlus>();
				if (!(keywordSlotPlus == null))
				{
					keywordSlotPlus.HideCollectionContent();
				}
			}
		}
	}

	private void HidePageContent(SlotPage page)
	{
		int pageBaseSlotIndex = GetPageBaseSlotIndex(page);
		int num = 0;
		Transform transform = null;
		KeywordSlotPlus keywordSlotPlus = null;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			for (int j = 0; j < m_slotColumnCount; j++)
			{
				num = i * m_GridLayoutGroup.constraintCount + j + pageBaseSlotIndex;
				transform = m_ContainerRT.GetChild(num);
				if (!(transform == null))
				{
					keywordSlotPlus = transform.gameObject.GetComponent<KeywordSlotPlus>();
					if (!(keywordSlotPlus == null))
					{
						keywordSlotPlus.HideCollectionContent();
					}
				}
			}
		}
	}

	private void ShowPageContent(SlotPage page)
	{
		int pageBaseSlotIndex = GetPageBaseSlotIndex(page);
		int num = 0;
		Transform transform = null;
		KeywordSlotPlus keywordSlotPlus = null;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			for (int j = 0; j < m_slotColumnCount; j++)
			{
				num = i * m_GridLayoutGroup.constraintCount + j + pageBaseSlotIndex;
				transform = m_ContainerRT.GetChild(num);
				if (!(transform == null))
				{
					keywordSlotPlus = transform.gameObject.GetComponent<KeywordSlotPlus>();
					if (!(keywordSlotPlus == null))
					{
						keywordSlotPlus.ShowCollectionContent();
					}
				}
			}
		}
	}

	public KeywordSlotPlus GetKeywordSlot_byData(Xls.CollKeyword data)
	{
		if (m_keywordDatas == null || data == null)
		{
			return null;
		}
		return GetKeywordSlot_byDataIndex(m_keywordDatas.IndexOf(data));
	}

	public KeywordSlotPlus GetKeywordSlot_byDataIndex(int dataIndex)
	{
		KeywordSlotPlus value = null;
		return (!m_dataIndexToSlotMap.TryGetValue(dataIndex, out value)) ? null : value;
	}

	public int GetDataIndexInKeywordSlot(KeywordSlotPlus keywordSlot)
	{
		SortedDictionary<int, KeywordSlotPlus>.Enumerator enumerator = m_dataIndexToSlotMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value == keywordSlot)
			{
				return enumerator.Current.Key;
			}
		}
		return -1;
	}

	private void OnNotice_SlotClicked(object sender, object args)
	{
		if (sender != null && sender is KeywordSlotPlus)
		{
			KeywordSlotPlus keywordSlot = sender as KeywordSlotPlus;
			SetOnCursorKeywordData(GetDataIndexInKeywordSlot(keywordSlot));
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
		}
	}

	public void SetOnCursorKeywordData(int keywordDataIndex, bool isIgnoreSame = true)
	{
		if (m_keywordDatas == null || m_keywordDatas.Count <= 0)
		{
			return;
		}
		if (keywordDataIndex < 0 || keywordDataIndex >= m_keywordDatas.Count)
		{
			keywordDataIndex = -1;
		}
		if (keywordDataIndex == m_onCursorDataIndex && isIgnoreSame)
		{
			return;
		}
		EventArg_OnCursorChanged eventArg_OnCursorChanged = new EventArg_OnCursorChanged();
		eventArg_OnCursorChanged.m_prevOnCursorDataIndex = m_onCursorDataIndex;
		eventArg_OnCursorChanged.m_curOnCursorDataIndex = keywordDataIndex;
		if (m_onCursorDataIndex >= 0)
		{
			KeywordSlotPlus keywordSlot_byDataIndex = GetKeywordSlot_byDataIndex(m_onCursorDataIndex);
			if (keywordSlot_byDataIndex != null)
			{
				keywordSlot_byDataIndex.SetSelect_forCollectionContent(_isSelect: false);
			}
			eventArg_OnCursorChanged.m_prevOnCursorSlot = keywordSlot_byDataIndex;
			eventArg_OnCursorChanged.m_prevOnCursorData = m_keywordDatas[m_onCursorDataIndex];
		}
		if (keywordDataIndex >= 0)
		{
			KeywordSlotPlus keywordSlot_byDataIndex2 = GetKeywordSlot_byDataIndex(keywordDataIndex);
			if (keywordSlot_byDataIndex2 != null)
			{
				keywordSlot_byDataIndex2.SetSelect_forCollectionContent(_isSelect: true);
			}
			eventArg_OnCursorChanged.m_curOnCursorSlot = keywordSlot_byDataIndex2;
			eventArg_OnCursorChanged.m_curOnCursorData = m_keywordDatas[keywordDataIndex];
		}
		m_onCursorDataIndex = keywordDataIndex;
		if (m_OnCursorChanged != null)
		{
			m_OnCursorChanged(this, eventArg_OnCursorChanged);
		}
	}

	private int GetDataIndex_NearHorizontal(int baseIndex, bool isRightSide)
	{
		int num = ((m_keywordDatas != null) ? m_keywordDatas.Count : 0);
		if (baseIndex < 0 || baseIndex >= num)
		{
			return -1;
		}
		int num2 = baseIndex / m_slotCountPerPage;
		int num3 = baseIndex - num2 * m_slotCountPerPage;
		int num4 = num3 % m_slotColumnCount;
		int num5 = num3 / m_slotColumnCount;
		int num6 = num2;
		int num7 = num4;
		int num8 = num5;
		int num9 = -1;
		bool flag = false;
		do
		{
			num7 += (isRightSide ? 1 : (-1));
			if (num7 >= m_slotColumnCount)
			{
				if (!flag)
				{
					num6++;
					if (num6 > m_pageCount)
					{
						num6 = 0;
					}
					num7 = 0;
					flag = true;
				}
				else if (num8 > 0)
				{
					num8--;
					num7 = 0;
				}
				else
				{
					num8 = num5;
					num7 = m_slotColumnCount - 1;
					flag = false;
				}
			}
			else if (num7 < 0)
			{
				if (!flag)
				{
					num6--;
					if (num6 < 0)
					{
						num6 = m_pageCount - 1;
					}
					num7 = m_slotColumnCount - 1;
					flag = true;
				}
				else if (num8 > 0)
				{
					num8--;
					num7 = m_slotColumnCount - 1;
				}
				else
				{
					num8 = num5;
					num7 = 0;
					flag = false;
				}
			}
			num9 = num6 * m_slotCountPerPage + num8 * m_slotColumnCount + num7;
		}
		while (num9 < 0 || num9 >= num);
		return num9;
	}

	private int GetDataIndex_NearVertical(int baseIndex, bool isDownSide)
	{
		int num = ((m_keywordDatas != null) ? m_keywordDatas.Count : 0);
		if (baseIndex < 0 || baseIndex >= num)
		{
			return -1;
		}
		int num2 = baseIndex / m_slotCountPerPage;
		int num3 = baseIndex - num2 * m_slotCountPerPage;
		int num4 = num3 % m_slotColumnCount;
		int num5 = num3 / m_slotColumnCount;
		int num6 = num2;
		int num7 = num4;
		int num8 = num5;
		int num9 = -1;
		do
		{
			num8 += (isDownSide ? 1 : (-1));
			if (num8 >= m_slotRawCount)
			{
				num8 = 0;
			}
			else if (num8 < 0)
			{
				num8 = m_slotRawCount - 1;
			}
			num9 = num6 * m_slotCountPerPage + num8 * m_slotColumnCount + num7;
		}
		while (num9 < 0 || num9 >= num);
		return num9;
	}

	private bool ChangeOnCursorContent(int deltaX, int deltaY)
	{
		if (m_pageCount <= 0 || m_curPageIndex < 0)
		{
			return false;
		}
		if (deltaX == 0 && deltaY == 0)
		{
			return false;
		}
		if (m_onCursorDataIndex < 0)
		{
			SetOnCursorKeywordData(0);
			return false;
		}
		int num = 0;
		if (deltaX != 0)
		{
			num = GetDataIndex_NearHorizontal(m_onCursorDataIndex, deltaX > 0);
		}
		else if (deltaY != 0)
		{
			num = GetDataIndex_NearVertical(m_onCursorDataIndex, deltaY > 0);
		}
		if (m_onCursorDataIndex == num)
		{
			return false;
		}
		SetOnCursorKeywordData(num);
		int num2 = num / m_slotCountPerPage;
		if (num2 != m_curPageIndex)
		{
			StartPageScroll(num2, (deltaX > 0) ? SlotPage.Next : SlotPage.Prev);
		}
		return true;
	}

	public void ProcKeyInput()
	{
		if (m_pageCount > 1)
		{
			SlotPage slotPage = SlotPage.Current;
			float axisValue = GamePadInput.GetAxisValue(PadInput.GameInput.RStickX);
			if (!GameGlobalUtil.IsAlmostSame(axisValue, 0f))
			{
				slotPage = ((axisValue > 0f) ? SlotPage.Next : SlotPage.Prev);
			}
			else
			{
				float y = Input.mouseScrollDelta.y;
				if (Input.GetKeyDown(KeyCode.PageUp) || y > 0f)
				{
					slotPage = SlotPage.Prev;
				}
				else if (Input.GetKeyDown(KeyCode.PageDown) || y < 0f)
				{
					slotPage = SlotPage.Next;
				}
			}
			if (slotPage == SlotPage.Prev || slotPage == SlotPage.Next)
			{
				StartPageScroll(slotPage, isOnCursorAtFirstSlot: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Scroll_Page");
				}
				if (m_KeyInputedScrollPage != null)
				{
					m_KeyInputedScrollPage(this, null);
				}
				return;
			}
		}
		float fAxisX = 0f;
		float fAxisY = 0f;
		if (!GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
		{
			return;
		}
		int deltaX = 0;
		int deltaY = 0;
		if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
		{
			if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
			{
				deltaX = -1;
				m_keyPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
			{
				deltaX = 1;
				m_keyPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
			{
				if (IsInputRepeatTimeCheck())
				{
					deltaX = -1;
				}
			}
			else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsInputRepeatTimeCheck())
			{
				deltaX = 1;
			}
		}
		else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
		{
			deltaY = -1;
			m_keyPushingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
		{
			deltaY = 1;
			m_keyPushingTime = 0f;
		}
		else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
		{
			if (IsInputRepeatTimeCheck())
			{
				deltaY = -1;
			}
		}
		else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsInputRepeatTimeCheck())
		{
			deltaY = 1;
		}
		if (ChangeOnCursorContent(deltaX, deltaY))
		{
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
			if (m_KeyInputedMoveCursor != null)
			{
				m_KeyInputedMoveCursor(this, null);
			}
		}
	}

	private bool IsInputRepeatTimeCheck()
	{
		m_keyPushingTime += Time.deltaTime;
		if (m_keyPushingTime >= m_keyInputRepeatTimeBound)
		{
			m_keyPushingTime = 0f;
			return true;
		}
		return false;
	}

	public void StartPageScroll(SlotPage scrollDir, bool isOnCursorAtFirstSlot = false)
	{
		if (m_pageCount <= 1)
		{
			return;
		}
		int num = m_curPageIndex;
		switch (scrollDir)
		{
		case SlotPage.Next:
			num++;
			if (num >= m_pageCount)
			{
				num = 0;
			}
			break;
		case SlotPage.Prev:
			num--;
			if (num < 0)
			{
				num = m_pageCount - 1;
			}
			break;
		}
		if (num != m_curPageIndex)
		{
			StartPageScroll(num, scrollDir, isOnCursorAtFirstSlot);
		}
	}

	public void StartPageScroll(int targetPageIndex, SlotPage scrollDir, bool isOnCursorAtFirstSlot = false)
	{
		if (m_pageCount > 1 && targetPageIndex >= 0 && targetPageIndex < m_pageCount && m_targetPageIndex != m_curPageIndex && scrollDir != SlotPage.Current)
		{
			m_pageScrollDir = scrollDir;
			m_targetPageIndex = targetPageIndex;
			int num = 0;
			num = ((scrollDir != SlotPage.Next) ? ((m_curPageIndex <= m_targetPageIndex) ? (m_pageCount - (m_targetPageIndex - m_curPageIndex)) : (m_curPageIndex - m_targetPageIndex)) : ((m_targetPageIndex <= m_curPageIndex) ? (m_pageCount - (m_curPageIndex - m_targetPageIndex)) : (m_targetPageIndex - m_curPageIndex)));
			SetScrollingPageInfos(m_curPageIndex);
			m_remainScrollDistance = (float)num * m_pageWidth;
			m_pageScrollSpeed = m_remainScrollDistance / 0.25f;
			m_checkScrolledDistance = 0f;
			m_spareScrollDistance = 0f;
			m_roopedColumnCount = 0;
			m_isOnCursorAtFirstSlot = isOnCursorAtFirstSlot;
		}
	}

	private void SetScrollingPageInfos(int basePageIndex)
	{
		if (m_pageScrollDir == SlotPage.Next)
		{
			m_reflashSlotColumnIdx = 0;
			m_curScrollingPageIndex = basePageIndex + 1;
			if (m_curScrollingPageIndex >= m_pageCount)
			{
				m_curScrollingPageIndex = 0;
				ShowPageContent(SlotPage.Next);
			}
		}
		else if (m_pageScrollDir == SlotPage.Prev)
		{
			m_reflashSlotColumnIdx = m_slotColumnCount - 1;
			m_curScrollingPageIndex = basePageIndex - 1;
			if (m_curScrollingPageIndex < 0)
			{
				m_curScrollingPageIndex = m_pageCount - 1;
				ShowPageContent(SlotPage.Prev);
			}
		}
	}

	public bool IsPageScrolling()
	{
		return m_pageScrollDir != SlotPage.Current;
	}

	public void UpdatePageScroll()
	{
		if (!IsPageScrolling())
		{
			return;
		}
		float num = 0f;
		if (!GameGlobalUtil.IsAlmostSame(m_remainScrollDistance, 0f))
		{
			num = m_pageScrollSpeed * Time.deltaTime;
			num = Mathf.Min(num, m_remainScrollDistance);
			m_remainScrollDistance -= num;
			if (!(m_remainScrollDistance <= 0f))
			{
				int num2 = (int)(num + m_spareScrollDistance);
				m_spareScrollDistance = num + m_spareScrollDistance - (float)num2;
				m_GridLayoutGroup.padding.left -= ((m_pageScrollDir != SlotPage.Next) ? (-num2) : num2);
				m_checkScrolledDistance += num2;
				while (m_checkScrolledDistance >= m_slotWidth)
				{
					if (m_pageScrollDir == SlotPage.Next)
					{
						RoopKeywordSlot_ToRight();
					}
					else
					{
						RoopKeywordSlot_ToLeft();
					}
					m_checkScrolledDistance -= m_slotWidth;
					m_roopedColumnCount++;
					if (m_roopedColumnCount >= m_slotColumnCount)
					{
						m_roopedColumnCount = 0;
						SetScrollingPageInfos(m_curScrollingPageIndex);
					}
				}
				m_GridLayoutGroup.enabled = false;
				m_GridLayoutGroup.enabled = true;
				return;
			}
		}
		m_checkScrolledDistance += num + m_spareScrollDistance;
		while (m_checkScrolledDistance >= m_slotWidth || GameGlobalUtil.IsAlmostSame(m_checkScrolledDistance, m_slotWidth))
		{
			if (m_pageScrollDir == SlotPage.Next)
			{
				RoopKeywordSlot_ToRight();
			}
			else
			{
				RoopKeywordSlot_ToLeft();
			}
			m_checkScrolledDistance -= m_slotWidth;
			m_roopedColumnCount++;
			if (m_roopedColumnCount >= m_slotColumnCount)
			{
				m_roopedColumnCount = 0;
				SetScrollingPageInfos(m_curScrollingPageIndex);
			}
		}
		m_checkScrolledDistance = 0f;
		m_pageScrollDir = SlotPage.Current;
		m_pageScrollSpeed = 0f;
		SetCurrentPage(m_targetPageIndex);
		m_targetPageIndex = -1;
		m_GridLayoutGroup.padding.left = -(int)m_pageWidth;
		m_GridLayoutGroup.enabled = false;
		m_GridLayoutGroup.enabled = true;
		int childCount = m_ContainerRT.childCount;
		string stateName = GameDefine.UIAnimationState.idle.ToString();
		Transform transform = null;
		for (int i = 0; i < childCount; i++)
		{
			transform = m_ContainerRT.GetChild(i);
			Animator[] componentsInChildren = transform.gameObject.GetComponentsInChildren<Animator>();
			Animator[] array = componentsInChildren;
			foreach (Animator animator in array)
			{
				animator.Rebind();
				animator.Play(stateName);
			}
		}
		if (m_isOnCursorAtFirstSlot)
		{
			SetOnCursorKeywordData(m_curPageIndex * m_slotCountPerPage, isIgnoreSame: false);
			m_isOnCursorAtFirstSlot = false;
		}
		else
		{
			SetOnCursorKeywordData(m_onCursorDataIndex, isIgnoreSame: false);
		}
	}

	private void RoopKeywordSlot_ToLeft()
	{
		int constraintCount = m_GridLayoutGroup.constraintCount;
		int num = 0;
		Transform transform = null;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			num = constraintCount * i + (constraintCount - 1);
			transform = m_ContainerRT.GetChild(num);
			if (transform == null)
			{
				break;
			}
			transform.SetSiblingIndex(constraintCount * i);
		}
		m_GridLayoutGroup.padding.left -= (int)m_slotWidth;
		if (m_reflashSlotColumnIdx >= 0)
		{
			int num2 = m_curScrollingPageIndex - 1;
			if (num2 < 0)
			{
				num2 = m_pageCount - 1;
			}
			int baseDataIndex = num2 * m_slotCountPerPage + m_reflashSlotColumnIdx;
			SetSlotColumnContent(0, baseDataIndex, enableClick: false);
			m_reflashSlotColumnIdx--;
			if (num2 == m_pageCount - 1)
			{
				HideColumnContent(0);
			}
		}
	}

	private void RoopKeywordSlot_ToRight()
	{
		int constraintCount = m_GridLayoutGroup.constraintCount;
		int num = 0;
		Transform transform = null;
		for (int i = 0; i < m_slotRawCount; i++)
		{
			num = constraintCount * i;
			transform = m_ContainerRT.GetChild(num);
			if (transform == null)
			{
				break;
			}
			transform.SetSiblingIndex(num + (constraintCount - 1));
		}
		m_GridLayoutGroup.padding.left += (int)m_slotWidth;
		if (m_reflashSlotColumnIdx < m_slotColumnCount)
		{
			int num2 = m_curScrollingPageIndex + 1;
			if (num2 >= m_pageCount)
			{
				num2 = 0;
			}
			int baseDataIndex = num2 * m_slotCountPerPage + m_reflashSlotColumnIdx;
			SetSlotColumnContent(constraintCount - 1, baseDataIndex, enableClick: false);
			m_reflashSlotColumnIdx++;
			if (num2 == 0)
			{
				HideColumnContent(constraintCount - 1);
			}
		}
	}
}
