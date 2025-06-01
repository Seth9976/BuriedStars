using System;
using System.Collections.Generic;
using System.Linq;
using GameEvent;
using Steamworks;
using UnityEngine;

namespace GameData;

[Serializable]
public class GameSwitch
{
	public enum eEventRunType
	{
		NONE,
		START_EVENT,
		ENABLE_OBJ,
		TOUCH_OBJ
	}

	public class VoteRank
	{
		public int m_iVoteCnt;

		public int m_iCharIdx;

		public int m_iRankNum;
	}

	private class CharPartySet
	{
		public string m_strCharKey;

		public byte m_byState;

		public int m_iIconShow;

		public float m_fPosX;

		public float m_fPosY;

		public int m_iSize;

		public string m_strMot;

		public int m_iDir;

		public int m_iOrderIdx;
	}

	public class SequenceInfo
	{
		public int m_iSeqIdx;

		public List<KeywordSet> m_clKeyword;
	}

	public enum eButType
	{
		OEnter,
		XEnter,
		Count
	}

	public enum eUIButType
	{
		DEF,
		KEYMOUSE,
		XBOX,
		PS4
	}

	public class KeywordSet
	{
		public int m_iKeywordIdx;

		public string m_strIDKeyword;
	}

	public enum eScreenMode
	{
		FullScreen,
		Window
	}

	public enum eResolution
	{
		R_1920x1080,
		R_1280x720
	}

	public enum eControllerType
	{
		Auto,
		SonyDualShock,
		XboxController
	}

	public enum eChrIconState
	{
		Zero,
		Low,
		Medium,
		High,
		Full
	}

	private static GameSwitch s_Instance = null;

	public MainLoadThing m_MainLoadThing;

	public static int SIZE_INT = 4;

	public static int SIZE_FLOAT = 4;

	private static int FLOAT_SDATA_SIZE = SIZE_FLOAT;

	private static int INT_SDATA_SIZE = SIZE_INT;

	private static int ETC_SDATA_SIZE = ConstGameSwitch.ARRLEN_SWITCH_EVENT + ConstGameSwitch.ARRLEN_SWITCH_SELECT + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_GROUP + ConstGameSwitch.ARRLEN_BIT_SWITCH_REPLY_GROUP;

	private static int CHAR_SDATA_SIZE = 10 + SIZE_INT * 8 + 8 + 8;

	private static int CHAR_PARTY_SDATA_SIZE = 8 * (5 + SIZE_FLOAT + 40) + SIZE_INT + SIZE_INT;

	private static int POST_SDATA_SIZE = ConstGameSwitch.ARRLEN_2BIT_SWITCH_SNS_POST + ConstGameSwitch.COUNT_SWITCH_SNS_POST * SIZE_INT + ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST + ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST + ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST;

	private static int MESSENGER_SDATA_SIZE = SIZE_INT + ConstGameSwitch.COUNT_MESSENGER_ORDER * SIZE_INT;

	private static int KEYWORD_SDATA_SIZE = ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL + ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_SEQ + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR;

	private static int SEQ_PHASE_SDATA_SIZE = SIZE_INT + SIZE_INT + SIZE_INT + ConstGameSwitch.COUNT_SEQUENCE_LIST * SIZE_INT;

	public static int SIZE_VECTOR = 3 * SIZE_FLOAT;

	public static int SAVE_SIZE_CAMERA = SIZE_VECTOR * 3 + SIZE_FLOAT;

	public static int SAVE_SIZE_GAMEMAIN_DATA = 1;

	public static int COLL_SAVE_SWITCH_SIZE = ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE + ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND + (ConstGameSwitch.COUNT_MAX_TROPHY_CNT * SIZE_INT + ConstGameSwitch.ARRLEN_2BIT_SWITCH_TROPHY) + ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE + ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL + ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_USE + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE + ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE + ConstGameSwitch.ARRLEN_BIT_SWITCH_TUTORIAL + SIZE_FLOAT + 1;

	public static int GAME_SAVE_SWITCH_SIZE = FLOAT_SDATA_SIZE + INT_SDATA_SIZE + ConstGameSwitch.COUNT_MAX_CHAR_POS_X_ALL_CNT * 4 + ConstGameSwitch.COUNT_MAX_CHAR_POS_Y_ALL_CNT * 4 + ETC_SDATA_SIZE + CHAR_SDATA_SIZE + CHAR_PARTY_SDATA_SIZE + POST_SDATA_SIZE + MESSENGER_SDATA_SIZE + ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE + KEYWORD_SDATA_SIZE + SEQ_PHASE_SDATA_SIZE + 21 + ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE + ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND + ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX + SIZE_INT + SIZE_INT + 1 + SAVE_SIZE_CAMERA + SAVE_SIZE_GAMEMAIN_DATA;

	public eEventRunType m_eEventRunType;

	private string m_strEventObjName;

	private string m_strPMSendEvent;

	public bool m_isExistEventSaveObj;

	public eEventRunType m_eEventRunTypeForSave;

	public string m_strEventObjNameForSave;

	private string m_strPMSendEventForSave;

	public bool m_isShareVideoRecord;

