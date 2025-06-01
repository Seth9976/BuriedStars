using System;
using System.Collections.Generic;
using System.Linq;
using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("UI/Soft Mask", 14)]
[RequireComponent(typeof(RectTransform))]
[HelpURL("https://docs.google.com/document/d/1w8ENeeE_wi_DSpCyU34voUJIOk9o3J8gDDAyOja_9yE")]
public class SoftMask : UIBehaviour, ICanvasRaycastFilter, ISoftMask
{
	[Serializable]
	public enum MaskSource
	{
		Graphic,
		Sprite,
		Texture
	}

	[Serializable]
	public enum BorderMode
	{
		Simple,
		Sliced,
		Tiled
	}

	[Serializable]
	[Flags]
	public enum Errors
	{
		NoError = 0,
		UnsupportedShaders = 1,
		NestedMasks = 2,
		TightPackedSprite = 4,
		AlphaSplitSprite = 8,
		UnsupportedImageType = 0x10
	}

	private static class Mathr
	{
		public static Vector4 ToVector(Rect r)
		{
			return new Vector4(r.xMin, r.yMin, r.xMax, r.yMax);
		}

		public static Vector4 Div(Vector4 v, Vector2 s)
		{
			return new Vector4(v.x / s.x, v.y / s.y, v.z / s.x, v.w / s.y);
		}

		public static Vector2 Div(Vector2 v, Vector2 s)
		{
			return new Vector2(v.x / s.x, v.y / s.y);
		}

		public static Vector4 Mul(Vector4 v, Vector2 s)
		{
			return new Vector4(v.x * s.x, v.y * s.y, v.z * s.x, v.w * s.y);
		}

		public static Vector2 Size(Vector4 r)
		{
			return new Vector2(r.z - r.x, r.w - r.y);
		}

		public static Vector4 Move(Vector4 v, Vector2 o)
		{
			return new Vector4(v.x + o.x, v.y + o.y, v.z + o.x, v.w + o.y);
		}

		public static Vector4 BorderOf(Vector4 outer, Vector4 inner)
		{
			return new Vector4(inner.x - outer.x, inner.y - outer.y, outer.z - inner.z, outer.w - inner.w);
		}

		public static Vector4 ApplyBorder(Vector4 v, Vector4 b)
		{
			return new Vector4(v.x + b.x, v.y + b.y, v.z - b.z, v.w - b.w);
		}

		public static Vector2 Min(Vector4 r)
		{
			return new Vector2(r.x, r.y);
		}

		public static Vector2 Max(Vector4 r)
		{
			return new Vector2(r.z, r.w);
		}

		public static Vector2 Remap(Vector2 c, Vector4 r1, Vector4 r2)
		{
			Vector2 s = Max(r1) - Min(r1);
			Vector2 b = Max(r2) - Min(r2);
			return Vector2.Scale(Div(c - Min(r1), s), b) + Min(r2);
		}

		public static bool Inside(Vector2 v, Vector4 r)
		{
			return v.x >= r.x && v.y >= r.y && v.x <= r.z && v.y <= r.w;
		}
	}

	private struct MaterialParameters
	{
		private static class Ids
		{
			public static readonly int SoftMask = Shader.PropertyToID("_SoftMask");

			public static readonly int SoftMask_Rect = Shader.PropertyToID("_SoftMask_Rect");

			public static readonly int SoftMask_UVRect = Shader.PropertyToID("_SoftMask_UVRect");

			public static readonly int SoftMask_ChannelWeights = Shader.PropertyToID("_SoftMask_ChannelWeights");

			public static readonly int SoftMask_WorldToMask = Shader.PropertyToID("_SoftMask_WorldToMask");

			public static readonly int SoftMask_BorderRect = Shader.PropertyToID("_SoftMask_BorderRect");

			public static readonly int SoftMask_UVBorderRect = Shader.PropertyToID("_SoftMask_UVBorderRect");

			public static readonly int SoftMask_TileRepeat = Shader.PropertyToID("_SoftMask_TileRepeat");
		}

		public Vector4 maskRect;

		public Vector4 maskBorder;

		public Vector4 maskRectUV;

		public Vector4 maskBorderUV;

		public Vector2 tileRepeat;

		public Color maskChannelWeights;

