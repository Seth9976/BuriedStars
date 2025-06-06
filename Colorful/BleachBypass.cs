using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/bleach-bypass.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/Bleach Bypass")]
public class BleachBypass : BaseEffect
{
	[Range(0f, 1f)]
	[Tooltip("Blending factor.")]
	public float Amount = 1f;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Amount <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		base.Material.SetFloat("_Amount", Amount);
		Graphics.Blit(source, destination, base.Material);
	}
}
