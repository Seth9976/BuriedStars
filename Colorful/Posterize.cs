using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/posterize.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/Posterize")]
public class Posterize : BaseEffect
{
	[Range(2f, 255f)]
	[Tooltip("Number of tonal levels (brightness values) for each channel.")]
	public int Levels = 16;

	[Range(0f, 1f)]
	[Tooltip("Blending factor.")]
	public float Amount = 1f;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetVector("_Params", new Vector2(Levels, Amount));
		Graphics.Blit(source, destination, base.Material);
	}
}
