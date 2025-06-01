using System;
using UnityEngine;
using UnityEngine.UI;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class DemoManager_XPTitles : MonoBehaviour
{
	private LoopingParticleSystemsManager list;

	public Text particleCountText;

	public Text currentParticleSystemText;

	private void Awake()
	{
		(list = GetComponent<LoopingParticleSystemsManager>()).init();
	}

	private void Start()
	{
		updateCurrentParticleSystemNameText();
	}

	private void Update()
	{
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			next();
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			previous();
		}
	}

	private void LateUpdate()
	{
		particleCountText.text = "PARTICLE COUNT: ";
		particleCountText.text += list.getParticleCount();
	}

	public void next()
	{
		list.next();
		updateCurrentParticleSystemNameText();
	}

	public void previous()
	{
		list.previous();
		updateCurrentParticleSystemNameText();
	}

	private void updateCurrentParticleSystemNameText()
	{
		currentParticleSystemText.text = list.getCurrentPrefabName(shorten: true);
	}
}
