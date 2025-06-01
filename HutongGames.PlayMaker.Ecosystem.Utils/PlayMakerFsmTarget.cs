using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class PlayMakerFsmTarget
{
	public ProxyFsmTarget target;

	public GameObject gameObject;

	public string fsmName;

	[SerializeField]
	private PlayMakerFSM _fsmComponent;

	private bool _initialized;

	public PlayMakerFSM fsmComponent
	{
		get
		{
			Initialize();
			return _fsmComponent;
		}
		set
		{
			_fsmComponent = value;
		}
	}

	public PlayMakerFsmTarget()
	{
	}

	public PlayMakerFsmTarget(ProxyFsmTarget target)
	{
		this.target = target;
	}

	public void Initialize()
	{
		if (!Application.isPlaying || _initialized)
		{
			return;
		}
		_initialized = true;
		if (target != ProxyFsmTarget.FsmComponent)
		{
			if (gameObject != null)
			{
				fsmComponent = PlayMakerUtils.FindFsmOnGameObject(gameObject, fsmName);
			}
			if (!(fsmComponent == null))
			{
			}
		}
	}
}
