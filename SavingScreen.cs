using System.Collections;
using UnityEngine;

public class SavingScreen : MonoBehaviour
{
	public Animator m_BaseAnimator;

	private static SavingScreen s_Instance;

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_SavingScreen";

	private static GameObject s_SrcObject;

	private static GameDefine.EventProc s_fpClosed;

	public static SavingScreen GetCurrentInstance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
	}

	private void OnDestroy()
	{
		s_SrcObject = null;
		if (s_fpClosed != null)
		{
			s_fpClosed(null, null);
		}
		s_Instance = null;
	}

	private void Start()
	{
		if (m_BaseAnimator != null)
		{
			m_BaseAnimator.Play(GameDefine.UIAnimationState.appear.ToString());
		}
	}

	private void Update()
	{
		if (m_BaseAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_BaseAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public static void Show()
	{
		if (!(s_SrcObject == null) && !(s_Instance != null))
		{
			GameObject gameObject = Object.Instantiate(s_SrcObject);
			SavingScreen component = gameObject.GetComponent<SavingScreen>();
			Transform component2 = gameObject.GetComponent<Transform>();
			component2.position = Vector3.zero;
			component2.rotation = Quaternion.identity;
			component2.localScale = Vector3.one;
			gameObject.SetActive(value: true);
		}
	}

	public static void Close(GameDefine.EventProc fpComplete = null)
	{
		if (!(s_Instance == null))
		{
			s_fpClosed = ((fpComplete == null) ? null : new GameDefine.EventProc(fpComplete.Invoke));
			GameGlobalUtil.PlayUIAnimation_WithChidren(s_Instance.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		}
	}

	public static IEnumerator LoadAssetObject()
	{
		if (!(s_SrcObject != null))
		{
			s_SrcObject = MainLoadThing.instance.m_prefabSavingScreen as GameObject;
			s_SrcObject.SetActive(value: false);
		}
		yield break;
	}
}
