using System.Collections;
using UnityEngine;

public class LoadingSWatchIcon : MonoBehaviour
{
	private RectTransform m_RectTransform;

	private Animator m_DisappearAnimator;

	private GameDefine.EventProc m_fpClosed;

	private const string c_PrefabAssetPath = "Prefabs/InGame/Menu/UI_SWatchLoadingIcon";

	private static GameObject s_SrcObject;

	public RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	private void OnDestroy()
	{
		m_RectTransform = null;
		m_DisappearAnimator = null;
		m_fpClosed = null;
	}

	private void Update()
	{
		if (!(m_DisappearAnimator != null))
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = m_DisappearAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			Object.Destroy(base.gameObject);
			if (m_fpClosed != null)
			{
				m_fpClosed(null, null);
			}
		}
	}

	public void Disappear(GameDefine.EventProc fpComplete = null)
	{
		if (!(m_DisappearAnimator != null))
		{
			m_fpClosed = ((fpComplete == null) ? null : new GameDefine.EventProc(fpComplete.Invoke));
			m_DisappearAnimator = GameGlobalUtil.PlayUIAnimation_WithChidren(base.gameObject, GameDefine.UIAnimationState.disappear.ToString());
		}
	}

	public static LoadingSWatchIcon Create(GameObject baseGameObject, float fAlignH = 0.5f, float fAlignV = 0.5f)
	{
		if (s_SrcObject == null)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(s_SrcObject);
		LoadingSWatchIcon component = gameObject.GetComponent<LoadingSWatchIcon>();
		RectTransform rectTransform = component.rectTransform;
		if (baseGameObject != null)
		{
			RectTransform component2 = baseGameObject.GetComponent<RectTransform>();
			if (component2 != null)
			{
				rectTransform.SetParent(component2);
				float x = component2.rect.width * fAlignH + component2.rect.xMin;
				float y = component2.rect.height * fAlignV + component2.rect.yMin;
				rectTransform.anchoredPosition = new Vector2(x, y);
			}
			else
			{
				Transform component3 = baseGameObject.GetComponent<Transform>();
				rectTransform.SetParent(component3);
			}
		}
		rectTransform.rotation = Quaternion.identity;
		rectTransform.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		return component;
	}

	public static IEnumerator LoadAssetObject()
	{
		if (!(s_SrcObject != null))
		{
			s_SrcObject = MainLoadThing.instance.m_prefabLoadingSWatchIcon as GameObject;
			s_SrcObject.SetActive(value: false);
		}
		yield break;
	}
}
