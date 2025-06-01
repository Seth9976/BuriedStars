using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace GameEvent;

public class TalkChar
{
	public class TalkCharUnit
	{
		public string m_strCharKey;

		public GameObject m_goChar;

		public int m_iDir;

		public int m_iPosition;

		public int m_iSize;

		public int m_iTmpPosition;

		public string m_strMotKey;

		public bool m_isBlackOn;

		public bool m_isSetLayer;

		public bool m_isSetPos;

		public int m_iMoveSpeedType;

		public float m_fMoveTime;

		public float m_fMovePassTime;

		public Vector3 m_vecMoveDest;

		public Vector3 m_vecMoveStart;

		public Vector2 m_vecStartPos;

		public Vector2 m_vecDestPos;

		public bool m_isSetSkipPos;

		public Vector2 m_vecSkipPos;

		public int m_iZoomSpeedType;

		public float m_fZoomTime;

		public float m_fZoomPassTime;

		public Vector3 m_vecZoomDest;

		public Vector3 m_vecZoomStart;

		public Vector3 m_vecZoomPosDest;

		public Vector3 m_vecZoomPosStart;

		public int m_iRotateType;

		public float m_fRotateTime;

		public float m_fRotatePassTime;

		public float m_fRotateDestZAngle;

		public float m_fRotateStartZAngle;

		public CharAnimatorPlay m_clCharAnimatorPlay;

		public bool m_isSetColor;

		public int m_iSetColor = 16777215;

		public Color m_colorSet = Color.white;

		public Color m_colorBefore = Color.white;

		public Color m_colorCur = Color.white;

		public float m_fColorTime;

		public float m_fColorPassTime;
	}

	private enum CHAR_DIR
	{
		LEFT,
		RIGHT
	}

	private string FOR_CHAR_POS_CALC = string.Empty;

	private GameObject m_goForCharPosCalc;

	private const int CNT_LOAD_CHR = 5;

	private const float CHR_MOVE_TIME = 1f;

	private const float CHR_MOVE_FIX_VALUE = 999f;

	private EventEngine m_EventEngine;

	private GameSwitch m_GameSwitch;

	private Camera m_MainCam;

	private Transform m_tfEventCanvas;

	private List<ResChar> m_listResChar;

	private static int MAX_CHR_POSITION_CNT = 7;

	private static int MAX_CHR_SIZE_CNT = 5;

	private static int CHR_SCREEN_Z_POS = 1;

	private List<TalkCharUnit> m_listTalkChar;

	private GameObject[] m_goTalkMark = new GameObject[8];

	private MarkerTalk[] m_clMarkerTalk = new MarkerTalk[8];

	private Animator[] m_animTalkMark = new Animator[8];

	private Button[] m_butTalkBack = new Button[8];

	private Button[] m_goTalkOKButton = new Button[8];

	private GameDefine.eAnimChangeState[] m_eMarkButChgState = new GameDefine.eAnimChangeState[8];

	private bool m_isShowPartyChar;

	private bool m_isAllCharLoad;

	private string m_strLoadAsyncChar;

	private bool[] m_isArrSetShowChar = new bool[8];

	private static int OUT_TYPE_LEFT;

	private static int OUT_TYPE_RIGHT = 1;

	private static int OUT_TYPE_BOTHSIDE = 2;

	private static int OUT_TYPE_CENTER_ALONE = 3;

	private static int CHAR_POS_LEFT_OUT = 5;

	private static int CHAR_POS_RIGHT_OUT = 6;

	private static string STR_NONE_MOT = "none";

	private GameObject m_goAssetBundleIconObj;

	public const string c_CharAssetPath = "character/";

	public const string c_PreCharName = "character_";

	private ResChar m_loadResChar;

	private bool m_isCreateCharComp;

	private TalkCharUnit m_tmpTalkChar;

	private bool m_isShowTalkIcon = true;

	private const float CHAR_KEYWORD_MOVE_TIME = 0.2f;

	private string m_strKeywordMenuMot = string.Empty;

	private const string c_strEventMarkerTalk = "Prefabs/InGame/Game/Icon_EventMarkerTalk";

	private GameDefine.eAnimChangeState m_eTalkBut;

	private int m_iPushButIdx = -1;

	private bool[] m_isSavedAllCharEnable = new bool[8];

	private string[] m_strSavedAllCharMotion = new string[8];

	private int[] m_iArrColorCharIdx;

	private int[] m_iArrCharColor;

	public void InitAfterLoad()
	{
		if (m_listTalkChar == null)
		{
			m_listTalkChar = new List<TalkCharUnit>();
		}
		if (m_listResChar == null)
		{
			m_listResChar = new List<ResChar>();
		}
		FOR_CHAR_POS_CALC = GameGlobalUtil.GetXlsProgramDefineStr("FOR_CHAR_POS_CALC");
		m_EventEngine = EventEngine.GetInstance();
		m_GameSwitch = GameSwitch.GetInstance();
		GameObject eventCanvas = m_EventEngine.GetEventCanvas();
		if (eventCanvas != null)
		{
			m_tfEventCanvas = eventCanvas.transform;
		}
		m_MainCam = Camera.main;
	}

	public void UnloadRes()
	{
		int num = 8;
		string text = null;
		for (int i = 0; i < num; i++)
		{
			if (m_goTalkMark[i] != null)
			{
				Object.Destroy(m_goTalkMark[i]);
			}
			m_goTalkMark[i] = null;
			m_clMarkerTalk[i] = null;
			m_animTalkMark[i] = null;
			m_butTalkBack[i] = null;
			m_goTalkOKButton[i] = null;
		}
		int num2 = ((m_listResChar != null) ? m_listResChar.Count : 0);
		for (int j = 0; j < num2; j++)
		{
			Object.Destroy(m_listResChar[j].m_objChar);
		}
		m_goForCharPosCalc = null;
		m_EventEngine = null;
		m_GameSwitch = null;
		m_MainCam = null;
		m_tfEventCanvas = null;
		if (m_listTalkChar != null)
		{
			m_listTalkChar.Clear();
		}
		m_listTalkChar = null;
		if (m_listResChar != null)
		{
			m_listResChar.Clear();
		}
		m_listResChar = null;
	}

