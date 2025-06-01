using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloatingUIHandler : MonoBehaviour
{
	private enum ContentType
	{
		Unknown,
		SNSContent,
		SNSContentScroll,
		MessengerContent,
		MessengerContentScroll
	}

	private class ContentCreateParam
	{
		public ContentType type;

		public string strDataName;

		public float fInputTime;

		public bool isOnlyInput;

		public float fNormalizedPosX;

		public float fNormalizedPosY;

		public float fScale;

		public bool isFirstTalk = true;

		public float fRotateX;

		public float fRotateY;

		public float fRotateZ;

		public string tag = string.Empty;

		public ContentCreateParam(ContentType _type, string _strDataName, float _fInputTime, bool _isOnlyInput, float _fNormalizedPosX, float _fNormalizedPosY, float _fScale, bool _isFirstTalk = false, float _fRotX = 0f, float _fRotY = 0f, float _fRotZ = 0f, string _tag = "")
		{
			type = _type;
			strDataName = _strDataName;
			fInputTime = _fInputTime;
			isOnlyInput = _isOnlyInput;
			fNormalizedPosX = _fNormalizedPosX;
			fNormalizedPosY = _fNormalizedPosY;
			fScale = _fScale;
			isFirstTalk = _isFirstTalk;
			fRotateX = _fRotX;
			fRotateY = _fRotY;
			fRotateZ = _fRotZ;
			tag = _tag;
		}
	}

	private enum State
	{
		None,
		Idle,
		WaitTouch,
		WaitForScroll,
		Hiding,
		WaitInputDir,
		OnlyInputDir
	}

	private enum StaticState
	{
		None,
		Showing,
		Hiding,
		HidingAll,
		OnlyInput
	}

	private List<ContentCreateParam> m_ContentCreateParams;

	private bool m_isStarted;

	private List<GameObject> m_Contents = new List<GameObject>();

	private GameObject m_Container;

	private GameObject m_TouchCheckObj;

	private AudioManager m_AudioManager;

	private State m_curState;

	private float m_fCurWaitTime;

	private float m_fCurWaitTimeForSkip;

	private int m_iFrameDelay = 1;

	private Button m_DisappearablePsIconButton;

	private static GameObject s_RootObject = null;

	private static FloatingUIRoot s_FloatingRoot = null;

	private static SortedDictionary<int, GameObject> s_CanvasObjects = new SortedDictionary<int, GameObject>();

	private static bool s_isInitailzedStaticParams = false;

	private static float s_SNSContent_Width = 0f;

	private static float s_SNSContent_MinX = 0f;

	private static float s_SNSContent_MaxX = 0f;

	private static float s_MsgContent_Width = 0f;

	private static float s_MsgContent_MinX = 0f;

	private static float s_MsgContent_MaxX = 0f;

	private const string c_XlsDataName_SNSContent_Width = "WORLD_UI_SNS_WIDTH";

	private const string c_XlsDataName_MsgContent_Width = "WORLD_UI_MSG_WIDTH";

	private static float s_fAutoModeWaitTime = 3f;

	private static float s_fSkipModeWaitTime = 0.5f;

	private static StaticState s_curStaticState = StaticState.None;

	private static int s_HidingLayerFlag = -1;

	private static string s_UnknownCharName = string.Empty;

	private static Color s_DefCharNameColor = Color.white;

	private static Color s_BacklogFaterNameColor = Color.white;

	private void Start()
	{
		m_AudioManager = GameGlobalUtil.GetAudioManager();
		m_isStarted = true;
	}

	private void OnDestroy()
	{
		if (m_ContentCreateParams != null)
		{
			m_ContentCreateParams.Clear();
		}
		if (m_Contents != null)
		{
			m_Contents.Clear();
		}
		m_Container = null;
		m_TouchCheckObj = null;
		m_AudioManager = null;
	}

	private void CreateContent_byParams()
	{
		if (!m_isStarted || m_ContentCreateParams == null || m_ContentCreateParams.Count <= 0)
		{
			return;
		}
		ContentCreateParam contentCreateParam = null;
		int count = m_ContentCreateParams.Count;
		for (int i = 0; i < count; i++)
		{
			contentCreateParam = m_ContentCreateParams[i];
			if (contentCreateParam.type == ContentType.SNSContent)
			{
				_ShowSNSPost(contentCreateParam.strDataName, contentCreateParam.fNormalizedPosX, contentCreateParam.fNormalizedPosY, contentCreateParam.fScale, contentCreateParam.fRotateX, contentCreateParam.fRotateY, contentCreateParam.fRotateZ, contentCreateParam.tag);
			}
			else if (contentCreateParam.type == ContentType.MessengerContent)
			{
				_ShowMSGTalk(contentCreateParam.strDataName, contentCreateParam.fInputTime, contentCreateParam.isOnlyInput, contentCreateParam.fNormalizedPosX, contentCreateParam.fNormalizedPosY, contentCreateParam.fScale, contentCreateParam.isFirstTalk, contentCreateParam.fRotateX, contentCreateParam.fRotateY, contentCreateParam.fRotateZ, contentCreateParam.tag);
			}
		}
		m_ContentCreateParams.Clear();
		m_ContentCreateParams = null;
	}

	private void Update()
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		if (instance == null)
		{
			return;
		}
		if (m_curState == State.None)
		{
			if (m_iFrameDelay == 0)
			{
				CreateContent_byParams();
			}
			else
			{
				m_iFrameDelay--;
			}
		}
		else if (m_curState == State.WaitTouch)
		{
			if (m_Contents != null && m_Contents.Count > 0 && (instance.GetSkip() || instance.GetAuto()))
			{
				GameObject gameObject = m_Contents[m_Contents.Count - 1];
				Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
				if (componentInChildren != null && componentInChildren.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					if (instance.GetSkip())
					{
						m_fCurWaitTimeForSkip -= Time.deltaTime;
						if (m_fCurWaitTimeForSkip <= 0f)
						{
							OnEventProc_TouchObjectClicked(null);
						}
					}
					else if (instance.GetAuto())
					{
						m_fCurWaitTime -= Time.deltaTime;
						if (m_fCurWaitTime <= 0f)
						{
							OnEventProc_TouchObjectClicked(null);
						}
					}
				}
			}
			if (!GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || PopupDialoguePlus.IsAnyPopupActivated())
			{
				return;
			}
			GameObject gameObject2 = m_Contents[m_Contents.Count - 1];
			IFloatingUIObject component = gameObject2.GetComponent<IFloatingUIObject>();
			if (component != null && component.foPsIconButton != null && m_DisappearablePsIconButton == null)
			{
				m_DisappearablePsIconButton = component.foPsIconButton;
				ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, component.foPsIconButton, null, null, null, OnPressed_PsIconButton);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Push_PopOK");
				}
			}
			else
			{
				OnEventProc_TouchObjectClicked(null);
			}
		}
		else if (m_curState == State.WaitInputDir)
		{
			GameObject gameObject3 = m_Contents[m_Contents.Count - 1];
			IFloatingUIObject component2 = gameObject3.GetComponent<IFloatingUIObject>();
			if (component2 != null && component2.foAnimator != null)
			{
				if (component2.foAnimator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					m_curState = State.WaitTouch;
				}
			}
			else
			{
				m_curState = State.WaitTouch;
			}
		}
		else
		{
			if (m_curState != State.Hiding)
			{
				return;
			}
			bool flag = true;
			string text = GameDefine.UIAnimationState.disappear.ToString();
			int num = ((m_Contents != null) ? m_Contents.Count : 0);
			if (!EventEngine.GetInstance().GetSkip())
			{
				bool flag2 = true;
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject4 = m_Contents[i];
					if (gameObject4 == null)
					{
						continue;
					}
					Animator componentInChildren2 = gameObject4.GetComponentInChildren<Animator>();
					if (componentInChildren2 != null)
					{
						AnimatorStateInfo currentAnimatorStateInfo = componentInChildren2.GetCurrentAnimatorStateInfo(0);
						if (!currentAnimatorStateInfo.IsName(text) || currentAnimatorStateInfo.normalizedTime < 0.99f)
						{
							flag2 = false;
							flag = false;
							break;
						}
					}
					if (flag2)
					{
						if (s_FloatingRoot != null)
						{
							s_FloatingRoot.RemoveTagedObject(gameObject4);
						}
						Object.Destroy(gameObject4);
						m_Contents[i] = null;
					}
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject5 = m_Contents[j];
					if (!(gameObject5 == null))
					{
						if (s_FloatingRoot != null)
						{
							s_FloatingRoot.RemoveTagedObject(gameObject5);
						}
						Object.Destroy(gameObject5);
						m_Contents[j] = null;
					}
				}
			}
			if (flag)
			{
				m_Contents.Clear();
				DestoryCanvasObject(base.gameObject);
			}
		}
	}

	private void OnPressed_PsIconButton()
	{
		OnEventProc_TouchObjectClicked(null);
		if (m_DisappearablePsIconButton != null)
		{
			GameGlobalUtil.PlayUIAnimation_WithChidren(m_DisappearablePsIconButton.gameObject, GameDefine.UIAnimationState.disappear.ToString());
			m_DisappearablePsIconButton = null;
		}
	}

	public void SetCamera(int layerFlag)
	{
		int layerID_byCharLayer = GameDefine.GetLayerID_byCharLayer(layerFlag);
		Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(layerID_byCharLayer);
		Canvas component = base.gameObject.GetComponent<Canvas>();
		component.worldCamera = camera_byLayerId;
		component.planeDistance = 1f;
	}

	private void InitChildObject()
	{
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if (m_Container == null)
		{
			m_Container = new GameObject("Container");
			Transform component2 = m_Container.GetComponent<Transform>();
			component2.position = Vector3.zero;
			component2.rotation = Quaternion.identity;
			component2.localScale = Vector3.one;
			component2.SetParent(component, worldPositionStays: false);
			RectTransform rectTransform = m_Container.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rect.width);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component.rect.height);
		}
		if (m_TouchCheckObj == null)
		{
			m_TouchCheckObj = new GameObject("TouchObj");
			Transform component3 = m_TouchCheckObj.GetComponent<Transform>();
			component3.position = Vector3.zero;
			component3.rotation = Quaternion.identity;
			component3.localScale = Vector3.one;
			component3.SetParent(component, worldPositionStays: false);
			RectTransform rectTransform2 = m_TouchCheckObj.AddComponent<RectTransform>();
			rectTransform2.anchorMin = Vector2.zero;
			rectTransform2.anchorMax = Vector2.one;
			rectTransform2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rect.width);
			rectTransform2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component.rect.height);
			Image image = m_TouchCheckObj.AddComponent<Image>();
			image.sprite = null;
			image.color = new Color(0f, 0f, 0f, 0f);
			image.raycastTarget = true;
			GameGlobalUtil.AddEventTrigger(m_TouchCheckObj, EventTriggerType.PointerClick, OnEventProc_TouchObjectClicked);
			m_TouchCheckObj.SetActive(value: false);
		}
	}

	private void _ShowSNSPost(string strSnsPostName, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		if (!m_isStarted)
		{
			if (m_ContentCreateParams == null)
			{
				m_ContentCreateParams = new List<ContentCreateParam>();
			}
			ContentCreateParam item = new ContentCreateParam(ContentType.SNSContent, strSnsPostName, 0f, _isOnlyInput: false, fNormalizedPosX, fNormalizedPosY, fScale, _isFirstTalk: false, fRotX, fRotY, fRotZ, tag);
			m_ContentCreateParams.Add(item);
			return;
		}
		GameObject gameObject = SNSContentPlus.CreateSNSContentObject(Xls.SNSPostData.GetData_byKey(strSnsPostName), SNSMenuPlus.Mode.WatchMenu, isForEvent: true);
		if (!(gameObject == null))
		{
			ShowContentCommon(gameObject, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag, s_SNSContent_Width);
			m_Contents.Add(gameObject);
			if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound("Float_SNS");
			}
			SNSContentPlus component = gameObject.GetComponent<SNSContentPlus>();
			if (component != null)
			{
				BacklogDataManager.AddBacklogData_Fater(component.text, s_BacklogFaterNameColor);
			}
		}
	}

	private void _ShowMSGTalk(string strMsgDataName, float inputTime, bool isOnlyInput, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, bool isFirstTalk = true, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		if (!m_isStarted)
		{
			if (m_ContentCreateParams == null)
			{
				m_ContentCreateParams = new List<ContentCreateParam>();
			}
			ContentCreateParam item = new ContentCreateParam(ContentType.MessengerContent, strMsgDataName, inputTime, isOnlyInput, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, fRotX, fRotY, fRotZ, tag);
			m_ContentCreateParams.Add(item);
			return;
		}
		GameObject gameObject = MSGContentPlus.CreateMSGContentObject(Xls.MessengerTalkData.GetData_byKey(strMsgDataName), isFirstTalk, isForEvent: true);
		if (!(gameObject == null))
		{
			ShowContentCommon(gameObject, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag, s_MsgContent_Width);
			m_Contents.Add(gameObject);
			MSGContentPlus component = gameObject.GetComponent<MSGContentPlus>();
			if (inputTime > 0.0001f)
			{
				component.SetInputDirectionParams(inputTime, isOnlyInput, OnFinished_OnlyInputDir);
				m_curState = ((!isOnlyInput) ? State.WaitInputDir : State.OnlyInputDir);
			}
			else if (m_AudioManager != null)
			{
				m_AudioManager.PlayUISound((!component.isMyTalk) ? "MSG_Other" : "MSG_Mine");
			}
			AddMsgTalkToBacklog(component);
		}
	}

	private void ShowContentCommon(GameObject contentObject, float fNormalizedPosX, float fNormalizedPosY, float fScale, float fRotX, float fRotY, float fRotZ, string tag, float fContentWidth)
	{
		RectTransform component = m_Container.GetComponent<RectTransform>();
		RectTransform component2 = contentObject.GetComponent<RectTransform>();
		component2.SetParent(component, worldPositionStays: false);
		component2.localScale = Vector3.one * fScale;
		component2.pivot = new Vector2(0.5f, 0.5f);
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(0f, 0f);
		component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fContentWidth);
		float x = fNormalizedPosX * 1920f;
		float y = fNormalizedPosY * 1080f;
		component2.anchoredPosition = new Vector2(x, y);
		component2.localRotation = Quaternion.Euler(fRotX, fRotY, fRotZ);
		contentObject.SetActive(value: true);
		int count = m_Contents.Count;
		IFloatingUIObject floatingUIObject = null;
		for (int i = 0; i < count; i++)
		{
			if (!(m_Contents[i] == contentObject))
			{
				floatingUIObject = m_Contents[i].GetComponent<IFloatingUIObject>();
				if (floatingUIObject != null && floatingUIObject.foPsIconButton != null)
				{
					floatingUIObject.foPsIconButton.gameObject.SetActive(value: false);
				}
			}
		}
		floatingUIObject = contentObject.GetComponent<IFloatingUIObject>();
		if (floatingUIObject != null && floatingUIObject.foPsIconButton != null)
		{
			floatingUIObject.foPsIconButton.gameObject.SetActive(value: true);
		}
		if (s_FloatingRoot != null)
		{
			s_FloatingRoot.AddTagedObject(contentObject, tag, this);
		}
		if (EventEngine.GetInstance().GetSkip())
		{
			OnSkipContentAppear();
			return;
		}
		m_TouchCheckObj.SetActive(value: true);
		m_curState = State.WaitTouch;
		m_fCurWaitTime = s_fAutoModeWaitTime;
		m_fCurWaitTimeForSkip = s_fSkipModeWaitTime;
	}

	private void OnFinished_OnlyInputDir(object sender, object arg)
	{
		IFloatingUIObject floatingUIObject = ((sender == null) ? null : (sender as IFloatingUIObject));
		if (floatingUIObject == null || floatingUIObject.foGameObject == null)
		{
			return;
		}
		GameObject gameObject = null;
		List<GameObject>.Enumerator enumerator = m_Contents.GetEnumerator();
		while (enumerator.MoveNext())
		{
			gameObject = enumerator.Current;
			if (object.ReferenceEquals(gameObject, floatingUIObject.foGameObject))
			{
				m_Contents.Remove(gameObject);
				if (s_FloatingRoot != null)
				{
					s_FloatingRoot.RemoveTagedObject(gameObject);
				}
				Object.Destroy(gameObject);
				m_curState = State.Idle;
				break;
			}
		}
		if (m_Contents.Count <= 0)
		{
			DestoryCanvasObject(base.gameObject);
		}
	}

	private void OnEventProc_TouchObjectClicked(BaseEventData evtData)
	{
		if (m_Contents != null && m_Contents.Count > 0)
		{
			GameObject gameObject = m_Contents[m_Contents.Count - 1];
			Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
			if (componentInChildren != null && componentInChildren.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.appear.ToString()))
			{
				componentInChildren.Play(GameDefine.UIAnimationState.idle.ToString());
			}
			IFloatingUIObject component = gameObject.GetComponent<IFloatingUIObject>();
			if (component != null && component.foPsIconButton != null)
			{
				GameGlobalUtil.PlayUIAnimation_WithChidren(component.foPsIconButton.gameObject, GameDefine.UIAnimationState.disappear.ToString());
			}
		}
		m_TouchCheckObj.SetActive(value: false);
		m_curState = State.Idle;
	}

	private void OnSkipContentAppear()
	{
		OnEventProc_TouchObjectClicked(null);
	}

	private void HideContents(float fSpeedRate = 1f)
	{
		int count = m_Contents.Count;
		GameObject gameObject = null;
		string stateName = GameDefine.UIAnimationState.disappear.ToString();
		for (int i = 0; i < count; i++)
		{
			gameObject = m_Contents[i];
			Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
			if ((bool)componentInChildren && !componentInChildren.GetCurrentAnimatorStateInfo(0).IsName(stateName))
			{
				componentInChildren.Play(stateName);
				componentInChildren.speed = fSpeedRate;
			}
		}
		m_curState = State.Hiding;
	}

	public void DestoryContent(GameObject contentObj, bool needContainCheck = false, bool removeEvents = true)
	{
		if (!needContainCheck || m_Contents.Contains(contentObj))
		{
			m_Contents.Remove(contentObj);
			if (s_FloatingRoot != null)
			{
				s_FloatingRoot.RemoveTagedObject(contentObj, removeEvents);
			}
			Object.Destroy(contentObj);
			if (m_Contents.Count <= 0)
			{
				DestoryCanvasObject(base.gameObject);
			}
		}
	}

	private void _SetContainerRotation(float rotX, float rotY, float rotZ)
	{
		if (!(m_Container == null))
		{
			RectTransform component = m_Container.GetComponent<RectTransform>();
			if (!(component == null))
			{
				component.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
			}
		}
	}

	private static void InitStaticParams()
	{
		if (s_isInitailzedStaticParams)
		{
			return;
		}
		Xls.ProgramDefineStr programDefineStr = null;
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("WORLD_UI_SNS_WIDTH");
		if (programDefineStr != null)
		{
			s_SNSContent_Width = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		programDefineStr = Xls.ProgramDefineStr.GetData_byKey("WORLD_UI_MSG_WIDTH");
		if (programDefineStr != null)
		{
			s_MsgContent_Width = float.Parse(programDefineStr.m_strTxt, CultureInfo.InvariantCulture);
		}
		s_SNSContent_MaxX = (1920f - s_SNSContent_Width) * 0.5f;
		s_SNSContent_MinX = 0f - s_SNSContent_MaxX;
		s_MsgContent_MaxX = (1920f - s_MsgContent_Width) * 0.5f;
		s_MsgContent_MinX = 0f - s_MsgContent_MaxX;
		s_fAutoModeWaitTime = GameSwitch.GetInstance().GetAutoDelayTime();
		s_fAutoModeWaitTime += 2f;
		s_UnknownCharName = GameGlobalUtil.GetXlsProgramText("CHR_UNKNOWN");
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("PRINTTALK_NAME_DEF_COLOR");
		if (!string.IsNullOrEmpty(xlsProgramDefineStr))
		{
			float[] fRGB = new float[3];
			if (GameGlobalUtil.ConvertHexStrToRGB(xlsProgramDefineStr, ref fRGB))
			{
				s_DefCharNameColor = new Color(fRGB[0], fRGB[1], fRGB[2]);
			}
		}
		xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("BACKLOG_FATER_NAME_COLOR");
		if (!string.IsNullOrEmpty(xlsProgramDefineStr))
		{
			float[] fRGB2 = new float[3];
			if (GameGlobalUtil.ConvertHexStrToRGB(xlsProgramDefineStr, ref fRGB2))
			{
				s_BacklogFaterNameColor = new Color(fRGB2[0], fRGB2[1], fRGB2[2]);
			}
		}
		s_isInitailzedStaticParams = true;
	}

	private static GameObject GetRootObject()
	{
		if (s_RootObject != null)
		{
			return s_RootObject;
		}
		s_RootObject = new GameObject("FloatingUIRoot");
		Transform component = s_RootObject.GetComponent<Transform>();
		component.position = Vector3.zero;
		component.rotation = Quaternion.identity;
		component.localScale = Vector3.one;
		s_FloatingRoot = s_RootObject.AddComponent<FloatingUIRoot>();
		InitStaticParams();
		return s_RootObject;
	}

	private static GameObject GetCanvasObject(int layerFlag)
	{
		GameObject rootObject = GetRootObject();
		if (rootObject == null)
		{
			return null;
		}
		GameObject gameObject = ((!s_CanvasObjects.ContainsKey(layerFlag)) ? null : s_CanvasObjects[layerFlag]);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = new GameObject($"CanvasObj_Layer{layerFlag}");
		gameObject.layer = GameDefine.GetLayerID_byCharLayer(layerFlag);
		if (s_CanvasObjects.ContainsKey(layerFlag))
		{
			s_CanvasObjects[layerFlag] = gameObject;
		}
		else
		{
			s_CanvasObjects.Add(layerFlag, gameObject);
		}
		FloatingUIHandler floatingUIHandler = gameObject.AddComponent<FloatingUIHandler>();
		Canvas canvas = gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		floatingUIHandler.SetCamera(layerFlag);
		canvas.sortingLayerName = SortingLayer.layers[GameDefine.GetSortLayerID_byCharLayer(layerFlag)].name;
		CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.matchWidthOrHeight = 0.5f;
		canvasScaler.referencePixelsPerUnit = 100f;
		GraphicRaycaster graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
		graphicRaycaster.ignoreReversedGraphics = true;
		graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.SetParent(rootObject.GetComponent<Transform>());
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1920f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1080f);
		floatingUIHandler.InitChildObject();
		return gameObject;
	}

	private static void DestoryCanvasObject(GameObject canvasObj)
	{
		if (canvasObj == null)
		{
			return;
		}
		int num = -1;
		foreach (KeyValuePair<int, GameObject> s_CanvasObject in s_CanvasObjects)
		{
			if (s_CanvasObject.Value == canvasObj)
			{
				num = s_CanvasObject.Key;
				break;
			}
		}
		if (num >= 0)
		{
			s_CanvasObjects.Remove(num);
		}
		Object.Destroy(canvasObj);
	}

	public static void ShowSNSPost(string strSnsPostName, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		GameObject canvasObject = GetCanvasObject(layerFlag);
		if (!(canvasObject == null))
		{
			FloatingUIHandler component = canvasObject.GetComponent<FloatingUIHandler>();
			component._ShowSNSPost(strSnsPostName, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag);
			s_curStaticState = StaticState.Showing;
		}
	}

	public static void ShowSNSPost_byScreen(string strSnsPostName, float fScreenPosX, float fScreenPosY, float fScale = 1f, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		float fNormalizedPosX = fScreenPosX / 1920f;
		float fNormalizedPosY = fScreenPosY / 1080f;
		ShowSNSPost(strSnsPostName, fNormalizedPosX, fNormalizedPosY, fScale, layerFlag, fRotX, fRotY, fRotZ, tag);
	}

	public static IEnumerator ShowSNSPost_Scroll(string strSnsPostName, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		List<IFloatingUIObject> floatingUIObjects = null;
		if (s_FloatingRoot != null)
		{
			floatingUIObjects = s_FloatingRoot.GetTagedObjectList(tag);
		}
		if (floatingUIObjects == null || floatingUIObjects.Count <= 0)
		{
			ShowSNSPost(strSnsPostName, fNormalizedPosX, fNormalizedPosY, fScale, layerFlag, fRotX, fRotY, fRotZ, tag);
			yield break;
		}
		GameObject canvasObj = GetCanvasObject(layerFlag);
		if (canvasObj == null)
		{
			yield break;
		}
		GameObject contentObj = SNSContentPlus.CreateSNSContentObject(Xls.SNSPostData.GetData_byKey(strSnsPostName), SNSMenuPlus.Mode.WatchMenu, isForEvent: true);
		if (!(contentObj == null))
		{
			contentObj.SetActive(value: false);
			s_curStaticState = StaticState.Showing;
			FloatingUIHandler thisComp = canvasObj.GetComponent<FloatingUIHandler>();
			thisComp.m_curState = State.WaitForScroll;
			thisComp.m_Contents.Add(contentObj);
			yield return MainLoadThing.instance.StartCoroutine(ScrollOldContents_CommonProc(thisComp, contentObj, floatingUIObjects, s_SNSContent_Width, fNormalizedPosY, fScale, tag, fMoveTime, moveTypeKey));
			thisComp.ShowContentCommon(contentObj, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag, s_SNSContent_Width);
			if (thisComp.m_AudioManager != null)
			{
				thisComp.m_AudioManager.PlayUISound("Float_SNS");
			}
			SNSContentPlus faterContent = contentObj.GetComponent<SNSContentPlus>();
			if (faterContent != null)
			{
				BacklogDataManager.AddBacklogData_Fater(faterContent.text, s_BacklogFaterNameColor);
			}
		}
	}

	public static void ShowMSGTalk(string strMsgDataName, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, bool isFirstTalk = true, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		GameObject canvasObject = GetCanvasObject(layerFlag);
		if (!(canvasObject == null))
		{
			FloatingUIHandler component = canvasObject.GetComponent<FloatingUIHandler>();
			component._ShowMSGTalk(strMsgDataName, 0f, isOnlyInput: false, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, fRotX, fRotY, fRotZ, tag);
			s_curStaticState = StaticState.Showing;
		}
	}

	public static void ShowMSGTalk_byScreen(string strMsgDataName, float fScreenPosX, float fScreenPosY, float fScale = 1f, bool isFirstTalk = true, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "")
	{
		float fNormalizedPosX = fScreenPosX / 1920f;
		float fNormalizedPosY = fScreenPosY / 1080f;
		ShowMSGTalk(strMsgDataName, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, layerFlag, fRotX, fRotY, fRotZ, tag);
	}

	public static void ShowMSGTalk_withInput(string msgDataName, float inputTime, float norPosX, float norPosY, float scale = 1f, bool isFirstTalk = true, int layerFlag = 0, float rotX = 0f, float rotY = 0f, float rotZ = 0f, string tag = "")
	{
		GameObject canvasObject = GetCanvasObject(layerFlag);
		if (!(canvasObject == null))
		{
			FloatingUIHandler component = canvasObject.GetComponent<FloatingUIHandler>();
			component._ShowMSGTalk(msgDataName, inputTime, isOnlyInput: false, norPosX, norPosY, scale, isFirstTalk, rotX, rotY, rotZ, tag);
			s_curStaticState = StaticState.Showing;
		}
	}

	public static void ShowMSGTalk_byScreen_withInput(string msgDataName, float inputTime, float screenPosX, float screenPosY, float scale = 1f, bool isFirstTalk = true, int layerFlag = 0, float rotX = 0f, float rotY = 0f, float rotZ = 0f, string tag = "")
	{
		float norPosX = screenPosX / 1920f;
		float norPosY = screenPosY / 1080f;
		ShowMSGTalk_withInput(msgDataName, inputTime, norPosX, norPosY, scale, isFirstTalk, layerFlag, rotX, rotY, rotZ, tag);
	}

	public static void ShowMSGTalk_onlyInput(string msgDataName, float inputTime, float norPosX, float norPosY, float scale = 1f, bool isFirstTalk = true, int layerFlag = 0, float rotX = 0f, float rotY = 0f, float rotZ = 0f, string tag = "")
	{
		GameObject canvasObject = GetCanvasObject(layerFlag);
		if (!(canvasObject == null))
		{
			FloatingUIHandler component = canvasObject.GetComponent<FloatingUIHandler>();
			component._ShowMSGTalk(msgDataName, inputTime, isOnlyInput: true, norPosX, norPosY, scale, isFirstTalk, rotX, rotY, rotZ, tag);
			s_curStaticState = StaticState.OnlyInput;
		}
	}

	public static void ShowMSGTalk_byScreen_onlyInput(string msgDataName, float inputTime, float screenPosX, float screenPosY, float scale = 1f, bool isFirstTalk = true, int layerFlag = 0, float rotX = 0f, float rotY = 0f, float rotZ = 0f, string tag = "")
	{
		float norPosX = screenPosX / 1920f;
		float norPosY = screenPosY / 1080f;
		ShowMSGTalk_onlyInput(msgDataName, inputTime, norPosX, norPosY, scale, isFirstTalk, layerFlag, rotX, rotY, rotZ, tag);
	}

	public static IEnumerator ShowMSGTalk_Scroll(string strMsgDataName, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, bool isFirstTalk = true, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		List<IFloatingUIObject> floatingUIObjects = null;
		if (s_FloatingRoot != null)
		{
			floatingUIObjects = s_FloatingRoot.GetTagedObjectList(tag);
		}
		if (floatingUIObjects == null || floatingUIObjects.Count <= 0)
		{
			ShowMSGTalk(strMsgDataName, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, layerFlag, fRotX, fRotY, fRotZ, tag);
			yield break;
		}
		GameObject canvasObj = GetCanvasObject(layerFlag);
		if (canvasObj == null)
		{
			yield break;
		}
		GameObject contentObj = MSGContentPlus.CreateMSGContentObject(Xls.MessengerTalkData.GetData_byKey(strMsgDataName), isFirstTalk, isForEvent: true);
		if (!(contentObj == null))
		{
			contentObj.SetActive(value: false);
			s_curStaticState = StaticState.Showing;
			FloatingUIHandler thisComp = canvasObj.GetComponent<FloatingUIHandler>();
			thisComp.m_curState = State.WaitForScroll;
			thisComp.m_Contents.Add(contentObj);
			yield return MainLoadThing.instance.StartCoroutine(ScrollOldContents_CommonProc(thisComp, contentObj, floatingUIObjects, s_MsgContent_Width, fNormalizedPosY, fScale, tag, fMoveTime, moveTypeKey));
			thisComp.ShowContentCommon(contentObj, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag, s_MsgContent_Width);
			MSGContentPlus msgComponent = contentObj.GetComponent<MSGContentPlus>();
			if (thisComp.m_AudioManager != null)
			{
				thisComp.m_AudioManager.PlayUISound((!msgComponent.isMyTalk) ? "MSG_Other" : "MSG_Mine");
			}
			AddMsgTalkToBacklog(msgComponent);
		}
	}

	public static IEnumerator ShowMSGTalk_withInput_Scroll(string strMsgDataName, float inputTime, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, bool isFirstTalk = true, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		List<IFloatingUIObject> floatingUIObjects = null;
		if (s_FloatingRoot != null)
		{
			floatingUIObjects = s_FloatingRoot.GetTagedObjectList(tag);
		}
		if (floatingUIObjects == null || floatingUIObjects.Count <= 0)
		{
			ShowMSGTalk_withInput(strMsgDataName, inputTime, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, layerFlag, fRotX, fRotY, fRotZ, tag);
			yield break;
		}
		GameObject canvasObj = GetCanvasObject(layerFlag);
		if (canvasObj == null)
		{
			yield break;
		}
		GameObject contentObj = MSGContentPlus.CreateMSGContentObject(Xls.MessengerTalkData.GetData_byKey(strMsgDataName), isFirstTalk, isForEvent: true);
		if (contentObj == null)
		{
			yield break;
		}
		contentObj.SetActive(value: false);
		s_curStaticState = StaticState.Showing;
		FloatingUIHandler thisComp = canvasObj.GetComponent<FloatingUIHandler>();
		thisComp.m_curState = State.WaitForScroll;
		MSGContentPlus msgComponent = contentObj.GetComponent<MSGContentPlus>();
		if (inputTime > 0.0001f)
		{
			EventEngine eventEngine = EventEngine.GetInstance();
			msgComponent.text = string.Empty;
			yield return MainLoadThing.instance.StartCoroutine(ScrollOldContents_CommonProc(thisComp, contentObj, floatingUIObjects, s_MsgContent_Width, fNormalizedPosY, fScale, tag, fMoveTime, moveTypeKey));
			float fPosX = fNormalizedPosX * 1920f;
			float fPosY = fNormalizedPosY * 1080f;
			RectTransform rtContent = contentObj.GetComponent<RectTransform>();
			rtContent.anchoredPosition = new Vector2(fPosX, fPosY);
			rtContent.localRotation = Quaternion.Euler(fRotX, fRotY, fRotZ);
			contentObj.SetActive(value: true);
			float aniSpeedRate = ((!eventEngine.GetSkip()) ? 1f : eventEngine.GetAnimatorSkipValue());
			Animator animator = GameGlobalUtil.PlayUIAnimation_WithChidren(contentObj, MSGContentPlus.SpectialAniState.input_appear.ToString(), aniSpeedRate);
			yield return null;
			string aniName_InputIdle = MSGContentPlus.SpectialAniState.input_idle.ToString();
			while (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName_InputIdle))
			{
				yield return null;
			}
			float timeScale = ((!eventEngine.GetSkip()) ? 1f : eventEngine.GetLerpSkipValue());
			do
			{
				yield return null;
				inputTime -= Time.deltaTime * timeScale;
			}
			while (inputTime > 0.0001f);
			string aniName_InputDisappear = MSGContentPlus.SpectialAniState.input_disappear.ToString();
			animator = GameGlobalUtil.PlayUIAnimation_WithChidren(contentObj, aniName_InputDisappear, aniSpeedRate);
			yield return null;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
			{
				yield return null;
			}
			msgComponent = null;
			Object.Destroy(contentObj);
			yield return null;
			contentObj = MSGContentPlus.CreateMSGContentObject(Xls.MessengerTalkData.GetData_byKey(strMsgDataName), isFirstTalk, isForEvent: true);
			if (contentObj == null)
			{
				yield break;
			}
			contentObj.SetActive(value: false);
		}
		thisComp.m_Contents.Add(contentObj);
		yield return MainLoadThing.instance.StartCoroutine(ScrollOldContents_CommonProc(thisComp, contentObj, floatingUIObjects, s_MsgContent_Width, fNormalizedPosY, fScale, tag, fMoveTime, moveTypeKey));
		thisComp.ShowContentCommon(contentObj, fNormalizedPosX, fNormalizedPosY, fScale, fRotX, fRotY, fRotZ, tag, s_MsgContent_Width);
		msgComponent = contentObj.GetComponent<MSGContentPlus>();
		if (thisComp.m_AudioManager != null)
		{
			thisComp.m_AudioManager.PlayUISound((!msgComponent.isMyTalk) ? "MSG_Other" : "MSG_Mine");
		}
		AddMsgTalkToBacklog(msgComponent);
	}

	public static IEnumerator ShowMSGTalk_onlyInput_Scroll(string strMsgDataName, float inputTime, float fNormalizedPosX, float fNormalizedPosY, float fScale = 1f, bool isFirstTalk = true, int layerFlag = 0, float fRotX = 0f, float fRotY = 0f, float fRotZ = 0f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		List<IFloatingUIObject> floatingUIObjects = null;
		if (s_FloatingRoot != null)
		{
			floatingUIObjects = s_FloatingRoot.GetTagedObjectList(tag);
		}
		if (floatingUIObjects == null || floatingUIObjects.Count <= 0)
		{
			ShowMSGTalk_withInput(strMsgDataName, inputTime, fNormalizedPosX, fNormalizedPosY, fScale, isFirstTalk, layerFlag, fRotX, fRotY, fRotZ, tag);
			yield break;
		}
		GameObject canvasObj = GetCanvasObject(layerFlag);
		if (canvasObj == null)
		{
			yield break;
		}
		GameObject contentObj = MSGContentPlus.CreateMSGContentObject(Xls.MessengerTalkData.GetData_byKey(strMsgDataName), isFirstTalk, isForEvent: true);
		if (contentObj == null)
		{
			yield break;
		}
		contentObj.SetActive(value: false);
		s_curStaticState = StaticState.OnlyInput;
		FloatingUIHandler thisComp = canvasObj.GetComponent<FloatingUIHandler>();
		thisComp.m_curState = State.OnlyInputDir;
		MSGContentPlus msgComponent = contentObj.GetComponent<MSGContentPlus>();
		if (inputTime > 0.0001f)
		{
			EventEngine eventEngine = EventEngine.GetInstance();
			msgComponent.text = string.Empty;
			yield return MainLoadThing.instance.StartCoroutine(ScrollOldContents_CommonProc(thisComp, contentObj, floatingUIObjects, s_MsgContent_Width, fNormalizedPosY, fScale, tag, fMoveTime, moveTypeKey));
			float fPosX = fNormalizedPosX * 1920f;
			float fPosY = fNormalizedPosY * 1080f;
			RectTransform rtContent = contentObj.GetComponent<RectTransform>();
			rtContent.anchoredPosition = new Vector2(fPosX, fPosY);
			rtContent.localRotation = Quaternion.Euler(fRotX, fRotY, fRotZ);
			contentObj.SetActive(value: true);
			float aniSpeedRate = ((!eventEngine.GetSkip()) ? 1f : eventEngine.GetAnimatorSkipValue());
			Animator animator = GameGlobalUtil.PlayUIAnimation_WithChidren(contentObj, MSGContentPlus.SpectialAniState.input_appear.ToString(), aniSpeedRate);
			yield return null;
			string aniName_InputIdle = MSGContentPlus.SpectialAniState.input_idle.ToString();
			while (!animator.GetCurrentAnimatorStateInfo(0).IsName(aniName_InputIdle))
			{
				yield return null;
			}
			float timeScale = ((!eventEngine.GetSkip()) ? 1f : eventEngine.GetLerpSkipValue());
			do
			{
				yield return null;
				inputTime -= Time.deltaTime * timeScale;
			}
			while (inputTime > 0.0001f);
			string aniName_InputDisappear = MSGContentPlus.SpectialAniState.input_disappear.ToString();
			animator = GameGlobalUtil.PlayUIAnimation_WithChidren(contentObj, aniName_InputDisappear, aniSpeedRate);
			yield return null;
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f)
			{
				yield return null;
			}
			msgComponent = null;
			Object.Destroy(contentObj);
			yield return null;
		}
		s_curStaticState = StaticState.None;
		thisComp.m_curState = State.Idle;
	}

	private static IEnumerator ScrollOldContents_CommonProc(FloatingUIHandler floatingHdr, GameObject contentObj, List<IFloatingUIObject> floatingUIObjects, float contentWidth, float fNormalizedPosY, float fScale = 1f, string tag = "", float fMoveTime = 0f, string moveTypeKey = "")
	{
		IFloatingUIObject floatingObj = contentObj.GetComponent<IFloatingUIObject>();
		if (floatingObj == null)
		{
			yield break;
		}
		while (!floatingHdr.m_isStarted)
		{
			yield return null;
		}
		floatingObj.foRectTransform.SetParent(floatingHdr.m_Container.GetComponent<RectTransform>(), worldPositionStays: false);
		RectTransform rtNew = floatingObj.foRectTransform;
		rtNew.localScale = Vector3.one * fScale;
		rtNew.pivot = new Vector2(0.5f, 0.5f);
		rtNew.anchorMin = new Vector2(0f, 0f);
		rtNew.anchorMax = new Vector2(0f, 0f);
		rtNew.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
		rtNew.anchoredPosition = new Vector2(-100000f, -100000f);
		Animator animator = contentObj.GetComponentInChildren<Animator>();
		if (animator != null)
		{
			animator.enabled = false;
		}
		contentObj.SetActive(value: true);
		yield return null;
		contentObj.SetActive(value: false);
		if (animator != null)
		{
			animator.enabled = true;
		}
		float fNewContentPosY = fNormalizedPosY * 1080f;
		float fNewContentTop = fNewContentPosY + rtNew.rect.height * rtNew.localScale.y * 0.5f;
		RectTransform rtPrev = floatingUIObjects[floatingUIObjects.Count - 1].foRectTransform;
		float fPrevContentPosY = rtPrev.anchoredPosition.y;
		float fPrevContentBottom = fPrevContentPosY - rtPrev.rect.height * rtPrev.localScale.y * 0.5f;
		float fMoveDist = fNewContentTop - fPrevContentBottom;
		fMoveDist = Mathf.Max(fMoveDist, 0f);
		if (fMoveDist > 0f)
		{
			int moveType = Xls.ScriptKeyValue.GetData_byKey(moveTypeKey)?.m_iValue ?? 0;
			SetEvent_Move(tag, fMoveTime, 0f, fMoveDist, moveType);
			while (!IsCompleteEvents())
			{
				yield return null;
			}
		}
	}

	private static void AddMsgTalkToBacklog(MSGContentPlus msgComponent)
	{
		if (msgComponent == null)
		{
			return;
		}
		Color colorCharName = s_DefCharNameColor;
		Xls.AccountData data_byKey = Xls.AccountData.GetData_byKey(msgComponent.xlsData.m_strIDAcc);
		if (data_byKey != null)
		{
			Xls.CharData data_byKey2 = Xls.CharData.GetData_byKey(data_byKey.m_charKey);
			if (data_byKey2 != null && !string.IsNullOrEmpty(data_byKey2.m_strTalkColor))
			{
				float[] fRGB = new float[3];
				if (GameGlobalUtil.ConvertHexStrToRGB(data_byKey2.m_strTalkColor, ref fRGB))
				{
					colorCharName = new Color(fRGB[0], fRGB[1], fRGB[2]);
				}
			}
		}
		BacklogDataManager.AddBacklogData_Messenger(msgComponent.text, msgComponent.profileText, colorCharName);
	}

	public static void HideFloatingUI(float fSpeedRate, int layerFlag)
	{
		s_curStaticState = StaticState.Hiding;
		s_HidingLayerFlag = layerFlag;
		if (s_CanvasObjects.ContainsKey(layerFlag))
		{
			GameObject gameObject = s_CanvasObjects[layerFlag];
			if (gameObject == null)
			{
				s_CanvasObjects.Remove(layerFlag);
				return;
			}
			FloatingUIHandler component = gameObject.GetComponent<FloatingUIHandler>();
			component.HideContents(fSpeedRate);
		}
	}

	public static void HideFloatingUI_All(float fSpeedRate)
	{
		s_curStaticState = StaticState.HidingAll;
		foreach (KeyValuePair<int, GameObject> s_CanvasObject in s_CanvasObjects)
		{
			if (!(s_CanvasObject.Value == null))
			{
				FloatingUIHandler component = s_CanvasObject.Value.GetComponent<FloatingUIHandler>();
				component.HideContents(fSpeedRate);
			}
		}
	}

	public static void SetContainerRotation(int layerFlag, float rotX, float rotY, float rotZ)
	{
		GameObject canvasObject = GetCanvasObject(layerFlag);
		if (!(canvasObject == null))
		{
			FloatingUIHandler component = canvasObject.GetComponent<FloatingUIHandler>();
			if (!(component == null))
			{
				component._SetContainerRotation(rotX, rotY, rotZ);
			}
		}
	}

	public static bool IsCompleteShow()
	{
		if (s_curStaticState != StaticState.Showing)
		{
			return true;
		}
		foreach (KeyValuePair<int, GameObject> s_CanvasObject in s_CanvasObjects)
		{
			GameObject value = s_CanvasObject.Value;
			if (!(value == null))
			{
				FloatingUIHandler component = value.GetComponent<FloatingUIHandler>();
				if (component.m_curState == State.WaitTouch || component.m_curState == State.WaitInputDir || component.m_curState == State.WaitForScroll || component.m_curState == State.None)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool IsCompleteHide()
	{
		if (s_curStaticState == StaticState.Hiding)
		{
			return !s_CanvasObjects.ContainsKey(s_HidingLayerFlag);
		}
		if (s_curStaticState == StaticState.HidingAll)
		{
			return s_CanvasObjects.Count <= 0;
		}
		return true;
	}

	public static bool IsCompleteOnlyInput()
	{
		if (s_curStaticState != StaticState.OnlyInput)
		{
			return true;
		}
		foreach (KeyValuePair<int, GameObject> s_CanvasObject in s_CanvasObjects)
		{
			GameObject value = s_CanvasObject.Value;
			if (!(value == null))
			{
				FloatingUIHandler component = value.GetComponent<FloatingUIHandler>();
				if (component.m_curState == State.OnlyInputDir || component.m_curState == State.None)
				{
					return false;
				}
			}
		}
		s_curStaticState = StaticState.None;
		return true;
	}

	public static bool IsCompleteEvents()
	{
		return s_FloatingRoot == null || !s_FloatingRoot.IsExistRunningEvent();
	}

	public static void SetEvent_Disappear(string tag, float speedRate)
	{
		if (!(s_FloatingRoot == null))
		{
			s_FloatingRoot.SetEvent_Disappear(tag, speedRate);
		}
	}

	public static void SetEvent_Move(string tag, float time, float moveX, float moveY, int moveType)
	{
		if (!(s_FloatingRoot == null))
		{
			s_FloatingRoot.SetEvent_Move(tag, time, new Vector2(moveX, moveY), moveType);
		}
	}

	public static void SetEvent_Rotate(string tag, float time, float rotX, float rotY, float rotZ, int moveType)
	{
		if (!(s_FloatingRoot == null))
		{
			s_FloatingRoot.SetEvent_Rotate(tag, time, new Vector3(rotX, rotY, rotZ), moveType);
		}
	}

	public static void SetEvent_Zoom(string tag, float time, float zoom, int moveType)
	{
		if (!(s_FloatingRoot == null))
		{
			s_FloatingRoot.SetEvent_Zoom(tag, time, zoom, moveType);
		}
	}

	public static void SetEvent_Motion(string tag, string motionName, float speedRate, int loopCount)
	{
		if (!(s_FloatingRoot == null))
		{
			s_FloatingRoot.SetEvent_Motion(tag, motionName, speedRate, loopCount);
		}
	}
}
