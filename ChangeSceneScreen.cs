using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSceneScreen : MonoBehaviour
{
	public Animator m_BaseAnimator;

	public Text m_textSwitchChannel;

	private static ChangeSceneScreen s_Instance;

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_ChangeSceneScreen";

	private static GameObject s_SrcObject;

	private static GameDefine.EventProc s_fpCompleteAppear;

	private static GameDefine.EventProc s_fpClosed;

	public static ChangeSceneScreen GetCurrentInstance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
	}

	private void OnEnable()
	{
		m_textSwitchChannel.text = GameGlobalUtil.GetXlsProgramText("CHANGE_SCENE_BG_TEXT");
		FontManager.ResetTextFontByCurrentLanguage(m_textSwitchChannel);
	}

	private void OnDestroy()
	{
		s_Instance = null;
		if (s_fpClosed != null)
		{
			s_fpClosed(null, null);
			s_fpClosed = null;
		}
	}

	private void Update()
	{
		if (m_BaseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_BaseAnimator.GetCurrentAnimatorStateInfo(0);
			if (s_fpCompleteAppear != null && ((currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.appear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f) || currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.idle.ToString())))
			{
				s_fpCompleteAppear(null, null);
				s_fpCompleteAppear = null;
			}
			else if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public static IEnumerator Show(GameDefine.EventProc fpCompleteAppear = null, MonoBehaviour parentMonoBehaviour = null)
	{
		GameObject gameObject = null;
		if (s_Instance == null)
		{
			if (parentMonoBehaviour == null)
			{
				parentMonoBehaviour = MainLoadThing.instance;
			}
			yield return parentMonoBehaviour.StartCoroutine(LoadAssetObject(parentMonoBehaviour));
			gameObject = Object.Instantiate(s_SrcObject);
			Object.DontDestroyOnLoad(gameObject);
		}
		else
		{
			gameObject = s_Instance.gameObject;
		}
		s_fpCompleteAppear = ((fpCompleteAppear == null) ? null : new GameDefine.EventProc(fpCompleteAppear.Invoke));
		Transform tr = gameObject.GetComponent<Transform>();
		tr.position = Vector3.zero;
		tr.rotation = Quaternion.identity;
		tr.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation_WithChidren(s_Instance.gameObject, GameDefine.UIAnimationState.appear.ToString());
	}

	public static void Close(GameDefine.EventProc fpComplete = null)
	{
		if (!(s_Instance == null))
		{
			s_fpClosed = ((fpComplete == null) ? null : new GameDefine.EventProc(fpComplete.Invoke));
			GameGlobalUtil.PlayUIAnimation_WithChidren(s_Instance.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		}
	}

	public static IEnumerator LoadAssetObject(MonoBehaviour parentMonoBehaviour = null)
	{
		if (!(s_SrcObject != null))
		{
			s_SrcObject = MainLoadThing.instance.m_prefabChangeSceneScreen as GameObject;
			s_SrcObject.SetActive(value: false);
		}
		yield break;
	}
}
