using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/ArrayList")]
[Tooltip("Active/Deactivate all GameObjects within an arrayList.")]
public class ArrayListActivateGameObjects : ArrayListActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
	[CheckForComponent(typeof(PlayMakerArrayListProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[RequiredField]
	[Tooltip("Check to activate, uncheck to deactivate Game Objects.")]
	public FsmBool activate;

	[Tooltip("Resets the affected game objects when exiting this state to their original activate state. Useful if you want an object to be controlled only while this state is active.")]
	public FsmBool resetOnExit;

	private bool[] _activeStates;

	public override void Reset()
	{
		gameObject = null;
		activate = null;
		resetOnExit = false;
	}

	public override void OnEnter()
	{
		if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			Finish();
		}
		DoAction();
		Finish();
	}

	private void DoAction()
	{
		if (!isProxyValid())
		{
			return;
		}
		_activeStates = new bool[proxy.arrayList.Count];
		int num = 0;
		foreach (GameObject array in proxy.arrayList)
		{
			if (!(array == null))
			{
				_activeStates[num] = array.activeSelf;
				array.SetActive(activate.Value);
				num++;
			}
		}
	}

	public override void OnExit()
	{
		if (!resetOnExit.Value || _activeStates == null)
		{
			return;
		}
		int num = 0;
		foreach (GameObject array in proxy.arrayList)
		{
			if (!(array == null))
			{
				array.SetActive(_activeStates[num]);
				num++;
			}
		}
	}
}
