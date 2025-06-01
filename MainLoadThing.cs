using System;
using System.Collections;
using System.Text;
using AssetBundles;
using GameData;
using GameEvent;
using Steamworks;
using UnityEngine;

public class MainLoadThing : MonoBehaviour
{
	public GameObject m_goMainLoadThing;

	public AudioManager m_AudioManager;

	public SaveLoad m_SaveLoad;

	public GameObject m_PopupMenuRoot;

	public EffSkip m_EffSkip;

	public CommonButtonGuide m_commonButtonGuide;

	public SkipButtonGuide m_skipButtonGuide;

	public GameObject m_goTouchLock;

	public GameObject m_goEventSystem;

	public GameObject m_goPointCursor;

	public Animator m_animPointCursor;

	public RectTransform m_rtfPointCursor;

	private GameSwitch m_gameSwitch;

	private bool m_isLoadUniteFileDone;

	private bool m_isLoadOptFile;

	private bool m_isSaveOptFile;

	private bool m_isConvertVerUp;

	private bool m_isLoadAfterConvert;

	private bool m_isSaveLoadIng;

	private bool m_isUniteLoadExistErr;

	private const string cStrWaterMarkPath = "WaterMark.png";

	private const string c_faterProfileImageBundleName = "image/sns_profile";

	private ContentThumbnailManager m_faterProfileImageManager = new ContentThumbnailManager("image/sns_profile");

	private bool m_isCompleteFaterProfileImageLoad = true;

	private const string c_colImageThumbnailBundleName = "image/collect_image_thumbnail";

	private ContentThumbnailManager m_colImageThumbnailManager = new ContentThumbnailManager("image/collect_image_thumbnail");

	private bool m_isCompleteColImageThumbnailLoad = true;

	private const string c_KeywordIconImageBundleName = "image/keyword_icon";

	private ContentThumbnailManager m_KeywordIconImageManager = new ContentThumbnailManager("image/keyword_icon");

	private bool m_isCompleteKeywordIconImageLoad = true;

	private const string c_KeywordUIImageBundleName = "image/keyword_ui";

	private ContentThumbnailManager m_KeywordUIImageManager = new ContentThumbnailManager("image/keyword_ui");

	private bool m_isCompleteKeywordUIImageLoad = true;

	private const string c_ImageDetailViewBundleName = "prefabs/ingame/menu/UI_Document_Popup";

	private ImageDetailViewer m_ImageDetailViewer;

	[Header("------FOR PREFAB LOAD------")]
	public UnityEngine.Object m_prefabDocumentPopup;

	public UnityEngine.Object m_prefabTalkWindow;

	public UnityEngine.Object m_prefabBackLogMenu;

	public UnityEngine.Object m_prefabSNSContent_Normal;

	public UnityEngine.Object m_prefabSNSContent_Reply;

	public UnityEngine.Object m_prefabSNSContent_Shared;

	public UnityEngine.Object m_prefabMSGContent_Partner;

	public UnityEngine.Object m_prefabMSGContent_PartnerFirst;

	public UnityEngine.Object m_prefabMSGContent_Me;

	public UnityEngine.Object m_prefabMentalGageRenewal;

	public UnityEngine.Object m_prefabRankMenu;

	public UnityEngine.Object m_prefabAutoWrite;

	public UnityEngine.Object m_prefabChangeSceneScreen;

	public UnityEngine.Object[] m_prefabEFF_CHR_Introduce_rank = new UnityEngine.Object[5];

	public UnityEngine.Object m_prefabEFF_BroadCasting;

	public UnityEngine.Object m_prefabLoadingScreen;

	public UnityEngine.Object m_prefabLoadingSWatchIcon;

	public UnityEngine.Object m_prefabMSGMenu;

	public UnityEngine.Object m_prefabPopup_1Button;

	public UnityEngine.Object m_prefabPopup_2Button;

	public UnityEngine.Object m_prefabPopup_Action_1Button;

	public UnityEngine.Object m_prefabPopup_Action_2Button;

	public UnityEngine.Object m_prefabProfileMenu;

	public UnityEngine.Object m_prefabRecordMenu;

	public UnityEngine.Object m_prefabSavingScreen;

	public UnityEngine.Object m_prefabSNSMenu;

	public UnityEngine.Object m_prefabSWatchConfigMenu;

	public UnityEngine.Object m_prefabSystemSaveMenu;

	public UnityEngine.Object m_prefabSystemLoadMenu;

	public UnityEngine.Object m_prefabSystemSoundMenu;

	public UnityEngine.Object m_prefabSystemConfigMenu;

