using System.Collections;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class EFF_Broadcast : MonoBehaviour
{
	private enum eState
	{
		None,
		Appear,
		Idle,
		Disappear,
		Exit
	}

	private const string c_PrefabAssetPath = "Prefabs/InGame/Game/EFF_Broadcasting";

	public GameObject m_goTop;

	private const int c_iLogoCnt = 3;

	public GameObject[] m_goLogo = new GameObject[3];

	public Animator m_animBroadcast;

	public Text m_textTitle;

	public Text m_textContent;

	public Text m_textLogoBelow;

	private GameDefine.eAnimChangeState m_eAnimChgState;

	private eState m_eState;

	private GameObject m_goEvtCanvas;

	private GameDefine.EventProc m_cbfpClose;

	private static EFF_Broadcast s_Instance;

	public static bool m_isCreating;

	public static EFF_Broadcast instance => s_Instance;

	private static IEnumerator Create()
	{
		if (!(s_Instance != null))
		{
			GameObject goLoadedObj = null;
			goLoadedObj = Object.Instantiate(MainLoadThing.instance.m_prefabEFF_BroadCasting) as GameObject;
			if (!(goLoadedObj == null))
			{
				s_Instance = goLoadedObj.transform.GetComponentInChildren<EFF_Broadcast>();
				s_Instance.m_goTop = goLoadedObj;
				s_Instance.m_goTop.SetActive(value: true);
				s_Instance.m_goEvtCanvas = EventEngine.GetInstance().GetEventCanvas();
				s_Instance.m_goTop.name = "EFF_Broadcast";
				s_Instance.m_goTop.transform.SetParent(s_Instance.m_goEvtCanvas.transform, worldPositionStays: false);
				yield return null;
			}
		}
	}

	public static IEnumerator Show(string strLogoType, string strTextListKey, GameDefine.EventProc cbfpClose = null)
	{
		m_isCreating = true;
		if (isShowAble(strLogoType, strTextListKey))
		{
			yield return GameMain.instance.StartCoroutine(Create());
			EFF_Broadcast instance = s_Instance;
			if (!(instance == null))
			{
				instance.m_cbfpClose = cbfpClose;
				instance.Make(strLogoType, strTextListKey);
				instance.SetState(eState.Appear);
				m_isCreating = false;
				yield return null;
			}
		}
	}

	public static bool isShowAble(string strLogoType, string strTextListKey)
	{
		bool result = true;
		if (string.IsNullOrEmpty(strLogoType))
		{
			result = false;
		}
		if (string.IsNullOrEmpty(strTextListKey))
		{
			result = false;
		}
		return result;
	}

	public static bool Disappear()
	{
		bool result = false;
		if (s_Instance == null)
		{
			return result;
		}
		s_Instance.SetState(eState.Disappear);
		return true;
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[3] { m_textTitle, m_textContent, m_textLogoBelow };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if (s_Instance != null && s_Instance.m_goTop != null)
		{
			Object.Destroy(s_Instance.m_goTop);
		}
		m_goEvtCanvas = null;
		m_cbfpClose = null;
		s_Instance = null;
	}

	private void Make(string strLogoType, string strTextListKey)
	{
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strLogoType);
		if (xlsScriptKeyValue < 3)
		{
			for (int i = 0; i < 3; i++)
			{
				m_goLogo[i].SetActive(i == xlsScriptKeyValue);
			}
			Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(strTextListKey);
			if (data_byKey != null)
			{
				m_textTitle.text = data_byKey.m_strTitle;
				m_textContent.text = data_byKey.m_strText;
			}
			m_textLogoBelow.text = GameGlobalUtil.GetXlsProgramText("BROADCAST_TYPE");
		}
	}

	private void SetState(eState state)
	{
		int num;
		GameDefine.UIAnimationState eState;
		switch (state)
		{
		case EFF_Broadcast.eState.Appear:
			num = 2;
			goto IL_002b;
		case EFF_Broadcast.eState.Disappear:
			num = 3;
			goto IL_002b;
		case EFF_Broadcast.eState.Exit:
			{
				s_Instance.m_goEvtCanvas = null;
				s_Instance = null;
				m_goTop.SetActive(value: false);
				Object.Destroy(m_goTop);
				break;
			}
			IL_002b:
			eState = (GameDefine.UIAnimationState)num;
			GameGlobalUtil.PlayUIAnimation(m_animBroadcast, eState, ref m_eAnimChgState);
			break;
		}
		m_eState = state;
	}

	private void Update()
	{
		if (m_isCreating)
		{
			return;
		}
		if (m_animBroadcast != null)
		{
			m_animBroadcast.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
		eState eState = m_eState;
		if (eState == eState.Appear || eState == eState.Disappear)
		{
			GameDefine.UIAnimationState eState2 = ((m_eState != eState.Appear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animBroadcast, eState2, ref m_eAnimChgState))
			{
				SetState((m_eState != eState.Appear) ? eState.Exit : eState.Idle);
			}
		}
	}

	public static bool IsAppearEnd()
	{
		return !m_isCreating && s_Instance != null && s_Instance.m_eState == eState.Idle;
	}

	public static bool IsDisappearEnd()
	{
		return s_Instance == null || (s_Instance != null && s_Instance.m_eState == eState.Exit);
	}
}
