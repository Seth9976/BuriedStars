using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class AutoWrite : MonoBehaviour
{
	private enum eBottomGuide
	{
		Submit,
		Exit
	}

	[Header("Root")]
	public GameObject m_goAutoWrite;

	[Header("GameMain")]
	public GameMain m_GameMain;

	[Header("Child Obj")]
	public GameObject m_goBGMultiply;

	public GameObject m_goRightContents;

	public GameObject m_goButtonAdjust;

	public GameObject m_goBGOverColor;

	public GameObject m_goMenuTitle;

	[Header("Text")]
	public Text m_textTitle;

	public Text m_textUserName;

	public Text m_textSNSMyText;

	public Text m_textContent;

	public Text m_textUploadTime;

	public Text m_textTimeCount;

	public Text m_textButtonChange;

	[Header("Image")]
	public Image m_imgUserIcon;

	[Header("For SWMenu")]
	public CommonCloseButton m_closeButton;

	[Header("For Active")]
	public GameObject m_goCircleButton;

	public GameObject m_goAdjustSel2;

	public GameObject m_goAdjustSel1;

	[Header("SWSub Menu")]
	public SWSub_AutoWrite m_swSubAutoWrite;

	[Header("Canvas")]
	public Canvas m_canvas;

	private Animator m_animCheckEndMot;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private GameDefine.eAnimChangeState m_eStartMotState;

	private bool m_isTutoKeyLock;

	private const string m_strAutoWriteTutoID = "";

	private static AutoWrite s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/game/ui_autowrite";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	private bool m_isQuitApplication;

	private bool m_isEventPlay;

	public static AutoWrite instance => s_activedInstance;

	public static bool IsActivated => s_activedInstance != null && s_activedInstance.gameObject.activeSelf;

	private void OnEnable()
	{
		Init();
	}

	private void OnApplicationQuit()
	{
		m_isQuitApplication = true;
	}

	private void OnDisable()
	{
		m_goBGMultiply.SetActive(value: false);
		m_goRightContents.SetActive(value: false);
		m_goButtonAdjust.SetActive(value: false);
		m_goBGOverColor.SetActive(value: false);
		m_goMenuTitle.SetActive(value: false);
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		m_imgUserIcon.sprite = null;
		if (!m_isQuitApplication)
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
			if (m_closeButton != null && m_closeButton.onClosed != null)
			{
				m_closeButton.onClosed(null, null);
			}
		}
		m_animCheckEndMot = null;
		Object.Destroy(base.gameObject);
		s_activedInstance = null;
		m_GameMain = null;
	}

	private void Init()
	{
		m_GameMain = GameMain.instance;
		m_goBGMultiply.SetActive(value: true);
		m_goRightContents.SetActive(value: true);
		m_goButtonAdjust.SetActive(value: true);
		m_goBGOverColor.SetActive(value: true);
		m_goMenuTitle.SetActive(value: true);
		Text[] textComps = new Text[7] { m_textTitle, m_textUserName, m_textSNSMyText, m_textContent, m_textUploadTime, m_textTimeCount, m_textButtonChange };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		m_textTitle.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_TITLE");
		m_textSNSMyText.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_SNS_AUTO_EXPLAIN");
		m_textUploadTime.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_UPLOAD_TIME");
		m_textTimeCount.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_TIME_COUNT");
		m_textButtonChange.text = GameGlobalUtil.GetXlsProgramText("AUTO_WRITE_SUBMIT_BUTTON");
		string key = "acc_00000";
		Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(key);
		if (data_byKey != null)
		{
			Xls.TextListData data_byKey2 = Xls.TextListData.GetData_byKey(data_byKey.m_nicknameID);
			if (data_byKey2 != null)
			{
				m_textUserName.text = data_byKey2.m_strTitle;
			}
			Sprite spriteFromImageXls = GameGlobalUtil.GetSpriteFromImageXls(data_byKey.m_snspicID);
			if (spriteFromImageXls != null)
			{
				m_imgUserIcon.sprite = spriteFromImageXls;
			}
		}
		SetActiveSubmitButton(isOn: true);
		m_eStartMotState = GameDefine.eAnimChangeState.none;
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		PlayStartMotion();
	}

	private void SetActiveSubmitButton(bool isOn)
	{
		SetSelAutoText();
		m_isEventPlay = !isOn;
		m_goCircleButton.SetActive(isOn);
		m_goAdjustSel1.SetActive(isOn);
		m_goAdjustSel2.SetActive(isOn);
		if (isOn)
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
			m_GameMain.m_CommonButtonGuide.AddContent(GetBottomGuideStr(eBottomGuide.Submit), PadInput.GameInput.CircleButton);
			m_GameMain.m_CommonButtonGuide.AddContent(GetBottomGuideStr(eBottomGuide.Exit), PadInput.GameInput.CrossButton);
			m_GameMain.m_CommonButtonGuide.BuildContents(CommonButtonGuide.AlignType.Left);
			m_GameMain.m_CommonButtonGuide.SetShow(isShow: true);
			m_GameMain.m_CommonButtonGuide.SetContentEnable(GetBottomGuideStr(eBottomGuide.Submit), isEnable: true);
			m_GameMain.m_CommonButtonGuide.SetContentEnable(GetBottomGuideStr(eBottomGuide.Exit), isEnable: true);
		}
		else
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
		}
	}

	private string GetBottomGuideStr(eBottomGuide eGuide)
	{
		string text = null;
		string result = null;
		switch (eGuide)
		{
		case eBottomGuide.Submit:
			text = "AUTO_WRITE_BOT_MENU_SUBMIT";
			break;
		case eBottomGuide.Exit:
			text = "AUTO_WRITE_BOT_MENU_EXIT";
			break;
		}
		if (text != null)
		{
			result = GameGlobalUtil.GetXlsProgramText(text);
		}
		return result;
	}

	private void SetSelAutoText()
	{
		int autoSNSText = GameSwitch.GetInstance().GetAutoSNSText();
		Xls.WatchFaterAutoText data_bySwitchIdx = Xls.WatchFaterAutoText.GetData_bySwitchIdx(autoSNSText);
		if (data_bySwitchIdx != null)
		{
			Xls.TextData data_byKey = Xls.TextData.GetData_byKey(data_bySwitchIdx.m_strTextID);
			if (data_byKey != null)
			{
				m_textContent.text = data_byKey.m_strTxt;
			}
		}
		if (m_swSubAutoWrite != null)
		{
			m_swSubAutoWrite.SetAutoText();
		}
	}

	private void Update()
	{
		ProcEndMot();
		if (m_eEndMotState == GameDefine.eAnimChangeState.none && m_goBGMultiply.activeInHierarchy)
		{
			ProcInputButton();
			if (EventEngine.GetInstance().GetCurFSM() == null && !m_goAdjustSel1.activeInHierarchy)
			{
				SetActiveSubmitButton(isOn: true);
			}
		}
	}

	private void ProcEndMot()
	{
		if (m_animCheckEndMot == null)
		{
			return;
		}
		if (m_eStartMotState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_animCheckEndMot, GameDefine.UIAnimationState.appear, ref m_eStartMotState) && GameSwitch.GetInstance().GetTutorial(string.Empty) == 0)
		{
			m_isTutoKeyLock = TutorialPopup.isShowAble(string.Empty);
			if (m_isTutoKeyLock)
			{
				StartCoroutine(TutorialPopup.Show(string.Empty, cbFcTutorialExit, m_canvas));
			}
			m_eStartMotState = GameDefine.eAnimChangeState.none;
		}
		if (m_eEndMotState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_animCheckEndMot, GameDefine.UIAnimationState.disappear, ref m_eEndMotState))
		{
			m_goAutoWrite.SetActive(value: false);
		}
	}

	private void cbFcTutorialExit(object sender, object arg)
	{
		m_isTutoKeyLock = false;
	}

	private void ProcInputButton()
	{
		if (m_goCircleButton.activeInHierarchy && !m_isTutoKeyLock)
		{
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				AudioManager.instance.PlayUISound("Watch_Out");
				m_GameMain.m_CommonButtonGuide.SetContentActivate(GetBottomGuideStr(eBottomGuide.Exit), isActivate: true);
				PlayEndMotion();
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				OnClickChangeButton();
			}
		}
	}

	public void OnClickChangeButton()
	{
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("RUN_AUTOWRITE");
		if (xlsProgramDefineStr != null)
		{
			AudioManager.instance.PlayUISound("Push_WatchOK");
			EventEngine.GetInstance().EnableAndRunObj(xlsProgramDefineStr);
		}
		SetActiveSubmitButton(isOn: false);
	}

	public static IEnumerator ShowAutoWrite_FormAssetBundle(GameDefine.EventProc fpClosed = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (Object.Instantiate(MainLoadThing.instance.m_prefabAutoWrite) as GameObject).GetComponent<AutoWrite>();
			if (GameMain.instance != null)
			{
				s_activedInstance.m_swSubAutoWrite = GameMain.instance.m_SmartWatchRoot.m_AutoWriteModeRoot.GetComponent<SWSub_AutoWrite>();
			}
			yield return null;
		}
		s_activedInstance.ShowAutoWriteMenu(fpClosed);
	}

	public void ShowAutoWriteMenu(GameDefine.EventProc fpClosed = null)
	{
		m_goAutoWrite.SetActive(value: true);
		m_closeButton.onClickedCloseButton = null;
		m_closeButton.onClosed = fpClosed;
	}

	private void PlayStartMotion()
	{
		Animator[] componentsInChildren = m_goAutoWrite.GetComponentsInChildren<Animator>();
		int num = componentsInChildren.Length;
		if (num > 0)
		{
			m_animCheckEndMot = componentsInChildren[0];
			GameGlobalUtil.PlayAllChildrenUIAnimation(m_goAutoWrite, GameDefine.UIAnimationState.appear, m_animCheckEndMot, ref m_eStartMotState);
		}
	}

	private void PlayEndMotion()
	{
		Animator[] componentsInChildren = m_goAutoWrite.GetComponentsInChildren<Animator>();
		int num = componentsInChildren.Length;
		if (num > 0)
		{
			m_animCheckEndMot = componentsInChildren[0];
			GameGlobalUtil.PlayAllChildrenUIAnimation(m_goAutoWrite, GameDefine.UIAnimationState.disappear, m_animCheckEndMot, ref m_eEndMotState);
		}
	}

	public static bool IsAutoWriteOn()
	{
		bool result = false;
		if (instance != null && instance.m_goAutoWrite != null && instance.m_goAutoWrite.activeInHierarchy)
		{
			result = true;
		}
		return result;
	}

	public void OnClickBackButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Watch_Out");
			PlayEndMotion();
		}
	}

	public void TouchChangeButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClickChangeButton();
		}
	}
}
