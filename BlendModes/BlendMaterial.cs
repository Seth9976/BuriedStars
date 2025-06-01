using UnityEngine;

namespace BlendModes;

public struct BlendMaterial
{
	public Material Material;

	public ObjectType ObjectType;

	public RenderMode RenderMode;

	public BlendMode BlendMode;

	public BlendMaterial(ObjectType objectType, RenderMode renderMode, BlendMode blendMode)
	{
		Material = null;
		ObjectType = objectType;
		RenderMode = renderMode;
		BlendMode = blendMode;
	}

	public bool IsEqual(BlendMaterial mat)
	{
		return mat.ObjectType == ObjectType && mat.RenderMode == RenderMode && mat.BlendMode == BlendMode;
	}
}