	private byte[] m_byArrCollImage = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE];

	private byte[] m_byArrCollSound = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND];

	private int[] m_iArrTrophyCnt = new int[ConstGameSwitch.COUNT_MAX_TROPHY_CNT];

	private byte[] m_byArrTrophyNew = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_TROPHY];

	private byte[] m_byArrCollProfile = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE];

	private byte[] m_byArrCollKeyword = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL];

	private byte[] m_byArrCollUseKeyword = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_USE];

	private byte[] m_byArrCollCutKeyword = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE];

	private byte[] m_byArrCollSelectKeyword = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE];

	private byte[] m_byArrCollTutorial = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_TUTORIAL];

	private float m_fTotalGamePlayTime;

	private float m_fGameStartTime;

	private float m_fGamePlayTime;

	private bool m_isPause;

	private bool m_isEndingShow;

	private float m_fGameTime;

	private int m_iMental;

	private byte[] m_byArrKeywordGroup = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_GROUP];

	private byte[] m_byArrReplyGroup = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_REPLY_GROUP];

	private float[] m_fArrPosX = new float[ConstGameSwitch.COUNT_MAX_CHAR_POS_X_ALL_CNT];

	private float[] m_fArrPosY = new float[ConstGameSwitch.COUNT_MAX_CHAR_POS_Y_ALL_CNT];

	private byte[] m_byArrEvtSwitch = new byte[ConstGameSwitch.ARRLEN_SWITCH_EVENT];

	private byte[] m_byArrSelectSwitch = new byte[ConstGameSwitch.ARRLEN_SWITCH_SELECT];

	private string[] m_arrStrInvestObj = new string[ConstGameSwitch.COUNT_INVEST_OBJ];

	private string[] m_arrStrInvestName = new string[ConstGameSwitch.COUNT_INVEST_OBJ];

	private bool[] m_arrIsInvestShow = new bool[ConstGameSwitch.COUNT_INVEST_OBJ];

	private bool[] m_arrIsCheckInvest = new bool[ConstGameSwitch.COUNT_INVEST_OBJ];

	private List<KeyValuePair<int, float>> m_lstInvestXOrder = new List<KeyValuePair<int, float>>();

	private List<KeyValuePair<int, float>> m_lstInvestYOrder = new List<KeyValuePair<int, float>>();

	private int m_iInvestObjSelMax = -1;

	private int m_iInvestObjSelIdx = -1;

	private byte[] m_byArrChrAnonymous = new byte[2];

	private byte[] m_byArrChrRelation = new byte[8];

	private List<VoteRank> m_listVoteRank = new List<VoteRank>();

	private CharPartySet[] m_clArrCharPartySet = new CharPartySet[8];

	private List<CharPartySet> m_listPartySet = new List<CharPartySet>();

	private int m_iPartySelMax = -1;

	private int m_iPartySelIdx = -1;

	private string m_strPostTalkWindowId;

	private byte[] m_byArrPostState = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_SNS_POST];

	private int[] m_iPostRetweetCnt = new int[ConstGameSwitch.COUNT_SWITCH_SNS_POST];

	private byte[] m_byArrPostReply = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST];

	private byte[] m_byArrPostKeyword = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST];

	private byte[] m_byArrPostRetweet = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST];

	private bool m_isMessageSwitchSettable = true;

	private int[] m_iArrMessageOrder = new int[ConstGameSwitch.COUNT_MESSENGER_ORDER];

	private int m_iMessageGetCount;

	private byte[] m_byArrCharProfile = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE];

	private byte[] m_byArrKeywordAllState = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL];

	private byte[] m_byArrKeywordSeqState = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_SEQ];

	private byte[] m_byArrKeywordSeqUseKeywordBef = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR];

	private byte[] m_byArrKeywordSeqUseKeyword = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR];

	private List<SequenceInfo> m_lstSequenceInfo;

	private string m_strCurSequence;

	private int m_icursequence = -1;

	private int m_icurphase;

	private List<int> m_lstGetKeywordByOrder = new List<int>();

	private byte[] m_byArrChrRelationConti = new byte[21];

	private byte[] m_byArrImage = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE];

	private byte[] m_byArrSound = new byte[ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND];

	private byte[] m_byArrCutCharCall = new byte[ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX];

	private int m_iSWRingTone;

	private int m_iSWBackImage;

	private int m_iAutoTextIdx;

	private bool m_isExistLoadCamValue;

	private Vector3 m_vecLoadCamPos;

	private Vector3 m_vecLoadCamScale;

	private Vector3 m_vecLoadCamAngle;

	private float m_fLoadCamFOV;

	private eButType m_eOXType = eButType.XEnter;

	private eUIButType m_eUIButType;

	public int m_iTutoState = -1;

	private int m_iLowMental;

	private int m_iHighMental;

	private bool m_isRestart;

	private float m_fBreakRelation;

	private float m_fLowRelation;

	private float m_fMediumRelation;

	private float m_fHighRelation;

	private float m_fEvtOccurLowRelation;

	private bool m_isRunLowRelationEvt;

	private string m_strSaveRunKeyword;

	private string m_strRunKeyword;

	private string m_strRunKeywordCharKey;

	public bool m_isRunKeywordEvt;

	private bool m_isRunKeywordGameOver;

	private bool m_isIgnoreMentalRelationEvt;

	private int m_iCutKeywordCnt;

	private int m_iCutProfileCnt;

	private byte[] m_byCutStartChrRelation = new byte[8];

	private int m_iCutStartMental;

	private int m_iCurCutEffIdx = -1;

	private bool m_isGetAllKeywordPopByCut;

	private bool m_isCutConsiderOn;

	private int m_iMustGetCutKeywordCnt;

	private int m_iMustMaxCutKeywordCnt;

	private byte[] m_byArrGetCutKeyword = new byte[ConstGameSwitch.ARRLEN_BIT_SWITCH_MUST_GET_CUT_KEYWORD];

	private string[] m_strGetCutKeyword = new string[ConstGameSwitch.COUNT_MAX_MUST_GET_CUT_KEYWORD];

	private SystemLanguage m_eSystemLanguage;

	public const byte MAX_TYPING_SPEED = 4;

	private byte m_byTypingSpeed = 1;

	private bool m_isVSyncOn;

	private ConstGameSwitch.eVoiceLang m_eVoiceLang;

	public const byte MAX_AUTO_DELAY_STEP = 4;

	private int m_iAutoDelayStep = 1;

	private float m_fAutoDelayTime;

	private float UNIT_DELAY_TIME = 1f;

	private eScreenMode m_eScreenMode;

	private eResolution m_eResolution;

	private eControllerType m_eController;

	private string m_strUseKeywordChar;

	private string m_strUseKeywordKey;

	private byte m_byUseKeywordBefRelation;

	private int m_iAddedKeywordRelaton;

	private bool m_isSetKeywordEventRelation;

	private int m_iCurCallChar = -1;

	private string m_strRecordFaterPlayConti;

	private int m_iReLoadSlot = -1;

	private bool m_isCheckSNSKeywordBut;

	private bool m_isShowBrightSNSBut;

	private int m_iTrickObjAMPM;

	private int m_iTrickObjH;

	private int m_iTrickObjM;

	private int TRP_00050_MENTAL_VALUE;

	private const float MAX_PLAY_TIME = 3599999f;

	public bool m_isNeedLoadKeywordChar;

	private bool m_isLoadEventData;

	public void SetFirst()
	{
		SetMentalPointUI(m_iMental);
	}

	public static GameSwitch GetInstance()
	{
		if (s_Instance == null)
		{
			s_Instance = new GameSwitch();
			s_Instance.InitNew();
		}
		return s_Instance;
	}

	public static void ReleaseInstance()
	{
		if (s_Instance != null)
		{
			s_Instance.SetStrEventObjName(null);
			s_Instance.m_strPMSendEvent = null;
			s_Instance.m_strEventObjNameForSave = null;
			s_Instance.m_strPMSendEventForSave = null;
			s_Instance.m_strPostTalkWindowId = null;
			s_Instance.m_strSaveRunKeyword = null;
			s_Instance.m_strRunKeyword = null;
			s_Instance.m_strRunKeywordCharKey = null;
			s_Instance.m_strUseKeywordChar = null;
			s_Instance.m_strUseKeywordKey = null;
			s_Instance.m_strRecordFaterPlayConti = null;
			s_Instance.m_strCurSequence = null;
			s_Instance.InitInvestObj();
			s_Instance.Free();
			s_Instance.SetMainLoadThing(null);
			s_Instance = null;
		}
	}

	public void SetMainLoadThing(MainLoadThing mainLoadThing)
	{
		m_MainLoadThing = mainLoadThing;
	}

	public void SetCurSequence(int iValue, bool isAddPostPhase = true)
	{
		if (m_icursequence != iValue)
		{
			bool flag = m_icursequence != iValue;
			SetIsCheckSNSKeywordBut(isSet: true);
			m_strCurSequence = Xls.SequenceData.GetData_bySwitchIdx(iValue).m_strKey;
			SequenceInfo seqInfo = CreateFindSequenceInfo(iValue);
			InitKeywordSeqList(iValue, seqInfo);
			m_lstGetKeywordByOrder.Clear();
			if (flag)
			{
				SetCurPhase(0, isAddPostPhase);
			}
			m_icursequence = iValue;
		}
	}

	public int GetCurSequence()
	{
		return m_icursequence;
	}

	public void SetCurPhase(int iValue, bool isAddPostPhase = true)
	{
		if (m_icurphase != iValue)
		{
			SetIsCheckSNSKeywordBut(isSet: true);
		}
		m_icurphase = iValue;
		if (isAddPostPhase)
		{
			AddPostCurPhase(iValue);
		}
	}

	public int GetCurPhase()
	{
		return m_icurphase;
	}

	public void SetUIButType(eUIButType eType)
	{
		m_eUIButType = eType;
		MainLoadThing.instance.SetCursorPoint(eType == eUIButType.KEYMOUSE);
	}

	public eUIButType GetUIButType()
	{
		return m_eUIButType;
	}

	public void SetOXType(eButType eOXType)
	{
		m_eOXType = eOXType;
		GamePadInput.SetCircleButtonSubmit(m_eOXType == eButType.OEnter);
	}

	public int GetOXType()
	{
		return (int)m_eOXType;
	}

	public void SaveCameraData(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		if (iBefVer != -1 && m_isExistLoadCamValue)
		{
			BitCalc.Vector3ToByteNCO(m_vecLoadCamPos, bySaveBuf, ref iOffset);
			BitCalc.Vector3ToByteNCO(m_vecLoadCamScale, bySaveBuf, ref iOffset);
			BitCalc.Vector3ToByteNCO(m_vecLoadCamAngle, bySaveBuf, ref iOffset);
			BitCalc.FloatToByteNCO(m_fLoadCamFOV, bySaveBuf, ref iOffset);
		}
		else
		{
			Transform transform = Camera.main.transform;
			BitCalc.Vector3ToByteNCO(transform.position, bySaveBuf, ref iOffset);
			BitCalc.Vector3ToByteNCO(transform.localScale, bySaveBuf, ref iOffset);
			BitCalc.Vector3ToByteNCO(transform.eulerAngles, bySaveBuf, ref iOffset);
			BitCalc.FloatToByteNCO(Camera.main.fieldOfView, bySaveBuf, ref iOffset);
		}
	}

	public void LoadCameraData(byte[] byLoadBuf, ref int iOffset)
	{
		m_isExistLoadCamValue = true;
		Vector3 vecTemp = new Vector3(0f, 0f, 0f);
		BitCalc.ByteToVector3NCO(ref vecTemp, byLoadBuf, ref iOffset);
		m_vecLoadCamPos = vecTemp;
		BitCalc.ByteToVector3NCO(ref vecTemp, byLoadBuf, ref iOffset);
		m_vecLoadCamScale = vecTemp;
		BitCalc.ByteToVector3NCO(ref vecTemp, byLoadBuf, ref iOffset);
		m_vecLoadCamAngle = vecTemp;
		m_fLoadCamFOV = BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset);
	}

	public void SetLoadDataToCamVal()
	{
		if (m_isExistLoadCamValue)
		{
			Transform transform = Camera.main.transform;
			transform.position = m_vecLoadCamPos;
			transform.localScale = m_vecLoadCamScale;
			transform.eulerAngles = m_vecLoadCamAngle;
			Camera.main.fieldOfView = m_fLoadCamFOV;
			m_isExistLoadCamValue = false;
			RenderManager.instance.ReflashRenderCamera();
			RenderManager.instance.SetFOV_SubCameras(Camera.main.fieldOfView);
		}
	}

	public void SaveGameMainData(byte[] bySaveBuf, ref int iOffset)
	{
		bySaveBuf[iOffset++] = (byte)GameMain.m_eDefType;
	}

	public void LoadGameMainData(byte[] byLoadBuf, ref int iOffset)
	{
		GameMain.SetDefaultSceneType((GameMain.eDefType)byLoadBuf[iOffset++]);
	}

	public void SetScreenMode(eScreenMode screenMode)
	{
		m_eScreenMode = screenMode;
		Screen.fullScreen = m_eScreenMode == eScreenMode.FullScreen;
	}

	public eScreenMode GetScreenMode()
	{
		return m_eScreenMode;
	}

	public void SetResolution(eResolution resolution)
	{
		m_eResolution = resolution;
		int width = 1920;
		int height = 1080;
		switch (resolution)
		{
		case eResolution.R_1920x1080:
			width = 1920;
			height = 1080;
			break;
		case eResolution.R_1280x720:
			width = 1280;
			height = 720;
			break;
		}
		Screen.SetResolution(width, height, m_eScreenMode == eScreenMode.FullScreen);
	}

	public eResolution GetResolution()
	{
		return m_eResolution;
	}

	public void SetControllerType(eControllerType controllerType)
	{
		m_eController = controllerType;
		GamePadInput.ReflashControllerType();
	}

	public eControllerType GetControllerType()
	{
		return m_eController;
	}

	private void InitNew()
	{
		m_lstGetKeywordByOrder = new List<int>();
		SetLoadEventData(isLoad: false);
	}

	public void InitGameSwitch(bool isRestart = false)
	{
		InitNonSaveData();
		m_iCurCutEffIdx = -1;
		SetAllKeywordPopByCut(isShowPop: false);
		m_fGameTime = 0f;
		SetMental(10);
		InitAllPivotXY();
		BitCalc.InitArray(m_byArrEvtSwitch, 0);
		BitCalc.InitArray(m_byArrSelectSwitch, 0);
		BitCalc.InitArray(m_byArrChrAnonymous, 0);
		BitCalc.InitArray(m_byArrChrRelation, 0);
		BitCalc.InitArray(m_byArrKeywordGroup, 0);
		InitReplyGroup();
		if (m_listVoteRank != null)
		{
			m_listVoteRank.Clear();
			m_listVoteRank = null;
		}
		m_listVoteRank = new List<VoteRank>();
		for (int i = 0; i < 8; i++)
		{
			AddVoteList(i, 0);
		}
		SortVoteRank();
		InitCharPartyMot(isRestart);
		m_iPartySelMax = -1;
		m_iPartySelIdx = -1;
		InitPost(isRestart);
		InitProfileKeyword();
		SetCurSequence(0);
		BitCalc.InitArray(m_byArrChrRelationConti, 0);
		BitCalc.InitArray(m_byArrImage, 0);
		BitCalc.InitArray(m_byArrSound, 0);
		BitCalc.InitArray(m_byArrCutCharCall, 0);
		m_iSWRingTone = 0;
		Xls.CollSounds collSounds = null;
		List<Xls.CollSounds> datas = Xls.CollSounds.datas;
		int count = datas.Count;
		for (int j = 0; j < count; j++)
		{
			collSounds = datas[j];
			if (collSounds.m_iCategory == 3)
			{
				break;
			}
		}
		if (collSounds != null)
		{
			SetSWRingtone(collSounds.m_strKey);
		}
		m_iSWBackImage = 0;
		SetAutoSNSText(0);
		InitInvestObj();
		InitTalkSelIdx();
	}

	public void InitCollectionSwitch()
	{
		BitCalc.InitArray(m_byArrCollImage, 0);
		BitCalc.InitArray(m_byArrCollSound, 0);
		BitCalc.InitArray(m_iArrTrophyCnt);
		BitCalc.InitArray(m_byArrTrophyNew, 0);
		BitCalc.InitArray(m_byArrCollProfile, 0);
		BitCalc.InitArray(m_byArrCollKeyword, 0);
		BitCalc.InitArray(m_byArrCollUseKeyword, 0);
		BitCalc.InitArray(m_byArrCollCutKeyword, 0);
		BitCalc.InitArray(m_byArrCollSelectKeyword, 0);
		BitCalc.InitArray(m_byArrCollTutorial, 0);
		m_fTotalGamePlayTime = 0f;
		m_isEndingShow = false;
	}

	public void InitNonSaveData()
	{
		if (m_clArrCharPartySet == null)
		{
			m_clArrCharPartySet = new CharPartySet[8];
		}
		for (int i = 0; i < 8; i++)
		{
			m_clArrCharPartySet[i] = new CharPartySet();
			Xls.CharData data_bySwitchIdx = Xls.CharData.GetData_bySwitchIdx(i);
			if (data_bySwitchIdx != null)
			{
				m_clArrCharPartySet[i].m_strCharKey = data_bySwitchIdx.m_strKey;
			}
		}
		if (m_listPartySet == null)
		{
			m_listPartySet = new List<CharPartySet>();
		}
		m_lstSequenceInfo = new List<SequenceInfo>();
		m_iCurCallChar = -1;
		m_strRecordFaterPlayConti = null;
		UNIT_DELAY_TIME = GameGlobalUtil.GetXlsProgramDefineStrToFloat("OPT_AUTO_DELAY_UNIT");
		TRP_00050_MENTAL_VALUE = (int)GameGlobalUtil.GetXlsProgramDefineStrToFloat("TROPHY_00050_MENTAL_VALUE");
	}

	public void InitForConvertLoad()
	{
		InitSaveRunKeyword(isAllInit: true);
		m_lstGetKeywordByOrder.Clear();
		m_strEventObjNameForSave = null;
		SetStrEventObjName(null);
		m_strPMSendEventForSave = null;
		m_strPMSendEvent = null;
	}

	public void InitReplyGroup()
	{
		BitCalc.InitArray(m_byArrReplyGroup, 0);
	}

	private void Free()
	{
		if (m_clArrCharPartySet != null)
		{
			for (int i = 0; i < m_clArrCharPartySet.Length; i++)
			{
				m_clArrCharPartySet[i] = null;
			}
			m_clArrCharPartySet = null;
		}
		if (m_listPartySet != null)
		{
			for (int j = 0; j < m_listPartySet.Count; j++)
			{
				m_listPartySet[j] = null;
			}
			m_listPartySet.Clear();
		}
		if (m_listVoteRank != null)
		{
			m_listVoteRank.Clear();
			m_listVoteRank = null;
		}
		FreeSequence();
	}

	public void FreeGoToMain()
	{
		Free();
		FreeForCurKeywordEvtVal();
		m_iCurCutEffIdx = -1;
		m_icursequence = -1;
	}

	public void InitGameVal(ConstGameSwitch.eSTARTTYPE eStart, int iIdx = 0, SaveLoad.SaveLoadCB onLoadDone = null, bool isSaveLoadCBShowClose = true)
	{
		EventEngine instance = EventEngine.GetInstance();
		instance.SetSkip(isSkip: false);
		int num = (int)(GameGlobalUtil.GetXlsProgramDefineStrToFloat("MENTAL_GAGE_LESS_BOUND") * 100f);
		int num2 = (int)(GameGlobalUtil.GetXlsProgramDefineStrToFloat("MENTAL_GAGE_MORELESS_BOUND") * 100f);
		m_iHighMental = ConstGameSwitch.MAX_MENTAL_POINT * num / 100;
		m_iLowMental = ConstGameSwitch.MAX_MENTAL_POINT * num2 / 100;
		m_fBreakRelation = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_ICON_BREAK");
		m_fLowRelation = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_ICON_LOW");
		m_fMediumRelation = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_ICON_MEDIUM");
		m_fHighRelation = GameGlobalUtil.GetXlsProgramDefineStrToFloat("RELATION_ICON_HIGH");
		m_fEvtOccurLowRelation = GameGlobalUtil.GetXlsProgramDefineStrToFloat("EVT_OCCUR_LOW_RELATION");
		m_isRunLowRelationEvt = false;
		m_fGamePlayTime = 0f;
		m_fGameStartTime = Time.time;
		m_isPause = false;
		SetIsCheckSNSKeywordBut(isSet: true);
		m_isShowBrightSNSBut = false;
		if (GameMain.instance != null)
		{
			GameMain.instance.m_KeywordMenu.FreeKeywordMenu();
		}
		if (eStart != ConstGameSwitch.eSTARTTYPE.CONTINUE)
		{
			InitGameSwitch(eStart == ConstGameSwitch.eSTARTTYPE.RESTART);
			SetRunEventObj(eEventRunType.NONE);
			InitSaveRunKeyword(isAllInit: true);
			InitCutEff();
		}
		EventEngine.GetInstance().InitEnterGame();
		SaveLoad instance2 = SaveLoad.GetInstance();
		switch (eStart)
		{
		case ConstGameSwitch.eSTARTTYPE.NEW:
			SetReLoadSlotIdx(-1);
			m_isRestart = false;
			InitCollectionSwitch();
			onLoadDone(isExistErr: false);
			break;
		case ConstGameSwitch.eSTARTTYPE.CONTINUE:
			SetReLoadSlotIdx(iIdx);
			instance2.SaveLoadWhat(SaveLoad.eSaveWhat.eLoadCollGame, iIdx, onLoadDone, isSaveLoadCBShowClose, isLoadingIconNotClose: true);
			break;
		case ConstGameSwitch.eSTARTTYPE.RESTART:
			SetReLoadSlotIdx(-1);
			m_isRestart = true;
			instance2.SaveLoadWhat(SaveLoad.eSaveWhat.eLoadColl, iIdx, onLoadDone, isSaveLoadCBShowClose, isLoadingIconNotClose: true);
			break;
		}
	}

	private bool IsCompareOverBelow(bool isOver, float fOrigin, float fCompare)
	{
		if (isOver)
		{
			return fOrigin >= fCompare;
		}
		return fOrigin <= fCompare;
	}

	private bool isCompareOverBelow(bool isOver, int iOrigin, int iCompare)
	{
		if (isOver)
		{
			return iOrigin >= iCompare;
		}
		return iOrigin <= iCompare;
	}

	private bool IsCompareOverBelow(bool isOver, byte byOrigin, int byCompare)
	{
		if (isOver)
		{
			return byOrigin >= byCompare;
		}
		return byOrigin <= byCompare;
	}

	public int GetCharIdx(string strCharKey)
	{
		int num = 0;
		if (strCharKey == null)
		{
			return -1;
		}
		return Xls.CharData.GetData_byKey(strCharKey)?.m_iIdx ?? (-1);
	}

	public string GetCharName(int iIdx)
	{
		string result = null;
		Xls.CharData data_byIdx = Xls.CharData.GetData_byIdx(iIdx);
		if (data_byIdx != null)
		{
			result = data_byIdx.m_strKey;
		}
		return result;
	}

	public Vector2 GetCanvViewPosByWorldPos(Camera mainCam, RectTransform rtfCanv, Vector3 vecPos)
	{
		Vector2 vector = mainCam.WorldToViewportPoint(vecPos);
		Vector2 result = new Vector2(vector.x * rtfCanv.sizeDelta.x - rtfCanv.sizeDelta.x * 0.5f, vector.y * rtfCanv.sizeDelta.y - rtfCanv.sizeDelta.y * 0.5f);
		return result;
	}

	public Vector3 GetPosByPercentDecimalPoint(float fX, float fY, float fZ)
	{
		return Camera.main.ViewportToWorldPoint(new Vector3(fX, fY, fZ));
	}

	public Vector3 GetPosByPercentZeroToOne(float fX, float fY, float fZ)
	{
		return Camera.main.ViewportToWorldPoint(new Vector3(fX, fY, fZ));
	}

	public Vector3 GetPosWorldPointToViewPort(Vector3 vecPos)
	{
		return Camera.main.WorldToViewportPoint(vecPos);
	}

	public Vector3 GetPosByPercent(float fX, float fY, float fZ)
	{
		float x = fX / 100f;
		float y = fY / 100f;
		return Camera.main.ViewportToWorldPoint(new Vector3(x, y, fZ));
	}

	public static void CheckMinMax(ref float fValue, float fMin, float fMax)
	{
		if (fValue < fMin)
		{
			fValue = fMin;
		}
		else if (fValue > fMax)
		{
			fValue = fMax;
		}
	}

	public static void CheckMinMax(ref int iValue, int iMin, int iMax)
	{
		if (iValue < iMin)
		{
			iValue = iMin;
		}
		else if (iValue > iMax)
		{
			iValue = iMax;
		}
	}

	public static void CheckMinMax(ref byte byValue, byte byMin, byte byMax)
	{
		if (byValue < byMin)
		{
			byValue = byMin;
		}
		else if (byValue > byMax)
		{
			byValue = byMax;
		}
	}

	private float GetFloatTime(int iH, int iM)
	{
		return iH * 3600 + iM * 60;
	}

	public void GetHMByFloatTime(float fTime, ref int iH, ref int iM)
	{
		iH = (int)(fTime / 3600f);
		fTime -= (float)(iH * 3600);
		iM = (int)(fTime / 60f);
	}

	public void IgnoreMentalRelationEvt(bool isSet)
	{
		m_isIgnoreMentalRelationEvt = isSet;
	}

	public int GetMental()
	{
		return m_iMental;
	}

	public bool IsMentalBeyoundOrBroken()
	{
		bool result = false;
		int mental = GetMental();
		if (mental <= ConstGameSwitch.MIN_MENTAL_POINT || mental > ConstGameSwitch.MAX_MENTAL_POINT)
		{
			result = true;
		}
		return result;
	}

	public void SetMental(int iMental, bool isAddSet = false)
	{
		if (!m_isIgnoreMentalRelationEvt)
		{
			m_iMental = iMental;
			CheckMinMax(ref m_iMental, ConstGameSwitch.MIN_MENTAL_POINT, ConstGameSwitch.MAX_MENTAL_POINT);
			SetMentalPointUI(m_iMental);
			if (!isAddSet)
			{
			}
			if (m_iMental >= TRP_00050_MENTAL_VALUE)
			{
				AddTrophyCnt("trp_00050");
			}
		}
	}

	public bool AddMental(int iAddMental, bool isEnableEff = true)
	{
		if (m_isIgnoreMentalRelationEvt)
		{
			return false;
		}
		int num = m_iMental;
		SetMental(m_iMental + iAddMental, isAddSet: true);
		bool flag = iAddMental != 0;
		if (flag)
		{
			if (num == 100 && GetMental() == 100)
			{
				num = 99;
			}
			SetMentalPointUI(num, isEnableEff);
		}
		return flag;
	}

	public ConstGameSwitch.eMental GetMentalLowHigh()
	{
		if (m_iMental <= m_iLowMental)
		{
			return ConstGameSwitch.eMental.LOW;
		}
		if (m_iMental <= m_iHighMental)
		{
			return ConstGameSwitch.eMental.NORMAL;
		}
		return ConstGameSwitch.eMental.HIGH;
	}

	private void SetMentalPointUI(int iPrevPoint, bool isEnableEff = false)
	{
		GameMain instance = GameMain.instance;
		if (!(instance == null) && !(instance.m_gameMainMenu == null))
		{
			instance.m_gameMainMenu.SetMentalPoint(m_iMental, iPrevPoint, isEnableEff);
		}
	}

	public bool IsCheckMental(bool isOver, float iCheckMental)
	{
		return IsCompareOverBelow(isOver, m_iMental, iCheckMental);
	}

	public eChrIconState GetCharIconState(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharIconState(charIdx);
	}

	public eChrIconState GetCharIconState(int iCharIdx)
	{
		int num = 0;
		eChrIconState eChrIconState = eChrIconState.Zero;
		num = ((iCharIdx != 0) ? GetCharRelation(iCharIdx) : GetMental());
		if ((float)num < m_fBreakRelation)
		{
			return eChrIconState.Zero;
		}
		if ((float)num < m_fLowRelation)
		{
			return eChrIconState.Low;
		}
		if ((float)num < m_fMediumRelation)
		{
			return eChrIconState.Medium;
		}
		if ((float)num < m_fHighRelation)
		{
			return eChrIconState.High;
		}
		return eChrIconState.Full;
	}

	public float GetGameTime()
	{
		return m_fGameTime;
	}

	public void SetGameTime(int iH, int iM)
	{
		m_fGameTime = GetFloatTime(iH, iM);
	}

	public void AddGameTime(bool isAdd, int iH, int iM)
	{
		m_fGameTime += (float)(isAdd ? 1 : (-1)) * GetFloatTime(iH, iM);
		int iH2 = 0;
		int iM2 = 0;
		GetHMByFloatTime(m_fGameTime, ref iH2, ref iM2);
	}

	public void GetGameTime(ref int iH, ref int iM)
	{
		GetHMByFloatTime(m_fGameTime, ref iH, ref iM);
	}

	public bool CheckGameTime(bool isOver, int iH, int iM)
	{
		float floatTime = GetFloatTime(iH, iM);
		return IsCompareOverBelow(isOver, m_fGameTime, floatTime);
	}

	private int GetStructVoteRank(int iCharIdx)
	{
		int result = -1;
		for (int i = 0; i < 8; i++)
		{
			if (m_listVoteRank[i].m_iCharIdx == iCharIdx)
			{
				result = i;
			}
		}
		return result;
	}

	public void SetCharVotesCount(string strCharKey, int iValue)
	{
		int charIdx = GetCharIdx(strCharKey);
		if (BitCalc.CheckArrayIdx(charIdx, 8))
		{
			CheckMinMax(ref iValue, ConstGameSwitch.COUNT_MIN_VOTES_CNT, ConstGameSwitch.COUNT_MAX_VOTES_CNT);
			int structVoteRank = GetStructVoteRank(charIdx);
			if (structVoteRank != -1)
			{
				m_listVoteRank[structVoteRank].m_iVoteCnt = iValue;
				SortVoteRank();
			}
		}
	}

	public void AddCharVotesCount(string strCharKey, int iValue)
	{
		int charIdx = GetCharIdx(strCharKey);
		if (BitCalc.CheckArrayIdx(charIdx, 8))
		{
			int structVoteRank = GetStructVoteRank(charIdx);
			if (structVoteRank != -1)
			{
				m_listVoteRank[structVoteRank].m_iVoteCnt += iValue;
				CheckMinMax(ref m_listVoteRank[structVoteRank].m_iVoteCnt, ConstGameSwitch.COUNT_MIN_VOTES_CNT, ConstGameSwitch.COUNT_MAX_VOTES_CNT);
				SortVoteRank();
			}
		}
	}

	public void SortVoteRank()
	{
		m_listVoteRank.Sort((VoteRank l, VoteRank r) => r.m_iVoteCnt.CompareTo(l.m_iVoteCnt));
		int num = 1;
		for (int num2 = 0; num2 < m_listVoteRank.Count; num2++)
		{
			if (num2 > 0 && m_listVoteRank[num2].m_iVoteCnt != m_listVoteRank[num2 - 1].m_iVoteCnt)
			{
				num++;
			}
			m_listVoteRank[num2].m_iRankNum = num;
		}
	}

	public VoteRank GetVoteRank(int iRank)
	{
		if (!BitCalc.CheckArrayIdx(iRank, 8))
		{
			return null;
		}
		return (iRank < 0 || iRank >= m_listVoteRank.Count) ? null : m_listVoteRank[iRank];
	}

	public bool AddVoteList(int iCharIdx, int iVoteCnt, int iRankNum = -1)
	{
		if (!BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			return false;
		}
		int count = m_listVoteRank.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listVoteRank[i].m_iCharIdx == iCharIdx)
			{
				return false;
			}
		}
		VoteRank voteRank = new VoteRank();
		voteRank.m_iCharIdx = iCharIdx;
		voteRank.m_iVoteCnt = iVoteCnt;
		if (iRankNum != -1)
		{
			voteRank.m_iRankNum = iRankNum;
		}
		m_listVoteRank.Add(voteRank);
		return true;
	}

	public void SetCharRelation(string strCharKey, int iValue, bool isAddSet = false)
	{
		if (m_isIgnoreMentalRelationEvt)
		{
			return;
		}
		int charIdx = GetCharIdx(strCharKey);
		if (BitCalc.CheckArrayIdx(charIdx, 8))
		{
			CheckMinMax(ref iValue, 0, 100);
			m_byArrChrRelation[charIdx] = (byte)iValue;
			if (isAddSet)
			{
			}
		}
	}

	public int GetCharRelation(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharRelation(charIdx);
	}

	public int GetCharRelation(int iCharIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			return -1;
		}
		return m_byArrChrRelation[iCharIdx];
	}

	public bool AddCharRelation(string strCharKey, int iRelation)
	{
		if (m_isIgnoreMentalRelationEvt)
		{
			return false;
		}
		int charIdx = GetCharIdx(strCharKey);
		if (!BitCalc.CheckArrayIdx(charIdx, 8))
		{
			return false;
		}
		byte b = m_byArrChrRelation[charIdx];
		SetCharRelation(strCharKey, iRelation + m_byArrChrRelation[charIdx], isAddSet: true);
		bool flag = iRelation != 0;
		if (flag)
		{
			if (!m_isSetKeywordEventRelation)
			{
				m_iAddedKeywordRelaton += iRelation;
			}
			bool flag2 = false;
			EventEngine instance = EventEngine.GetInstance();
			GameMain gameMain = null;
			if (instance != null)
			{
				gameMain = instance.m_GameMain;
			}
			if (!(gameMain != null) || !(gameMain.m_SmartWatchRoot != null) || gameMain.m_SmartWatchRoot.GetCurrentPhoneState() == SWSub_PhoneMenu.EngageState.Unknown)
			{
				GameObject gameObject = null;
				if (instance != null)
				{
					gameObject = instance.m_TalkChar.GetCharObjInScreen(strCharKey);
				}
				flag2 = gameObject != null;
				if (flag2)
				{
					if (GameMain.instance != null)
					{
						GameMain.instance.PlayCharMental(gameObject, iRelation);
					}
					AudioManager.instance.PlayUISound((iRelation <= 0) ? "NPC_Minus" : "NPC_Plus");
				}
			}
			if (!flag2)
			{
				NoticeUIManager.ActiveNotice_ChangeRelation_S(strCharKey, iRelation);
			}
			flag = true;
		}
		return flag;
	}

	public string GetLargestRelation(string strCharKey0, string strCharKey1, string strCharKey2, string strCharKey3, string strCharKey4)
	{
		int num = 5;
		string[] array = new string[num];
		int[] array2 = new int[num];
		array[0] = strCharKey0;
		array[1] = strCharKey1;
		array[2] = strCharKey2;
		array[3] = strCharKey3;
		array[4] = strCharKey4;
		for (int i = 0; i < num; i++)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(array[i]);
			if (data_byKey == null)
			{
				array2[i] = -1;
			}
			else
			{
				array2[i] = data_byKey.m_iIdx;
			}
		}
		Dictionary<int, byte> dictionary = new Dictionary<int, byte>();
		int num2 = 0;
		for (int j = 0; j < num; j++)
		{
			num2 = array2[j];
			if (BitCalc.CheckArrayIdx(num2, 8))
			{
				dictionary.Add(num2, m_byArrChrRelation[num2]);
			}
		}
		IOrderedEnumerable<KeyValuePair<int, byte>> orderedEnumerable = dictionary.OrderByDescending((KeyValuePair<int, byte> pair) => pair.Value);
		int num3 = 0;
		int num4 = -1;
		foreach (KeyValuePair<int, byte> item in orderedEnumerable)
		{
			if (num4 == -1)
			{
				num4 = item.Value;
				num3++;
				continue;
			}
			if (item.Value == num4)
			{
				num3++;
				continue;
			}
			break;
		}
		if (num3 > 1)
		{
			int num5 = 0;
			int num6 = 0;
			foreach (KeyValuePair<int, byte> item2 in orderedEnumerable)
			{
				if (num5 >= num3)
				{
					break;
				}
				for (int num7 = 0; num7 < num; num7++)
				{
					if (item2.Key == array2[num7])
					{
						num6 = item2.Value;
						dictionary[item2.Key] = (byte)(item2.Value + (num - num7));
						break;
					}
				}
				num5++;
			}
			orderedEnumerable = dictionary.OrderByDescending((KeyValuePair<int, byte> pair) => pair.Value);
		}
		int key = orderedEnumerable.First().Key;
		return Xls.CharData.GetData_byIdx(key).m_strKey;
	}

	public bool IsCheckCharRelation(bool isOver, string strCharKey, int iRelation)
	{
		int charIdx = GetCharIdx(strCharKey);
		if (!BitCalc.CheckArrayIdx(charIdx, 8))
		{
			return false;
		}
		return isCompareOverBelow(isOver, m_byArrChrRelation[charIdx], iRelation);
	}

	private int GetCharRelationContiIdx(string strCharKey, int iMental)
	{
		List<Xls.CharRelationEvt> datas = Xls.CharRelationEvt.datas;
		int count = datas.Count;
		int result = -1;
		string empty = string.Empty;
		for (int i = 0; i < count; i++)
		{
			if (datas[i].m_strKey == strCharKey)
			{
				empty = datas[i].m_strCheckTrpKey;
				if (iMental >= datas[i].m_iRelationMax && GetCharRelationConti(i) == 0 && CheckEventSwitchAndTrophy(datas[i]))
				{
					result = i;
					break;
				}
				if (GetCharRelationConti(i) == 0)
				{
					break;
				}
			}
		}
		return result;
	}

	private bool CheckEventSwitchAndTrophy(Xls.CharRelationEvt xlsCharRelation)
	{
		int[] intSeparateText = GameGlobalUtil.GetIntSeparateText(xlsCharRelation.m_strCheckEvtNum, ',');
		int num = intSeparateText.Length;
		string text = xlsCharRelation.m_strCheckTrpKey.Trim();
		bool flag = false;
		bool flag2 = false;
		if (num < 1 || (num == 1 && intSeparateText[0] == -1))
		{
			flag = true;
		}
		else
		{
			flag = true;
			for (int i = 0; i < num; i++)
			{
				if (GetEventSwitch(intSeparateText[i]) == 0)
				{
					flag = false;
					break;
				}
			}
		}
		flag2 = text == string.Empty || text == null || GetTrophyComplete(text);
		return flag && flag2;
	}

	public string CheckAndOnCharRelationContiIdx(int iCharIdx)
	{
		return CheckAndOnCharRelationContiIdx(Xls.CharData.GetData_bySwitchIdx(iCharIdx).m_strKey);
	}

	public string CheckAndOnCharRelationContiIdx(string strCharKey, bool isSetSwitch = true)
	{
		string result = null;
		if (strCharKey != null)
		{
			int charRelation = GetCharRelation(strCharKey);
			if (m_iAddedKeywordRelaton > 0)
			{
				int charRelationContiIdx = GetCharRelationContiIdx(strCharKey, charRelation);
				if (charRelationContiIdx != -1)
				{
					if (isSetSwitch)
					{
						SetCharRelationConti(charRelationContiIdx, 1);
					}
					result = Xls.CharRelationEvt.datas[charRelationContiIdx].m_strContiName;
				}
			}
		}
		return result;
	}

	private void SetCharRelationConti(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, 160))
		{
			SetBitSwitch(m_byArrChrRelationConti, iIdx, byValue);
		}
	}

	private sbyte GetCharRelationConti(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, 160))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrChrRelationConti, iIdx);
	}

	public void InitTalkSelIdx()
	{
		m_iPartySelIdx = 0;
	}

	public int GetSelPartyCharIdx()
	{
		int result = -1;
		for (int i = 0; i < 8; i++)
		{
			if (m_clArrCharPartySet[i].m_iOrderIdx == m_iPartySelIdx)
			{
				result = i;
			}
		}
		return result;
	}

	private void SetPartyOrderIdx(bool isResetPartyData = false)
	{
		if (m_iCurCutEffIdx == -1)
		{
			return;
		}
		int num = 8;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			m_clArrCharPartySet[i].m_iOrderIdx = -1;
		}
		int count = m_listPartySet.Count;
		for (int j = 0; j < count; j++)
		{
			if (m_listPartySet[j].m_byState == 1)
			{
				Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(m_listPartySet[j].m_strCharKey);
				if (talkCutSetting != null && talkCutSetting.m_iIconShow == 1)
				{
					m_listPartySet[j].m_iIconShow = talkCutSetting.m_iIconShow;
					m_listPartySet[j].m_iSize = talkCutSetting.m_iSize;
					m_listPartySet[j].m_iDir = talkCutSetting.m_iDir;
					m_listPartySet[j].m_fPosX = talkCutSetting.m_fPosX;
					m_listPartySet[j].m_fPosY = talkCutSetting.m_fPosY;
				}
			}
		}
		m_listPartySet.Sort((CharPartySet a, CharPartySet b) => a.m_fPosX.CompareTo(b.m_fPosX));
		for (int num3 = 0; num3 < count; num3++)
		{
			if (m_listPartySet[num3].m_byState == 1 && m_listPartySet[num3].m_iIconShow == 1)
			{
				int num4 = 0;
				Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_listPartySet[num3].m_strCharKey);
				if (data_byKey != null)
				{
					num4 = data_byKey.m_iIdx;
				}
				m_clArrCharPartySet[num4].m_iOrderIdx = num2++;
			}
		}
		if (num2 > 0)
		{
			m_iPartySelMax = num2 - 1;
		}
		if (m_iPartySelIdx > m_iPartySelMax)
		{
			m_iPartySelIdx = m_iPartySelMax;
		}
		if (m_iPartySelMax > 0 && m_iPartySelIdx < 0)
		{
			m_iPartySelIdx = 0;
		}
	}

	public void MoveTalkSelIdx(bool isLeft)
	{
		if (m_iPartySelIdx == -1)
		{
			m_iPartySelIdx = 0;
		}
		else if (isLeft)
		{
			if (m_iPartySelIdx > 0)
			{
				m_iPartySelIdx--;
			}
			else
			{
				m_iPartySelIdx = m_iPartySelMax;
			}
		}
		else if (m_iPartySelIdx < m_iPartySelMax)
		{
			m_iPartySelIdx++;
		}
		else
		{
			m_iPartySelIdx = 0;
		}
	}

	public void SetTalkSelChar(string strCharKey)
	{
		int num = m_clArrCharPartySet.Length;
		for (int i = 0; i < num; i++)
		{
			if (m_clArrCharPartySet[i].m_strCharKey == strCharKey)
			{
				m_iPartySelIdx = m_clArrCharPartySet[i].m_iOrderIdx;
			}
		}
	}

	private CharPartySet FindListParty(int iCharIdx)
	{
		CharPartySet result = null;
		int count = m_listPartySet.Count;
		for (int i = 0; i < count; i++)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_listPartySet[i].m_strCharKey);
			if (data_byKey != null && iCharIdx == data_byKey.m_iIdx)
			{
				result = m_listPartySet[i];
			}
		}
		return result;
	}

	private void DelTalkSelIdx(int iCharIdx)
	{
		if (m_clArrCharPartySet[iCharIdx].m_iOrderIdx == m_iPartySelIdx)
		{
			MoveTalkSelIdx(m_iPartySelIdx >= 0);
		}
		CharPartySet item = FindListParty(iCharIdx);
		m_listPartySet.Remove(item);
		SetPartyOrderIdx();
	}

	private void InitCharPartyMot(bool isRestart = false)
	{
		for (int i = 0; i < 8; i++)
		{
			m_clArrCharPartySet[i].m_strMot = string.Empty;
		}
	}

	public void SetCharPartyState(string strCharKey, string strEventKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		byte b = (byte)GameGlobalUtil.GetXlsScriptKeyValue(strEventKey);
		if (!BitCalc.CheckArrayIdx(charIdx, 8))
		{
			return;
		}
		m_clArrCharPartySet[charIdx].m_byState = b;
		if (b == 1)
		{
			CharPartySet charPartySet = FindListParty(charIdx);
			if (charPartySet == null)
			{
				Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(strCharKey);
				if (talkCutSetting != null)
				{
					m_clArrCharPartySet[charIdx].m_iIconShow = talkCutSetting.m_iIconShow;
					m_clArrCharPartySet[charIdx].m_iSize = talkCutSetting.m_iSize;
					m_clArrCharPartySet[charIdx].m_iDir = talkCutSetting.m_iDir;
					m_clArrCharPartySet[charIdx].m_fPosX = talkCutSetting.m_fPosX;
					m_clArrCharPartySet[charIdx].m_fPosY = talkCutSetting.m_fPosY;
				}
			}
			charPartySet = m_clArrCharPartySet[charIdx];
			int count = m_listPartySet.Count;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				flag = m_listPartySet[i].m_strCharKey == strCharKey;
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				m_listPartySet.Add(charPartySet);
			}
			SetPartyOrderIdx();
		}
		else
		{
			DelTalkSelIdx(charIdx);
		}
	}

	public bool IsThisPosSelTalkPos(int iCharIdx)
	{
		bool result = false;
		CharPartySet charPartySet = null;
		for (int i = 0; i < 8; i++)
		{
			if (i == iCharIdx)
			{
				charPartySet = m_clArrCharPartySet[i];
				break;
			}
		}
		if (charPartySet != null)
		{
			result = charPartySet.m_iOrderIdx == m_iPartySelIdx;
		}
		return result;
	}

	public int GetCharPartyOrder(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return m_clArrCharPartySet[charIdx].m_iOrderIdx;
	}

	public int GetRealSelPartyObjIdx()
	{
		int result = -1;
		for (int i = 0; i < 8; i++)
		{
			if (m_iPartySelIdx == m_clArrCharPartySet[i].m_iOrderIdx)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public string GetCharPosKeyInCutSetting(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPosKeyInCutSetting(charIdx);
	}

	public string GetCharPosKeyInCutSetting(int iCharKey)
	{
		string result = null;
		if (m_iCurCutEffIdx != -1)
		{
			Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(m_iCurCutEffIdx);
			if (data_bySwitchIdx != null)
			{
				switch (iCharKey)
				{
				case 0:
					result = data_bySwitchIdx.m_strHanPos;
					break;
				case 1:
					result = data_bySwitchIdx.m_strMinPos;
					break;
				case 2:
					result = data_bySwitchIdx.m_strSeoPos;
					break;
				case 3:
					result = data_bySwitchIdx.m_strOhPos;
					break;
				case 4:
					result = data_bySwitchIdx.m_strLeePos;
					break;
				case 5:
					result = data_bySwitchIdx.m_strChangPos;
					break;
				}
			}
		}
		return result;
	}

	private Xls.TalkCutChrSetting GetTalkCutSetting(int iCharIdx)
	{
		Xls.TalkCutChrSetting result = null;
		if (BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			result = GetTalkCutSetting(Xls.CharData.GetData_bySwitchIdx(iCharIdx).m_strKey);
		}
		return result;
	}

	public Xls.TalkCutChrSetting GetTalkCutSetting(string strCharKey)
	{
		Xls.TalkCutChrSetting result = null;
		string charPosKeyInCutSetting = GetCharPosKeyInCutSetting(strCharKey);
		if (charPosKeyInCutSetting != null)
		{
			result = Xls.TalkCutChrSetting.GetData_byKey(charPosKeyInCutSetting);
		}
		return result;
	}

	public int GetCharParty(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharParty(charIdx);
	}

	public int GetCharParty(int iCharIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			return 0;
		}
		return m_clArrCharPartySet[iCharIdx].m_byState;
	}

	public Vector2 GetCharPartyPos(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPartyPos(charIdx);
	}

	public Vector2 GetCharPartyPos(int iCharIdx)
	{
		Vector2 result = Vector2.zero;
		if (BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
			if (talkCutSetting != null)
			{
				result = new Vector2(talkCutSetting.m_fPosX, talkCutSetting.m_fPosY);
			}
		}
		return result;
	}

	public float GetCharKeywordPosY(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharKeywordPosY(charIdx);
	}

	public float GetCharKeywordPosY(int iCharIdx)
	{
		float result = -1f;
		if (BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
			if (talkCutSetting != null)
			{
				result = talkCutSetting.m_fPosY_Keyword;
			}
		}
		return result;
	}

	public float GetCharKeywordPosX(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharKeywordPosX(charIdx);
	}

	public float GetCharKeywordPosX(int iCharIdx)
	{
		float result = -1f;
		if (BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
			if (talkCutSetting != null)
			{
				result = talkCutSetting.m_fPosX_Keyword;
			}
		}
		return result;
	}

	public int GetCharPartySize(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPartySize(charIdx);
	}

	public int GetCharPartySize(int iCharIdx)
	{
		int result = -1;
		Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
		if (talkCutSetting != null)
		{
			result = talkCutSetting.m_iSize;
		}
		return result;
	}

	public string GetCharPartyMotion(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPartyMotion(charIdx);
	}

	public string GetCharPartyMotion(int iCharIdx)
	{
		string result = null;
		Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
		if (talkCutSetting != null)
		{
			result = talkCutSetting.m_strMotion;
		}
		return result;
	}

	public int GetCharPartyDir(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPartyDir(charIdx);
	}

	public int GetCharPartyDir(int iCharIdx)
	{
		int result = -1;
		Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
		if (talkCutSetting != null)
		{
			result = talkCutSetting.m_iDir;
		}
		return result;
	}

	public int GetCharPartyShowIcon(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharPartyShowIcon(charIdx);
	}

	public int GetCharPartyShowIcon(int iCharIdx)
	{
		int result = -1;
		Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(iCharIdx);
		if (talkCutSetting != null)
		{
			result = talkCutSetting.m_iIconShow;
		}
		return result;
	}

	public bool IsCharPartyState(string strCharKey, string strEventKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return IsCharPartyState(charIdx, strEventKey);
	}

	public bool IsCharPartyState(int iCharIdx, string strEventKey)
	{
		byte byValue = (byte)GameGlobalUtil.GetXlsScriptKeyValue(strEventKey);
		return IsCharPartyState(iCharIdx, byValue);
	}

	public bool IsCharPartyState(int iCharIdx, byte byValue)
	{
		if (iCharIdx < 0 || iCharIdx >= 8)
		{
			return false;
		}
		return GetCharParty(iCharIdx).Equals(byValue);
	}

	public bool IsExistPartyIn()
	{
		bool result = false;
		for (int i = 0; i < 8; i++)
		{
			if (m_clArrCharPartySet[i] != null && m_clArrCharPartySet[i].m_byState == 1)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void ChangeCharKeyNamToChang(ref string strCharKey)
	{
		if (strCharKey == "남세일")
		{
			strCharKey = "장세일";
		}
	}

	public int GetCharGetXlsIdx(string strCharKey, int iSize, int iPos)
	{
		ChangeCharKeyNamToChang(ref strCharKey);
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
		int num = 0;
		if (data_byKey != null)
		{
			num = data_byKey.m_iIdx;
		}
		return num * ConstGameSwitch.COUNT_MAX_1_CHAR_POS_XLS_CNT + iSize * ConstGameSwitch.COUNT_MAX_CHAR_POS_CNT + iPos;
	}

	public int GetCharGetDataYIdx(string strCharKey, int iSize)
	{
		ChangeCharKeyNamToChang(ref strCharKey);
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
		int num = 0;
		if (data_byKey != null)
		{
			num = data_byKey.m_iIdx;
		}
		return num * ConstGameSwitch.COUNT_MAX_CHAR_SIZE_CNT + iSize;
	}

	public void SetPivotXY(string strCharKey, int iSize, int iPos, float fX, float fY)
	{
		int charGetXlsIdx = GetCharGetXlsIdx(strCharKey, iSize, iPos);
		int charGetDataYIdx = GetCharGetDataYIdx(strCharKey, iSize);
		m_fArrPosX[charGetXlsIdx] = fX;
		m_fArrPosY[charGetDataYIdx] = fY;
	}

	public float GetCharPivotX(string strCharKey, int iSize, int iPos)
	{
		int charGetXlsIdx = GetCharGetXlsIdx(strCharKey, iSize, iPos);
		return m_fArrPosX[charGetXlsIdx];
	}

	public float GetCharPivotY(string strCharKey, int iSize)
	{
		int charGetDataYIdx = GetCharGetDataYIdx(strCharKey, iSize);
		return m_fArrPosY[charGetDataYIdx];
	}

	public void InitAllPivotXY(string strCharKey = null)
	{
		if (strCharKey == null)
		{
			BitCalc.InitArray(m_fArrPosX, 999f);
			BitCalc.InitArray(m_fArrPosY, 999f);
			return;
		}
		int charGetXlsIdx = GetCharGetXlsIdx(strCharKey, 0, 0);
		int charGetXlsIdx2 = GetCharGetXlsIdx(strCharKey, 4, 6);
		int charGetDataYIdx = GetCharGetDataYIdx(strCharKey, 0);
		int charGetDataYIdx2 = GetCharGetDataYIdx(strCharKey, 4);
		for (int i = charGetXlsIdx; i < charGetXlsIdx2; i++)
		{
			m_fArrPosX[i] = 999f;
		}
		for (int j = charGetDataYIdx; j < charGetDataYIdx2; j++)
		{
			m_fArrPosY[j] = 999f;
		}
	}

	public void SetKeywordGroup(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_GROUP_CNT))
		{
			SetBitSwitch(m_byArrKeywordGroup, iIdx, byValue);
		}
	}

	public sbyte GetKeywordGroup(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_GROUP_CNT))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrKeywordGroup, iIdx);
	}

	public void SetReplyGroup(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_REPLY_GROUP_CNT))
		{
			SetBitSwitch(m_byArrReplyGroup, iIdx, byValue);
		}
	}

	public sbyte GetReplyGroup(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_REPLY_GROUP_CNT))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrReplyGroup, iIdx);
	}

	public void SetEventSwitch(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_EVENT))
		{
			SetBitSwitch(m_byArrEvtSwitch, iIdx, byValue);
		}
	}

	public sbyte GetEventSwitch(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_EVENT))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrEvtSwitch, iIdx);
	}

	public void SetSelectSwitch(string strSelKey, byte byValue)
	{
		Xls.SelectData data_byKey = Xls.SelectData.GetData_byKey(strSelKey);
		if (data_byKey != null)
		{
			SetSelectSwitch(data_byKey.m_iIndex, byValue);
		}
	}

	public void SetSelectSwitch(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SELECT) && (byValue == 1 || byValue == 2))
		{
			int num = byValue - 1;
			int iIdx2 = (iIdx << 1) + num;
			SetBitSwitch(m_byArrSelectSwitch, iIdx2, 1);
		}
	}

	public bool GetBefSelectSwitchOn(int iIdx, ConstGameSwitch.eSELECT eSel)
	{
		byte b = 0;
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SELECT))
		{
			int num = (byte)eSel - 1;
			int iIdx2 = (iIdx << 1) + num;
			b = GetBitSwitch(m_byArrSelectSwitch, iIdx2);
		}
		return b == 1;
	}

	public void InitInvestObj()
	{
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			m_arrStrInvestObj[i] = null;
			m_arrStrInvestName[i] = null;
			m_arrIsInvestShow[i] = false;
			m_arrIsCheckInvest[i] = false;
			m_lstInvestXOrder.Clear();
			m_lstInvestYOrder.Clear();
		}
		m_iInvestObjSelIdx = -1;
		m_iInvestObjSelMax = -1;
	}

	public int GetInvestObjCnt()
	{
		return m_lstInvestXOrder.Count;
	}

	public int GetInvestSelOrder()
	{
		return m_iInvestObjSelIdx;
	}

	public void MoveInvestXYPlusMinus(bool isX, bool isPlus, bool isPlayUISound = true)
	{
		int count = m_lstInvestXOrder.Count;
		if (count < 1)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (m_iInvestObjSelIdx == ((!isX) ? m_lstInvestYOrder[i].Key : m_lstInvestXOrder[i].Key))
			{
				num = i;
				break;
			}
		}
		int iInvestObjSelIdx = m_iInvestObjSelIdx;
		int num2 = (isPlus ? 1 : (-1));
		int num3 = num2 + num;
		num3 = ((num3 >= 0) ? num3 : (count - 1));
		num = num3 % count;
		m_iInvestObjSelIdx = ((!isX) ? m_lstInvestYOrder[num].Key : m_lstInvestXOrder[num].Key);
		if (isPlayUISound && iInvestObjSelIdx != m_iInvestObjSelIdx)
		{
			AudioManager.instance.PlayUISound("Select_Marker");
		}
	}

	public void SetInvestListIdx(int iIdx, bool isPlayUISound = true)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ) && m_iInvestObjSelIdx != iIdx)
		{
			m_iInvestObjSelIdx = iIdx;
			if (isPlayUISound)
			{
				AudioManager.instance.PlayUISound("Select_Marker");
			}
		}
	}

	private int GetInvestListIdx(bool isX, int iKey)
	{
		int count = m_lstInvestXOrder.Count;
		int result = -1;
		for (int i = 0; i < count; i++)
		{
			if (iKey == ((!isX) ? m_lstInvestYOrder[i].Key : m_lstInvestXOrder[i].Key))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public int GetInvestListCount()
	{
		return m_lstInvestXOrder.Count;
	}

	public void DelInvestIdx()
	{
		if (m_iInvestObjSelIdx == -1)
		{
			return;
		}
		int iInvestObjSelIdx = m_iInvestObjSelIdx;
		int count = m_lstInvestXOrder.Count;
		int count2 = m_lstInvestXOrder.Count;
		MoveInvestXYPlusMinus(isX: true, isPlus: true, isPlayUISound: false);
		m_arrIsCheckInvest[iInvestObjSelIdx] = true;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < count2; i++)
		{
			if (!flag && m_lstInvestXOrder[i].Key == iInvestObjSelIdx)
			{
				flag = true;
				m_lstInvestXOrder.Remove(m_lstInvestXOrder[i]);
			}
			if (!flag2 && m_lstInvestYOrder[i].Key == iInvestObjSelIdx)
			{
				flag2 = true;
				m_lstInvestYOrder.Remove(m_lstInvestYOrder[i]);
			}
		}
		bool flag3 = true;
		for (int j = 0; j < ConstGameSwitch.COUNT_INVEST_OBJ; j++)
		{
			if (m_arrStrInvestName[j] != null)
			{
				flag3 &= m_arrIsCheckInvest[j];
				if (!m_arrIsCheckInvest[j])
				{
					break;
				}
			}
		}
		if (flag3)
		{
			if (iInvestObjSelIdx != -1 && count > 0)
			{
				EventEngine.GetInstance().EnableAndRunObj(GameGlobalUtil.GetXlsProgramDefineStr("RUN_INVEST_END"));
			}
			m_iInvestObjSelIdx = -1;
		}
	}

	public void SetInvestOrder(GameObject[] goInvestObj)
	{
		int num = goInvestObj.Length;
		m_lstInvestXOrder.Clear();
		m_lstInvestYOrder.Clear();
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("FIND_MARKER_PRE_NAME");
		bool flag = false;
		int i = 0;
		int index = 0;
		for (; i < num; i++)
		{
			flag = false;
			for (int j = 0; j < ConstGameSwitch.COUNT_INVEST_OBJ; j++)
			{
				if (goInvestObj[i] != null && goInvestObj[i].name == xlsProgramDefineStr + m_arrStrInvestName[j])
				{
					flag = m_arrIsInvestShow[j];
					break;
				}
			}
			if (flag)
			{
				Vector3 position = goInvestObj[i].GetComponent<RectTransform>().position;
				m_lstInvestXOrder.Insert(index, new KeyValuePair<int, float>(i, position.x));
				m_lstInvestYOrder.Insert(index++, new KeyValuePair<int, float>(i, position.y));
			}
		}
		m_lstInvestXOrder.Sort((KeyValuePair<int, float> x, KeyValuePair<int, float> y) => -y.Value.CompareTo(x.Value));
		m_lstInvestYOrder.Sort((KeyValuePair<int, float> x, KeyValuePair<int, float> y) => -x.Value.CompareTo(y.Value));
		if (m_iInvestObjSelIdx == -1 && m_lstInvestXOrder.Count > 0)
		{
			m_iInvestObjSelIdx = m_lstInvestXOrder[0].Key;
		}
	}

	public void DelInvestObj(int iIdx)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ))
		{
			m_arrStrInvestObj[iIdx] = null;
			m_arrStrInvestName[iIdx] = null;
			m_arrIsInvestShow[iIdx] = false;
			m_arrIsCheckInvest[iIdx] = false;
		}
	}

	public bool IsExistInvestObj()
	{
		bool result = false;
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			if (m_arrStrInvestObj[i] != null)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void AddInvestObj(string strInvestObj, string strTextKey, bool isShow = true)
	{
		int num = -1;
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			if (m_arrStrInvestObj[i] == null)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			m_arrStrInvestObj[num] = strInvestObj;
			m_arrStrInvestName[num] = strTextKey;
			m_arrIsInvestShow[num] = isShow;
			m_arrIsCheckInvest[num] = false;
		}
	}

	public void SetShowInvestObj(string strInvestObj)
	{
		int num = -1;
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			if (m_arrStrInvestName[i] == strInvestObj)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			m_arrIsInvestShow[num] = true;
		}
	}

	public string GetInvestObj(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ))
		{
			return null;
		}
		return m_arrStrInvestObj[iIdx];
	}

	public string GetInvestNameKey(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ))
		{
			return null;
		}
		return m_arrStrInvestName[iIdx];
	}

	public bool GetInvestShow(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ))
		{
			return false;
		}
		return m_arrIsInvestShow[iIdx];
	}

	public int GetInvestSaveSize()
	{
		int num = 0;
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			num += SIZE_INT;
			num += BitCalc.GetSizeStringToByte(m_arrStrInvestObj[i]);
			num += SIZE_INT;
			num += BitCalc.GetSizeStringToByte(m_arrStrInvestName[i]);
			num += 2;
		}
		return num + SIZE_INT;
	}

	public void SaveInvestObj(byte[] bySaveBuf, ref int iOffset)
	{
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			BitCalc.StringToByteWithSizeNCO(m_arrStrInvestObj[i], bySaveBuf, ref iOffset);
			BitCalc.StringToByteWithSizeNCO(m_arrStrInvestName[i], bySaveBuf, ref iOffset);
			BitCalc.BooleanToByteNCO(m_arrIsInvestShow[i], bySaveBuf, ref iOffset);
			BitCalc.BooleanToByteNCO(m_arrIsCheckInvest[i], bySaveBuf, ref iOffset);
		}
		BitCalc.IntToByteNCO(m_iInvestObjSelIdx, bySaveBuf, ref iOffset);
	}

	public void LoadInvestObj(byte[] byLoadBuf, ref int iOffset)
	{
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			m_arrStrInvestObj[i] = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			m_arrStrInvestName[i] = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			m_arrIsInvestShow[i] = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
			m_arrIsCheckInvest[i] = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
		}
		m_iInvestObjSelIdx = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
	}

	public void AddTrophyCnt(string strTrophyKey)
	{
		if (GetTrophyComplete(strTrophyKey))
		{
			return;
		}
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		if (BitCalc.CheckArrayIdx(trophyIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			int trophyCnt = GetTrophyCnt(trophyIdx);
			SetTrophyCnt(strTrophyKey, m_iArrTrophyCnt[trophyIdx] + 1);
			int trophyCnt2 = GetTrophyCnt(trophyIdx);
			Xls.Trophys data_byKey = Xls.Trophys.GetData_byKey(strTrophyKey);
			if (trophyCnt != trophyCnt2 && data_byKey != null && data_byKey.m_iMax == trophyCnt2)
			{
				SetTrophyNew(trophyIdx);
			}
		}
	}

	public void SetTrophyCnt(string strTrophyKey, int iCnt)
	{
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		SetTrophyCnt(trophyIdx, iCnt);
	}

	public void SetTrophyCnt(int iIdx, int iCnt)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			return;
		}
		int iMax = Xls.Trophys.GetData_bySwitchIdx(iIdx).m_iMax;
		if (m_iArrTrophyCnt[iIdx] >= iMax)
		{
			return;
		}
		m_iArrTrophyCnt[iIdx] = iCnt;
		if (iMax <= m_iArrTrophyCnt[iIdx])
		{
			m_iArrTrophyCnt[iIdx] = iMax;
			NoticeUIManager.ActiveNotice_S_SetKey(NoticeUIManager.NoticeType.ToDo, iIdx);
			Xls.Trophys data_bySwitchIdx = Xls.Trophys.GetData_bySwitchIdx(iIdx);
			if (data_bySwitchIdx != null && !string.IsNullOrEmpty(data_bySwitchIdx.m_isteamTrpIdx) && SteamUserStats.SetAchievement(data_bySwitchIdx.m_isteamTrpIdx))
			{
				SteamUserStats.StoreStats();
			}
		}
	}

	public int GetTrophyIdx(string strTrophyKey)
	{
		return Xls.Trophys.GetData_byKey(strTrophyKey)?.m_iIndex ?? (-1);
	}

	public int GetTrophyCnt(string strTrophyKey)
	{
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		return GetTrophyCnt(trophyIdx);
	}

	public int GetTrophyCnt(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			return -1;
		}
		return m_iArrTrophyCnt[iIdx];
	}

	public bool GetTrophyComplete(string strTrophyKey)
	{
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		return GetTrophyComplete(trophyIdx);
	}

	public bool GetTrophyComplete(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			return false;
		}
		int iMax = Xls.Trophys.GetData_bySwitchIdx(iIdx).m_iMax;
		return GetTrophyCnt(iIdx) >= iMax;
	}

	private void SetTrophyNew(int iTrophyIdx)
	{
		if (BitCalc.CheckArrayIdx(iTrophyIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			sbyte trophyNew = GetTrophyNew(iTrophyIdx);
			if (trophyNew < 1)
			{
				Set2BitSwitch(m_byArrTrophyNew, iTrophyIdx, 1);
				CheckAndAddEndingTrophy(iTrophyIdx);
				CheckAndAllTrophy();
			}
		}
	}

	private void CheckAndAddEndingTrophy(int iTrophyIdx)
	{
		string strTrophyKey = "trp_00076";
		if (GetTrophyComplete(strTrophyKey))
		{
			return;
		}
		int dataCount = Xls.Trophys.GetDataCount();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < dataCount; i++)
		{
			if (Xls.Trophys.GetData_byIdx(i).m_iGoal == 15)
			{
				num++;
				if (GetTrophyComplete(i))
				{
					num2++;
				}
			}
		}
		if (num == num2)
		{
			AddTrophyCnt(strTrophyKey);
		}
	}

	private void CheckAndAllTrophy()
	{
		string strTrophyKey = "trp_00077";
		if (GetTrophyComplete(strTrophyKey))
		{
			return;
		}
		int dataCount = Xls.Trophys.GetDataCount();
		int num = 0;
		for (int i = 0; i < dataCount; i++)
		{
			if (GetTrophyComplete(i))
			{
				num++;
			}
		}
		if (num == dataCount - 1)
		{
			AddTrophyCnt(strTrophyKey);
		}
	}

	public void SetTrophyRead(string strTrophyKey)
	{
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		SetTrophyRead(trophyIdx);
	}

	public void SetTrophyRead(int iTrophyIdx)
	{
		if (BitCalc.CheckArrayIdx(iTrophyIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			Set2BitSwitch(m_byArrTrophyNew, iTrophyIdx, 2);
		}
	}

	public sbyte GetTrophyNew(string strTrophyKey)
	{
		int trophyIdx = GetTrophyIdx(strTrophyKey);
		return GetTrophyNew(trophyIdx);
	}

	public sbyte GetTrophyNew(int iTrophyIdx)
	{
		if (!BitCalc.CheckArrayIdx(iTrophyIdx, ConstGameSwitch.COUNT_MAX_TROPHY_CNT))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrTrophyNew, iTrophyIdx);
	}

	public void SetMessageSwitchSettable(bool isSet)
	{
		m_isMessageSwitchSettable = isSet;
	}

	public void SetMessage(string strKey, byte byValue)
	{
		if (!m_isMessageSwitchSettable || m_iMessageGetCount >= ConstGameSwitch.COUNT_MESSENGER_ORDER)
		{
			return;
		}
		Xls.MessengerTalkData data_byKey = Xls.MessengerTalkData.GetData_byKey(strKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MESSENGER_MESSAGE))
			{
				m_iArrMessageOrder[m_iMessageGetCount] = iIdx;
				m_iMessageGetCount++;
			}
		}
	}

	public int[] GetMessageOrderArray()
	{
		return m_iArrMessageOrder;
	}

	public int GetMessageOrderByIdx(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MESSENGER_ORDER))
		{
			return -1;
		}
		return m_iArrMessageOrder[iIdx];
	}

	public int GetMessageGetCount()
	{
		return m_iMessageGetCount;
	}

	private void InitPost(bool isRestart = false)
	{
		InitOnlyPostData(isRestart);
		m_iMessageGetCount = 0;
		BitCalc.InitArray(m_iArrMessageOrder);
		BitCalc.InitArray(m_byArrKeywordAllState, 0);
	}

	private void InitOnlyPostData(bool isRestart = false)
	{
		BitCalc.InitArray(m_byArrPostState, 0);
		List<Xls.SNSPostData> datas = Xls.SNSPostData.datas;
		int count = datas.Count;
		Xls.SNSPostData sNSPostData = null;
		int num = 0;
		BitCalc.InitArray(m_iPostRetweetCnt);
		for (int i = 0; i < count; i++)
		{
			sNSPostData = datas[i];
			num = sNSPostData.m_iIdx;
			if (BitCalc.CheckArrayIdx(num, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
			{
				m_iPostRetweetCnt[num] = sNSPostData.m_iRetweetCnt;
			}
		}
		BitCalc.InitArray(m_byArrPostReply, 0);
		BitCalc.InitArray(m_byArrPostKeyword, 0);
		BitCalc.InitArray(m_byArrPostRetweet, 0);
	}

	private void InitProfileKeyword()
	{
		BitCalc.InitArray(m_byArrCharProfile, 0);
		BitCalc.InitArray(m_byArrKeywordSeqState, 0);
		BitCalc.InitArray(m_byArrKeywordSeqUseKeywordBef, 0);
		BitCalc.InitArray(m_byArrKeywordSeqUseKeyword, 0);
	}

	public void SetPostTalkWindowString(string strTextId)
	{
		m_iCutKeywordCnt = 0;
		m_iCutProfileCnt = 0;
		Buffer.BlockCopy(m_byArrChrRelation, 0, m_byCutStartChrRelation, 0, 8);
		m_iCutStartMental = m_iMental;
		m_strPostTalkWindowId = strTextId;
	}

	public void SetOnlyPostTalkWindowString(string strTextId)
	{
		m_strPostTalkWindowId = strTextId;
	}

	public string GetPostTalkWindowString()
	{
		return m_strPostTalkWindowId;
	}

	public void SetPostState(string strPostKey, byte byValue)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			SetPostState(data_byKey.m_iIdx, byValue);
		}
	}

	public bool SetPostState(int iIdx, byte byValue)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return false;
		}
		sbyte postState = GetPostState(iIdx);
		Set2BitSwitch(m_byArrPostState, iIdx, byValue);
		return true;
	}

	public sbyte GetPostState(string strPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey == null)
		{
			return -1;
		}
		return GetPostState(data_byKey.m_iIdx);
	}

	public sbyte GetPostState(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrPostState, iIdx);
	}

	public void SetPostKeyword(string strPostKey, byte byValue)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			SetPostKeyword(data_byKey.m_iIdx, byValue);
		}
	}

	public void SetPostKeyword(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			SetBitSwitch(m_byArrPostKeyword, iIdx, byValue);
		}
	}

	public sbyte GetPostKeyword(string strPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey == null)
		{
			return -1;
		}
		return GetPostKeyword(data_byKey.m_iIdx);
	}

	public sbyte GetPostKeyword(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrPostKeyword, iIdx);
	}

	public void SetPostRetweet(string strPostKey, byte byValue)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			SetPostRetweet(data_byKey.m_iIdx, byValue);
		}
	}

	public void SetPostRetweet(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			SetBitSwitch(m_byArrPostRetweet, iIdx, byValue);
		}
	}

	public sbyte GetPostRetweet(string strPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey == null)
		{
			return -1;
		}
		return GetPostRetweet(data_byKey.m_iIdx);
	}

	public sbyte GetPostRetweet(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrPostRetweet, iIdx);
	}

	public void SetPostReply(string strPostKey, byte byValue)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			SetPostReply(data_byKey.m_iIdx, byValue);
		}
	}

	public void SetPostReply(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			SetBitSwitch(m_byArrPostReply, iIdx, byValue);
		}
	}

	public sbyte GetPostReply(string strPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey == null)
		{
			return -1;
		}
		return GetPostReply(data_byKey.m_iIdx);
	}

	public sbyte GetPostReply(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrPostReply, iIdx);
	}

	public void AddPostRT(string strPostKey, int iPlusRT)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			AddPostRT(iIdx, iPlusRT);
		}
	}

	public void AddPostRT(int iPostIdx, int iPlusRT)
	{
		if (BitCalc.CheckArrayIdx(iPostIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			m_iPostRetweetCnt[iPostIdx] += iPlusRT;
			CheckMinMax(ref m_iPostRetweetCnt[iPostIdx], ConstGameSwitch.MIN_RETWEET_CNT, ConstGameSwitch.MAX_RETWEET_CNT);
		}
	}

	public int GetPostRT(string strPostKey)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey == null)
		{
			return -1;
		}
		return GetPostRT(data_byKey.m_iIdx);
	}

	public int GetPostRT(int iPostIdx)
	{
		if (!BitCalc.CheckArrayIdx(iPostIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			return -1;
		}
		return m_iPostRetweetCnt[iPostIdx];
	}

	public int GetPostCurSeqNewCnt()
	{
		int num = 0;
		Xls.SNSPostData sNSPostData = null;
		List<Xls.SNSPostData> datas = Xls.SNSPostData.datas;
		int dataCount = Xls.SNSPostData.GetDataCount();
		for (int i = 0; i < dataCount; i++)
		{
			sNSPostData = datas[i];
			if (sNSPostData != null && sNSPostData.m_strIDSeq.Equals(m_strCurSequence) && GetPostState(sNSPostData.m_iIdx) == 1)
			{
				num++;
			}
		}
		return num;
	}

	public void ReplacePostSwitch(string strPostOrgKey, string strReplacePostKey)
	{
		SetPostState(strPostOrgKey, 0);
		int iCheckEvtSwitch = Xls.SNSPostData.GetData_byKey(strReplacePostKey).m_iCheckEvtSwitch;
		if (iCheckEvtSwitch == -1 || GetEventSwitch(iCheckEvtSwitch) == 1)
		{
			SetPostState(strReplacePostKey, 1);
			SNSMenuPlus.ReplacePostData(strPostOrgKey, strReplacePostKey);
		}
	}

	public void CheckEvtAndAddPost(string strPostKey, bool moveToContent, float moveTime, int moveType)
	{
		Xls.SNSPostData data_byKey = Xls.SNSPostData.GetData_byKey(strPostKey);
		if (data_byKey != null)
		{
			CheckEvtAndAddPost(data_byKey.m_iIdx, moveToContent, moveTime, moveType);
		}
	}

	public void CheckEvtAndAddPost(int iIdx, bool moveToContent, float moveTime, int moveType)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_SWITCH_SNS_POST))
		{
			Xls.SNSPostData data_bySwitchIdx = Xls.SNSPostData.GetData_bySwitchIdx(iIdx);
			int iCheckEvtSwitch = data_bySwitchIdx.m_iCheckEvtSwitch;
			if (iCheckEvtSwitch == -1 || (iCheckEvtSwitch != -1 && GetEventSwitch(iCheckEvtSwitch) == 1))
			{
				SNSMenuPlus.AddInsertPostData(data_bySwitchIdx, moveToContent, moveTime, moveType);
			}
		}
	}

	private void AddPostCurPhase(int iPhase)
	{
		Xls.SNSPostData sNSPostData = null;
		int dataCount = Xls.SNSPostData.GetDataCount();
		List<Xls.SNSPostData> datas = Xls.SNSPostData.datas;
		for (int i = 0; i < dataCount; i++)
		{
			sNSPostData = datas[i];
			if (sNSPostData != null)
			{
				int iCheckEvtSwitch = sNSPostData.m_iCheckEvtSwitch;
				bool flag = iCheckEvtSwitch == -1 || (iCheckEvtSwitch != -1 && GetEventSwitch(iCheckEvtSwitch) == 1);
				if (sNSPostData.m_strIDSeq.Equals(m_strCurSequence) && sNSPostData.m_iPhase == iPhase && sNSPostData.m_iIsSelPost == 0 && flag && sNSPostData.m_iPostInitState == 0)
				{
					SetPostState(sNSPostData.m_iIdx, 1);
				}
			}
		}
	}

	public bool SetCharProfile(string strProfileKey, byte byValue, bool isPop = true)
	{
		Xls.Profiles data_byKey = Xls.Profiles.GetData_byKey(strProfileKey);
		if (data_byKey == null)
		{
			return false;
		}
		int iIdx = data_byKey.m_iIdx;
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return false;
		}
		sbyte charProfile = GetCharProfile(strProfileKey);
		if (charProfile == byValue)
		{
			return false;
		}
		bool flag = charProfile == 0 && byValue == 1;
		bool isNew = false;
		Set2BitSwitch(m_byArrCharProfile, iIdx, byValue);
		if (byValue == 1)
		{
			sbyte collProfile = GetCollProfile(strProfileKey);
			if (collProfile == 0 && byValue >= 1)
			{
				isNew = true;
			}
			SetCollProfile(strProfileKey, byValue);
		}
		if (flag)
		{
			AddCutProfileCnt();
			if (isPop)
			{
				GameMain.instance.StartCoroutine(ProfileGetPopup.Show(strProfileKey, isNew));
				return true;
			}
		}
		return false;
	}

	public sbyte GetCharProfile(string strProfileKey)
	{
		Xls.Profiles data_byKey = Xls.Profiles.GetData_byKey(strProfileKey);
		if (data_byKey == null)
		{
			return -1;
		}
		int iIdx = data_byKey.m_iIdx;
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCharProfile, iIdx);
	}

	public sbyte GetCharProfile(int switchIndex)
	{
		if (!BitCalc.CheckArrayIdx(switchIndex, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCharProfile, switchIndex);
	}

	public void SetCollProfile(string strProfileKey, byte byValue)
	{
		Xls.Profiles data_byKey = Xls.Profiles.GetData_byKey(strProfileKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			SetCollProfile(iIdx, byValue);
		}
	}

	private void SetCollProfile(int iIdx, byte byValue)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return;
		}
		sbyte collProfile = GetCollProfile(iIdx);
		if (collProfile < byValue)
		{
			Set2BitSwitch(m_byArrCollProfile, iIdx, byValue);
			if (byValue >= 1)
			{
				CheckAndAddProfileTrophy(iIdx);
			}
		}
	}

	public sbyte GetCollProfile(string strProfileKey)
	{
		Xls.Profiles data_byKey = Xls.Profiles.GetData_byKey(strProfileKey);
		if (data_byKey == null)
		{
			return -1;
		}
		int iIdx = data_byKey.m_iIdx;
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCollProfile, iIdx);
	}

	public sbyte GetCollProfile(int profileIndex)
	{
		if (!BitCalc.CheckArrayIdx(profileIndex, ConstGameSwitch.COUNT_CHAR_PROFILE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCollProfile, profileIndex);
	}

	private void CheckAndAddProfileTrophy(string strProfileKey)
	{
		Xls.Profiles data_byKey = Xls.Profiles.GetData_byKey(strProfileKey);
		if (data_byKey != null)
		{
			CheckAndAddProfileTrophy(data_byKey.m_iIdx);
		}
	}

	private void CheckAndAddProfileTrophy(int iIdx)
	{
		Xls.Profiles data_bySwitchIdx = Xls.Profiles.GetData_bySwitchIdx(iIdx);
		if (data_bySwitchIdx == null)
		{
			return;
		}
		int iCtgIdx = data_bySwitchIdx.m_iCtgIdx;
		string text = null;
		switch (iCtgIdx)
		{
		case 0:
			text = "trp_00068";
			break;
		case 1:
			text = "trp_00069";
			break;
		case 2:
			text = "trp_00070";
			break;
		case 3:
			text = "trp_00071";
			break;
		case 4:
			text = "trp_00072";
			break;
		case 5:
			text = "trp_00073";
			break;
		case 7:
			text = "trp_00074";
			break;
		}
		if (text == null || GetTrophyComplete(text))
		{
			return;
		}
		int dataCount = Xls.Profiles.GetDataCount();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < dataCount; i++)
		{
			Xls.Profiles data_byIdx = Xls.Profiles.GetData_byIdx(i);
			if (iCtgIdx == data_byIdx.m_iCtgIdx)
			{
				num++;
				sbyte collProfile = GetCollProfile(data_byIdx.m_strKey);
				if (collProfile >= 1)
				{
					num2++;
				}
			}
		}
		if (num2 >= num)
		{
			AddTrophyCnt(text);
		}
	}

	public void SetCollKeyword(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_ALL_COUNT))
		{
			sbyte collKeyword = GetCollKeyword(iIdx);
			if (collKeyword < byValue)
			{
				Set2BitSwitch(m_byArrCollKeyword, iIdx, byValue);
			}
		}
	}

	public void SetCollKeyword(string strKeywordKey, byte byValue)
	{
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
		if (data_byKey != null)
		{
			int iIndex = data_byKey.m_iIndex;
			SetCollKeyword(iIndex, byValue);
		}
	}

	public sbyte GetCollKeyword(string strKeywodKey)
	{
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywodKey);
		if (data_byKey == null)
		{
			return 0;
		}
		int iIndex = data_byKey.m_iIndex;
		return GetCollKeyword(iIndex);
	}

	public sbyte GetCollKeyword(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_ALL_COUNT))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCollKeyword, iIdx);
	}

	private void CheckAndAddKeywordTrophy()
	{
		string strTrophyKey = "trp_00075";
		if (GetTrophyComplete(strTrophyKey))
		{
			return;
		}
		int dataCount = Xls.CollKeyword.GetDataCount();
		int num = 0;
		Xls.CollKeyword collKeyword = null;
		for (int i = 0; i < dataCount; i++)
		{
			collKeyword = Xls.CollKeyword.GetData_byIdx(i);
			if (GetCollKeyword(collKeyword.m_strKey) >= 1)
			{
				num++;
			}
		}
		if (num >= dataCount)
		{
			AddTrophyCnt(strTrophyKey);
		}
	}

	public void TempSaveCurCharRelation(string strKeywordKey, string strCharKey)
	{
		m_strUseKeywordKey = strKeywordKey;
		m_strUseKeywordChar = strCharKey;
		m_isSetKeywordEventRelation = false;
		m_byUseKeywordBefRelation = (byte)GetCharRelation(strCharKey);
		m_iAddedKeywordRelaton = 0;
	}

	public void FreeForCurKeywordEvtVal()
	{
		m_strUseKeywordKey = null;
		m_strUseKeywordChar = null;
	}

	public string GetCurRunEventKeywordKey()
	{
		return m_strUseKeywordKey;
	}

	public void SetKeyUseSwitch(bool isReleaseKeywordSet = true)
	{
		if (m_strUseKeywordChar == null)
		{
			return;
		}
		if (!m_isSetKeywordEventRelation)
		{
			byte b = (byte)GetCharRelation(m_strUseKeywordChar);
			ConstGameSwitch.eKeywordUsing keywordUsing = ConstGameSwitch.eKeywordUsing.NONE;
			if (m_iAddedKeywordRelaton == 0)
			{
				keywordUsing = ConstGameSwitch.eKeywordUsing.NO_CHANGE;
			}
			else if (m_iAddedKeywordRelaton > 0)
			{
				keywordUsing = ConstGameSwitch.eKeywordUsing.UP;
			}
			else if (m_iAddedKeywordRelaton < 0)
			{
				keywordUsing = ConstGameSwitch.eKeywordUsing.DOWN;
			}
			SetCollKeywordUse(m_strUseKeywordChar, m_strUseKeywordKey, keywordUsing);
			m_isSetKeywordEventRelation = true;
		}
		if (isReleaseKeywordSet)
		{
			m_strUseKeywordChar = null;
			m_strUseKeywordKey = null;
		}
	}

	public string GetUseKeywordChar()
	{
		return m_strUseKeywordChar;
	}

	private int GetCollUseKeywordIdx(string strCharKey, string strKeyword)
	{
		if (strCharKey == null)
		{
			return -1;
		}
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
		if (data_byKey == null)
		{
			return -1;
		}
		int iUseIdx = data_byKey.m_iUseIdx;
		return GetCollUseKeywordIdx(iUseIdx, strKeyword);
	}

	private int GetCollUseKeywordIdx(int iCharIdx, string strKeywordKey)
	{
		if (strKeywordKey == null)
		{
			return -1;
		}
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
		if (data_byKey == null)
		{
			return -1;
		}
		int iIndex = data_byKey.m_iIndex;
		return iIndex * ConstGameSwitch.COUNT_KEYWORD_USE_CHAR_CNT + iCharIdx;
	}

	private void SetCollKeywordUse(string strCharKey, string strKeyword, ConstGameSwitch.eKeywordUsing keywordUsing)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(strCharKey, strKeyword);
		if (BitCalc.CheckArrayIdx(collUseKeywordIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			Set2BitSwitch(m_byArrCollUseKeyword, collUseKeywordIdx, (byte)keywordUsing);
		}
	}

	public byte GetCollKeywordUse(int iCharIdx, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(iCharIdx, strKeyword);
		return GetCollKeywordUse(collUseKeywordIdx);
	}

	public byte GetCollKeywordUse(string strCharKey, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(strCharKey, strKeyword);
		return GetCollKeywordUse(collUseKeywordIdx);
	}

	private byte GetCollKeywordUse(int iCollKeyUseIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCollKeyUseIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			return 0;
		}
		return Get2BitSwitch(m_byArrCollUseKeyword, iCollKeyUseIdx);
	}

	private void SetCollCutKeyword(string strCharKey, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(strCharKey, strKeyword);
		if (BitCalc.CheckArrayIdx(collUseKeywordIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			SetBitSwitch(m_byArrCollCutKeyword, collUseKeywordIdx, 1);
		}
	}

	public byte GetCollCutKeyword(int iCharIdx, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(iCharIdx, strKeyword);
		return GetCollCutKeyword(collUseKeywordIdx);
	}

	public byte GetCollCutKeyword(string strCharKey, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(strCharKey, strKeyword);
		return GetCollCutKeyword(collUseKeywordIdx);
	}

	private byte GetCollCutKeyword(int iCollKeyUseIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCollKeyUseIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			return 0;
		}
		return GetBitSwitch(m_byArrCollCutKeyword, iCollKeyUseIdx);
	}

	public void SetCollSelKeyword(string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(m_strRunKeywordCharKey, strKeyword);
		if (BitCalc.CheckArrayIdx(collUseKeywordIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			SetBitSwitch(m_byArrCollSelectKeyword, collUseKeywordIdx, 1);
		}
	}

	public byte GetCollSelKeyword(string strKeyword)
	{
		return GetCollSelKeyword(m_strRunKeywordCharKey, strKeyword);
	}

	public byte GetCollSelKeyword(string strCharKey, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(strCharKey, strKeyword);
		if (!BitCalc.CheckArrayIdx(collUseKeywordIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			return 0;
		}
		return GetBitSwitch(m_byArrCollSelectKeyword, collUseKeywordIdx);
	}

	public byte GetCollSelKeywod(int iCharIdx, string strKeyword)
	{
		int collUseKeywordIdx = GetCollUseKeywordIdx(iCharIdx, strKeyword);
		if (!BitCalc.CheckArrayIdx(collUseKeywordIdx, ConstGameSwitch.COUNT_KEYWORD_USE_ALL_CNT))
		{
			return 0;
		}
		return GetBitSwitch(m_byArrCollSelectKeyword, collUseKeywordIdx);
	}

	public int GetTutorialIdx(string strKey)
	{
		return Xls.TutorialListData.GetData_byKey(strKey)?.m_iTutorialIdx ?? (-1);
	}

	public void SetTutorial(string strKey, byte byValue)
	{
		int tutorialIdx = GetTutorialIdx(strKey);
		SetTutorial(tutorialIdx, byValue);
	}

	public void SetTutorial(int iTutoIdx, byte byValue)
	{
		SetBitSwitch(m_byArrCollTutorial, iTutoIdx, byValue);
	}

	public sbyte GetTutorial(string strKey)
	{
		int tutorialIdx = GetTutorialIdx(strKey);
		if (!BitCalc.CheckArrayIdx(tutorialIdx, ConstGameSwitch.COUNT_TUTORIAL_CNT))
		{
			return 1;
		}
		return GetTutorial(tutorialIdx);
	}

	public sbyte GetTutorial(int iTutoIdx)
	{
		if (!BitCalc.CheckArrayIdx(iTutoIdx, ConstGameSwitch.COUNT_TUTORIAL_CNT))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrCollTutorial, iTutoIdx);
	}

	public void SetCollImage(string strCollImgKey, byte byValue, bool isPop = false)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strCollImgKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			SetCollImage(iIdx, byValue, isPop);
		}
	}

	public void SetCollImage(int iIdx, byte byValue, bool isPop = false)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_IMAGE))
		{
			return;
		}
		sbyte collImage = GetCollImage(iIdx);
		if (collImage < byValue)
		{
			if (isPop && byValue == 1)
			{
				NoticeUIManager.ActiveNotice_S_SetKey(NoticeUIManager.NoticeType.Image, iIdx);
			}
			Set2BitSwitch(m_byArrCollImage, iIdx, byValue);
		}
	}

	public sbyte GetCollImage(string strCollImgKey)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strCollImgKey);
		if (data_byKey == null)
		{
			return 0;
		}
		int iIdx = data_byKey.m_iIdx;
		return GetCollImage(iIdx);
	}

	public sbyte GetCollImage(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_IMAGE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCollImage, iIdx);
	}

	public void SetCollSound(string strCollSoundKey, byte byValue, bool isPop = false)
	{
		Xls.CollSounds data_byKey = Xls.CollSounds.GetData_byKey(strCollSoundKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			SetCollSound(iIdx, byValue, isPop);
		}
	}

	public void SetCollSound(int iIdx, byte byValue, bool isPop = false)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_SOUND))
		{
			return;
		}
		sbyte collSound = GetCollSound(iIdx);
		if (collSound < byValue)
		{
			if (isPop && byValue == 1)
			{
				NoticeUIManager.ActiveNotice_S_SetKey(NoticeUIManager.NoticeType.Sound, iIdx);
			}
			Set2BitSwitch(m_byArrCollSound, iIdx, byValue);
		}
	}

	public sbyte GetCollSound(string strCOllSoundKey)
	{
		Xls.CollSounds data_byKey = Xls.CollSounds.GetData_byKey(strCOllSoundKey);
		if (data_byKey != null)
		{
			return 0;
		}
		int iIdx = data_byKey.m_iIdx;
		return GetCollSound(iIdx);
	}

	public sbyte GetCollSound(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_SOUND))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrCollSound, iIdx);
	}

	public List<SequenceInfo> GetSequencceList()
	{
		return m_lstSequenceInfo;
	}

	private SequenceInfo CreateFindSequenceInfo(int iSeq)
	{
		SequenceInfo sequenceInfo = GetSeqInfo(iSeq);
		if (sequenceInfo == null)
		{
			sequenceInfo = new SequenceInfo();
			sequenceInfo.m_iSeqIdx = iSeq;
			sequenceInfo.m_clKeyword = new List<KeywordSet>();
			m_lstSequenceInfo.Add(sequenceInfo);
		}
		return sequenceInfo;
	}

	private void FreeSequence()
	{
		if (m_lstSequenceInfo != null)
		{
			int count = m_lstSequenceInfo.Count;
			for (int i = 0; i < count; i++)
			{
				m_lstSequenceInfo[i].m_clKeyword = null;
			}
			m_lstSequenceInfo.Clear();
			m_lstSequenceInfo = null;
			m_lstGetKeywordByOrder.Clear();
		}
	}

	public SequenceInfo GetSeqInfo(int iSeq)
	{
		SequenceInfo result = null;
		if (m_lstSequenceInfo != null)
		{
			int count = m_lstSequenceInfo.Count;
			for (int i = 0; i < count; i++)
			{
				SequenceInfo sequenceInfo = m_lstSequenceInfo[i];
				if (iSeq == sequenceInfo.m_iSeqIdx)
				{
					result = sequenceInfo;
					break;
				}
			}
		}
		return result;
	}

	public List<KeywordSet> GetKeyword(int iSeq)
	{
		return GetSeqInfo(iSeq)?.m_clKeyword;
	}

	public int GetShowKeywordCntBySeq(int iSeq, bool isUsedCheck = true)
	{
		int num = 0;
		List<KeywordSet> keyword = GetKeyword(iSeq);
		if (keyword != null)
		{
			sbyte b = 0;
			sbyte b2 = 0;
			int count = keyword.Count;
			for (int i = 0; i < count; i++)
			{
				KeywordSet keywordSet = keyword[i];
				b = GetKeywordSeqState(iSeq, keywordSet.m_strIDKeyword);
				b2 = GetKeywordSeqUsed(iSeq, keywordSet.m_strIDKeyword);
				if (b > 0 && (!isUsedCheck || (isUsedCheck && b2 == 0)))
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetShowEventKeywordCntByCut()
	{
		int num = 0;
		for (int i = 0; i < m_iMustMaxCutKeywordCnt; i++)
		{
			Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(m_strGetCutKeyword[i]);
			if (data_byKey != null && data_byKey.m_iCtg == 0)
			{
				num++;
			}
		}
		return num;
	}

	public string[] GetCutKeywordList()
	{
		return m_strGetCutKeyword;
	}

	public void SetAllKeywordUsed(int iSeq = -1)
	{
		if (iSeq == -1)
		{
			iSeq = m_icursequence;
		}
		SequenceInfo seqInfo = GetSeqInfo(iSeq);
		List<KeywordSet> clKeyword = seqInfo.m_clKeyword;
		int count = clKeyword.Count;
		int num = 8;
		for (int i = 0; i < count; i++)
		{
			string strIDKeyword = clKeyword[i].m_strIDKeyword;
			if (GetKeywordSeqState(iSeq, strIDKeyword) > 0)
			{
				for (int j = 0; j < num; j++)
				{
					SetKeywordSeqUsed(iSeq, strIDKeyword, 1, j);
				}
			}
		}
	}

	private void InitKeywordSeqList(int iCurSeq, SequenceInfo seqInfo)
	{
		string strKeywordSet = Xls.SequenceData.GetData_bySwitchIdx(iCurSeq).m_strKeywordSet;
		if (seqInfo.m_clKeyword.Count > 0 || strKeywordSet.Equals(string.Empty))
		{
			return;
		}
		char[] array = strKeywordSet.ToCharArray();
		int num = array.Length;
		char[] array2 = new char[20];
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			if (array[i] == ',' || array[i] == '\0')
			{
				KeywordSet keywordSet = new KeywordSet();
				array2[num3++] = '\0';
				string strIDKeyword = (keywordSet.m_strIDKeyword = new string(array2).TrimEnd(default(char)));
				keywordSet.m_iKeywordIdx = num2++;
				seqInfo.m_clKeyword.Add(keywordSet);
				if (GetKeywordAllState(strIDKeyword) >= 1)
				{
					SetKeywordSeqState(iCurSeq, strIDKeyword, 1);
				}
				BitCalc.InitArray(array2);
				num3 = 0;
			}
			else
			{
				array2[num3++] = array[i];
			}
		}
	}

	private int GetKeywordSeqStateIdx(int iCurSeq, string strIDKeyword)
	{
		SequenceInfo seqInfo = GetSeqInfo(iCurSeq);
		if (seqInfo == null)
		{
			return -1;
		}
		int num = -1;
		int num2 = ConstGameSwitch.COUNT_KEYWORD_BY_SEQ * iCurSeq;
		int count = seqInfo.m_clKeyword.Count;
		for (int i = 0; i < count; i++)
		{
			KeywordSet keywordSet = seqInfo.m_clKeyword[i];
			if (keywordSet.m_strIDKeyword.Equals(strIDKeyword))
			{
				num = keywordSet.m_iKeywordIdx;
				break;
			}
		}
		if (num == -1)
		{
			return -1;
		}
		return num2 + num;
	}

	private int GetKeywordSeqIdxByChar(int iSeq, string strCharKey, string strIDKeyword)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetKeywordSeqIdxByChar(iSeq, charIdx, strIDKeyword);
	}

	private int GetKeywordSeqIdxByChar(int iSeq, int iCharIdx, string strIDKeyword)
	{
		SequenceInfo seqInfo = GetSeqInfo(iSeq);
		if (seqInfo == null)
		{
			return -1;
		}
		int num = -1;
		if (iCharIdx == -1)
		{
			return -1;
		}
		int num2 = ConstGameSwitch.COUNT_KEYWORD_SEQ_STATE * iCharIdx + ConstGameSwitch.COUNT_KEYWORD_BY_SEQ * iSeq;
		int count = seqInfo.m_clKeyword.Count;
		for (int i = 0; i < count; i++)
		{
			KeywordSet keywordSet = seqInfo.m_clKeyword[i];
			if (keywordSet.m_strIDKeyword.Equals(strIDKeyword))
			{
				num = keywordSet.m_iKeywordIdx;
				break;
			}
		}
		if (num == -1)
		{
			return -1;
		}
		return num2 + num;
	}

	public bool SetKeywordSeqState(int iSeq, string strIDKeyword, byte byValue)
	{
		bool flag = false;
		int keywordSeqStateIdx = GetKeywordSeqStateIdx(iSeq, strIDKeyword);
		sbyte keywordSeqState = GetKeywordSeqState(iSeq, strIDKeyword);
		if (keywordSeqState == byValue)
		{
			return false;
		}
		if (keywordSeqState >= 1 && byValue <= 1)
		{
			return false;
		}
		if (keywordSeqState <= 0 && byValue == 1)
		{
			flag = true;
		}
		bool result = SetMustGetCutKeywordState(strIDKeyword, byValue, isCheckGet: true);
		SetKeywordSeqState(keywordSeqStateIdx, byValue);
		if (flag)
		{
			AddCutKeywordCnt();
			if (keywordSeqState == 0)
			{
				Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strIDKeyword);
				if (data_byKey != null)
				{
					m_lstGetKeywordByOrder.Add(data_byKey.m_iIndex);
				}
			}
		}
		return result;
	}

	private void SetKeywordSeqState(int iIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_SEQ_STATE))
		{
			Set2BitSwitch(m_byArrKeywordSeqState, iIdx, byValue);
		}
	}

	public List<int> GetLstKewordByOrder()
	{
		return m_lstGetKeywordByOrder;
	}

	public sbyte GetKeywordSeqState(int iSeq, string strIDKeyword)
	{
		int keywordSeqStateIdx = GetKeywordSeqStateIdx(iSeq, strIDKeyword);
		return GetKeywordSeqState(keywordSeqStateIdx);
	}

	public sbyte GetKeywordSeqState(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_SEQ_STATE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrKeywordSeqState, iIdx);
	}

	public void SetKeywordSeqUsed(int iSeq, string strIDKeyword, byte byValue)
	{
		int keywordSeqIdxByChar = GetKeywordSeqIdxByChar(iSeq, m_strRunKeywordCharKey, strIDKeyword);
		if (BitCalc.CheckArrayIdx(keywordSeqIdxByChar, ConstGameSwitch.COUNT_KEYWORD_SEQ_BY_CHAR))
		{
			SetBitSwitch(m_byArrKeywordSeqUseKeyword, keywordSeqIdxByChar, byValue);
			byte bitSwitch = GetBitSwitch(m_byArrKeywordSeqUseKeywordBef, keywordSeqIdxByChar);
			if (bitSwitch != 1)
			{
				SetBitSwitch(m_byArrKeywordSeqUseKeywordBef, keywordSeqIdxByChar, byValue);
			}
		}
	}

	public void SetKeywordSeqUsed(int iSeq, string strIDKeyword, byte byValue, int iCharIdx)
	{
		int keywordSeqIdxByChar = GetKeywordSeqIdxByChar(iSeq, iCharIdx, strIDKeyword);
		if (BitCalc.CheckArrayIdx(keywordSeqIdxByChar, ConstGameSwitch.COUNT_KEYWORD_SEQ_BY_CHAR))
		{
			SetBitSwitch(m_byArrKeywordSeqUseKeyword, keywordSeqIdxByChar, byValue);
			byte bitSwitch = GetBitSwitch(m_byArrKeywordSeqUseKeywordBef, keywordSeqIdxByChar);
			if (bitSwitch != 1)
			{
				SetBitSwitch(m_byArrKeywordSeqUseKeywordBef, keywordSeqIdxByChar, byValue);
			}
		}
	}

	public sbyte GetKeywordSeqUsedBef(int iSeq, string strIDKeyword)
	{
		int keywordSeqIdxByChar = GetKeywordSeqIdxByChar(iSeq, m_strRunKeywordCharKey, strIDKeyword);
		if (!BitCalc.CheckArrayIdx(keywordSeqIdxByChar, ConstGameSwitch.COUNT_KEYWORD_SEQ_BY_CHAR))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrKeywordSeqUseKeywordBef, keywordSeqIdxByChar);
	}

	public sbyte GetKeywordSeqUsed(int iSeq, string strIDKeyword)
	{
		return GetKeywordSeqUsed(m_strRunKeywordCharKey, iSeq, strIDKeyword);
	}

	public sbyte GetKeywordSeqUsed(string strCharKey, int iSeq, string strIDKeyword)
	{
		int keywordSeqIdxByChar = GetKeywordSeqIdxByChar(iSeq, strCharKey, strIDKeyword);
		if (!BitCalc.CheckArrayIdx(keywordSeqIdxByChar, ConstGameSwitch.COUNT_KEYWORD_SEQ_BY_CHAR))
		{
			return -1;
		}
		return (sbyte)GetBitSwitch(m_byArrKeywordSeqUseKeyword, keywordSeqIdxByChar);
	}

	public bool SetKeywordAllState(string strIDKeyword, byte byValue, bool isSetPop = false, GameDefine.EventProc fpClosedPopup = null)
	{
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strIDKeyword);
		if (data_byKey == null)
		{
			return false;
		}
		int iIndex = data_byKey.m_iIndex;
		sbyte keywordAllState = GetKeywordAllState(strIDKeyword);
		if (keywordAllState >= 1 && byValue == 0)
		{
			return false;
		}
		if (keywordAllState == 0 && byValue == 2)
		{
			return false;
		}
		if (!BitCalc.CheckArrayIdx(iIndex, ConstGameSwitch.COUNT_KEYWORD_ALL_COUNT))
		{
			return false;
		}
		if (byValue != 2)
		{
			SetKeywordSeqState(GetCurSequence(), strIDKeyword, byValue);
		}
		if (keywordAllState == byValue)
		{
			return false;
		}
		bool flag = byValue == 1;
		if (flag)
		{
			if (m_strUseKeywordChar != null && m_strUseKeywordKey != null && !m_isSetKeywordEventRelation)
			{
				SetCollCutKeyword(m_strUseKeywordChar, m_strUseKeywordKey);
			}
			SetCollKeyword(strIDKeyword, byValue);
			if (isSetPop)
			{
				MainLoadThing.instance.StartCoroutine(KeywordGetPopupPlus.Show(strIDKeyword, fpClosedPopup));
			}
		}
		Set2BitSwitch(m_byArrKeywordAllState, iIndex, byValue);
		if (byValue >= 1)
		{
			CheckAndAddKeywordTrophy();
		}
		if (isSetPop || byValue == 1)
		{
		}
		return flag && isSetPop;
	}

	public sbyte GetKeywordAllState(string strIDKeyword)
	{
		Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strIDKeyword);
		if (data_byKey == null)
		{
			return -1;
		}
		int iIndex = data_byKey.m_iIndex;
		return GetKeywordAllState(iIndex);
	}

	public sbyte GetKeywordAllState(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_KEYWORD_ALL_COUNT))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrKeywordAllState, iIdx);
	}

	public void SetRunLowRealtionEvt(bool isSet)
	{
		m_isRunLowRelationEvt = isSet;
	}

	public bool GetRunLowRelationEvt()
	{
		return m_isRunLowRelationEvt;
	}

	public bool IsCheckRunLowRelationEvt(string strKeywordKey, string strCharKey)
	{
		bool result = false;
		if ((float)GetCharRelation(strCharKey) <= m_fEvtOccurLowRelation)
		{
			Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(strKeywordKey);
			if (data_byKey != null && data_byKey.m_strChkRelation != string.Empty)
			{
				string[] array = GameGlobalUtil.GetSeparateText(chSeparate: new char[1] { ',' }, strText: data_byKey.m_strChkRelation, isAddNullChar: false);
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					if (array[i] == strCharKey)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public void InitSaveRunKeyword(bool isAllInit = false)
	{
		m_strSaveRunKeyword = null;
		KeywordMenuPlus.SetRunKeywordPage(0);
		if (isAllInit)
		{
			KeywordMenuPlus.SetIsShowBackButton(isShow: false);
			KeywordMenuPlus.SetCharKey(-1);
		}
	}

	public void SetRunKeyword(string strKeyword)
	{
		m_strRunKeyword = strKeyword;
		if (strKeyword != null)
		{
			m_strSaveRunKeyword = strKeyword;
		}
	}

	public string GetRunKeyword()
	{
		return m_strRunKeyword;
	}

	public void SetRunKeywordCharKey(string strCharKey)
	{
		m_strRunKeywordCharKey = strCharKey;
	}

	public string GetRunKeywordCharKey()
	{
		return m_strRunKeywordCharKey;
	}

	public void SetRunKeywordGameOver(bool isRun)
	{
		m_isRunKeywordGameOver = isRun;
	}

	public bool GetRunKeywordGameOver()
	{
		return m_isRunKeywordGameOver;
	}

	public void SetRunKeywordEvt(bool isRun)
	{
		m_isRunKeywordEvt = isRun;
	}

	public bool GetRunKeywordEvt()
	{
		return m_isRunKeywordEvt;
	}

	public bool IsExistUsableKeyword(int iCharIdx)
	{
		int iSize = 8;
		if (!BitCalc.CheckArrayIdx(iCharIdx, iSize))
		{
			return false;
		}
		string strKey = Xls.CharData.GetData_bySwitchIdx(iCharIdx).m_strKey;
		return IsExistUsableKeyword(strKey);
	}

	public bool IsExistUsableKeyword(string strCharKey)
	{
		bool result = false;
		int count = m_lstGetKeywordByOrder.Count;
		string text = null;
		Xls.CollKeyword collKeyword = null;
		for (int i = 0; i < count; i++)
		{
			collKeyword = Xls.CollKeyword.GetData_bySwitchIdx(m_lstGetKeywordByOrder[i]);
			if (collKeyword != null)
			{
				text = collKeyword.m_strKey;
				if (GetKeywordSeqState(GetCurSequence(), text) > 0 && GetKeywordSeqUsed(strCharKey, GetCurSequence(), text) == 0)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void SetSoundSwitch(string strSoundKey, byte byValue, bool isPop = false)
	{
		Xls.CollSounds data_byKey = Xls.CollSounds.GetData_byKey(strSoundKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			SetSoundSwitch(iIdx, byValue, isPop);
		}
	}

	public void SetSoundSwitch(int iIdx, byte byValue, bool isPop = false)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_SOUND))
		{
			return;
		}
		sbyte soundSwitch = GetSoundSwitch(iIdx);
		if (soundSwitch < byValue)
		{
			if (byValue == 1)
			{
				SetCollSound(iIdx, byValue, isPop);
			}
			Set2BitSwitch(m_byArrSound, iIdx, byValue);
		}
	}

	public sbyte GetSoundSwitch(string strSoundKey)
	{
		Xls.CollSounds data_byKey = Xls.CollSounds.GetData_byKey(strSoundKey);
		if (data_byKey == null)
		{
			return 0;
		}
		return GetSoundSwitch(data_byKey.m_iIdx);
	}

	public sbyte GetSoundSwitch(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_SOUND))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrSound, iIdx);
	}

	public void SetImageSwitch(string strImageKey, byte byValue, bool isPop = false)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strImageKey);
		if (data_byKey != null)
		{
			int iIdx = data_byKey.m_iIdx;
			SetImageSwitch(iIdx, byValue, isPop);
		}
	}

	public void SetImageSwitch(int iIdx, byte byValue, bool isPop = false)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_IMAGE))
		{
			return;
		}
		sbyte imageSwitch = GetImageSwitch(iIdx);
		if (imageSwitch < byValue)
		{
			if (byValue == 1)
			{
				SetCollImage(iIdx, byValue, isPop);
			}
			Set2BitSwitch(m_byArrImage, iIdx, byValue);
		}
	}

	public sbyte GetImageSwitch(string strImageKey)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strImageKey);
		if (data_byKey == null)
		{
			return 0;
		}
		return GetImageSwitch(data_byKey.m_iIdx);
	}

	public sbyte GetImageSwitch(int iIdx)
	{
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_COLL_IMAGE))
		{
			return -1;
		}
		return (sbyte)Get2BitSwitch(m_byArrImage, iIdx);
	}

	public void SetCharCall(string strCharKey, byte byValue)
	{
		int cutCharCallIdx = GetCutCharCallIdx(m_iCurCutEffIdx, strCharKey);
		SetCutCharCall(cutCharCallIdx, byValue);
	}

	private void SetCutCharCall(int iCallIdx, byte byValue)
	{
		if (BitCalc.CheckArrayIdx(iCallIdx, ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX))
		{
			m_byArrCutCharCall[iCallIdx] = byValue;
		}
	}

	public void SetCutAllCharDoNotCall()
	{
		int cOUNT_CUT_CHAR_CALL_MAX = ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX;
		byte byValue = 0;
		for (int i = 0; i < cOUNT_CUT_CHAR_CALL_MAX; i++)
		{
			sbyte cutCharCall = GetCutCharCall(i);
			if (cutCharCall != 3 || cutCharCall != 4)
			{
				SetCutCharCall(i, byValue);
			}
		}
	}

	public sbyte GetCharCallState(string strCharKey)
	{
		int cutCharCallIdx = GetCutCharCallIdx(m_iCurCutEffIdx, strCharKey);
		return GetCutCharCall(cutCharCallIdx);
	}

	private sbyte GetCutCharCall(int iCutIdx, string strCharKey)
	{
		int cutCharCallIdx = GetCutCharCallIdx(iCutIdx, strCharKey);
		return GetCutCharCall(cutCharCallIdx);
	}

	private sbyte GetCutCharCall(int iCutIdx, int iCharIdx)
	{
		int cutCharCallIdx = GetCutCharCallIdx(iCutIdx, iCharIdx);
		return GetCutCharCall(cutCharCallIdx);
	}

	private sbyte GetCutCharCall(int iCallIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCallIdx, ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX))
		{
			return -1;
		}
		return (sbyte)m_byArrCutCharCall[iCallIdx];
	}

	public bool CheckCharCallable()
	{
		return GetCutCharCall(m_iCurCutEffIdx, m_iCurCallChar) == 1;
	}

	private int GetCutCharCallIdx(string strCutKey, string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCutCharCallIdx(strCutKey, charIdx);
	}

	private int GetCutCharCallIdx(string strCutKey, int iCharIdx)
	{
		int iCutIdx = -1;
		Xls.TalkCutSetting data_byKey = Xls.TalkCutSetting.GetData_byKey(strCutKey);
		if (data_byKey != null)
		{
			iCutIdx = data_byKey.m_iIdx;
		}
		return GetCutCharCallIdx(iCutIdx, iCharIdx);
	}

	private int GetCutCharCallIdx(int iCutIdx, string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCutCharCallIdx(iCutIdx, charIdx);
	}

	private int GetCutCharCallIdx(int iCutIdx, int iCharIdx)
	{
		int result = -1;
		if (iCutIdx != -1 && iCharIdx != -1)
		{
			result = iCutIdx * 8 + iCharIdx;
		}
		return result;
	}

	public void SetCallChar(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		SetCallChar(charIdx);
	}

	public void SetCallChar(int iCharIdx)
	{
		m_iCurCallChar = iCharIdx;
	}

	public int GetCallChar()
	{
		return m_iCurCallChar;
	}

	public void PlayCharRingingBell(bool isPlay, string strCharKey)
	{
		int iCallState = GetCharCallState(strCharKey);
		PlayCharRingingBellByState(isPlay, iCallState);
	}

	public void PlayCharRingingBellByState(bool isPlay, int iCallState)
	{
		string text = null;
		text = iCallState switch
		{
			1 => "Call_Callable", 
			2 => "Call_Talking", 
			_ => "Call_NotReceive", 
		};
		if (isPlay)
		{
			AudioManager.instance.PlayUISound(text);
		}
		else
		{
			AudioManager.instance.StopUISound(text);
		}
	}

	public void SetRecordFaterConti(string strConti)
	{
		m_strRecordFaterPlayConti = strConti;
	}

	public string GetRecordFaterContiName()
	{
		return m_strRecordFaterPlayConti;
	}

	public void SetReLoadSlotIdx(int iIdx)
	{
		m_iReLoadSlot = iIdx;
	}

	public int GetReLoadSlotIdx()
	{
		return m_iReLoadSlot;
	}

	public void SetSWRingtone(string strRingTone, bool isFromMenu = false)
	{
		Xls.CollSounds data_byKey = Xls.CollSounds.GetData_byKey(strRingTone);
		if (data_byKey != null)
		{
			m_iSWRingTone = data_byKey.m_iIdx;
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("CSND_001_KEY_NAME");
			bool flag = xlsProgramDefineStr != null && xlsProgramDefineStr == data_byKey.m_strKey;
			string xlsProgramDefineStr2 = GameGlobalUtil.GetXlsProgramDefineStr("RIGNTONE_CSND_001_EVT");
			if (xlsProgramDefineStr2 != null)
			{
				SetEventSwitch(int.Parse(xlsProgramDefineStr2), (byte)(flag ? 1u : 0u));
			}
			if (isFromMenu)
			{
				NoticeUIManager.ActiveNotice_S_SetKey(NoticeUIManager.NoticeType.Config_Sound, data_byKey.m_iIdx);
			}
		}
	}

	public int GetSwRingtone()
	{
		return m_iSWRingTone;
	}

	public void SetSWBackImage(string strBackImage, bool isFromMenu = false)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strBackImage);
		if (data_byKey != null)
		{
			m_iSWBackImage = data_byKey.m_iIdx;
			if (isFromMenu)
			{
				NoticeUIManager.ActiveNotice_S_SetKey(NoticeUIManager.NoticeType.Config_Image, data_byKey.m_iIdx);
			}
		}
	}

	public int GetSWBackImage()
	{
		return m_iSWBackImage;
	}

	public void SetAutoSNSText(int iIdx)
	{
		m_iAutoTextIdx = iIdx;
		int dataCount = Xls.WatchFaterAutoText.GetDataCount();
		for (int i = 0; i < dataCount; i++)
		{
			SetEventSwitch(Xls.WatchFaterAutoText.GetData_bySwitchIdx(i).m_iEvtSwitch, (byte)((i == iIdx) ? 1u : 0u));
		}
	}

	public int GetAutoSNSText()
	{
		return m_iAutoTextIdx;
	}

	private byte Get4BitSwitch(byte[] byArrSwitch, int iIdx)
	{
		byte b = byArrSwitch[iIdx / 2];
		return (byte)((b >> (1 - iIdx % 2 << 2)) & 0xF);
	}

	private void Set4BitSwitch(byte[] byArrSwitch, int iIdx, byte byValue)
	{
		byte b = (byte)(240 >> (iIdx % 2 << 2));
		byArrSwitch[iIdx / 2] = (byte)(byArrSwitch[iIdx / 2] & ~b);
		byArrSwitch[iIdx / 2] |= (byte)(byValue << (1 - iIdx % 2 << 2));
	}

	private byte Get2BitSwitch(byte[] byArrSwitch, int iIdx)
	{
		byte b = byArrSwitch[iIdx / 4];
		return (byte)((b >> (3 - iIdx % 4 << 1)) & 3);
	}

	private byte Get2BitSwitch(byte[,] byArrSwitch, int iType, int iIdx)
	{
		byte b = byArrSwitch[iType, iIdx / 4];
		return (byte)((b >> (3 - iIdx % 4 << 1)) & 3);
	}

	private void Set2BitSwitch(byte[] byArrSwitch, int iIdx, byte byValue)
	{
		byte b = (byte)(192 >> (iIdx % 4 << 1));
		byArrSwitch[iIdx / 4] = (byte)(byArrSwitch[iIdx / 4] & ~b);
		byArrSwitch[iIdx / 4] |= (byte)(byValue << (3 - iIdx % 4 << 1));
	}

	private void Set2BitSwitch(byte[,] byArrSwitch, int iType, int iIdx, byte byValue)
	{
		byte b = (byte)(192 >> (iIdx % 4 << 1));
		int num = iIdx / 4;
		byArrSwitch[iType, num] = (byte)(byArrSwitch[iType, num] & ~b);
		byArrSwitch[iType, num] |= (byte)(byValue << (3 - iIdx % 4 << 1));
	}

	private byte GetBitSwitch(byte[] byArrSwitch, int iIdx)
	{
		byte b = byArrSwitch[iIdx / 8];
		return (byte)((b >> 7 - iIdx % 8) & 1);
	}

	private void SetBitSwitch(byte[] byArrSwitch, int iIdx, byte byValue)
	{
		byte b = byArrSwitch[iIdx / 8];
		if (byValue == 0)
		{
			byArrSwitch[iIdx / 8] = (byte)(b & ~(1 << 7 - iIdx % 8));
		}
		else
		{
			byArrSwitch[iIdx / 8] = (byte)(b | (1 << 7 - iIdx % 8));
		}
	}

	public void SetRunEventObj(eEventRunType eRunType, string strObjName = null, string strSendEvent = null, bool isForEventSave = false)
	{
		m_eEventRunType = eRunType;
		SetStrEventObjName(strObjName);
		m_strPMSendEvent = strSendEvent;
		if (isForEventSave)
		{
			SetRunEventObjForEvtSave(eRunType, strObjName, strSendEvent);
		}
	}

	private void SetRunEventObjForEvtSave(eEventRunType eRunType, string strObjName = null, string strSendEvent = null)
	{
		m_isExistEventSaveObj = true;
		m_eEventRunTypeForSave = eRunType;
		m_strEventObjNameForSave = strObjName;
		m_strPMSendEventForSave = strSendEvent;
	}

	public string GetStrEventObjName()
	{
		return m_strEventObjName;
	}

	public void SetStrEventObjName(string strEventObjName)
	{
		m_strEventObjName = strEventObjName;
	}

	public void InitForSetUseEventSaveObj()
	{
		m_isExistEventSaveObj = false;
	}

	private eEventRunType GetEventRunType()
	{
		return (!m_isExistEventSaveObj) ? m_eEventRunType : m_eEventRunTypeForSave;
	}

	private string GetEventObjName()
	{
		return (!m_isExistEventSaveObj) ? GetStrEventObjName() : m_strEventObjNameForSave;
	}

	public string GetPMSendEvent()
	{
		return (!m_isExistEventSaveObj) ? m_strPMSendEvent : m_strPMSendEventForSave;
	}

	public void InitEventObjName()
	{
		m_strEventObjNameForSave = null;
		m_strEventObjName = null;
	}

	public void SetPMSendEventWhenSaveNotExist()
	{
		if (m_strPMSendEvent == null)
		{
			Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey("PM_EVENT_START_NOT_EXIST_SAVE");
			if (data_byKey != null)
			{
				m_strPMSendEvent = data_byKey.m_strTxt;
			}
		}
	}

	public void SetPMSendEvent(string strSendEvent)
	{
		m_strPMSendEvent = strSendEvent;
	}

	private void SaveEventSwitchData(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		int num = 0;
		BitCalc.FloatToByteNCO(m_fGameTime, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iMental, bySaveBuf, ref iOffset);
		num = 0;
		BitCalc.FloatArrToByteArrNCO(m_fArrPosX, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_MAX_CHAR_POS_X_ALL_CNT);
		num = 0;
		BitCalc.FloatArrToByteArrNCO(m_fArrPosY, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_MAX_CHAR_POS_Y_ALL_CNT);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrKeywordGroup, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_GROUP);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrReplyGroup, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_REPLY_GROUP);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrEvtSwitch, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_SWITCH_EVENT);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrSelectSwitch, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_SWITCH_SELECT);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrChrAnonymous, ref num, bySaveBuf, ref iOffset, 2);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrChrRelation, ref num, bySaveBuf, ref iOffset, 8);
		int num2 = ((m_listVoteRank != null) ? m_listVoteRank.Count : 0);
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < 8; i++)
		{
			if (i >= num2)
			{
				num3 = 0;
				num4 = 0;
				num5 = 0;
			}
			else
			{
				num3 = m_listVoteRank[i].m_iVoteCnt;
				num4 = m_listVoteRank[i].m_iCharIdx;
				num5 = m_listVoteRank[i].m_iRankNum;
			}
			BitCalc.IntToByteNCO(num3, bySaveBuf, ref iOffset);
			bySaveBuf[iOffset++] = (byte)num4;
			bySaveBuf[iOffset++] = (byte)num5;
		}
		SaveCharPartySwitch(bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iPartySelMax, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iPartySelIdx, bySaveBuf, ref iOffset);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrPostState, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.IntArrayToByteArrayNCO(m_iPostRetweetCnt, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrPostReply, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrPostKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrPostRetweet, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		BitCalc.IntToByteNCO(m_iMessageGetCount, bySaveBuf, ref iOffset);
		num = 0;
		BitCalc.IntArrayToByteArrayNCO(m_iArrMessageOrder, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_MESSENGER_ORDER);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCharProfile, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrKeywordAllState, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrKeywordSeqState, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_SEQ);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrKeywordSeqUseKeywordBef, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrKeywordSeqUseKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR);
		BitCalc.IntToByteNCO(GetCurSequence(), bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(GetCurPhase(), bySaveBuf, ref iOffset);
		int count = m_lstSequenceInfo.Count;
		BitCalc.IntToByteNCO(count, bySaveBuf, ref iOffset);
		for (int j = 0; j < count; j++)
		{
			BitCalc.IntToByteNCO(m_lstSequenceInfo[j].m_iSeqIdx, bySaveBuf, ref iOffset);
		}
		int num6 = ConstGameSwitch.COUNT_SEQUENCE_LIST - count;
		iOffset += num6 * SIZE_INT;
		int count2 = m_lstGetKeywordByOrder.Count;
		BitCalc.IntToByteNCO(count2, bySaveBuf, ref iOffset);
		for (int k = 0; k < count2; k++)
		{
			BitCalc.IntToByteNCO(m_lstGetKeywordByOrder[k], bySaveBuf, ref iOffset);
		}
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrChrRelationConti, ref num, bySaveBuf, ref iOffset, 21);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrImage, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrSound, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCutCharCall, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX);
		BitCalc.IntToByteNCO(m_iSWRingTone, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iSWBackImage, bySaveBuf, ref iOffset);
		bySaveBuf[iOffset++] = (byte)m_iAutoTextIdx;
		SaveCameraData(bySaveBuf, ref iOffset, iBefVer);
		SaveGameMainData(bySaveBuf, ref iOffset);
	}

	public void LoadEventSwitchData(byte[] byLoadBuf, ref int iOffset, int iBefVer, int iCurVer)
	{
		int num = 0;
		m_fGameTime = BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset);
		SetMental(BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset));
		num = 0;
		BitCalc.ByteArrToFloatArrNCO(byLoadBuf, ref iOffset, m_fArrPosX, ref num, ConstGameSwitch.COUNT_MAX_CHAR_POS_X_ALL_CNT);
		num = 0;
		BitCalc.ByteArrToFloatArrNCO(byLoadBuf, ref iOffset, m_fArrPosY, ref num, ConstGameSwitch.COUNT_MAX_CHAR_POS_Y_ALL_CNT);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrKeywordGroup, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_GROUP);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrReplyGroup, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_REPLY_GROUP);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrEvtSwitch, ref num, ConstGameSwitch.ARRLEN_SWITCH_EVENT);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrSelectSwitch, ref num, ConstGameSwitch.ARRLEN_SWITCH_SELECT);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrChrAnonymous, ref num, 2);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrChrRelation, ref num, 8);
		m_listVoteRank = new List<VoteRank>();
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < 8; i++)
		{
			num2 = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			num3 = byLoadBuf[iOffset++];
			num4 = byLoadBuf[iOffset++];
			AddVoteList(num3, num2, num4);
		}
		LoadCharPartySwitch(byLoadBuf, ref iOffset);
		m_iPartySelMax = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_iPartySelIdx = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrPostState, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteArrayToIntArrayNCO(byLoadBuf, ref iOffset, m_iPostRetweetCnt, ref num, ConstGameSwitch.COUNT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrPostReply, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrPostKeyword, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrPostRetweet, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_SNS_POST);
		m_iMessageGetCount = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		num = 0;
		BitCalc.ByteArrayToIntArrayNCO(byLoadBuf, ref iOffset, m_iArrMessageOrder, ref num, ConstGameSwitch.COUNT_MESSENGER_ORDER);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCharProfile, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrKeywordAllState, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrKeywordSeqState, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_SEQ);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrKeywordSeqUseKeywordBef, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrKeywordSeqUseKeyword, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR);
		int iValue = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		int iValue2 = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		int num5 = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		if (iBefVer != -1)
		{
			FreeSequence();
			if (m_lstSequenceInfo == null)
			{
				m_lstSequenceInfo = new List<SequenceInfo>();
			}
		}
		for (int j = 0; j < num5; j++)
		{
			int iSeq = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			CreateFindSequenceInfo(iSeq);
		}
		SetCurSequence(iValue, isAddPostPhase: false);
		SetCurPhase(iValue2, isAddPostPhase: false);
		int num6 = ConstGameSwitch.COUNT_SEQUENCE_LIST - num5;
		iOffset += num6 * SIZE_INT;
		if (iBefVer != -1)
		{
			m_lstGetKeywordByOrder.Clear();
		}
		int num7 = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		for (int k = 0; k < num7; k++)
		{
			int item = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_lstGetKeywordByOrder.Add(item);
		}
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrChrRelationConti, ref num, 21);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrImage, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrSound, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCutCharCall, ref num, ConstGameSwitch.COUNT_CUT_CHAR_CALL_MAX);
		m_iSWRingTone = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_iSWBackImage = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_iAutoTextIdx = byLoadBuf[iOffset++];
		LoadCameraData(byLoadBuf, ref iOffset);
		LoadGameMainData(byLoadBuf, ref iOffset);
	}

	public int GetSaveEventDataSize(bool isOnlyLoadFile = false)
	{
		int num = SIZE_INT;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		string text = ((!isOnlyLoadFile) ? EventEngine.m_strLoadedLevel : EventEngine.m_strNeedLoadLevel);
		if (text != null)
		{
			num += BitCalc.GetSizeStringToByte(text);
		}
		if (GetEventRunType() == eEventRunType.ENABLE_OBJ)
		{
			string eventObjName = GetEventObjName();
			string pMSendEvent = GetPMSendEvent();
			if (eventObjName != null)
			{
				num2 = SIZE_INT + BitCalc.GetSizeStringToByte(eventObjName);
				num3 = SIZE_INT + BitCalc.GetSizeStringToByte(pMSendEvent);
			}
		}
		num4 = SIZE_INT;
		if (m_strSaveRunKeyword != null)
		{
			num4 += BitCalc.GetSizeStringToByte(m_strSaveRunKeyword);
		}
		int num5 = num + GAME_SAVE_SWITCH_SIZE;
		num5 += SIZE_INT + SIZE_INT * m_lstGetKeywordByOrder.Count;
		num5 += 1 + num2 + num3;
		num5 += num4;
		num5 += SIZE_INT + SIZE_INT + 1;
		num5 += m_MainLoadThing.m_AudioManager.GetSavePlaySoundSize(isOnlyLoadFile);
		num5 += GetInvestSaveSize();
		num5 += GetSaveSizeCutEffData();
		num5++;
		num5 += EventCameraEffect.Instance.GetSaveDataSize(isOnlyLoadFile);
		EventEngine instance = EventEngine.GetInstance();
		num5 += instance.m_TalkChar.GetSizeSaveTalkChar(isOnlyLoadFile);
		return num5 + instance.m_EventObject.GetSizeSaveEvtObj(isOnlyLoadFile);
	}

	public void SaveConfigData(byte[] bySaveBuf, ref int iOffset)
	{
		bySaveBuf[iOffset++] = (byte)m_eSystemLanguage;
		bySaveBuf[iOffset++] = m_byTypingSpeed;
		BitCalc.BooleanToByteNCO(m_isVSyncOn, bySaveBuf, ref iOffset);
		bySaveBuf[iOffset++] = (byte)m_iAutoDelayStep;
		bySaveBuf[iOffset++] = (byte)m_eVoiceLang;
		bySaveBuf[iOffset++] = (byte)m_eOXType;
		bySaveBuf[iOffset++] = (byte)m_eScreenMode;
		bySaveBuf[iOffset++] = (byte)m_eResolution;
		bySaveBuf[iOffset++] = (byte)m_eController;
	}

	public void LoadConfigData(byte[] byLoadBuf, ref int iOffset)
	{
		m_eSystemLanguage = (SystemLanguage)byLoadBuf[iOffset++];
		m_byTypingSpeed = byLoadBuf[iOffset++];
		SetVSyncOn(BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset));
		byte autoDelayTime = byLoadBuf[iOffset++];
		SetAutoDelayTime(autoDelayTime);
		SetVoiceLang((ConstGameSwitch.eVoiceLang)byLoadBuf[iOffset++]);
		iOffset++;
		m_eScreenMode = (eScreenMode)byLoadBuf[iOffset++];
		m_eResolution = (eResolution)byLoadBuf[iOffset++];
		m_eController = (eControllerType)byLoadBuf[iOffset++];
		SetScreenMode(m_eScreenMode);
		SetResolution(m_eResolution);
		SetControllerType(m_eController);
	}

	public int GetConfigDataSize()
	{
		int num = 6;
		return num + 3;
	}

	private void PauseGameTime()
	{
		if (!m_isPause)
		{
			float time = Time.time;
			float num = time - m_fGameStartTime;
			m_fGamePlayTime += num;
			if (m_fGamePlayTime >= 3599999f)
			{
				m_fGamePlayTime = 3599999f;
			}
			m_isPause = true;
		}
	}

	private void ResumeGameTime()
	{
		if (m_isPause)
		{
			m_fGameStartTime = Time.time;
			m_isPause = false;
		}
	}

	private void SaveTotalGamePlayTime(byte[] bySaveBuf, ref int iOffset)
	{
		PauseGameTime();
		m_fTotalGamePlayTime += m_fGamePlayTime;
		if (m_fTotalGamePlayTime >= 3599999f)
		{
			m_fTotalGamePlayTime = 3599999f;
		}
		m_fGamePlayTime = 0f;
		m_fGameStartTime = Time.time;
		BitCalc.FloatToByteNCO(m_fTotalGamePlayTime, bySaveBuf, ref iOffset);
		ResumeGameTime();
	}

	private void LoadTotalGamePlayTime(byte[] byLoadBuf, ref int iOffset)
	{
		m_fTotalGamePlayTime = BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset);
		m_fGamePlayTime = 0f;
		m_fGameStartTime = Time.time;
		m_isPause = false;
	}

	public float GetTotalGamePlayTime()
	{
		return m_fTotalGamePlayTime;
	}

	public int GetSaveCollDataSize()
	{
		return COLL_SAVE_SWITCH_SIZE;
	}

	public void SetShowEnding(bool isShowEnding)
	{
		m_isEndingShow = isShowEnding;
	}

	public bool GetShowEnding()
	{
		return m_isEndingShow;
	}

	public void SaveCollData(byte[] bySaveBuf, ref int iOffset)
	{
		int num = 0;
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollImage, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollSound, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND);
		num = 0;
		BitCalc.IntArrayToByteArrayNCO(m_iArrTrophyCnt, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.COUNT_MAX_TROPHY_CNT);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrTrophyNew, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_TROPHY);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollProfile, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollUseKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollCutKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollSelectKeyword, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(m_byArrCollTutorial, ref num, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_TUTORIAL);
		SaveTotalGamePlayTime(bySaveBuf, ref iOffset);
		BitCalc.BooleanToByteNCO(m_isEndingShow, bySaveBuf, ref iOffset);
	}

	public void LoadCollData(byte[] byLoadBuf, ref int iOffset, int iBefVer, int iCurVer)
	{
		int num = 0;
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollImage, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_IMAGE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollSound, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_COLL_SOUND);
		num = 0;
		BitCalc.ByteArrayToIntArrayNCO(byLoadBuf, ref iOffset, m_iArrTrophyCnt, ref num, ConstGameSwitch.COUNT_MAX_TROPHY_CNT);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrTrophyNew, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_TROPHY);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollProfile, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_CHAR_PROFILE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollKeyword, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_ALL);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollUseKeyword, ref num, ConstGameSwitch.ARRLEN_2BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollCutKeyword, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollSelectKeyword, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_KEYWORD_USE);
		num = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrCollTutorial, ref num, ConstGameSwitch.ARRLEN_BIT_SWITCH_TUTORIAL);
		LoadTotalGamePlayTime(byLoadBuf, ref iOffset);
		if (iBefVer >= 34 || iBefVer == -1)
		{
			m_isEndingShow = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
		}
	}

	public void SaveGameDataAll(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		BitCalc.StringToByteWithSizeNCO(EventEngine.m_strLoadedLevel, bySaveBuf, ref iOffset);
		SaveCutEffData(bySaveBuf, ref iOffset);
		SaveEventRunData(bySaveBuf, ref iOffset, iBefVer);
		m_MainLoadThing.m_AudioManager.SavePlaySound(bySaveBuf, ref iOffset, iBefVer);
		SaveInvestObj(bySaveBuf, ref iOffset);
		EventEngine instance = EventEngine.GetInstance();
		instance.m_TalkChar.SaveTalkChar(bySaveBuf, ref iOffset, iBefVer);
		instance.m_EventObject.SaveEvtObj(bySaveBuf, ref iOffset);
	}

	public void LoadGameDataAll(byte[] byLoadBuf, ref int iOffset, int iBefVer, int iCurVer)
	{
		InitNonSaveData();
		if (iBefVer != -1)
		{
			EventEngine.m_strLoadedLevel = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
		}
		else
		{
			EventEngine.m_strNeedLoadLevel = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
		}
		LoadCutEffData(byLoadBuf, ref iOffset);
		LoadEventRunData(byLoadBuf, ref iOffset, iBefVer, iCurVer);
		m_MainLoadThing.m_AudioManager.LoadPlaySound(byLoadBuf, ref iOffset, isInit: true);
		LoadInvestObj(byLoadBuf, ref iOffset);
		EventEngine instance = EventEngine.GetInstance();
		instance.m_TalkChar.LoadTalkChar(byLoadBuf, ref iOffset);
		instance.m_EventObject.LoadEvtObj(byLoadBuf, ref iOffset, iBefVer, iCurVer);
		if (GetStrEventObjName() == null)
		{
			Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey("RUN_HANDLOAD_CUTEFF");
			if (data_byKey != null)
			{
				m_eEventRunType = eEventRunType.ENABLE_OBJ;
				SetStrEventObjName(data_byKey.m_strTxt);
				m_strPMSendEvent = null;
			}
		}
	}

	public void SaveEventRunData(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		SaveEventSwitchData(bySaveBuf, ref iOffset, iBefVer);
		eEventRunType eEventRunType = eEventRunType.NONE;
		eEventRunType = ((iBefVer == -1) ? GetEventRunType() : m_eEventRunType);
		bySaveBuf[iOffset++] = (byte)eEventRunType;
		if (eEventRunType == eEventRunType.ENABLE_OBJ)
		{
			if (iBefVer != -1)
			{
				BitCalc.StringToByteWithSizeNCO(m_strEventObjName, bySaveBuf, ref iOffset);
				BitCalc.StringToByteWithSizeNCO(m_strPMSendEvent, bySaveBuf, ref iOffset);
			}
			else
			{
				BitCalc.StringToByteWithSizeNCO(GetEventObjName(), bySaveBuf, ref iOffset);
				BitCalc.StringToByteWithSizeNCO(GetPMSendEvent(), bySaveBuf, ref iOffset);
			}
		}
		m_isExistEventSaveObj = false;
		BitCalc.StringToByteWithSizeNCO(m_strSaveRunKeyword, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(KeywordMenuPlus.GetCharKeyIdx(), bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(KeywordMenuPlus.GetRunKeywordPage(), bySaveBuf, ref iOffset);
		BitCalc.BooleanToByteNCO(KeywordMenuPlus.GetIsShowBackButton(), bySaveBuf, ref iOffset);
		bySaveBuf[iOffset++] = (byte)GetTutoMenuState();
		EventCameraEffect.Instance.SaveCameraEfffectState(bySaveBuf, ref iOffset, iBefVer);
	}

	public void SetLoadEventData(bool isLoad)
	{
		m_isLoadEventData = isLoad;
	}

	public bool GetLoadEventData()
	{
		return m_isLoadEventData;
	}

	public void LoadEventRunData(byte[] byLoadBuf, ref int iOffset, int iBefVer, int iCurVer)
	{
		SetLoadEventData(isLoad: true);
		LoadEventSwitchData(byLoadBuf, ref iOffset, iBefVer, iCurVer);
		m_eEventRunType = (eEventRunType)byLoadBuf[iOffset++];
		if (m_eEventRunType == eEventRunType.ENABLE_OBJ)
		{
			SetStrEventObjName(BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset));
			m_strPMSendEvent = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
		}
		m_strRunKeyword = (m_strSaveRunKeyword = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset));
		int num = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		KeywordMenuPlus.SetCharKey(num);
		SetRunKeywordCharKey((num == -1) ? null : Xls.CharData.GetData_bySwitchIdx(num).m_strKey);
		KeywordMenuPlus.SetRunKeywordPage(BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset));
		m_isNeedLoadKeywordChar = m_strRunKeyword != null && m_strRunKeyword != string.Empty;
		KeywordMenuPlus.SetIsShowBackButton(BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset));
		SetTutoMenuObj(byLoadBuf[iOffset++]);
		EventCameraEffect.Instance.LoadCameraEfffectState(byLoadBuf, ref iOffset);
	}

	public void SetShareVideoRecord(bool isOn)
	{
	}

	public void SetCharAnonymous(string strCharKey, int iVal)
	{
		int charIdx = GetCharIdx(strCharKey);
		if (charIdx >= 0 && charIdx < 8)
		{
			SetBitSwitch(m_byArrChrAnonymous, charIdx, (byte)iVal);
		}
	}

	public bool GetCharAnonymous(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCharAnonymous(charIdx);
	}

	public bool GetCharAnonymous(int iCharIdx)
	{
		if (iCharIdx < 0 || iCharIdx >= 8)
		{
			return false;
		}
		return GetBitSwitch(m_byArrChrAnonymous, iCharIdx).Equals(1);
	}

	private void SaveCharPartySwitch(byte[] bySaveBuf, ref int iOffset)
	{
		int num = 8;
		int num2 = 40;
		for (int i = 0; i < num; i++)
		{
			bySaveBuf[iOffset++] = m_clArrCharPartySet[i].m_byState;
			bySaveBuf[iOffset++] = (byte)m_clArrCharPartySet[i].m_iIconShow;
			bySaveBuf[iOffset++] = (byte)m_clArrCharPartySet[i].m_iSize;
			bySaveBuf[iOffset++] = (byte)m_clArrCharPartySet[i].m_iDir;
			BitCalc.FloatToByteNCO(m_clArrCharPartySet[i].m_fPosX, bySaveBuf, ref iOffset);
			bySaveBuf[iOffset++] = (byte)m_clArrCharPartySet[i].m_iOrderIdx;
			string strMot = m_clArrCharPartySet[i].m_strMot;
			int num3 = iOffset;
			BitCalc.StringToByteWithSizeNCO(strMot, bySaveBuf, ref iOffset);
			iOffset = num3 + num2;
		}
	}

	private void LoadCharPartySwitch(byte[] byLoadBuf, ref int iOffset)
	{
		m_listPartySet.Clear();
		int num = 8;
		int num2 = 40;
		for (int i = 0; i < num; i++)
		{
			m_clArrCharPartySet[i].m_byState = byLoadBuf[iOffset++];
			m_clArrCharPartySet[i].m_iIconShow = byLoadBuf[iOffset++];
			m_clArrCharPartySet[i].m_iSize = byLoadBuf[iOffset++];
			m_clArrCharPartySet[i].m_iDir = byLoadBuf[iOffset++];
			m_clArrCharPartySet[i].m_fPosX = BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset);
			m_clArrCharPartySet[i].m_iOrderIdx = byLoadBuf[iOffset++];
			int num3 = iOffset;
			m_clArrCharPartySet[i].m_strMot = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			iOffset += num2 - (iOffset - num3);
			Xls.TalkCutChrSetting talkCutSetting = GetTalkCutSetting(m_clArrCharPartySet[i].m_strCharKey);
			if (m_clArrCharPartySet[i].m_byState == 1 && talkCutSetting != null && talkCutSetting.m_iIconShow == 1)
			{
				m_listPartySet.Add(m_clArrCharPartySet[i]);
			}
		}
	}

	public void SetTutoMenuObj(int iStateIdx)
	{
		GameMain instance = GameMain.instance;
		if (m_iTutoState != iStateIdx)
		{
			if (instance != null && instance.m_gameMainMenu != null)
			{
				instance.m_gameMainMenu.SetTutoObj(iStateIdx);
			}
			m_iTutoState = iStateIdx;
		}
	}

	public int GetTutoMenuState()
	{
		return m_iTutoState;
	}

	public int GetSaveSizeCutEffData()
	{
		return 1 + SIZE_INT * 4 + 8 + 1 + ConstGameSwitch.ARRLEN_BIT_SWITCH_MUST_GET_CUT_KEYWORD;
	}

	public void SaveCutEffData(byte[] bySaveBuf, ref int iOffset)
	{
		BitCalc.IntToByteNCO(m_iCurCutEffIdx, bySaveBuf, ref iOffset);
		BitCalc.BooleanToByteNCO(m_isGetAllKeywordPopByCut, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iCutKeywordCnt, bySaveBuf, ref iOffset);
		BitCalc.IntToByteNCO(m_iCutProfileCnt, bySaveBuf, ref iOffset);
		Buffer.BlockCopy(m_byCutStartChrRelation, 0, bySaveBuf, iOffset, 8);
		iOffset += 8;
		BitCalc.IntToByteNCO(m_iCutStartMental, bySaveBuf, ref iOffset);
		BitCalc.BooleanToByteNCO(m_isCutConsiderOn, bySaveBuf, ref iOffset);
		int iSrcOffset = 0;
		BitCalc.ByteToByteNCO(m_byArrGetCutKeyword, ref iSrcOffset, bySaveBuf, ref iOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_MUST_GET_CUT_KEYWORD);
	}

	public void LoadCutEffData(byte[] byLoadBuf, ref int iOffset)
	{
		int curCutIdx = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		SetCurCutIdx(curCutIdx);
		m_isGetAllKeywordPopByCut = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
		m_iCutKeywordCnt = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_iCutProfileCnt = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		Buffer.BlockCopy(byLoadBuf, iOffset, m_byCutStartChrRelation, 0, 8);
		iOffset += 8;
		m_iCutStartMental = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_isCutConsiderOn = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
		int iDstOffset = 0;
		BitCalc.ByteToByteNCO(byLoadBuf, ref iOffset, m_byArrGetCutKeyword, ref iDstOffset, ConstGameSwitch.ARRLEN_BIT_SWITCH_MUST_GET_CUT_KEYWORD);
		CheckGetCutKeywordCount();
	}

	public void StartCutEff(string strCutID)
	{
		Xls.TalkCutSetting data_byKey = Xls.TalkCutSetting.GetData_byKey(strCutID);
		if (data_byKey != null)
		{
			SetCurCutIdx(data_byKey.m_iIdx);
		}
		SetPartyOrderIdx(isResetPartyData: true);
	}

	public void SetCurCutIdx(int iCutIdx)
	{
		if (m_iCurCutEffIdx == iCutIdx)
		{
			return;
		}
		SetAllKeywordPopByCut(isShowPop: false);
		SetCutConsider(isOn: false);
		m_iMustMaxCutKeywordCnt = 0;
		m_iMustGetCutKeywordCnt = 0;
		if (iCutIdx != -1)
		{
			m_iCurCutEffIdx = iCutIdx;
		}
		BitCalc.InitArray(m_byArrGetCutKeyword, 0);
		if (iCutIdx == -1)
		{
			return;
		}
		Xls.TalkCutSetting data_bySwitchIdx = Xls.TalkCutSetting.GetData_bySwitchIdx(iCutIdx);
		if (data_bySwitchIdx == null)
		{
			return;
		}
		int num = 0;
		char[] array = data_bySwitchIdx.m_strKeywordSet.ToCharArray();
		char[] array2 = new char[20];
		int num2 = array.Length;
		int num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			if (array[i] == ',' || array[i] == '\0')
			{
				array2[num3++] = '\0';
				num3 = 0;
				m_strGetCutKeyword[num++] = new string(array2).TrimEnd(default(char));
			}
			else
			{
				array2[num3++] = array[i];
			}
		}
		m_iMustMaxCutKeywordCnt = num;
		for (int j = num; j < ConstGameSwitch.COUNT_MAX_MUST_GET_CUT_KEYWORD; j++)
		{
			m_strGetCutKeyword[j++] = null;
		}
	}

	public void SetAllKeywordPopByCut(bool isShowPop)
	{
		m_isGetAllKeywordPopByCut = isShowPop;
	}

	public bool GetAllKeywordPopByCut()
	{
		return m_isGetAllKeywordPopByCut;
	}

	public void InitCutEff()
	{
		SetCurCutIdx(-1);
	}

	public int GetCurCutIdx()
	{
		return m_iCurCutEffIdx;
	}

	public bool CheckCurCutID(string strCutID)
	{
		Xls.TalkCutSetting data_byKey = Xls.TalkCutSetting.GetData_byKey(strCutID);
		if (data_byKey == null)
		{
			return false;
		}
		return data_byKey.m_iIdx == m_iCurCutEffIdx;
	}

	private bool SetMustGetCutKeywordState(string strKeywordKey, byte byValue, bool isCheckGet = false)
	{
		if (byValue != 1)
		{
			return false;
		}
		bool result = false;
		int iIdx = -1;
		for (int i = 0; i < m_iMustMaxCutKeywordCnt; i++)
		{
			if (strKeywordKey == m_strGetCutKeyword[i])
			{
				iIdx = i;
				break;
			}
		}
		if (!BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MAX_MUST_GET_CUT_KEYWORD))
		{
			return result;
		}
		int mustGetKeywordCnt = GetMustGetKeywordCnt();
		SetBitSwitch(m_byArrGetCutKeyword, iIdx, byValue);
		if (isCheckGet)
		{
			CheckGetCutKeywordCount();
		}
		int mustGetKeywordCnt2 = GetMustGetKeywordCnt();
		return mustGetKeywordCnt != mustGetKeywordCnt2;
	}

	private sbyte GetMustGetCutKeyword(string strKeywordKey)
	{
		sbyte result = -1;
		int iIdx = -1;
		for (int i = 0; i < m_iMustMaxCutKeywordCnt; i++)
		{
			if (strKeywordKey == m_strGetCutKeyword[i])
			{
				iIdx = i;
				break;
			}
		}
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_MAX_MUST_GET_CUT_KEYWORD))
		{
			result = (sbyte)GetBitSwitch(m_byArrGetCutKeyword, iIdx);
		}
		return result;
	}

	private void CheckGetCutKeywordCount()
	{
		int num = 0;
		for (int i = 0; i < m_iMustMaxCutKeywordCnt; i++)
		{
			if (GetBitSwitch(m_byArrGetCutKeyword, i) == 1)
			{
				num++;
			}
		}
		m_iMustGetCutKeywordCnt = num;
		if (m_iMustMaxCutKeywordCnt != 0 && m_iMustMaxCutKeywordCnt == m_iMustGetCutKeywordCnt)
		{
			SetCutConsider(isOn: true);
		}
	}

	public void SetCutConsider(bool isOn)
	{
		m_isCutConsiderOn = isOn;
	}

	public bool GetCutConsider()
	{
		return m_isCutConsiderOn;
	}

	public int GetMustMaxKeywordCnt()
	{
		return m_iMustMaxCutKeywordCnt;
	}

	public int GetMustGetKeywordCnt()
	{
		return m_iMustGetCutKeywordCnt;
	}

	public void AddCutKeywordCnt()
	{
		m_iCutKeywordCnt++;
	}

	public void AddCutProfileCnt()
	{
		m_iCutProfileCnt++;
	}

	public int GetCutKeywordCnt()
	{
		return m_iCutKeywordCnt;
	}

	public int GetCutProfileCnt()
	{
		return m_iCutProfileCnt;
	}

	public int GetCutStartMental()
	{
		return m_iCutStartMental;
	}

	public int GetCutStartRelation(string strCharKey)
	{
		int charIdx = GetCharIdx(strCharKey);
		return GetCutStartRelation(charIdx);
	}

	public int GetCutStartRelation(int iCharIdx)
	{
		if (!BitCalc.CheckArrayIdx(iCharIdx, 8))
		{
			return -1;
		}
		return m_byCutStartChrRelation[iCharIdx];
	}

	public byte[] GetCutStartRelation()
	{
		return m_byCutStartChrRelation;
	}

	private void InitGamePadSubmitCancelButton()
	{
		eButType oXType = eButType.XEnter;
		SetOXType(oXType);
	}

	public void InitOption()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		SetOptLang(SteamApps.GetCurrentGameLanguage() switch
		{
			"koreana" => SystemLanguage.Korean, 
			"japanese" => SystemLanguage.Japanese, 
			"schinese" => SystemLanguage.ChineseSimplified, 
			"tchinese" => SystemLanguage.ChineseTraditional, 
			_ => SystemLanguage.English, 
		});
		ConstGameSwitch.eVoiceLang eVoiceLang = ConstGameSwitch.eVoiceLang.KOR;
		SystemLanguage curSubtitleLanguage = GetCurSubtitleLanguage();
		eVoiceLang = ((curSubtitleLanguage == SystemLanguage.Japanese) ? ConstGameSwitch.eVoiceLang.JPN : ConstGameSwitch.eVoiceLang.KOR);
		SetVoiceLang(eVoiceLang);
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("OPT_TEXT_TYPING");
		if (xlsProgramDefineStr != null)
		{
			SetTypingEff((byte)int.Parse(xlsProgramDefineStr));
		}
		string xlsProgramDefineStr2 = GameGlobalUtil.GetXlsProgramDefineStr("OPT_AUTO_DELAY");
		if (xlsProgramDefineStr2 != null)
		{
			SetAutoDelayTime(int.Parse(xlsProgramDefineStr2));
		}
		InitGamePadSubmitCancelButton();
		SetUIButType(eUIButType.KEYMOUSE);
	}

	public void SetOptLang(SystemLanguage sysLang, GameDefine.EventProc eventFinished = null)
	{
		m_eSystemLanguage = sysLang;
	}

	public SystemLanguage GetCurLanguage()
	{
		return m_eSystemLanguage;
	}

	public SystemLanguage GetCurSubtitleLanguage()
	{
		SystemLanguage systemLanguage = SystemLanguage.English;
		switch (m_eSystemLanguage)
		{
		case SystemLanguage.Japanese:
		case SystemLanguage.Korean:
			return m_eSystemLanguage;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
			return SystemLanguage.ChineseSimplified;
		case SystemLanguage.ChineseTraditional:
			return SystemLanguage.ChineseTraditional;
		default:
			return SystemLanguage.English;
		}
	}

	public void SetVoiceLang(ConstGameSwitch.eVoiceLang eLang)
	{
		m_eVoiceLang = eLang;
	}

	public ConstGameSwitch.eVoiceLang GetVoiceLang()
	{
		return m_eVoiceLang;
	}

	public void SetTypingEff(byte bySpeed)
	{
		if (BitCalc.CheckArrayIdx(bySpeed, 5))
		{
			m_byTypingSpeed = bySpeed;
		}
	}

	public byte GetTypingEff()
	{
		return m_byTypingSpeed;
	}

	public void SetVSyncOn(bool isOn)
	{
		m_isVSyncOn = isOn;
		if (isOn)
		{
			QualitySettings.vSyncCount = 2;
			return;
		}
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}

	public bool GetVSyncOn()
	{
		return m_isVSyncOn;
	}

	public void SetAutoDelayTime(int iStep)
	{
		if (iStep > 4)
		{
			iStep = 4;
		}
		m_iAutoDelayStep = iStep;
		m_fAutoDelayTime = (float)iStep * UNIT_DELAY_TIME;
	}

	public float GetAutoDelayTime()
	{
		return m_fAutoDelayTime;
	}

	public void CheckShowSNSButton()
	{
		if (!m_isCheckSNSKeywordBut)
		{
			return;
		}
		m_isShowBrightSNSBut = false;
		List<Xls.SNSPostData> datas = Xls.SNSPostData.datas;
		int count = datas.Count;
		Xls.SNSPostData sNSPostData = null;
		Xls.SequenceData sequenceData = null;
		int curPhase = GetCurPhase();
		for (int i = 0; i < count; i++)
		{
			sNSPostData = datas[i];
			sequenceData = Xls.SequenceData.GetData_byKey(sNSPostData.m_strIDSeq);
			if (sequenceData != null && sequenceData.m_iIdx == GetCurSequence() && sNSPostData.m_iPhase == curPhase)
			{
				if (GetPostState(sNSPostData.m_strID) == 0)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(sNSPostData.m_strIDReply) && sNSPostData.m_iIsSelPost == 0 && GetPostReply(sNSPostData.m_iIdx) == 0)
				{
					if (sNSPostData.m_iReplyGroup < 0 || GetReplyGroup(sNSPostData.m_iReplyGroup) == 0)
					{
						m_isShowBrightSNSBut = true;
					}
				}
				else if (!string.IsNullOrEmpty(sNSPostData.m_strIDKeyword) && GetKeywordAllState(sNSPostData.m_strIDKeyword) == 0 && (sNSPostData.m_iGroupKeyword < 0 || GetKeywordGroup(sNSPostData.m_iGroupKeyword) == 0))
				{
					m_isShowBrightSNSBut = true;
				}
			}
			if (m_isShowBrightSNSBut)
			{
				break;
			}
		}
		SetIsCheckSNSKeywordBut(isSet: false);
	}

	public void SetIsCheckSNSKeywordBut(bool isSet)
	{
		m_isCheckSNSKeywordBut = isSet;
	}

	public bool IsSNSButtonBright()
	{
		return m_isShowBrightSNSBut;
	}

	public void SetTrickObjTime(int iAMPM, int iH, int iM)
	{
		m_iTrickObjAMPM = iAMPM;
		m_iTrickObjH = iH;
		m_iTrickObjM = iM;
	}

	public void GetTrickObjTime(ref int iAMPM, ref int iH, ref int iM)
	{
		iAMPM = m_iTrickObjAMPM;
		iH = m_iTrickObjH;
		iM = m_iTrickObjM;
	}
}
