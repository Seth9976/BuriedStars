using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace GameEvent;

public class EventObject
{
	private enum UI_STRETCH_TYPE
	{
		NONE,
		W_STRETCH,
		H_STRETCH,
		BOTH_STRETCH
	}

	private enum UI_ANCHOR
	{
		TOP = 1,
		VCENTER = 2,
		BOTTOM = 4,
		LEFT = 8,
		HCENTER = 0x10,
		RIGHT = 0x20,
		W_STRETCH = 0x40,
		H_STRETCH = 0x80
	}

	public class ObjectUnit
	{
		public bool m_isUIObj;

		public string m_strName;

		public bool m_isCreatePrefab;

		public GameObject m_goObj;

		public bool m_isAnimation;

		public SpriteRenderer m_srObj;

		public Image m_imgObj;

		public Animator m_aniObj;

		public Transform m_transform;

		public RectTransform m_rectTransform;

		public bool m_isSetMove;

		public int m_iMoveSpeedType;

		public float m_fMoveTime;

		public float m_fMovePassTime;

		public Vector3 m_vecMoveDest;

		public Vector3 m_vecMoveStart;

		public Vector2 m_vecUIMoveStart;

		public Vector2 m_vecUIMoveDest;

		public Vector2 m_vecUIMoveCur;

		public Vector3 m_vecMoveUIFirstPos;

		public Vector2 m_vecMoveUILTFirst;

		public Vector2 m_vecMoveUIRBFirst;

		public Vector3 m_vecMoveUIPos;

		public Vector2 m_vecMoveUILT;

		public Vector2 m_vecMoveUIRB;

		public byte m_byAnchor;

		public bool m_isYRevert;

		public int m_iZoomSpeedType;

		public float m_fZoomTime;

		public float m_fZoomPassTime;

		public Vector3 m_vecZoomDest;

		public Vector3 m_vecZoomStart;

		public bool m_isZoomSet;

		public int m_iRotateSpeedType;

		public float m_fRotateTime;

		public float m_fRotatePassTime;

		public float m_fRotateStartZ;

		public float m_fRotateDestZ;

		public bool m_isRotate;

		public float m_fAlpha;

		public float m_fAlphaTime;

		public float m_fAlphaPassTime;

		public float m_fDestAlpha;

		public bool m_isAlphaEff;

		public bool m_isAlphaSet;

		public string m_strCheckAnimationName;

		public AnimatorStateInfo m_asiCheckAni;

		public bool m_isSetColor;

		public int m_iSetColor = 16777215;

		public Color m_colorSet = Color.white;

		public Color m_colorBefore = Color.white;

		public Color m_colorCur = Color.white;

		public float m_fColorTime;

		public float m_fColorPassTime;
	}

	public class ObjCharUnit
	{
		public ObjectUnit m_clObjUnit;

		public string m_strCharKey;

		public string m_strCharMot;

		public bool m_isHasTalkMot;
	}

	private enum eCIState
	{
		Create,
		Appear,
		Disappear,
		None
	}

	private GameSwitch m_GameSwitch;

	private EventEngine m_EventEngine;

	private Camera m_mainCam;

	private Transform m_tfEventCanvasEff;

	private Transform m_tfEventCanvas;

	private GameObject m_goEventCanvasEff;

	private GameObject m_goEventCanvas;

	private GameObject m_goBaseInvestIcon;

	private Vector3 m_vecMadeFindMarkerCamera = Vector3.zero;

	private GameObject[] m_goFindMarker = new GameObject[ConstGameSwitch.COUNT_INVEST_OBJ];

	private string[] m_strFindObjName = new string[ConstGameSwitch.COUNT_INVEST_OBJ];

	private Animator[] m_animFindMarker = new Animator[ConstGameSwitch.COUNT_INVEST_OBJ];

	private GameObject[] m_goOkButton = new GameObject[ConstGameSwitch.COUNT_INVEST_OBJ];

	private Button[] m_butFindMarker = new Button[ConstGameSwitch.COUNT_INVEST_OBJ];

	private GameDefine.eAnimChangeState m_eEndMotState;

	private string STR_INVEST_OBJ = "INVEST_OBJ";

	private GameObject m_goInvestName;

	private Animator m_animInvestName;

	private Text m_textInvestName;

	private bool m_isObjRotateSetting;

	private bool m_isObjRevertYing;

	private List<ObjectUnit> m_listObject;

	private Vector2 m_vecCanvasSize;

	private bool m_isCheckDelay;

	private List<ObjCharUnit> m_listCharObject;

	private GameObject m_goCI;

	private Image m_imgCI;

	private float m_fCIAlphaTime;

	private float m_fCIAlphaPassTime;

	private float m_fCIAlpha;

	private float m_fCIDestAlpha;

	private eCIState m_eCIState = eCIState.None;

	private const string c_strEventMarkerDetect = "Prefabs/InGame/Game/Icon_EventMarkerDetect";

	private const string c_strMarkerDetect_Name = "Prefabs/InGame/Game/MarkerDetect_Name";

	private bool m_isInitailized;

	private bool m_isCharObjectCreating;

	private ObjCharUnit m_ocunitCharCreate;

	private bool[] m_isObjCreating = new bool[30];

	private ObjectUnit[] m_ounitCreate = new ObjectUnit[30];

	private bool m_isObjectCreating;

	private const int m_iFindArrayCnt = 30;

	private bool[] m_isFindingObj = new bool[30];

	private ObjectUnit[] m_ounitFind = new ObjectUnit[30];

	private bool m_isDeleteComp;

	private bool m_isObjMoveSetting;

	private bool m_isObjZoomSetting;

	private bool m_isObjPlayAnimationSetting;

	private int m_iHitFindObjIdx = -1;

	private bool m_isSetObjLuxSetting;

	private string[] m_strArrLoadColorObjName;

	private int[] m_iArrLoadColor;

	public void Free()
	{
		if (m_isInitailized)
		{
			m_goCI = null;
			m_imgCI = null;
			if (m_listCharObject != null)
			{
				m_listCharObject.Clear();
			}
			m_listCharObject = null;
			DeleteListObj();
			m_textInvestName = null;
			m_animInvestName = null;
			m_goInvestName = null;
			AllFindMakrerDestroy();
			int cOUNT_INVEST_OBJ = ConstGameSwitch.COUNT_INVEST_OBJ;
			for (int i = 0; i < cOUNT_INVEST_OBJ; i++)
			{
				m_strFindObjName[i] = null;
				m_goOkButton[i] = null;
				m_butFindMarker[i] = null;
			}
			m_goBaseInvestIcon = null;
			m_goEventCanvas = null;
			m_goEventCanvasEff = null;
			m_tfEventCanvas = null;
			m_tfEventCanvasEff = null;
			m_mainCam = null;
			m_EventEngine = null;
			if (m_GameSwitch != null)
			{
				m_GameSwitch.InitInvestObj();
			}
			m_GameSwitch = null;
			m_isInitailized = false;
		}
	}

