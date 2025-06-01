using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class OneshotParticleSystemsManager : ParticleManager
{
	public LayerMask mouseRaycastLayerMask;

	private List<ParticleSystems> spawnedPrefabs;

	public bool disableSpawn { get; set; }

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		disableSpawn = false;
		spawnedPrefabs = new List<ParticleSystems>();
	}

	private void OnEnable()
	{
	}

	public void clear()
	{
		if (spawnedPrefabs == null)
		{
			return;
		}
		for (int i = 0; i < spawnedPrefabs.Count; i++)
		{
			if ((bool)spawnedPrefabs[i])
			{
				UnityEngine.Object.Destroy(spawnedPrefabs[i].gameObject);
			}
		}
		spawnedPrefabs.Clear();
	}

	protected override void Update()
	{
		base.Update();
	}

	public void instantiateParticlePrefab(Vector2 mousePosition, float maxDistance)
	{
		if (spawnedPrefabs != null && !disableSpawn)
		{
			Vector3 position = mousePosition;
			position.z = maxDistance;
			Vector3 vector = Camera.main.ScreenToWorldPoint(position);
			Vector3 direction = vector - Camera.main.transform.position;
			Physics.Raycast(Camera.main.transform.position, direction, out var hitInfo, maxDistance);
			Vector3 position2 = ((!hitInfo.collider) ? vector : hitInfo.point);
			ParticleSystems particleSystems = particlePrefabs[currentParticlePrefab];
			ParticleSystems particleSystems2 = UnityEngine.Object.Instantiate(particleSystems, position2, particleSystems.transform.rotation);
			particleSystems2.gameObject.SetActive(value: true);
			particleSystems2.transform.parent = base.transform;
			spawnedPrefabs.Add(particleSystems2);
		}
	}

	public void randomize()
	{
		currentParticlePrefab = UnityEngine.Random.Range(0, particlePrefabs.Count);
	}

	public override int getParticleCount()
	{
		int num = 0;
		if (spawnedPrefabs != null)
		{
			for (int i = 0; i < spawnedPrefabs.Count; i++)
			{
				if ((bool)spawnedPrefabs[i])
				{
					num += spawnedPrefabs[i].getParticleCount();
				}
				else
				{
					spawnedPrefabs.RemoveAt(i);
				}
			}
		}
		return num;
	}
}
