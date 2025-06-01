using UnityEngine;
using UnityEngine.UI;

namespace BlendModes;

[AddComponentMenu("Effects/Blend Mode")]
[ExecuteInEditMode]
public class BlendModeEffect : MonoBehaviour
{
	private const int DEFAULT_STENCIL_REF = 8;

	[HideInInspector]
	public Material OriginalMaterial;

	[SerializeField]
	private BlendMode _blendMode;

	[SerializeField]
	private RenderMode _renderMode;

	[SerializeField]
	private Texture _texture;

	[SerializeField]
	private Color _tintColor = Color.white;

	[SerializeField]
	private MaskMode _maskMode;

	[SerializeField]
	private MaskBehaviour _maskBehaviour;

	[SerializeField]
	private bool _enableAutoMask;

	public BlendMode BlendMode
	{
		get
		{
			return _blendMode;
		}
		set
		{
			SetBlendMode(value, RenderMode);
		}
	}

	public RenderMode RenderMode
	{
		get
		{
			return _renderMode;
		}
		set
		{
			SetBlendMode(BlendMode, value);
		}
	}

	public Texture Texture
	{
		get
		{
			return _texture;
		}
		set
		{
			_texture = value;
			SetupTexture();
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
			SetupTintColor();
		}
	}

	public MaskMode MaskMode
	{
		get
		{
			return _maskMode;
		}
		set
		{
			_maskMode = value;
			SetupStencil();
		}
	}

	public MaskBehaviour MaskBehaviour
	{
		get
		{
			return _maskBehaviour;
		}
		set
		{
			SetupStencil();
			_maskBehaviour = value;
		}
	}

	public bool EnableAutoMask
	{
		get
		{
			return _enableAutoMask;
		}
		set
		{
			SetupStencil();
			_enableAutoMask = value;
		}
	}

	public ObjectType ObjectType
	{
		get
		{
			if ((bool)GetComponent<Text>())
			{
				return ObjectType.UIDefaultFont;
			}
			if ((bool)GetComponent<MaskableGraphic>())
			{
				return ObjectType.UIDefault;
			}
			if ((bool)GetComponent<SpriteRenderer>())
			{
				return ObjectType.SpriteDefault;
			}
			if ((bool)GetComponent<MeshRenderer>())
			{
				return ObjectType.MeshDefault;
			}
			if ((bool)GetComponent<ParticleSystem>())
			{
				return ObjectType.ParticleDefault;
			}
			return ObjectType.Unknown;
		}
	}

	public Material Material
	{
		get
		{
			return ObjectType switch
			{
				ObjectType.UIDefault => GetComponent<MaskableGraphic>().material, 
				ObjectType.UIDefaultFont => GetComponent<Text>().material, 
				ObjectType.SpriteDefault => GetComponent<SpriteRenderer>().sharedMaterial, 
				ObjectType.MeshDefault => GetComponent<MeshRenderer>().sharedMaterial, 
				ObjectType.ParticleDefault => GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial, 
				_ => null, 
			};
		}
		set
		{
			switch (ObjectType)
			{
			case ObjectType.UIDefault:
				GetComponent<MaskableGraphic>().material = value;
				break;
			case ObjectType.UIDefaultFont:
				GetComponent<Text>().material = value;
				break;
			case ObjectType.SpriteDefault:
				GetComponent<SpriteRenderer>().sharedMaterial = value;
				break;
			case ObjectType.MeshDefault:
				GetComponent<MeshRenderer>().sharedMaterial = value;
				break;
			case ObjectType.ParticleDefault:
				GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial = value;
				break;
			}
		}
	}

	public void SetBlendMode(BlendMode blendMode, RenderMode renderMode = RenderMode.Grab)
	{
		_blendMode = blendMode;
		_renderMode = renderMode;
		if (ObjectType != ObjectType.Unknown)
		{
			Material = BlendMaterials.GetMaterial(ObjectType, renderMode, blendMode);
			SetupTexture();
			SetupTintColor();
			SetupStencil();
		}
	}

	public void OnEnable()
	{
		if ((bool)GetComponent<Camera>())
		{
			base.gameObject.AddComponent<CameraOverlay>();
			if (Application.isEditor)
			{
				Object.DestroyImmediate(this);
			}
			else
			{
				Object.Destroy(this);
			}
		}
		else
		{
			SetBlendMode(BlendMode, RenderMode);
		}
	}

	public void OnDisable()
	{
		Texture texture = Texture;
		Material = OriginalMaterial;
		if ((bool)Material && (bool)texture)
		{
			Material.mainTexture = texture;
		}
	}

	private void SetupTintColor()
	{
		if ((bool)Material && (ObjectType == ObjectType.MeshDefault || ObjectType == ObjectType.ParticleDefault))
		{
			Material.color = TintColor;
		}
	}

	private void SetupTexture()
	{
		if ((bool)Material && (ObjectType == ObjectType.MeshDefault || ObjectType == ObjectType.ParticleDefault))
		{
			Material.mainTexture = Texture;
		}
	}

	private void SetupStencil()
	{
		if (!Material)
		{
			return;
		}
		Material.SetFloat("_StencilRef", 8f);
		float num = -1f;
		float value = -1f;
		float value2 = ((!EnableAutoMask) ? 1 : 0);
		if (BlendMode == BlendMode.Normal)
		{
			num = 1f;
			value = 0f;
		}
		else
		{
			num = ((MaskMode != MaskMode.Disabled) ? ((MaskMode != MaskMode.NothingButMask) ? 6 : 3) : 0);
			if (MaskMode == MaskMode.Disabled)
			{
				value = 1f;
			}
			else if (MaskBehaviour == MaskBehaviour.Cutout)
			{
				value = 1f;
			}
			else if (MaskMode == MaskMode.NothingButMask)
			{
				value = 6f;
			}
			else if (MaskMode == MaskMode.EverythingButMask)
			{
				value = 3f;
			}
		}
		Material.SetFloat("_BlendStencilComp", num);
		Material.SetFloat("_NormalStencilComp", value);
		Material.SetFloat("_MaskStencilComp", value2);
	}
}
