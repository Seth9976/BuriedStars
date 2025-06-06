using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/gradient-ramp-dynamic.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/Gradient Ramp (Dynamic)")]
public class GradientRampDynamic : BaseEffect
{
	[Tooltip("Gradient used to remap the pixels luminosity.")]
	public Gradient Ramp;

	[Range(0f, 1f)]
	[Tooltip("Blending factor.")]
	public float Amount = 1f;

	protected Texture2D m_RampTexture;

	protected override void Start()
	{
		base.Start();
		if (Ramp != null)
		{
			UpdateGradientCache();
		}
	}

	protected virtual void Reset()
	{
		Ramp = new Gradient();
		Ramp.colorKeys = new GradientColorKey[2]
		{
			new GradientColorKey(Color.black, 0f),
			new GradientColorKey(Color.white, 1f)
		};
		Ramp.alphaKeys = new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		};
		UpdateGradientCache();
	}

	public void UpdateGradientCache()
	{
		if (m_RampTexture == null)
		{
			m_RampTexture = new Texture2D(256, 1, TextureFormat.RGB24, mipmap: false);
			m_RampTexture.filterMode = FilterMode.Bilinear;
			m_RampTexture.wrapMode = TextureWrapMode.Clamp;
			m_RampTexture.hideFlags = HideFlags.HideAndDontSave;
		}
		Color[] array = new Color[256];
		for (int i = 0; i < 256; i++)
		{
			ref Color reference = ref array[i];
			reference = Ramp.Evaluate((float)i / 255f);
		}
		m_RampTexture.SetPixels(array);
		m_RampTexture.Apply();
	}

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Ramp == null || Amount <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		base.Material.SetTexture("_RampTex", m_RampTexture);
		base.Material.SetFloat("_Amount", Amount);
		Graphics.Blit(source, destination, base.Material);
	}
}
