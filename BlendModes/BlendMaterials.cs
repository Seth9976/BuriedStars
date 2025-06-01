using System.Collections.Generic;
using UnityEngine;

namespace BlendModes;

public static class BlendMaterials
{
	private static List<BlendMaterial> cachedMaterials = new List<BlendMaterial>();

	public static Material GetMaterial(ObjectType objectType, RenderMode renderMode = RenderMode.Grab, BlendMode blendMode = BlendMode.Normal, Shader shader = null)
	{
		if (Application.isEditor && renderMode == RenderMode.Framebuffer)
		{
			renderMode = RenderMode.Grab;
		}
		BlendMaterial blendMataterial = new BlendMaterial(objectType, renderMode, blendMode);
		if (objectType != ObjectType.MeshDefault && objectType != ObjectType.ParticleDefault && cachedMaterials.Exists((BlendMaterial m) => m.IsEqual(blendMataterial)))
		{
			return cachedMaterials.Find((BlendMaterial m) => m.IsEqual(blendMataterial)).Material;
		}
		Material material = new Material((!shader) ? Resources.Load<Shader>($"BlendModes/{objectType}/{renderMode}") : shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		material.EnableKeyword("Bm" + blendMode);
		blendMataterial.Material = material;
		cachedMaterials.Add(blendMataterial);
		return material;
	}
}
