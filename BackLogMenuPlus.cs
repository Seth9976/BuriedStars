using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackLogMenuPlus : MonoBehaviour
{
	public GameObject m_RootObject;

	public ScrollRect m_ScrollRect;

	public RectTransform m_ContentContanierRT;

	public float m_BottomSpacing = 80f;

	public Button m_CloseButtonIcon;

	public Text m_CloseButtonText;

	[Header("Content Src Object")]
	public GameObject m_ContentObjectNormal;

	public GameObject m_ContentObjectDialog;

	public GameObject m_ContentObjectFater;

	public GameObject m_ContentObjectMessenger;

	[Header("Scroll Members")]
	public GameObject m_ScrollRoot;

	public Button m_ScrollButtonToFirst;

	public Button m_ScrollButtonRStick;

	public bool m_ScrollLoopEnable = true;

	public UIUtil_ScrollHandler m_ScrollHandler = new UIUtil_ScrollHandler();

	public float m_ScrollRepeatTime = 0.2f;

	private float m_fScrollButtonPusingTime;

	private GameDefine.EventProc m_fpClosedCB;

	private Animator m_AnimatorCheck;

	private List<BackLogContentPlus> m_ContentObjects = new List<BackLogContentPlus>();

	private BackLogContentPlus m_OnCursorContent;

	private bool m_isNeedContentBuild;

	private bool m_isNeedAlignContents;

	private AudioManager m_AudioManager;

	private static BackLogMenuPlus s_Instance;

	public static BackLogMenuPlus instance => s_Instance;

	public void SetInstance()
	{
		s_Instance = this;
	}

	private void Awake()
	{
		if (m_ContentObjectNormal != null)
		{
			m_ContentObjectNormal.SetActive(value: false);
		}
		if (m_ContentObjectDialog != null)
		{
			m_ContentObjectDialog.SetActive(value: false);
		}
		if (m_ContentObjectFater != null)
		{
			m_ContentObjectFater.SetActive(value: false);
		}
		if (m_ContentObjectMessenger != null)
		{
			m_ContentObjectMessenger.SetActive(value: false);
		}
		m_ScrollHandler.Init(UIUtil_ScrollHandler.ScrollType.Vertical, m_ScrollRect, m_ContentContanierRT);
	}

	private void OnDestroy()
	{
		if (m_ScrollHandler != null)
		{
			m_ScrollHandler.ReleaseScroll();
		}
		m_fpClosedCB = null;
		m_AnimatorCheck = null;
		if (m_ContentObjects != null)
		{
			m_ContentObjects.Clear();
		}
		m_OnCursorContent = null;
		m_AudioManager = null;
		s_Instance = null;
	}

	private void Update()
	{
		if (m_AnimatorCheck != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_AnimatorCheck.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				DisappearComplete();
			}
			else if (m_isNeedContentBuild && ((currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.appear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f) || currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString())))
			{
				m_AnimatorCheck = null;
				m_isNeedContentBuild = false;
				StartCoroutine(BuildBacklogContent_Coroutine());
			}
		}
		else if (m_ScrollHandler.IsScrolling)
		{
			m_ScrollHandler.Update();
			if (m_ScrollHandler.CurScrollEvent == UIUtil_ScrollHandler.ScrollEvent.None && m_OnCursorContent != null)
			{
				ChangeOnCursorContent_byScrollPos();
			}
		}
		else
		{
			if (PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			float num = 0f;
			if (m_ScrollHandler.isScrollable)
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.TriangleButton, m_ScrollButtonToFirst);
				if (GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
				{
					OnClick_ScrollToTop();
				}
				num = GamePadInput.GetAxisValue(PadInput.GameInput.RStickY);
				if (!GameGlobalUtil.IsAlmostSame(num, 0f))
				{
					m_ScrollHandler.ScrollByDirection(num > 0f, Mathf.Abs(num));
					ChangeOnCursorContent_byScrollPos();
				}
				else
				{
					num = GamePadInput.GetAxisValue(PadInput.GameInput.LStickX);
					if (GameGlobalUtil.IsAlmostSame(num, 0f))
					{
						num = 0f - GamePadInput.GetMouseWheelScrollDelta();
					}
					if (num > 0.9f)
					{
						ChangeScrollPage(isUpSide: false);
					}
					else if (num < -0.9f)
					{
						ChangeScrollPage(isUpSide: true);
					}
				}
			}
			if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Down))
			{
				ChangeCurContentInfo(isUpSide: true);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Down))
			{
				ChangeCurContentInfo(isUpSide: false);
				m_fScrollButtonPusingTime = 0f;
			}
			else if (GamePadInput.IsLStickState_Up(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeCurContentInfo(isUpSide: true);
				}
			}
			else if (GamePadInput.IsLStickState_Down(PadInput.ButtonState.Pushing))
			{
				m_fScrollButtonPusingTime += Time.deltaTime;
				if (m_fScrollButtonPusingTime >= m_ScrollRepeatTime)
				{
					m_fScrollButtonPusingTime = 0f;
					ChangeCurContentInfo(isUpSide: false);
				}
			}
			if (m_OnCursorContent != null && ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_OnCursorContent.m_SelectionIconButton, m_OnCursorContent.m_ButtonVoicePlayOnCursor))
			{
				PlayVoice(m_OnCursorContent);
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_CloseButtonIcon);
				Close();
			}
		}
	}

	public void Show(GameDefine.EventProc fpClosedCB = null)
	{
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_AudioManager != null)
		{
			m_AudioManager.BackupMixerForBacklog();
		}
		FontManager.ResetTextFontByCurrentLanguage(m_CloseButtonText);
		m_CloseButtonText.text = GameGlobalUtil.GetXlsProgramText("BACKLOG_EXIT");
		m_fpClosedCB = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		base.gameObject.SetActive(value: true);
		m_RootObject.SetActive(value: true);
		m_isNeedContentBuild = true;
		m_AnimatorCheck = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.appear.ToString());
		m_ScrollRoot.SetActive(value: false);
	}

	public void Close(bool isEnableAnimation = true)
	{
		ClearBacklogContents();
		if (m_AudioManager != null)
		{
			m_AudioManager.StopVoice();
			m_AudioManager.PlayUISound("Menu_Cancel");
			m_AudioManager.RestoreMixerForBacklog();
		}
		if (!isEnableAnimation)
		{
			DisappearComplete();
			return;
		}
		m_AnimatorCheck = GameGlobalUtil.PlayUIAnimation_WithChidren(m_RootObject, GameDefine.UIAnimationState.disappear.ToString());
		if (m_AnimatorCheck == null)
		{
			DisappearComplete();
		}
	}

	private void DisappearComplete()
	{
		m_RootObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, null);
		}
		m_fpClosedCB = null;
	}

	private IEnumerator BuildBacklogContent_Coroutine()
	{
		ClearBacklogContents();
		List<BacklogData> backlogDatas = BacklogDataManager.backlogDatas;
		int iCount = backlogDatas?.Count ?? 0;
		if (iCount <= 0)
		{
			yield break;
		}
		LoadingSWatchIcon loadingIcon = LoadingSWatchIcon.Create(base.gameObject);
		BacklogData backlogData = null;
		BackLogContentPlus backlogContentComp = null;
		GameObject srcObj = null;
		GameObject newContentObj = null;
		TextGenerator textGenerator = new TextGenerator();
		for (int i = 0; i < iCount; i++)
		{
			backlogData = backlogDatas[i];
			srcObj = GetSrcObject_ByBacklogType(backlogData.m_Type);
			if (!(srcObj == null))
			{
				newContentObj = Object.Instantiate(srcObj);
				backlogContentComp = newContentObj.GetComponent<BackLogContentPlus>();
				if (!(backlogContentComp == null))
				{
					m_ContentObjects.Add(backlogContentComp);
					backlogContentComp.rectTransform.SetParent(m_ContentContanierRT, worldPositionStays: false);
					backlogContentComp.BacklogMenu = this;
					string convertText = TagText.TransTagTextToUnityText(backlogData.m_strDialog, isIgnoreHideTag: false);
					backlogContentComp.SetBacklogData(convertText, backlogData.m_strCharName, backlogData.m_colorCharName, backlogData.m_strVoiceName, backlogData.m_isContinuous);
					newContentObj.SetActive(value: true);
					yield return null;
					newContentObj.SetActive(value: false);
				}
			}
		}
		float fCurY = 0f;
		iCount = m_ContentObjects.Count;
		if (iCount > 0)
		{
			BackLogContentPlus content = null;
			RectTransform rtContent = null;
			while (!m_ContentObjects[iCount - 1].isInitialized)
			{
				yield return null;
			}
			for (int j = 0; j < iCount; j++)
			{
				content = m_ContentObjects[j];
				content.gameObject.SetActive(value: true);
			}
			Canvas.ForceUpdateCanvases();
			for (int k = 0; k < iCount; k++)
			{
				content = m_ContentObjects[k];
				rtContent = content.rectTransform;
				rtContent.anchoredPosition = new Vector2(0f, fCurY);
				rtContent.localRotation = Quaternion.identity;
				rtContent.localScale = Vector3.one;
				fCurY -= rtContent.rect.height;
			}
			fCurY -= m_BottomSpacing;
		}
		float fContainerSize = Mathf.Max(b: Mathf.Abs(fCurY), a: m_ScrollRect.viewport.rect.height);
		m_ContentContanierRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fContainerSize);
		m_ScrollHandler.ResetScrollRange();
		m_ScrollRoot.SetActive(m_ScrollHandler.isScrollable);
		m_ContentContanierRT.anchoredPosition = new Vector2(m_ContentContanierRT.anchoredPosition.x, m_ScrollHandler.scrollPos_Max);
		if (m_ContentObjects.Count > 0)
		{
			SetOnCursorContent(m_ContentObjects[m_ContentObjects.Count - 1]);
		}
		if (loadingIcon != null)
		{
			loadingIcon.Disappear();
		}
	}

	private bool IsBackLogNormalType(BacklogData backlogData)
	{
		return backlogData.m_Type == BacklogData._Type.MonoText || string.IsNullOrEmpty(backlogData.m_strCharName);
	}

	private GameObject GetSrcObject_ByBacklogType(BacklogData._Type type)
	{
		return type switch
		{
			BacklogData._Type.MonoText => m_ContentObjectNormal, 
			BacklogData._Type.Dialog => m_ContentObjectDialog, 
			BacklogData._Type.Fater => m_ContentObjectFater, 
			BacklogData._Type.Messenger => m_ContentObjectMessenger, 
			_ => null, 
		};
	}

	private void ClearBacklogContents()
	{
		int count = m_ContentObjects.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				Object.Destroy(m_ContentObjects[i].gameObject);
			}
			m_ContentObjects.Clear();
		}
	}

	private bool ChangeCurContentInfo(bool isUpSide, bool isAdjustScrollPos = true)
	{
		int count = m_ContentObjects.Count;
		if (m_OnCursorContent == null)
		{
			if (count > 0)
			{
				SetOnCursorContent(m_ContentObjects[count - 1], isAdjustScrollPos: false);
			}
			return false;
		}
		int num = m_ContentObjects.IndexOf(m_OnCursorContent);
		num = ((!isUpSide) ? (num + 1) : (num - 1));
		if (num < 0)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = count - 1;
		}
		else if (num >= count)
		{
			if (!m_ScrollLoopEnable)
			{
				return false;
			}
			num = 0;
		}
		SetOnCursorContent(m_ContentObjects[num], isAdjustScrollPos);
		if (m_AudioManager != null)
		{
			m_AudioManager.PlayUISound("Menu_Select");
		}
		return true;
	}

	private void ChangeOnCursorContent_byScrollPos()
	{
		if (m_OnCursorContent == null)
		{
			return;
		}
		float height = m_ScrollRect.viewport.rect.height;
		float num = 0f - m_ContentContanierRT.offsetMax.y;
		float f = num - height;
		int num2 = Mathf.CeilToInt(num);
		int num3 = Mathf.FloorToInt(f);
		float y = m_OnCursorContent.rectTransform.offsetMax.y;
		float f2 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
		int num4 = m_ContentObjects.IndexOf(m_OnCursorContent);
		RectTransform rectTransform = null;
		if (Mathf.CeilToInt(f2) < num3)
		{
			int num5 = num4;
			while (num5 > 0)
			{
				num5--;
				rectTransform = m_ContentObjects[num5].rectTransform;
				f2 = rectTransform.offsetMax.y - rectTransform.rect.height * rectTransform.localScale.y;
				if (Mathf.CeilToInt(f2) >= num3)
				{
					SetOnCursorContent(m_ContentObjects[num5]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
		else
		{
			if (Mathf.FloorToInt(y) <= num2)
			{
				return;
			}
			int count = m_ContentObjects.Count;
			for (int i = num4 + 1; i < count; i++)
			{
				rectTransform = m_ContentObjects[i].rectTransform;
				y = rectTransform.offsetMax.y;
				if (Mathf.FloorToInt(y) <= num2)
				{
					SetOnCursorContent(m_ContentObjects[i]);
					if (m_AudioManager != null)
					{
						m_AudioManager.PlayUISound("Menu_Select");
					}
					break;
				}
			}
		}
	}

	private bool ChangeScrollPage(bool isUpSide)
	{
		if (!m_ScrollHandler.isScrollable || m_ScrollHandler.IsScrolling)
		{
			return false;
		}
		if ((isUpSide && m_ScrollHandler.scrollPos <= 0f) || (!isUpSide && m_ScrollHandler.scrollPos >= m_ScrollHandler.scrollPos_Max))
		{
			return false;
		}
		float num = m_ScrollRect.viewport.rect.height - m_BottomSpacing;
		float value = m_ScrollHandler.scrollPos + ((!isUpSide) ? num : (0f - num));
		value = Mathf.Clamp(value, 0f, m_ScrollHandler.scrollPos_Max);
		if (GameGlobalUtil.IsAlmostSame(value, m_ScrollHandler.scrollPos))
		{
			return false;
		}
		m_ScrollHandler.ScrollToTargetPos(0f - value);
		return true;
	}

	public void SetOnCursorContent(BackLogContentPlus newCursorContent, bool isAdjustScrollPos = true)
	{
		if (m_OnCursorContent != null)
		{
			m_OnCursorContent.selected = false;
		}
		if (newCursorContent != null)
		{
			newCursorContent.selected = true;
		}
		m_OnCursorContent = newCursorContent;
		if (isAdjustScrollPos)
		{
			AdjustScrollPos_byOnCursonContent();
		}
	}

	public void TouchContent(BackLogContentPlus cursorContent)
	{
		SetOnCursorContent(cursorContent);
	}

	private void AdjustScrollPos_byOnCursonContent()
	{
		if (!(m_OnCursorContent == null))
		{
			float height = m_ScrollRect.viewport.rect.height;
			float num = 0f - m_ContentContanierRT.offsetMax.y;
			float num2 = num - height;
			float y = m_OnCursorContent.rectTransform.offsetMax.y;
			float num3 = m_OnCursorContent.rectTransform.offsetMax.y - m_OnCursorContent.rectTransform.rect.height * m_OnCursorContent.rectTransform.localScale.y;
			num3 -= m_BottomSpacing;
			if (num3 < num2)
			{
				float fTargetPos = num3 + height;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos);
			}
			else if (y > num)
			{
				float fTargetPos2 = y;
				m_ScrollHandler.ScrollToTargetPos(fTargetPos2);
			}
		}
	}

	public void OnClick_ScrollToTop()
	{
		if (m_ScrollHandler.isScrollable)
		{
			m_ScrollHandler.ScrollToTargetPos(0f);
			int count = m_ContentObjects.Count;
			if (count > 0)
			{
				SetOnCursorContent(m_ContentObjects[0]);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Scroll_Page");
			}
		}
	}

	public void PlayVoice(BackLogContentPlus content)
	{
		if (!(content == null) && content.IsExistVoice)
		{
			if (content != m_OnCursorContent)
			{
				SetOnCursorContent(content);
			}
			if (m_AudioManager != null)
			{
				m_AudioManager.StopVoice();
				m_AudioManager.PlayVoice(content.VoiceName);
			}
		}
	}

	public void OnClick_CloseButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			Close();
		}
	}
}