	public IEnumerator InitAfterLoad()
	{
		if (m_listObject == null)
		{
			m_listObject = new List<ObjectUnit>();
		}
		if (m_listCharObject == null)
		{
			m_listCharObject = new List<ObjCharUnit>();
		}
		m_vecMadeFindMarkerCamera = Vector3.zero;
		m_mainCam = Camera.main;
		m_GameSwitch = GameSwitch.GetInstance();
		m_EventEngine = EventEngine.GetInstance();
		m_goEventCanvasEff = m_EventEngine.GetEventCanvasEff();
		if (m_goEventCanvasEff != null)
		{
			m_tfEventCanvasEff = m_goEventCanvasEff.transform;
			m_vecCanvasSize = m_goEventCanvasEff.GetComponent<CanvasScaler>().referenceResolution;
		}
		m_goEventCanvas = m_EventEngine.GetEventCanvas();
		if (m_goEventCanvas != null)
		{
			m_tfEventCanvas = m_goEventCanvas.transform;
		}
		m_isCheckDelay = false;
		if (m_goBaseInvestIcon == null)
		{
			m_goBaseInvestIcon = Object.Instantiate(MainLoadThing.instance.m_prefabEventMarkerDetect) as GameObject;
		}
		if (m_goInvestName == null)
		{
			m_goInvestName = Object.Instantiate(MainLoadThing.instance.m_prefabMarkerDetect_Name) as GameObject;
			m_goInvestName.name = STR_INVEST_OBJ;
			m_goInvestName.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
			m_animInvestName = m_goInvestName.transform.GetComponentInChildren<Animator>();
			m_textInvestName = m_goInvestName.transform.GetComponentInChildren<Text>();
		}
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			m_goFindMarker[i] = null;
		}
		if (m_goCI == null)
		{
			m_goCI = new GameObject();
			m_imgCI = m_goCI.AddComponent<Image>();
			m_goCI.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
			m_goCI.SetActive(value: false);
		}
		BitCalc.InitArray(m_isFindingObj);
		BitCalc.InitArray(m_isObjCreating);
		m_isInitailized = true;
		yield return null;
	}

	public void InitForConvertFile()
	{
		m_strArrLoadColorObjName = null;
		m_iArrLoadColor = null;
	}

	private void CheckStretchTypeAndAnchor(ObjectUnit ouTemp)
	{
		RectTransform component = ouTemp.m_goObj.GetComponent<RectTransform>();
		ouTemp.m_byAnchor = 0;
		if (component == null)
		{
			return;
		}
		if (component.anchorMin.x == 0f)
		{
			if (component.anchorMax.x == 0f)
			{
				ouTemp.m_byAnchor |= 8;
			}
			else if (component.anchorMax.x == 1f)
			{
				ouTemp.m_byAnchor |= 64;
			}
		}
		else if (component.anchorMin.x == 0.5f && component.anchorMax.x == 0.5f)
		{
			ouTemp.m_byAnchor |= 2;
		}
		else if (component.anchorMin.x == 1f && component.anchorMax.x == 1f)
		{
			ouTemp.m_byAnchor |= 32;
		}
		if (component.anchorMin.y == 0f)
		{
			if (component.anchorMax.y == 0f)
			{
				ouTemp.m_byAnchor |= 4;
			}
			else if (component.anchorMax.y == 1f)
			{
				ouTemp.m_byAnchor |= 128;
			}
		}
		else if (component.anchorMin.y == 0.5f && component.anchorMax.y == 0.5f)
		{
			ouTemp.m_byAnchor |= 16;
		}
		else if (component.anchorMin.y == 1f && component.anchorMax.y == 1f)
		{
			ouTemp.m_byAnchor |= 1;
		}
	}

	public IEnumerator CharObjCreate(string strObjName, string strCharKey, string strMotion, bool isHaveTalkMot)
	{
		m_isCharObjectCreating = true;
		m_ocunitCharCreate = null;
		int iListCnt = m_listCharObject.Count;
		for (int i = 0; i < iListCnt; i++)
		{
			if (m_listCharObject[i].m_strCharKey == strCharKey)
			{
				m_ocunitCharCreate = m_listCharObject[i];
				break;
			}
		}
		if (m_ocunitCharCreate == null)
		{
			int iFindObjIdx = -1;
			while (true)
			{
				iFindObjIdx = GetFindingObjIdx();
				if (iFindObjIdx != -1)
				{
					break;
				}
				yield return null;
			}
			yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
			if (m_ounitFind != null)
			{
				m_ocunitCharCreate = new ObjCharUnit();
				m_ocunitCharCreate.m_strCharKey = strCharKey;
				m_ocunitCharCreate.m_clObjUnit = m_ounitFind[iFindObjIdx];
				InitIsFindingValue(iFindObjIdx);
				m_listCharObject.Add(m_ocunitCharCreate);
			}
		}
		m_ocunitCharCreate.m_strCharMot = strMotion;
		m_ocunitCharCreate.m_isHasTalkMot = isHaveTalkMot;
		m_isCharObjectCreating = false;
	}

	public bool IsCharObjectCreateComp()
	{
		return !m_isCharObjectCreating;
	}

	public void CharObjDelete(string strObjName)
	{
		ObjCharUnit objCharUnit = null;
		int count = m_listCharObject.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listCharObject[i].m_clObjUnit.m_strName == strObjName)
			{
				objCharUnit = m_listCharObject[i];
				break;
			}
		}
		if (objCharUnit != null)
		{
			m_listCharObject.Remove(objCharUnit);
		}
	}

	public void PlayTalkMotion(string strCharKey, bool isTalk)
	{
		ObjCharUnit objCharUnit = null;
		int count = m_listCharObject.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_listCharObject[i].m_strCharKey == strCharKey)
			{
				objCharUnit = m_listCharObject[i];
				break;
			}
		}
		if (objCharUnit != null)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("CHR_ANI_MOT_TALK");
			bool flag = objCharUnit.m_isHasTalkMot && isTalk && xlsProgramDefineStr != null;
			string strAnimation = objCharUnit.m_strCharMot + ((!flag) ? string.Empty : xlsProgramDefineStr);
			if (objCharUnit.m_clObjUnit != null)
			{
				GameMain.instance.StartCoroutine(PlayAnimation(objCharUnit.m_clObjUnit.m_strName, strAnimation, isDelay: false));
			}
		}
	}

	private GameObject FindChildObj(Transform tfParent, string strObj)
	{
		GameObject gameObject = null;
		int num = 0;
		if (tfParent != null)
		{
			Transform[] componentsInChildren = tfParent.GetComponentsInChildren<Transform>();
			num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				if (componentsInChildren[i].gameObject.name == strObj)
				{
					gameObject = componentsInChildren[i].gameObject;
					break;
				}
			}
			if (gameObject == null)
			{
				num = tfParent.childCount;
				for (int j = 0; j < num; j++)
				{
					Transform child = tfParent.GetChild(j);
					if (child.name == strObj)
					{
						gameObject = child.gameObject;
						break;
					}
				}
			}
		}
		return gameObject;
	}

	private int GetCreateObjIdx()
	{
		int result = -1;
		for (int i = 0; i < 30; i++)
		{
			if (!m_isObjCreating[i])
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void InitIsCreating(int iCreateIdx)
	{
		if (iCreateIdx != -1)
		{
			m_isObjCreating[iCreateIdx] = false;
		}
	}

	public IEnumerator ObjCreate(bool isPrefab, string strObjName, float fTime, bool isPositionSet = false, float fPosX = 0f, float fPosY = 0f, bool isListAdd = true, bool isCanvasSetting = false, int iCanvasType = 0, bool isEnable = true)
	{
		int iCreateObjIdx = -1;
		while (true)
		{
			iCreateObjIdx = GetCreateObjIdx();
			if (iCreateObjIdx == -1)
			{
				yield return null;
				continue;
			}
			break;
		}
		yield return ObjCreate(iCreateObjIdx, isAlreadyFind: false, isPrefab, strObjName, fTime, isPositionSet, fPosX, fPosY, isListAdd, isCanvasSetting, iCanvasType, isEnable);
		InitIsCreating(iCreateObjIdx);
	}

	public IEnumerator ObjCreate(int iCreateIdx, bool isAlreadyFind, bool isPrefab, string strObjName, float fTime, bool isPositionSet = false, float fPosX = 0f, float fPosY = 0f, bool isListAdd = true, bool isCanvasSetting = false, int iCanvasType = 0, bool isEnable = true)
	{
		m_isObjectCreating = true;
		ObjectUnit ouCreate = (m_ounitCreate[iCreateIdx] = new ObjectUnit());
		ouCreate.m_isCreatePrefab = isPrefab;
		ouCreate.m_isYRevert = false;
		GameObject goTemp = null;
		ObjectUnit ouFind = null;
		if (!isAlreadyFind)
		{
			int iFindObjIdx = -1;
			while (true)
			{
				iFindObjIdx = GetFindingObjIdx();
				if (iFindObjIdx != -1)
				{
					break;
				}
				yield return null;
			}
			yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx, ifNotExistThenCreate: false));
			ouFind = m_ounitFind[iFindObjIdx];
			InitIsFindingValue(iFindObjIdx);
		}
		if (ouFind != null)
		{
			goTemp = ouFind.m_goObj;
			goto IL_0357;
		}
		if (isPrefab)
		{
			string strPath = "EventObjects/" + strObjName;
			yield return GameMain.instance.StartCoroutine(GameGlobalUtil.InstantiateLoadAssetBundleObj(strPath));
			goTemp = GameGlobalUtil.m_goLoadAssetBundleObject;
			if (!(goTemp == null))
			{
				goto IL_0357;
			}
			m_ounitCreate[iCreateIdx] = null;
		}
		else
		{
			goTemp = FindChildObj(m_tfEventCanvasEff, strObjName);
			if (goTemp == null)
			{
				goTemp = FindChildObj(m_tfEventCanvas, strObjName);
			}
			if (goTemp == null)
			{
				goTemp = FindChildObj(m_EventEngine.m_goPMEventParent.transform, strObjName);
			}
			if (goTemp == null && m_EventEngine.m_goEachSceneEventParent != null)
			{
				goTemp = FindChildObj(m_EventEngine.m_goEachSceneEventParent.transform, strObjName);
			}
			if (goTemp == null && SplitScreenReaction.instance != null && SplitScreenReaction.instance.gameObject != null)
			{
				goTemp = FindChildObj(SplitScreenReaction.instance.gameObject.transform, strObjName);
			}
			if (!(goTemp == null))
			{
				goto IL_0357;
			}
			m_ounitCreate[iCreateIdx] = null;
		}
		goto IL_0791;
		IL_0791:
		m_isObjectCreating = false;
		yield break;
		IL_0357:
		if (goTemp != null && isEnable)
		{
			goTemp.SetActive(value: true);
		}
		CanvasRenderer crTemp = goTemp.GetComponent<CanvasRenderer>();
		ouCreate.m_isUIObj = crTemp != null;
		if (isPrefab)
		{
			if (isCanvasSetting)
			{
				if (iCanvasType == 0)
				{
					goTemp.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
				}
				else
				{
					goTemp.transform.SetParent(m_tfEventCanvasEff, worldPositionStays: false);
				}
			}
			else if (ouCreate.m_isUIObj)
			{
				goTemp.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
			}
			else
			{
				goTemp.transform.SetParent(m_EventEngine.m_goPMEventParent.transform);
			}
		}
		if (ouCreate.m_isUIObj)
		{
			ouCreate.m_rectTransform = goTemp.GetComponent<RectTransform>();
		}
		else
		{
			ouCreate.m_transform = goTemp.GetComponent<Transform>();
		}
		if (ouFind == null)
		{
			ouCreate.m_goObj = goTemp;
			CheckStretchTypeAndAnchor(ouCreate);
		}
		if (isPrefab && ouFind == null)
		{
			isPositionSet = true;
		}
		if (isPositionSet)
		{
			ObjectUnit objectUnit = ((ouFind == null) ? ouCreate : ouFind);
			if (objectUnit.m_isUIObj)
			{
				UICalcObjPosition(objectUnit, fPosX, fPosY);
				RectTransform component = goTemp.GetComponent<RectTransform>();
				if ((objectUnit.m_byAnchor & 0x3F) != 0)
				{
					component.localPosition = ouCreate.m_vecMoveUIPos;
				}
				if ((objectUnit.m_byAnchor & 0xC0) != 0)
				{
					component.offsetMin = objectUnit.m_vecMoveUILT;
					component.offsetMax = objectUnit.m_vecMoveUIRB;
				}
			}
			else
			{
				goTemp.transform.position = m_GameSwitch.GetPosByPercent(fPosX, fPosY, goTemp.transform.position.z - m_EventEngine.m_goPMEventParent.transform.position.z);
			}
		}
		yield return null;
		if (goTemp != null && isEnable)
		{
			goTemp.SetActive(value: true);
		}
		Animator srAnimator = goTemp.GetComponent<Animator>();
		ouCreate.m_isAnimation = srAnimator != null;
		if (srAnimator != null)
		{
			ouCreate.m_aniObj = srAnimator;
		}
		else
		{
			SpriteRenderer component2 = goTemp.GetComponent<SpriteRenderer>();
			if (component2 != null)
			{
				ouCreate.m_srObj = component2;
			}
			else
			{
				Image component3 = goTemp.GetComponent<Image>();
				ouCreate.m_imgObj = component3;
				ouCreate.m_fAlpha = 0f;
				ouCreate.m_fDestAlpha = 1f;
				ouCreate.m_fAlphaPassTime = 0f;
				ouCreate.m_fAlphaTime = fTime;
				ouCreate.m_isAlphaEff = true;
				if (ouCreate.m_imgObj != null)
				{
					SetImageAlpha(ouCreate.m_imgObj, ouCreate.m_fAlpha);
				}
			}
		}
		ouCreate.m_strName = strObjName;
		if (isListAdd && ouFind == null)
		{
			m_listObject.Add(ouCreate);
		}
		goto IL_0791;
	}

	public bool IsObjCreateComp()
	{
		return !m_isObjectCreating;
	}

	private int GetFindingObjIdx()
	{
		int num = -1;
		for (int i = 0; i < 30; i++)
		{
			if (!m_isFindingObj[i])
			{
				num = i;
				break;
			}
		}
		m_isFindingObj[num] = true;
		return num;
	}

	private void InitIsFindingValue(int iFindIdx)
	{
		if (iFindIdx != -1)
		{
			m_isFindingObj[iFindIdx] = false;
		}
	}

	private IEnumerator FindObjUnit(string strObjName, int iFindingIdx, bool ifNotExistThenCreate = true, bool isEnable = true)
	{
		m_ounitFind[iFindingIdx] = null;
		int iCnt = m_listObject.Count;
		for (int i = 0; i < iCnt; i++)
		{
			if (m_listObject[i].m_strName.Equals(strObjName))
			{
				m_ounitFind[iFindingIdx] = m_listObject[i];
				break;
			}
		}
		if (m_ounitFind[iFindingIdx] == null && ifNotExistThenCreate)
		{
			int iCreateObjIdx = -1;
			do
			{
				iCreateObjIdx = GetCreateObjIdx();
				yield return null;
			}
			while (iCreateObjIdx == -1);
			yield return GameMain.instance.StartCoroutine(ObjCreate(iCreateObjIdx, isAlreadyFind: true, isPrefab: false, strObjName, 0f, isPositionSet: false, 0f, 0f, isListAdd: true, isCanvasSetting: false, 0, isEnable));
			m_ounitFind[iFindingIdx] = m_ounitCreate[iCreateObjIdx];
			InitIsCreating(iCreateObjIdx);
		}
	}

	public IEnumerator ObjDelete(string strObjName, float fTime)
	{
		m_isDeleteComp = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp == null)
		{
			m_isDeleteComp = false;
			yield break;
		}
		CharObjDelete(strObjName);
		if (!(ouTemp.m_imgObj != null) || !(fTime > 0f))
		{
			HideObject(ouTemp);
		}
		else
		{
			SetAlpha(ouTemp, isShow: false, fTime);
		}
		m_isDeleteComp = false;
	}

	public bool IsObjDeleteComp()
	{
		return !m_isDeleteComp;
	}

	public void SetAlpha(ObjectUnit ouTemp, bool isShow, float fTime)
	{
		if (!(ouTemp.m_imgObj == null))
		{
			Color color = ouTemp.m_imgObj.color;
			if (isShow)
			{
				ouTemp.m_fAlpha = 0f;
				ouTemp.m_fDestAlpha = 1f;
			}
			else
			{
				ouTemp.m_fAlpha = color.a;
				ouTemp.m_fDestAlpha = 0f;
			}
			ouTemp.m_fAlphaPassTime = 0f;
			ouTemp.m_fAlphaTime = fTime;
			ouTemp.m_isAlphaEff = true;
			ouTemp.m_imgObj.color = new Color(color.r, color.g, color.b, ouTemp.m_fAlpha);
			ouTemp.m_isAlphaSet = true;
		}
	}

	public void HideObject(ObjectUnit ouTemp)
	{
		if (ouTemp != null && !(ouTemp.m_goObj == null))
		{
			ouTemp.m_goObj.SetActive(value: false);
		}
	}

	public void DeleteListObj()
	{
		if (m_listObject == null)
		{
			return;
		}
		int count = m_listObject.Count;
		ObjectUnit objectUnit = null;
		for (int i = 0; i < count; i++)
		{
			objectUnit = m_listObject[i];
			if (objectUnit.m_isCreatePrefab)
			{
				GameGlobalUtil.UnloadAssetBundle("EventObjects/" + objectUnit.m_strName);
				Object.Destroy(objectUnit.m_goObj);
			}
		}
		m_listObject.Clear();
		m_listObject = null;
	}

	public bool ProcObjAlpha()
	{
		bool flag = true;
		bool flag2 = true;
		float num = 0f;
		int count = m_listObject.Count;
		if (count == 0)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				ObjectUnit objectUnit = m_listObject[i];
				if (objectUnit.m_isAlphaSet)
				{
					flag2 = false;
					if (objectUnit.m_isAlphaEff)
					{
						objectUnit.m_fAlphaPassTime += Time.deltaTime * m_EventEngine.GetLerpSkipValue();
						num = Mathf.Lerp(objectUnit.m_fAlpha, objectUnit.m_fDestAlpha, objectUnit.m_fAlphaPassTime / objectUnit.m_fAlphaTime);
						if (objectUnit.m_fAlphaPassTime >= objectUnit.m_fAlphaTime)
						{
							if (num == 0f)
							{
								objectUnit.m_goObj.SetActive(value: false);
							}
							num = objectUnit.m_fDestAlpha;
							objectUnit.m_isAlphaEff = false;
							flag2 = true;
						}
						if (objectUnit.m_imgObj != null)
						{
							Color color = objectUnit.m_imgObj.color;
							objectUnit.m_imgObj.color = new Color(color.r, color.g, color.b, num);
						}
						else
						{
							objectUnit.m_isAlphaEff = false;
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
					if (flag2)
					{
						objectUnit.m_isAlphaSet = false;
					}
				}
				flag = flag && flag2;
			}
			int count2 = m_listObject.Count;
			for (int j = 0; j < count2; j++)
			{
				if (m_listObject[j].m_imgObj != null && !m_listObject[j].m_isAlphaEff && m_listObject[j].m_fDestAlpha == 0f)
				{
					HideObject(m_listObject[j]);
				}
			}
		}
		return flag;
	}

	private void UICalcObjPosition(ObjectUnit ouTemp, float fX, float fY)
	{
		float x = m_vecCanvasSize.x;
		float y = m_vecCanvasSize.y;
		fX = x * fX / 100f;
		fY = y * fY / 100f;
		float num = fX;
		float num2 = fY;
		float x2 = 0f;
		float y2 = 0f;
		if ((ouTemp.m_byAnchor & 0x38) != 0)
		{
			num = fX - x / 2f;
		}
		if ((ouTemp.m_byAnchor & 7) != 0)
		{
			num2 = fY - y / 2f;
		}
		RectTransform component = ouTemp.m_goObj.GetComponent<RectTransform>();
		ouTemp.m_vecMoveUILTFirst = component.offsetMin;
		ouTemp.m_vecMoveUIRBFirst = component.offsetMax;
		ouTemp.m_vecMoveUIFirstPos = component.localPosition;
		if ((ouTemp.m_byAnchor & 0x40) == 64)
		{
			x2 = component.offsetMin.x;
			float num3 = num - component.offsetMin.x;
			ouTemp.m_vecMoveUILT = new Vector2(num, component.offsetMin.y);
			ouTemp.m_vecMoveUIRB = new Vector2(component.offsetMax.x + num3, component.offsetMax.y);
		}
		else if ((ouTemp.m_byAnchor & 0x38) != 0)
		{
			x2 = component.localPosition.x;
			ouTemp.m_vecMoveUIPos = new Vector3(num, component.localPosition.y, component.localPosition.z);
		}
		if ((ouTemp.m_byAnchor & 0x80) == 128)
		{
			y2 = component.offsetMin.y;
			float num4 = num2 - component.offsetMin.y;
			ouTemp.m_vecMoveUILT = new Vector2(ouTemp.m_vecMoveUILT.x, num2);
			ouTemp.m_vecMoveUIRB = new Vector2(ouTemp.m_vecMoveUIRB.x, component.offsetMax.y + num4);
		}
		else if ((ouTemp.m_byAnchor & 7) != 0)
		{
			y2 = component.localPosition.y;
			ouTemp.m_vecMoveUIPos = new Vector3(ouTemp.m_vecMoveUIPos.x, num2, component.localPosition.z);
		}
		ouTemp.m_vecUIMoveCur = new Vector2(x2, y2);
		ouTemp.m_vecUIMoveDest = new Vector2(num, num2);
	}

	public bool IsObjectMoveSettingComp()
	{
		return !m_isObjMoveSetting;
	}

	public IEnumerator ObjMove(string strObjName, float fX, float fY, float fTime, string strSpeed)
	{
		m_isObjMoveSetting = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null)
		{
			int iSpeed = GameGlobalUtil.GetXlsScriptKeyValue(strSpeed);
			if (iSpeed != -1)
			{
				if (ouTemp.m_isUIObj)
				{
					UICalcObjPosition(ouTemp, fX, fY);
					ouTemp.m_vecUIMoveStart = ouTemp.m_vecUIMoveCur;
				}
				else
				{
					ouTemp.m_vecMoveStart = ouTemp.m_goObj.transform.position;
					ouTemp.m_vecMoveDest = m_GameSwitch.GetPosByPercent(fX, fY, ouTemp.m_goObj.transform.position.z - m_mainCam.transform.position.z);
				}
				ouTemp.m_iMoveSpeedType = iSpeed;
				ouTemp.m_fMoveTime = fTime;
				ouTemp.m_fMovePassTime = 0f;
				ouTemp.m_isSetMove = true;
			}
		}
		m_isObjMoveSetting = false;
	}

	public bool ProcObjMove()
	{
		bool flag = true;
		Transform transform = null;
		RectTransform rectTransform = null;
		bool flag2 = true;
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			flag2 = true;
			if (objectUnit.m_isSetMove && objectUnit.m_iMoveSpeedType != 5)
			{
				if (objectUnit.m_isUIObj)
				{
					rectTransform = objectUnit.m_goObj.GetComponent<RectTransform>();
				}
				else
				{
					transform = objectUnit.m_goObj.transform;
				}
				float fSmoothStepFactor = 1f;
				flag2 = m_EventEngine.ProcTime(objectUnit.m_iMoveSpeedType, ref fSmoothStepFactor, ref objectUnit.m_fMovePassTime, objectUnit.m_fMoveTime);
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				flag3 = (objectUnit.m_byAnchor & 0x40) != 0;
				flag4 = (objectUnit.m_byAnchor & 0x80) != 0;
				flag5 = (objectUnit.m_byAnchor & 0x38) != 0;
				flag6 = (objectUnit.m_byAnchor & 7) != 0;
				if (flag2)
				{
					if (objectUnit.m_isUIObj)
					{
						if (flag5 || flag6)
						{
							rectTransform.localPosition = objectUnit.m_vecMoveUIPos;
						}
						if (flag4 || flag3)
						{
							rectTransform.offsetMin = objectUnit.m_vecMoveUILT;
							rectTransform.offsetMax = objectUnit.m_vecMoveUIRB;
						}
					}
					else
					{
						transform.position = objectUnit.m_vecMoveDest;
					}
					objectUnit.m_iMoveSpeedType = 5;
				}
				else if (objectUnit.m_isUIObj)
				{
					Vector3 localPosition = objectUnit.m_vecMoveUIFirstPos;
					Vector2 offsetMin = objectUnit.m_vecMoveUILTFirst;
					Vector2 offsetMax = objectUnit.m_vecMoveUIRBFirst;
					objectUnit.m_vecUIMoveCur = Vector2.Lerp(objectUnit.m_vecUIMoveStart, objectUnit.m_vecUIMoveDest, fSmoothStepFactor);
					offsetMin = new Vector2((!flag3) ? offsetMin.x : objectUnit.m_vecUIMoveCur.x, (!flag4) ? offsetMin.y : objectUnit.m_vecUIMoveCur.y);
					offsetMax = new Vector2((!flag3) ? offsetMax.x : (offsetMax.x + (objectUnit.m_vecUIMoveCur.x - objectUnit.m_vecMoveUILTFirst.x)), (!flag4) ? offsetMax.y : (offsetMax.y + (objectUnit.m_vecUIMoveCur.y - objectUnit.m_vecMoveUILTFirst.y)));
					localPosition = new Vector3((!flag5) ? localPosition.x : objectUnit.m_vecUIMoveCur.x, (!flag6) ? localPosition.y : objectUnit.m_vecUIMoveCur.y, localPosition.z);
					if (flag4 || flag3)
					{
						rectTransform.offsetMin = offsetMin;
						rectTransform.offsetMax = offsetMax;
					}
					if (flag5 || flag6)
					{
						rectTransform.localPosition = localPosition;
					}
				}
				else
				{
					Vector3 position = Vector3.Lerp(objectUnit.m_vecMoveStart, objectUnit.m_vecMoveDest, fSmoothStepFactor);
					transform.position = position;
				}
				if (flag2 && objectUnit.m_isSetMove)
				{
					objectUnit.m_isSetMove = false;
				}
			}
			flag = flag && flag2;
		}
		return flag;
	}

	public bool IsObjRevertYComp()
	{
		return !m_isObjRevertYing;
	}

	public IEnumerator ObjRevertY(string strObjName, bool isRevert)
	{
		m_isObjRevertYing = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null && ouTemp.m_isYRevert != isRevert)
		{
			ouTemp.m_goObj.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
			ouTemp.m_isYRevert = isRevert;
		}
		m_isObjRevertYing = false;
	}

	public bool IsObjRotateSettingComp()
	{
		return !m_isObjRotateSetting;
	}

	public IEnumerator ObjRotate(string strObjName, float fZ, float fTime, string strSpeedType, bool isClockwise)
	{
		m_isObjRotateSetting = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null && (!ouTemp.m_isUIObj || !(ouTemp.m_rectTransform == null)) && (ouTemp.m_isUIObj || !(ouTemp.m_rectTransform == null)))
		{
			int iSpeedType = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
			ouTemp.m_iRotateSpeedType = iSpeedType;
			ouTemp.m_fRotateStartZ = ((!ouTemp.m_isUIObj) ? ouTemp.m_transform.localEulerAngles.z : ouTemp.m_rectTransform.localEulerAngles.z);
			ouTemp.m_fRotateTime = fTime;
			ouTemp.m_fRotatePassTime = 0f;
			float fAngle = 0f;
			float fA = ((!isClockwise) ? ouTemp.m_fRotateStartZ : fZ);
			float fB = ((!isClockwise) ? fZ : ouTemp.m_fRotateStartZ);
			float fSub = fA - fB;
			fAngle = fSub + (float)((fSub < 0f) ? 360 : 0);
			ouTemp.m_fRotateDestZ = ouTemp.m_fRotateStartZ + fAngle;
			ouTemp.m_isRotate = true;
		}
		m_isObjRotateSetting = false;
	}

	public bool ProcRotate()
	{
		bool flag = true;
		bool flag2 = true;
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			if (objectUnit.m_isRotate)
			{
				flag2 = true;
				Vector3 vector = ((!objectUnit.m_isUIObj) ? objectUnit.m_transform.localEulerAngles : objectUnit.m_rectTransform.localEulerAngles);
				float fSmoothStepFactor = 1f;
				flag2 = m_EventEngine.ProcTime(objectUnit.m_iRotateSpeedType, ref fSmoothStepFactor, ref objectUnit.m_fRotatePassTime, objectUnit.m_fRotateTime);
				Vector3 localEulerAngles = new Vector3(z: (!flag2) ? Mathf.Lerp(objectUnit.m_fRotateStartZ, objectUnit.m_fRotateDestZ, fSmoothStepFactor) : (objectUnit.m_fRotateStartZ = objectUnit.m_fRotateDestZ), x: vector.x, y: vector.y);
				if (objectUnit.m_isUIObj)
				{
					objectUnit.m_rectTransform.localEulerAngles = localEulerAngles;
				}
				else
				{
					objectUnit.m_transform.localEulerAngles = localEulerAngles;
				}
				if (flag2)
				{
					objectUnit.m_isRotate = false;
				}
			}
			flag = flag && flag2;
		}
		return flag;
	}

	public bool IsObjZoomSettingComp()
	{
		return !m_isObjZoomSetting;
	}

	public IEnumerator ObjZoom(string strObjName, float fZoomSize, float fTime, string strSpeed)
	{
		m_isObjZoomSetting = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null)
		{
			int iZoomSpeed = GameGlobalUtil.GetXlsScriptKeyValue(strSpeed);
			if (iZoomSpeed != -1)
			{
				ouTemp.m_iZoomSpeedType = iZoomSpeed;
				ouTemp.m_fZoomTime = fTime;
				ouTemp.m_fZoomPassTime = 0f;
				ouTemp.m_vecZoomDest = new Vector3(fZoomSize, fZoomSize, fZoomSize);
				ouTemp.m_vecZoomStart = ouTemp.m_goObj.transform.localScale;
				ouTemp.m_isZoomSet = true;
			}
		}
		m_isObjZoomSetting = false;
	}

	public bool ProcObjZoom()
	{
		bool flag = true;
		bool flag2 = true;
		Transform transform = null;
		RectTransform rectTransform = null;
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			flag = true;
			if (objectUnit.m_isZoomSet && objectUnit.m_goObj != null && objectUnit.m_iZoomSpeedType != 5)
			{
				if (objectUnit.m_isUIObj)
				{
					rectTransform = objectUnit.m_goObj.GetComponent<RectTransform>();
					Vector3 localScale = rectTransform.localScale;
				}
				else
				{
					transform = objectUnit.m_goObj.transform;
					Vector3 localScale = transform.localScale;
				}
				float fSmoothStepFactor = 1f;
				flag = m_EventEngine.ProcTime(objectUnit.m_iZoomSpeedType, ref fSmoothStepFactor, ref objectUnit.m_fZoomPassTime, objectUnit.m_fZoomTime);
				if (flag)
				{
					if (objectUnit.m_isUIObj)
					{
						rectTransform.localScale = objectUnit.m_vecZoomDest;
					}
					else
					{
						transform.localScale = objectUnit.m_vecZoomDest;
					}
					objectUnit.m_iZoomSpeedType = 5;
				}
				else
				{
					Vector3 localScale2 = Vector3.Lerp(objectUnit.m_vecZoomStart, objectUnit.m_vecZoomDest, fSmoothStepFactor);
					if (objectUnit.m_isUIObj)
					{
						rectTransform.localScale = localScale2;
					}
					else
					{
						transform.localScale = localScale2;
					}
				}
				if (flag)
				{
					objectUnit.m_isZoomSet = false;
				}
			}
			flag2 = flag2 && flag;
		}
		return flag2;
	}

	public bool IsObjPlayAnimationSettingComp()
	{
		return !m_isObjPlayAnimationSetting;
	}

	public IEnumerator PlayAnimation(string strObjName, string strAnimation, bool isDelay, bool isPlayObjMot = false)
	{
		m_isObjPlayAnimationSetting = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null)
		{
			if (isPlayObjMot)
			{
				CharObjDelete(strObjName);
			}
			if (!(ouTemp.m_aniObj == null))
			{
				ouTemp.m_aniObj.speed = m_EventEngine.GetAnimatorSkipValue();
				if (!ouTemp.m_aniObj.gameObject.activeInHierarchy)
				{
					ouTemp.m_aniObj.gameObject.SetActive(value: true);
				}
				ouTemp.m_aniObj.Play(strAnimation);
				if (isDelay)
				{
					ouTemp.m_asiCheckAni = ouTemp.m_aniObj.GetCurrentAnimatorStateInfo(0);
					if (ouTemp.m_asiCheckAni.IsName(strAnimation))
					{
						ouTemp.m_strCheckAnimationName = strAnimation;
						m_isCheckDelay = true;
					}
				}
			}
		}
		m_isObjPlayAnimationSetting = false;
	}

	public void StopSkipPlayAnimation()
	{
		if (m_listObject == null)
		{
			return;
		}
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			if (objectUnit.m_goObj != null && objectUnit.m_goObj.activeInHierarchy && objectUnit.m_aniObj != null)
			{
				objectUnit.m_aniObj.speed = m_EventEngine.GetAnimatorSkipValue();
			}
		}
	}

	public bool ProcDelayAnimaton()
	{
		if (!m_isCheckDelay)
		{
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			flag = false;
			if (objectUnit.m_aniObj != null && objectUnit.m_strCheckAnimationName != null)
			{
				objectUnit.m_aniObj.speed = m_EventEngine.GetAnimatorSkipValue();
				flag = objectUnit.m_asiCheckAni.loop || !objectUnit.m_asiCheckAni.IsName(objectUnit.m_strCheckAnimationName) || objectUnit.m_asiCheckAni.normalizedTime >= 1f;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				objectUnit.m_strCheckAnimationName = null;
			}
			flag2 = flag2 && flag;
		}
		if (flag2)
		{
			m_isCheckDelay = false;
		}
		return flag2;
	}

	public void ShowFindMarker()
	{
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("FIND_MARKER_PRE_NAME");
		string text = null;
		string text2 = null;
		Xls.TextData textData = null;
		bool flag = m_vecMadeFindMarkerCamera == Vector3.zero || m_vecMadeFindMarkerCamera != Camera.main.transform.position;
		if (flag)
		{
			AllFindMakrerDestroy();
			m_vecMadeFindMarkerCamera = Camera.main.transform.position;
		}
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			if (m_GameSwitch.GetInvestObj(i) == null)
			{
				continue;
			}
			text = m_GameSwitch.GetInvestObj(i);
			textData = Xls.TextData.GetData_byKey(m_GameSwitch.GetInvestNameKey(i));
			if (textData != null)
			{
				m_strFindObjName[i] = textData.m_strTxt;
			}
			if (text != null)
			{
				text2 = xlsProgramDefineStr + text;
			}
			if (flag || m_EventEngine.ExistEvtCanvasObj(text2) == null)
			{
				if (m_goFindMarker[i] != null)
				{
					DestroyFindMarker(i);
				}
				MakeFindMarker(i, text, text2);
			}
			else
			{
				PlayAppearMot_FindObj(isAppear: true, i);
			}
			if (m_goFindMarker[i] != null)
			{
				m_goFindMarker[i].SetActive(m_GameSwitch.GetInvestShow(i));
			}
		}
		m_goInvestName.SetActive(value: true);
		if (flag)
		{
			GameGlobalUtil.PlayUIAnimation(m_animInvestName, GameDefine.UIAnimationState.appear);
		}
		m_GameSwitch.SetInvestOrder(m_goFindMarker);
		SetInvestButAnim(isFirstSet: true);
	}

	public void AllFindMakrerDestroy()
	{
		int num = m_goFindMarker.Length;
		for (int i = 0; i < num; i++)
		{
			Object.Destroy(m_goFindMarker[i]);
			m_goFindMarker[i] = null;
			m_animFindMarker[i] = null;
		}
	}

	public void SetInvestOrder()
	{
		m_GameSwitch.SetInvestOrder(m_goFindMarker);
	}

	public void SetInvestButAnim(bool isFirstSet = false)
	{
		int investSelOrder = m_GameSwitch.GetInvestSelOrder();
		for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
		{
			if (!(m_goFindMarker[i] != null))
			{
				continue;
			}
			bool flag = i == investSelOrder;
			ButtonSpriteSwap.PressButton(m_butFindMarker[i], flag);
			m_goOkButton[i].SetActive(flag);
			if (m_animFindMarker[i] != null)
			{
				if (m_animFindMarker[i].gameObject.activeInHierarchy)
				{
					m_animFindMarker[i].SetBool("m_isSel", flag);
				}
				if (!isFirstSet)
				{
					GameGlobalUtil.PlayUIAnimation(m_animFindMarker[i], (!flag) ? GameDefine.UIAnimationState.idle : GameDefine.UIAnimationState.idle2);
				}
			}
		}
		if (investSelOrder != -1)
		{
			m_textInvestName.text = m_strFindObjName[investSelOrder];
			FontManager.ResetTextFontByCurrentLanguage(m_textInvestName);
		}
	}

	private void MakeFindMarker(int iIdx, string strObjName, string strFindMarkerName)
	{
		GameObject gameObject = m_EventEngine.ExistEvtObj(strObjName);
		if (!(gameObject == null))
		{
			GameObject gameObject2 = Object.Instantiate(m_goBaseInvestIcon);
			gameObject2.name = strFindMarkerName;
			gameObject2.transform.SetParent(m_tfEventCanvas, worldPositionStays: false);
			m_goFindMarker[iIdx] = gameObject2;
			m_goFindMarker[iIdx].SetActive(value: true);
			RectTransform component = m_goEventCanvas.GetComponent<RectTransform>();
			Vector2 canvViewPosByWorldPos = m_GameSwitch.GetCanvViewPosByWorldPos(m_mainCam, component, gameObject.transform.position);
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			component2.anchoredPosition = canvViewPosByWorldPos;
			m_animFindMarker[iIdx] = gameObject2.transform.GetChild(0).GetComponent<Animator>();
			m_goOkButton[iIdx] = gameObject2.transform.GetChild(0).Find("PSButtons_O").gameObject;
			m_butFindMarker[iIdx] = gameObject2.transform.GetChild(0).Find("Marker_detect_icon").GetComponent<Button>();
		}
	}

	private void DestroyFindMarker(int iIdx)
	{
		if (iIdx != -1)
		{
			Object.Destroy(m_goFindMarker[iIdx]);
			m_goFindMarker[iIdx] = null;
			m_animFindMarker[iIdx] = null;
			m_GameSwitch.DelInvestObj(iIdx);
		}
	}

	public void PlayAppearMot_FindObj(bool isAppear, int iIdx = -1)
	{
		if (iIdx == -1)
		{
			for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
			{
				PlayFindMarkerAni(i, isAppear);
			}
		}
		else
		{
			PlayFindMarkerAni(iIdx, isAppear);
		}
		if (isAppear)
		{
			m_goInvestName.SetActive(value: true);
		}
		GameGlobalUtil.PlayUIAnimation(m_animInvestName, (!isAppear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
	}

	private void PlayFindMarkerAni(int iIdx, bool isAppear)
	{
		if (BitCalc.CheckArrayIdx(iIdx, ConstGameSwitch.COUNT_INVEST_OBJ) && m_goFindMarker[iIdx] != null && m_animFindMarker[iIdx] != null && m_GameSwitch.GetInvestShow(iIdx))
		{
			GameGlobalUtil.PlayUIAnimation(m_animFindMarker[iIdx], (!isAppear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
		}
	}

	private int GetFindMakrerObjIdx(GameObject goFind)
	{
		int result = -1;
		if (goFind != null)
		{
			for (int i = 0; i < ConstGameSwitch.COUNT_INVEST_OBJ; i++)
			{
				if (goFind == m_goFindMarker[i])
				{
					result = i;
					break;
				}
			}
		}
		return result;
	}

	public bool IsPressInvestOKButton()
	{
		return m_iHitFindObjIdx != -1;
	}

	public bool TouchFindMarker(GameObject goHit = null)
	{
		AudioManager.instance.PlayUISound("Push_Marker");
		if (goHit == null)
		{
			int investSelOrder = m_GameSwitch.GetInvestSelOrder();
			if (!BitCalc.CheckArrayIdx(investSelOrder, ConstGameSwitch.COUNT_INVEST_OBJ))
			{
				return false;
			}
			goHit = m_goFindMarker[investSelOrder];
		}
		if (m_iHitFindObjIdx != -1)
		{
			return false;
		}
		int findMakrerObjIdx = GetFindMakrerObjIdx(goHit);
		if (findMakrerObjIdx == -1)
		{
			return false;
		}
		if (m_GameSwitch.GetInvestSelOrder() == findMakrerObjIdx)
		{
			m_iHitFindObjIdx = findMakrerObjIdx;
			m_eEndMotState = GameDefine.eAnimChangeState.none;
			GameGlobalUtil.PlayUIAnimation(m_animFindMarker[m_iHitFindObjIdx], "steam_push", ref m_eEndMotState);
		}
		else
		{
			m_GameSwitch.SetInvestListIdx(findMakrerObjIdx);
			SetInvestButAnim();
		}
		return true;
	}

	public void Proc()
	{
		if (m_iHitFindObjIdx != -1 && GameGlobalUtil.CheckPlayEndUIAnimation(m_animFindMarker[m_iHitFindObjIdx], "steam_push", ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("FIND_MARKER_PRE_NAME");
			int length = xlsProgramDefineStr.Length;
			string name = m_goFindMarker[m_iHitFindObjIdx].name;
			string strGameObj = name.Substring(length, name.Length - length);
			bool flag = m_EventEngine.EnableAndRunObj(strGameObj);
			m_GameSwitch.DelInvestIdx();
			if (!flag)
			{
				ShowFindMarker();
			}
			DestroyFindMarker(m_iHitFindObjIdx);
			m_eEndMotState = GameDefine.eAnimChangeState.none;
			m_iHitFindObjIdx = -1;
		}
	}

	public bool IsSetObjLuxSettingComp()
	{
		return !m_isSetObjLuxSetting;
	}

	public IEnumerator SetObjLux(string strObjName, int iColor, float fTime)
	{
		m_isSetObjLuxSetting = true;
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx, ifNotExistThenCreate: true, isEnable: false));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null && !(ouTemp.m_goObj == null))
		{
			ouTemp.m_isSetColor = true;
			ouTemp.m_colorBefore = (ouTemp.m_colorCur = GameGlobalUtil.HexToColor(ouTemp.m_iSetColor));
			ouTemp.m_iSetColor = iColor;
			ouTemp.m_colorSet = GameGlobalUtil.HexToColor(iColor);
			ouTemp.m_fColorPassTime = 0f;
			ouTemp.m_fColorTime = fTime;
		}
		m_isSetObjLuxSetting = false;
	}

	public bool ProcObjLux()
	{
		bool flag = true;
		int count = m_listObject.Count;
		for (int i = 0; i < count; i++)
		{
			ObjectUnit objectUnit = m_listObject[i];
			bool flag2 = true;
			if (objectUnit.m_goObj != null && objectUnit.m_isSetColor)
			{
				flag2 = objectUnit.m_fColorTime >= objectUnit.m_fColorPassTime;
				if (flag2)
				{
					objectUnit.m_fColorPassTime += Time.deltaTime * m_EventEngine.GetLerpSkipValue();
					objectUnit.m_colorCur = Color.Lerp(objectUnit.m_colorBefore, objectUnit.m_colorSet, objectUnit.m_fColorPassTime / objectUnit.m_fColorTime);
				}
				else
				{
					objectUnit.m_isSetColor = false;
					objectUnit.m_fColorTime = (objectUnit.m_fColorPassTime = 0f);
					objectUnit.m_colorCur = objectUnit.m_colorSet;
				}
				GameMain.instance.StartCoroutine(SetObjColor(objectUnit.m_strName, objectUnit.m_colorCur));
				flag = flag && !flag2;
			}
		}
		return flag;
	}

	private IEnumerator SetObjColor(string strObjName, Color colorValue)
	{
		int iFindObjIdx = -1;
		while (true)
		{
			iFindObjIdx = GetFindingObjIdx();
			if (iFindObjIdx != -1)
			{
				break;
			}
			yield return null;
		}
		yield return GameMain.instance.StartCoroutine(FindObjUnit(strObjName, iFindObjIdx));
		ObjectUnit ouTemp = m_ounitFind[iFindObjIdx];
		InitIsFindingValue(iFindObjIdx);
		if (ouTemp != null && ouTemp.m_goObj != null)
		{
			Image[] componentsInChildren = ouTemp.m_goObj.GetComponentsInChildren<Image>(includeInactive: true);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				componentsInChildren[i].color = colorValue;
			}
		}
	}

	public int GetSizeSaveEvtObj(bool isOnlyLoadFile = false)
	{
		int sIZE_INT = GameSwitch.SIZE_INT;
		int num = sIZE_INT;
		int num2 = 0;
		if (isOnlyLoadFile)
		{
			if (m_strArrLoadColorObjName != null)
			{
				num2 = m_strArrLoadColorObjName.Length;
			}
		}
		else if (m_listObject != null)
		{
			num2 = m_listObject.Count;
		}
		int num3 = 0;
		string text = null;
		for (int i = 0; i < num2; i++)
		{
			text = null;
			num3 = 16777215;
			if (isOnlyLoadFile)
			{
				if (m_strArrLoadColorObjName != null && m_iArrLoadColor != null)
				{
					num3 = m_iArrLoadColor[i];
					text = m_strArrLoadColorObjName[i];
				}
			}
			else
			{
				ObjectUnit objectUnit = m_listObject[i];
				if (objectUnit != null)
				{
					num3 = objectUnit.m_iSetColor;
					text = objectUnit.m_strName;
				}
			}
			if (num3 != 16777215)
			{
				num += sIZE_INT;
				int stringToByteSize = BitCalc.GetStringToByteSize(text);
				if (stringToByteSize != 0)
				{
					num += stringToByteSize;
				}
				num += sIZE_INT;
			}
		}
		return num;
	}

	public void SaveEvtObj(byte[] bySaveBuf, ref int iOffset)
	{
		int count = m_listObject.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (m_listObject[i].m_iSetColor != 16777215)
			{
				num++;
			}
		}
		BitCalc.IntToByteNCO(num, bySaveBuf, ref iOffset);
		for (int j = 0; j < count; j++)
		{
			ObjectUnit objectUnit = m_listObject[j];
			if (objectUnit.m_iSetColor != 16777215)
			{
				BitCalc.StringToByteWithSizeNCO(objectUnit.m_strName, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(objectUnit.m_iSetColor, bySaveBuf, ref iOffset);
			}
		}
	}

	public void LoadEvtObj(byte[] byLoadBuf, ref int iOffset, int iBefVer, int iCurVer)
	{
		m_strArrLoadColorObjName = null;
		m_iArrLoadColor = null;
		int num = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		m_strArrLoadColorObjName = new string[num];
		m_iArrLoadColor = new int[num];
		for (int i = 0; i < num; i++)
		{
			m_strArrLoadColorObjName[i] = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			m_iArrLoadColor[i] = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		}
		if (iBefVer != -1)
		{
			if (m_listObject != null)
			{
				m_listObject.Clear();
				m_listObject = null;
			}
			m_listObject = new List<ObjectUnit>();
			for (int j = 0; j < num; j++)
			{
				ObjectUnit objectUnit = new ObjectUnit();
				objectUnit.m_strName = m_strArrLoadColorObjName[j];
				objectUnit.m_iSetColor = m_iArrLoadColor[j];
				m_listObject.Add(objectUnit);
			}
		}
	}

	public void AfterLoadSetColor()
	{
		if (m_listObject == null)
		{
			return;
		}
		int count = m_listObject.Count;
		if (m_strArrLoadColorObjName == null)
		{
			return;
		}
		int num = m_strArrLoadColorObjName.Length;
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			flag = false;
			int j;
			for (j = 0; j < num; j++)
			{
				if (m_listObject[i].m_strName == m_strArrLoadColorObjName[j])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				GameMain.instance.StartCoroutine(SetObjColor(m_listObject[i].m_strName, GameGlobalUtil.HexToColor(m_iArrLoadColor[j])));
			}
		}
		m_strArrLoadColorObjName = null;
		m_iArrLoadColor = null;
	}

	public void SetImageAlpha(Image imgBase, float fAlpha)
	{
		Color color = imgBase.color;
		imgBase.color = new Color(color.r, color.g, color.b, fAlpha);
	}

	public void ShowCollectImage(string strCollectKey, float fTime)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strCollectKey);
		if (data_byKey != null)
		{
			string strIDImg = data_byKey.m_strIDImg;
			m_eCIState = eCIState.Create;
			m_fCIAlphaTime = fTime;
			m_fCIAlphaPassTime = 0f;
			m_fCIAlpha = 0f;
			m_fCIDestAlpha = 1f;
			SetImageAlpha(m_imgCI, m_fCIAlpha);
			GameMain.instance.StartCoroutine(GameGlobalUtil.GetSprRequestFromImageXls(strIDImg));
		}
	}

	public void HideCollectImage(float fTime)
	{
		m_eCIState = eCIState.Disappear;
		m_fCIAlphaTime = fTime;
		m_fCIAlphaPassTime = 0f;
		m_fCIAlpha = 1f;
		m_fCIDestAlpha = 0f;
		SetImageAlpha(m_imgCI, m_fCIAlpha);
	}

	public bool IsCompleteCIImage()
	{
		bool flag = false;
		switch (m_eCIState)
		{
		case eCIState.Create:
			if (GameGlobalUtil.m_sprLoadFromImgXls != null)
			{
				m_imgCI.sprite = GameGlobalUtil.m_sprLoadFromImgXls;
				m_goCI.GetComponent<RectTransform>().sizeDelta = new Vector2(m_imgCI.preferredWidth, m_imgCI.preferredHeight);
				m_goCI.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
				m_eCIState = eCIState.Appear;
				m_goCI.SetActive(value: true);
			}
			break;
		case eCIState.Appear:
		case eCIState.Disappear:
		{
			float fAlpha = Mathf.Lerp(m_fCIAlpha, m_fCIDestAlpha, m_fCIAlphaPassTime / m_fCIAlphaTime);
			SetImageAlpha(m_imgCI, fAlpha);
			m_fCIAlphaPassTime += Time.deltaTime * m_EventEngine.GetLerpSkipValue();
			if (m_fCIAlphaPassTime >= m_fCIAlphaTime)
			{
				flag = true;
			}
			if (m_eCIState == eCIState.Disappear && flag)
			{
				Sprite sprite = m_imgCI.sprite;
				m_imgCI.sprite = null;
				m_goCI.SetActive(value: false);
			}
			break;
		}
		}
		return flag;
	}
}
