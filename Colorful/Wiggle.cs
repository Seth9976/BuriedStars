using UnityEngine;

namespace Colorful;

[HelpURL("http://www.thomashourdel.com/colorful/doc/camera-effects/wiggle.html")]
[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Camera Effects/Wiggle")]
public class Wiggle : BaseEffect
{
	public enum Algorithm
	{
		Simple,
		Complex
	}

	[Tooltip("Animation type. Complex is slower but looks more natural.")]
	public Algorithm Mode = Algorithm.Complex;

	public float Timer;

	[Tooltip("Wave animation speed.")]
	public float Speed = 1f;

	[Tooltip("Wave frequency (higher means more waves).")]
	public float Frequency = 12f;

	[Tooltip("Wave amplitude (higher means bigger waves).")]
	public float Amplitude = 0.01f;

	[Tooltip("Automatically animate this effect at runtime.")]
	public bool AutomaticTimer = true;

	protected virtual void Update()
	{
		if (AutomaticTimer)
		{
			if (Timer > 1000f)
			{
				Timer -= 1000f;
			}
			Timer += Speed * Time.deltaTime;
		}
	}

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetVector("_Params", new Vector3(Frequency, Amplitude, Timer * ((Mode != Algorithm.Complex) ? 1f : 0.1f)));
		Graphics.Blit(source, destination, base.Material, (int)Mode);
	}
}
