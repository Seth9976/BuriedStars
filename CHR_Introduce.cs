using System.Collections;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class CHR_Introduce : MonoBehaviour
{
	private enum eState
	{
		None,
		Appear,
		Idle,
		Disappear,
		Exit
	}

	private const string c_PrefabAssetPath = "Prefabs/InGame/Game/EFF_CHR_Introduce_rank";

	public GameObject m_goTop;

	public Text m_textCharName;

	public Text m_textRank;

	public Text m_textScroe;

	public Animator m_animIntroduce;

	private GameDefine.eAnimChangeState m_eAnimChgState;

	private static CHR_Introduce s_Instance;

	private eState m_eState;

	private GameObject m_goEvtCanvas;

	private GameDefine.EventProc m_cbfpClose;

	private string m_strLoadAssetBundleName;

	private static bool m_isCreating;

	public static CHR_Introduce instance => s_Instance;

	private static IEnumerator Create(int iRank)
	{
		if (!(s_Instance != null))
		{
			string strFilePath = "Prefabs/InGame/Game/EFF_CHR_Introduce_rank" + iRank;
			GameObject goTemp = null;
			goTemp = Object.Instantiate(MainLoadThing.instance.m_prefabEFF_CHR_Introduce_rank[iRank - 1]) as GameObject;
			if (!(goTemp == null))
			{
				s_Instance = goTemp.transform.GetComponentInChildren<CHR_Introduce>();
				s_Instance.m_goTop = goTemp;
				s_Instance.m_goTop.SetActive(value: true);
				s_Instance.m_goEvtCanvas = EventEngine.GetInstance().GetEventCanvas();
				s_Instance.m_strLoadAssetBundleName = strFilePath;
				s_Instance.m_goTop.name = "EFF_Introduce" + iRank;
				s_Instance.m_goTop.transform.SetParent(s_Instance.m_goEvtCanvas.transform, worldPositionStays: false);
				yield return null;
			}
		}
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[3] { m_textCharName, m_textRank, m_textScroe };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void OnDestroy()
	{
		m_goEvtCanvas = null;
		m_cbfpClose = null;
		m_strLoadAssetBundleName = null;
		s_Instance = null;
	}

	public static int GetRankNum(string strCharKey, string strTextListKey)
	{
		int result = 0;
		if (!string.IsNullOrEmpty(strCharKey) && !string.IsNullOrEmpty(strTextListKey))
		{
			switch (strCharKey)
			{
			case "한도윤":
				result = 4;
				break;
			case "민주영":
				result = 2;
				break;
			case "서혜성":
				result = 5;
				break;
			case "오인하":
				result = 3;
				break;
			case "이규혁":
				result = 1;
				break;
			}
		}
		return result;
	}

	public static IEnumerator Show(string strCharKey, string strTextListKey, GameDefine.EventProc cbfpClose = null)
	{
		m_isCreating = true;
		int iRankNum = GetRankNum(strCharKey, strTextListKey);
		if (iRankNum == 0)
		{
			m_isCreating = false;
			yield break;
		}
		yield return GameMain.instance.StartCoroutine(Create(iRankNum));
		CHR_Introduce instance = s_Instance;
		if (instance == null)
		{
			m_isCreating = false;
			yield break;
		}
		instance.Make(strCharKey, strTextListKey);
		instance.m_cbfpClose = cbfpClose;
		m_isCreating = false;
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

	private void Make(string strCharKey, string strTextListKey)
	{
		Xls.CharData data_byKey = Xls.CharData.GetData_byKey(strCharKey);
		if (data_byKey != null)
		{
			Xls.TextData data_byKey2 = Xls.TextData.GetData_byKey(data_byKey.m_strNameKey);
			if (data_byKey2 != null)
			{
				m_textCharName.text = data_byKey2.m_strTxt;
			}
		}
		Xls.TextListData data_byKey3 = Xls.TextListData.GetData_byKey(strTextListKey);
		if (data_byKey3 != null)
		{
			m_textRank.text = data_byKey3.m_strTitle;
			m_textScroe.text = data_byKey3.m_strText;
		}
		m_goTop.SetActive(value: true);
		SetState(eState.Appear);
	}

	private void SetState(eState state)
	{
		int num;
		GameDefine.UIAnimationState eState;
		switch (state)
		{
		case CHR_Introduce.eState.Appear:
			num = 2;
			goto IL_002b;
		case CHR_Introduce.eState.Disappear:
			num = 3;
			goto IL_002b;
		case CHR_Introduce.eState.Exit:
			{
				s_Instance.m_strLoadAssetBundleName = null;
				s_Instance.m_goEvtCanvas = null;
				s_Instance = null;
				m_goTop.SetActive(value: false);
				Object.Destroy(m_goTop);
				break;
			}
			IL_002b:
			eState = (GameDefine.UIAnimationState)num;
			GameGlobalUtil.PlayUIAnimation(m_animIntroduce, eState, ref m_eAnimChgState);
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
		if (m_animIntroduce != null)
		{
			m_animIntroduce.speed = EventEngine.GetInstance().GetAnimatorSkipValue();
		}
		eState eState = m_eState;
		if (eState == eState.Appear || eState == eState.Disappear)
		{
			GameDefine.UIAnimationState eState2 = ((m_eState != eState.Appear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animIntroduce, eState2, ref m_eAnimChgState))
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
