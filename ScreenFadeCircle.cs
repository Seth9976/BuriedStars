using Colorful;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Largo Custom/Screen Fade Circle")]
public class ScreenFadeCircle : BaseEffect
{
	[Range(0f, 1f)]
	public float Red;

	[Range(0f, 1f)]
	public float Green;

	[Range(0f, 1f)]
	public float Blue;

	[Range(0f, 1f)]
	public float Alpha = 1f;

	[Range(0f, 1f)]
	public float CenterX = 0.5f;

	[Range(0f, 1f)]
	public float CenterY = 0.5f;

	[Range(0f, 1f)]
	public float Radius = 0.1f;

	private float m_fCenterScreenX;

	private float m_fCenterScreenY;

	private float m_fMaxLength;

	private void Awake()
	{
		Shader = Shader.Find("Hidden/LargoCustom/ScreenFadeCircle");
	}

	protected override void Start()
	{
		base.Start();
		m_fCenterScreenX = (float)Screen.width * CenterX;
		m_fCenterScreenY = (float)Screen.height * CenterY;
		float num = ((!(CenterX < 0.5f)) ? CenterX : (1f - CenterX));
		float num2 = ((!(CenterY < 0.5f)) ? CenterY : (1f - CenterY));
		num = (float)Screen.width * num;
		num2 = (float)Screen.height * num2;
		m_fMaxLength = num * num + num2 * num2;
		m_fMaxLength = Mathf.Sqrt(m_fMaxLength);
	}

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Alpha <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		float value = m_fMaxLength * Radius;
		base.Material.SetVector("_Center", new Vector4(m_fCenterScreenX, m_fCenterScreenY, 0f, 0f));
		base.Material.SetVector("_Color", new Vector4(Red, Green, Blue, Alpha));
		base.Material.SetFloat("_Radius", value);
		Graphics.Blit(source, destination, base.Material);
	}
}
