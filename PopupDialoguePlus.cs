using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDialoguePlus : MonoBehaviour
{
	public enum PopupType
	{
		Unknown,
		OneButtonType_OK,
		TwoButtonType_YesNo,
		OneButtonActionType_OK,
		TwoButtonActionType_YesNo
	}

	public enum Result
	{
		Unknown,
		OK,
		Yes,
		No,
		Close
	}

	public enum ButtonType
	{
		Button1,
		Button2,
		Close
	}

	[Serializable]
	public class ButtonMembers
	{
		public Result m_Result;

		public PadInput.GameInput[] m_GameInputs;

		public Button m_ButtonObj;

		public Button m_PadIconButton;

		public Text m_Text;
	}

	public delegate void CallBack_PopupResult(Result result);

	private class PopupData
	{
		public PopupType m_Type;

		public string m_Text = string.Empty;

		public Sprite m_ExtraImage;

		public CallBack_PopupResult m_fpResultCB;
	}

	public Animator m_BaseAnimator;

	public GameObject m_ExtraIconBG;

	public GameObject m_KeywordIconRoot;

	public Image m_KeywordIconImage;

	public Text m_Text;

	public ButtonMembers[] m_Buttons;

	private Result m_DlgResult;

	private bool m_isDisappearing;

	private bool m_isInputedByGamePad;

	private static Queue<PopupData> s_quePopupDatas = new Queue<PopupData>();

	private static bool s_isExistActivePopup = false;

	private static bool s_isLoadingAssetBundle = false;

	private static GameObject s_PopupRootObject = null;

	private static Transform s_trPopupRootObject = null;

	private static GameObject s_SrcObject_1ButtonType = null;

	private static GameObject s_SrcObject_2ButtonType = null;

	private static GameObject s_SrcObject_1ButtonActionType = null;

	private static GameObject s_SrcObject_2ButtonActionType = null;

	public const string c_strPrefabName_1ButtonType = "Prefabs/InGame/Menu/UI_Popup_1Button";

	public const string c_strPrefabName_2ButtonType = "Prefabs/InGame/Menu/UI_Popup_2Button";

	public const string c_strPrefabName_1ButtonActionType = "Prefabs/InGame/Menu/UI_Popup_Action_1Button";

	public const string c_strPrefabName_2ButtonActionType = "Prefabs/InGame/Menu/UI_Popup_Action_2Button";

	private static string s_ButtonText_OK = string.Empty;

	private static string s_ButtonText_Yes = string.Empty;

	private static string s_ButtonText_No = string.Empty;

	private static AudioManager s_AudioManager = null;

	private void Awake()
	{
		PopupData popupData = ((s_quePopupDatas.Count <= 0) ? null : s_quePopupDatas.Peek());
		if (popupData == null)
		{
			return;
		}
		FontManager.ResetTextFontByCurrentLanguage(m_Text);
		if (m_Text != null)
		{
			m_Text.text = popupData.m_Text;
		}
		if (m_Buttons != null)
		{
			ButtonMembers[] buttons = m_Buttons;
			foreach (ButtonMembers buttonMembers in buttons)
			{
				if (buttonMembers != null)
				{
					FontManager.ResetTextFontByCurrentLanguage(buttonMembers.m_Text);
				}
			}
			string empty = string.Empty;
			string empty2 = string.Empty;
			switch (popupData.m_Type)
			{
			case PopupType.OneButtonType_OK:
			case PopupType.OneButtonActionType_OK:
				empty = s_ButtonText_OK;
				break;
			case PopupType.TwoButtonType_YesNo:
			case PopupType.TwoButtonActionType_YesNo:
				empty = s_ButtonText_Yes;
				empty2 = s_ButtonText_No;
				break;
			}
			if (m_Buttons.Length > 0 && m_Buttons[0].m_Text != null)
			{
				m_Buttons[0].m_Text.text = empty;
			}
			if (m_Buttons.Length > 1 && m_Buttons[1].m_Text != null)
			{
				m_Buttons[1].m_Text.text = empty2;
			}
		}
		if (popupData.m_ExtraImage != null)
		{
			if (m_ExtraIconBG != null)
			{
				m_ExtraIconBG.SetActive(value: true);
			}
			if (m_KeywordIconImage != null)
			{
				m_KeywordIconImage.gameObject.SetActive(value: true);
				m_KeywordIconImage.sprite = popupData.m_ExtraImage;
			}
			if (m_KeywordIconRoot != null)
			{
				m_KeywordIconRoot.SetActive(value: true);
			}
		}
		else
		{
			if (m_ExtraIconBG != null)
			{
				m_ExtraIconBG.SetActive(value: false);
			}
			if (m_KeywordIconImage != null)
			{
				m_KeywordIconImage.gameObject.SetActive(value: false);
			}
			if (m_KeywordIconRoot != null)
			{
				m_KeywordIconRoot.SetActive(value: false);
			}
		}
	}

	private void Start()
	{
		m_isDisappearing = false;
		if (s_AudioManager == null)
		{
			s_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (s_AudioManager != null)
		{
			s_AudioManager.PlayUISound("View_PopWindow");
		}
	}

	private void OnDestroy()
	{
		s_trPopupRootObject = null;
		s_SrcObject_1ButtonType = null;
		s_SrcObject_2ButtonType = null;
		s_SrcObject_1ButtonActionType = null;
		s_SrcObject_2ButtonActionType = null;
		s_AudioManager = null;
	}

	private void Update()
	{
		if (m_isDisappearing && m_BaseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_BaseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				ProcPopupResult(this);
			}
		}
		else
		{
			if (m_Buttons == null || ButtonPadInput.IsPlayingButPressAnim())
			{
				return;
			}
			int num = m_Buttons.Length;
			ButtonMembers buttonMembers = null;
			for (int i = 0; i < num; i++)
			{
				buttonMembers = m_Buttons[i];
				if (buttonMembers.m_GameInputs == null || buttonMembers.m_GameInputs.Length <= 0)
				{
					continue;
				}
				bool flag = false;
				PadInput.GameInput gameInput = PadInput.GameInput.None;
				PadInput.GameInput[] gameInputs = buttonMembers.m_GameInputs;
				foreach (PadInput.GameInput gameInput2 in gameInputs)
				{
					if (GamePadInput.IsButtonState_Down(gameInput2))
					{
						flag = true;
						gameInput = gameInput2;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (buttonMembers.m_ButtonObj != null)
				{
					ButtonPadInput.PressInputButton(gameInput, buttonMembers.m_ButtonObj, buttonMembers.m_PadIconButton);
				}
				if (!(s_AudioManager != null))
				{
					break;
				}
				switch (buttonMembers.m_Result)
				{
				case Result.OK:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopOK");
					}
					break;
				case Result.Yes:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopYes");
					}
					break;
				case Result.No:
				case Result.Close:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopNo");
					}
					break;
				}
				m_isInputedByGamePad = true;
				break;
			}
		}
	}

	public void OnClick_PopupButton(int buttonIdx)
	{
		PopupData popupData = s_quePopupDatas.Peek();
		m_DlgResult = ((m_Buttons != null && buttonIdx >= 0 && buttonIdx < m_Buttons.Length) ? (m_DlgResult = m_Buttons[buttonIdx].m_Result) : Result.Unknown);
		if (!m_isInputedByGamePad)
		{
			if (s_AudioManager != null)
			{
				switch (m_DlgResult)
				{
				case Result.OK:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopOK");
					}
					break;
				case Result.Yes:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopYes");
					}
					break;
				case Result.No:
				case Result.Close:
					if (s_AudioManager != null)
					{
						s_AudioManager.PlayUISound("Push_PopNo");
					}
					break;
				}
			}
		}
		else
		{
			m_isInputedByGamePad = false;
		}
		Animator animator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		if (animator != null)
		{
			m_isDisappearing = true;
		}
		else
		{
			ProcPopupResult(this);
		}
	}

	public static void InitPopupDialogues(GameObject rootObject)
	{
		s_PopupRootObject = rootObject;
		s_trPopupRootObject = s_PopupRootObject.GetComponent<Transform>();
	}

	public static void ShowPopup(string text, PopupType popupType, CallBack_PopupResult fpResultCB = null, Sprite extraImage = null)
	{
		if (!(s_PopupRootObject == null))
		{
			s_ButtonText_OK = GameGlobalUtil.GetXlsProgramGlobalText("POPUP_BTN_OK");
			s_ButtonText_Yes = GameGlobalUtil.GetXlsProgramGlobalText("POPUP_BTN_YES");
			s_ButtonText_No = GameGlobalUtil.GetXlsProgramGlobalText("POPUP_BTN_NO");
			PopupData popupData = new PopupData();
			popupData.m_Type = popupType;
			popupData.m_Text = text;
			popupData.m_ExtraImage = extraImage;
			popupData.m_fpResultCB = ((fpResultCB == null) ? null : new CallBack_PopupResult(fpResultCB.Invoke));
			s_quePopupDatas.Enqueue(popupData);
			if (!s_isExistActivePopup && !s_isLoadingAssetBundle)
			{
				MainLoadThing.instance.StartCoroutine(ActivatePopup(popupData));
			}
		}
	}

	public static void ShowPopup_OK(string text, CallBack_PopupResult fpResultCB = null, Sprite extraImage = null)
	{
		ShowPopup(text, PopupType.OneButtonType_OK, fpResultCB, extraImage);
	}

	public static void ShowPopup_YesNo(string text, CallBack_PopupResult fpResultCB = null, Sprite extraImage = null)
	{
		ShowPopup(text, PopupType.TwoButtonType_YesNo, fpResultCB, extraImage);
	}

	public static void ShowActionPopup_OK(string text, CallBack_PopupResult fpResultCB = null, Sprite extraImage = null)
	{
		ShowPopup(text, PopupType.OneButtonActionType_OK, fpResultCB, extraImage);
	}

	public static void ShowActionPopup_YesNo(string text, CallBack_PopupResult fpResultCB = null, Sprite extraImage = null)
	{
		ShowPopup(text, PopupType.TwoButtonActionType_YesNo, fpResultCB, extraImage);
	}

	public static bool ActivateStockPopupData()
	{
		if (s_isExistActivePopup || s_quePopupDatas.Count <= 0)
		{
			return false;
		}
		MainLoadThing.instance.StartCoroutine(ActivatePopup(s_quePopupDatas.Peek()));
		return true;
	}

	public static int GetStockPopupDataCount()
	{
		return s_quePopupDatas.Count;
	}

	public static bool IsAnyPopupActivated()
	{
		return s_isExistActivePopup || s_isLoadingAssetBundle;
	}

	private static IEnumerator ActivatePopup(PopupData data)
	{
		if (s_isExistActivePopup)
		{
			yield break;
		}
		s_isLoadingAssetBundle = true;
		s_isExistActivePopup = true;
		GameObject original = null;
		switch (data.m_Type)
		{
		case PopupType.OneButtonType_OK:
			if (s_SrcObject_1ButtonType == null)
			{
				s_SrcObject_1ButtonType = MainLoadThing.instance.m_prefabPopup_1Button as GameObject;
			}
			original = s_SrcObject_1ButtonType;
			break;
		case PopupType.TwoButtonType_YesNo:
			if (s_SrcObject_2ButtonType == null)
			{
				s_SrcObject_2ButtonType = MainLoadThing.instance.m_prefabPopup_2Button as GameObject;
			}
			original = s_SrcObject_2ButtonType;
			break;
		case PopupType.OneButtonActionType_OK:
			if (s_SrcObject_1ButtonActionType == null)
			{
				s_SrcObject_1ButtonActionType = MainLoadThing.instance.m_prefabPopup_Action_1Button as GameObject;
			}
			original = s_SrcObject_1ButtonActionType;
			break;
		case PopupType.TwoButtonActionType_YesNo:
			if (s_SrcObject_2ButtonActionType == null)
			{
				s_SrcObject_2ButtonActionType = MainLoadThing.instance.m_prefabPopup_Action_2Button as GameObject;
			}
			original = s_SrcObject_2ButtonActionType;
			break;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		Transform component = gameObject.GetComponent<Transform>();
		component.SetParent(s_trPopupRootObject, worldPositionStays: false);
		component.position = Vector3.zero;
		component.rotation = Quaternion.identity;
		gameObject.SetActive(value: true);
		s_isLoadingAssetBundle = false;
	}

	private static void ProcPopupResult(PopupDialoguePlus popupObj)
	{
		popupObj.gameObject.SetActive(value: false);
		s_isExistActivePopup = false;
		PopupData popupData = s_quePopupDatas.Dequeue();
		if (popupData.m_fpResultCB != null)
		{
			popupData.m_fpResultCB(popupObj.m_DlgResult);
		}
		UnityEngine.Object.DestroyImmediate(popupObj.gameObject);
		ActivateStockPopupData();
	}

	public void TouchPopupButton(int buttonIdx)
	{
		OnClick_PopupButton(buttonIdx);
	}
}