	public UnityEngine.Object m_prefabTutorialPopup;

	public UnityEngine.Object m_prefabMainMenuCommonBG;

	public UnityEngine.Object m_prefabModeMainMenu;

	public UnityEngine.Object m_prefabModeSystemMenu;

	public UnityEngine.Object m_prefabModeCollectionMain;

	public UnityEngine.Object m_prefabModeCollectionSound;

	public UnityEngine.Object m_prefabModeCollectionImage;

	public UnityEngine.Object m_prefabModeCollectionKeyword;

	public UnityEngine.Object m_prefabModeCollectionProfile;

	public UnityEngine.Object m_prefabModeCollectionTrophy;

	public UnityEngine.Object m_prefabKeywordGetPopup;

	public UnityEngine.Object m_prefabProfilePopup;

	public UnityEngine.Object m_prefabEventMarkerDetect;

	public UnityEngine.Object m_prefabMarkerDetect_Name;

	public UnityEngine.Object m_prefabEventMarkerTalk;

	public UnityEngine.Object m_prefabCharHan;

	public UnityEngine.Object m_prefabCharMin;

	public UnityEngine.Object m_prefabCharSeo;

	public UnityEngine.Object m_prefabCharOh;

	public UnityEngine.Object m_prefabCharLee;

	public UnityEngine.Object m_prefabCharChang;

	public UnityEngine.Object m_prefabCharShin;

	public UnityEngine.Object m_prefabCharHa;

	public UnityEngine.Object m_prefabCharNam;

	public Texture2D m_textureCursor;

	public Texture2D m_textureCursorEmpty;

	private static MainLoadThing s_Instance;

	private bool m_SteamAPI_Initialized;

	private SteamAPIWarningMessageHook_t SteamAPIWarningMessageHook;

	private Callback<UserStatsReceived_t> SteamCallback_UserStateReceived;

	public CommonButtonGuide commonButtonGuide => m_commonButtonGuide;

	public SkipButtonGuide skipButtonGuide => m_skipButtonGuide;

	public ContentThumbnailManager faterProfileImageManager => m_faterProfileImageManager;

	public bool isCompleteFaterProfileImageLoad => m_isCompleteFaterProfileImageLoad;

	public ContentThumbnailManager colImageThumbnailManager => m_colImageThumbnailManager;

	public bool isCompleteColImageThumbnailLoad => m_isCompleteColImageThumbnailLoad;

	public ContentThumbnailManager keywordIconImageManager => m_KeywordIconImageManager;

	public bool isCompleteKeywordIconImageLoad => m_isCompleteKeywordIconImageLoad;

	public ContentThumbnailManager keywordUIImageManager => m_KeywordUIImageManager;

	public bool isCompleteKeywordUIImageLoad => m_isCompleteKeywordUIImageLoad;

	public ImageDetailViewer imageDetailViewer => m_ImageDetailViewer;

