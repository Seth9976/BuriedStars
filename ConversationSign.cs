using System.Collections;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class ConversationSign : MonoBehaviour
{
	public enum eDialogSignType
	{
		None = -1,
		Off,
		Keyword,
		Profile
	}

	[Header("Dialog Sign Type")]
	public GameObject m_goSignRoot;

	public Text m_textSignType;

	public Text m_textSignName;

	public Image m_imgSignIcon;

	public Animator m_animSign;

	private GameDefine.UIAnimationState m_eCurPlayAnimState = GameDefine.UIAnimationState.idle;

	private eDialogSignType m_eDialogSignType;

	private eDialogSignType m_eNextSignType = eDialogSignType.None;

	private bool m_isNextSignSetting;

	private bool m_isMotEnd;

	private bool m_isNextImgSetting;

	private GameDefine.eAnimChangeState m_eEndMotState;

	private EventEngine m_EventEngine;

	[HideInInspector]
	public string m_strSignKey;

	private void OnEnable()
	{
		Text[] textComps = new Text[2] { m_textSignType, m_textSignName };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void OnDestroy()
	{
		m_EventEngine = null;
	}

	public bool InitDialgSignType()
	{
		if (!m_goSignRoot.activeInHierarchy)
		{
			return false;
		}
		return SetDialogSignType(eDialogSignType.Off, null, isSetAnim: false);
	}

	public bool SetDialogSignType(eDialogSignType eSignType, string strKey = null, bool isSetAnim = true)
	{
		bool result = false;
		if (m_EventEngine == null)
		{
			m_EventEngine = EventEngine.GetInstance();
		}
		if (isSetAnim)
		{
			if (m_eDialogSignType == eSignType)
			{
				return false;
			}
			if (!m_goSignRoot.activeInHierarchy)
			{
				m_goSignRoot.SetActive(value: true);
			}
			m_isNextImgSetting = false;
			result = true;
			m_isNextSignSetting = eSignType == eDialogSignType.Keyword || eSignType == eDialogSignType.Profile;
			FreeSignUI();
			if ((m_eDialogSignType == eDialogSignType.None || m_eDialogSignType == eDialogSignType.Off) && m_isNextSignSetting)
			{
				m_isMotEnd = false;
				AudioManager.instance.PlayUISound("View_Talk_Title");
				GameGlobalUtil.PlayUIAnimation(m_animSign, GameDefine.UIAnimationState.appear, ref m_eEndMotState);
				m_eCurPlayAnimState = GameDefine.UIAnimationState.appear;
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation(m_animSign, GameDefine.UIAnimationState.disappear, ref m_eEndMotState);
				m_eCurPlayAnimState = GameDefine.UIAnimationState.disappear;
			}
			m_eNextSignType = eSignType;
		}
		else
		{
			m_eDialogSignType = eSignType;
			m_isNextSignSetting = false;
			if (m_eDialogSignType == eDialogSignType.Off || m_eDialogSignType == eDialogSignType.None)
			{
				m_goSignRoot.SetActive(value: false);
			}
		}
		if (eSignType != eDialogSignType.Off)
		{
			m_strSignKey = strKey;
		}
		return result;
	}

	private void Update()
	{
		if (m_EventEngine != null && m_animSign != null && m_animSign.gameObject.activeInHierarchy)
		{
			m_animSign.speed = m_EventEngine.GetAnimatorSkipValue();
		}
	}

	public bool ProcCoversationSign()
	{
		bool result = false;
		if (m_eCurPlayAnimState == GameDefine.UIAnimationState.disappear)
		{
			if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animSign, GameDefine.UIAnimationState.disappear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
			{
				if (m_isNextSignSetting)
				{
					GameGlobalUtil.PlayUIAnimation(m_animSign, GameDefine.UIAnimationState.appear, ref m_eEndMotState);
					m_eCurPlayAnimState = GameDefine.UIAnimationState.appear;
				}
				else
				{
					m_goSignRoot.SetActive(value: false);
					m_eDialogSignType = eDialogSignType.Off;
					result = true;
				}
			}
		}
		else if (m_eCurPlayAnimState == GameDefine.UIAnimationState.appear)
		{
			if (m_isMotEnd)
			{
				if (!m_isNextImgSetting)
				{
					result = true;
				}
			}
			else if (GameGlobalUtil.CheckPlayEndUIAnimation(m_animSign, GameDefine.UIAnimationState.appear, ref m_eEndMotState) && m_eEndMotState == GameDefine.eAnimChangeState.play_end)
			{
				m_isMotEnd = true;
				m_isNextImgSetting = true;
				StartCoroutine(SetSignUI());
				m_eDialogSignType = m_eNextSignType;
			}
		}
		return result;
	}

	public IEnumerator SetSignUI()
	{
		bool isKeyword;
		string strImgPath;
		string strTextListID;
		if (!(m_goSignRoot == null) && m_goSignRoot.activeInHierarchy)
		{
			isKeyword = m_eNextSignType == eDialogSignType.Keyword;
			string strTitleKey = ((!isKeyword) ? "DIALOG_PFLOT_TITLE" : "DIALOG_KFLOT_TITLE");
			m_textSignType.text = GameGlobalUtil.GetXlsProgramText(strTitleKey);
			strImgPath = null;
			strTextListID = null;
			if (!isKeyword || m_strSignKey == null)
			{
				goto IL_012e;
			}
			Xls.CollKeyword data_byKey = Xls.CollKeyword.GetData_byKey(m_strSignKey);
			if (data_byKey != null)
			{
				strTextListID = data_byKey.m_strTitleID;
				Xls.ImageFile data_byKey2 = Xls.ImageFile.GetData_byKey(data_byKey.m_strIconImgID);
				if (data_byKey2 != null)
				{
					strImgPath = data_byKey2.m_strAssetPath + "_s";
					goto IL_012e;
				}
			}
		}
		goto IL_0241;
		IL_012e:
		if (isKeyword || m_strSignKey == null)
		{
			goto IL_01af;
		}
		Xls.Profiles data_byKey3 = Xls.Profiles.GetData_byKey(m_strSignKey);
		if (data_byKey3 != null)
		{
			strTextListID = data_byKey3.m_strName;
			Xls.CharData data_bySwitchIdx = Xls.CharData.GetData_bySwitchIdx(data_byKey3.m_iCtgIdx);
			Xls.ImageFile imageFile = null;
			if (data_bySwitchIdx != null)
			{
				imageFile = Xls.ImageFile.GetData_byKey(data_bySwitchIdx.m_strProfIcon);
			}
			if (imageFile != null)
			{
				strImgPath = imageFile.m_strAssetPath;
				goto IL_01af;
			}
		}
		goto IL_0241;
		IL_01af:
		if (strTextListID != null)
		{
			Xls.TextListData data_byKey4 = Xls.TextListData.GetData_byKey(strTextListID);
			if (data_byKey4 != null)
			{
				m_textSignName.text = data_byKey4.m_strTitle;
			}
		}
		if (strImgPath != null)
		{
			m_imgSignIcon.gameObject.SetActive(value: true);
			yield return GameGlobalUtil.GetSprRequestFromImgPath(strImgPath, isOneFileBundle: false);
			m_imgSignIcon.sprite = GameGlobalUtil.m_sprLoadFromImgXls;
		}
		goto IL_0241;
		IL_0241:
		m_isNextImgSetting = false;
		yield return null;
	}

	private void FreeSignUI()
	{
		m_textSignName.text = string.Empty;
		m_imgSignIcon.gameObject.SetActive(value: false);
	}
}