	private bool IsLoadedChar(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return false;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (talkCharUnit.m_strCharKey == strCharKey && talkCharUnit.m_goChar.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	public void Update()
	{
		int num = 0;
		if (m_iPushButIdx != -1 && m_goTalkMark[m_iPushButIdx] != null && m_goTalkMark[m_iPushButIdx].gameObject.activeInHierarchy)
		{
			if (GameGlobalUtil.IsCheckPlayAnimation(m_animTalkMark[m_iPushButIdx], "steam_push") && GameGlobalUtil.CheckPlayEndUIAnimation(m_animTalkMark[m_iPushButIdx], "steam_push", ref m_eTalkBut))
			{
				m_eTalkBut = GameDefine.eAnimChangeState.none;
				num = 8;
				for (int i = 0; i < num; i++)
				{
					if (m_goTalkMark[i] != null && m_goTalkMark[i].gameObject.activeInHierarchy)
					{
						if (i == m_iPushButIdx)
						{
							GameGlobalUtil.PlayUIAnimation(m_animTalkMark[m_iPushButIdx], "disappear", ref m_eTalkBut);
						}
						else
						{
							GameGlobalUtil.PlayUIAnimation(m_animTalkMark[i], "disappear");
						}
					}
				}
			}
			else if (GameGlobalUtil.IsCheckPlayAnimation(m_animTalkMark[m_iPushButIdx], "disappear") && GameGlobalUtil.CheckPlayEndUIAnimation(m_animTalkMark[m_iPushButIdx], "disappear", ref m_eTalkBut))
			{
				m_eTalkBut = GameDefine.eAnimChangeState.none;
				string strCharKey = m_goTalkMark[m_iPushButIdx].GetComponent<MarkerTalk>().m_strCharKey;
				m_EventEngine.m_TalkChar.TouchTalkButton(strCharKey);
				m_iPushButIdx = -1;
			}
		}
		num = 8;
		for (int j = 0; j < num; j++)
		{
			if (m_goTalkMark != null && m_goTalkMark[j] != null && m_goTalkMark[j].activeInHierarchy && m_eMarkButChgState[j] != GameDefine.eAnimChangeState.none && GameGlobalUtil.IsCheckPlayAnimation(m_animTalkMark[j], GameDefine.UIAnimationState.disappear.ToString()) && GameGlobalUtil.CheckPlayEndUIAnimation(m_animTalkMark[j], GameDefine.UIAnimationState.disappear, ref m_eMarkButChgState[j]))
			{
				m_eMarkButChgState[j] = GameDefine.eAnimChangeState.none;
				m_goTalkMark[j].SetActive(value: false);
			}
		}
	}

	public void PreLoadChar(string strCharKey)
	{
		m_EventEngine.m_GameMain.StartCoroutine(LoadChar(strCharKey));
	}

	private IEnumerator LoadChar(string strCharKey)
	{
		m_loadResChar = null;
		ResChar resChar = ExistResChar(strCharKey);
		if (resChar == null)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
			_ = data_byKey.m_strResName;
			resChar = new ResChar();
			resChar.m_strChar = strCharKey;
			resChar.m_objChar = Object.Instantiate(GetCharResource(strCharKey)) as GameObject;
			m_listResChar.Add(resChar);
		}
		m_loadResChar = resChar;
		yield break;
	}

	public Object GetCharResource(string strCharKey)
	{
		Object result = null;
		switch (strCharKey)
		{
		case "한도윤":
			result = MainLoadThing.instance.m_prefabCharHan;
			break;
		case "민주영":
			result = MainLoadThing.instance.m_prefabCharMin;
			break;
		case "서혜성":
			result = MainLoadThing.instance.m_prefabCharSeo;
			break;
		case "오인하":
			result = MainLoadThing.instance.m_prefabCharOh;
			break;
		case "이규혁":
			result = MainLoadThing.instance.m_prefabCharLee;
			break;
		case "장세일":
			result = MainLoadThing.instance.m_prefabCharChang;
			break;
		case "신승연":
			result = MainLoadThing.instance.m_prefabCharShin;
			break;
		case "하수창":
			result = MainLoadThing.instance.m_prefabCharHa;
			break;
		case "남세일":
			result = MainLoadThing.instance.m_prefabCharNam;
			break;
		}
		return result;
	}

	public void SetCharPosXlsOrData(ref TalkCharUnit tcTemp, string strCharKey, int iPos, int iSize)
	{
		bool flag = false;
		bool flag2 = false;
		float charPivotX = m_GameSwitch.GetCharPivotX(strCharKey, iSize, iPos);
		float charPivotY = m_GameSwitch.GetCharPivotY(strCharKey, iSize);
		if (charPivotX == 999f)
		{
			flag = true;
		}
		if (charPivotY == 999f)
		{
			flag2 = true;
		}
		Vector2 charPosVec = GetCharPosVec(strCharKey, iSize, iPos);
		tcTemp.m_vecDestPos = new Vector2((!flag) ? charPivotX : charPosVec.x, (!flag2) ? charPivotY : charPosVec.y);
	}

	public Vector2 GetCharPosXlsOrData(string strCharKey, int iPos, int iSize)
	{
		bool flag = false;
		bool flag2 = false;
		float charPivotX = m_GameSwitch.GetCharPivotX(strCharKey, iSize, iPos);
		float charPivotY = m_GameSwitch.GetCharPivotY(strCharKey, iSize);
		if (charPivotX == 999f)
		{
			flag = true;
		}
		if (charPivotY == 999f)
		{
			flag2 = true;
		}
		Vector2 charPosVec = GetCharPosVec(strCharKey, iSize, iPos);
		Vector2 result = new Vector2((!flag) ? charPivotX : charPosVec.x, (!flag2) ? charPivotY : charPosVec.y);
		return result;
	}

	public IEnumerator CreateChar(string strCharKey, string strPosition, string strSize, string strMot, string strDir)
	{
		m_isCreateCharComp = false;
		int iPosition = GameGlobalUtil.GetXlsScriptKeyValue(strPosition);
		int iSize = GameGlobalUtil.GetXlsScriptKeyValue(strSize);
		int iDir = GameGlobalUtil.GetXlsScriptKeyValue(strDir);
		yield return m_EventEngine.m_GameMain.StartCoroutine(CreateChar(strCharKey, iPosition, iSize, strMot, iDir, isSaveDataPosSet: true));
		m_isCreateCharComp = true;
	}

	public bool IsCompCreateChar()
	{
		return m_isCreateCharComp;
	}

	public IEnumerator CreateChar(string strCharKey, int iPosition, int iSize, string strMot, int iDir, bool isSaveDataPosSet = false)
	{
		if (iPosition < 0 || iPosition > MAX_CHR_POSITION_CNT)
		{
			iPosition = 0;
		}
		if (iSize < 0 || iSize > MAX_CHR_SIZE_CNT)
		{
			iSize = 0;
		}
		int iDataIdx = m_GameSwitch.GetCharGetXlsIdx(strCharKey, iSize, iPosition);
		Xls.CharZoomPos chZoomPos = Xls.CharZoomPos.GetData_bySwitchIdx(iDataIdx);
		Vector2 vecDest = ((!isSaveDataPosSet) ? new Vector2(chZoomPos.m_fPosX, chZoomPos.m_fPosY) : GetCharPosXlsOrData(strCharKey, iPosition, iSize));
		yield return m_EventEngine.m_GameMain.StartCoroutine(CreateCharPosVec(strCharKey, vecDest, iSize, strMot, iDir, isSaveDataPosSet));
		if (m_tmpTalkChar != null)
		{
			m_tmpTalkChar.m_iPosition = iPosition;
		}
	}

	public IEnumerator CreateCharPosVec(string strCharKey, Vector2 vecPos, int iSize, string strMot, int iDir, bool isSaveDataPosSet = false, bool isEnable = true)
	{
		if (iSize < 0 || iSize > MAX_CHR_SIZE_CNT)
		{
			iSize = 0;
		}
		bool isAddList = false;
		m_tmpTalkChar = null;
		TalkCharUnit tmpTalkChar = GetTalkCharUnit(strCharKey);
		Xls.CharData xlsChData = Xls.CharData.GetData_byKey(strCharKey);
		if (tmpTalkChar == null)
		{
			ResChar resChar = null;
			yield return m_EventEngine.m_GameMain.StartCoroutine(LoadChar(strCharKey));
			resChar = m_loadResChar;
			tmpTalkChar = new TalkCharUnit();
			tmpTalkChar.m_goChar = resChar.m_objChar;
			tmpTalkChar.m_goChar.transform.SetParent(Camera.main.transform);
			isAddList = true;
		}
		if (!tmpTalkChar.m_goChar.activeInHierarchy && !isEnable)
		{
			tmpTalkChar.m_goChar.SetActive(value: false);
		}
		else
		{
			tmpTalkChar.m_goChar.SetActive(value: true);
		}
		tmpTalkChar.m_iDir = iDir;
		if ((tmpTalkChar.m_iDir == 1 && xlsChData.m_iShowLeft == 0) || (tmpTalkChar.m_iDir == 0 && xlsChData.m_iShowLeft == 1))
		{
			tmpTalkChar.m_goChar.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		else
		{
			tmpTalkChar.m_goChar.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		if (isAddList)
		{
			tmpTalkChar.m_strCharKey = strCharKey;
			tmpTalkChar.m_goChar.name = "character_" + strCharKey;
		}
		tmpTalkChar.m_vecStartPos = (tmpTalkChar.m_vecDestPos = vecPos);
		tmpTalkChar.m_goChar.transform.position = m_GameSwitch.GetPosByPercentZeroToOne(tmpTalkChar.m_vecDestPos.x, tmpTalkChar.m_vecDestPos.y, CHR_SCREEN_Z_POS);
		tmpTalkChar.m_iMoveSpeedType = 5;
		tmpTalkChar.m_iZoomSpeedType = 5;
		tmpTalkChar.m_isBlackOn = false;
		tmpTalkChar.m_iSize = (byte)iSize;
		int iDataIdx = m_GameSwitch.GetCharGetXlsIdx(strCharKey, iSize, 0);
		Xls.CharZoomPos chZoomPos = Xls.CharZoomPos.GetData_bySwitchIdx(iDataIdx);
		tmpTalkChar.m_goChar.transform.localScale = new Vector3(chZoomPos.m_fZoomValue, chZoomPos.m_fZoomValue, chZoomPos.m_fZoomValue);
		SetCharLayer(tmpTalkChar.m_goChar, iSize);
		if (isAddList)
		{
			TalkCharUnit talkCharUnit = GetTalkCharUnit(FOR_CHAR_POS_CALC);
			if (talkCharUnit == null)
			{
				GameObject gameObject = new GameObject(FOR_CHAR_POS_CALC);
				gameObject.SetActive(value: false);
				gameObject.name = FOR_CHAR_POS_CALC;
				gameObject.transform.SetParent(Camera.main.transform);
				talkCharUnit = new TalkCharUnit();
				talkCharUnit.m_goChar = gameObject;
				talkCharUnit.m_strCharKey = FOR_CHAR_POS_CALC;
				m_listTalkChar.Add(talkCharUnit);
				m_goForCharPosCalc = gameObject;
			}
			m_listTalkChar.Add(tmpTalkChar);
		}
		tmpTalkChar.m_clCharAnimatorPlay = tmpTalkChar.m_goChar.GetComponent<CharAnimatorPlay>();
		ChangeCharMot(strCharKey, strMot, iDir);
		m_tmpTalkChar = tmpTalkChar;
	}

	public IEnumerator PreLoadChar(int iCharIdx)
	{
		int iSize = 0;
		int iPos = CHAR_POS_LEFT_OUT;
		string strCharKey = Xls.CharData.GetData_bySwitchIdx(iCharIdx).m_strKey;
		yield return m_EventEngine.m_GameMain.StartCoroutine(CreateChar(strCharKey, iPos, iSize, STR_NONE_MOT, 0));
		if (m_tmpTalkChar != null)
		{
			m_tmpTalkChar.m_goChar.SetActive(value: false);
		}
	}

	private TalkCharUnit GetTalkCharUnit(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return null;
		}
		int count = m_listTalkChar.Count;
		TalkCharUnit result = null;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (talkCharUnit.m_strCharKey == strCharKey)
			{
				result = talkCharUnit;
				break;
			}
		}
		return result;
	}

	public void HidePartyTalkIcon()
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		if (!m_GameSwitch.GetRunKeywordEvt())
		{
			int count = m_listTalkChar.Count;
			for (int i = 0; i < count && m_listTalkChar.Count > i; i++)
			{
				ActiveTalkIcon(m_listTalkChar[i].m_strCharKey, isShow: false);
			}
		}
		m_isShowPartyChar = false;
	}

