using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class ParticleManager : MonoBehaviour
{
	protected List<ParticleSystems> particlePrefabs;

	protected List<GameObject> particlePrefabLightGameObjects = new List<GameObject>();

	public int currentParticlePrefab;

	public List<ParticleSystems> particlePrefabsAppend;

	public int prefabNameUnderscoreCountCutoff = 4;

	public bool disableChildrenAtStart = true;

	private bool initialized;

	public void init()
	{
		particlePrefabs = GetComponentsInChildren<ParticleSystems>(includeInactive: true).ToList();
		particlePrefabs.AddRange(particlePrefabsAppend);
		if (disableChildrenAtStart)
		{
			for (int i = 0; i < particlePrefabs.Count; i++)
			{
				particlePrefabs[i].gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < particlePrefabs.Count; j++)
		{
			AnimatedLight[] componentsInChildren = particlePrefabs[j].GetComponentsInChildren<AnimatedLight>(includeInactive: true);
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				particlePrefabLightGameObjects.Add(componentsInChildren[k].gameObject);
			}
		}
		initialized = true;
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		if (initialized)
		{
			init();
		}
	}

	public virtual void next()
	{
		currentParticlePrefab++;
		if (currentParticlePrefab > particlePrefabs.Count - 1)
		{
			currentParticlePrefab = 0;
		}
	}

	public virtual void previous()
	{
		currentParticlePrefab--;
		if (currentParticlePrefab < 0)
		{
			currentParticlePrefab = particlePrefabs.Count - 1;
		}
	}

	public string getCurrentPrefabName(bool shorten = false)
	{
		string text = particlePrefabs[currentParticlePrefab].name;
		if (shorten)
		{
			int num = 0;
			for (int i = 0; i < prefabNameUnderscoreCountCutoff; i++)
			{
				num = text.IndexOf("_", num) + 1;
				if (num == 0)
				{
					MonoBehaviour.print("Iteration of underscore not found.");
					break;
				}
			}
			text = text.Substring(num, text.Length - num);
		}
		return "PARTICLE SYSTEM: #" + (currentParticlePrefab + 1).ToString("00") + " / " + particlePrefabs.Count.ToString("00") + " (" + text + ")";
	}

	public virtual int getParticleCount()
	{
		return 0;
	}

	public void setLights(bool value)
	{
		for (int i = 0; i < particlePrefabLightGameObjects.Count; i++)
		{
			particlePrefabLightGameObjects[i].SetActive(value);
		}
	}

	protected virtual void Update()
	{
	}
}
