using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class NoticeUIManager : MonoBehaviour
{
	public enum NoticeType
	{
		Normal,
		Mental,
		Profile,
		ToDo,
		Record,
		Config_Sound,
		Config_Image,
		Sound,
		Image
	}

	private class SlotInfo
	{
		public GameObject m_NoticeItemObj;

		public float m_fTimer;

		private int m_iIndex;

		public int slotIndex => m_iIndex;

		public SlotInfo(int idx)
		{
			m_iIndex = idx;
		}
	}

	private class BackupData
	{
		public NoticeType m_noticeType;

		public string m_text = string.Empty;
	}

	public GameObject m_RootObject;

	public GameObject m_OriginNoticeItem;

	public int m_MaxSlotCount = 5;

	public float m_BaseYPos;

	public float m_ItemGab;

	public float m_ActiveTime = 3f;

	[Header("Icon Images")]
	public Sprite m_IconNormal;

	public Sprite m_IconMental;

	public Sprite m_IconProfile;

	public Sprite m_IconToDo;

	public Sprite m_IconRecord;

	public Sprite m_IconConfig;

	public Sprite m_IconPic;

	private SlotInfo[] m_slotInfos;

	private List<BackupData> m_backupDatas = new List<BackupData>();

	private RectTransform m_rtRootObject;

	private const string c_XlsDataName_ChangedMentalNone = "NOTICE_CHANGE_MENTAL_NONE";

	private const string c_XlsDataName_MentalUpSmall = "NOTICE_MENTAL_SMALL_UP";

	private const string c_XlsDataName_MentalUpMidium = "NOTICE_MENTAL_MIDIUM_UP";

	private const string c_XlsDataName_MentalUpLarge = "NOTICE_MENTAL_LARGE_UP";

	private const string c_XlsDataName_MentalDownSmall = "NOTICE_MENTAL_SMALL_DOWN";

	private const string c_XlsDataName_MentalDownMidium = "NOTICE_MENTAL_MIDIUM_DOWN";

	private const string c_XlsDataName_MentalDownLarge = "NOTICE_MENTAL_LARGE_DOWN";

	private const string c_XlsDataName_ChangedRelationNone1 = "NOTICE_CHANGE_RELATION_NONE1";

	private const string c_XlsDataName_ChangedRelationNone2 = "NOTICE_CHANGE_RELATION_NONE2";

	private const string c_XlsDataName_RelationUpSmall1 = "NOTICE_RELATION_SMALL_UP1";

	private const string c_XlsDataName_RelationUpSmall2 = "NOTICE_RELATION_SMALL_UP2";

	private const string c_XlsDataName_RelationUpMidium1 = "NOTICE_RELATION_MIDIUM_UP1";

	private const string c_XlsDataName_RelationUpMidium2 = "NOTICE_RELATION_MIDIUM_UP2";

	private const string c_XlsDataName_RelationUpLarge1 = "NOTICE_RELATION_LARGE_UP1";

	private const string c_XlsDataName_RelationUpLarge2 = "NOTICE_RELATION_LARGE_UP2";

	private const string c_XlsDataName_RelationDownSmall1 = "NOTICE_RELATION_SMALL_DOWN1";

	private const string c_XlsDataName_RelationDownSmall2 = "NOTICE_RELATION_SMALL_DOWN2";

	private const string c_XlsDataName_RelationDownMidium1 = "NOTICE_RELATION_MIDIUM_DOWN1";

	private const string c_XlsDataName_RelationDownMidium2 = "NOTICE_RELATION_MIDIUM_DOWN2";

	private const string c_XlsDataName_RelationDownLarge1 = "NOTICE_RELATION_LARGE_DOWN1";

	private const string c_XlsDataName_RelationDownLarge2 = "NOTICE_RELATION_LARGE_DOWN2";

	private const string c_XlsDataName_MentalDeltaBound_Small = "MENTAL_GAGE_DELTA_SMALL_BOUND";

	private const string c_XlsDataName_MentalDeltaBound_Medium = "MENTAL_GAGE_DELTA_MEDIUM_BOUND";

	private const string c_XlsDataName_RelationDeltaBound_Small = "RELATION_DELTA_SMALL_BOUND";

	private const string c_XlsDataName_RelationDeltaBound_Medium = "RELATION_DELTA_MIDIUM_BOUND";

	private float m_fMentalDeltaBoundSmall = 4f;

	private float m_fMentalDeltaBoundMidium = 9f;

	private float m_fRelationDeltaBoundSmall = 3f;

	private float m_fRelationDeltaBoundMidium = 6f;

	private const string c_XlsDataName_MainActorName = "CHR_han";

	private Xls.TextData m_xlsDataMainActorName;

	private readonly string[] c_CharKeys_InXlsType2 = new string[1] { "오인하" };

	private AudioManager m_AudioManager;

	private static NoticeUIManager s_Instance;

	public static NoticeUIManager instance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
	}

	private void OnDestory()
	{
		m_slotInfos = null;
		m_rtRootObject = null;
		m_AudioManager = null;
		s_Instance = null;
	}

	private void Start()
	{
		m_rtRootObject = m_RootObject.GetComponent<RectTransform>();
		m_OriginNoticeItem.SetActive(value: false);
		m_slotInfos = new SlotInfo[m_MaxSlotCount];
		for (int i = 0; i < m_MaxSlotCount; i++)
		{
			m_slotInfos[i] = new SlotInfo(i);
		}
		Xls.ProgramDefineStr programDefineStr = null;
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_DELTA_SMALL_BOUND");
		if (programDefineStr != null)
		{
			m_fMentalDeltaBoundSmall = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("MENTAL_GAGE_DELTA_MEDIUM_BOUND");
		if (programDefineStr != null)
		{
			m_fMentalDeltaBoundMidium = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("RELATION_DELTA_SMALL_BOUND");
		if (programDefineStr != null)
		{
			m_fRelationDeltaBoundSmall = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("RELATION_DELTA_MIDIUM_BOUND");
		if (programDefineStr != null)
		{
			m_fRelationDeltaBoundMidium = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		m_xlsDataMainActorName = Xls.TextData.GetData_byKey("CHR_han");
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
	}

	private void Update()
	{
		if (m_slotInfos == null || m_slotInfos.Length <= 0)
		{
			return;
		}
		int num = m_slotInfos.Length;
		SlotInfo slotInfo = null;
		float deltaTime = Time.deltaTime;
		string stateName = GameDefine.UIAnimationState.disappear.ToString();
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			slotInfo = m_slotInfos[i];
			if (slotInfo.m_NoticeItemObj == null)
			{
				flag = true;
			}
			else if (slotInfo.m_fTimer <= 0f)
			{
				Animator component = slotInfo.m_NoticeItemObj.GetComponent<Animator>();
				AnimatorStateInfo currentAnimatorStateInfo = component.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(stateName))
				{
					if (currentAnimatorStateInfo.normalizedTime >= 0.99f)
					{
						UnityEngine.Object.Destroy(slotInfo.m_NoticeItemObj);
						slotInfo.m_NoticeItemObj = null;
						flag = true;
					}
				}
				else
				{
					component.Play(stateName);
				}
			}
			else
			{
				slotInfo.m_fTimer -= deltaTime;
			}
		}
		if (flag && m_backupDatas.Count > 0)
		{
			BackupData backupData = m_backupDatas[0];
			if (ActiveNotice(backupData.m_noticeType, backupData.m_text, isEnableBackup: false))
			{
				m_backupDatas.RemoveAt(0);
			}
		}
	}

	private SlotInfo GetEmptySlot()
	{
		if (m_slotInfos == null || m_slotInfos.Length <= 0)
		{
			return null;
		}
		int num = m_slotInfos.Length;
		SlotInfo slotInfo = null;
		for (int i = 0; i < num; i++)
		{
			slotInfo = m_slotInfos[i];
			if (slotInfo.m_NoticeItemObj == null)
			{
				return slotInfo;
			}
		}
		return null;
	}

	public bool ActiveNotice(NoticeType noticeType, string text, bool isEnableBackup = true)
	{
		SlotInfo emptySlot = GetEmptySlot();
		if (emptySlot == null)
		{
			if (!isEnableBackup)
			{
				return false;
			}
			BackupData backupData = new BackupData();
			backupData.m_noticeType = noticeType;
			backupData.m_text = text;
			m_backupDatas.Add(backupData);
			return true;
		}
		emptySlot.m_NoticeItemObj = UnityEngine.Object.Instantiate(m_OriginNoticeItem);
		emptySlot.m_fTimer = m_ActiveTime;
		NoticeUI component = emptySlot.m_NoticeItemObj.GetComponent<NoticeUI>();
		Sprite iconSprite = null;
		switch (noticeType)
		{
		case NoticeType.Normal:
			iconSprite = m_IconNormal;
			break;
		case NoticeType.Mental:
			iconSprite = m_IconMental;
			break;
		case NoticeType.Profile:
			iconSprite = m_IconProfile;
			break;
		case NoticeType.ToDo:
			iconSprite = m_IconToDo;
			break;
		case NoticeType.Record:
			iconSprite = m_IconRecord;
			break;
		case NoticeType.Config_Sound:
			iconSprite = m_IconConfig;
			break;
		case NoticeType.Config_Image:
			iconSprite = m_IconConfig;
			break;
		case NoticeType.Sound:
			iconSprite = m_IconRecord;
			break;
		case NoticeType.Image:
			iconSprite = m_IconPic;
			break;
		}
		component.SetContent(iconSprite, text);
		RectTransform component2 = emptySlot.m_NoticeItemObj.GetComponent<RectTransform>();
		component2.SetParent(m_rtRootObject, worldPositionStays: true);
		component2.anchoredPosition = new Vector2(0f, m_BaseYPos - m_ItemGab * (float)emptySlot.slotIndex);
		component2.localRotation = Quaternion.identity;
		component2.localScale = new Vector3(1f, 1f, 1f);
		emptySlot.m_NoticeItemObj.SetActive(value: true);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Pop_Line");
		}
		return true;
	}

	public void ActiveNotice_ChangedMental(float fDeltaMental)
	{
		string arg = ((m_xlsDataMainActorName == null) ? "Main Actor" : m_xlsDataMainActorName.m_strTxt);
		string text = null;
		if (GameGlobalUtil.IsAlmostSame(fDeltaMental, 0f))
		{
			text = "NOTICE_CHANGE_MENTAL_NONE";
		}
		else if (fDeltaMental > 0f)
		{
			text = ((fDeltaMental > m_fMentalDeltaBoundMidium) ? "NOTICE_MENTAL_LARGE_UP" : ((!(fDeltaMental <= m_fMentalDeltaBoundSmall)) ? "NOTICE_MENTAL_MIDIUM_UP" : "NOTICE_MENTAL_SMALL_UP"));
		}
		else
		{
			float num = Mathf.Abs(fDeltaMental);
			text = ((num > m_fMentalDeltaBoundMidium) ? "NOTICE_MENTAL_LARGE_DOWN" : ((!(num <= m_fMentalDeltaBoundSmall)) ? "NOTICE_MENTAL_MIDIUM_DOWN" : "NOTICE_MENTAL_SMALL_DOWN"));
		}
		Xls.ProgramText data_byKey = Xls.ProgramText.GetData_byKey(text);
		if (data_byKey != null)
		{
			string text2 = string.Format(data_byKey.m_strTxt, arg);
			ActiveNotice(NoticeType.Mental, text2);
		}
	}

	public void ActiveNotice_ChangeRelation(string strTargetCharKey, float fDeltaRelation)
	{
		bool flag = Array.IndexOf(c_CharKeys_InXlsType2, strTargetCharKey) >= 0;
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strTargetCharKey);
		if (data_byKey == null)
		{
			return;
		}
		Xls.TextData data_byKey2 = Xls.TextData.GetData_byKey(data_byKey.m_strNameKey);
		if (data_byKey2 != null)
		{
			string text = null;
			if (GameGlobalUtil.IsAlmostSame(fDeltaRelation, 0f))
			{
				text = ((!flag) ? "NOTICE_CHANGE_RELATION_NONE1" : "NOTICE_CHANGE_RELATION_NONE2");
			}
			else if (fDeltaRelation > 0f)
			{
				text = ((fDeltaRelation > m_fRelationDeltaBoundMidium) ? ((!flag) ? "NOTICE_RELATION_LARGE_UP1" : "NOTICE_RELATION_LARGE_UP2") : ((!(fDeltaRelation <= m_fRelationDeltaBoundSmall)) ? ((!flag) ? "NOTICE_RELATION_MIDIUM_UP1" : "NOTICE_RELATION_MIDIUM_UP2") : ((!flag) ? "NOTICE_RELATION_SMALL_UP1" : "NOTICE_RELATION_SMALL_UP2")));
			}
			else
			{
				float num = Mathf.Abs(fDeltaRelation);
				text = ((num > m_fRelationDeltaBoundMidium) ? ((!flag) ? "NOTICE_RELATION_LARGE_DOWN1" : "NOTICE_RELATION_LARGE_DOWN2") : ((!(num <= m_fRelationDeltaBoundSmall)) ? ((!flag) ? "NOTICE_RELATION_MIDIUM_DOWN1" : "NOTICE_RELATION_MIDIUM_DOWN2") : ((!flag) ? "NOTICE_RELATION_SMALL_DOWN1" : "NOTICE_RELATION_SMALL_DOWN2")));
			}
			Xls.ProgramText data_byKey3 = Xls.ProgramText.GetData_byKey(text);
			if (data_byKey3 != null)
			{
				string text2 = string.Format(data_byKey3.m_strTxt, data_byKey2.m_strTxt);
				ActiveNotice(NoticeType.Mental, text2);
			}
		}
	}

	public static bool ActiveNotice_S_SetKey(NoticeType noticeType, string strKey)
	{
		int iIdx = -1;
		switch (noticeType)
		{
		case NoticeType.Profile:
		{
			Xls.Profiles data_byKey2 = Xls.Profiles.GetData_byKey(strKey);
			if (data_byKey2 != null)
			{
				iIdx = data_byKey2.m_iIdx;
			}
			break;
		}
		case NoticeType.Sound:
		{
			Xls.CollSounds data_byKey3 = Xls.CollSounds.GetData_byKey(strKey);
			if (data_byKey3 != null)
			{
				iIdx = data_byKey3.m_iIdx;
			}
			break;
		}
		case NoticeType.Image:
		{
			Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strKey);
			if (data_byKey != null)
			{
				iIdx = data_byKey.m_iIdx;
			}
			break;
		}
		}
		return ActiveNotice_S_SetKey(noticeType, iIdx);
	}

	public static bool ActiveNotice_S_SetKey(NoticeType noticeType, int iIdx)
	{
		string text = null;
		switch (noticeType)
		{
		case NoticeType.Profile:
		{
			Xls.Profiles data_byIdx = Xls.Profiles.GetData_byIdx(iIdx);
			if (data_byIdx == null)
			{
				return false;
			}
			Xls.TextListData data_byKey3 = Xls.TextListData.GetData_byKey(data_byIdx.m_strName);
			if (data_byKey3 == null)
			{
				return false;
			}
			string xlsProgramText = GameGlobalUtil.GetXlsProgramText("NOTICE_S_PROFILE");
			Xls.TextData data_byKey4 = Xls.TextData.GetData_byKey(data_byIdx.m_strChrTxtKey);
			if (data_byKey4 == null)
			{
				return false;
			}
			text = string.Format(xlsProgramText, data_byKey4.m_strTxt, data_byKey3.m_strTitle);
			break;
		}
		case NoticeType.Sound:
		{
			Xls.CollSounds data_byIdx2 = Xls.CollSounds.GetData_byIdx(iIdx);
			if (data_byIdx2 == null)
			{
				return false;
			}
			Xls.TextListData data_byKey6 = Xls.TextListData.GetData_byKey(data_byIdx2.m_strIDtext);
			if (data_byKey6 == null)
			{
				return false;
			}
			text = string.Format(GameGlobalUtil.GetXlsProgramText("NOTICE_S_SOUND"), data_byKey6.m_strTitle);
			break;
		}
		case NoticeType.Image:
		{
			Xls.CollImages data_bySwitchIdx3 = Xls.CollImages.GetData_bySwitchIdx(iIdx);
			if (data_bySwitchIdx3 == null)
			{
				return false;
			}
			Xls.TextListData data_byKey5 = Xls.TextListData.GetData_byKey(data_bySwitchIdx3.m_strIDtext);
			if (data_byKey5 == null)
			{
				return false;
			}
			text = string.Format(GameGlobalUtil.GetXlsProgramText("NOTICE_S_IMAGE"), data_byKey5.m_strTitle);
			break;
		}
		case NoticeType.Config_Sound:
		{
			Xls.CollSounds data_bySwitchIdx2 = Xls.CollSounds.GetData_bySwitchIdx(iIdx);
			if (data_bySwitchIdx2 == null)
			{
				return false;
			}
			Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_bySwitchIdx2.m_strIDtext);
			if (data_byKey2 == null)
			{
				return false;
			}
			text = string.Format(GameGlobalUtil.GetXlsProgramText("NOTICE_S_WATCH_RING"), data_byKey2.m_strTitle);
			break;
		}
		case NoticeType.Config_Image:
		{
			Xls.CollImages data_bySwitchIdx4 = Xls.CollImages.GetData_bySwitchIdx(iIdx);
			if (data_bySwitchIdx4 == null)
			{
				return false;
			}
			Xls.TextListData data_byKey7 = Xls.TextListData.GetData_byKey(data_bySwitchIdx4.m_strIDtext);
			if (data_byKey7 == null)
			{
				return false;
			}
			text = string.Format(GameGlobalUtil.GetXlsProgramText("NOTICE_S_WATCH_IMG"), data_byKey7.m_strTitle);
			break;
		}
		case NoticeType.ToDo:
		{
			Xls.Trophys data_bySwitchIdx = Xls.Trophys.GetData_bySwitchIdx(iIdx);
			if (data_bySwitchIdx == null)
			{
				return false;
			}
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(data_bySwitchIdx.m_strName);
			if (data_byKey == null)
			{
				return false;
			}
			text = string.Format(GameGlobalUtil.GetXlsProgramText("NOTICE_S_TODO"), data_byKey.m_strTitle);
			break;
		}
		}
		return ActiveNotice_S(noticeType, text);
	}

	public static bool ActiveNotice_S(NoticeType noticeType, string text, bool isEnableBackup = true)
	{
		if (s_Instance == null)
		{
			return false;
		}
		return s_Instance.ActiveNotice(noticeType, text, isEnableBackup);
	}

	public static void ActiveNotice_ChangedMental_S(float fDeltaMental)
	{
		if (!(s_Instance == null))
		{
			s_Instance.ActiveNotice_ChangedMental(fDeltaMental);
		}
	}

	public static void ActiveNotice_ChangeRelation_S(string strTargetCharKey, float fDeltaRelation)
	{
		if (!(s_Instance == null))
		{
			s_Instance.ActiveNotice_ChangeRelation(strTargetCharKey, fDeltaRelation);
		}
	}
}
