using System;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class ParticleSystemsSimulationSpeed : ParticleSystems
{
	public float speed = 1f;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
		setPlaybackSpeed(speed);
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}
}
