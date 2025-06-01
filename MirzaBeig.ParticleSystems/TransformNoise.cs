using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class TransformNoise : MonoBehaviour
{
	public PerlinNoiseXYZ positionNoise;

	public PerlinNoiseXYZ rotationNoise;

	private void Start()
	{
		positionNoise.init();
		rotationNoise.init();
	}

	private void Update()
	{
		base.transform.localPosition = positionNoise.xyz;
		base.transform.localEulerAngles = rotationNoise.xyz;
	}
}
