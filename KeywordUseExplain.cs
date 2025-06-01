using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class KeywordUseExplain : MonoBehaviour
{
	private enum eState
	{
		NONE = -1,
		DEF,
		SELECTED,
		FINISH,
		EXPLAIN_CLOSE
	}

	public KeywordMenuPlus m_KeywordMenuPlus;

	public GameObject m_goKeywordUseExplain;

	public GameObject m_goQuestion;

	public GameObject m_goQuestionIcon;

	public Text m_textQuestion;

	public RectTransform m_rtfQuestionIcon;

	private const int KEYWORD_LIST_CNT = 10;

	public GameObject[] m_goAnswer = new GameObject[10];

	public Text[] m_textKeyword = new Text[10];

	public GameObject[] m_goRightAnswer = new GameObject[10];

	public GameObject[] m_goWrongAnswer = new GameObject[10];

	public Image[] m_imgIcon = new Image[10];

	public GameObject[] m_goSelected = new GameObject[10];

	public Animator m_animListExam;

	public Animator m_animQuestion;

	public Animator[] m_animSelect = new Animator[10];

	public Animator[] m_animSelO = new Animator[10];

	public Animator[] m_animSelX = new Animator[10];

	private Animator m_animSelectPlay;

	private Sprite[] m_sprArrIcon_s = new Sprite[10];

	private Sprite[] m_sprArrIcon_ns = new Sprite[10];

	private string[] m_strKeywordKey = new string[10];

	private string m_strQuestID;

	private string[] m_strAnswer;

	private Animator m_animCheckEndPlay;

	private Animator m_animOXMot;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private GameDefine.eAnimChangeState m_eStartMotState;

	private GameDefine.eAnimChangeState m_eSelMotState;

	private GameDefine.eAnimChangeState m_eOXMotState;

	private eState m_eState = eState.NONE;

	private string[] m_strSelKeyword = new string[10];

	private bool[] m_isRightAnswer = new bool[10];

	private int m_iAnswerCnt;

	private EventEngine m_EventEngine;

	private bool m_isSetting;

	private int m_iDisappearObjIdx;

	private void Init()
	{
		m_animCheckEndPlay = null;
		for (int i = 0; i < 10; i++)
		{
			m_goAnswer[i].SetActive(value: false);
			m_goRightAnswer[i].SetActive(value: false);
			m_goWrongAnswer[i].SetActive(value: false);
			m_goSelected[i].SetActive(value: false);
		}
		m_EventEngine = EventEngine.GetInstance();
	}

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textQuestion);
		FontManager.ResetTextFontByCurrentLanguage(m_textKeyword);
	}

	private void OnDisable()
	{
		FreeRes();
	}

	private void FreeRes()
	{
		m_strAnswer = null;
		for (int i = 0; i < 10; i++)
		{
			m_sprArrIcon_s[i] = null;
			m_sprArrIcon_ns[i] = null;
			m_strKeywordKey[i] = null;
		}
		m_strQuestID = null;
		m_animCheckEndPlay = null;
		m_animOXMot = null;
		m_EventEngine = null;
	}

	private IEnumerator SetState(eState eState)
	{
		m_isSetting = true;
		if (eState == eState.SELECTED || m_eState != eState)
		{
			switch (eState)
			{
			case eState.DEF:
			{
				if (m_strQuestID == null)
				{
					break;
				}
				m_strAnswer = m_KeywordMenuPlus.GetKeywordUseAnswer(m_strQuestID);
				if (m_strAnswer == null)
				{
					break;
				}
				Init();
				m_textQuestion.gameObject.SetActive(value: true);
				Xls.KeywordUsing xlsKeywordUsing = Xls.KeywordUsing.GetData_byKey(m_strQuestID);
				if (xlsKeywordUsing != null)
				{
					string xlsTextData = GameGlobalUtil.GetXlsTextData(xlsKeywordUsing.m_strQuestTextID);
					if (xlsTextData != null)
					{
						m_textQuestion.text = xlsTextData;
					}
				}
				m_rtfQuestionIcon.localPosition = new Vector3(0f - (m_textQuestion.preferredWidth / 2f + m_rtfQuestionIcon.rect.width), m_rtfQuestionIcon.localPosition.y, m_rtfQuestionIcon.localPosition.z);
				GameGlobalUtil.PlayUIAnimation(m_animListExam, GameDefine.UIAnimationState.appear);
				BitCalc.InitArray(m_strSelKeyword);
				BitCalc.InitArray(m_isRightAnswer);
				m_iAnswerCnt = 0;
				m_strAnswer = m_KeywordMenuPlus.GetKeywordUseAnswer(m_strQuestID);
				for (int k = 0; k < 10; k++)
				{
					if (m_strAnswer[k] == null)
					{
						continue;
					}
					m_goAnswer[k].SetActive(value: true);
					m_strKeywordKey[k] = m_strAnswer[k];
					Xls.CollKeyword xlsCollKeyword = Xls.CollKeyword.GetData_byKey(m_strAnswer[k]);
					if (xlsCollKeyword != null)
					{
						Xls.TextListData data_byKey = Xls.TextListData.GetData_byKey(xlsCollKeyword.m_strTitleID);
						if (data_byKey != null)
						{
							m_textKeyword[k].text = data_byKey.m_strTitle;
						}
					}
					SetTextColor(k, isSel: false);
					xlsCollKeyword = Xls.CollKeyword.GetData_byKey(m_strAnswer[k]);
					if (xlsCollKeyword != null)
					{
						string strImgID = xlsCollKeyword.m_strIconImgID;
						Xls.ImageFile xlsImageFileData = Xls.ImageFile.GetData_byKey(strImgID);
						if (xlsImageFileData != null)
						{
							string strAssetPath = xlsImageFileData.m_strAssetPath;
							yield return GameGlobalUtil.GetSprRequestFromImgPath(strAssetPath + "_s", isOneFileBundle: false);
							m_sprArrIcon_s[k] = GameGlobalUtil.m_sprLoadFromImgXls;
							yield return GameGlobalUtil.GetSprRequestFromImgPath(strAssetPath, isOneFileBundle: false);
							m_sprArrIcon_ns[k] = GameGlobalUtil.m_sprLoadFromImgXls;
							m_imgIcon[k].sprite = m_sprArrIcon_ns[k];
						}
					}
					m_goAnswer[k].SetActive(value: true);
					m_goRightAnswer[k].SetActive(value: false);
					m_goWrongAnswer[k].SetActive(value: false);
					m_goSelected[k].SetActive(value: false);
					m_iAnswerCnt++;
				}
				m_eStartMotState = GameDefine.eAnimChangeState.none;
				if (m_goQuestion.activeInHierarchy)
				{
					GameGlobalUtil.PlayUIAnimation(m_animQuestion, GameDefine.UIAnimationState.appear, ref m_eStartMotState);
				}
				else
				{
					m_goQuestion.SetActive(value: true);
				}
				goto default;
			}
			case eState.SELECTED:
			{
				m_eSelMotState = GameDefine.eAnimChangeState.none;
				bool[] array = new bool[10];
				for (int l = 0; l < 10; l++)
				{
					array[l] = false;
				}
				for (int m = 0; m < 10 && m_strSelKeyword[m] != null; m++)
				{
					int answerObjIdx2 = GetAnswerObjIdx(m_strSelKeyword[m]);
					if (answerObjIdx2 != -1)
					{
						array[answerObjIdx2] = true;
						if (!m_goRightAnswer[answerObjIdx2].activeInHierarchy && !m_goWrongAnswer[answerObjIdx2].activeInHierarchy && !m_goSelected[answerObjIdx2].activeInHierarchy)
						{
							m_goSelected[answerObjIdx2].SetActive(value: true);
							SetTextColor(answerObjIdx2, isSel: true);
							m_animSelectPlay = m_animSelect[answerObjIdx2];
							GameGlobalUtil.PlayUIAnimation(m_animSelectPlay, GameDefine.UIAnimationState.appear, ref m_eSelMotState);
						}
					}
				}
				for (int n = 0; n < 10; n++)
				{
					m_imgIcon[n].sprite = ((!array[n]) ? m_sprArrIcon_ns[n] : m_sprArrIcon_s[n]);
				}
				goto default;
			}
			case eState.FINISH:
			{
				for (int j = 0; j < 10 && m_strSelKeyword[j] != null; j++)
				{
					int answerObjIdx = GetAnswerObjIdx(m_strSelKeyword[j]);
					if (answerObjIdx != -1)
					{
						if (m_isRightAnswer[j])
						{
							m_goRightAnswer[answerObjIdx].SetActive(value: true);
						}
						else
						{
							m_goWrongAnswer[answerObjIdx].SetActive(value: true);
						}
					}
				}
				goto default;
			}
			case eState.EXPLAIN_CLOSE:
			{
				m_eEndMotState = GameDefine.eAnimChangeState.none;
				Animator[] componentsInChildren = m_goKeywordUseExplain.GetComponentsInChildren<Animator>();
				int num = componentsInChildren.Length;
				for (int i = 0; i < num; i++)
				{
					if (componentsInChildren[i].gameObject.activeInHierarchy)
					{
						if (m_animCheckEndPlay != null)
						{
							m_animCheckEndPlay = componentsInChildren[i];
							GameGlobalUtil.PlayUIAnimation(componentsInChildren[i], GameDefine.UIAnimationState.disappear, ref m_eEndMotState);
						}
						else
						{
							GameGlobalUtil.PlayUIAnimation(componentsInChildren[i], GameDefine.UIAnimationState.disappear);
						}
					}
				}
				goto default;
			}
			default:
				m_eState = eState;
				break;
			}
		}
		m_isSetting = false;
		yield return null;
	}

	private int GetAnswerObjIdx(string strKeyword)
	{
		int result = -1;
		for (int i = 0; i < 10; i++)
		{
			if (m_strKeywordKey[i] == strKeyword)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public void SetQuestID(string strQuestID)
	{
		m_strQuestID = strQuestID;
		m_goKeywordUseExplain.SetActive(value: true);
		StartCoroutine(SetState(eState.DEF));
	}

	public void CloseWindow()
	{
		StartCoroutine(SetState(eState.EXPLAIN_CLOSE));
	}

	public void SetSelAnswer(string strAnswer, bool isRightAnswer)
	{
		int num = -1;
		for (int i = 0; i < 10; i++)
		{
			if (m_strSelKeyword[i] == null)
			{
				if (num == -1)
				{
					num = i;
					break;
				}
			}
			else if (m_strSelKeyword[i] == strAnswer)
			{
				int answerObjIdx = GetAnswerObjIdx(strAnswer);
				num = answerObjIdx;
				break;
			}
		}
		if (num != -1)
		{
			m_isRightAnswer[num] = isRightAnswer;
			m_strSelKeyword[num] = strAnswer;
			StartCoroutine(SetState(eState.SELECTED));
		}
	}

	public void SetOXMotion()
	{
		bool flag = false;
		m_eOXMotState = GameDefine.eAnimChangeState.none;
		for (int i = 0; i < 10; i++)
		{
			if (m_strSelKeyword[i] == null)
			{
				continue;
			}
			int num = (m_iDisappearObjIdx = GetAnswerObjIdx(m_strSelKeyword[i]));
			if (num == -1)
			{
				continue;
			}
			Animator animator = ((!m_isRightAnswer[i]) ? m_animSelX[num] : m_animSelO[num]);
			AudioManager.instance.PlayUISound((!m_isRightAnswer[i]) ? "View_AnswerBad" : "View_AnswerGood");
			animator.gameObject.SetActive(value: true);
			if (num != -1)
			{
				GameGlobalUtil.PlayUIAnimation(m_animSelect[num], GameDefine.UIAnimationState.disappear);
				if (!flag)
				{
					m_animOXMot = animator;
					GameGlobalUtil.PlayUIAnimation(animator, GameDefine.UIAnimationState.appear, ref m_eOXMotState);
					flag = true;
				}
				else
				{
					GameGlobalUtil.PlayUIAnimation(animator, GameDefine.UIAnimationState.appear);
				}
			}
		}
	}

	public bool ProcOXMotCheck()
	{
		bool flag = false;
		GameGlobalUtil.CheckPlayEndUIAnimation(m_animOXMot, GameDefine.UIAnimationState.appear, ref m_eOXMotState);
		flag = m_eOXMotState == GameDefine.eAnimChangeState.play_end;
		if (flag)
		{
			for (int i = 0; i < 10; i++)
			{
				if (m_strSelKeyword[i] != null)
				{
					int answerObjIdx = GetAnswerObjIdx(m_strSelKeyword[i]);
					SetTextColor(answerObjIdx, isSel: false);
					m_strSelKeyword[i] = null;
				}
			}
		}
		return flag;
	}

	private void Update()
	{
		if (m_EventEngine != null)
		{
			float animatorSkipValue = m_EventEngine.GetAnimatorSkipValue();
			for (int i = 0; i < 10; i++)
			{
				if (m_animSelect != null && m_animSelect[i] != null)
				{
					m_animSelect[i].speed = animatorSkipValue;
				}
			}
			if (m_animOXMot != null)
			{
				m_animOXMot.speed = animatorSkipValue;
			}
		}
		if (!ProcEndAnim())
		{
		}
	}

	private bool ProcEndAnim()
	{
		if (m_eEndMotState == GameDefine.eAnimChangeState.none)
		{
			return false;
		}
		if (m_eEndMotState == GameDefine.eAnimChangeState.play_end)
		{
			return false;
		}
		if (m_animCheckEndPlay != null)
		{
			if (m_EventEngine != null)
			{
				m_animCheckEndPlay.speed = m_EventEngine.GetAnimatorSkipValue();
			}
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animCheckEndPlay, GameDefine.UIAnimationState.disappear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
			{
				DestroyGame();
			}
		}
		else
		{
			DestroyGame();
		}
		return true;
	}

	public bool ProcCheckEndOrStartAnim()
	{
		bool flag = false;
		if (m_eState == eState.DEF)
		{
			if (m_EventEngine != null)
			{
				m_animQuestion.speed = m_EventEngine.GetAnimatorSkipValue();
			}
			GameGlobalUtil.CheckPlayEndUIAnimation(m_animQuestion, GameDefine.UIAnimationState.appear, ref m_eStartMotState);
			return !m_isSetting && m_eStartMotState == GameDefine.eAnimChangeState.play_end;
		}
		return !m_isSetting && !m_KeywordMenuPlus.gameObject.activeInHierarchy;
	}

	public bool ProcCheckSelAnim()
	{
		bool flag = false;
		if (m_EventEngine != null)
		{
			m_animSelectPlay.speed = m_EventEngine.GetAnimatorSkipValue();
		}
		GameGlobalUtil.CheckPlayEndUIAnimation(m_animSelectPlay, GameDefine.UIAnimationState.appear, ref m_eSelMotState);
		return m_eSelMotState == GameDefine.eAnimChangeState.play_end;
	}

	private void DestroyGame()
	{
		FreeRes();
		m_goKeywordUseExplain.SetActive(value: false);
	}

	public void SetTextColor(int iIdx, bool isSel)
	{
		if (BitCalc.CheckArrayIdx(iIdx, 10))
		{
			m_textKeyword[iIdx].color = GameGlobalUtil.HexToColor((!isSel) ? 5395273 : 16777215);
		}
	}
}
