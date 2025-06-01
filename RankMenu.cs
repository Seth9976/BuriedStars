using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class RankMenu : MonoBehaviour
{
	private enum eBottomGuide
	{
		Exit
	}

	public GameObject m_goRankWindow;

	public Text m_txtTitle;

	public Text m_txtRankTitle;

	private const int COUNT_RANK = 5;

	public GameObject m_goBackButton;

	public GameObject[] m_goRank = new GameObject[5];

	public Text[] m_arrTxtRankName = new Text[5];

	public Text[] m_arrTxtRankScore = new Text[5];

	public Text[] m_arrTxtRankNum = new Text[5];

	public Image[] m_arrImgRankFace = new Image[5];

	public Text[] m_arrTxtDead = new Text[5];

	public Animator m_animTitle;

	public Animator m_animSubTitle;

	public Animator m_animBackButton;

	public Animator[] m_animRank = new Animator[5];

	public CommonCloseButton m_closeButton;

	public Canvas m_canvas;

	private GameSwitch m_gameSwitch;

	private GameMain m_GameMain;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private GameDefine.eAnimChangeState m_eStartMotState;

	private bool m_isTutoKeyLock;

	private const string m_strRankTutorialID = "";

	private GameObject m_parentObject;

	private static RankMenu s_activedInstance;

	private const string c_assetBundleName = "prefabs/ingame/menu/ui_rank";

	private static AssetBundleObjectHandler s_assetBundleHdr;

	public static RankMenu instance => s_activedInstance;

	public static bool IsActivated => s_activedInstance != null && s_activedInstance.gameObject.activeSelf;

	private void OnEnable()
	{
		Text[] textComps = new Text[2] { m_txtTitle, m_txtRankTitle };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		FontManager.ResetTextFontByCurrentLanguage(m_arrTxtRankName);
		FontManager.ResetTextFontByCurrentLanguage(m_arrTxtRankScore);
		FontManager.ResetTextFontByCurrentLanguage(m_arrTxtRankNum);
		FontManager.ResetTextFontByCurrentLanguage(m_arrTxtDead);
		m_parentObject = base.gameObject.GetComponent<RectTransform>().parent.gameObject;
		if (m_gameSwitch == null)
		{
			m_gameSwitch = GameSwitch.GetInstance();
		}
		if (m_GameMain == null)
		{
			m_GameMain = GameMain.instance;
		}
		StartCoroutine(EventCameraEffect.Instance.Activate_KeywordMenuBG());
		InitAllObject();
		m_eStartMotState = GameDefine.eAnimChangeState.none;
		GameGlobalUtil.PlayUIAnimation(m_animTitle, GameDefine.UIAnimationState.appear, ref m_eStartMotState);
		GameGlobalUtil.PlayUIAnimation(m_animBackButton, GameDefine.UIAnimationState.appear);
		for (int i = 0; i < 5; i++)
		{
			GameGlobalUtil.PlayUIAnimation(m_animRank[i], GameDefine.UIAnimationState.appear);
		}
		m_txtTitle.text = GameGlobalUtil.GetXlsProgramText("SMART_WATCH_BUTTON_RANK");
		m_txtRankTitle.text = GameGlobalUtil.GetXlsProgramText("RANK_SUB_TITLE");
		StartCoroutine(SetChar());
		if (m_GameMain != null)
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
			m_GameMain.m_CommonButtonGuide.AddContent(GetStrBottomGuide(eBottomGuide.Exit), PadInput.GameInput.CrossButton);
			m_GameMain.m_CommonButtonGuide.BuildContents(CommonButtonGuide.AlignType.Center);
			m_GameMain.m_CommonButtonGuide.SetShow(isShow: true);
			m_GameMain.m_CommonButtonGuide.SetContentEnable(GetStrBottomGuide(eBottomGuide.Exit), isEnable: true);
		}
		m_goBackButton.SetActive(value: true);
	}

	private void OnDisable()
	{
		EventCameraEffect.Instance.Deactivate_KeywordMenuBG();
		if (m_closeButton != null)
		{
			m_closeButton.onClosed(null, null);
		}
		m_eEndMotState = GameDefine.eAnimChangeState.none;
		for (int i = 0; i < 5; i++)
		{
			m_arrImgRankFace[i].sprite = null;
		}
		if (m_GameMain != null && m_GameMain.m_CommonButtonGuide != null)
		{
			m_GameMain.m_CommonButtonGuide.ClearContents();
		}
		s_activedInstance = null;
		m_gameSwitch = null;
		m_GameMain = null;
	}

	private void InitAllObject()
	{
		for (int i = 0; i < 5; i++)
		{
			m_arrTxtRankName[i].text = string.Empty;
			m_arrTxtRankScore[i].text = string.Empty;
			m_arrTxtRankNum[i].text = string.Empty;
			m_arrImgRankFace[i].gameObject.SetActive(value: false);
			m_arrTxtDead[i].gameObject.SetActive(value: false);
		}
	}

	private IEnumerator SetChar()
	{
		string strDead = GameGlobalUtil.GetXlsProgramText("RANKMENU_DEAD");
		Xls.CharData xlsCharData = null;
		for (int i = 0; i < 5; i++)
		{
			GameSwitch.VoteRank voteRank = m_gameSwitch.GetVoteRank(i);
			if (voteRank == null)
			{
				continue;
			}
			xlsCharData = Xls.CharData.GetData_bySwitchIdx(voteRank.m_iCharIdx);
			if (xlsCharData != null)
			{
				Xls.TextData data_byKey = Xls.TextData.GetData_byKey(xlsCharData.m_strNameKey);
				if (data_byKey != null)
				{
					m_arrTxtRankName[i].text = data_byKey.m_strTxt;
				}
			}
			m_arrTxtRankScore[i].text = $"{voteRank.m_iVoteCnt:#,###0}";
			m_arrTxtRankNum[i].text = voteRank.m_iRankNum.ToString();
			if (xlsCharData != null)
			{
				string strCharKey = xlsCharData.m_strKey;
				xlsCharData = Xls.CharData.GetData_byKey(strCharKey);
				if (xlsCharData != null)
				{
					Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(xlsCharData.m_strRankImg);
					if (data_byKey2 != null)
					{
						string strAssetPath = data_byKey2.m_strAssetPath;
						ContentThumbnailManager rankCharImageManager = m_GameMain.RankCharImageManager;
						m_arrImgRankFace[i].sprite = rankCharImageManager.GetThumbnailImageInCache(strAssetPath);
						m_arrImgRankFace[i].gameObject.SetActive(value: true);
					}
				}
			}
			bool isDie = m_gameSwitch.GetCharParty(voteRank.m_iCharIdx) == 2;
			m_arrTxtDead[i].gameObject.SetActive(isDie);
			if (isDie)
			{
				m_arrTxtDead[i].text = strDead;
			}
		}
		yield return null;
	}

	private string GetStrBottomGuide(eBottomGuide eGuide)
	{
		string text = null;
		if (eGuide == eBottomGuide.Exit)
		{
			text = "RANK_BOT_MENU_EXIT";
		}
		if (text != null)
		{
			text = GameGlobalUtil.GetXlsProgramText(text);
		}
		return text;
	}

	public static IEnumerator ShowRankMenu_FormAssetBundle(GameDefine.EventProc fpClosed = null)
	{
		if (s_activedInstance == null)
		{
			s_activedInstance = (Object.Instantiate(MainLoadThing.instance.m_prefabRankMenu) as GameObject).GetComponentInChildren<RankMenu>(includeInactive: true);
			yield return null;
		}
		s_activedInstance.InitRankMenu(fpClosed);
	}

	public void InitRankMenu(GameDefine.EventProc fpClosedCB = null)
	{
		if (m_closeButton != null)
		{
			m_closeButton.onClickedCloseButton = OnTouchBackButton;
			m_closeButton.onClosed = fpClosedCB;
		}
		if (m_goRankWindow != null)
		{
			m_goRankWindow.gameObject.SetActive(value: true);
		}
		if (m_parentObject != null)
		{
			m_parentObject.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (m_eStartMotState != GameDefine.eAnimChangeState.none && GameGlobalUtil.CheckPlayEndUIAnimation(m_animTitle, GameDefine.UIAnimationState.appear, ref m_eStartMotState))
		{
			if (m_gameSwitch.GetTutorial(string.Empty) == 0)
			{
				m_isTutoKeyLock = TutorialPopup.isShowAble(string.Empty);
				if (m_isTutoKeyLock)
				{
					StartCoroutine(TutorialPopup.Show(string.Empty, cbFcTutorialExit, m_canvas));
				}
			}
			m_eStartMotState = GameDefine.eAnimChangeState.none;
		}
		if (m_eEndMotState != GameDefine.eAnimChangeState.none)
		{
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animTitle, GameDefine.UIAnimationState.disappear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
			{
				CloseWindow();
			}
		}
		else if (!m_isTutoKeyLock && GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
		{
			AudioManager.instance.PlayUISound("Watch_Out");
			GameMain.instance.m_CommonButtonGuide.SetContentActivate(GetStrBottomGuide(eBottomGuide.Exit), isActivate: true);
			PlayEndMot();
		}
	}

	private void cbFcTutorialExit(object sender, object arg)
	{
		m_isTutoKeyLock = false;
	}

	private void CloseWindow()
	{
		m_goRankWindow.SetActive(value: false);
		if (m_parentObject != null)
		{
			Object.Destroy(m_parentObject);
			m_parentObject = null;
		}
		s_activedInstance = null;
	}

	private void PlayEndMot()
	{
		if (m_eEndMotState == GameDefine.eAnimChangeState.none)
		{
			GameGlobalUtil.PlayUIAnimation(m_animTitle, GameDefine.UIAnimationState.disappear, ref m_eEndMotState);
			GameGlobalUtil.PlayUIAnimation(m_animSubTitle, GameDefine.UIAnimationState.disappear);
			GameGlobalUtil.PlayUIAnimation(m_animBackButton, GameDefine.UIAnimationState.disappear);
			for (int i = 0; i < 5; i++)
			{
				GameGlobalUtil.PlayUIAnimation(m_animRank[i], GameDefine.UIAnimationState.disappear);
			}
		}
	}

	public void OnTouchBackButton(object sender, object arg)
	{
		PlayEndMot();
	}

	public void OnClickBackButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Watch_Out");
			PlayEndMot();
		}
	}
}
