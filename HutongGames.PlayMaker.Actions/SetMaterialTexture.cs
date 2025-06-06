using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Material)]
[Tooltip("Sets a named texture in a game object's material.")]
public class SetMaterialTexture : ComponentAction<Renderer>
{
	[Tooltip("The GameObject that the material is applied to.")]
	[CheckForComponent(typeof(Renderer))]
	public FsmOwnerDefault gameObject;

	[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
	public FsmInt materialIndex;

	[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
	public FsmMaterial material;

	[UIHint(UIHint.NamedTexture)]
	[Tooltip("A named parameter in the shader.")]
	public FsmString namedTexture;

	public FsmTexture texture;

	public override void Reset()
	{
		gameObject = null;
		materialIndex = 0;
		material = null;
		namedTexture = "_MainTex";
		texture = null;
	}

	public override void OnEnter()
	{
		DoSetMaterialTexture();
		Finish();
	}

	private void DoSetMaterialTexture()
	{
		string text = namedTexture.Value;
		if (text == string.Empty)
		{
			text = "_MainTex";
		}
		if (material.Value != null)
		{
			material.Value.SetTexture(text, texture.Value);
			return;
		}
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (UpdateCache(ownerDefaultTarget))
		{
			if (base.renderer.material == null)
			{
				LogError("Missing Material!");
			}
			else if (materialIndex.Value == 0)
			{
				base.renderer.material.SetTexture(text, texture.Value);
			}
			else if (base.renderer.materials.Length > materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[materialIndex.Value].SetTexture(text, texture.Value);
				base.renderer.materials = materials;
			}
		}
	}
}
