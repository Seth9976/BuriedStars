using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/gradient-ramp.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/Gradient Ramp")]
public class GradientRamp : BaseEffect
{
	[Tooltip("Texture used to remap the pixels luminosity.")]
	public Texture RampTexture;

	[Range(0f, 1f)]
	[Tooltip("Blending factor.")]
	public float Amount = 1f;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (RampTexture == null || Amount <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		base.Material.SetTexture("_RampTex", RampTexture);
		base.Material.SetFloat("_Amount", Amount);
		Graphics.Blit(source, destination, base.Material);
	}
}
