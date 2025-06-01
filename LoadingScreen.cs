using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	public enum Type
	{
		Random = -1,
		Type0,
		Type1,
		Type2,
		Count
	}

	public Animator m_BaseAnimator;

	private Type m_Type;

	public Text m_textLoadingPercent;

	public GameObject m_goLoadingPercentGauge;

	public Image m_imgLoadingPercentGauge;

	private static LoadingScreen s_Instance;

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_LoadingScreen";

	private static GameObject s_SrcObject;

	private static GameDefine.EventProc s_fpClosed;

	private static Type s_PrevType = Type.Random;

	private static int m_iLoadingPercent;

	public static LoadingScreen GetCurrentInstance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
	}

	private void OnDestroy()
	{
		s_Instance = null;
		if (s_fpClosed != null)
		{
			s_fpClosed(null, null);
		}
	}

	private void Start()
	{
		if (m_BaseAnimator != null)
		{
			m_BaseAnimator.SetInteger("Type", (int)m_Type);
		}
	}

	private void OnEnable()
	{
		m_iLoadingPercent = 0;
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

	public static void Show(Type type = Type.Random)
	{
		if (s_SrcObject == null)
		{
			return;
		}
		GameObject gameObject = null;
		if (s_Instance == null)
		{
			gameObject = Object.Instantiate(s_SrcObject);
			Object.DontDestroyOnLoad(gameObject);
		}
		else
		{
			gameObject = s_Instance.gameObject;
		}
		LoadingScreen component = gameObject.GetComponent<LoadingScreen>();
		if (type == Type.Random)
		{
			int max = 3;
			int num = 0;
			do
			{
				num = Random.Range(0, max);
				type = (Type)num;
			}
			while (type == s_PrevType);
		}
		component.m_Type = type;
		s_PrevType = type;
		Transform component2 = gameObject.GetComponent<Transform>();
		component2.position = Vector3.zero;
		component2.rotation = Quaternion.identity;
		component2.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		GameGlobalUtil.PlayUIAnimation_WithChidren(s_Instance.gameObject, GameDefine.UIAnimationState.appear.ToString());
	}

	public static void Close(GameDefine.EventProc fpComplete = null)
	{
		if (!(s_Instance == null))
		{
			ActiveLoadingPercent(isOn: false);
			s_fpClosed = ((fpComplete == null) ? null : new GameDefine.EventProc(fpComplete.Invoke));
			s_Instance.gameObject.SetActive(value: false);
			Object.Destroy(s_Instance.gameObject);
		}
	}

	public static IEnumerator LoadAssetObject(GameDefine.EventProc fpComplete = null)
	{
		if (!(s_SrcObject != null))
		{
			s_SrcObject = MainLoadThing.instance.m_prefabLoadingScreen as GameObject;
			s_SrcObject.SetActive(value: false);
			fpComplete?.Invoke(null, null);
		}
		yield break;
	}

	public static void ActiveLoadingPercent(bool isOn)
	{
		if (!(s_Instance == null) && !(s_Instance.m_textLoadingPercent == null) && !(s_Instance.m_goLoadingPercentGauge == null) && !(s_Instance.m_imgLoadingPercentGauge == null))
		{
			SetLoadingPercent(0);
			s_Instance.m_textLoadingPercent.gameObject.SetActive(isOn);
			s_Instance.m_goLoadingPercentGauge.SetActive(isOn);
		}
	}

	public static void SetLoadingPercent(int iPercent)
	{
		if (!(s_Instance == null) && !(s_Instance.m_textLoadingPercent == null) && !(s_Instance.m_goLoadingPercentGauge == null) && !(s_Instance.m_imgLoadingPercentGauge == null))
		{
			if (iPercent < 0)
			{
				iPercent = 0;
			}
			if (iPercent > 100)
			{
				iPercent = 100;
			}
			m_iLoadingPercent = iPercent;
			s_Instance.m_textLoadingPercent.text = iPercent + "%";
			s_Instance.m_imgLoadingPercentGauge.fillAmount = (float)iPercent / 100f;
		}
	}
}
