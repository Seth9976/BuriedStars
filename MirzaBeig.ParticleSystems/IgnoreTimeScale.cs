using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
[RequireComponent(typeof(ParticleSystems))]
public class IgnoreTimeScale : MonoBehaviour
{
	private ParticleSystems particleSystems;

	private void Awake()
	{
	}

	private void Start()
	{
		particleSystems = GetComponent<ParticleSystems>();
	}

	private void Update()
	{
		particleSystems.simulate(Time.unscaledDeltaTime);
	}
}
