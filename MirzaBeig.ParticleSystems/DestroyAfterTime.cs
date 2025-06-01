using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class DestroyAfterTime : MonoBehaviour
{
	public float time = 2f;

	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, time);
	}
}