	public static MainLoadThing instance => s_Instance;

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
	}

	private static void OnSteamCallback_UserStateRecived(UserStatsReceived_t userStatsReceived_T)
	{
	}

	private void Awake()
	{
		SetOffCursor();
		s_Instance = this;
		QualitySettings.antiAliasing = 2;
		if (!Packsize.Test())
		{
			throw new Exception("Packsize is wrong! You are likely using a Linux/OSX build on Windows or vice versa.");
		}
		if (!DllCheck.Test())
		{
			throw new Exception("DllCheck returned false.");
		}
		AppId_t unOwnAppID = new AppId_t(1025960u);
		if (SteamAPI.RestartAppIfNecessary(unOwnAppID))
		{
			Application.Quit();
			return;
		}
		try
		{
			m_SteamAPI_Initialized = SteamAPI.Init();
		}
		catch (DllNotFoundException)
		{
			Application.Quit();
			return;
		}
		if (m_SteamAPI_Initialized)
		{
			SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(SteamAPIWarningMessageHook);
			SteamCallback_UserStateReceived = Callback<UserStatsReceived_t>.Create(OnSteamCallback_UserStateRecived);
			SteamUserStats.RequestCurrentStats();
		}
	}

	private IEnumerator Start()
	{
		AssetBundleManager.SetSourceAssetBundleDirectory("/AssetBundles/" + Utility.GetPlatformName() + "/");
		AssetBundleLoadOperation operation = AssetBundleManager.Initialize();
		if (operation != null)
		{
			yield return StartCoroutine(operation);
		}
		yield return StartCoroutine(LoadingScreen.LoadAssetObject());
		yield return null;
		LoadingScreen.Show();
		yield return StartCoroutine(SavingScreen.LoadAssetObject());
		yield return StartCoroutine(LoadingSWatchIcon.LoadAssetObject());
		if (!XlsDataHandler.isInitailized)
		{
			yield return StartCoroutine(XlsDataHandler.Init("xls", isLoadAllData: true, isForceLoad: false, this));
			yield return null;
		}
		FontManager.Init();
		TagText.BuildGlobalDefinedTag();
		if (m_AudioManager != null)
		{
			m_AudioManager.gameObject.SetActive(value: true);
			m_AudioManager.Init();
		}
		yield return StartCoroutine(CommonButtonGuide.LoadAllButtonImagesInBundle());
		UnityEngine.Object.DontDestroyOnLoad(m_goMainLoadThing);
		GamePadInput.Init();
		ButtonPadInput.Init();
		if (m_commonButtonGuide != null)
		{
			m_commonButtonGuide.Init();
		}
		if (m_skipButtonGuide != null)
		{
			m_skipButtonGuide.Init();
		}
		m_gameSwitch = GameSwitch.GetInstance();
		m_gameSwitch.InitNonSaveData();
		m_gameSwitch.SetMainLoadThing(this);
		m_gameSwitch.InitOption();
		yield return StartCoroutine(XlsDataHandler.SetCurrentLanguage(m_gameSwitch.GetCurSubtitleLanguage()));
		PopupDialoguePlus.InitPopupDialogues(m_PopupMenuRoot);
		m_isCompleteFaterProfileImageLoad = false;
		StartCoroutine(m_faterProfileImageManager.LoadAssetsAll(this, OnComplete_AssetBundleLoaded));
		m_isCompleteColImageThumbnailLoad = false;
		StartCoroutine(m_colImageThumbnailManager.LoadAssetsAll(this, OnComplete_AssetBundleLoaded));
		m_isCompleteKeywordIconImageLoad = false;
		StartCoroutine(m_KeywordIconImageManager.LoadAssetsAll(this, OnComplete_AssetBundleLoaded));
		m_isCompleteKeywordIconImageLoad = false;
		StartCoroutine(m_KeywordUIImageManager.LoadAssetsAll(this, OnComplete_AssetBundleLoaded));
		yield return StartCoroutine(EventCameraEffect.Instance.PreLoadPrefabAssets());
		m_ImageDetailViewer = (UnityEngine.Object.Instantiate(m_prefabDocumentPopup) as GameObject).GetComponent<ImageDetailViewer>();
		if (m_ImageDetailViewer != null)
		{
			m_ImageDetailViewer.gameObject.GetComponent<Transform>().SetParent(m_goMainLoadThing.GetComponent<Transform>(), worldPositionStays: false);
		}
		m_SaveLoad = SaveLoad.GetInstance();
		m_SaveLoad.SetMainLoadThing(this);
		m_isSaveLoadIng = true;
		m_isUniteLoadExistErr = false;
		m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eLoadEntireBuf, 0, OnPSSaveLoadDone);
		yield return null;
	}

	private void OnPSSaveLoadDone(bool isExistErr)
	{
		m_isLoadUniteFileDone = true;
		m_isSaveLoadIng = false;
		m_isUniteLoadExistErr = isExistErr;
	}

	private void OnCompConvertVerUpSaveFile(bool isExistErr)
	{
		m_isSaveLoadIng = false;
		m_isLoadAfterConvert = true;
	}

	private void OnCompConvertVerUpSaveFile2(bool isExistErr)
	{
		m_isSaveLoadIng = false;
	}

	private void OnSaveOptFile(bool isExistErr)
	{
		m_isSaveOptFile = true;
		m_isSaveLoadIng = false;
	}

	private void OnGameDataLoadDone(bool isExistErr)
	{
		m_isLoadOptFile = true;
		m_isSaveLoadIng = false;
		StartCoroutine(XlsDataHandler.SetCurrentLanguage(m_gameSwitch.GetCurSubtitleLanguage(), clearPrevLanguageText: false, OnFinished_LoadLanguagePack));
	}

	private void OnSonyNpInitailized(object sender, object arg)
	{
		m_isLoadOptFile = true;
		m_isSaveLoadIng = false;
		StartCoroutine(XlsDataHandler.SetCurrentLanguage(m_gameSwitch.GetCurSubtitleLanguage(), clearPrevLanguageText: false, OnFinished_LoadLanguagePack));
	}

	private void OnFinished_LoadLanguagePack(object sender, object args)
	{
		StartCoroutine(LogoScene.LoadScene());
	}

	private void OnComplete_AssetBundleLoaded(object sender, object arg)
	{
		if (object.ReferenceEquals(sender, m_faterProfileImageManager))
		{
			m_isCompleteFaterProfileImageLoad = true;
		}
		else if (object.ReferenceEquals(sender, m_KeywordIconImageManager))
		{
			m_isCompleteKeywordIconImageLoad = true;
		}
		else if (object.ReferenceEquals(sender, m_colImageThumbnailManager))
		{
			m_isCompleteColImageThumbnailLoad = true;
		}
		else if (object.ReferenceEquals(sender, m_KeywordUIImageManager))
		{
			m_isCompleteKeywordUIImageLoad = true;
		}
	}

	private void Update()
	{
		if (m_SteamAPI_Initialized)
		{
			SteamAPI.RunCallbacks();
		}
		GamePadInput.Update();
		ButtonPadInput.Update();
		SaveLoad.Update();
		if (m_isSaveLoadIng || m_SaveLoad == null)
		{
			return;
		}
		if (m_isLoadUniteFileDone && !m_isSaveOptFile)
		{
			if (!m_isConvertVerUp)
			{
				m_isConvertVerUp = true;
				if (m_SaveLoad.GetBefFileLoad())
				{
					m_isSaveLoadIng = true;
					m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eConvertForVersionUp, 0, OnCompConvertVerUpSaveFile);
					return;
				}
			}
			if (m_isLoadAfterConvert)
			{
				m_isLoadAfterConvert = false;
				m_isSaveLoadIng = true;
				m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eLoadAllOptConfigInfoColl, 0, OnCompConvertVerUpSaveFile2);
				return;
			}
			if (m_isUniteLoadExistErr)
			{
				m_isSaveOptFile = true;
				m_isLoadOptFile = true;
				OnGameDataLoadDone(isExistErr: false);
			}
			else
			{
				uint iReturnValue = 0u;
				if (!m_SaveLoad.ExistEventSaveFile(SaveLoad.eSaveType.OPT, ref iReturnValue))
				{
					m_isSaveLoadIng = true;
					m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eSaveOptConfig, 0, OnSaveOptFile);
				}
				else
				{
					m_isSaveOptFile = true;
				}
			}
		}
		else if (m_isLoadUniteFileDone && m_isSaveOptFile && !m_isLoadOptFile)
		{
			m_isSaveLoadIng = true;
			m_SaveLoad.SaveLoadWhat(SaveLoad.eSaveWhat.eLoadOptConfigInfoColl, 0, OnGameDataLoadDone);
		}
		if (m_goPointCursor.activeInHierarchy)
		{
			m_rtfPointCursor.transform.position = Input.mousePosition;
			if (Input.GetMouseButtonUp(0))
			{
				GameGlobalUtil.PlayUIAnimation(m_animPointCursor, "up");
			}
			else if (Input.GetMouseButtonDown(0))
			{
				GameGlobalUtil.PlayUIAnimation(m_animPointCursor, "down");
			}
		}
	}

	private void OnDestroy()
	{
		EventCameraEffect.Instance.ClearSrcGameObjects();
		SaveLoad.ReleaseInstance();
		GameSwitch.ReleaseInstance();
		if (m_ImageDetailViewer != null)
		{
			UnityEngine.Object.Destroy(m_ImageDetailViewer.gameObject);
		}
		UnityEngine.Object.Destroy(m_goMainLoadThing);
		EventEngine.ReleaseInstance();
		EventCameraEffect.ReleaseInstance();
		RenderManager.ReleaseInstance();
		FontManager.Clear();
		XlsDataHandler.Release();
		m_AudioManager = null;
		m_SaveLoad = null;
		m_gameSwitch = null;
		s_Instance = null;
		if (m_SteamAPI_Initialized)
		{
			SteamAPI.Shutdown();
		}
	}

	public void SetTouchLock(bool isLock)
	{
	}

	public void SetEventSystemSelectNull()
	{
	}

	public bool IsTouchableState()
	{
		bool flag = true;
		return m_gameSwitch.GetUIButType() == GameSwitch.eUIButType.KEYMOUSE;
	}

	public void SetCursorPoint(bool isShow)
	{
		if (isShow)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.SetCursor(m_textureCursor, Vector2.zero, CursorMode.ForceSoftware);
		}
		else
		{
			Cursor.SetCursor(m_textureCursorEmpty, Vector2.zero, CursorMode.ForceSoftware);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		SetOffCursor(pauseStatus);
	}

	private void SetOffCursor(bool isOn = false)
	{
		Cursor.SetCursor(m_textureCursor, Vector2.zero, CursorMode.ForceSoftware);
	}
}
