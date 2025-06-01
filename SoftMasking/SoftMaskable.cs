using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("")]
public class SoftMaskable : UIBehaviour, IMaterialModifier
{
	private ISoftMask _mask;

	private Graphic _graphic;

	private Material _replacement;

	private bool _affectedByMask;

	private bool _destroyed;

	public bool shaderIsNotSupported { get; private set; }

	public bool isMaskingEnabled => mask != null && mask.isAlive && mask.isMaskingEnabled && _affectedByMask;

	public ISoftMask mask
	{
		get
		{
			return _mask;
		}
		private set
		{
			if (_mask != value)
			{
				if (_mask != null)
				{
					replacement = null;
				}
				_mask = ((value == null || !value.isAlive) ? null : value);
				Invalidate();
			}
		}
	}

	private Graphic graphic => (!_graphic) ? (_graphic = GetComponent<Graphic>()) : _graphic;

	private Material replacement
	{
		get
		{
			return _replacement;
		}
		set
		{
			if (_replacement != value)
			{
				if (_replacement != null && mask != null)
				{
					mask.ReleaseReplacement(_replacement);
				}
				_replacement = value;
			}
		}
	}

	public Material GetModifiedMaterial(Material baseMaterial)
	{
		if (isMaskingEnabled)
		{
			Material material = mask.GetReplacement(baseMaterial);
			replacement = material;
			if ((bool)replacement)
			{
				shaderIsNotSupported = false;
				return replacement;
			}
			if (!baseMaterial.HasDefaultUIShader())
			{
				SetShaderNotSupported(baseMaterial);
			}
		}
		else
		{
			shaderIsNotSupported = false;
			replacement = null;
		}
		return baseMaterial;
	}

	public void Invalidate()
	{
		if ((bool)graphic)
		{
			graphic.SetMaterialDirty();
		}
	}

	public void MaskMightChanged()
	{
		if (FindMaskOrDie())
		{
			Invalidate();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.hideFlags = HideFlags.HideInInspector;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (FindMaskOrDie())
		{
			RequestChildTransformUpdate();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		mask = null;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_destroyed = true;
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		FindMaskOrDie();
	}

	protected override void OnCanvasHierarchyChanged()
	{
		base.OnCanvasHierarchyChanged();
		FindMaskOrDie();
	}

	private void OnTransformChildrenChanged()
	{
		RequestChildTransformUpdate();
	}

	private void RequestChildTransformUpdate()
	{
		if (mask != null)
		{
			mask.UpdateTransformChildren(base.transform);
		}
	}

	private bool FindMaskOrDie()
	{
		if (_destroyed)
		{
			return false;
		}
		mask = NearestMask(base.transform, out _affectedByMask) ?? NearestMask(base.transform, out _affectedByMask, enabledOnly: false);
		if (mask == null)
		{
			_destroyed = true;
			Object.DestroyImmediate(this);
			return false;
		}
		return true;
	}

	private static ISoftMask NearestMask(Transform transform, out bool processedByThisMask, bool enabledOnly = true)
	{
		processedByThisMask = true;
		Transform transform2 = transform;
		ISoftMask iSoftMask;
		while (true)
		{
			if (!transform2)
			{
				return null;
			}
			if (transform2 != transform)
			{
				iSoftMask = GetISoftMask(transform2, enabledOnly);
				if (iSoftMask != null)
				{
					break;
				}
			}
			if (IsOverridingSortingCanvas(transform2))
			{
				processedByThisMask = false;
			}
			transform2 = transform2.parent;
		}
		return iSoftMask;
	}

	private static ISoftMask GetISoftMask(Transform current, bool shouldBeEnabled = true)
	{
		ISoftMask component = current.GetComponent<ISoftMask>();
		if (component != null && component.isAlive && (!shouldBeEnabled || component.isMaskingEnabled))
		{
			return component;
		}
		return null;
	}

	private static bool IsOverridingSortingCanvas(Transform transform)
	{
		Canvas component = transform.GetComponent<Canvas>();
		if ((bool)component && component.overrideSorting)
		{
			return true;
		}
		return false;
	}

	private void SetShaderNotSupported(Material material)
	{
		if (!shaderIsNotSupported)
		{
			shaderIsNotSupported = true;
		}
	}
}