		public Matrix4x4 worldToMask;

		public Texture2D texture;

		public BorderMode borderMode;

		public Texture2D activeTexture => (!texture) ? Texture2D.whiteTexture : texture;

		public bool SampleMask(Vector2 localPos, out float mask)
		{
			Vector2 vector = XY2UV(localPos);
			try
			{
				mask = MaskValue(texture.GetPixelBilinear(vector.x, vector.y));
				return true;
			}
			catch (UnityException)
			{
				mask = 0f;
				return false;
			}
		}

		public void Apply(Material mat)
		{
			mat.SetTexture(Ids.SoftMask, activeTexture);
			mat.SetVector(Ids.SoftMask_Rect, maskRect);
			mat.SetVector(Ids.SoftMask_UVRect, maskRectUV);
			mat.SetColor(Ids.SoftMask_ChannelWeights, maskChannelWeights);
			mat.SetMatrix(Ids.SoftMask_WorldToMask, worldToMask);
			mat.EnableKeyword("SOFTMASK_SIMPLE", borderMode == BorderMode.Simple);
			mat.EnableKeyword("SOFTMASK_SLICED", borderMode == BorderMode.Sliced);
			mat.EnableKeyword("SOFTMASK_TILED", borderMode == BorderMode.Tiled);
			if (borderMode != BorderMode.Simple)
			{
				mat.SetVector(Ids.SoftMask_BorderRect, maskBorder);
				mat.SetVector(Ids.SoftMask_UVBorderRect, maskBorderUV);
				if (borderMode == BorderMode.Tiled)
				{
					mat.SetVector(Ids.SoftMask_TileRepeat, tileRepeat);
				}
			}
		}

		private Vector2 XY2UV(Vector2 localPos)
		{
			return borderMode switch
			{
				BorderMode.Simple => MapSimple(localPos), 
				BorderMode.Sliced => MapBorder(localPos, repeat: false), 
				BorderMode.Tiled => MapBorder(localPos, repeat: true), 
				_ => MapSimple(localPos), 
			};
		}

		private Vector2 MapSimple(Vector2 localPos)
		{
			return Mathr.Remap(localPos, maskRect, maskRectUV);
		}

		private Vector2 MapBorder(Vector2 localPos, bool repeat)
		{
			return new Vector2(Inset(localPos.x, maskRect.x, maskBorder.x, maskBorder.z, maskRect.z, maskRectUV.x, maskBorderUV.x, maskBorderUV.z, maskRectUV.z, (!repeat) ? 1f : tileRepeat.x), Inset(localPos.y, maskRect.y, maskBorder.y, maskBorder.w, maskRect.w, maskRectUV.y, maskBorderUV.y, maskBorderUV.w, maskRectUV.w, (!repeat) ? 1f : tileRepeat.y));
		}

		private float Inset(float v, float x1, float x2, float u1, float u2, float repeat = 1f)
		{
			float num = x2 - x1;
			return Mathf.Lerp(u1, u2, (num == 0f) ? 0f : Frac((v - x1) / num * repeat));
		}

		private float Inset(float v, float x1, float x2, float x3, float x4, float u1, float u2, float u3, float u4, float repeat = 1f)
		{
			if (v < x2)
			{
				return Inset(v, x1, x2, u1, u2);
			}
			if (v < x3)
			{
				return Inset(v, x2, x3, u2, u3, repeat);
			}
			return Inset(v, x3, x4, u3, u4);
		}

		private float Frac(float v)
		{
			return v - Mathf.Floor(v);
		}

		private float MaskValue(Color mask)
		{
			Color color = mask * maskChannelWeights;
			return color.a + color.r + color.g + color.b;
		}
	}

	private struct Diagnostics
	{
		private SoftMask _softMask;

		private Image image => _softMask._graphic as Image;

		private Sprite sprite => _softMask.source switch
		{
			MaskSource.Sprite => _softMask._sprite, 
			MaskSource.Graphic => (!image) ? null : image.sprite, 
			_ => null, 
		};

		public Diagnostics(SoftMask softMask)
		{
			_softMask = softMask;
		}

