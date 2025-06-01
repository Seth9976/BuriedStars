using System;
using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/color-correction/white-balance.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Color Correction/White Balance")]
public class WhiteBalance : BaseEffect
{
	public enum BalanceMode
	{
		Simple,
		Complex
	}

	[ColorUsage(false)]
	[Tooltip("Reference white point or midtone value.")]
	public Color White = new Color(0.5f, 0.5f, 0.5f);

	[Tooltip("Algorithm used.")]
	public BalanceMode Mode = BalanceMode.Complex;

	protected virtual void Reset()
	{
		White = ((!CLib.IsLinearColorSpace()) ? new Color(0.5f, 0.5f, 0.5f) : new Color((float)Math.PI * 59f / 254f, (float)Math.PI * 59f / 254f, (float)Math.PI * 59f / 254f));
	}

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetColor("_White", White);
		Graphics.Blit(source, destination, base.Material, (int)Mode);
	}
}
