using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class PerlinNoiseXYZ
{
	public PerlinNoise x;

	public PerlinNoise y;

	public PerlinNoise z;

	public float amplitudeScale = 1f;

	public float frequencyScale = 1f;

	public Vector3 xyz
	{
		get
		{
			float time = Time.time * frequencyScale;
			return new Vector3(x.getValue(time), y.getValue(time), z.getValue(time)) * amplitudeScale;
		}
	}

	public void init()
	{
		x.init();
		y.init();
		z.init();
	}
}
