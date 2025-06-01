using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;
using UnityEngine.UI;

public class CommonButtonGuide : MonoBehaviour
{
	public enum AlignType
	{
		Left,
		Center,
		Right
	}

	[Serializable]
	public class BackGoundImageInfo
	{
		public Sprite m_Image0;

		public Sprite m_Image1;
	}

	private class GameInputInfo
	{
		public PadInput.GameInput m_gameInputType = PadInput.GameInput.None;

		public bool isIgnoreAxis;
	}

	private class IconImageInfo
	{
		public GameObject m_gameObject;

		public RectTransform m_rectTrans;

		public Image m_ImageComp;

		public Sprite m_iconNormal;

		public Sprite m_iconPressed;

		public void Destory()
		{
			if (m_gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(m_gameObject);
				m_gameObject = null;
				m_rectTrans = null;
				m_ImageComp = null;
			}
			if (m_iconNormal != null)
			{
				m_iconNormal = null;
			}
			if (m_iconPressed != null)
			{
				m_iconPressed = null;
			}
		}
	}

	private class ContentInfo
	{
		public string m_GuideText = string.Empty;

		public GameObject m_GuideTextObject;

		public RectTransform m_rtGuideText;

		public Text m_textGuideComp;

		public List<GameInputInfo> m_gameInputInfos = new List<GameInputInfo>();

		public List<IconImageInfo> m_iconImageInfos = new List<IconImageInfo>();

		public bool m_isEnabled = true;

		public bool m_isBulided;

		public bool m_isPressed;

		public float m_fRemainActiveTime;

		public void Destroy()
		{
			if (m_GuideTextObject != null)
			{
				UnityEngine.Object.DestroyImmediate(m_GuideTextObject);
				m_GuideTextObject = null;
				m_rtGuideText = null;
				m_textGuideComp = null;
			}
			int count = m_iconImageInfos.Count;
			for (int i = 0; i < count; i++)
			{
				m_iconImageInfos[i].Destory();
			}
			m_iconImageInfos.Clear();
		}

