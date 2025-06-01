using System.Collections;
using System.IO;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTest : MonoBehaviour
{
	public Text m_TestText;

	public Behaviour[] m_ImageEffects;

	public TalkCutStart m_TalkCutStart;

	public GameObject m_PopupRootObject;

	public SWatchMenuPlus m_SWatchMenu;

	public GameObject m_LoadingIconArea;

	private LoadingSWatchIcon m_LoadingIcon;

	private const int c_TrophyCount = 15;

	private int m_curTrophyIdx = 1;

	public CommonSelectionPlus m_SelectionPopup;

	public bool m_isCompleteCheck_FloatingUIEvent;

	public Image m_RenderTestImage;

	public Sprite m_TestSprite;

	public TagText m_TestTagText;

	private bool m_needCheckMsgContent;

	private bool m_isCaptureScreen;

	private bool m_isOn;

	public Camera m_CaptureCam;

	private Texture2D m_CapturedImg;

	private void Awake()
	{
		ContiDataHandler.Init("ContiData");
		GameObject gameObject = GameObject.Find("Button_OpenMenu");
		if (gameObject != null)
		{
			Button component = gameObject.GetComponent<Button>();
			if (component != null)
			{
				Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
				buttonClickedEvent.AddListener(OnButtonClicked_Menu);
				component.onClick = buttonClickedEvent;
			}
		}
		PopupDialoguePlus.InitPopupDialogues(m_PopupRootObject);
		LoadingScreen.LoadAssetObject();
		LoadingSWatchIcon.LoadAssetObject();
		EventCameraEffect.Instance.PreLoadPrefabAssets();
		EventCameraEffect.Instance.SetCurrentCamera(Camera.main);
		GamePadInput.Init();
	}

	private void OnDestory()
	{
		PS4LargoPluginHandler.ReleaseAll();
	}

	private IEnumerator Start()
	{
		yield return StartCoroutine(XlsDataHandler.Init("xls", isLoadAllData: true, isForceLoad: false, this));
		TagText.BuildGlobalDefinedTag();
		yield return StartCoroutine(MSGContentPlus.LoadMsgContentPrefabs(this));
		RenderManager.instance.ReflashRenderCamera();
		yield return StartCoroutine(RenderManager.instance.ActivateBGImage("Cimg_601E2E", this));
	}

	private void Update()
	{
		EventCameraEffect.Instance.Update();
		GamePadInput.Update();
		ButtonPadInput.Update();
		RenderManager.instance.Render();
		if (m_isCompleteCheck_FloatingUIEvent && FloatingUIHandler.IsCompleteEvents())
		{
			m_isCompleteCheck_FloatingUIEvent = false;
		}
		if (GamePadInput.IsButtonState_Down(PadInput.InputType.ButtonL1))
		{
			if (ChangeSceneScreen.GetCurrentInstance == null)
			{
				StartCoroutine(ChangeSceneScreen.Show(null, this));
			}
			else
			{
				ChangeSceneScreen.Close();
			}
		}
	}

	private void OnComplete_ChangeSceneScreenAppeared(object sender, object arg)
	{
	}

	private void OnComplete_ChangeSceneScreenClosed(object sender, object arg)
	{
	}

	private void OnComplete_SWatchStateChanged(object sender, object arg)
	{
	}

	private void OnComplete_MentalGageHide(object sender, object arg)
	{
	}

	private void EventProc_ClosedMainMenuCommon(object sender, object arg)
	{
		MainMenuCommon.DestoryInstance();
	}

	private void LoadingIconDisappeared(object sender, object arg)
	{
	}

	private void LateUpdate()
	{
	}

	public void OnButtonClicked_Menu()
	{
		if (m_TestTagText != null && m_TestTagText.pageCount > 1 && !m_TestTagText.ToNextPage())
		{
			m_TestTagText.SetCurrentPage(0);
		}
	}

	private void OnCompleteTyping()
	{
	}

	private void OnEventComplete_SplitScreenReaction(object sender, object arg)
	{
	}

	private void OnResult_SelectionPopup(object sender, object arg)
	{
	}

	private void OnProcPopupResult(PopupDialoguePlus.Result result)
	{
	}

	private IEnumerator RenderToTextrue()
	{
		yield return new WaitForEndOfFrame();
		if (!(m_CaptureCam == null))
		{
			m_CaptureCam.gameObject.SetActive(value: true);
			m_CaptureCam.CopyFrom(Camera.main);
			int iRTWidth = Screen.width;
			int iRTHeight = Screen.height;
			RenderTexture rTex = new RenderTexture(iRTWidth, iRTHeight, 24, RenderTextureFormat.ARGB32);
			m_CaptureCam.targetTexture = rTex;
			m_CaptureCam.Render();
			RenderTexture.active = rTex;
			if (m_CapturedImg == null)
			{
				m_CapturedImg = new Texture2D(iRTWidth, iRTHeight, TextureFormat.ARGB32, mipmap: true, linear: true);
			}
			m_CapturedImg.ReadPixels(new Rect(0f, 0f, iRTWidth, iRTHeight), 0, 0);
			m_CapturedImg.Apply();
			RenderTexture.active = null;
			byte[] byTest = m_CapturedImg.EncodeToPNG();
			File.WriteAllBytes("cap.png", byTest);
			m_CaptureCam.gameObject.SetActive(value: false);
			m_isCaptureScreen = false;
		}
	}
}
