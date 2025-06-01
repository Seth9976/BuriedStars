using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class ImageDetailViewer : MonoBehaviour
{
	private enum Mode
	{
		Unknown,
		Normal,
		OnlyImage,
		ShowImage
	}

	private enum AniStateSpecial
	{
		appear_no_text,
		idle_no_text,
		disappear_no_text,
		next_page_disappear,
		next_page_appear,
		prev_page_disappear,
		prev_page_appear
	}

	public Canvas m_Canvas;

	public AnimationEventHandler m_AniEventHandler;

	public GameObject m_TouchBlockObject;

	[Header("Content Objects")]
	public Image m_imageContent;

	public Text m_textContent;

	[Header("Page Scroll Objects")]
	public GridLayoutGroup m_pageNodeGroup;

	public GameObject m_pageArrowLeft;

	public GameObject m_pageArrowRight;

	public Image m_pageNodeSrcImage;

	public Sprite m_pageSpriteNotSelected;

	public Sprite m_pageSpriteSelected;

	[Header("Guide Objects")]
	public GameObject m_GuideObjectRootLeft;

	public GameObject m_GuideObjectRootRight;

	public Button m_PadInputButton_HoriDir;

	public Text m_GuideText_PageMove;

	public Button m_PadInputButton_ShowImage;

	public Text m_GuideText_ShowImage;

	public Button m_PadInputButton_NextPage;

	public Text m_GuideText_NextPage;

	public Button m_PadInputButton_Exit;

	public Text m_GuideText_Exit;

	[Header("Touch Buttons")]
	public Button m_TouchButton_Left;

	public Button m_TouchButton_Right;

	public Button m_TouchButton_HideText;

	public Button m_TouchButton_Exit;

	private List<string> m_textContentKeys;

	private List<Image> m_pageNodeImages = new List<Image>();

	private int m_textPageCount;

	private int m_curTextPageIndex = -1;

	private CanvasScaler m_CanvasScaler;

	private AssetBundleObjectHandler m_assetbundleObjectHdr;

	private GameDefine.EventProc m_fpClosedCB;

	private Animator m_animatorCheck;

	private bool m_isInputBlocked;

	private const string c_aniStateName_None = "none";

	private Mode m_curMode;

	private void Start()
	{
		if (m_pageNodeSrcImage != null)
		{
			m_pageNodeSrcImage.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if (m_GuideText_Exit != null)
		{
			m_GuideText_Exit.text = GameGlobalUtil.GetXlsProgramText("IMAGE_DOCUMENT_EXIT");
		}
		if (m_GuideText_PageMove != null)
		{
			m_GuideText_PageMove.text = GameGlobalUtil.GetXlsProgramText("IMAGE_DOCUMENT_CHANGE_PAGE");
		}
		if (m_GuideText_NextPage != null)
		{
			m_GuideText_NextPage.text = GameGlobalUtil.GetXlsProgramText("IMAGE_DOCUMENT_NEXT_PAGE");
		}
		if (m_GuideText_ShowImage != null)
		{
			m_GuideText_ShowImage.text = GameGlobalUtil.GetXlsProgramText("IMAGE_DOCUMENT_SHOW_IMAGE");
		}
		if (m_AniEventHandler != null)
		{
			m_AniEventHandler.fpEventCB = OnAniEventProc;
		}
	}

	private void OnDestroy()
	{
		if (m_textContentKeys != null)
		{
			m_textContentKeys.Clear();
		}
		m_textContentKeys = null;
		if (m_pageNodeImages != null)
		{
			m_pageNodeImages.Clear();
		}
		m_pageNodeImages = null;
		m_assetbundleObjectHdr = null;
		m_fpClosedCB = null;
		m_animatorCheck = null;
	}

	private void Update()
	{
		switch (m_curMode)
		{
		case Mode.Normal:
			Update_Normal();
			break;
		case Mode.OnlyImage:
			Update_OnlyImage();
			break;
		case Mode.ShowImage:
			Update_ShowImage();
			break;
		}
	}

	private void Update_Normal()
	{
		if (m_animatorCheck != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_animatorCheck.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				ClosedComplete();
			}
		}
		else
		{
			if (m_isInputBlocked)
			{
				return;
			}
			if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton))
			{
				AudioManager.instance.PlayUISound("Menu_Cancel");
				ButtonPadInput.PressInputButton(PadInput.GameInput.CrossButton, m_PadInputButton_Exit);
				Close();
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton))
			{
				ChangeMode(Mode.ShowImage);
				AudioManager.instance.PlayUISound("Menu_Detail");
				ButtonPadInput.PressInputButton(PadInput.GameInput.SquareButton, m_PadInputButton_ShowImage);
			}
			else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				if (ChangeCurrentPage(isNextPage: true))
				{
					AudioManager.instance.PlayUISound("Push_PopOK");
					ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_PadInputButton_NextPage);
				}
			}
			else if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
			{
				if (ChangeCurrentPage(isNextPage: false))
				{
					AudioManager.instance.PlayUISound("Push_PopOK");
					ButtonPadInput.PressInputButton(PadInput.GameInput.LStickX, m_PadInputButton_HoriDir);
				}
			}
			else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down) && ChangeCurrentPage(isNextPage: true))
			{
				AudioManager.instance.PlayUISound("Push_PopOK");
				ButtonPadInput.PressInputButton(PadInput.GameInput.LStickX, m_PadInputButton_HoriDir);
			}
		}
	}

	private void Update_OnlyImage()
	{
		if (m_animatorCheck != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_animatorCheck.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(AniStateSpecial.disappear_no_text.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				ClosedComplete();
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
		{
			AudioManager.instance.PlayUISound("Menu_Cancel");
			Close();
		}
	}

	private void Update_ShowImage()
	{
		if (m_animatorCheck != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_animatorCheck.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				ClosedComplete();
			}
		}
		else if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
		{
			AudioManager.instance.PlayUISound("Menu_Cancel");
			ChangeMode(Mode.Normal);
		}
	}

	private void ChangeMode(Mode newMode)
	{
		switch (newMode)
		{
		case Mode.Normal:
			if (m_curMode == Mode.ShowImage)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.idle.ToString());
			}
			else
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.appear.ToString());
			}
			if (m_GuideObjectRootLeft != null)
			{
				m_GuideObjectRootLeft.SetActive(value: true);
			}
			if (m_GuideObjectRootRight != null)
			{
				m_GuideObjectRootRight.SetActive(value: true);
			}
			if (m_TouchButton_HideText != null)
			{
				m_TouchButton_HideText.gameObject.SetActive(value: true);
			}
			if (m_TouchButton_Exit != null)
			{
				m_TouchButton_Exit.gameObject.SetActive(value: true);
			}
			SetVisibleScrollObjects(isShow: true);
			break;
		case Mode.OnlyImage:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, AniStateSpecial.appear_no_text.ToString());
			if (m_GuideObjectRootLeft != null)
			{
				m_GuideObjectRootLeft.SetActive(value: true);
			}
			if (m_GuideObjectRootRight != null)
			{
				m_GuideObjectRootRight.SetActive(value: false);
			}
			if (m_TouchButton_HideText != null)
			{
				m_TouchButton_HideText.gameObject.SetActive(value: false);
			}
			if (m_TouchButton_Exit != null)
			{
				m_TouchButton_Exit.gameObject.SetActive(value: true);
			}
			SetVisibleScrollObjects(isShow: false);
			break;
		case Mode.ShowImage:
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, AniStateSpecial.idle_no_text.ToString());
			if (m_GuideObjectRootLeft != null)
			{
				m_GuideObjectRootLeft.SetActive(value: false);
			}
			if (m_GuideObjectRootRight != null)
			{
				m_GuideObjectRootRight.SetActive(value: false);
			}
			if (m_TouchButton_HideText != null)
			{
				m_TouchButton_HideText.gameObject.SetActive(value: true);
			}
			if (m_TouchButton_Exit != null)
			{
				m_TouchButton_Exit.gameObject.SetActive(value: false);
			}
			SetVisibleScrollObjects(isShow: false);
			break;
		}
		m_curMode = newMode;
	}

	private void EnableInputBlock(bool isEnable)
	{
		m_isInputBlocked = isEnable;
		if (m_TouchBlockObject != null)
		{
			m_TouchBlockObject.SetActive(isEnable);
		}
	}

	public void SetCanvasOrder(int order)
	{
		if (m_Canvas == null)
		{
			m_Canvas = GetComponentInChildren<Canvas>();
			if (m_Canvas == null)
			{
				return;
			}
		}
		m_Canvas.sortingOrder = order;
	}

	public IEnumerator ShowImage(string collectionImageDataKey, GameDefine.EventProc fpClosedCB = null)
	{
		Xls.CollImages xlsCollectionImageData = Xls.CollImages.GetData_byKey(collectionImageDataKey);
		if (xlsCollectionImageData == null)
		{
			fpClosedCB?.Invoke(this, null);
		}
		else
		{
			yield return MainLoadThing.instance.StartCoroutine(ShowImage(xlsCollectionImageData, fpClosedCB));
		}
	}

	public IEnumerator ShowImage(Xls.CollImages xlsCollectionImageData, GameDefine.EventProc fpClosedCB = null)
	{
		if (xlsCollectionImageData == null)
		{
			fpClosedCB?.Invoke(this, null);
			yield break;
		}
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		sbyte switchState = gameSwitch.GetCollImage(xlsCollectionImageData.m_iIdx);
		if (switchState == 0)
		{
			gameSwitch.SetCollImage(xlsCollectionImageData.m_iIdx, 1, isPop: true);
		}
		Xls.ImageFile xlsImageData = Xls.ImageFile.GetData_byKey(xlsCollectionImageData.m_strIDImg);
		yield return MainLoadThing.instance.StartCoroutine(ShowImage(xlsImageData, xlsCollectionImageData.m_strIDColImageDest, fpClosedCB));
	}

	public IEnumerator ShowImage(Xls.ImageFile xlsImageData, string keyCollImageDestListData, GameDefine.EventProc fpClosedCB = null)
	{
		Sprite sprite = null;
		if (xlsImageData != null)
		{
			m_assetbundleObjectHdr = new AssetBundleObjectHandler(xlsImageData.m_strAssetPath);
			yield return MainLoadThing.instance.StartCoroutine(m_assetbundleObjectHdr.LoadAssetBundle());
			yield return null;
			sprite = m_assetbundleObjectHdr.GetLoadedAsset_ToSprite();
		}
		ShowImage(sprite, keyCollImageDestListData, fpClosedCB);
	}

	public void ShowImage(Sprite sprite, string keyCollImageDestListData, GameDefine.EventProc fpClosedCB = null)
	{
		m_fpClosedCB = fpClosedCB;
		Text[] textComps = new Text[5] { m_textContent, m_GuideText_PageMove, m_GuideText_ShowImage, m_GuideText_NextPage, m_GuideText_Exit };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
		base.gameObject.SetActive(value: true);
		if (m_imageContent != null)
		{
			m_imageContent.sprite = sprite;
			if (sprite != null)
			{
				float num = 1f;
				RectTransform component = m_imageContent.gameObject.GetComponent<RectTransform>();
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.rect.width * num);
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.rect.height * num);
			}
		}
		m_textContentKeys = null;
		if (!string.IsNullOrEmpty(keyCollImageDestListData))
		{
			List<string> collectionImageDestList = XlsDataHandler.GetCollectionImageDestList(keyCollImageDestListData);
			if (collectionImageDestList != null && collectionImageDestList.Count > 0)
			{
				m_textContentKeys = collectionImageDestList;
			}
		}
		SetTextContentPageCount((m_textContentKeys != null) ? m_textContentKeys.Count : 0);
		SetCurrentPage(0);
		EnableInputBlock(isEnable: false);
		ChangeMode((m_textPageCount > 0) ? Mode.Normal : Mode.OnlyImage);
	}

	public void Close()
	{
		EnableInputBlock(isEnable: true);
		string strMot = ((m_curMode != Mode.OnlyImage) ? GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear) : AniStateSpecial.disappear_no_text.ToString());
		m_animatorCheck = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, strMot);
		if (m_animatorCheck == null)
		{
			ClosedComplete();
		}
	}

	private void ClosedComplete()
	{
		m_animatorCheck = null;
		base.gameObject.SetActive(value: false);
		if (m_assetbundleObjectHdr != null)
		{
			m_assetbundleObjectHdr.UnloadAssetBundle();
			m_assetbundleObjectHdr = null;
		}
		if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, null);
		}
	}

	private void SetVisibleScrollObjects(bool isShow)
	{
		bool flag = m_textPageCount > 1;
		bool active = isShow && flag;
		if (m_GuideText_NextPage != null)
		{
			m_GuideText_NextPage.gameObject.SetActive(active);
		}
		if (m_GuideText_PageMove != null)
		{
			m_GuideText_PageMove.gameObject.SetActive(active);
		}
		if (m_PadInputButton_HoriDir != null)
		{
			m_PadInputButton_HoriDir.gameObject.SetActive(active);
		}
		if (m_PadInputButton_NextPage != null)
		{
			m_PadInputButton_NextPage.gameObject.SetActive(active);
		}
		if (m_TouchButton_Left != null)
		{
			m_TouchButton_Left.gameObject.SetActive(active);
		}
		if (m_TouchButton_Right != null)
		{
			m_TouchButton_Right.gameObject.SetActive(active);
		}
		m_pageNodeGroup.gameObject.SetActive(active);
	}

	private void SetTextContentPageCount(int count)
	{
		if (m_pageNodeSrcImage == null || m_pageNodeGroup == null)
		{
			return;
		}
		m_textPageCount = count;
		m_curTextPageIndex = -1;
		if (count < 2)
		{
			m_pageNodeGroup.gameObject.SetActive(value: false);
			if (m_GuideObjectRootRight != null)
			{
				m_GuideObjectRootRight.SetActive(value: false);
			}
			return;
		}
		int count2 = m_pageNodeImages.Count;
		if (count > count2)
		{
			RectTransform component = m_pageNodeGroup.gameObject.GetComponent<RectTransform>();
			int num = count - count2;
			int num2 = num;
			while (num2 > 0)
			{
				num2--;
				GameObject gameObject = Object.Instantiate(m_pageNodeSrcImage.gameObject, component, worldPositionStays: false);
				Image component2 = gameObject.GetComponent<Image>();
				m_pageNodeImages.Add(component2);
			}
			if (m_pageArrowLeft != null)
			{
				RectTransform component3 = m_pageArrowLeft.GetComponent<RectTransform>();
				if (component3 != null)
				{
					component3.SetAsFirstSibling();
				}
			}
			if (m_pageArrowRight != null)
			{
				RectTransform component4 = m_pageArrowRight.GetComponent<RectTransform>();
				if (component4 != null)
				{
					component4.SetAsLastSibling();
				}
			}
		}
		int i = 0;
		int count3;
		for (count3 = m_pageNodeImages.Count; i < count && i < count3; i++)
		{
			Image image = m_pageNodeImages[i];
			image.gameObject.SetActive(value: true);
			image.sprite = m_pageSpriteNotSelected;
		}
		for (; i < count3; i++)
		{
			m_pageNodeImages[i].gameObject.SetActive(value: false);
		}
		m_pageNodeGroup.gameObject.SetActive(value: true);
		if (m_GuideObjectRootRight != null)
		{
			m_GuideObjectRootRight.SetActive(value: true);
		}
	}

	private void SetCurrentPage(int pageIndex, bool isIgnoreSame = false)
	{
		if (m_curTextPageIndex != pageIndex || !isIgnoreSame)
		{
			int num = Mathf.Min(m_textPageCount, m_pageNodeImages.Count);
			if (m_curTextPageIndex >= 0 && m_curTextPageIndex < num)
			{
				m_pageNodeImages[m_curTextPageIndex].sprite = m_pageSpriteNotSelected;
			}
			if (pageIndex >= 0 && pageIndex < num)
			{
				m_pageNodeImages[pageIndex].sprite = m_pageSpriteSelected;
			}
			m_curTextPageIndex = pageIndex;
			if (m_textContentKeys != null && m_curTextPageIndex >= 0 && m_curTextPageIndex < m_textContentKeys.Count && m_textContent != null)
			{
				string key = m_textContentKeys[m_curTextPageIndex];
				Xls.TextData data_byKey = Xls.TextData.GetData_byKey(key);
				m_textContent.text = ((data_byKey == null) ? string.Empty : data_byKey.m_strTxt);
			}
		}
	}

	private bool ChangeCurrentPage(bool isNextPage)
	{
		if (m_textPageCount < 2)
		{
			return false;
		}
		if (!isNextPage)
		{
			if (m_curTextPageIndex <= 0)
			{
				return false;
			}
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, AniStateSpecial.prev_page_disappear.ToString());
		}
		else
		{
			if (m_curTextPageIndex >= m_textPageCount - 1)
			{
				return false;
			}
			GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, AniStateSpecial.next_page_disappear.ToString());
		}
		EnableInputBlock(isEnable: true);
		return true;
	}

	private void OnAniEventProc(object sender, object arg)
	{
		switch (arg.ToString())
		{
		case "prev":
			SetCurrentPage(m_curTextPageIndex - 1);
			break;
		case "next":
			SetCurrentPage(m_curTextPageIndex + 1);
			break;
		case "finish":
			EnableInputBlock(isEnable: false);
			break;
		}
	}

	public void OnClickButton_HideText()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Menu_Detail");
			if (m_curMode != Mode.ShowImage)
			{
				ChangeMode(Mode.ShowImage);
			}
			else
			{
				ChangeMode(Mode.Normal);
			}
		}
	}

	public void OnClickButton_Exit()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Menu_Cancel");
			Close();
		}
	}

	public void OnClickButton_ScrollLeft()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Push_PopOK");
			ChangeCurrentPage(isNextPage: false);
		}
	}

	public void OnClickButton_ScrollRight()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			AudioManager.instance.PlayUISound("Push_PopOK");
			ChangeCurrentPage(isNextPage: true);
		}
	}
}
