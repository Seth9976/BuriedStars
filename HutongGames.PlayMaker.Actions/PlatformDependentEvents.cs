using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Sends Events based on platform dependent flags")]
public class PlatformDependentEvents : FsmStateAction
{
	public enum platformDependentFlags
	{
		UNITY_EDITOR,
		UNITY_UNITY_PSP2,
		UNITY_PS4,
		UNITY_STANDALONE_WIN,
		UNITY_ANDROID,
		UNITY_IOS
	}

	[ActionSection("Platforms")]
	[CompoundArray("Count", "Key", "Event")]
	[RequiredField]
	[Tooltip("The platform")]
	public platformDependentFlags[] platforms;

	[Tooltip("The event to send for that platform")]
	public FsmEvent[] events;

	public override void Reset()
	{
		platforms = new platformDependentFlags[1];
		platforms[0] = platformDependentFlags.UNITY_EDITOR;
		events = new FsmEvent[1];
		events[0] = null;
	}

	private bool isMatch(platformDependentFlags valueA, platformDependentFlags valueB)
	{
		string a = Enum.GetName(typeof(platformDependentFlags), valueA);
		string b = Enum.GetName(typeof(platformDependentFlags), valueB);
		return string.Equals(a, b);
	}

	public override void OnEnter()
	{
		int num = 0;
		platformDependentFlags[] array = platforms;
		foreach (platformDependentFlags platformDependentFlags in array)
		{
			FsmEvent fsmEvent = events[num];
			if (platformDependentFlags == platformDependentFlags.UNITY_STANDALONE_WIN)
			{
				base.Fsm.Event(fsmEvent);
			}
			num++;
		}
		Finish();
	}
}
