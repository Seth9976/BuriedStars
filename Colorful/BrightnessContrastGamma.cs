using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/brightness-contrast-gamma.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/Brightness, Contrast, Gamma")]
public class BrightnessContrastGamma : BaseEffect
{
	[Range(-100f, 100f)]
	[Tooltip("Moving the slider to the right increases tonal values and expands highlights, to the left decreases values and expands shadows.")]
	public float Brightness;

	[Range(-100f, 100f)]
	[Tooltip("Expands or shrinks the overall range of tonal values.")]
	public float Contrast;

	public Vector3 ContrastCoeff = new Vector3(0.5f, 0.5f, 0.5f);

	[Range(0.1f, 9.9f)]
	[Tooltip("Simple power function.")]
	public float Gamma = 1f;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Brightness == 0f && Contrast == 0f && Gamma == 1f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		base.Material.SetVector("_BCG", new Vector4((Brightness + 100f) * 0.01f, (Contrast + 100f) * 0.01f, 1f / Gamma));
		base.Material.SetVector("_Coeffs", ContrastCoeff);
		Graphics.Blit(source, destination, base.Material);
	}
}
