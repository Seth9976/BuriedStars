using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

public abstract class ComponentAction<T> : FsmStateAction where T : Component
{
	private GameObject cachedGameObject;

	private T component;

	protected Rigidbody rigidbody => component as Rigidbody;

	protected Rigidbody2D rigidbody2d => component as Rigidbody2D;

	protected Renderer renderer => component as Renderer;

	protected Animation animation => component as Animation;

	protected AudioSource audio => component as AudioSource;

	protected Camera camera => component as Camera;

	protected GUIText guiText => component as GUIText;

	protected GUITexture guiTexture => component as GUITexture;

	protected Light light => component as Light;

	protected NetworkView networkView => component as NetworkView;

	protected bool UpdateCache(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		if (component == null || cachedGameObject != go)
		{
			component = go.GetComponent<T>();
			cachedGameObject = go;
			if (component == null)
			{
				LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
			}
		}
		return component != null;
	}
}
