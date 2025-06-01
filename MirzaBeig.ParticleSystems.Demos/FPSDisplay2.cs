using System;
using UnityEngine;
using UnityEngine.UI;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
[ExecuteInEditMode]
public class FPSDisplay2 : MonoBehaviour
{
	private static FPSDisplay2 instance;

	public float fpsMeasurePeriod = 0.5f;

	private int m_FpsAccumulator;

	private float m_FpsNextPeriod;

	private int m_CurrentFps;

	private Text fpsText;

	public string textFormat = "FPS (X/Xs-AVG): 00.00";

	private void Awake()
	{
		if ((bool)instance && !Application.isEditor)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		fpsText = GetComponent<Text>();
		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
	}

	private void Update()
	{
		m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > m_FpsNextPeriod)
		{
			m_CurrentFps = (int)((float)m_FpsAccumulator / fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
			fpsText.text = m_CurrentFps.ToString(textFormat);
		}
	}
}
