using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/ArrayList")]
[Tooltip("Sort GameObjects within an arrayList based on the distance of a transform or position.")]
public class ArrayListSortGameObjectByDistance : ArrayListActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
	[CheckForComponent(typeof(PlayMakerArrayListProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
	public FsmGameObject distanceFrom;

	[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
	public FsmVector3 orDistanceFromVector3;

	public bool everyframe;

	public override void Reset()
	{
		gameObject = null;
		reference = null;
		distanceFrom = null;
		orDistanceFromVector3 = null;
		everyframe = true;
	}

	public override void OnEnter()
	{
		if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			Finish();
		}
		DoSortByDistance();
		if (!everyframe)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoSortByDistance();
	}

	private void DoSortByDistance()
	{
		if (!isProxyValid())
		{
			return;
		}
		Vector3 value = orDistanceFromVector3.Value;
		GameObject value2 = distanceFrom.Value;
		if (value2 != null)
		{
			value += value2.transform.position;
		}
		for (int i = 0; i < proxy.arrayList.Count - 1; i++)
		{
			GameObject gameObject = (GameObject)proxy.arrayList[i];
			GameObject gameObject2 = (GameObject)proxy.arrayList[i + 1];
			float sqrMagnitude = (gameObject.transform.position - value).sqrMagnitude;
			float sqrMagnitude2 = (gameObject2.transform.position - value).sqrMagnitude;
			if (sqrMagnitude2 < sqrMagnitude)
			{
				GameObject value3 = (GameObject)proxy.arrayList[i];
				proxy.arrayList[i] = proxy.arrayList[i + 1];
				proxy.arrayList[i + 1] = value3;
				i = 0;
			}
		}
	}
}
