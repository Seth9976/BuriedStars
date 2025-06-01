using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class DestroyOnParticlesDead : ParticleSystems
{
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		base.onParticleSystemsDeadEvent += onParticleSystemsDead;
	}

	private void onParticleSystemsDead()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}
}
