using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Level)]
[Tooltip("Get the name of the level that was last loaded (Read Only)")]
public class LevelNameToString : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmString levelName;

	public override void Reset()
	{
		levelName = null;
	}

	public override void OnEnter()
	{
		levelName.Value = SceneManager.GetActiveScene().name;
		Finish();
	}
}
