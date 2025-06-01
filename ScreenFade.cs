using Colorful;
using GameEvent;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful FX/Largo Custom/Screen Fade")]
public class ScreenFade : BaseEffect
{
	[Range(0f, 1f)]
	public float Red;

	[Range(0f, 1f)]
	public float Green;

	[Range(0f, 1f)]
	public float Blue;

	[Range(0f, 1f)]
	public float Amount = 0.5f;

	[HideInInspector]
	public EventCameraEffect.Effects UsedEffType = EventCameraEffect.Effects.Cover;

	private void Awake()
	{
		Shader = Shader.Find("Hidden/LargoCustom/ScreenFade");
	}

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Amount <= 0f)
		{
			Graphics.Blit(source, destination);
			return;
		}
		base.Material.SetVector("_Params", new Vector4(Red, Green, Blue, Amount));
		Graphics.Blit(source, destination, base.Material);
	}
}