		public Errors PollErrors()
		{
			SoftMask softMask = _softMask;
			Errors errors = Errors.NoError;
			softMask.GetComponentsInChildren(s_maskables);
			if (s_maskables.Any((SoftMaskable m) => object.ReferenceEquals(m.mask, softMask) && m.shaderIsNotSupported))
			{
				errors |= Errors.UnsupportedShaders;
			}
			if (ThereAreNestedMasks())
			{
				errors |= Errors.NestedMasks;
			}
			errors |= CheckSprite(sprite);
			return errors | CheckImage();
		}

		public static Errors CheckSprite(Sprite sprite)
		{
			Errors errors = Errors.NoError;
			if (!sprite)
			{
				return errors;
			}
			if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
			{
				errors |= Errors.TightPackedSprite;
			}
			if ((bool)sprite.associatedAlphaSplitTexture)
			{
				errors |= Errors.AlphaSplitSprite;
			}
			return errors;
		}

		private bool ThereAreNestedMasks()
		{
			SoftMask softMask = _softMask;
			bool flag = false;
			softMask.GetComponentsInParent(includeInactive: false, s_masks);
			flag |= s_masks.Any((SoftMask x) => AreCompeting(softMask, x));
			softMask.GetComponentsInChildren(includeInactive: false, s_masks);
			return flag | s_masks.Any((SoftMask x) => AreCompeting(softMask, x));
		}

		private Errors CheckImage()
		{
			Errors errors = Errors.NoError;
			if (!_softMask.isBasedOnGraphic)
			{
				return errors;
			}
			if ((bool)image && image.type == Image.Type.Filled)
			{
				errors |= Errors.UnsupportedImageType;
			}
			return errors;
		}

		private static bool AreCompeting(SoftMask softMask, SoftMask other)
		{
			return softMask.isMaskingEnabled && softMask != other && other.isMaskingEnabled && softMask.canvas.rootCanvas == other.canvas.rootCanvas && !Child(softMask, other).canvas.overrideSorting;
		}

		private static T Child<T>(T first, T second) where T : Component
		{
			return (!first.transform.IsChildOf(second.transform)) ? second : first;
		}
	}

	[SerializeField]
	private Shader _defaultShader;

	[SerializeField]
	private Shader _defaultETC1Shader;

	[SerializeField]
	private MaskSource _source;

	[SerializeField]
	private RectTransform _separateMask;

	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	private BorderMode _spriteBorderMode;

	[SerializeField]
	private Texture2D _texture;

	[SerializeField]
	private Rect _textureUVRect = DefaultUVRect;

	[SerializeField]
	private Color _channelWeights = MaskChannel.alpha;

	[SerializeField]
	private float _raycastThreshold;

	private MaterialReplacements _materials;

	private MaterialParameters _parameters;

	private Sprite _lastUsedSprite;

	private bool _maskingWasEnabled;

	private bool _destroyed;

	private bool _dirty;

	private RectTransform _maskTransform;

	private Graphic _graphic;

	private Canvas _canvas;

	private static readonly Rect DefaultUVRect = new Rect(0f, 0f, 1f, 1f);

	private static readonly List<SoftMask> s_masks = new List<SoftMask>();

	private static readonly List<SoftMaskable> s_maskables = new List<SoftMaskable>();

	bool ISoftMask.isAlive => (bool)this && !_destroyed;

	public Shader defaultShader
	{
		get
		{
			return _defaultShader;
		}
		set
		{
			SetShader(ref _defaultShader, value);
		}
	}

	public Shader defaultETC1Shader
	{
		get
		{
			return _defaultETC1Shader;
		}
		set
		{
			SetShader(ref _defaultETC1Shader, value, warnIfNotSet: false);
		}
	}

	public MaskSource source
	{
		get
		{
			return _source;
		}
		set
		{
			if (_source != value)
			{
				Set(ref _source, value);
			}
		}
	}

	public RectTransform separateMask
	{
		get
		{
			return _separateMask;
		}
		set
		{
			if (_separateMask != value)
			{
				Set(ref _separateMask, value);
				_graphic = null;
				_maskTransform = null;
			}
		}
	}