	public void SetDisalbeAllTalkIcon()
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		Xls.CharData charData = null;
		int num = 0;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			charData = Xls.CharData.GetData_byKey(m_listTalkChar[i].m_strCharKey);
			if (charData != null)
			{
				num = charData.m_iIdx;
				if (BitCalc.CheckArrayIdx(num, 8) && m_goTalkMark[num] != null)
				{
					m_goTalkMark[num].SetActive(value: false);
				}
			}
		}
	}

	public IEnumerator ShowPartyChar(bool isShowTalkIcon = true, GameDefine.EventProc fEvtCBFunc = null)
	{
		if (GameMain.GetDefaultSceneType() != GameMain.eDefType.Investigate)
		{
			if (m_isShowPartyChar && isShowTalkIcon)
			{
				SetTalkMarkAppear(isAppear: true);
			}
			m_isShowTalkIcon = isShowTalkIcon;
			m_isAllCharLoad = false;
			BitCalc.InitArray(m_isArrSetShowChar);
			yield return m_EventEngine.m_GameMain.StartCoroutine(CheckCreateChar());
			fEvtCBFunc?.Invoke(null, null);
		}
	}

	public bool GetAllPartyCharLoad()
	{
		bool result = true;
		if (GameMain.GetDefaultSceneType() == GameMain.eDefType.Talk)
		{
			result = m_isAllCharLoad;
		}
		return result;
	}

	public void TouchTalkButton(string strCharKey = null)
	{
		m_EventEngine.m_GameMain.ShowKeywordMenu(isShow: true, isKeywordMenu: false, strCharKey);
	}

	public void MovePartyCharKeywordSet(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (!talkCharUnit.m_strCharKey.Equals(strCharKey))
			{
				talkCharUnit.m_goChar.SetActive(value: false);
			}
			ActiveTalkIcon(talkCharUnit.m_strCharKey, isShow: false);
		}
		if (strCharKey != null)
		{
			ShowCharKeywordMenu(strCharKey, 0.2f);
		}
	}

	public void SetMoveCharSNSSet(bool isShow)
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		string text = "한시우";
		bool flag = false;
		if (isShow)
		{
			int count = m_listTalkChar.Count;
			for (int i = 0; i < count; i++)
			{
				TalkCharUnit talkCharUnit = m_listTalkChar[i];
				if (!talkCharUnit.m_strCharKey.Equals(text))
				{
					talkCharUnit.m_goChar.SetActive(value: false);
					ActiveTalkIcon(talkCharUnit.m_strCharKey, isShow: false);
				}
				else
				{
					flag = true;
					talkCharUnit.m_goChar.SetActive(value: true);
				}
			}
			HidePartyTalkIcon();
		}
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue("중");
		int xlsScriptKeyValue2 = GameGlobalUtil.GetXlsScriptKeyValue("좌밖");
		int xlsScriptKeyValue3 = GameGlobalUtil.GetXlsScriptKeyValue("극좌");
		if (isShow)
		{
			Vector2 charPosVec;
			if (!flag)
			{
				m_EventEngine.m_GameMain.StartCoroutine(CreateChar(text, xlsScriptKeyValue2, xlsScriptKeyValue, "기본", 1, isSaveDataPosSet: true));
			}
			else
			{
				charPosVec = GetCharPosVec(text, xlsScriptKeyValue, xlsScriptKeyValue2, isSaveData: true);
				SetCharZoomWithPos(text, xlsScriptKeyValue, 0, isUseDest: true, charPosVec, 0f);
			}
			charPosVec = GetCharPosVec(text, xlsScriptKeyValue, xlsScriptKeyValue3, isSaveData: true);
			SetCharZoomWithPos(text, xlsScriptKeyValue, 0, isUseDest: true, charPosVec, 0.5f);
		}
		else
		{
			Vector2 charPosVec = GetCharPosVec(text, xlsScriptKeyValue, xlsScriptKeyValue2, isSaveData: true);
			SetCharZoomWithPos(text, xlsScriptKeyValue, 0, isUseDest: true, charPosVec, 0.5f);
		}
		ChangeHanMotByMental();
		m_EventEngine.m_GameMain.AddProcEvent(GameMain.eProcFunc.CHAR_ZOOM);
	}

	public void ChangeHanMotByMental()
	{
		string strCharKey = "한시우";
		ChangeCharMot(strCharKey, m_GameSwitch.GetMentalLowHigh() switch
		{
			ConstGameSwitch.eMental.HIGH => "기본", 
			ConstGameSwitch.eMental.NORMAL => "생각", 
			_ => "의혹아래", 
		}, "우향", isSaveCurMot: false);
	}

	private Vector2 GetPercentPosFromLocalPos(Vector2 vecLocalPos)
	{
		m_goForCharPosCalc.transform.localPosition = new Vector3(vecLocalPos.x, vecLocalPos.y, 1f);
		Vector2 vector = m_GameSwitch.GetPosWorldPointToViewPort(m_goForCharPosCalc.transform.position);
		return new Vector2(vector.x, vector.y);
	}

	public void RevertPartyCharKeywordSet(string strCharKey)
	{
		if (strCharKey != null)
		{
			int charPartySize = m_GameSwitch.GetCharPartySize(strCharKey);
			Vector2 percentPosFromLocalPos = GetPercentPosFromLocalPos(m_GameSwitch.GetCharPartyPos(strCharKey));
			SetCharZoomWithPos(strCharKey, charPartySize, 0, isUseDest: true, percentPosFromLocalPos, 0.2f, isSetLayer: false);
			ChangeCharMot(strCharKey, m_GameSwitch.GetCharPartyMotion(strCharKey), m_GameSwitch.GetCharPartyDir(strCharKey));
			TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
			talkCharUnit.m_isSetLayer = true;
		}
		m_isShowTalkIcon = true;
		BitCalc.InitArray(m_isArrSetShowChar);
		m_EventEngine.m_GameMain.StartCoroutine(CheckCreateChar(strCharKey));
		m_EventEngine.m_GameMain.AddProcEvent(GameMain.eProcFunc.CHAR_ZOOM, GameMain.eFromMenu.KEYWORD);
	}

	public void SetRunKeywordCharSet(string strCharKey)
	{
		if (strCharKey != null)
		{
			SetCharMoveDetail(strCharKey, 0.5f, 999f, 0.5f, 0, isSavePivot: false);
			m_EventEngine.m_GameMain.AddProcEvent(GameMain.eProcFunc.CHAR_MOVE, GameMain.eFromMenu.KEYWORD);
		}
	}

	public void SetKeywordMenuMot(string strMot, bool isChangeMot = false)
	{
		m_strKeywordMenuMot = strMot;
	}

	private string GetKeywordMenuMot()
	{
		return m_strKeywordMenuMot;
	}

	public void ShowCharKeywordMenu(string strCharKey, float fTime)
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		TalkCharUnit talkCharUnit = null;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit2 = m_listTalkChar[i];
			if (talkCharUnit2.m_strCharKey.Equals(strCharKey))
			{
				string strMotionKey = ((!(GetKeywordMenuMot() == string.Empty)) ? GetKeywordMenuMot() : m_GameSwitch.GetCharPartyMotion(strCharKey));
				int charPartyDir = m_GameSwitch.GetCharPartyDir(strCharKey);
				talkCharUnit = talkCharUnit2;
				if (!talkCharUnit.m_goChar.activeSelf)
				{
					talkCharUnit.m_goChar.SetActive(value: true);
					ChangeCharMot(strCharKey, strMotionKey, charPartyDir, isSaveCurMot: false);
				}
				else
				{
					fTime = 0.2f;
					ChangeCharMot(strCharKey, strMotionKey, charPartyDir, isSaveCurMot: false);
				}
				break;
			}
		}
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue("중");
		float charKeywordPosX = m_GameSwitch.GetCharKeywordPosX(strCharKey);
		float charKeywordPosY = m_GameSwitch.GetCharKeywordPosY(strCharKey);
		Vector2 vecLocalPos = new Vector2(charKeywordPosX, charKeywordPosY);
		Vector2 percentPosFromLocalPos = GetPercentPosFromLocalPos(vecLocalPos);
		percentPosFromLocalPos.x = charKeywordPosX;
		SetCharZoomWithPos(strCharKey, xlsScriptKeyValue, 0, isUseDest: true, percentPosFromLocalPos, fTime, isSetLayer: false);
		SetCharLayer(strCharKey, 5, isOnlySortingLayer: true);
		if (fTime > 0f)
		{
			m_EventEngine.m_GameMain.AddProcEvent(GameMain.eProcFunc.CHAR_ZOOM);
		}
		SetKeywordMenuMot(string.Empty);
	}

	public bool ProcShowPartyChar()
	{
		if (GameMain.GetDefaultSceneType() == GameMain.eDefType.Investigate)
		{
			return true;
		}
		if (m_isShowPartyChar)
		{
			return true;
		}
		bool flag = false;
		if (m_strLoadAsyncChar == null)
		{
			flag = m_isAllCharLoad && EventCameraEffect.Instance.IsCompleteEffectProc(EventCameraEffect.Effects.Cover);
		}
		if (flag)
		{
			m_isShowPartyChar = true;
		}
		return flag;
	}

	public ResChar ExistResChar(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return null;
		}
		ResChar result = null;
		int count = m_listResChar.Count;
		for (int i = 0; i < count; i++)
		{
			ResChar resChar = m_listResChar[i];
			if (resChar.m_strChar == strCharKey)
			{
				result = resChar;
				break;
			}
		}
		return result;
	}

	private IEnumerator LoadCharAsync(string strCharKey)
	{
		ResChar resChar = ExistResChar(strCharKey);
		if (resChar == null)
		{
			m_strLoadAsyncChar = strCharKey;
			string strResName = null;
			Xls.CharData xlsCharData = Xls.CharData.GetData_byKey(strCharKey);
			if (xlsCharData != null)
			{
				strResName = xlsCharData.m_strResName;
			}
			if (strResName != null)
			{
				yield return m_EventEngine.m_GameMain.StartCoroutine(CBDoneLoadChar(Object.Instantiate(GetCharResource(strCharKey))));
			}
			else
			{
				yield return m_EventEngine.m_GameMain.StartCoroutine(CBDoneLoadChar(null));
			}
		}
		else
		{
			yield return m_EventEngine.m_GameMain.StartCoroutine(CBDoneLoadChar(null));
		}
	}

	private IEnumerator CBDoneLoadChar(Object obj)
	{
		if (obj != null)
		{
			yield return GameMain.instance.StartCoroutine(AddCharResInResList(obj, m_strLoadAsyncChar));
			yield return GameMain.instance.StartCoroutine(SetCharPartyState(m_strLoadAsyncChar));
			m_strLoadAsyncChar = null;
		}
		yield return GameMain.instance.StartCoroutine(CheckCreateChar());
	}

	public IEnumerator AddCharResInResList(Object obj, string strCharKey)
	{
		if (!(obj == null))
		{
			ResChar resChar = new ResChar();
			resChar.m_strChar = strCharKey;
			resChar.m_objChar = Object.Instantiate(obj) as GameObject;
			resChar.m_objChar.transform.SetParent(Camera.main.transform);
			resChar.m_objChar.name = "character_" + strCharKey;
			m_listResChar.Add(resChar);
			int iPos = ((m_GameSwitch.GetCharPartyShowIcon(strCharKey) != 1) ? 5 : 0);
			int iSize = m_GameSwitch.GetCharPartySize(strCharKey);
			string strMot = m_GameSwitch.GetCharPartyMotion(strCharKey);
			int iDir = m_GameSwitch.GetCharPartyDir(strCharKey);
			yield return m_EventEngine.m_GameMain.StartCoroutine(CreateChar(resChar.m_strChar, iPos, iSize, strMot, iDir));
		}
	}

	private IEnumerator SetCharPartyState(string strChar, bool isEnable = true)
	{
		int iCharIdx = -1;
		Xls.CharData xlsCharData = Xls.CharData.GetData_byKey(strChar);
		if (xlsCharData != null)
		{
			iCharIdx = xlsCharData.m_iIdx;
		}
		if (iCharIdx != -1 && !m_isArrSetShowChar[iCharIdx])
		{
			Xls.TalkCutChrSetting xlsTalkCutSet = m_GameSwitch.GetTalkCutSetting(strChar);
			if (xlsTalkCutSet != null && xlsTalkCutSet.m_iIconShow == 1)
			{
				Vector2 vecPartyDest = GetPercentPosFromLocalPos(m_GameSwitch.GetCharPartyPos(strChar));
				int iSize = m_GameSwitch.GetCharPartySize(iCharIdx);
				string strMot = m_GameSwitch.GetCharPartyMotion(iCharIdx);
				int iDir = m_GameSwitch.GetCharPartyDir(iCharIdx);
				yield return m_EventEngine.m_GameMain.StartCoroutine(CreateCharPosVec(strChar, vecPartyDest, iSize, strMot, iDir, isSaveDataPosSet: true, isEnable));
			}
			m_isArrSetShowChar[iCharIdx] = true;
		}
	}

	public void ActivePartyAllChar()
	{
		string text = null;
		int num = 8;
		TalkCharUnit talkCharUnit = null;
		for (int i = 0; i < num; i++)
		{
			if (i != 0 && m_GameSwitch.IsCharPartyState(i, 1))
			{
				text = Xls.CharData.GetData_bySwitchIdx(i).m_strKey;
				talkCharUnit = GetTalkCharUnit(text);
				if (talkCharUnit != null)
				{
					talkCharUnit.m_goChar.SetActive(value: true);
					ChangeCharMot(text, string.Empty, -1);
				}
			}
		}
	}

	public void ActiveTalkIconAll(bool isShow)
	{
		int num = 8;
		string text = null;
		string[] array = new string[num];
		int[] iArray = new int[num];
		BitCalc.InitArray(array);
		BitCalc.InitArray(iArray);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (m_GameSwitch.IsCharPartyState(i, 1))
			{
				text = Xls.CharData.GetData_bySwitchIdx(i).m_strKey;
				Xls.TalkCutChrSetting talkCutSetting = m_GameSwitch.GetTalkCutSetting(text);
				if (talkCutSetting != null && talkCutSetting.m_iIconShow == 1)
				{
					array[num2++] = text;
				}
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (array[j] != null)
			{
				ActiveTalkIcon(array[j], isShow, isSetTalkable: true, m_GameSwitch.IsExistUsableKeyword(array[j]));
			}
		}
	}

	public void ActiveTalkIcon(string strCharKey, bool isShow, bool isSetTalkable = false, bool isTalkable = false)
	{
		if (strCharKey == null || strCharKey == FOR_CHAR_POS_CALC)
		{
			return;
		}
		string text = "Icon_Talk_" + strCharKey;
		bool flag = false;
		GameObject gameObject = null;
		int childCount = m_tfEventCanvas.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = m_tfEventCanvas.GetChild(i);
			if (child.name.Equals(text))
			{
				gameObject = child.gameObject;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			gameObject = Object.Instantiate(MainLoadThing.instance.m_prefabEventMarkerTalk) as GameObject;
			gameObject.name = text;
			gameObject.GetComponent<MarkerTalk>().m_strCharKey = strCharKey;
			gameObject.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
		}
		if (isSetTalkable)
		{
			MarkerTalk component = gameObject.GetComponent<MarkerTalk>();
			if (component != null)
			{
				component.SetNoKeyword(!isTalkable);
				component.SetConditionImg(IconCondition.instance.GetConditionSprite(strCharKey));
			}
		}
		gameObject.SetActive(isShow);
		if (isShow)
		{
			GameObject gameObject2 = null;
			int count = m_listTalkChar.Count;
			TalkCharUnit talkCharUnit = FindCharUnit(strCharKey);
			if (talkCharUnit == null)
			{
				return;
			}
			if (talkCharUnit != null)
			{
				gameObject2 = talkCharUnit.m_goChar;
			}
			if (gameObject2 == null)
			{
				return;
			}
			RectTransform component2 = m_tfEventCanvas.GetComponent<RectTransform>();
			Vector2 percentPosFromLocalPos = GetPercentPosFromLocalPos(m_GameSwitch.GetCharPartyPos(strCharKey));
			Vector3 posByPercentZeroToOne = m_GameSwitch.GetPosByPercentZeroToOne(percentPosFromLocalPos.x, percentPosFromLocalPos.y, 1f);
			Vector2 canvViewPosByWorldPos = m_GameSwitch.GetCanvViewPosByWorldPos(m_MainCam, component2, posByPercentZeroToOne);
			RectTransform component3 = gameObject.GetComponent<RectTransform>();
			component3.anchoredPosition = new Vector2(canvViewPosByWorldPos.x, component3.anchoredPosition.y);
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
			if (data_byKey != null)
			{
				int iIdx = data_byKey.m_iIdx;
				if (BitCalc.CheckArrayIdx(iIdx, 8))
				{
					m_goTalkMark[iIdx] = gameObject;
					m_clMarkerTalk[iIdx] = m_goTalkMark[iIdx].GetComponent<MarkerTalk>();
					m_animTalkMark[iIdx] = m_goTalkMark[iIdx].transform.GetChild(0).GetComponent<Animator>();
					m_butTalkBack[iIdx] = m_goTalkMark[iIdx].transform.GetChild(0).Find("Marker_conversation_BG").GetComponent<Button>();
					m_goTalkOKButton[iIdx] = gameObject.GetComponent<MarkerTalk>().m_butO;
					SetTalkMarkAnim(iIdx);
				}
			}
			return;
		}
		Xls.CharData data_byKey2 = Xls.CharData.GetData_byKey(strCharKey);
		if (data_byKey2 != null)
		{
			int iIdx2 = data_byKey2.m_iIdx;
			if (BitCalc.CheckArrayIdx(iIdx2, 8) && m_goTalkMark != null && m_goTalkMark[iIdx2] != null)
			{
				m_goTalkMark[iIdx2].SetActive(value: false);
			}
		}
	}

	public void SetTalkMarkAppear(bool isAppear, int iIdx = -1)
	{
		if (iIdx == -1)
		{
			for (int i = 0; i < MAX_CHR_POSITION_CNT; i++)
			{
				if (m_butTalkBack[i] != null)
				{
					ButtonSpriteSwap.PressButton(m_butTalkBack[i], isPress: false);
				}
				PlayTalkMarkAppear(i, isAppear);
			}
		}
		else
		{
			PlayTalkMarkAppear(iIdx, isAppear);
		}
	}

	private void PlayTalkMarkAppear(int iIdx, bool isAppear)
	{
		if (BitCalc.CheckArrayIdx(iIdx, MAX_CHR_POSITION_CNT) && m_animTalkMark[iIdx] != null)
		{
			m_eMarkButChgState[iIdx] = GameDefine.eAnimChangeState.none;
			GameGlobalUtil.PlayUIAnimation(m_animTalkMark[iIdx], (!isAppear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear, ref m_eMarkButChgState[iIdx]);
		}
	}

	public void SetTalkMarkAnim(int iCharIdx = -1)
	{
		if (iCharIdx == -1)
		{
			for (int i = 0; i < 8; i++)
			{
				if (m_goTalkMark[i] != null)
				{
					SetEachButton(i);
				}
			}
		}
		else
		{
			SetEachButton(iCharIdx, isFirstSet: true);
		}
	}

	private void SetEachButton(int iCharIdx, bool isFirstSet = false)
	{
		if (m_goTalkMark[iCharIdx] == null)
		{
			return;
		}
		bool flag = m_clMarkerTalk[iCharIdx].IsTalkable();
		bool flag2 = iCharIdx != -1 && m_GameSwitch.IsThisPosSelTalkPos(iCharIdx);
		ButtonSpriteSwap.PressButton(m_butTalkBack[iCharIdx], flag2);
		m_goTalkOKButton[iCharIdx].gameObject.SetActive(flag2);
		Text componentInChildren = m_goTalkMark[iCharIdx].GetComponentInChildren<Text>();
		if (componentInChildren != null && componentInChildren.gameObject.activeInHierarchy)
		{
			if (!flag)
			{
				componentInChildren.color = GameGlobalUtil.HexToColor((!flag2) ? 5920595 : 4144442);
			}
			else
			{
				componentInChildren.color = GameGlobalUtil.HexToColor((!flag2) ? 11969145 : 4144442);
			}
		}
		m_clMarkerTalk[iCharIdx].SetConditionImg(IconCondition.instance.GetConditionSprite(iCharIdx, flag2));
		if (m_animTalkMark[iCharIdx].gameObject.activeInHierarchy)
		{
			m_animTalkMark[iCharIdx].SetBool("m_isSel", flag2);
			if (flag2)
			{
				int childCount = m_tfEventCanvas.childCount;
				m_goTalkMark[iCharIdx].transform.SetSiblingIndex(childCount);
			}
		}
		if (!isFirstSet)
		{
			GameGlobalUtil.PlayUIAnimation(m_animTalkMark[iCharIdx], (!flag2) ? GameDefine.UIAnimationState.idle : GameDefine.UIAnimationState.idle2);
		}
		else
		{
			GameGlobalUtil.PlayUIAnimation(m_animTalkMark[iCharIdx], GameDefine.UIAnimationState.appear);
		}
	}

	public void TalkPressOKButton()
	{
		if (IsPressTalkOKButton())
		{
			return;
		}
		int realSelPartyObjIdx = m_GameSwitch.GetRealSelPartyObjIdx();
		Xls.CharData charData = null;
		try
		{
			charData = Xls.CharData.GetData_byKey(m_clMarkerTalk[realSelPartyObjIdx].m_strCharKey);
		}
		catch
		{
			return;
		}
		int num = 0;
		if (charData != null)
		{
			num = charData.m_iIdx;
		}
		if (m_goTalkOKButton[num] != null && m_goTalkOKButton[num].gameObject.activeInHierarchy && (GameGlobalUtil.IsCheckPlayUIAnimation(m_animTalkMark[num], "idle") || GameGlobalUtil.IsCheckPlayUIAnimation(m_animTalkMark[num], "idle2")))
		{
			AudioManager.instance.PlayUISound("Push_Maker");
			GameGlobalUtil.PlayUIAnimation(m_animTalkMark[num], "steam_push", ref m_eTalkBut);
			if (m_clMarkerTalk[num].IsTalkable())
			{
				m_iPushButIdx = num;
				ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_goTalkOKButton[num]);
			}
			else
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_goTalkOKButton[num], null, null, null, CBOKButton);
			}
		}
	}

	public bool IsPressTalkOKButton()
	{
		return m_iPushButIdx != -1;
	}

	public void CBOKButton()
	{
		int realSelPartyObjIdx = m_GameSwitch.GetRealSelPartyObjIdx();
		if (!m_clMarkerTalk[realSelPartyObjIdx].IsTalkable())
		{
			PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("HAVE_NO_USABLE_KEYWORD"), CB_PopupExit);
		}
	}

	private void CB_PopupExit(PopupDialoguePlus.Result result)
	{
		SetTalkMarkAnim();
	}

	public void TouchTalkIcon(string strCharKey)
	{
		bool flag = false;
		int num = 8;
		for (int i = 0; i < num; i++)
		{
			if (m_clMarkerTalk[i] != null && m_clMarkerTalk[i].m_strCharKey == strCharKey)
			{
				flag = m_clMarkerTalk[i].IsTalkable();
				break;
			}
		}
		AudioManager.instance.PlayUISound("Push_Maker");
		if (flag)
		{
			m_EventEngine.m_TalkChar.TouchTalkButton(strCharKey);
		}
		else
		{
			PopupDialoguePlus.ShowActionPopup_OK(GameGlobalUtil.GetXlsProgramText("HAVE_NO_USABLE_KEYWORD"), CB_PopupExit);
		}
		m_GameSwitch.SetTalkSelChar(strCharKey);
	}

	private IEnumerator CheckCreateChar(string strCharKey = null)
	{
		string strLoadChar = null;
		int iCnt = 8;
		int iListCnt = m_listResChar.Count;
		string strPartyChrName = null;
		bool isAlreadyLoaded = false;
		for (int i = 0; i < iCnt; i++)
		{
			if (i == 0 || !m_GameSwitch.IsCharPartyState(i, 1))
			{
				continue;
			}
			strPartyChrName = Xls.CharData.GetData_bySwitchIdx(i).m_strKey;
			isAlreadyLoaded = false;
			for (int j = 0; j < iListCnt; j++)
			{
				if (m_listResChar[j].m_strChar.Equals(strPartyChrName))
				{
					isAlreadyLoaded = true;
					break;
				}
			}
			if (!isAlreadyLoaded)
			{
				strLoadChar = strPartyChrName;
				break;
			}
			if (!m_isArrSetShowChar[i])
			{
				if (strCharKey != null && strCharKey.Equals(strPartyChrName))
				{
					m_isArrSetShowChar[i] = true;
				}
				else
				{
					yield return GameMain.instance.StartCoroutine(SetCharPartyState(strPartyChrName, isEnable: false));
				}
			}
		}
		if (strLoadChar == null)
		{
			m_isAllCharLoad = true;
			if (!EventEngine.GetLoadIng())
			{
				EventCameraEffect.Instance.Activate_FadeOut(0.5f);
			}
			ActiveTalkIconAll(m_isShowTalkIcon);
			ActivePartyAllChar();
		}
		else
		{
			yield return GameMain.instance.StartCoroutine(LoadCharAsync(strLoadChar));
		}
	}

	public void SetTalkMot(string strCharKey, bool isTalk)
	{
		ChangeCharMot(strCharKey, string.Empty, string.Empty, isSaveCurMot: true, isTalk);
		if (m_EventEngine != null)
		{
			m_EventEngine.m_EventObject.PlayTalkMotion(strCharKey, isTalk);
		}
	}

	public void SetCharBlack(string strCharKey, bool isBlack)
	{
		TalkCharUnit talkCharUnit = FindCharUnit(strCharKey);
		if (talkCharUnit != null)
		{
			talkCharUnit.m_isBlackOn = isBlack;
			SpriteRenderer[] componentsInChildren = talkCharUnit.m_goChar.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			int num = componentsInChildren.Length;
			int num2 = ((!isBlack) ? 255 : 0);
			talkCharUnit.m_iSetColor = 255;
			for (int i = 0; i < num; i++)
			{
				componentsInChildren[i].color = new Color(num2, num2, num2, 255f);
			}
		}
	}

	public void SetDefMotion(string strCharKey, string strMotionKey, int iSetDir)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit != null)
		{
			Xls.CharMotData data_byKey = Xls.CharMotData.GetData_byKey(strMotionKey);
			if (data_byKey != null)
			{
				talkCharUnit.m_strMotKey = strMotionKey;
			}
		}
	}

	public void ChangeCharMot(string strCharKey, string strMotion, string strDir, bool isSaveCurMot = true, bool isTalk = false)
	{
		int iSetDir = ((!strDir.Equals(string.Empty)) ? GameGlobalUtil.GetXlsScriptKeyValue(strDir) : (-1));
		ChangeCharMot(strCharKey, strMotion, iSetDir, isSaveCurMot, isTalk);
	}

	public void ChangeCharMot(string strCharKey, string strMotionKey, int iSetDir, bool isSaveCurMot = true, bool isTalk = false)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit == null || talkCharUnit.m_goChar == m_goForCharPosCalc)
		{
			return;
		}
		string text = string.Empty;
		if (strMotionKey.Equals(string.Empty))
		{
			strMotionKey = talkCharUnit.m_strMotKey;
		}
		Xls.CharMotData data_byKey = Xls.CharMotData.GetData_byKey(strMotionKey);
		if (data_byKey != null)
		{
			text = data_byKey.m_strMotName;
		}
		if (iSetDir == -1)
		{
			iSetDir = talkCharUnit.m_iDir;
		}
		Xls.CharData data_byKey2 = Xls.CharData.GetData_byKey(strCharKey);
		Transform transform = talkCharUnit.m_goChar.transform;
		if (data_byKey2 == null)
		{
			return;
		}
		float num = Mathf.Abs(Mathf.Round(transform.localRotation.eulerAngles.y));
		bool flag = 180f == num;
		int num2 = ((data_byKey2.m_iShowLeft != 0) ? ((!flag) ? 1 : 0) : (flag ? 1 : 0));
		if (num2 != iSetDir)
		{
			transform.Rotate(new Vector3(0f, 180f, 0f));
		}
		if (isSaveCurMot)
		{
			talkCharUnit.m_strMotKey = strMotionKey;
		}
		if (talkCharUnit.m_clCharAnimatorPlay != null && talkCharUnit.m_clCharAnimatorPlay.gameObject.activeInHierarchy)
		{
			talkCharUnit.m_clCharAnimatorPlay.PlayAnimation(text, isTalk);
		}
		else
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("CHR_ANI_MOT_TALK");
			string stateName = text + ((!isTalk || xlsProgramDefineStr == null) ? string.Empty : xlsProgramDefineStr);
			Animator component = talkCharUnit.m_goChar.transform.GetChild(0).GetComponent<Animator>();
			if (component != null && talkCharUnit.m_goChar.activeInHierarchy)
			{
				component.Rebind();
				component.Play(stateName);
			}
		}
		if (isSaveCurMot)
		{
			talkCharUnit.m_iDir = iSetDir;
		}
	}

	public bool SetRotate(string strCharKey, float fZ, float fTime, string strSpeedType)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit == null)
		{
			return false;
		}
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		talkCharUnit.m_iRotateType = xlsScriptKeyValue;
		talkCharUnit.m_fRotateStartZAngle = talkCharUnit.m_goChar.transform.localEulerAngles.z;
		talkCharUnit.m_fRotateDestZAngle = fZ;
		talkCharUnit.m_fRotateTime = fTime;
		talkCharUnit.m_fRotatePassTime = 0f;
		return true;
	}

	public bool ProcCharRotate()
	{
		if (m_listTalkChar == null)
		{
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			flag2 = true;
			float fSmoothStepFactor = 1f;
			flag2 = m_EventEngine.ProcTime(talkCharUnit.m_iRotateType, ref fSmoothStepFactor, ref talkCharUnit.m_fRotatePassTime, talkCharUnit.m_fRotateTime);
			if (flag2)
			{
				talkCharUnit.m_fRotateStartZAngle = talkCharUnit.m_fRotateDestZAngle;
				talkCharUnit.m_goChar.transform.localEulerAngles = new Vector3(talkCharUnit.m_goChar.transform.localEulerAngles.x, talkCharUnit.m_goChar.transform.localEulerAngles.y, talkCharUnit.m_fRotateDestZAngle);
			}
			else
			{
				float z = Mathf.Lerp(talkCharUnit.m_fRotateStartZAngle, talkCharUnit.m_fRotateDestZAngle, fSmoothStepFactor);
				talkCharUnit.m_goChar.transform.localEulerAngles = new Vector3(talkCharUnit.m_goChar.transform.localEulerAngles.x, talkCharUnit.m_goChar.transform.localEulerAngles.y, z);
			}
			flag = flag && flag2;
		}
		return flag;
	}

	public void PrintCharMove(string strPrintTitle, string strCharKey, string strPos, string strSize, float fY = 999f, bool isSaveData = false)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		int iSize = ((strSize == null) ? talkCharUnit.m_iSize : GameGlobalUtil.GetXlsScriptKeyValue(strSize));
		int iPos = ((strPos == null) ? talkCharUnit.m_iPosition : GameGlobalUtil.GetXlsScriptKeyValue(strPos));
		Vector2 charPosVec = GetCharPosVec(strCharKey, iSize, iPos, isSaveData);
		if (talkCharUnit != null)
		{
			Vector3 posWorldPointToViewPort = m_GameSwitch.GetPosWorldPointToViewPort(talkCharUnit.m_goChar.transform.position);
			if (fY != 999f)
			{
			}
		}
		else if (fY != 999f)
		{
		}
	}

	public bool SetCharMove(string strCharKey, string strPosition, float fTime, string strSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strPosition);
		int xlsScriptKeyValue2 = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		return SetCharMoveXlsOrData(strCharKey, xlsScriptKeyValue, fTime, xlsScriptKeyValue2);
	}

	public bool SetCharMove(string strCharKey, string strPosition, float fTime, int iSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strPosition);
		return SetCharMoveXlsOrData(strCharKey, xlsScriptKeyValue, fTime, iSpeedType);
	}

	public bool SetCharMoveRevert(string strCharKey, float fTime, int iSpeed)
	{
		if (m_listTalkChar == null)
		{
			return false;
		}
		TalkCharUnit talkCharUnit = null;
		int count = m_listTalkChar.Count;
		bool result = false;
		for (int i = 0; i < count; i++)
		{
			if (m_listTalkChar[i].m_strCharKey == strCharKey)
			{
				talkCharUnit = m_listTalkChar[i];
				break;
			}
		}
		m_GameSwitch.InitAllPivotXY(strCharKey);
		if (talkCharUnit != null)
		{
			result = SetCharMove(talkCharUnit.m_strCharKey, talkCharUnit.m_iPosition, fTime, iSpeed);
		}
		return result;
	}

	private void SetCharMoveImmediately(TalkCharUnit tcChar, bool isSetPos)
	{
		if (isSetPos)
		{
			tcChar.m_iPosition = tcChar.m_iTmpPosition;
		}
		if (tcChar.m_isSetSkipPos)
		{
			m_goForCharPosCalc.transform.localPosition = new Vector3(tcChar.m_vecSkipPos.x, tcChar.m_vecSkipPos.y, CHR_SCREEN_Z_POS);
			tcChar.m_goChar.transform.position = m_goForCharPosCalc.transform.position;
			Vector3 posWorldPointToViewPort = m_GameSwitch.GetPosWorldPointToViewPort(m_goForCharPosCalc.transform.position);
			tcChar.m_vecDestPos = (tcChar.m_vecStartPos = new Vector2(posWorldPointToViewPort.x, posWorldPointToViewPort.y));
			tcChar.m_isSetSkipPos = false;
		}
		else
		{
			tcChar.m_goChar.transform.position = m_GameSwitch.GetPosByPercentZeroToOne(tcChar.m_vecDestPos.x, tcChar.m_vecDestPos.y, CHR_SCREEN_Z_POS);
			tcChar.m_vecStartPos = tcChar.m_vecDestPos;
		}
		tcChar.m_iMoveSpeedType = 5;
	}

	public bool SetCharMoveDetailWorld(string strCharKey, float fX, float fY, float fZ, float fTime, string strSpeedType)
	{
		Vector3 posWorldPointToViewPort = m_GameSwitch.GetPosWorldPointToViewPort(new Vector3(fX, fY, fZ));
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		if (m_goForCharPosCalc == null)
		{
			return false;
		}
		m_goForCharPosCalc.transform.localPosition = new Vector3(fX, fY, fZ);
		posWorldPointToViewPort = m_GameSwitch.GetPosWorldPointToViewPort(m_goForCharPosCalc.transform.position);
		return SetCharMoveDetail(strCharKey, (fX != 999f) ? posWorldPointToViewPort.x : 999f, (fY != 999f) ? posWorldPointToViewPort.y : 999f, fTime, xlsScriptKeyValue);
	}

	public void SetCharSkipWorldPos(string strCharKey, float fX, float fY)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit != null)
		{
			if (fX == 999f)
			{
				fX = talkCharUnit.m_goChar.transform.localPosition.x;
			}
			if (fY == 999f)
			{
				fY = talkCharUnit.m_goChar.transform.localPosition.y;
			}
			talkCharUnit.m_vecSkipPos = new Vector2(fX, fY);
			talkCharUnit.m_isSetSkipPos = true;
		}
	}

	public bool SetCharMoveDetail(string strCharKey, float fX, float fY, float fTime, int iSpeedType, bool isSavePivot = true)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit == null)
		{
			return false;
		}
		if (fX == 999f)
		{
			fX = ((Vector2)m_GameSwitch.GetPosWorldPointToViewPort(talkCharUnit.m_goChar.transform.position)).x;
		}
		if (fY == 999f)
		{
			fY = ((Vector2)m_GameSwitch.GetPosWorldPointToViewPort(talkCharUnit.m_goChar.transform.position)).y;
		}
		if (isSavePivot)
		{
			m_GameSwitch.SetPivotXY(strCharKey, talkCharUnit.m_iSize, talkCharUnit.m_iPosition, fX, fY);
		}
		Vector3 posWorldPointToViewPort = m_GameSwitch.GetPosWorldPointToViewPort(talkCharUnit.m_goChar.transform.position);
		talkCharUnit.m_vecStartPos.x = posWorldPointToViewPort.x;
		talkCharUnit.m_vecStartPos.y = posWorldPointToViewPort.y;
		Vector2 vecDestPos = new Vector3(fX, fY);
		talkCharUnit.m_isSetPos = false;
		talkCharUnit.m_iTmpPosition = talkCharUnit.m_iPosition;
		talkCharUnit.m_vecMoveStart = talkCharUnit.m_vecStartPos;
		talkCharUnit.m_vecDestPos = vecDestPos;
		talkCharUnit.m_iMoveSpeedType = iSpeedType;
		talkCharUnit.m_fMoveTime = fTime;
		talkCharUnit.m_fMovePassTime = 0f;
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			SetCharMoveImmediately(talkCharUnit, isSetPos: false);
			return false;
		}
		return true;
	}

	public bool SetCharMoveXlsOrData(string strCharKey, int iMovePos, float fTime, int iSpeedType)
	{
		TalkCharUnit tcTemp = GetTalkCharUnit(strCharKey);
		if (tcTemp == null)
		{
			return false;
		}
		if (strCharKey == FOR_CHAR_POS_CALC)
		{
			return false;
		}
		tcTemp.m_isSetPos = true;
		SetCharPosXlsOrData(ref tcTemp, strCharKey, iMovePos, tcTemp.m_iSize);
		tcTemp.m_vecMoveStart = tcTemp.m_vecStartPos;
		tcTemp.m_iTmpPosition = iMovePos;
		tcTemp.m_iMoveSpeedType = iSpeedType;
		tcTemp.m_fMoveTime = fTime;
		tcTemp.m_fMovePassTime = 0f;
		return true;
	}

	public bool SetCharMove(string strCharKey, int iMovePos, float fTime, int iSpeedType)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit == null)
		{
			return false;
		}
		talkCharUnit.m_isSetPos = true;
		talkCharUnit.m_vecDestPos = GetCharPosVec(strCharKey, talkCharUnit.m_iSize, iMovePos);
		talkCharUnit.m_iTmpPosition = iMovePos;
		talkCharUnit.m_iMoveSpeedType = iSpeedType;
		talkCharUnit.m_fMoveTime = fTime;
		talkCharUnit.m_fMovePassTime = 0f;
		return true;
	}

	public bool ProcCharMove(bool isMoveDoneDel = false)
	{
		if (m_listTalkChar == null)
		{
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			flag2 = true;
			if (talkCharUnit.m_iMoveSpeedType != 5)
			{
				Transform transform = talkCharUnit.m_goChar.transform;
				float fSmoothStepFactor = 1f;
				flag2 = m_EventEngine.ProcTime(talkCharUnit.m_iMoveSpeedType, ref fSmoothStepFactor, ref talkCharUnit.m_fMovePassTime, talkCharUnit.m_fMoveTime);
				if (flag2)
				{
					talkCharUnit.m_vecStartPos = talkCharUnit.m_vecDestPos;
					SetCharMoveImmediately(talkCharUnit, talkCharUnit.m_isSetPos);
					if (isMoveDoneDel)
					{
						talkCharUnit.m_goChar.SetActive(value: false);
					}
				}
				else
				{
					talkCharUnit.m_vecStartPos = Vector2.Lerp(talkCharUnit.m_vecMoveStart, talkCharUnit.m_vecDestPos, fSmoothStepFactor);
					transform.position = m_GameSwitch.GetPosByPercentZeroToOne(talkCharUnit.m_vecStartPos.x, talkCharUnit.m_vecStartPos.y, CHR_SCREEN_Z_POS);
				}
			}
			flag = flag && flag2;
		}
		return flag;
	}

	private Vector2 GetCharPosVec(string strChar, string strSize, string strPos)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSize);
		int xlsScriptKeyValue2 = GameGlobalUtil.GetXlsScriptKeyValue(strPos);
		return GetCharPosVec(strChar, xlsScriptKeyValue, xlsScriptKeyValue2);
	}

	private Vector2 GetCharPosVec(string strChar, int iSize, int iPos, bool isSaveData = false)
	{
		Xls.CharZoomPos data_bySwitchIdx = Xls.CharZoomPos.GetData_bySwitchIdx(m_GameSwitch.GetCharGetXlsIdx(strChar, iSize, iPos));
		Vector2 result = new Vector2(data_bySwitchIdx.m_fPosX, data_bySwitchIdx.m_fPosY);
		if (isSaveData)
		{
			float num = m_GameSwitch.GetCharPivotX(strChar, iSize, iPos);
			float num2 = m_GameSwitch.GetCharPivotY(strChar, iSize);
			if (num == 999f)
			{
				num = data_bySwitchIdx.m_fPosX;
			}
			if (num2 == 999f)
			{
				num2 = data_bySwitchIdx.m_fPosY;
			}
			result = new Vector2(num, num2);
		}
		return result;
	}

	private void SetCharLayer(string strChar, string strSize, bool isOnlySortingLayer = false)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSize);
		SetCharLayer(strChar, xlsScriptKeyValue, isOnlySortingLayer);
	}

	private void SetCharLayer(string strChar, int iSize, bool isOnlySortingLayer = false)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strChar);
		if (talkCharUnit != null)
		{
			SetCharLayer(talkCharUnit.m_goChar, iSize, isOnlySortingLayer);
		}
	}

	private void SetCharLayer(GameObject go, int iSize, bool isOnlySortingLayer = false)
	{
		SpriteRenderer[] componentsInChildren = go.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		Transform[] componentsInChildren2 = go.GetComponentsInChildren<Transform>(includeInactive: true);
		string sortingLayerName = "LAYER_CHR_" + iSize;
		int num2;
		if (!isOnlySortingLayer)
		{
			int layer = (go.layer = LayerMask.NameToLayer("CHR_" + iSize));
			num2 = componentsInChildren2.Length;
			for (int i = 0; i < num2; i++)
			{
				componentsInChildren2[i].gameObject.layer = layer;
			}
		}
		num2 = componentsInChildren.Length;
		for (int j = 0; j < num2; j++)
		{
			componentsInChildren[j].sortingLayerName = sortingLayerName;
		}
	}

	public bool SetCharZoom(string strCharKey, string strSize, float fTime, int iZoomSpeed)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSize);
		return SetCharZoom(strCharKey, xlsScriptKeyValue, fTime, iZoomSpeed);
	}

	public bool SetCharZoom(string strCharKey, int iSize, float fTime, int iZoomSpeed)
	{
		return SetCharZoomWithPos(strCharKey, iSize, iZoomSpeed, isUseDest: false, Vector2.one, fTime, isSetLayer: false);
	}

	public bool SetCharZoomWithPos(string strCharKey, int iSize, int iZoomSpeed, bool isUseDest, Vector2 vecDest, float fTime = 999f, bool isSetLayer = true, bool isChangeYPos = true)
	{
		TalkCharUnit talkCharUnit = GetTalkCharUnit(strCharKey);
		if (talkCharUnit == null)
		{
			return false;
		}
		talkCharUnit.m_iZoomSpeedType = iZoomSpeed;
		talkCharUnit.m_fZoomTime = ((fTime != 999f) ? fTime : 1f);
		talkCharUnit.m_fZoomPassTime = 0f;
		int charGetXlsIdx = m_GameSwitch.GetCharGetXlsIdx(strCharKey, iSize, talkCharUnit.m_iPosition);
		Xls.CharZoomPos data_bySwitchIdx = Xls.CharZoomPos.GetData_bySwitchIdx(charGetXlsIdx);
		talkCharUnit.m_vecZoomDest = new Vector3(data_bySwitchIdx.m_fZoomValue, data_bySwitchIdx.m_fZoomValue, data_bySwitchIdx.m_fZoomValue);
		talkCharUnit.m_iSize = iSize;
		if (isUseDest)
		{
			talkCharUnit.m_vecZoomPosDest = m_GameSwitch.GetPosByPercentZeroToOne(vecDest.x, vecDest.y, CHR_SCREEN_Z_POS);
		}
		else
		{
			float fY = ((!isChangeYPos) ? talkCharUnit.m_vecDestPos.y : data_bySwitchIdx.m_fPosY);
			talkCharUnit.m_vecZoomPosDest = m_GameSwitch.GetPosByPercentZeroToOne(data_bySwitchIdx.m_fPosX, fY, CHR_SCREEN_Z_POS);
		}
		talkCharUnit.m_isSetLayer = isSetLayer;
		if (isSetLayer)
		{
			SetCharLayer(strCharKey, iSize);
		}
		if (fTime <= 0f)
		{
			Transform transform = talkCharUnit.m_goChar.transform;
			transform.localScale = talkCharUnit.m_vecZoomDest;
			transform.position = talkCharUnit.m_vecZoomPosDest;
			talkCharUnit.m_iZoomSpeedType = 5;
			return false;
		}
		talkCharUnit.m_vecZoomStart = talkCharUnit.m_goChar.transform.localScale;
		talkCharUnit.m_vecZoomPosStart = talkCharUnit.m_goChar.transform.position;
		return true;
	}

	public bool ProcCharZoom()
	{
		if (m_listTalkChar == null)
		{
			return true;
		}
		bool flag = true;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			bool flag2 = true;
			if (talkCharUnit.m_iZoomSpeedType != 5)
			{
				Transform transform = talkCharUnit.m_goChar.transform;
				float fSmoothStepFactor = 1f;
				flag2 = m_EventEngine.ProcTime(talkCharUnit.m_iZoomSpeedType, ref fSmoothStepFactor, ref talkCharUnit.m_fZoomPassTime, talkCharUnit.m_fZoomTime);
				if (flag2)
				{
					transform.localScale = talkCharUnit.m_vecZoomDest;
					transform.position = talkCharUnit.m_vecZoomPosDest;
					talkCharUnit.m_iZoomSpeedType = 5;
					if (talkCharUnit.m_isSetLayer)
					{
						SetCharLayer(talkCharUnit.m_strCharKey, talkCharUnit.m_iSize);
					}
				}
				else
				{
					transform.localScale = Vector3.Lerp(talkCharUnit.m_vecZoomStart, talkCharUnit.m_vecZoomDest, fSmoothStepFactor);
					transform.position = Vector3.Lerp(talkCharUnit.m_vecZoomPosStart, talkCharUnit.m_vecZoomPosDest, fSmoothStepFactor);
				}
			}
			flag = flag && flag2;
		}
		return flag;
	}

	public void CharHide(string strChar)
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (talkCharUnit.m_strCharKey.Equals(strChar))
			{
				talkCharUnit.m_goChar.SetActive(value: false);
				break;
			}
		}
	}

	public void CharAllHide()
	{
		if (m_listTalkChar != null)
		{
			int count = m_listTalkChar.Count;
			for (int i = 0; i < count; i++)
			{
				TalkCharUnit talkCharUnit = m_listTalkChar[i];
				ChangeCharMot(talkCharUnit.m_strCharKey, STR_NONE_MOT, string.Empty, isSaveCurMot: false);
				talkCharUnit.m_goChar.SetActive(value: false);
			}
		}
	}

	public void ChangeCharPosByCameraShake(Vector3 vecShake)
	{
		if (m_listTalkChar != null)
		{
			int count = m_listTalkChar.Count;
			for (int i = 0; i < count; i++)
			{
				Transform transform = m_listTalkChar[i].m_goChar.transform;
				transform.Translate(vecShake);
			}
		}
	}

	public GameObject GetCharObjInScreen(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return null;
		}
		int count = m_listTalkChar.Count;
		TalkCharUnit talkCharUnit = null;
		GameObject result = null;
		for (int i = 0; i < count; i++)
		{
			talkCharUnit = m_listTalkChar[i];
			if (talkCharUnit.m_strCharKey == strCharKey && talkCharUnit.m_goChar.activeInHierarchy)
			{
				result = talkCharUnit.m_goChar;
				break;
			}
		}
		return result;
	}

	public void CharDel(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (talkCharUnit.m_strCharKey == strCharKey)
			{
				ChangeCharMot(talkCharUnit.m_strCharKey, STR_NONE_MOT, string.Empty);
				talkCharUnit.m_goChar.SetActive(value: false);
				break;
			}
		}
	}

	public bool SetCharOutAll(string strOutType, float fTime, string strSpeedType)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strOutType);
		int xlsScriptKeyValue2 = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		return SetCharOutAll(xlsScriptKeyValue, fTime, xlsScriptKeyValue2);
	}

	public bool SetCharOutAll(int iOutType, float fTime, int iSpeedType)
	{
		if (m_listTalkChar == null)
		{
			return false;
		}
		bool flag = true;
		int iMovePos = CHAR_POS_LEFT_OUT;
		if (iOutType == OUT_TYPE_LEFT)
		{
			iMovePos = CHAR_POS_LEFT_OUT;
		}
		else if (iOutType == OUT_TYPE_RIGHT)
		{
			iMovePos = CHAR_POS_RIGHT_OUT;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			if (iOutType == OUT_TYPE_BOTHSIDE || iOutType == OUT_TYPE_CENTER_ALONE)
			{
				iMovePos = ((talkCharUnit.m_iPosition > 2) ? CHAR_POS_RIGHT_OUT : CHAR_POS_LEFT_OUT);
			}
			if (iOutType != OUT_TYPE_CENTER_ALONE || talkCharUnit.m_iPosition != 2)
			{
				flag |= SetCharMoveXlsOrData(talkCharUnit.m_strCharKey, iMovePos, fTime, iSpeedType);
			}
		}
		return flag;
	}

	public void CharAllDisable()
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		BitCalc.InitArray(m_isSavedAllCharEnable);
		int count = m_listTalkChar.Count;
		int iSize = 8;
		for (int i = 0; i < count; i++)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_listTalkChar[i].m_strCharKey);
			if (data_byKey != null)
			{
				int iIdx = data_byKey.m_iIdx;
				if (BitCalc.CheckArrayIdx(iIdx, iSize))
				{
					m_isSavedAllCharEnable[iIdx] = m_listTalkChar[i].m_goChar.activeInHierarchy;
				}
			}
			m_listTalkChar[i].m_goChar.SetActive(value: false);
		}
	}

	public void CharEnableFromSavedBuf()
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			Xls.CharData data_byKey = Xls.CharData.GetData_byKey(m_listTalkChar[i].m_strCharKey);
			if (data_byKey != null)
			{
				int iIdx = data_byKey.m_iIdx;
				m_listTalkChar[i].m_goChar.SetActive(m_isSavedAllCharEnable[iIdx]);
				ChangeCharMot(m_listTalkChar[i].m_strCharKey, m_listTalkChar[i].m_strMotKey, m_listTalkChar[i].m_iDir, isSaveCurMot: false);
			}
		}
	}

	public TalkCharUnit FindCharUnit(string strCharKey)
	{
		if (m_listTalkChar == null)
		{
			return null;
		}
		TalkCharUnit result = null;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listTalkChar[i].m_strCharKey.Equals(strCharKey))
			{
				result = m_listTalkChar[i];
				break;
			}
		}
		return result;
	}

	public void SetCharLux(string strChareKey, int iColor, float fTime)
	{
		TalkCharUnit talkCharUnit = FindCharUnit(strChareKey);
		if (talkCharUnit != null)
		{
			talkCharUnit.m_isSetColor = true;
			talkCharUnit.m_colorBefore = (talkCharUnit.m_colorCur = GameGlobalUtil.HexToColor(talkCharUnit.m_iSetColor));
			talkCharUnit.m_iSetColor = iColor;
			talkCharUnit.m_colorSet = GameGlobalUtil.HexToColor(iColor);
			talkCharUnit.m_fColorPassTime = 0f;
			talkCharUnit.m_fColorTime = fTime;
		}
	}

	public bool ProcCharLux()
	{
		if (m_listTalkChar == null)
		{
			return true;
		}
		bool flag = true;
		int count = m_listTalkChar.Count;
		for (int i = 0; i < count; i++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			bool flag2 = true;
			if (talkCharUnit.m_isSetColor)
			{
				flag2 = talkCharUnit.m_fColorTime >= talkCharUnit.m_fColorPassTime;
				if (flag2)
				{
					talkCharUnit.m_fColorPassTime += Time.deltaTime * m_EventEngine.GetLerpSkipValue();
					talkCharUnit.m_colorCur = Color.Lerp(talkCharUnit.m_colorBefore, talkCharUnit.m_colorSet, talkCharUnit.m_fColorPassTime / talkCharUnit.m_fColorTime);
				}
				else
				{
					talkCharUnit.m_fColorTime = (talkCharUnit.m_fColorPassTime = 0f);
					talkCharUnit.m_colorCur = talkCharUnit.m_colorSet;
					talkCharUnit.m_isSetColor = false;
				}
				SetCharColor(m_listTalkChar[i].m_strCharKey, talkCharUnit.m_colorCur);
				flag = flag && !flag2;
			}
		}
		return flag;
	}

	public void SetCharColor(string strCharKey, Color colorValue)
	{
		TalkCharUnit talkCharUnit = FindCharUnit(strCharKey);
		if (talkCharUnit != null)
		{
			SpriteRenderer[] componentsInChildren = talkCharUnit.m_goChar.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				componentsInChildren[i].color = colorValue;
			}
		}
	}

	public int GetSizeSaveTalkChar(bool isOnlyLoadFile = false)
	{
		int num = 0;
		if (m_listTalkChar != null)
		{
			if (isOnlyLoadFile)
			{
				if (m_iArrColorCharIdx != null)
				{
					num = m_iArrColorCharIdx.Length;
				}
			}
			else if (m_listTalkChar != null)
			{
				num = m_listTalkChar.Count;
			}
		}
		int sIZE_INT = GameSwitch.SIZE_INT;
		return sIZE_INT + num * (sIZE_INT * 2);
	}

	public void InitForConvertFile()
	{
		m_iArrColorCharIdx = null;
		m_iArrCharColor = null;
	}

	public void SaveTalkChar(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		if (iBefVer != -1)
		{
			int num = m_iArrColorCharIdx.Length;
			BitCalc.IntToByteNCO(num, bySaveBuf, ref iOffset);
			for (int i = 0; i < num; i++)
			{
				BitCalc.IntToByteNCO(m_iArrColorCharIdx[i], bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_iArrCharColor[i], bySaveBuf, ref iOffset);
			}
			return;
		}
		int num2 = 0;
		if (m_listTalkChar != null)
		{
			num2 = m_listTalkChar.Count;
		}
		BitCalc.IntToByteNCO(num2, bySaveBuf, ref iOffset);
		for (int j = 0; j < num2; j++)
		{
			TalkCharUnit talkCharUnit = m_listTalkChar[j];
			int charIdx = m_GameSwitch.GetCharIdx(talkCharUnit.m_strCharKey);
			BitCalc.IntToByteNCO(charIdx, bySaveBuf, ref iOffset);
			BitCalc.IntToByteNCO(talkCharUnit.m_iSetColor, bySaveBuf, ref iOffset);
		}
	}

	public void LoadTalkChar(byte[] byLoadBuf, ref int iOffset)
	{
		m_iArrColorCharIdx = null;
		m_iArrCharColor = null;
		int num = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_iArrColorCharIdx = new int[num];
		m_iArrCharColor = new int[num];
		for (int i = 0; i < num; i++)
		{
			m_iArrColorCharIdx[i] = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_iArrCharColor[i] = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		}
	}

	public void AfterLoadSetColor()
	{
		if (m_listTalkChar == null)
		{
			return;
		}
		int count = m_listTalkChar.Count;
		bool flag = false;
		if (m_iArrColorCharIdx == null)
		{
			return;
		}
		int num = m_iArrColorCharIdx.Length;
		for (int i = 0; i < count; i++)
		{
			flag = false;
			TalkCharUnit talkCharUnit = m_listTalkChar[i];
			int charIdx = m_GameSwitch.GetCharIdx(talkCharUnit.m_strCharKey);
			int j;
			for (j = 0; j < num; j++)
			{
				if (charIdx == m_iArrColorCharIdx[j])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				SetCharColor(talkCharUnit.m_strCharKey, GameGlobalUtil.HexToColor(m_iArrCharColor[j]));
				talkCharUnit.m_iSetColor = m_iArrCharColor[j];
			}
		}
		m_iArrColorCharIdx = null;
		m_iArrCharColor = null;
	}
}
