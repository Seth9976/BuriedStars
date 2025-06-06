using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/other-effects/pixel-matrix.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Other Effects/Pixel Matrix")]
public class PixelMatrix : BaseEffect
{
	[Min(3f)]
	[Tooltip("Tile size. Works best with multiples of 3.")]
	public int Size = 9;

	[Range(0f, 10f)]
	[Tooltip("Tile brightness booster.")]
	public float Brightness = 1.4f;

	[Tooltip("Show / hide black borders on every tile.")]
	public bool BlackBorder = true;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetVector("_Params", new Vector4(Size, Mathf.Floor((float)Size / 3f), (float)Size - Mathf.Floor((float)Size / 3f), Brightness));
		Graphics.Blit(source, destination, base.Material, BlackBorder ? 1 : 0);
	}
}
