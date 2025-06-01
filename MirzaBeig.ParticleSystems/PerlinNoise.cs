using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class PerlinNoise
{
	private Vector2 offset;

	public float amplitude = 1f;

	public float frequency = 1f;

	public float value => getValue(Time.time);

	public void init()
	{
		offset.x = UnityEngine.Random.Range(0f, 99999f);
		offset.y = UnityEngine.Random.Range(0f, 99999f);
	}

	public float getValue(float time)
	{
		float num = time * frequency;
		return (Mathf.PerlinNoise(num + offset.x, num + offset.y) - 0.5f) * amplitude;
	}
}
