using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class KeywordGetPopupPlus : MonoBehaviour
{
	[Serializable]
	public class PopupMembers
	{
		public GameObject m_RootObject;

		public Text m_TitleText;

		public Text m_ContentText;

		public Image m_IconImage;

		public Text m_TypeText;

		public Button m_PsIconButton;

		public Text m_textSpecialKeyword;

		public Text m_textSpecialKeywordBehind;
	}

	public enum PopupType
	{
		Auto,
		Normal,
		Event,
		Inference
	}

	private enum State
	{
		none,
		appearAll,
		appearContent,
		idle,
		disappearContent,
		disappearAll
	}

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_Keyword_GetPopup";

	public GameObject m_BGRoot;

	public PopupMembers m_PopupType1 = new PopupMembers();

	public PopupMembers m_PopupType2 = new PopupMembers();

	public PopupMembers m_PopupType3 = new PopupMembers();

	private PopupMembers m_activePopup;

	private PopupType m_defPopupType;

	private PopupType m_curPopupType = PopupType.Normal;

	private State m_curState;

	private bool m_isTutotialActived;

	private bool m_isKeyLock;

	private string[] m_keywordIDs;

	private int m_curIdx;

	private GameDefine.EventProc m_fpClose;

	private Animator m_checkAnimator;

	private float m_prevAniSpeed = 1f;

	private float m_curDelayTime;

	private static KeywordGetPopupPlus s_Instance;

	private static GameObject s_srcObject;

	private static AudioManager s_AudioManager;

	private static float s_AutoModeDelayTime;

	private static float s_SkipModeDelayTime;

	private static AssetBundleObjectHandler s_assetBundleHdr;

	public static bool m_isCreating;

	private void OnDestroy()
	{
		m_activePopup = null;
		m_keywordIDs = null;
		m_fpClose = null;
		m_checkAnimator = null;
		if (s_Instance == this)
		{
			s_Instance = null;
		}
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[11]
		{
			m_PopupType1.m_TitleText, m_PopupType1.m_ContentText, m_PopupType1.m_TypeText, m_PopupType2.m_TitleText, m_PopupType2.m_ContentText, m_PopupType2.m_TypeText, m_PopupType3.m_TitleText, m_PopupType3.m_ContentText, m_PopupType3.m_TypeText, m_PopupType3.m_textSpecialKeyword,
			m_PopupType3.m_textSpecialKeywordBehind
		};
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		if (m_PopupType1.m_TypeText != null)
		{
			m_PopupType1.m_TypeText.text = GameGlobalUtil.GetXlsProgramText("KEWORD_GETPOPUP_NORMAL_KEYWORD");
		}
		if (m_PopupType2.m_TypeText != null)
		{
			m_PopupType2.m_TypeText.text = GameGlobalUtil.GetXlsProgramText("KEWORD_GETPOPUP_EVENT_KEYWORD");
		}
		if (m_PopupType3.m_TypeText != null)
		{
			m_PopupType3.m_TypeText.text = GameGlobalUtil.GetXlsProgramText("KEWORD_GETPOPUP_INFERENCE_KEYWORD");
		}
		string xlsProgramText = GameGlobalUtil.GetXlsProgramText("KEYWORD_GETPOPUP_SPECIAL_KEYWORD");
		if (m_PopupType3.m_textSpecialKeywordBehind != null)
		{
			m_PopupType3.m_textSpecialKeywordBehind.text = xlsProgramText;
		}
		if (m_PopupType3.m_textSpecialKeyword != null)
		{
			m_PopupType3.m_textSpecialKeyword.text = xlsProgramText;
		}
	}

	private void Start()
	{
		Canvas componentInChildren = base.gameObject.GetComponentInChildren<Canvas>();
		if (componentInChildren != null)
		{
			componentInChildren.sortingOrder = 100;
		}
	}

	private void Update()
	{
		if (!base.enabled || !base.gameObject.activeSelf)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		if (!GameGlobalUtil.IsAlmostSame(m_prevAniSpeed, num))
		{
			Animator[] componentsInChildren = base.gameObject.GetComponentsInChildren<Animator>();
			Animator[] array = componentsInChildren;
			foreach (Animator animator in array)
			{
				if (!(animator == null) && animator.gameObject.activeInHierarchy)
				{
					animator.speed = num;
				}
			}
			m_prevAniSpeed = num;
		}
		if (m_checkAnimator != null)
		{
			m_checkAnimator.speed = instance.GetAnimatorSkipValue();
		}
		switch (m_curState)
		{
		case State.appearAll:
		case State.appearContent:
		{
			bool flag2 = true;
			if (m_checkAnimator != null)
			{
				flag2 = m_checkAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString());
				if (!flag2 && (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton)))
				{
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopOK");
					}
					GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.idle.ToString(), m_prevAniSpeed);
					ChangeState(State.idle);
				}
			}
			if (flag2)
			{
				ChangeState(State.idle);
			}
			break;
		}
		case State.idle:
		{
			if (m_isTutotialActived || PopupDialoguePlus.IsAnyPopupActivated() || m_isKeyLock)
			{
				break;
			}
			bool flag4 = false;
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				if (s_AudioManager != null)
				{
					s_AudioManager.PlayUISound("Push_PopOK");
				}
				if (m_activePopup != null && m_activePopup.m_PsIconButton != null)
				{
					ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_activePopup.m_PsIconButton);
					ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_activePopup.m_PsIconButton);
				}
				flag4 = true;
			}
			else
			{
				if (EventEngine.GetInstance().GetSkip() && m_curDelayTime >= s_SkipModeDelayTime)
				{
					flag4 = true;
				}
				else if (EventEngine.GetInstance().GetAuto() && m_curDelayTime >= s_AutoModeDelayTime)
				{
					flag4 = true;
				}
				m_curDelayTime += Time.deltaTime;
			}
			if (flag4)
			{
				ChangeState((m_curIdx + 1 >= m_keywordIDs.Length) ? State.disappearAll : State.disappearContent);
			}
			break;
		}
		case State.disappearContent:
		{
			bool flag3 = true;
			if (m_checkAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo2 = m_checkAnimator.GetCurrentAnimatorStateInfo(0);
				flag3 = currentAnimatorStateInfo2.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo2.normalizedTime >= 0.99f;
			}
			if (flag3)
			{
				GameMain.instance.StartCoroutine(SetCurrentContentIdx(m_curIdx + 1));
				ChangeState(State.appearContent);
			}
			break;
		}
		case State.disappearAll:
		{
			bool flag = true;
			if (m_checkAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_checkAnimator.GetCurrentAnimatorStateInfo(0);
				flag = currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f;
			}
			if (flag)
			{
				ChangeState(State.none);
			}
			break;
		}
		}
	}

	private void ChangeState(State newState, bool isIgnoreSame = true)
	{
		if (m_curState == newState && isIgnoreSame)
		{
			return;
		}
		m_checkAnimator = null;
		EventEngine instance = EventEngine.GetInstance();
		m_prevAniSpeed = ((!instance.GetSkip()) ? 1f : instance.GetAnimatorSkipValue());
		switch (newState)
		{
		case State.none:
			base.gameObject.SetActive(value: false);
			if (m_fpClose != null)
			{
				m_fpClose(this, null);
			}
			break;
		case State.appearAll:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.appear.ToString(), m_prevAniSpeed);
			if (m_activePopup != null)
			{
				m_checkAnimator = m_activePopup.m_RootObject.GetComponentInChildren<Animator>();
			}
			break;
		case State.appearContent:
			if (m_activePopup != null)
			{
				m_checkAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_activePopup.m_RootObject, GameDefine.UIAnimationState.appear.ToString(), m_prevAniSpeed);
			}
			break;
		case State.idle:
		{
			m_curDelayTime = 0f;
			string text = string.Empty;
			switch (m_curPopupType)
			{
			case PopupType.Normal:
				text = "tuto_00004";
				break;
			case PopupType.Event:
				text = string.Empty;
				break;
			case PopupType.Inference:
				text = "tuto_00005";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				m_isTutotialActived = TutorialPopup.isShowAble(text);
				if (m_isTutotialActived)
				{
					StartCoroutine(TutorialPopup.Show(text, OnClosed_TutorialPopup, base.gameObject.GetComponentInChildren<Canvas>()));
				}
			}
			break;
		}
		case State.disappearContent:
			if (m_activePopup != null)
			{
				m_checkAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(m_activePopup.m_RootObject, GameDefine.UIAnimationState.disappear.ToString(), m_prevAniSpeed);
			}
			break;
		case State.disappearAll:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString(), m_prevAniSpeed);
			if (m_activePopup != null)
			{
				m_checkAnimator = m_activePopup.m_RootObject.GetComponentInChildren<Animator>();
			}
			break;
		}
		m_curState = newState;
	}

	private void OnClosed_TutorialPopup(object sender, object arg)
	{
		m_isTutotialActived = false;
	}

	private IEnumerator SetCurrentContentIdx(int idx)
	{
		m_isKeyLock = true;
		if (m_keywordIDs != null && idx >= 0 && idx < m_keywordIDs.Length)
		{
			m_curIdx = idx;
			string keywordID = m_keywordIDs[idx];
			Xls.CollKeyword xlsKeywordData = Xls.CollKeyword.GetData_byKey(keywordID);
			if (xlsKeywordData != null)
			{
				PopupType popupType = m_defPopupType;
				if (popupType == PopupType.Auto)
				{
					popupType = xlsKeywordData.m_iCtg switch
					{
						0 => PopupType.Event, 
						1 => PopupType.Normal, 
						2 => PopupType.Inference, 
						_ => PopupType.Normal, 
					};
				}
				m_curPopupType = popupType;
				string popupSoundKey = string.Empty;
				switch (popupType)
				{
				case PopupType.Normal:
					m_activePopup = m_PopupType1;
					popupSoundKey = "Pop_Key";
					break;
				case PopupType.Event:
					m_activePopup = m_PopupType2;
					popupSoundKey = "View_EviKey";
					break;
				case PopupType.Inference:
					m_activePopup = m_PopupType3;
					popupSoundKey = "View_ArrangeKey";
					break;
				default:
					m_activePopup = m_PopupType1;
					break;
				}
				m_PopupType1.m_RootObject.SetActive(m_PopupType1 == m_activePopup);
				m_PopupType2.m_RootObject.SetActive(m_PopupType2 == m_activePopup);
				m_PopupType3.m_RootObject.SetActive(m_PopupType3 == m_activePopup);
				Xls.TextListData textListData = Xls.TextListData.GetData_byKey(xlsKeywordData.m_strTitleID);
				if (textListData != null)
				{
					m_activePopup.m_TitleText.text = textListData.m_strTitle;
					m_activePopup.m_ContentText.text = textListData.m_strText;
				}
				else
				{
					m_activePopup.m_TitleText.text = "No Title";
					m_activePopup.m_ContentText.text = "No Text";
				}
				Sprite keywordIcon = null;
				if (!string.IsNullOrEmpty(xlsKeywordData.m_strIconImgID))
				{
					Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(xlsKeywordData.m_strIconImgID);
					if (data_byKey != null)
					{
						string imagePath = data_byKey.m_strAssetPath + "_s";
						keywordIcon = MainLoadThing.instance.keywordIconImageManager.GetThumbnailImageInCache(imagePath);
					}
				}
				m_activePopup.m_IconImage.sprite = keywordIcon;
				if (!string.IsNullOrEmpty(popupSoundKey) && s_AudioManager != null)
				{
					s_AudioManager.PlayUISound(popupSoundKey);
				}
			}
		}
		m_isKeyLock = false;
		yield return null;
	}

	public static IEnumerator LoadPrefab()
	{
		if (!(s_srcObject != null))
		{
			s_srcObject = MainLoadThing.instance.m_prefabKeywordGetPopup as GameObject;
		}
		yield break;
	}

	public static IEnumerator Create()
	{
		if (!(s_Instance != null))
		{
			yield return MainLoadThing.instance.StartCoroutine(LoadPrefab());
			GameObject popupGameObject = UnityEngine.Object.Instantiate(s_srcObject);
			if (!(popupGameObject == null))
			{
				popupGameObject.SetActive(value: false);
				s_Instance = popupGameObject.GetComponentInChildren<KeywordGetPopupPlus>();
				s_AutoModeDelayTime = GameSwitch.GetInstance().GetAutoDelayTime() * 2f + 1f;
				s_SkipModeDelayTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("POPUP_SKIP_DELAY_TIME");
			}
		}
	}

	public static void Destroy()
	{
		if (s_Instance != null)
		{
			UnityEngine.Object.Destroy(s_Instance.gameObject);
			s_Instance = null;
		}
		s_srcObject = null;
	}

	public static IEnumerator Show(string keywordID, GameDefine.EventProc fpClose = null, PopupType popupType = PopupType.Auto)
	{
		if (!(s_Instance == null) && !string.IsNullOrEmpty(keywordID))
		{
			m_isCreating = true;
			KeywordGetPopupPlus popup = s_Instance;
			if (popup.m_keywordIDs == null || popup.m_keywordIDs.Length != 1)
			{
				popup.m_keywordIDs = new string[1];
			}
			popup.m_keywordIDs[0] = keywordID;
			popup.m_fpClose = ((fpClose == null) ? null : new GameDefine.EventProc(fpClose.Invoke));
			popup.m_defPopupType = popupType;
			if (s_AudioManager == null)
			{
				s_AudioManager = GameGlobalUtil.GetAudioManager();
			}
			yield return GameMain.instance.StartCoroutine(popup.SetCurrentContentIdx(0));
			m_isCreating = false;
			popup.gameObject.SetActive(value: true);
			popup.ChangeState(State.appearAll);
		}
	}

	public static void ShowList(List<string> keywordIDs, GameDefine.EventProc fpClose = null, PopupType popupType = PopupType.Auto)
	{
		if (!(s_Instance == null) && keywordIDs != null && keywordIDs.Count > 0)
		{
			KeywordGetPopupPlus keywordGetPopupPlus = s_Instance;
			keywordGetPopupPlus.m_keywordIDs = keywordIDs.ToArray();
			keywordGetPopupPlus.m_fpClose = ((fpClose == null) ? null : new GameDefine.EventProc(fpClose.Invoke));
			keywordGetPopupPlus.m_defPopupType = popupType;
			if (s_AudioManager == null)
			{
				s_AudioManager = GameGlobalUtil.GetAudioManager();
			}
			keywordGetPopupPlus.gameObject.SetActive(value: true);
			GameMain.instance.StartCoroutine(keywordGetPopupPlus.SetCurrentContentIdx(0));
			keywordGetPopupPlus.ChangeState(State.appearAll);
		}
	}

	public static void Close()
	{
		if (!(s_Instance == null) && s_Instance.gameObject.activeSelf)
		{
			Animator[] componentsInChildren = s_Instance.gameObject.GetComponentsInChildren<Animator>();
			if (componentsInChildren == null || componentsInChildren.Length <= 0)
			{
				s_Instance.ChangeState(State.none);
			}
			else
			{
				s_Instance.ChangeState(State.disappearAll);
			}
		}
	}

	public static bool IsActivated()
	{
		bool flag = true;
		if (m_isCreating)
		{
			return true;
		}
		return s_Instance != null && s_Instance.gameObject.activeSelf;
	}

	public static bool IsInactivated()
	{
		return !IsActivated();
	}

	public void OnEventProc_PointClick()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_curState == State.idle)
		{
			ChangeState((m_curIdx + 1 >= m_keywordIDs.Length) ? State.disappearAll : State.disappearContent);
		}
	}
}
