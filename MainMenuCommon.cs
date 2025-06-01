using System;
using System.Collections;
using AssetBundles;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCommon : MonoBehaviour
{
	public enum Mode
	{
		None,
		MainMenu,
		SystemMenu,
		QuickSave,
		CollectionMain,
		CollectionSound,
		CollectionImage,
		CollectionKeyword,
		CollectionProfile,
		CollectionTrophy
	}

	public enum BGType
	{
		ImageBG,
		NoImageBG,
		ScreenShotBG
	}

	private class ModeInfo
	{
		public string m_assetBundleName;

		public AssetBundleObjectHandler m_assetBundleHdr;

		public GameObject m_InstanceObject;
	}

	private enum State
	{
		Idle,
		WaitingForInitialized,
		DisappearToNextMode,
		DisappearToExit
	}

	public enum ExitMode
	{
		None,
		NewGame,
		RestartGame,
		LoadGame,
		BackToTitle,
		QuitGame
	}

	public Canvas m_RootCanvas;

	public Transform m_RootChildTr;

	public GameObject m_ImageBGRoot;

	public GameObject m_NoImageBGRoot;

	public GameObject m_BGStageRoot;

	public GameObject m_ScreenShotBG;

	public GameObject m_EventPrefabObj;

	[Header("Controlable Animators")]
	public Animator[] m_Animators;

	[Header("Mode To Prefabs")]
	public UnityEngine.Object m_MainMenu;

	public UnityEngine.Object m_SystemMenu;

	public UnityEngine.Object m_CollectionMain;

	public UnityEngine.Object m_CollectionSound;

	public UnityEngine.Object m_CollectionImage;

	public UnityEngine.Object m_CollectionKeyword;

	public UnityEngine.Object m_CollectionProfile;

	public UnityEngine.Object m_CollectionTrophy;

	private ModeInfo[] m_ModeInfos;

	private ExitMode m_exitMode;

	private object m_exitArg;

	private AudioManager m_AudioManager;

	public const string c_SceneGameMainName = "Scene/GameMain";

	private static MainMenuCommon s_Instance;

	private static bool s_isCreating;

	public const string c_SceneName = "Scene/UI_PS/0037_ui_MainMenu_Collection_BG";

	public const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_MainMenuCommonBG";

	public const string c_RootObjectName = "UI_MainMenuCommonBG";

	private static GameObject s_SrcObject;

	private static bool s_isCreatedbyLoadScene;

	private static Mode s_curMode;

	private static Mode s_nextMode;

	private static object s_nextModeSubArg;

	private static State s_curState;

	private static bool s_isTransrationNextMode;

	private static BGType s_curBGType;

	private static bool s_isNeedSetBGType;

	private static GameDefine.EventProc s_fpClosedComplete;

	public static MainMenuCommon instance => s_Instance;

	public static bool isCreating => s_isCreating;

	public static GameDefine.EventProc eventCloseComplete
	{
		set
		{
			s_fpClosedComplete = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void Awake()
	{
		if (!(s_Instance != null) || s_Instance != this)
		{
		}
		s_Instance = this;
		s_isCreating = false;
		int length = Enum.GetValues(typeof(Mode)).Length;
		m_ModeInfos = new ModeInfo[length];
		int num = length;
		while (num > 0)
		{
			num--;
			m_ModeInfos[num] = new ModeInfo();
		}
		m_ModeInfos[1].m_assetBundleName = "prefabs/ingame/menu/ui_mainmenu";
		m_ModeInfos[2].m_assetBundleName = "prefabs/ingame/menu/ui_systemmenu";
		m_ModeInfos[4].m_assetBundleName = "prefabs/ingame/menu/ui_collectionmain";
		m_ModeInfos[5].m_assetBundleName = "prefabs/ingame/menu/ui_collectionsound";
		m_ModeInfos[6].m_assetBundleName = "prefabs/ingame/menu/ui_collectionimage";
		m_ModeInfos[7].m_assetBundleName = "prefabs/ingame/menu/ui_collectionkeyword";
		m_ModeInfos[8].m_assetBundleName = "prefabs/ingame/menu/ui_collectionprofile";
		m_ModeInfos[9].m_assetBundleName = "prefabs/ingame/menu/ui_collectiontrophy";
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_ScreenShotBG != null)
		{
			m_ScreenShotBG.SetActive(value: false);
		}
		if (m_EventPrefabObj != null)
		{
			m_EventPrefabObj.SetActive(value: false);
		}
		if (s_isCreatedbyLoadScene)
		{
			LoadingScreen.Close();
		}
	}

	private void OnDestroy()
	{
		if (m_ModeInfos != null)
		{
			ModeInfo[] modeInfos = m_ModeInfos;
			foreach (ModeInfo modeInfo in modeInfos)
			{
				if (modeInfo.m_InstanceObject != null)
				{
					UnityEngine.Object.Destroy(modeInfo.m_InstanceObject);
					modeInfo.m_InstanceObject = null;
				}
				if (modeInfo.m_InstanceObject != null)
				{
					UnityEngine.Object.Destroy(modeInfo.m_InstanceObject);
					modeInfo.m_InstanceObject = null;
				}
			}
		}
		s_SrcObject = null;
		m_exitArg = null;
		s_fpClosedComplete = null;
		m_AudioManager = null;
		s_Instance = null;
		s_isCreating = false;
		s_isTransrationNextMode = false;
	}

	private void Update()
	{
		switch (s_curState)
		{
		case State.Idle:
			break;
		case State.WaitingForInitialized:
			if (s_isNeedSetBGType)
			{
				SetBGType(s_curBGType);
			}
			StartCoroutine(ShowChildMenu(s_nextMode, s_nextModeSubArg));
			break;
		case State.DisappearToNextMode:
			break;
		case State.DisappearToExit:
		{
			bool flag = true;
			if (m_Animators != null && m_Animators.Length > 0)
			{
				string text = GameDefine.UIAnimationState.disappear.ToString();
				Animator[] animators = m_Animators;
				foreach (Animator animator in animators)
				{
					if (!(animator == null) && animator.gameObject.activeInHierarchy)
					{
						AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
						if (!currentAnimatorStateInfo.IsName(text) || !(currentAnimatorStateInfo.normalizedTime >= 0.99f))
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag)
			{
				ExitProcComplete();
			}
			break;
		}
		}
	}

	private GameObject GetInstantiateModeObject(int iModeIdx)
	{
		UnityEngine.Object obj = null;
		switch (iModeIdx)
		{
		case 1:
			obj = MainLoadThing.instance.m_prefabModeMainMenu;
			break;
		case 2:
			obj = MainLoadThing.instance.m_prefabModeSystemMenu;
			break;
		case 4:
			obj = MainLoadThing.instance.m_prefabModeCollectionMain;
			break;
		case 5:
			obj = MainLoadThing.instance.m_prefabModeCollectionSound;
			break;
		case 6:
			obj = MainLoadThing.instance.m_prefabModeCollectionImage;
			break;
		case 7:
			obj = MainLoadThing.instance.m_prefabModeCollectionKeyword;
			break;
		case 8:
			obj = MainLoadThing.instance.m_prefabModeCollectionProfile;
			break;
		case 9:
			obj = MainLoadThing.instance.m_prefabModeCollectionTrophy;
			break;
		}
		GameObject result = null;
		if (obj != null)
		{
			result = UnityEngine.Object.Instantiate(obj) as GameObject;
		}
		return result;
	}

	private IEnumerator ShowChildMenu(Mode mode, object subArg = null)
	{
		if (s_isTransrationNextMode)
		{
			yield break;
		}
		s_isTransrationNextMode = true;
		int modeIdx = (int)((mode == Mode.QuickSave) ? Mode.SystemMenu : mode);
		ModeInfo modeInfo = m_ModeInfos[modeIdx];
		if (modeInfo.m_InstanceObject == null && !string.IsNullOrEmpty(modeInfo.m_assetBundleName))
		{
			modeInfo.m_InstanceObject = GetInstantiateModeObject(modeIdx);
		}
		GameObject instanceObject = modeInfo.m_InstanceObject;
		instanceObject.SetActive(value: true);
		Transform trChild = modeInfo.m_InstanceObject.GetComponent<Transform>();
		trChild.SetParent(m_RootChildTr);
		Canvas canvasChild = modeInfo.m_InstanceObject.GetComponentInChildren<Canvas>();
		canvasChild.sortingOrder = m_RootCanvas.sortingOrder + 1;
		switch (mode)
		{
		case Mode.MainMenu:
		{
			MainMenu mainMenu = instanceObject.GetComponent<MainMenu>();
			mainMenu.eventCloseComplete = EventProc_ExitMainMenu;
			mainMenu.eventNoticeExit = EventProc_NoticeExitMainMenu;
			if (subArg != null)
			{
				mainMenu.Show((int)subArg, isPlayBGM: false);
			}
			else
			{
				mainMenu.Show();
			}
			break;
		}
		case Mode.SystemMenu:
		{
			SystemMenuPlus component = instanceObject.GetComponent<SystemMenuPlus>();
			component.eventCloseComplete = EventProc_ExitSystemMenu;
			component.eventNoticeExit = EventProc_NoticeExitSystemMenu;
			component.Show((subArg != null) ? ((SystemMenuPlus.Mode)subArg) : SystemMenuPlus.Mode.Save);
			break;
		}
		case Mode.QuickSave:
		{
			SystemMenuPlus systemMenu = instanceObject.GetComponent<SystemMenuPlus>();
			systemMenu.eventCloseComplete = EventProc_ExitSystemMenu;
			systemMenu.eventNoticeExit = EventProc_NoticeExitSystemMenu;
			yield return StartCoroutine(systemMenu.ShowQuickSaveMode());
			break;
		}
		case Mode.CollectionMain:
		{
			CollectionMainMenu colMainMenu = instanceObject.GetComponent<CollectionMainMenu>();
			colMainMenu.isNowMainMenu = s_isCreatedbyLoadScene;
			colMainMenu.eventCloseComplete = EventProc_ExitCollectionMain;
			colMainMenu.eventNoticeExit = EventProc_NoticeExit;
			colMainMenu.Show((subArg != null) ? ((CollectionMainMenu.Buttons)subArg) : CollectionMainMenu.Buttons.Sound, subArg == null);
			break;
		}
		case Mode.CollectionSound:
		{
			CollectionSoundMenu colSoundMenu = instanceObject.GetComponent<CollectionSoundMenu>();
			colSoundMenu.eventCloseComplete = EventProc_ExitCollectionMenu;
			colSoundMenu.eventNoticeExit = EventProc_NoticeExit;
			colSoundMenu.isMainMenuScene = subArg != null && (bool)subArg;
			colSoundMenu.Show();
			break;
		}
		case Mode.CollectionImage:
		{
			CollectionImageMenu colImageMenu = instanceObject.GetComponent<CollectionImageMenu>();
			colImageMenu.eventCloseComplete = EventProc_ExitCollectionMenu;
			colImageMenu.eventNoticeExit = EventProc_NoticeExit;
			colImageMenu.Show();
			break;
		}
		case Mode.CollectionKeyword:
		{
			CollectionKeywordMenu colKeywordMenu = instanceObject.GetComponent<CollectionKeywordMenu>();
			colKeywordMenu.eventCloseComplete = EventProc_ExitCollectionMenu;
			colKeywordMenu.eventNoticeExit = EventProc_NoticeExit;
			MainLoadThing.instance.StartCoroutine(colKeywordMenu.Show());
			break;
		}
		case Mode.CollectionProfile:
		{
			CollectionProfileMenu colProfileMenu = instanceObject.GetComponent<CollectionProfileMenu>();
			colProfileMenu.eventCloseComplete = EventProc_ExitCollectionMenu;
			colProfileMenu.eventNoticeExit = EventProc_NoticeExit;
			colProfileMenu.Show();
			break;
		}
		case Mode.CollectionTrophy:
		{
			CollectionTrophyMenu colTrophyMenu = instanceObject.GetComponent<CollectionTrophyMenu>();
			colTrophyMenu.eventCloseComplete = EventProc_ExitCollectionMenu;
			colTrophyMenu.eventNoticeExit = EventProc_NoticeExit;
			colTrophyMenu.Show();
			break;
		}
		}
		s_curState = State.Idle;
		s_curMode = mode;
		s_isTransrationNextMode = false;
		if (!m_RootCanvas.gameObject.activeSelf)
		{
			m_RootCanvas.gameObject.SetActive(value: true);
		}
		if (s_Instance.m_BGStageRoot != null)
		{
			s_Instance.m_BGStageRoot.SetActive(s_curBGType == BGType.ImageBG);
		}
	}

	private void ExitProcStart()
	{
		s_curState = State.DisappearToExit;
		if (m_Animators != null && m_Animators.Length > 0)
		{
			string stateName = GameDefine.UIAnimationState.disappear.ToString();
			Animator[] animators = m_Animators;
			foreach (Animator animator in animators)
			{
				if (!(animator == null) && animator.gameObject.activeInHierarchy)
				{
					animator.Play(stateName);
				}
			}
		}
		else
		{
			ExitProcComplete();
		}
	}

	private void ExitProcComplete()
	{
		s_curState = State.Idle;
		Mode mode = s_curMode;
		if (mode == Mode.SystemMenu || mode == Mode.QuickSave)
		{
			GameObject instanceObject = m_ModeInfos[2].m_InstanceObject;
			if (instanceObject != null)
			{
				SystemMenuPlus component = instanceObject.GetComponent<SystemMenuPlus>();
				if (component != null)
				{
					component.ClosedComplete();
				}
			}
		}
		switch (m_exitMode)
		{
		case ExitMode.None:
			base.gameObject.SetActive(value: false);
			if (s_fpClosedComplete != null)
			{
				s_fpClosedComplete(this, null);
				s_fpClosedComplete = null;
			}
			break;
		case ExitMode.NewGame:
			LoadingScreen.Show();
			GameSwitch.GetInstance().InitGameVal(ConstGameSwitch.eSTARTTYPE.NEW, 0, OnGameDataLoadDone);
			break;
		case ExitMode.RestartGame:
			LoadingScreen.Show();
			GameSwitch.GetInstance().InitGameVal(ConstGameSwitch.eSTARTTYPE.RESTART, 0, OnGameDataLoadDone);
			break;
		case ExitMode.LoadGame:
			LoadingScreen.Show();
			StartLoadGame((int)m_exitArg);
			break;
		case ExitMode.BackToTitle:
			break;
		case ExitMode.QuitGame:
			Application.Quit();
			break;
		}
	}

	private void StartLoadGame(int slotIdx)
	{
		if (s_Instance.m_AudioManager != null)
		{
			s_Instance.m_AudioManager.Stop(0);
		}
		GameMain gameMain = GameMain.instance;
		if (gameMain != null)
		{
			gameMain.ReLoadGameScene(slotIdx, isEvent: false, OnGameDataLoadDone_Reload);
			return;
		}
		EventEngine eventEngine = EventEngine.GetInstance(isCreate: false);
		if (eventEngine != null && eventEngine.IsEventRunning())
		{
			eventEngine.StopEvent();
		}
		LoadingScreen.Show();
		EventEngine.GetInstance().OnlyFreeResForGoToMain();
		GameSwitch gameSwitch = GameSwitch.GetInstance();
		gameSwitch.FreeGoToMain();
		gameSwitch.InitGameVal(ConstGameSwitch.eSTARTTYPE.CONTINUE, slotIdx, OnGameDataLoadDone, isSaveLoadCBShowClose: false);
	}

	private static void OnGameDataLoadDone_Reload()
	{
		if (!(s_Instance == null))
		{
			s_Instance.gameObject.SetActive(value: false);
		}
	}

	private static void OnGameDataLoadDone(bool isExistErr)
	{
		MainLoadThing.instance.StartCoroutine(LoadGameMain());
	}

	private static IEnumerator LoadGameMain()
	{
		EventEngine.m_strLoadedLevel = null;
		LoadingScreen.Show();
		LoadingScreen.ActiveLoadingPercent(isOn: true);
		AsyncOperation loadAsync = SceneManager.LoadSceneAsync("Scene/GameMain");
		while (!loadAsync.isDone || loadAsync.progress < 1f)
		{
			yield return null;
		}
		yield return null;
	}

	public static AsyncOperation LoadScene(bool isAsync = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		if (s_Instance != null || s_isCreatedbyLoadScene)
		{
		}
		s_isCreating = true;
		s_isCreatedbyLoadScene = true;
		AsyncOperation result = null;
		if (isAsync)
		{
			result = SceneManager.LoadSceneAsync("Scene/UI_PS/0037_ui_MainMenu_Collection_BG", loadSceneMode);
		}
		else
		{
			SceneManager.LoadScene("Scene/UI_PS/0037_ui_MainMenu_Collection_BG", loadSceneMode);
		}
		return result;
	}

	public static void UnloadScene()
	{
	}

	public static IEnumerator CreateWithPrefab()
	{
		if (!(s_Instance != null))
		{
			s_isCreatedbyLoadScene = false;
			if (s_SrcObject == null)
			{
				s_SrcObject = MainLoadThing.instance.m_prefabMainMenuCommonBG as GameObject;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(s_SrcObject);
			s_Instance = gameObject.GetComponent<MainMenuCommon>();
		}
		yield break;
	}

	public static void DestoryInstance(bool isImmidiate = false)
	{
		if (!(s_Instance == null) && !s_isCreatedbyLoadScene)
		{
			if (isImmidiate)
			{
				UnityEngine.Object.DestroyImmediate(s_Instance.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(s_Instance.gameObject);
			}
			AssetBundleManager.UnloadAssetBundle("Prefabs/InGame/Menu/UI_MainMenuCommonBG");
			Resources.UnloadUnusedAssets();
		}
	}

	public static IEnumerator PreloadToInGameMain()
	{
		yield return MainLoadThing.instance.StartCoroutine(CreateWithPrefab());
		if (instance != null)
		{
			instance.gameObject.SetActive(value: false);
		}
	}

	public static IEnumerator ChangeMode(Mode newMode, object subArg = null)
	{
		if (s_Instance != null)
		{
			s_Instance.gameObject.SetActive(value: true);
			yield return s_Instance.StartCoroutine(s_Instance.ShowChildMenu(newMode, subArg));
		}
		else if (s_isCreating)
		{
			s_curState = State.WaitingForInitialized;
			s_nextMode = newMode;
			s_nextModeSubArg = subArg;
		}
	}

	public static void SetBGType(BGType bgType)
	{
		if (s_Instance == null)
		{
			s_curBGType = bgType;
			s_isNeedSetBGType = true;
			s_curState = State.WaitingForInitialized;
			return;
		}
		if (s_Instance.m_BGStageRoot != null)
		{
			s_Instance.m_BGStageRoot.SetActive(value: false);
		}
		s_curBGType = bgType;
		switch (s_curBGType)
		{
		case BGType.ImageBG:
			s_Instance.m_ImageBGRoot.SetActive(value: true);
			s_Instance.m_NoImageBGRoot.SetActive(value: false);
			s_Instance.m_ScreenShotBG.SetActive(value: false);
			break;
		case BGType.NoImageBG:
			s_Instance.m_ImageBGRoot.SetActive(value: false);
			s_Instance.m_NoImageBGRoot.SetActive(value: true);
			s_Instance.m_ScreenShotBG.SetActive(value: false);
			break;
		case BGType.ScreenShotBG:
			s_Instance.m_ImageBGRoot.SetActive(value: false);
			s_Instance.m_NoImageBGRoot.SetActive(value: true);
			s_Instance.m_ScreenShotBG.SetActive(value: true);
			break;
		}
		s_isNeedSetBGType = false;
	}

	public static void Close()
	{
		if (!(s_Instance == null))
		{
			s_Instance.ExitProcStart();
		}
	}

	public static Mode GetCurMode()
	{
		if (s_Instance == null || !s_Instance.gameObject.activeSelf)
		{
			return Mode.None;
		}
		return s_curMode;
	}

	private static void EventProc_NoticeExitMainMenu(object sender, object arg)
	{
		if (s_Instance == null || sender == null)
		{
			return;
		}
		MainMenu mainMenu = sender as MainMenu;
		if (arg != null && arg is ExitMode && (ExitMode)arg != ExitMode.None)
		{
			s_Instance.m_exitMode = (ExitMode)arg;
			s_Instance.m_exitArg = mainMenu.loadableSaveSlotIdx;
			s_Instance.ExitProcStart();
			if (s_Instance.m_AudioManager != null)
			{
				s_Instance.m_AudioManager.Stop(0);
			}
		}
	}

	private static void EventProc_ExitMainMenu(object sender, object arg)
	{
		if (!(s_Instance == null) && s_curState != State.DisappearToExit && (Mode)arg == Mode.CollectionMain)
		{
			CollectionMainMenu.prevMenuType = CollectionMainMenu.PrevMenuType.MainMenu;
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionMain));
		}
	}

	private static void EventProc_NoticeExitSystemMenu(object sender, object arg)
	{
		if (arg != null && (SystemMenuPlus.ExitType)arg == SystemMenuPlus.ExitType.InGame)
		{
			s_curState = State.DisappearToExit;
			s_Instance.ExitProcStart();
		}
	}

	private static void EventProc_ExitSystemMenu(object sender, object arg)
	{
		if (s_Instance == null || s_curState == State.DisappearToExit)
		{
			return;
		}
		switch ((SystemMenuPlus.ExitType)arg)
		{
		case SystemMenuPlus.ExitType.Collection:
			CollectionMainMenu.prevMenuType = CollectionMainMenu.PrevMenuType.SystemMenu;
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionMain));
			break;
		case SystemMenuPlus.ExitType.MainMenu:
		{
			GameMain gameMain2 = GameMain.instance;
			if (gameMain2 != null)
			{
				if (s_Instance.m_AudioManager != null)
				{
					s_Instance.m_AudioManager.SetVol(0, 0f, 0.9f);
				}
				gameMain2.StartFullCover_FadeOut(1f, EventProc_ExitToMainMenu);
			}
			else
			{
				if (s_Instance.m_AudioManager != null)
				{
					s_Instance.m_AudioManager.SetVol(0, 0f, 0f);
				}
				EventProc_ExitToMainMenu(null, null);
			}
			break;
		}
		case SystemMenuPlus.ExitType.LoadGame:
		{
			s_Instance.m_exitArg = (sender as SystemMenuPlus).loadableSlotIdx;
			GameMain gameMain = GameMain.instance;
			if (gameMain != null)
			{
				if (s_Instance.m_AudioManager != null)
				{
					s_Instance.m_AudioManager.SetVol(0, 0f, 0.9f);
				}
				gameMain.StartFullCover_FadeOut(1f, EventProc_LoadGame);
			}
			else
			{
				if (s_Instance.m_AudioManager != null)
				{
					s_Instance.m_AudioManager.SetVol(0, 0f, 0f);
				}
				EventProc_LoadGame(null, null);
			}
			break;
		}
		}
	}

	private static void EventProc_ExitToMainMenu(object sender, object arg)
	{
		DestoryInstance(isImmidiate: true);
		EventEngine.GetInstance(isCreate: false)?.GoMainMenu();
	}

	private static void EventProc_LoadGame(object sender, object arg)
	{
		s_Instance.StartLoadGame((int)s_Instance.m_exitArg);
	}

	private static void EventProc_ExitCollectionMain(object sender, object arg)
	{
		if (s_Instance == null || s_curState == State.DisappearToExit)
		{
			return;
		}
		switch ((CollectionMainMenu.Buttons)arg)
		{
		case CollectionMainMenu.Buttons.Sound:
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionSound, CollectionMainMenu.prevMenuType == CollectionMainMenu.PrevMenuType.MainMenu));
			break;
		case CollectionMainMenu.Buttons.Image:
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionImage));
			break;
		case CollectionMainMenu.Buttons.Keyword:
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionKeyword));
			break;
		case CollectionMainMenu.Buttons.Profile:
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionProfile));
			break;
		case CollectionMainMenu.Buttons.Trophy:
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionTrophy));
			break;
		case CollectionMainMenu.Buttons.Exit:
			if (CollectionMainMenu.prevMenuType == CollectionMainMenu.PrevMenuType.SystemMenu)
			{
				s_Instance.StartCoroutine(ChangeMode(Mode.SystemMenu, SystemMenuPlus.Mode.Collection));
			}
			else if (CollectionMainMenu.prevMenuType == CollectionMainMenu.PrevMenuType.MainMenu)
			{
				s_Instance.StartCoroutine(ChangeMode(Mode.MainMenu, 4));
			}
			break;
		}
	}

	private static void EventProc_ExitCollectionMenu(object sender, object arg)
	{
		if (!(s_Instance == null) && s_curState != State.DisappearToExit && !(bool)arg)
		{
			CollectionMainMenu.Buttons buttons = CollectionMainMenu.Buttons.None;
			buttons = s_curMode switch
			{
				Mode.CollectionSound => CollectionMainMenu.Buttons.Sound, 
				Mode.CollectionImage => CollectionMainMenu.Buttons.Image, 
				Mode.CollectionKeyword => CollectionMainMenu.Buttons.Keyword, 
				Mode.CollectionProfile => CollectionMainMenu.Buttons.Profile, 
				Mode.CollectionTrophy => CollectionMainMenu.Buttons.Trophy, 
				_ => CollectionMainMenu.Buttons.Sound, 
			};
			s_Instance.StartCoroutine(ChangeMode(Mode.CollectionMain, buttons));
		}
	}

	private static void EventProc_NoticeExit(object sender, object arg)
	{
		if ((bool)arg)
		{
			s_curState = State.DisappearToExit;
			s_Instance.ExitProcStart();
		}
	}
}
