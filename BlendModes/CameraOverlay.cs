using UnityEngine;

namespace BlendModes;

[AddComponentMenu("Image Effects/Camera Overlay")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraOverlay : MonoBehaviour
{
	[SerializeField]
	private Shader _overlayShader;

	[SerializeField]
	private Material _overlayMaterial;

	[SerializeField]
	private BlendMode _blendMode;

	[SerializeField]
	private Texture2D _overlayTexture;

	[SerializeField]
	private Color _tintColor = Color.white;

	private bool isSupported = true;

	private Vector4 screenUv = new Vector4(1f, 0f, 0f, 1f);

	public BlendMode BlendMode
	{
		get
		{
			return _blendMode;
		}
		set
		{
			SetBlendMode(value);
		}
	}

	public Texture2D OverlayTexture
	{
		get
		{
			return _overlayTexture;
		}
		set
		{
			_overlayTexture = value;
		}
	}

	public Color TintColor
	{
		get
		{
			return _tintColor;
		}
		set
		{
			_tintColor = value;
		}
	}

	public Material OverlayMaterial
	{
		get
		{
			if (!_overlayMaterial)
			{
				ObjectType objectType = ObjectType.ScreenOverlay;
				Shader overlayShader = OverlayShader;
				_overlayMaterial = BlendMaterials.GetMaterial(objectType, RenderMode.Grab, BlendMode.Normal, overlayShader);
			}
			return _overlayMaterial;
		}
		set
		{
			_overlayMaterial = value;
		}
	}

	public Shader OverlayShader
	{
		get
		{
			if (!_overlayShader)
			{
				_overlayShader = Resources.Load<Shader>("BlendModes/Extra/CameraOverlay");
			}
			return _overlayShader;
		}
		set
		{
			_overlayShader = value;
		}
	}

	public void SetBlendMode(BlendMode blendMode)
	{
		if (blendMode == BlendMode.Normal)
		{
			OverlayMaterial.SetFloat("_IsNormalBlendMode", 1f);
		}
		else
		{
			ObjectType objectType = ObjectType.ScreenOverlay;
			OverlayMaterial = BlendMaterials.GetMaterial(objectType, RenderMode.Grab, blendMode, OverlayShader);
			OverlayMaterial.SetFloat("_IsNormalBlendMode", 0f);
		}
		_blendMode = blendMode;
		_overlayTexture = OverlayTexture;
		_tintColor = TintColor;
	}

	private void OnEnable()
	{
		isSupported = true;
		SetBlendMode(BlendMode);
	}

	private void Start()
	{
		CheckResources();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		OverlayMaterial.SetVector("_UV_Transform", screenUv);
		OverlayMaterial.SetColor("_OverlayTintColor", TintColor);
		OverlayMaterial.SetTexture("_OverlayTex", OverlayTexture);
		Graphics.Blit(source, destination, OverlayMaterial);
	}

	private bool CheckResources()
	{
		isSupported = true;
		if (!SystemInfo.supportsImageEffects)
		{
			SetNotSupported();
			isSupported = false;
		}
		if (!isSupported)
		{
		}
		return isSupported;
	}

	private void SetNotSupported()
	{
		base.enabled = false;
		isSupported = false;
	}
}
