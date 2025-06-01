using System;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class LoopingParticleSystemsManager : ParticleManager
{
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		particlePrefabs[currentParticlePrefab].gameObject.SetActive(value: true);
	}

	public override void next()
	{
		particlePrefabs[currentParticlePrefab].gameObject.SetActive(value: false);
		base.next();
		particlePrefabs[currentParticlePrefab].gameObject.SetActive(value: true);
	}

	public override void previous()
	{
		particlePrefabs[currentParticlePrefab].gameObject.SetActive(value: false);
		base.previous();
		particlePrefabs[currentParticlePrefab].gameObject.SetActive(value: true);
	}

	protected override void Update()
	{
		base.Update();
	}

	public override int getParticleCount()
	{
		return particlePrefabs[currentParticlePrefab].getParticleCount();
	}
}
