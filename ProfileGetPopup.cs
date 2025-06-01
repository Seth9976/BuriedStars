using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class ProfileGetPopup : MonoBehaviour
{
	public enum eState
	{
		None = -1,
		Appear,
		Idle,
		Disappear,
		Exit
	}

	private const string c_PrefabAssetPath = "Prefabs/InGame/Game/UI_GetProfile_Popup";

	private static ProfileGetPopup s_Instance;

	private const int c_iChrCnt = 9;

	public GameObject[] m_goCharacter = new GameObject[9];

	public Text[] m_textCharacter = new Text[9];

	public Animator[] m_animCharacter = new Animator[9];

	public Text m_textTitle;

	public Text m_textContent;

	public GameObject m_goNew;

	public Text m_textNew;

	public Button m_butOk;

	public GameObject m_goBG;

	public Animator m_animBG;

	public GameObject m_goTopObj;

	public GameObject m_goPlusHObj;

	private int m_iChrHaIdx;

	private string m_strProfileKey;

	private GameDefine.EventProc m_cbfpClose;

	private Xls.Profiles m_xlsProfile;

	private GameSwitch m_GameSwitch;

	private EventEngine m_EventEngine;

	private GameDefine.eAnimChangeState m_eBGAnimState;

	private float m_fPlusH;

	private float m_fSkipDelayTime;

	private float m_fAutoDelayTime;

	public eState m_eState = eState.None;

	public static bool m_isCreating;

	public static GameObject s_srcRootObject;

	public static ProfileGetPopup instance => s_Instance;

	public static IEnumerator Create()
	{
		if (!(s_Instance != null))
		{
			GameObject goTemp = null;
			goTemp = Object.Instantiate(MainLoadThing.instance.m_prefabProfilePopup) as GameObject;
			if (!(goTemp == null))
			{
				goTemp.name = "UI_GetProfile_Popup";
				goTemp.SetActive(value: false);
				s_Instance = goTemp.transform.GetComponentInChildren<ProfileGetPopup>();
				s_srcRootObject = goTemp;
				yield return null;
			}
		}
	}

	public static void UnloadAssetBundle()
	{
		if (s_srcRootObject != null)
		{
			Object.Destroy(s_srcRootObject);
			s_srcRootObject = null;
		}
		s_Instance = null;
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[3] { m_textTitle, m_textContent, m_textNew };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		FontManager.ResetTextFontByCurrentLanguage(m_textCharacter);
	}

	private void OnDisable()
	{
		if (!(s_srcRootObject == null))
		{
			s_srcRootObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		m_strProfileKey = null;
		m_cbfpClose = null;
		m_xlsProfile = null;
		m_GameSwitch = null;
		m_EventEngine = null;
		s_srcRootObject = null;
		s_Instance = null;
	}

	public static IEnumerator Show(string strProfileKey, bool isNew, GameDefine.EventProc cbfpClose = null)
	{
		if (s_Instance == null || string.IsNullOrEmpty(strProfileKey))
		{
			yield break;
		}
		m_isCreating = true;
		if (!(s_Instance == null) && s_Instance.m_strProfileKey == null)
		{
			s_Instance.m_xlsProfile = Xls.Profiles.GetData_byKey(strProfileKey);
			if (s_Instance.m_xlsProfile != null)
			{
				Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(s_Instance.m_xlsProfile.m_strName);
				if (data_byKey != null)
				{
					Xls.CharData data_byKey2 = Xls.CharData.GetData_byKey("하수창");
					if (data_byKey2 != null)
					{
						s_Instance.m_iChrHaIdx = data_byKey2.m_iIdx;
						int charResIdx = s_Instance.GetCharResIdx();
						if (charResIdx != -1)
						{
							s_Instance.gameObject.SetActive(value: true);
							s_Instance.m_GameSwitch = GameSwitch.GetInstance();
							s_Instance.m_EventEngine = EventEngine.GetInstance();
							for (int i = 0; i < 9; i++)
							{
								if (i <= s_Instance.m_iChrHaIdx)
								{
									data_byKey2 = Xls.CharData.GetData_byIdx(i);
									if (data_byKey2 != null)
									{
										string xlsTextData = GameGlobalUtil.GetXlsTextData(data_byKey2.m_strNameKey);
										if (xlsTextData != null)
										{
											s_Instance.m_textCharacter[i].text = xlsTextData;
										}
									}
								}
								else
								{
									s_Instance.m_textCharacter[i].text = GameGlobalUtil.GetXlsProgramText("CHR_UNKNOWN");
								}
								s_Instance.m_goCharacter[i].SetActive(value: false);
							}
							s_Instance.m_goCharacter[charResIdx].SetActive(value: true);
							s_Instance.m_strProfileKey = strProfileKey;
							s_Instance.m_cbfpClose = cbfpClose;
							s_Instance.m_GameSwitch = GameSwitch.GetInstance();
							s_Instance.m_goTopObj.SetActive(value: true);
							s_Instance.m_textTitle.text = data_byKey.m_strTitle;
							s_Instance.m_textContent.text = data_byKey.m_strText;
							s_Instance.m_goCharacter[charResIdx].SetActive(value: true);
							s_Instance.m_goNew.SetActive(isNew);
							s_Instance.m_textNew.text = GameGlobalUtil.GetXlsProgramText("PROFILE_POPUP_NEW");
							s_Instance.m_fAutoDelayTime = s_Instance.m_GameSwitch.GetAutoDelayTime() * 2f + 2f;
							s_Instance.m_fSkipDelayTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("POPUP_SKIP_DELAY_TIME");
							s_Instance.SetState(eState.Appear);
						}
					}
				}
			}
		}
		m_isCreating = false;
	}

	private int GetCharResIdx()
	{
		if (s_Instance == null)
		{
			return -1;
		}
		if (s_Instance.m_xlsProfile == null)
		{
			return -1;
		}
		int iChrHaIdx = s_Instance.m_iChrHaIdx;
		int iCtgIdx = s_Instance.m_xlsProfile.m_iCtgIdx;
		int num = iCtgIdx;
		if (iCtgIdx == iChrHaIdx)
		{
			num++;
		}
		return num;
	}

	private void SetState(eState state)
	{
		int charResIdx = s_Instance.GetCharResIdx();
		if (charResIdx == -1)
		{
			return;
		}
		switch (state)
		{
		case ProfileGetPopup.eState.Appear:
		case ProfileGetPopup.eState.Disappear:
		{
			Animator animator = m_animCharacter[charResIdx];
			GameDefine.UIAnimationState eState = GameDefine.UIAnimationState.appear;
			m_eBGAnimState = GameDefine.eAnimChangeState.none;
			if (state == ProfileGetPopup.eState.Disappear)
			{
				eState = GameDefine.UIAnimationState.disappear;
			}
			else
			{
				AudioManager.instance.PlayUISound("View_Profile");
			}
			GameGlobalUtil.PlayUIAnimation(m_animBG, eState, ref m_eBGAnimState);
			GameGlobalUtil.PlayUIAnimation(animator, eState);
			break;
		}
		case ProfileGetPopup.eState.Exit:
			s_srcRootObject.SetActive(value: false);
			s_Instance.m_strProfileKey = null;
			if (m_cbfpClose != null)
			{
				m_cbfpClose(null, null);
			}
			break;
		}
		m_eState = state;
	}

	private void Update()
	{
		if (m_animBG != null && m_EventEngine != null)
		{
			m_animBG.speed = m_EventEngine.GetAnimatorSkipValue();
		}
		switch (m_eState)
		{
		case ProfileGetPopup.eState.Appear:
		case ProfileGetPopup.eState.Disappear:
		{
			GameDefine.UIAnimationState eState = ((m_eState != ProfileGetPopup.eState.Appear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
			GameGlobalUtil.CheckPlayEndUIAnimation(m_animBG, eState, ref m_eBGAnimState);
			if (m_eBGAnimState == GameDefine.eAnimChangeState.play_end)
			{
				SetState((m_eState == ProfileGetPopup.eState.Appear) ? ProfileGetPopup.eState.Idle : ProfileGetPopup.eState.Exit);
			}
			break;
		}
		case ProfileGetPopup.eState.Idle:
		{
			bool flag = false;
			if (m_EventEngine.GetAuto())
			{
				m_fAutoDelayTime -= Time.deltaTime;
				flag = m_fAutoDelayTime <= 0f;
			}
			else if (m_EventEngine.GetSkip())
			{
				m_fSkipDelayTime -= Time.deltaTime;
				flag = m_fSkipDelayTime <= 0f;
			}
			else
			{
				if (PopupDialoguePlus.IsAnyPopupActivated())
				{
					break;
				}
				flag = ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_butOk);
				if (flag)
				{
					AudioManager.instance.PlayUISound("Push_PopOK");
				}
			}
			if (flag)
			{
				SetState(ProfileGetPopup.eState.Disappear);
			}
			break;
		}
		}
	}

	public void TouchProfilePop()
	{
		if (MainLoadThing.instance.IsTouchableState() && !m_EventEngine.GetAuto() && !m_EventEngine.GetSkip() && !PopupDialoguePlus.IsAnyPopupActivated() && m_eState == eState.Idle)
		{
			SetState(eState.Disappear);
		}
	}

	public static bool IsProfilePopupEnd()
	{
		bool flag = false;
		if (m_isCreating)
		{
			return false;
		}
		return s_Instance != null && s_Instance.m_eState == eState.Exit;
	}
}
