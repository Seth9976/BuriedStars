using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Application)]
[Tooltip("Get a scene isLoaded flag.")]
public class GetSceneIsLoaded : FsmStateAction
{
	[RequiredField]
	public FsmString SceneName;

	[ActionSection("Result")]
	[Tooltip("Event sent if the scene is loaded.")]
	public FsmEvent isLoadedEvent;

	[Tooltip("Event sent if the scene is not loaded.")]
	public FsmEvent isNotLoadedEvent;

	[Tooltip("Repeat every Frame")]
	public bool everyFrame;

	private Scene CurrentScene;

	public override void Reset()
	{
		base.Reset();
		SceneName = string.Empty;
		isLoadedEvent = null;
		isNotLoadedEvent = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		base.OnEnter();
		DoGetSceneIsLoaded();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoGetSceneIsLoaded();
	}

	private void DoGetSceneIsLoaded()
	{
		if (SceneManager.GetSceneByName(SceneName.Value).isLoaded)
		{
			base.Fsm.Event(isLoadedEvent);
		}
		else
		{
			base.Fsm.Event(isNotLoadedEvent);
		}
	}
}
