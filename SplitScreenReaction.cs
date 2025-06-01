using System;
using System.Collections;
using System.Collections.Generic;
using GameEvent;
using UnityEngine;
using UnityEngine.UI;

public class SplitScreenReaction : MonoBehaviour
{
	[Serializable]
	public class MotionToAsset
	{
		public string m_MotionName = string.Empty;

		public Sprite m_SpriteAsset;
	}

	[Serializable]
	public class SceneToAsset
	{
		public string m_SceneName = string.Empty;

		public Sprite m_SpriteAsset;
	}

	public Canvas m_Canvas;

	public Image m_BGImage;

	public Image m_HanImage;

	public MotionToAsset[] m_HanMotionDatas;

	public SceneToAsset[] m_SceneBGDatas;

	private SortedDictionary<string, Sprite> m_dicMotionDatas = new SortedDictionary<string, Sprite>();

	private SortedDictionary<string, Sprite> m_dicBGDatas = new SortedDictionary<string, Sprite>();

	private Animator m_BaseAnimator;

	private static SplitScreenReaction s_Instance;

	private static bool s_isNeedInitMembers;

	private static int s_CanvasOrder;

	private static string s_StockedSceneName = string.Empty;

	private static string s_StockedMotionName = string.Empty;

	private static GameDefine.EventProc s_fpCompleteCB;

	private static bool s_isEventComplete = true;

	private const string s_PrefabAssetPath = "Prefabs/InGame/Game/UI_SplitReaction";

	private static GameObject s_SrcObject;

	public static SplitScreenReaction instance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
		m_dicMotionDatas.Clear();
		if (m_HanMotionDatas != null && m_HanMotionDatas.Length > 0)
		{
			MotionToAsset[] hanMotionDatas = m_HanMotionDatas;
			foreach (MotionToAsset motionToAsset in hanMotionDatas)
			{
				if (motionToAsset != null && !(motionToAsset.m_SpriteAsset == null) && !string.IsNullOrEmpty(motionToAsset.m_MotionName) && !m_dicMotionDatas.ContainsKey(motionToAsset.m_MotionName))
				{
					m_dicMotionDatas.Add(motionToAsset.m_MotionName, motionToAsset.m_SpriteAsset);
				}
			}
		}
		m_dicBGDatas.Clear();
		if (m_SceneBGDatas == null || m_SceneBGDatas.Length <= 0)
		{
			return;
		}
		SceneToAsset[] sceneBGDatas = m_SceneBGDatas;
		foreach (SceneToAsset sceneToAsset in sceneBGDatas)
		{
			if (sceneToAsset != null && !(sceneToAsset.m_SpriteAsset == null) && !string.IsNullOrEmpty(sceneToAsset.m_SceneName) && !m_dicBGDatas.ContainsKey(sceneToAsset.m_SceneName))
			{
				m_dicBGDatas.Add(sceneToAsset.m_SceneName, sceneToAsset.m_SpriteAsset);
			}
		}
	}

	private void OnDestroy()
	{
		if (m_dicMotionDatas != null)
		{
			m_dicMotionDatas.Clear();
		}
		if (m_dicBGDatas != null)
		{
			m_dicBGDatas.Clear();
		}
		m_BaseAnimator = null;
		s_fpCompleteCB = null;
		s_SrcObject = null;
		s_Instance = null;
	}

	private void Start()
	{
		if (s_isNeedInitMembers)
		{
			InitMembers(s_StockedSceneName, s_StockedMotionName, s_CanvasOrder);
		}
	}

	private void Update()
	{
		if (!(m_BaseAnimator != null))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_BaseAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.disappear)) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			base.gameObject.SetActive(value: false);
			s_isEventComplete = true;
			if (s_fpCompleteCB != null)
			{
				s_fpCompleteCB(null, null);
				s_fpCompleteCB = null;
			}
		}
		else
		{
			if (currentAnimatorStateInfo.IsName(GameDefine.GetAnimationStateName(GameDefine.UIAnimationState.idle)) && GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton))
			{
				m_BaseAnimator.Play(GameDefine.UIAnimationState.disappear.ToString());
			}
			m_BaseAnimator.speed = ((!EventEngine.GetInstance().GetSkip()) ? 1f : EventEngine.GetInstance().GetLerpSkipValue());
		}
	}

	private void InitMembers(string sceneName, string motionName, int canvasOrder)
	{
		m_BGImage.sprite = ((!m_dicBGDatas.ContainsKey(sceneName)) ? null : m_dicBGDatas[sceneName]);
		m_HanImage.sprite = ((!m_dicMotionDatas.ContainsKey(motionName)) ? null : m_dicMotionDatas[motionName]);
		m_Canvas.sortingOrder = canvasOrder;
		m_BaseAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.appear.ToString());
		s_isNeedInitMembers = false;
	}

	public static IEnumerator PreloadFromAsset()
	{
		if (!(s_Instance != null))
		{
			if (s_SrcObject == null)
			{
				s_SrcObject = GameMain.instance.m_prefabSplitReaction as GameObject;
			}
			GameObject instanceObj = UnityEngine.Object.Instantiate(s_SrcObject);
			if (s_Instance == null)
			{
				s_Instance = instanceObj.GetComponentInChildren<SplitScreenReaction>();
			}
			instanceObj.SetActive(value: false);
			s_isNeedInitMembers = true;
			yield return null;
		}
	}

	public static void UnloadLoadedAssetBundle()
	{
		s_SrcObject = null;
		Destory();
	}

	public static IEnumerator Show(string motionName, GameDefine.EventProc fpCompleteCB = null, int canvasOrder = 8)
	{
		string sceneName = EventEngine.m_strLoadedLevel;
		yield return Show(sceneName, motionName, fpCompleteCB, canvasOrder);
	}

	public static IEnumerator Show(string sceneName, string motionName, GameDefine.EventProc fpCompleteCB = null, int canvasOrder = 8)
	{
		s_fpCompleteCB = ((fpCompleteCB == null) ? null : new GameDefine.EventProc(fpCompleteCB.Invoke));
		s_isEventComplete = false;
		if (s_Instance == null)
		{
			yield return GameMain.instance.StartCoroutine(PreloadFromAsset());
		}
		if (s_isNeedInitMembers)
		{
			s_CanvasOrder = canvasOrder;
			s_StockedSceneName = sceneName;
			s_StockedMotionName = motionName;
			s_Instance.gameObject.SetActive(value: true);
		}
		else
		{
			s_Instance.gameObject.SetActive(value: true);
			s_Instance.InitMembers(sceneName, motionName, canvasOrder);
			yield return null;
		}
	}

	public static void Destory()
	{
		if (!(s_Instance == null))
		{
			UnityEngine.Object.DestroyImmediate(s_Instance.gameObject);
			s_isEventComplete = true;
		}
	}

	public static bool IsEventComplete()
	{
		return s_isEventComplete;
	}
}
