using System;
using System.Collections;
using System.Collections.Generic;
using Colorful;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class RenderManager
{
	private class BGImageEvent
	{
		public enum EventType
		{
			None,
			Move,
			Rotate,
			Zoom
		}

		private EventType m_eventType;

		private int m_procType;

		protected RectTransform m_rtBGImage;

		public EventType eventType => m_eventType;

		public int procType => m_procType;

		protected BGImageEvent(EventType _eventType, int _procType)
		{
			m_eventType = _eventType;
			m_procType = _procType;
			m_rtBGImage = instance.m_bgImage.gameObject.GetComponent<RectTransform>();
		}

		public virtual bool ProcEvent()
		{
			return true;
		}

		public virtual void FinishEvent()
		{
		}
	}

	private class BGImageEvent_Move : BGImageEvent
	{
		private Vector2 m_startPosition = Vector2.zero;

		private Vector2 m_targetPosition = Vector2.zero;

		private float m_targetTime;

		private float m_curTime;

		public BGImageEvent_Move(int _procType, float xNormalizedPos, float yNormalizedPos, float targetTime)
			: base(EventType.Move, _procType)
		{
			m_startPosition = m_rtBGImage.anchoredPosition;
			float num = m_rtBGImage.rect.width - (float)Screen.width;
			float num2 = m_rtBGImage.rect.height - (float)Screen.height;
			Vector2 zero = Vector2.zero;
			zero.x = m_rtBGImage.rect.width * m_rtBGImage.pivot.x - (float)Screen.width * m_rtBGImage.anchorMin.x;
			zero.y = m_rtBGImage.rect.height * m_rtBGImage.pivot.y - (float)Screen.height * m_rtBGImage.anchorMin.y;
			Vector2 zero2 = Vector2.zero;
			zero2.x = zero.x - num;
			zero2.y = zero.y - num2;
			m_targetPosition.x = (0f - num) * xNormalizedPos + zero.x;
			m_targetPosition.y = (0f - num2) * yNormalizedPos + zero.y;
			m_targetTime = targetTime;
		}

		public override bool ProcEvent()
		{
			float fSmoothStepFactor = 1f;
			bool flag = EventEngine.GetInstance().ProcTime(base.procType, ref fSmoothStepFactor, ref m_curTime, m_targetTime);
			if (flag)
			{
				FinishEvent();
			}
			else
			{
				float x = Mathf.Lerp(m_startPosition.x, m_targetPosition.x, fSmoothStepFactor);
				float y = Mathf.Lerp(m_startPosition.y, m_targetPosition.y, fSmoothStepFactor);
				m_rtBGImage.anchoredPosition = new Vector2(x, y);
			}
			return flag;
		}

		public override void FinishEvent()
		{
			m_rtBGImage.anchoredPosition = m_targetPosition;
		}
	}

	private class BGImageEvent_Rotate : BGImageEvent
	{
		private Vector2 m_startRatateValue = Vector2.zero;

		private Vector2 m_targetRotateValue = Vector2.zero;

		private float m_targetTime;

		private float m_curTime;

		public BGImageEvent_Rotate(int _procType, float targetRotateX, float targetRotateY, float targetTime)
			: base(EventType.Rotate, _procType)
		{
			m_startRatateValue.x = m_rtBGImage.localEulerAngles.x;
			m_startRatateValue.y = m_rtBGImage.localEulerAngles.y;
			m_targetRotateValue.x = targetRotateX;
			m_targetRotateValue.y = targetRotateY;
			m_targetTime = targetTime;
		}

		public override bool ProcEvent()
		{
			float fSmoothStepFactor = 1f;
			bool flag = EventEngine.GetInstance().ProcTime(base.procType, ref fSmoothStepFactor, ref m_curTime, m_targetTime);
			if (flag)
			{
				FinishEvent();
			}
			else
			{
				float x = Mathf.LerpAngle(m_startRatateValue.x, m_targetRotateValue.x, fSmoothStepFactor);
				float y = Mathf.LerpAngle(m_startRatateValue.y, m_targetRotateValue.y, fSmoothStepFactor);
				m_rtBGImage.localEulerAngles = new Vector3(x, y, 0f);
			}
			return flag;
		}

		public override void FinishEvent()
		{
			m_rtBGImage.localEulerAngles = new Vector3(m_targetRotateValue.x, m_targetRotateValue.y, 0f);
		}
	}

	private class BGImageEvent_Zoom : BGImageEvent
	{
		private float m_startZoomFactor;

		private float m_targetZoomFactor;

		private float m_targetTime;

		private float m_curTime;

		public BGImageEvent_Zoom(int _procType, float targetZoomFactor, float targetTime)
			: base(EventType.Zoom, _procType)
		{
			m_startZoomFactor = m_rtBGImage.localScale.x;
			m_targetZoomFactor = targetZoomFactor;
			m_targetTime = targetTime;
		}

		public override bool ProcEvent()
		{
			float fSmoothStepFactor = 1f;
			bool flag = EventEngine.GetInstance().ProcTime(base.procType, ref fSmoothStepFactor, ref m_curTime, m_targetTime);
			if (flag)
			{
				FinishEvent();
			}
			else
			{
				float num = Mathf.Lerp(m_startZoomFactor, m_targetZoomFactor, fSmoothStepFactor);
				m_rtBGImage.localScale = new Vector3(num, num, 1f);
			}
			return flag;
		}

		public override void FinishEvent()
		{
			m_rtBGImage.localScale = new Vector3(m_targetZoomFactor, m_targetZoomFactor, 1f);
		}
	}

	private static RenderManager s_Instance;

	private SortedList<int, Camera> m_BoundLayerIds = new SortedList<int, Camera>();

	private int m_defCullingMask;

	private GameObject m_activedCameraObj;

	private Canvas m_bgImageCanvas;

	private Image m_bgImage;

	private List<BGImageEvent> m_bgImageEvents;

	private AssetBundleObjectHandler m_ABHdr_BGSprite;

	public static RenderManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new RenderManager();
			}
			return s_Instance;
		}
	}

	public RenderManager()
	{
		m_BoundLayerIds.Add(10, null);
		m_BoundLayerIds.Add(11, null);
		m_BoundLayerIds.Add(12, null);
		m_BoundLayerIds.Add(13, null);
		m_BoundLayerIds.Add(14, null);
		m_BoundLayerIds.Add(15, null);
		m_defCullingMask = 0;
		int count = m_BoundLayerIds.Count;
		for (int i = 0; i < count; i++)
		{
			m_defCullingMask |= GetCullingMaskByLayerID(m_BoundLayerIds.Keys[i]);
		}
		m_defCullingMask = ~m_defCullingMask;
	}

	public static void ReleaseInstance()
	{
		if (s_Instance != null)
		{
			if (s_Instance.m_BoundLayerIds != null)
			{
				s_Instance.m_BoundLayerIds.Clear();
			}
			s_Instance.m_activedCameraObj = null;
			s_Instance.m_bgImageCanvas = null;
			s_Instance.m_bgImage = null;
			if (s_Instance.m_bgImageEvents != null)
			{
				s_Instance.m_bgImageEvents.Clear();
			}
			s_Instance.m_bgImageEvents = null;
		}
		s_Instance = null;
	}

	public void Render(Camera camera = null)
	{
		if (camera == null)
		{
			camera = Camera.main;
		}
		if (m_activedCameraObj != camera.gameObject)
		{
			ReflashRenderCamera(camera);
		}
		UpdateBGImageEvent();
	}

	public void ReflashRenderCamera(Camera srcCamera = null)
	{
		if (srcCamera == null)
		{
			srcCamera = Camera.main;
		}
		if (srcCamera == null || m_activedCameraObj == srcCamera.gameObject)
		{
			return;
		}
		m_activedCameraObj = srcCamera.gameObject;
		Transform component = m_activedCameraObj.GetComponent<Transform>();
		int count = m_BoundLayerIds.Count;
		int num = count - 1;
		int num2 = m_BoundLayerIds.Keys[num];
		Camera camera = m_BoundLayerIds.Values[num];
		if (camera != null)
		{
			UnityEngine.Object.DestroyImmediate(camera);
			m_BoundLayerIds[num2] = null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(m_activedCameraObj);
		gameObject.name = $"LayeredCamera_{num}";
		Transform component2 = gameObject.GetComponent<Transform>();
		component2.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		component2.localScale = Vector3.one;
		component2.SetParent(component, worldPositionStays: false);
		component2.SetAsFirstSibling();
		Camera component3 = gameObject.GetComponent<Camera>();
		component3.cullingMask = GetCullingMaskByLayerID(num2);
		component3.clearFlags = CameraClearFlags.Depth;
		component3.depth = srcCamera.depth + (float)count;
		m_BoundLayerIds[num2] = component3;
		int num3 = component2.childCount;
		while (num3 > 0)
		{
			num3--;
			GameObject gameObject2 = component2.GetChild(num3).gameObject;
			UnityEngine.Object.DestroyImmediate(gameObject2);
		}
		ComponentGroup component4 = gameObject.GetComponent<ComponentGroup>();
		Component[] components = gameObject.GetComponents<Component>();
		if (component4 != null && component4.Components.Length > 0)
		{
			int num4 = components.Length;
			while (num4 > 0)
			{
				num4--;
				Component component5 = components[num4];
				if (!(component5 is Transform) && !(component5 is Camera) && !(component5 is GUILayer) && !(component5 is FlareLayer) && !(component5 is ComponentGroup))
				{
					int num5 = Array.IndexOf(component4.Components, component5);
					if (num5 < 0)
					{
						UnityEngine.Object.DestroyImmediate(component5);
					}
				}
			}
			UnityEngine.Object.DestroyImmediate(component4);
		}
		else
		{
			int num6 = components.Length;
			while (num6 > 0)
			{
				num6--;
				Component component6 = components[num6];
				if (!(component6 is Transform) && !(component6 is Camera) && !(component6 is GUILayer) && !(component6 is FlareLayer))
				{
					UnityEngine.Object.DestroyImmediate(component6);
				}
			}
		}
		int num7 = count - 1;
		while (num7 > 0)
		{
			num7--;
			int num8 = m_BoundLayerIds.Keys[num7];
			Camera camera2 = m_BoundLayerIds.Values[num7];
			if (camera2 != null)
			{
				UnityEngine.Object.DestroyImmediate(camera2);
				m_BoundLayerIds[num8] = null;
			}
			GameObject gameObject3 = new GameObject($"LayeredCamera_{num7}");
			Transform transform = gameObject3.GetComponent<Transform>();
			if (transform == null)
			{
				transform = gameObject3.AddComponent<Transform>();
			}
			transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			transform.localScale = Vector3.one;
			transform.SetParent(component, worldPositionStays: false);
			transform.SetAsFirstSibling();
			Camera camera3 = gameObject3.AddComponent<Camera>();
			camera3.CopyFrom(srcCamera);
			camera3.cullingMask = GetCullingMaskByLayerID(num8);
			camera3.clearFlags = CameraClearFlags.Depth;
			camera3.depth = srcCamera.depth + (float)num7 + 1f;
			m_BoundLayerIds[num8] = camera3;
			gameObject3.AddComponent<GUILayer>();
			gameObject3.AddComponent<FlareLayer>();
		}
		ComponentGroup component7 = m_activedCameraObj.GetComponent<ComponentGroup>();
		if (component7 != null && component7.Components != null && component7.Components.Length > 0)
		{
			Component[] components2 = m_activedCameraObj.GetComponents<Component>();
			int num9 = components2.Length;
			while (num9 > 0)
			{
				num9--;
				Component component8 = components2[num9];
				if (Array.IndexOf(component7.Components, component8) >= 0)
				{
					UnityEngine.Object.DestroyImmediate(component8);
				}
			}
		}
		srcCamera.cullingMask = m_defCullingMask;
		DestroyBGImageCanvasObject();
	}

	public void SetFOV_SubCameras(float fov)
	{
		int count = m_BoundLayerIds.Count;
		for (int i = 0; i < count; i++)
		{
			Camera camera = m_BoundLayerIds.Values[i];
			if (!(camera == null))
			{
				camera.fieldOfView = fov;
			}
		}
	}

	public Camera GetCamera_byLayerId(int layerId)
	{
		return (!m_BoundLayerIds.ContainsKey(layerId)) ? Camera.main : m_BoundLayerIds[layerId];
	}

	public Camera GetCamera_Nearest()
	{
		int count = m_BoundLayerIds.Count;
		int num = count;
		while (num > 0)
		{
			num--;
			Camera camera = m_BoundLayerIds.Values[num];
			if (camera != null)
			{
				return camera;
			}
		}
		return Camera.main;
	}

	private int GetCullingMaskByLayerID(int layerID)
	{
		int num = 0;
		return num | (1 << layerID);
	}

	public Texture2D RenderToTexture(int width = 0, int height = 0, bool enableBlur = true, bool onlyBGLayer = false)
	{
		if (m_BoundLayerIds.Count <= 0)
		{
			return null;
		}
		int width2 = ((width > 0) ? width : Screen.width);
		int height2 = ((height > 0) ? height : Screen.height);
		RenderTexture renderTexture = (RenderTexture.active = new RenderTexture(width2, height2, 24, RenderTextureFormat.ARGB32));
		Camera main = Camera.main;
		main.enabled = false;
		main.targetTexture = renderTexture;
		main.Render();
		main.enabled = true;
		main.targetTexture = null;
		int num = m_BoundLayerIds.Count - 1;
		if (!onlyBGLayer)
		{
			for (int i = 0; i < num; i++)
			{
				main = m_BoundLayerIds.Values[i];
				if (!(main == null))
				{
					main.enabled = false;
					main.targetTexture = renderTexture;
					main.Render();
					main.enabled = true;
					main.targetTexture = null;
				}
			}
		}
		if (num >= 0)
		{
			main = m_BoundLayerIds.Values[num];
			if (main != null)
			{
				GaussianBlur gaussianBlur = null;
				if (enableBlur)
				{
					gaussianBlur = main.gameObject.AddComponent<GaussianBlur>();
					gaussianBlur.Shader = Shader.Find("Hidden/Colorful/Gaussian Blur");
					gaussianBlur.Passes = 1;
					gaussianBlur.Downscaling = 2f;
					gaussianBlur.Amount = 1f;
				}
				main.enabled = false;
				main.targetTexture = renderTexture;
				main.Render();
				main.enabled = true;
				main.targetTexture = null;
				if (gaussianBlur != null)
				{
					UnityEngine.Object.DestroyImmediate(gaussianBlur);
				}
			}
		}
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, mipmap: false);
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0, recalculateMipMaps: false);
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.Apply();
		RenderTexture.active = null;
		UnityEngine.Object.DestroyImmediate(renderTexture);
		return texture2D;
	}

	public Sprite RenderToSprite(int width, int height, bool enableBlur = true, bool onlyBGLayer = false)
	{
		Texture2D texture2D = RenderToTexture(width, height, enableBlur, onlyBGLayer);
		if (texture2D == null)
		{
			return null;
		}
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
	}

	public void ActivateCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = m_defCullingMask;
		}
		if (m_BoundLayerIds == null || m_BoundLayerIds.Count <= 0)
		{
			return;
		}
		int count = m_BoundLayerIds.Count;
		for (int i = 0; i < count; i++)
		{
			main = m_BoundLayerIds.Values[i];
			if (!(main == null))
			{
				main.cullingMask = GetCullingMaskByLayerID(m_BoundLayerIds.Keys[i]);
			}
		}
	}

	public void DeactivateCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = 0;
		}
		if (m_BoundLayerIds == null || m_BoundLayerIds.Count <= 0)
		{
			return;
		}
		int count = m_BoundLayerIds.Count;
		for (int i = 0; i < count; i++)
		{
			main = m_BoundLayerIds.Values[i];
			if (!(main == null))
			{
				main.cullingMask = 0;
			}
		}
	}

	public void DeactivateCamera(int layerLevel)
	{
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = 0;
		}
		int num = Mathf.Min(m_BoundLayerIds.Count, layerLevel + 1);
		for (int i = 0; i < num; i++)
		{
			main = m_BoundLayerIds.Values[i];
			if (!(main == null))
			{
				main.cullingMask = 0;
			}
		}
		for (int j = num; j < m_BoundLayerIds.Count; j++)
		{
			main = m_BoundLayerIds.Values[j];
			if (!(main == null))
			{
				main.cullingMask = GetCullingMaskByLayerID(m_BoundLayerIds.Keys[j]);
			}
		}
	}

	public void ActivateBGCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = m_defCullingMask;
		}
	}

	public void DeactivateBGCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = 0;
		}
	}

	private void CreateBGImageCanvasObject(float imageWidth, float imageHeight)
	{
		if (m_bgImageCanvas == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "BG_Image_Canvas";
			gameObject.layer = LayerMask.NameToLayer("UI");
			m_bgImageCanvas = gameObject.AddComponent<Canvas>();
			m_bgImageCanvas.sortingOrder = -1;
			m_bgImageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
			m_bgImageCanvas.worldCamera = Camera.main;
			m_bgImageCanvas.planeDistance = m_bgImageCanvas.worldCamera.nearClipPlane + 0.1f;
			CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
			if (canvasScaler == null)
			{
				canvasScaler = gameObject.AddComponent<CanvasScaler>();
			}
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 0f;
			canvasScaler.referencePixelsPerUnit = 100f;
		}
		Transform transform = m_bgImageCanvas.gameObject.GetComponent<Transform>();
		if (transform == null)
		{
			transform = m_bgImageCanvas.gameObject.AddComponent<Transform>();
		}
		Transform component = m_bgImageCanvas.worldCamera.GetComponent<Transform>();
		transform.SetParent(component, worldPositionStays: false);
		RectTransform rectTransform = null;
		if (m_bgImage == null)
		{
			GameObject gameObject2 = new GameObject();
			gameObject2.name = "BG_Image";
			gameObject2.layer = LayerMask.NameToLayer("UI");
			m_bgImage = gameObject2.AddComponent<Image>();
			rectTransform = gameObject2.GetComponent<RectTransform>();
			if (rectTransform == null)
			{
				rectTransform = gameObject2.AddComponent<RectTransform>();
			}
			rectTransform.SetParent(transform, worldPositionStays: false);
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		}
		else
		{
			rectTransform = m_bgImage.gameObject.GetComponent<RectTransform>();
		}
		rectTransform.localScale = Vector3.one;
		rectTransform.localRotation = Quaternion.identity;
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight);
		rectTransform.anchoredPosition = new Vector2(0f, 0f);
		m_bgImageCanvas.gameObject.SetActive(value: false);
	}

	private void DestroyBGImageCanvasObject()
	{
		if (m_bgImage != null)
		{
			GameObject gameObject = m_bgImage.gameObject;
			gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(gameObject);
			m_bgImage = null;
		}
		if (m_bgImageCanvas != null)
		{
			GameObject gameObject2 = m_bgImageCanvas.gameObject;
			gameObject2.SetActive(value: false);
			UnityEngine.Object.Destroy(gameObject2);
			m_bgImageCanvas = null;
		}
	}

	public IEnumerator ActivateBGImage(string keyImageData, MonoBehaviour parentMonoBehaviour = null)
	{
		if (IsActivatingBGImage() || string.IsNullOrEmpty(keyImageData))
		{
			yield break;
		}
		Xls.ImageFile xlsImageData = Xls.ImageFile.GetData_byKey(keyImageData);
		if (xlsImageData == null)
		{
			yield break;
		}
		Sprite sprite = null;
		m_ABHdr_BGSprite = new AssetBundleObjectHandler(xlsImageData.m_strAssetPath);
		if (parentMonoBehaviour == null)
		{
			parentMonoBehaviour = MainLoadThing.instance;
		}
		parentMonoBehaviour.StartCoroutine(m_ABHdr_BGSprite.LoadAssetBundle());
		yield return null;
		sprite = m_ABHdr_BGSprite.GetLoadedAsset_ToSprite();
		if (!(sprite == null))
		{
			CreateBGImageCanvasObject(sprite.rect.width, sprite.rect.height);
			if (m_bgImage != null)
			{
				m_bgImage.sprite = sprite;
			}
			m_bgImageCanvas.gameObject.SetActive(value: true);
			Camera cam = Camera.main;
			if (cam != null)
			{
				cam.cullingMask = 1 << m_bgImageCanvas.gameObject.layer;
			}
		}
	}

	public void DeactivateBGImage()
	{
		if (m_bgImageCanvas != null)
		{
			m_bgImageCanvas.gameObject.SetActive(value: false);
		}
		if (m_bgImage != null)
		{
			m_bgImage.sprite = null;
		}
		if (m_ABHdr_BGSprite != null)
		{
			m_ABHdr_BGSprite.UnloadAssetBundle();
			m_ABHdr_BGSprite = null;
		}
		Camera main = Camera.main;
		if (main != null)
		{
			main.cullingMask = m_defCullingMask;
		}
	}

	public bool IsActivatingBGImage()
	{
		return m_bgImageCanvas != null && m_bgImageCanvas.gameObject.activeInHierarchy;
	}

	public void MoveBGImage(float xNormalizedPos, float yNormalizedPos, float time, int procType)
	{
		if (!(m_bgImage == null) && m_bgImage.gameObject.activeInHierarchy)
		{
			BGImageEvent_Move bGImageEvent_Move = new BGImageEvent_Move(procType, xNormalizedPos, yNormalizedPos, time);
			if (time <= 0f)
			{
				bGImageEvent_Move.FinishEvent();
			}
			else
			{
				AddBGImageEvent(bGImageEvent_Move);
			}
		}
	}

	public void RotateBGImage(float xRotateAngle, float yRotateAngle, float time, int procType)
	{
		if (!(m_bgImage == null) && m_bgImage.gameObject.activeInHierarchy)
		{
			BGImageEvent_Rotate bGImageEvent_Rotate = new BGImageEvent_Rotate(procType, xRotateAngle, yRotateAngle, time);
			if (time <= 0f)
			{
				bGImageEvent_Rotate.FinishEvent();
			}
			else
			{
				AddBGImageEvent(bGImageEvent_Rotate);
			}
		}
	}

	public void ZoomBGImage(float zoomFactor, float time, int procType)
	{
		if (!(m_bgImage == null) && m_bgImage.gameObject.activeInHierarchy)
		{
			BGImageEvent_Zoom bGImageEvent_Zoom = new BGImageEvent_Zoom(procType, zoomFactor, time);
			if (time <= 0f)
			{
				bGImageEvent_Zoom.FinishEvent();
			}
			else
			{
				AddBGImageEvent(bGImageEvent_Zoom);
			}
		}
	}

	private void AddBGImageEvent(BGImageEvent bgImageEvent)
	{
		if (bgImageEvent != null)
		{
			if (m_bgImageEvents == null)
			{
				m_bgImageEvents = new List<BGImageEvent>();
			}
			m_bgImageEvents.Add(bgImageEvent);
		}
	}

	public bool IsCompleteBGImageEvent_Move()
	{
		return !IsExistBGImageEvent(BGImageEvent.EventType.Move);
	}

	public bool IsCompleteBGImageEvent_Rotate()
	{
		return !IsExistBGImageEvent(BGImageEvent.EventType.Rotate);
	}

	public bool IsCompleteBGImageEvent_Zoom()
	{
		return !IsExistBGImageEvent(BGImageEvent.EventType.Zoom);
	}

	private bool IsExistBGImageEvent(BGImageEvent.EventType eventType)
	{
		if (m_bgImageEvents == null || m_bgImageEvents.Count <= 0)
		{
			return false;
		}
		BGImageEvent bGImageEvent = null;
		int num = m_bgImageEvents.Count;
		while (num > 0)
		{
			num--;
			bGImageEvent = m_bgImageEvents[num];
			if (bGImageEvent == null || bGImageEvent.eventType != eventType)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	private void UpdateBGImageEvent()
	{
		if (m_bgImageEvents == null || m_bgImageEvents.Count <= 0)
		{
			return;
		}
		int count = m_bgImageEvents.Count;
		BGImageEvent bGImageEvent = null;
		int num = m_bgImageEvents.Count;
		while (num > 0)
		{
			num--;
			bGImageEvent = m_bgImageEvents[num];
			if (bGImageEvent != null && bGImageEvent.ProcEvent())
			{
				m_bgImageEvents.Remove(bGImageEvent);
			}
		}
	}
}