	public Sprite sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			if (_sprite != value)
			{
				Set(ref _sprite, value);
			}
		}
	}

	public BorderMode spriteBorderMode
	{
		get
		{
			return _spriteBorderMode;
		}
		set
		{
			if (_spriteBorderMode != value)
			{
				Set(ref _spriteBorderMode, value);
			}
		}
	}

	public Texture2D texture
	{
		get
		{
			return _texture;
		}
		set
		{
			if (_texture != value)
			{
				Set(ref _texture, value);
			}
		}
	}

	public Rect textureUVRect
	{
		get
		{
			return _textureUVRect;
		}
		set
		{
			if (_textureUVRect != value)
			{
				Set(ref _textureUVRect, value);
			}
		}
	}

	public Color channelWeights
	{
		get
		{
			return _channelWeights;
		}
		set
		{
			if (_channelWeights != value)
			{
				Set(ref _channelWeights, value);
			}
		}
	}

	public float raycastThreshold
	{
		get
		{
			return _raycastThreshold;
		}
		set
		{
			_raycastThreshold = value;
		}
	}

	public bool isMaskingEnabled => base.isActiveAndEnabled && (bool)canvas;

	private RectTransform maskTransform => (!_maskTransform) ? (_maskTransform = ((!_separateMask) ? GetComponent<RectTransform>() : _separateMask)) : _maskTransform;

	private Canvas canvas => (!_canvas) ? (_canvas = NearestEnabledCanvas()) : _canvas;

	private bool isBasedOnGraphic => _source == MaskSource.Graphic;

	public SoftMask()
	{
		_materials = new MaterialReplacements(Replace, delegate(Material m)
		{
			_parameters.Apply(m);
		});
	}

	public Errors PollErrors()
	{
		return new Diagnostics(this).PollErrors();
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera cam)
	{
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(maskTransform, sp, cam, out var localPoint))
		{
			return false;
		}
		if (!Mathr.Inside(localPoint, LocalMaskRect(Vector4.zero)))
		{
			return false;
		}
		if (!_parameters.texture)
		{
			return true;
		}
		if (_raycastThreshold <= 0f)
		{
			return true;
		}
		if (!_parameters.SampleMask(localPoint, out var mask))
		{
			return true;
		}
		return mask >= _raycastThreshold;
	}

	protected override void Start()
	{
		base.Start();
		WarnIfDefaultShaderIsNotSet();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SpawnMaskablesInChildren(base.transform);
		FindGraphic();
		if (isMaskingEnabled)
		{
			UpdateMask();
		}
		NotifyChildrenThatMaskMightChanged();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)_graphic)
		{
			_graphic.UnregisterDirtyVerticesCallback(OnGraphicDirty);
			_graphic.UnregisterDirtyMaterialCallback(OnGraphicDirty);
			_graphic = null;
		}
		NotifyChildrenThatMaskMightChanged();
		DestroyMaterials();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_destroyed = true;
		NotifyChildrenThatMaskMightChanged();
	}

	protected virtual void LateUpdate()
	{
		bool flag = isMaskingEnabled;
		if (flag)
		{
			if (_maskingWasEnabled != flag)
			{
				SpawnMaskablesInChildren(base.transform);
			}
			Graphic graphic = _graphic;
			FindGraphic();
			if (maskTransform.hasChanged || _dirty || !object.ReferenceEquals(_graphic, graphic))
			{
				UpdateMask();
			}
		}
		_maskingWasEnabled = flag;
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		_dirty = true;
	}

	protected override void OnDidApplyAnimationProperties()
	{
		base.OnDidApplyAnimationProperties();
		_dirty = true;
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		_canvas = null;
		_dirty = true;
	}

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		_canvas = null;
		_dirty = true;
		NotifyChildrenThatMaskMightChanged();
	}

	private void OnTransformChildrenChanged()
	{
		SpawnMaskablesInChildren(base.transform);
	}

	Material ISoftMask.GetReplacement(Material original)
	{
		return _materials.Get(original);
	}

	void ISoftMask.ReleaseReplacement(Material replacement)
	{
		_materials.Release(replacement);
	}

	void ISoftMask.UpdateTransformChildren(Transform transform)
	{
		SpawnMaskablesInChildren(transform);
	}

	private void OnGraphicDirty()
	{
		if (isBasedOnGraphic)
		{
			_dirty = true;
		}
	}

	private void FindGraphic()
	{
		if (!_graphic)
		{
			_graphic = maskTransform.GetComponent<Graphic>();
			if ((bool)_graphic)
			{
				_graphic.RegisterDirtyVerticesCallback(OnGraphicDirty);
				_graphic.RegisterDirtyMaterialCallback(OnGraphicDirty);
			}
		}
	}

	private Canvas NearestEnabledCanvas()
	{
		Canvas[] componentsInParent = GetComponentsInParent<Canvas>(includeInactive: false);
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (componentsInParent[i].isActiveAndEnabled)
			{
				return componentsInParent[i];
			}
		}
		return null;
	}

	private void UpdateMask()
	{
		CalculateMaskParameters();
		_materials.ApplyAll();
		maskTransform.hasChanged = false;
		_dirty = false;
	}

	private void SpawnMaskablesInChildren(Transform root)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			Transform child = root.GetChild(i);
			if (!child.GetComponent<SoftMaskable>())
			{
				child.gameObject.AddComponent<SoftMaskable>();
			}
		}
	}

	private void InvalidateChildren()
	{
		ForEachChildMaskable(delegate(SoftMaskable x)
		{
			x.Invalidate();
		});
	}

	private void NotifyChildrenThatMaskMightChanged()
	{
		ForEachChildMaskable(delegate(SoftMaskable x)
		{
			x.MaskMightChanged();
		});
	}

	private void ForEachChildMaskable(Action<SoftMaskable> f)
	{
		base.transform.GetComponentsInChildren(s_maskables);
		for (int i = 0; i < s_maskables.Count; i++)
		{
			SoftMaskable softMaskable = s_maskables[i];
			if ((bool)softMaskable && softMaskable.gameObject != base.gameObject)
			{
				f(softMaskable);
			}
		}
	}

	private void DestroyMaterials()
	{
		_materials.DestroyAllAndClear();
	}

	private Material Replace(Material original)
	{
		if (original == null || original.HasDefaultUIShader())
		{
			return Replace(original, _defaultShader);
		}
		if (original.HasDefaultETC1UIShader())
		{
			return Replace(original, _defaultETC1Shader);
		}
		if (original.SupportsSoftMask())
		{
			return new Material(original);
		}
		return null;
	}

	private static Material Replace(Material original, Shader defaultReplacementShader)
	{
		Material material = ((!defaultReplacementShader) ? null : new Material(defaultReplacementShader));
		if ((bool)material && (bool)original)
		{
			material.CopyPropertiesFromMaterial(original);
		}
		return material;
	}

	private void CalculateMaskParameters()
	{
		switch (_source)
		{
		case MaskSource.Graphic:
			if (_graphic is Image)
			{
				CalculateImageBased((Image)_graphic);
			}
			else if (_graphic is RawImage)
			{
				CalculateRawImageBased((RawImage)_graphic);
			}
			else
			{
				CalculateSolidFill();
			}
			break;
		case MaskSource.Sprite:
			CalculateSpriteBased(_sprite, _spriteBorderMode);
			break;
		case MaskSource.Texture:
			CalculateTextureBased(_texture, _textureUVRect);
			break;
		default:
			CalculateSolidFill();
			break;
		}
	}

	private BorderMode ToBorderMode(Image.Type imageType)
	{
		return imageType switch
		{
			Image.Type.Simple => BorderMode.Simple, 
			Image.Type.Sliced => BorderMode.Sliced, 
			Image.Type.Tiled => BorderMode.Tiled, 
			_ => BorderMode.Simple, 
		};
	}

	private void CalculateImageBased(Image image)
	{
		CalculateSpriteBased(image.sprite, ToBorderMode(image.type));
	}

	private void CalculateRawImageBased(RawImage image)
	{
		CalculateTextureBased(image.texture as Texture2D, image.uvRect);
	}

	private void CalculateSpriteBased(Sprite sprite, BorderMode borderMode)
	{
		Sprite lastUsedSprite = _lastUsedSprite;
		_lastUsedSprite = sprite;
		Errors errors = Diagnostics.CheckSprite(sprite);
		if (errors != Errors.NoError)
		{
			if (lastUsedSprite != sprite)
			{
				WarnSpriteErrors(errors);
			}
			CalculateSolidFill();
			return;
		}
		if (!sprite)
		{
			CalculateSolidFill();
			return;
		}
		FillCommonParameters();
		Vector4 vector = Mathr.Move(Mathr.ToVector(sprite.rect), sprite.textureRect.position - sprite.rect.position - sprite.textureRectOffset);
		Vector4 vector2 = Mathr.ToVector(sprite.textureRect);
		Vector4 vector3 = Mathr.BorderOf(vector, vector2);
		Vector2 s = new Vector2(sprite.texture.width, sprite.texture.height);
		Vector4 vector4 = LocalMaskRect(Vector4.zero);
		_parameters.maskRectUV = Mathr.Div(vector2, s);
		if (borderMode == BorderMode.Simple)
		{
			Vector4 v = Mathr.Div(vector3, Mathr.Size(vector));
			_parameters.maskRect = Mathr.ApplyBorder(vector4, Mathr.Mul(v, Mathr.Size(vector4)));
		}
		else
		{
			_parameters.maskRect = Mathr.ApplyBorder(vector4, vector3 * GraphicToCanvasScale(sprite));
			Vector4 v2 = Mathr.Div(vector, s);
			Vector4 border = AdjustBorders(sprite.border * GraphicToCanvasScale(sprite), vector4);
			_parameters.maskBorder = LocalMaskRect(border);
			_parameters.maskBorderUV = Mathr.ApplyBorder(v2, Mathr.Div(sprite.border, s));
		}
		_parameters.texture = sprite.texture;
		_parameters.borderMode = borderMode;
		if (borderMode == BorderMode.Tiled)
		{
			_parameters.tileRepeat = MaskRepeat(sprite, _parameters.maskBorder);
		}
	}

	private static Vector4 AdjustBorders(Vector4 border, Vector4 rect)
	{
		Vector2 vector = Mathr.Size(rect);
		for (int i = 0; i <= 1; i++)
		{
			float num = border[i] + border[i + 2];
			if (vector[i] < num && num != 0f)
			{
				float num2 = vector[i] / num;
				border[i] *= num2;
				border[i + 2] *= num2;
			}
		}
		return border;
	}

	private void CalculateTextureBased(Texture2D texture, Rect uvRect)
	{
		FillCommonParameters();
		_parameters.maskRect = LocalMaskRect(Vector4.zero);
		_parameters.maskRectUV = Mathr.ToVector(uvRect);
		_parameters.texture = texture;
		_parameters.borderMode = BorderMode.Simple;
	}

	private void CalculateSolidFill()
	{
		CalculateTextureBased(null, DefaultUVRect);
	}

	private void FillCommonParameters()
	{
		_parameters.worldToMask = WorldToMask();
		_parameters.maskChannelWeights = _channelWeights;
	}

	private float GraphicToCanvasScale(Sprite sprite)
	{
		float num = ((!canvas) ? 100f : canvas.referencePixelsPerUnit);
		float num2 = ((!sprite) ? 100f : sprite.pixelsPerUnit);
		return num / num2;
	}

	private Matrix4x4 WorldToMask()
	{
		return maskTransform.worldToLocalMatrix * canvas.rootCanvas.transform.localToWorldMatrix;
	}

	private Vector4 LocalMaskRect(Vector4 border)
	{
		return Mathr.ApplyBorder(Mathr.ToVector(maskTransform.rect), border);
	}

	private Vector2 MaskRepeat(Sprite sprite, Vector4 centralPart)
	{
		Vector4 r = Mathr.ApplyBorder(Mathr.ToVector(sprite.textureRect), sprite.border);
		return Mathr.Div(Mathr.Size(centralPart) * GraphicToCanvasScale(sprite), Mathr.Size(r));
	}

	private void WarnIfDefaultShaderIsNotSet()
	{
		if ((bool)_defaultShader)
		{
		}
	}

	private void WarnSpriteErrors(Errors errors)
	{
		if ((errors & Errors.TightPackedSprite) != Errors.NoError)
		{
		}
		if ((errors & Errors.AlphaSplitSprite) == 0)
		{
		}
	}

	private void Set<T>(ref T field, T value)
	{
		field = value;
		_dirty = true;
	}

	private void SetShader(ref Shader field, Shader value, bool warnIfNotSet = true)
	{
		if (field != value)
		{
			field = value;
			if (warnIfNotSet)
			{
				WarnIfDefaultShaderIsNotSet();
			}
			DestroyMaterials();
			InvalidateChildren();
		}
	}
}
