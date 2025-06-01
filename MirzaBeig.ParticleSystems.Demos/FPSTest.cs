using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class FPSTest : MonoBehaviour
{
	public int targetFPS1 = 60;

	public int targetFPS2 = 10;

	private int previousVSyncCount;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			Application.targetFrameRate = targetFPS2;
			previousVSyncCount = QualitySettings.vSyncCount;
			QualitySettings.vSyncCount = 0;
		}
		else if (Input.GetKeyUp(KeyCode.Space))
		{
			Application.targetFrameRate = targetFPS1;
			QualitySettings.vSyncCount = previousVSyncCount;
		}
	}
}
