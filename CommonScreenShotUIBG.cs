using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[AddComponentMenu("UI/Screen Shot UI BG", 10)]
public class CommonScreenShotUIBG : MonoBehaviour
{
	[Range(0.1f, 2f)]
	public float m_ResolutionRate = 1f;

	public bool m_EnableScreenShot = true;

	public bool m_EnableBlur = true;

	public bool m_OnlyBGLayer;

	private Image m_Image;

	private void OnEnable()
	{
		if (!m_EnableScreenShot)
		{
			RenderManager.instance.DeactivateCamera();
			return;
		}
		GetImageComponent();
		if (m_Image == null)
		{
			return;
		}
		int width = Mathf.RoundToInt((float)Screen.width * m_ResolutionRate);
		int height = Mathf.RoundToInt((float)Screen.height * m_ResolutionRate);
		Sprite sprite = RenderManager.instance.RenderToSprite(width, height, m_EnableBlur, m_OnlyBGLayer);
		m_Image.sprite = sprite;
		if (sprite != null)
		{
			m_Image.color = Color.white;
			base.gameObject.SetActive(value: true);
			if (!m_OnlyBGLayer)
			{
				RenderManager.instance.DeactivateCamera();
			}
			else
			{
				RenderManager.instance.DeactivateBGCamera();
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		if (!m_OnlyBGLayer)
		{
			RenderManager.instance.ActivateCamera();
		}
		else
		{
			RenderManager.instance.ActivateBGCamera();
		}
		if (m_EnableScreenShot && m_Image.sprite != null)
		{
			UnityEngine.Object.DestroyImmediate(m_Image.sprite);
			m_Image.sprite = null;
			m_Image.color = new Color(1f, 1f, 1f, 0f);
		}
	}

	private void OnDestroy()
	{
		m_Image = null;
	}

	private void GetImageComponent()
	{
		if (!(m_Image != null))
		{
			m_Image = base.gameObject.GetComponent<Image>();
			if (!(m_Image == null))
			{
			}
		}
	}
}