		public GameInputInfo FindGameInputInfo(PadInput.GameInput gameInput)
		{
			int count = m_gameInputInfos.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_gameInputInfos[i].m_gameInputType == gameInput)
				{
					return m_gameInputInfos[i];
				}
			}
			return null;
		}

		public GameInputInfo AddGameInputInfo(PadInput.GameInput gameInput, bool isIgnoreAxis)
		{
			GameInputInfo gameInputInfo = FindGameInputInfo(gameInput);
			if (gameInputInfo == null)
			{
				gameInputInfo = new GameInputInfo();
				gameInputInfo.m_gameInputType = gameInput;
				m_gameInputInfos.Add(gameInputInfo);
			}
			gameInputInfo.isIgnoreAxis = isIgnoreAxis;
			return gameInputInfo;
		}

		public void SetEnable(bool isEnable)
		{
			if (m_GuideTextObject != null)
			{
				m_GuideTextObject.SetActive(isEnable);
			}
			int count = m_iconImageInfos.Count;
			IconImageInfo iconImageInfo = null;
			for (int i = 0; i < count; i++)
			{
				iconImageInfo = m_iconImageInfos[i];
				if (iconImageInfo != null && !(iconImageInfo.m_gameObject == null))
				{
					iconImageInfo.m_gameObject.SetActive(isEnable);
				}
			}
			m_isEnabled = isEnable;
			SetPressed(isPressed: false);
			m_fRemainActiveTime = 0f;
		}

		public void SetPressed(bool isPressed, bool isIngnoreSame = true)
		{
			if (isIngnoreSame && isPressed == m_isPressed)
			{
				return;
			}
			int count = m_iconImageInfos.Count;
			IconImageInfo iconImageInfo = null;
			for (int i = 0; i < count; i++)
			{
				iconImageInfo = m_iconImageInfos[i];
				if (iconImageInfo != null && !(iconImageInfo.m_ImageComp == null))
				{
					iconImageInfo.m_ImageComp.sprite = ((!isPressed || !(iconImageInfo.m_iconPressed != null)) ? iconImageInfo.m_iconNormal : iconImageInfo.m_iconPressed);
				}
			}
			m_isPressed = isPressed;
		}

		public void Update()
		{
			if (m_isPressed && !(m_fRemainActiveTime <= 0f))
			{
				m_fRemainActiveTime -= Time.deltaTime;
				if (m_fRemainActiveTime <= 0f)
				{
					SetPressed(isPressed: false);
					m_fRemainActiveTime = 0f;
				}
			}
		}
	}

	public class ImageAssetCacheData
	{
		public string m_bundleName = string.Empty;

		public AssetBundle m_bundle;

		public SortedDictionary<string, Sprite> m_imageCaches = new SortedDictionary<string, Sprite>();
	}

	public GameObject m_RootObject;

	public RectTransform m_rtContentContainer;

	[Header("Background Images")]
	public Image m_BackGound0;

	public Image m_BackGound1;

	public BackGoundImageInfo[] m_BackGoundImages;

	[Header("Content Info")]
	public GameObject m_IconObjectOrigin;

	public GameObject m_TextObjectOrigin;

	public float m_LeftMargin;

	public float m_RightMargin;

	public float m_ElementInteval;

	[HideInInspector]
	public float m_fButtonPressEffTime;

	private AlignType m_AlignType;

	private bool m_isAlignReserved;

	private int m_defCanvasSortOrder;

	private List<ContentInfo> m_ContentInfos = new List<ContentInfo>();

	private static ImageAssetCacheData s_imageChachesBase = new ImageAssetCacheData();

	private static ImageAssetCacheData s_imageChachesXBox = new ImageAssetCacheData();

	private static ImageAssetCacheData s_imageChachesNS = new ImageAssetCacheData();

	private static ImageAssetCacheData s_imageChachesKeyboard = new ImageAssetCacheData();

	private void Awake()
	{
	}

	private void Start()
	{
		Canvas componentInChildren = m_RootObject.GetComponentInChildren<Canvas>();
		if (componentInChildren != null)
		{
			m_defCanvasSortOrder = componentInChildren.sortingOrder;
		}
	}

	public void Init()
	{
		m_fButtonPressEffTime = GameGlobalUtil.GetXlsProgramDefineStrToFloat("BUTTON_PRESS_EFF_TIME");
	}

	private void Update()
	{
		int count = m_ContentInfos.Count;
		ContentInfo contentInfo = null;
		for (int i = 0; i < count; i++)
		{
			contentInfo = m_ContentInfos[i];
			if (contentInfo.m_isBulided && contentInfo.m_isEnabled)
			{
				contentInfo.Update();
			}
		}
	}

	private void LateUpdate()
	{
		if (m_isAlignReserved)
		{
			AlignContents_Run(m_AlignType, isIgnoreSame: false);
			m_isAlignReserved = false;
		}
	}

	public void SetShow(bool isShow)
	{
		if (!(base.gameObject == null))
		{
			base.gameObject.SetActive(isShow);
		}
	}

	public bool IsShow()
	{
		return base.gameObject.activeSelf;
	}

	public void SetCanvasSortOrder(int iOrder)
	{
		if (!(m_RootObject == null))
		{
			Canvas componentInChildren = m_RootObject.GetComponentInChildren<Canvas>();
			if (componentInChildren != null)
			{
				componentInChildren.sortingOrder = iOrder;
			}
		}
	}

	public void SetCanvasSortOrder_Default()
	{
		SetCanvasSortOrder(m_defCanvasSortOrder);
	}

	public void ClearContents()
	{
		int count = m_ContentInfos.Count;
		if (count > 0)
		{
			ContentInfo contentInfo = null;
			for (int i = 0; i < count; i++)
			{
				contentInfo = m_ContentInfos[i];
				contentInfo.Destroy();
			}
			m_ContentInfos.Clear();
			SetCanvasSortOrder(m_defCanvasSortOrder);
		}
	}

	private ContentInfo FindContentInfo_byGuideText(string guideText)
	{
		int count = m_ContentInfos.Count;
		for (int i = 0; i < count; i++)
		{
			if (string.Equals(m_ContentInfos[i].m_GuideText, guideText))
			{
				return m_ContentInfos[i];
			}
		}
		return null;
	}

	public bool IsExistContentInfo_byGuideText(string guideText)
	{
		return FindContentInfo_byGuideText(guideText) != null;
	}

	public void AddContent(string strGuideText, PadInput.GameInput gameInputType, bool isIngoreAxis = false)
	{
		if (!string.IsNullOrEmpty(strGuideText) && gameInputType != PadInput.GameInput.None)
		{
			ContentInfo contentInfo = FindContentInfo_byGuideText(strGuideText);
			if (contentInfo == null)
			{
				contentInfo = new ContentInfo();
				contentInfo.m_GuideText = strGuideText;
				m_ContentInfos.Add(contentInfo);
			}
			contentInfo.AddGameInputInfo(gameInputType, isIngoreAxis);
		}
	}

	public void BuildContents(AlignType alignType)
	{
		int count = m_ContentInfos.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				CreateContentObjects(m_ContentInfos[i]);
			}
			AlignContents(alignType);
		}
	}

	public void ReflashContents()
	{
		int count = m_ContentInfos.Count;
		if (count > 0)
		{
			bool[] array = new bool[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = m_ContentInfos[i].m_isEnabled;
			}
			BuildContents(m_AlignType);
			for (int j = 0; j < count; j++)
			{
				m_ContentInfos[j].SetEnable(array[j]);
			}
			AlignContents(m_AlignType);
		}
	}

	private void CreateContentObjects(ContentInfo contentInfo)
	{
		if (contentInfo.m_isBulided)
		{
			contentInfo.Destroy();
		}
		contentInfo.m_isBulided = true;
		int count = contentInfo.m_gameInputInfos.Count;
		if (count <= 0)
		{
			return;
		}
		GameInputInfo gameInputInfo = null;
		List<string> list = new List<string>();
		for (int i = 0; i < count; i++)
		{
			gameInputInfo = contentInfo.m_gameInputInfos[i];
			string[] iconImageNames = GamePadInput.GetIconImageNames(gameInputInfo.m_gameInputType, gameInputInfo.isIgnoreAxis);
			if (iconImageNames == null || iconImageNames.Length <= 0)
			{
				continue;
			}
			string[] array = iconImageNames;
			foreach (string item in array)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		count = list.Count;
		if (count <= 0)
		{
			return;
		}
		for (int k = 0; k < count; k++)
		{
			IconImageInfo iconImageInfo = CreateIconImageInfo(list[k]);
			if (iconImageInfo != null)
			{
				contentInfo.m_iconImageInfos.Add(iconImageInfo);
			}
		}
		if (contentInfo.m_iconImageInfos.Count > 0)
		{
			contentInfo.m_GuideTextObject = UnityEngine.Object.Instantiate(m_TextObjectOrigin);
			contentInfo.m_rtGuideText = contentInfo.m_GuideTextObject.GetComponent<RectTransform>();
			contentInfo.m_textGuideComp = contentInfo.m_GuideTextObject.GetComponent<Text>();
			contentInfo.m_rtGuideText.SetParent(m_rtContentContainer, worldPositionStays: false);
			contentInfo.m_rtGuideText.anchoredPosition = Vector2.zero;
			contentInfo.m_rtGuideText.localRotation = Quaternion.identity;
			contentInfo.m_rtGuideText.localScale = Vector3.one;
			FontManager.ResetTextFontByCurrentLanguage(contentInfo.m_textGuideComp);
			if (contentInfo.m_textGuideComp != null)
			{
				contentInfo.m_textGuideComp.text = contentInfo.m_GuideText;
			}
			contentInfo.m_GuideTextObject.SetActive(value: true);
			contentInfo.SetPressed(isPressed: false, isIngnoreSame: false);
		}
	}

	private IconImageInfo CreateIconImageInfo(string iconImageName)
	{
		string imageName = iconImageName;
		Sprite buttonImageInCache = GetButtonImageInCache(imageName);
		if (buttonImageInCache == null)
		{
			return null;
		}
		IconImageInfo iconImageInfo = new IconImageInfo();
		iconImageInfo.m_iconNormal = buttonImageInCache;
		imageName = string.Format("{0}{1}", iconImageName, "_pressed");
		iconImageInfo.m_iconPressed = GetButtonImageInCache(imageName);
		if (iconImageInfo.m_iconPressed == null)
		{
		}
		iconImageInfo.m_gameObject = UnityEngine.Object.Instantiate(m_IconObjectOrigin);
		iconImageInfo.m_rectTrans = iconImageInfo.m_gameObject.GetComponent<RectTransform>();
		iconImageInfo.m_ImageComp = iconImageInfo.m_gameObject.GetComponent<Image>();
		float height = m_rtContentContainer.rect.height;
		float num = iconImageInfo.m_iconNormal.rect.width;
		float num2 = iconImageInfo.m_iconNormal.rect.height;
		if (num2 > height)
		{
			float num3 = height / num2;
			num *= num3;
			num2 *= num3;
		}
		iconImageInfo.m_rectTrans.SetParent(m_rtContentContainer, worldPositionStays: false);
		iconImageInfo.m_rectTrans.anchoredPosition = Vector2.zero;
		iconImageInfo.m_rectTrans.localRotation = Quaternion.identity;
		iconImageInfo.m_rectTrans.localScale = Vector3.one;
		iconImageInfo.m_rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
		iconImageInfo.m_rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
		iconImageInfo.m_gameObject.SetActive(value: true);
		return iconImageInfo;
	}

	public void AlignContents(AlignType alignType)
	{
		m_isAlignReserved = true;
		m_AlignType = alignType;
	}

	private void AlignContents_Run(AlignType alignType, bool isIgnoreSame = true)
	{
		if (isIgnoreSame && m_AlignType == alignType)
		{
			return;
		}
		if (alignType >= AlignType.Left && (int)alignType < m_BackGoundImages.Length)
		{
			BackGoundImageInfo backGoundImageInfo = m_BackGoundImages[(int)alignType];
			m_BackGound0.sprite = backGoundImageInfo.m_Image0;
			m_BackGound1.sprite = backGoundImageInfo.m_Image1;
		}
		float num = 0f;
		int count = m_ContentInfos.Count;
		int num2 = 0;
		ContentInfo contentInfo = null;
		IconImageInfo iconImageInfo = null;
		if (alignType == AlignType.Left)
		{
			num = 0f;
		}
		else
		{
			float num3 = 0f;
			for (int i = 0; i < count; i++)
			{
				contentInfo = m_ContentInfos[i];
				if (contentInfo.m_isEnabled)
				{
					num3 += m_LeftMargin;
					num2 = contentInfo.m_iconImageInfos.Count;
					for (int j = 0; j < num2; j++)
					{
						iconImageInfo = contentInfo.m_iconImageInfos[j];
						num3 += iconImageInfo.m_rectTrans.rect.width;
					}
					num3 += m_ElementInteval;
					num3 += ((!(contentInfo.m_textGuideComp != null)) ? 0f : contentInfo.m_textGuideComp.preferredWidth);
					num3 += m_RightMargin;
				}
			}
			switch (alignType)
			{
			case AlignType.Center:
				num = (m_rtContentContainer.rect.width - num3) * 0.5f;
				break;
			case AlignType.Right:
				num = m_rtContentContainer.rect.width - num3;
				break;
			}
		}
		for (int k = 0; k < count; k++)
		{
			contentInfo = m_ContentInfos[k];
			if (contentInfo.m_isEnabled)
			{
				num += m_LeftMargin;
				num2 = contentInfo.m_iconImageInfos.Count;
				for (int l = 0; l < num2; l++)
				{
					iconImageInfo = contentInfo.m_iconImageInfos[l];
					iconImageInfo.m_rectTrans.anchoredPosition = new Vector2(num, 0f);
					num += iconImageInfo.m_rectTrans.rect.width;
				}
				num += m_ElementInteval;
				contentInfo.m_rtGuideText.anchoredPosition = new Vector2(num, 0f);
				num += ((!(contentInfo.m_textGuideComp != null)) ? 0f : contentInfo.m_textGuideComp.preferredWidth);
				num += m_RightMargin;
			}
		}
		m_AlignType = alignType;
	}

	public void SetContentEnable(string strGuideText, bool isEnable, bool isNeedAlign = true)
	{
		ContentInfo contentInfo = FindContentInfo_byGuideText(strGuideText);
		if (contentInfo != null)
		{
			contentInfo.SetEnable(isEnable);
			if (isNeedAlign)
			{
				m_isAlignReserved = true;
			}
		}
	}

	public void SetContentActivate(string strGuideText, bool isActivate, float fActiveTime = -9999f)
	{
		ContentInfo contentInfo = FindContentInfo_byGuideText(strGuideText);
		if (contentInfo != null)
		{
			if (fActiveTime == -9999f)
			{
				fActiveTime = m_fButtonPressEffTime;
			}
			contentInfo.SetPressed(isActivate);
			if (isActivate)
			{
				contentInfo.m_fRemainActiveTime = fActiveTime;
			}
		}
	}

	public void SetBGImageEnable(bool isEnable)
	{
		m_BackGound0.enabled = isEnable;
		m_BackGound1.enabled = isEnable;
	}

	public static IEnumerator LoadButtonImageBundle()
	{
		s_imageChachesBase.m_bundleName = "Image/ControllerButtons_1".ToLower();
		yield return MainLoadThing.instance.StartCoroutine(LoadButtonImageBundle(s_imageChachesBase));
		s_imageChachesXBox.m_bundleName = "Image/ControllerButtons_1_Xpad".ToLower();
		yield return MainLoadThing.instance.StartCoroutine(LoadButtonImageBundle(s_imageChachesXBox));
		s_imageChachesNS.m_bundleName = "Image/ControllerButtons_1_NSwitch".ToLower();
		yield return MainLoadThing.instance.StartCoroutine(LoadButtonImageBundle(s_imageChachesNS));
		s_imageChachesKeyboard.m_bundleName = "Image/KeyboardKeys_1".ToLower();
		yield return MainLoadThing.instance.StartCoroutine(LoadButtonImageBundle(s_imageChachesKeyboard));
	}

	public static IEnumerator LoadButtonImageBundle(ImageAssetCacheData imageCacheData)
	{
		if (imageCacheData.m_bundle != null)
		{
			yield break;
		}
		string assetBundleName = imageCacheData.m_bundleName;
		AssetBundleLoadAssetOperation operation = AssetBundleManager.LoadAssetAsync(assetBundleName, string.Empty, typeof(UnityEngine.Object));
		if (operation != null)
		{
			while (!operation.IsDone())
			{
				yield return null;
			}
			string errorMsg = string.Empty;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out errorMsg);
			if (loadedAssetBundle != null && !(loadedAssetBundle.m_AssetBundle == null))
			{
				imageCacheData.m_bundle = loadedAssetBundle.m_AssetBundle;
			}
		}
	}

	public static void UnloadButtonImageBundle()
	{
		UnloadButtonImageBundle(s_imageChachesBase);
		UnloadButtonImageBundle(s_imageChachesXBox);
		UnloadButtonImageBundle(s_imageChachesNS);
		UnloadButtonImageBundle(s_imageChachesKeyboard);
	}

	public static void UnloadButtonImageBundle(ImageAssetCacheData imageCacheData)
	{
		if (!(imageCacheData.m_bundle == null))
		{
			AssetBundleManager.UnloadAssetBundle(imageCacheData.m_bundleName);
			imageCacheData.m_bundle = null;
		}
	}

	public static IEnumerator LoadAllButtonImagesInBundle()
	{
		yield return MainLoadThing.instance.StartCoroutine(LoadButtonImageBundle());
		yield return MainLoadThing.instance.StartCoroutine(LoadAllButtonImagesInBundle(s_imageChachesBase));
		yield return MainLoadThing.instance.StartCoroutine(LoadAllButtonImagesInBundle(s_imageChachesXBox));
		yield return MainLoadThing.instance.StartCoroutine(LoadAllButtonImagesInBundle(s_imageChachesNS));
		yield return MainLoadThing.instance.StartCoroutine(LoadAllButtonImagesInBundle(s_imageChachesKeyboard));
	}

	public static IEnumerator LoadAllButtonImagesInBundle(ImageAssetCacheData imageCacheData)
	{
		ClearButtonImageCaches(imageCacheData);
		if (imageCacheData.m_bundle == null)
		{
			string error = string.Empty;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(imageCacheData.m_bundleName, out error);
			if (loadedAssetBundle == null || loadedAssetBundle.m_AssetBundle == null)
			{
				yield break;
			}
			imageCacheData.m_bundle = loadedAssetBundle.m_AssetBundle;
			if (imageCacheData.m_bundle == null)
			{
				yield break;
			}
		}
		AssetBundleRequest request = imageCacheData.m_bundle.LoadAllAssetsAsync();
		if (request == null)
		{
			yield break;
		}
		while (!request.isDone)
		{
			yield return null;
		}
		Sprite sprite = null;
		Texture2D texture2D = null;
		UnityEngine.Object[] allAssets = request.allAssets;
		foreach (UnityEngine.Object obj in allAssets)
		{
			if (obj is Sprite)
			{
				sprite = obj as Sprite;
			}
			else if (obj is Texture2D)
			{
				texture2D = obj as Texture2D;
				Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
				sprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
			}
			else
			{
				sprite = null;
			}
			if (!imageCacheData.m_imageCaches.ContainsKey(obj.name))
			{
				imageCacheData.m_imageCaches.Add(obj.name, sprite);
			}
		}
	}

	public static void ClearButtonImageCaches(ImageAssetCacheData imageCacheData)
	{
		SortedDictionary<string, Sprite>.Enumerator enumerator = imageCacheData.m_imageCaches.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Sprite> current = enumerator.Current;
			if (!(current.Value == null))
			{
				UnityEngine.Object.Destroy(current.Value);
			}
		}
		imageCacheData.m_imageCaches.Clear();
	}

	public static Sprite GetButtonImageInCache(string imageName)
	{
		ImageAssetCacheData imageAssetCacheData = null;
		imageAssetCacheData = PadInput.CurButtonIconType switch
		{
			PadInput.ButtonIconType.PS4 => s_imageChachesBase, 
			PadInput.ButtonIconType.XBox => s_imageChachesXBox, 
			PadInput.ButtonIconType.Switch => s_imageChachesNS, 
			PadInput.ButtonIconType.Keyboard => s_imageChachesKeyboard, 
			_ => s_imageChachesXBox, 
		};
		if (imageAssetCacheData == null)
		{
			return null;
		}
		Sprite value = null;
		return (!imageAssetCacheData.m_imageCaches.TryGetValue(imageName, out value)) ? null : value;
	}
}
