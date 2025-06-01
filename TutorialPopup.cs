using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
	private class TutorialPage
	{
		public int m_iXlsIdx;

		public string m_strTitle;

		public string m_strContent;

		public string m_strImgPath;

		public Sprite m_sprTutoImg;
	}

	private enum eState
	{
		None = -1,
		AppearDelay,
		Appear,
		Next_ImgToImg,
		Next_ImgToNone,
		Next_NoneToImg,
		Next_NoneToNone,
		Idle,
		Disappear,
		Exit
	}

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_Tutorial_Popup";

	public int m_iSortingOrder;

	public GameObject m_goTop;

	public Text m_textTitle;

	public Text m_textContent;

	public GameObject m_goScrollParent;

	public GameObject m_goSelScrollIcon;

	public GameObject m_goNSelScrollIcon;

	public Button m_butOK;

	public Button m_butDirection;

	public GameObject m_goImgGroup;

	public Image m_imgTuto;

	public Animator m_animImgGroup;

	public Animator m_animPopup;

	public Canvas m_canvas;

	public GameObject m_goObjs;

	public Text m_textTutorial;

	public GameObject m_goDirection;

	public Text m_textDirection;

	public GameObject m_goOK;

	public Text m_textOK;

	public GameObject m_goLArrow;

	public GameObject m_goRArrow;

	public GridLayoutGroup m_gridLayoutGroup;

	[Header("Touch Object")]
	public GameObject m_goTouchLeft;

	public GameObject m_goTouchRight;

	public GameObject m_goTouchClose;

	private GameDefine.eAnimChangeState m_eAnimPopupState;

	private const string ANIM_NEXT_PAGE = "next_page_disappear";

	private const string ANIM_PREV_PAGE = "prev_page_disappear";

	private string m_strNextPageAnim;

	private GameDefine.eAnimChangeState m_eAnimImgGroupState;

	private GameDefine.EventProc m_cbfpClose;

	private string m_strTutorialKey;

	private List<TutorialPage> m_lstTutorialPage;

	private int m_iTotalPage;

	private int m_iCurPage;

	private GameSwitch m_gameSwitch;

	private EventEngine m_eventEngine;

	private eState m_eState = eState.None;

	private GameObject[] m_goIcon;

	private float m_fDelaySetTime;

	private float m_fDelayPassTime;

	private float m_fDelayMakeTime = 0.5f;

	private float m_fCurMakeTime;

	private bool m_isMakeTutoData;

	private static TutorialPopup s_Instance;

	public static bool m_isCreating;

	public float m_InputRepeatTimeBound = 0.2f;

	private float m_PadStickPushingTime;

	public static TutorialPopup instance => s_Instance;

	public static IEnumerator Create()
	{
		if (!(s_Instance != null))
		{
			GameObject goTemp = null;
			goTemp = Object.Instantiate(MainLoadThing.instance.m_prefabTutorialPopup) as GameObject;
			if (!(goTemp == null))
			{
				goTemp.name = "UI_Tutorial_Popup";
				s_Instance = goTemp.transform.GetComponentInChildren<TutorialPopup>();
				yield return null;
			}
		}
	}

	private void OnDisable()
	{
		Object.Destroy(s_Instance.gameObject);
	}

	private void OnDestroy()
	{
		m_cbfpClose = null;
		if (m_lstTutorialPage != null)
		{
			m_lstTutorialPage.Clear();
		}
		m_gameSwitch = null;
		m_eventEngine = null;
		m_goIcon = null;
		s_Instance = null;
	}

	public static IEnumerator Show(string strTutorialKey, GameDefine.EventProc cbfpClose, Canvas canvas)
	{
		yield return GameMain.instance.StartCoroutine(Show(strTutorialKey, cbfpClose, canvas.sortingOrder + 1));
	}

	public static bool isShowAble(string strTutorialKey)
	{
		bool result = false;
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		if (gameSwitch != null && !string.IsNullOrEmpty(strTutorialKey) && gameSwitch.GetTutorialIdx(strTutorialKey) != -1 && gameSwitch.GetTutorial(strTutorialKey) != 1)
		{
			result = true;
		}
		return result;
	}

	public static IEnumerator Show(string strTutorialKey, GameDefine.EventProc cbfpClose = null, int iSortingOrder = 95)
	{
		m_isCreating = true;
		bool isCreateComp = false;
		if (!string.IsNullOrEmpty(strTutorialKey))
		{
			yield return GameMain.instance.StartCoroutine(Create());
			TutorialPopup instance = s_Instance;
			if (instance != null)
			{
				instance.m_textTutorial.text = GameGlobalUtil.GetXlsProgramText("TUTORIAL_BG_TEXT");
				Text[] textComps = new Text[5] { instance.m_textTitle, instance.m_textContent, instance.m_textDirection, instance.m_textOK, instance.m_textTutorial };
				FontManager.ResetTextFontByCurrentLanguage(textComps);
				instance.m_goTop.SetActive(value: true);
				if (instance.m_strTutorialKey == null)
				{
					instance.m_gameSwitch = GameSwitch.GetInstance();
					instance.m_eventEngine = EventEngine.GetInstance();
					if (instance.m_canvas != null)
					{
						instance.SetSortingOrder(iSortingOrder);
					}
					if (instance.m_gameSwitch.GetTutorialIdx(strTutorialKey) != -1)
					{
						instance.m_goObjs.SetActive(value: false);
						if (instance.m_gameSwitch.GetTutorial(strTutorialKey) == 1)
						{
							instance.m_goTop.SetActive(value: false);
						}
						else
						{
							instance.m_gameSwitch.SetTutorial(strTutorialKey, 1);
							instance.m_strTutorialKey = strTutorialKey;
							instance.m_cbfpClose = cbfpClose;
							instance.SetState(eState.AppearDelay);
							instance.m_eventEngine.StopSkip(isShowSkipBut: false);
							isCreateComp = true;
						}
					}
				}
			}
		}
		m_isCreating = false;
		if (!isCreateComp)
		{
			cbfpClose?.Invoke(null, null);
		}
		yield return null;
	}

	private void SetSortingOrder(int iSortingOrder = 95)
	{
		if (m_canvas != null)
		{
			m_canvas.sortingOrder = iSortingOrder;
		}
	}

	private IEnumerator MakeTutoData()
	{
		if (string.IsNullOrEmpty(m_strTutorialKey))
		{
			yield break;
		}
		m_isMakeTutoData = false;
		m_fCurMakeTime = 0f;
		TutorialPage tutoPageTemp = null;
		int iXlsCnt = Xls.TutorialData.GetDataCount();
		Xls.TutorialData xlsTuto = null;
		Xls.ImageFile xlsImgFile = null;
		for (int i = 0; i < iXlsCnt; i++)
		{
			xlsTuto = Xls.TutorialData.GetData_byIdx(i);
			if (!(xlsTuto.m_strTutorialID == m_strTutorialKey))
			{
				continue;
			}
			if (m_lstTutorialPage == null)
			{
				m_lstTutorialPage = new List<TutorialPage>();
			}
			tutoPageTemp = new TutorialPage
			{
				m_iXlsIdx = i
			};
			if (!string.IsNullOrEmpty(xlsTuto.m_strImgKey))
			{
				xlsImgFile = Xls.ImageFile.GetData_byKey(xlsTuto.m_strImgKey);
				if (xlsImgFile != null)
				{
					SystemLanguage sysLang = m_gameSwitch.GetCurSubtitleLanguage();
					string strFolderKey = string.Empty;
					Xls.ProgramDefineStr xlsDefineStr = Xls.ProgramDefineStr.GetData_byKey(sysLang switch
					{
						SystemLanguage.Japanese => "TUTORIAL_IMG_JPN", 
						SystemLanguage.ChineseSimplified => "TUTORIAL_IMG_CN_SIMPLE", 
						SystemLanguage.ChineseTraditional => "TUTORIAL_IMG_CN_TRADITONAL", 
						SystemLanguage.English => "TUTORIAL_IMG_EN", 
						_ => "TUTORIAL_IMG_KOR", 
					});
					if (xlsDefineStr != null && xlsTuto.m_strImgKey != null)
					{
						tutoPageTemp.m_strImgPath = Path.Combine(path2: GameGlobalUtil.GetPathStrFromImageXls(xlsTuto.m_strImgKey), path1: xlsDefineStr.m_strTxt);
						yield return GameMain.instance.StartCoroutine(GameGlobalUtil.GetSprRequestFromImgPath(tutoPageTemp.m_strImgPath));
						tutoPageTemp.m_sprTutoImg = GameGlobalUtil.m_sprLoadFromImgXls;
					}
				}
			}
			Xls.TextListData xlsTextList = Xls.TextListData.GetData_byKey(xlsTuto.m_strTextKey);
			if (xlsTextList != null)
			{
				tutoPageTemp.m_strTitle = xlsTextList.m_strTitle;
				tutoPageTemp.m_strContent = xlsTextList.m_strText;
			}
			m_lstTutorialPage.Add(tutoPageTemp);
		}
		m_iTotalPage = m_lstTutorialPage.Count;
		GameObject goTemp = null;
		if (m_iTotalPage > 1)
		{
			m_goIcon = new GameObject[m_iTotalPage];
			for (int j = 0; j < m_iTotalPage; j++)
			{
				if (j == 0)
				{
					goTemp = Object.Instantiate(m_goSelScrollIcon);
					goTemp.name = "Sel";
				}
				else
				{
					goTemp = Object.Instantiate(m_goNSelScrollIcon);
					goTemp.name = "NSel_" + j;
				}
				goTemp.transform.SetParent(m_goScrollParent.transform, worldPositionStays: false);
				goTemp.SetActive(value: true);
				m_goIcon[j] = goTemp;
				m_goIcon[j].transform.SetSiblingIndex(j + 1);
			}
		}
		m_goLArrow.transform.SetSiblingIndex(0);
		m_goRArrow.transform.SetSiblingIndex(m_iTotalPage + 1);
		bool isMultiPage = m_iTotalPage > 1;
		m_goDirection.SetActive(isMultiPage);
		if (isMultiPage)
		{
			m_textDirection.text = GameGlobalUtil.GetXlsProgramText("TUTORIAL_PAGE_LR");
		}
		m_goOK.SetActive(value: true);
		m_goLArrow.SetActive(isMultiPage);
		m_goRArrow.SetActive(isMultiPage);
		m_goTouchRight.SetActive(isMultiPage);
		m_goTouchLeft.SetActive(isMultiPage);
		if (m_goTouchClose != null)
		{
			m_goTouchClose.SetActive(!isMultiPage);
		}
		m_iCurPage = -1;
		SetNextPrevPage(isNext: true);
		do
		{
			m_fCurMakeTime += Time.deltaTime;
		}
		while (!(m_fCurMakeTime > m_fDelayMakeTime));
		m_isMakeTutoData = true;
		yield return null;
	}

	public void SetNextPageText()
	{
		if (m_lstTutorialPage != null)
		{
			TutorialPage tutorialPage = m_lstTutorialPage[m_iCurPage];
			m_textTitle.text = tutorialPage.m_strTitle;
			m_textContent.text = tutorialPage.m_strContent;
		}
	}

	public void SetStateIdle()
	{
		if (m_eState == eState.Next_NoneToNone)
		{
			SetState(eState.Idle);
		}
	}

	public void BuildTagText()
	{
	}

	private void SetNextPrevPage(bool isNext, bool isOKBut = false)
	{
		if (isOKBut && m_iCurPage == m_iTotalPage - 1)
		{
			SetState(eState.Disappear);
			return;
		}
		int iCurPage = m_iCurPage;
		if (isNext)
		{
			if (m_iCurPage >= m_iTotalPage - 1)
			{
				m_iCurPage = 0;
			}
			else
			{
				m_iCurPage++;
			}
		}
		else if (m_iCurPage == 0)
		{
			m_iCurPage = m_iTotalPage - 1;
		}
		else
		{
			m_iCurPage--;
		}
		m_textOK.text = GameGlobalUtil.GetXlsProgramText((m_iCurPage != m_iTotalPage - 1) ? "TUTORIAL_NEXT" : "TUTORIAL_CLOSE");
		bool flag = m_iCurPage == m_iTotalPage - 1;
		m_goTouchRight.SetActive(!flag);
		m_goTouchLeft.SetActive(m_iCurPage != 0);
		if (m_goTouchClose != null)
		{
			m_goTouchClose.SetActive(flag);
		}
		if (m_iCurPage >= m_lstTutorialPage.Count)
		{
			return;
		}
		TutorialPage tutorialPage = m_lstTutorialPage[m_iCurPage];
		if (tutorialPage.m_sprTutoImg != null)
		{
			m_imgTuto.sprite = tutorialPage.m_sprTutoImg;
		}
		if (m_iTotalPage > 1)
		{
			m_goLArrow.transform.SetSiblingIndex(0);
			m_goRArrow.transform.SetSiblingIndex(m_iTotalPage + 1);
			m_goIcon[0].transform.SetSiblingIndex(1 + m_iCurPage);
			int num = m_iTotalPage - 1;
			int num2 = 1;
			while (num < m_iTotalPage)
			{
				if (num != m_iCurPage)
				{
					m_goIcon[num2++].transform.SetSiblingIndex(1 + num);
				}
				if (num2 >= m_iTotalPage)
				{
					break;
				}
				num--;
			}
		}
		bool flag2 = m_lstTutorialPage[m_iCurPage].m_sprTutoImg != null;
		if (iCurPage != -1)
		{
			m_strNextPageAnim = ((!isNext) ? "prev_page_disappear" : "next_page_disappear");
			if (m_lstTutorialPage[iCurPage].m_sprTutoImg != null)
			{
				SetState((!flag2) ? eState.Next_ImgToNone : eState.Next_ImgToImg);
			}
			else
			{
				SetState((!flag2) ? eState.Next_NoneToNone : eState.Next_NoneToImg);
			}
		}
		else
		{
			m_goImgGroup.SetActive(flag2);
		}
	}

	private void SetState(eState state)
	{
		switch (state)
		{
		case TutorialPopup.eState.AppearDelay:
		{
			m_textTitle.text = string.Empty;
			m_textContent.text = string.Empty;
			m_fDelayPassTime = 0f;
			GameMain.instance.StartCoroutine(MakeTutoData());
			Xls.TutorialListData data_byKey = Xls.TutorialListData.GetData_byKey(m_strTutorialKey);
			if (data_byKey != null)
			{
				m_fDelaySetTime = data_byKey.m_fDelay;
			}
			break;
		}
		case TutorialPopup.eState.Appear:
			m_goObjs.SetActive(value: true);
			goto case TutorialPopup.eState.Disappear;
		case TutorialPopup.eState.Disappear:
		{
			m_eAnimPopupState = GameDefine.eAnimChangeState.none;
			GameDefine.UIAnimationState eState = ((state != TutorialPopup.eState.Appear) ? GameDefine.UIAnimationState.disappear : GameDefine.UIAnimationState.appear);
			GameGlobalUtil.PlayUIAnimation(m_animPopup, eState, ref m_eAnimPopupState);
			GameGlobalUtil.PlayUIAnimation(m_animImgGroup, eState);
			break;
		}
		case TutorialPopup.eState.Next_ImgToImg:
		case TutorialPopup.eState.Next_ImgToNone:
		case TutorialPopup.eState.Next_NoneToImg:
		case TutorialPopup.eState.Next_NoneToNone:
			m_eAnimImgGroupState = GameDefine.eAnimChangeState.none;
			if (m_strNextPageAnim != null)
			{
				GameGlobalUtil.PlayUIAnimation(m_animPopup, m_strNextPageAnim);
			}
			if (state != TutorialPopup.eState.Next_NoneToNone)
			{
				if (state == TutorialPopup.eState.Next_NoneToImg)
				{
					m_goImgGroup.SetActive(value: true);
				}
				GameGlobalUtil.PlayUIAnimation(m_animImgGroup, state.ToString(), ref m_eAnimImgGroupState);
			}
			break;
		case TutorialPopup.eState.Exit:
			SetSortingOrder(m_iSortingOrder);
			m_imgTuto.sprite = null;
			m_goImgGroup.SetActive(value: false);
			foreach (TutorialPage item in m_lstTutorialPage)
			{
				if (item.m_sprTutoImg != null)
				{
					item.m_sprTutoImg = null;
				}
				item.m_strTitle = null;
				item.m_strContent = null;
			}
			m_lstTutorialPage.Clear();
			m_lstTutorialPage = null;
			if (m_iTotalPage > 1)
			{
				for (int i = 0; i < m_iTotalPage; i++)
				{
					Object.Destroy(m_goIcon[i]);
				}
			}
			m_goTop.SetActive(value: false);
			m_strTutorialKey = null;
			if (m_eventEngine.IsEventRunning())
			{
				m_eventEngine.SetShowSkipBut(isShowSkipBut: true);
			}
			m_eventEngine = null;
			m_gameSwitch = null;
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
		switch (m_eState)
		{
		case eState.AppearDelay:
			if (m_isMakeTutoData)
			{
				m_fDelayPassTime += Time.deltaTime;
				if (m_fDelayPassTime >= m_fDelaySetTime)
				{
					SetState(eState.Appear);
				}
			}
			break;
		case eState.Appear:
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animPopup, GameDefine.UIAnimationState.appear, ref m_eAnimPopupState))
			{
				SetState(eState.Idle);
			}
			break;
		case eState.Next_ImgToImg:
		case eState.Next_ImgToNone:
		case eState.Next_NoneToImg:
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animImgGroup, m_eState.ToString(), ref m_eAnimImgGroupState))
			{
				if (m_eState == eState.Next_ImgToNone)
				{
					m_goImgGroup.SetActive(value: false);
				}
				SetState(eState.Idle);
			}
			break;
		case eState.Disappear:
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animPopup, GameDefine.UIAnimationState.disappear, ref m_eAnimPopupState))
			{
				SetState(eState.Exit);
			}
			break;
		case eState.Idle:
			if (PopupDialoguePlus.IsAnyPopupActivated() || m_eState != eState.Idle)
			{
				break;
			}
			if (ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_butOK))
			{
				AudioManager.instance.PlayUISound("Push_PopOK");
				SetNextPrevPage(isNext: true, isOKBut: true);
			}
			if (m_iTotalPage > 1)
			{
				GamePadInput.StickDir stickDir = GetMoveDir();
				if (stickDir == GamePadInput.StickDir.None)
				{
					stickDir = GameGlobalUtil.GetMouseWheelAxis();
				}
				if ((stickDir == GamePadInput.StickDir.Left || stickDir == GamePadInput.StickDir.Right) && (m_iCurPage != 0 || stickDir != GamePadInput.StickDir.Left) && (m_iCurPage != m_iTotalPage - 1 || stickDir != GamePadInput.StickDir.Right))
				{
					AudioManager.instance.PlayUISound("Push_PopOK");
					ButtonPadInput.AddAndShowPressAnim(m_butDirection);
					SetNextPrevPage(stickDir == GamePadInput.StickDir.Right);
				}
			}
			break;
		case eState.Next_NoneToNone:
			break;
		}
	}

	private bool IsOverScrollButtonPushingTime()
	{
		m_PadStickPushingTime += Time.deltaTime;
		if (m_PadStickPushingTime >= m_InputRepeatTimeBound)
		{
			m_PadStickPushingTime = 0f;
			return true;
		}
		return false;
	}

	private GamePadInput.StickDir GetMoveDir()
	{
		GamePadInput.StickDir result = GamePadInput.StickDir.None;
		float fAxisX = 0f;
		float fAxisY = 0f;
		if (GamePadInput.GetLStickMove(out fAxisX, out fAxisY))
		{
			if (Mathf.Abs(fAxisX) >= Mathf.Abs(fAxisY))
			{
				if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
				{
					result = GamePadInput.StickDir.Left;
					m_PadStickPushingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
				{
					result = GamePadInput.StickDir.Right;
					m_PadStickPushingTime = 0f;
				}
				else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Pushing))
				{
					if (IsOverScrollButtonPushingTime())
					{
						result = GamePadInput.StickDir.Left;
					}
				}
				else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
				{
					result = GamePadInput.StickDir.Right;
				}
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				result = GamePadInput.StickDir.Up;
				m_PadStickPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				result = GamePadInput.StickDir.Down;
				m_PadStickPushingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				if (IsOverScrollButtonPushingTime())
				{
					result = GamePadInput.StickDir.Up;
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing) && IsOverScrollButtonPushingTime())
			{
				result = GamePadInput.StickDir.Down;
			}
		}
		return result;
	}

	public static bool IsTutorialPopupEnd()
	{
		bool flag = false;
		if (m_isCreating)
		{
			return false;
		}
		return s_Instance == null || (s_Instance != null && (s_Instance.m_eState == eState.None || s_Instance.m_eState == eState.Exit));
	}

	public void PressClose()
	{
		if (MainLoadThing.instance.IsTouchableState() && m_eState == eState.Idle)
		{
			AudioManager.instance.PlayUISound("Push_PopOK");
			SetNextPrevPage(isNext: true, isOKBut: true);
		}
	}

	public void PressLeftRightButton(bool isLeft)
	{
		if (MainLoadThing.instance.IsTouchableState() && m_eState == eState.Idle)
		{
			AudioManager.instance.PlayUISound("Push_PopOK");
			SetNextPrevPage(!isLeft);
		}
	}
}
