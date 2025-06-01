using System;
using UnityEngine;
using UnityEngine.UI;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
[ExecuteInEditMode]
public class FPSDisplay : MonoBehaviour
{
	private static FPSDisplay instance;

	private Text fpsText;

	private int frames;

	private float time;

	public int targetFrameRate = 60;

	public float updateTime = 1f;

	public string textFormat = "FPS (X/Xs-AVG): 00.00";

	private void Awake()
	{
		if ((bool)instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
		}
		Application.targetFrameRate = targetFrameRate;
	}

	private void Start()
	{
		fpsText = GetComponent<Text>();
	}

	private void Update()
	{
		time += Time.deltaTime;
		frames++;
		if (time > updateTime)
		{
			fpsText.text = (1f / (time / (float)frames)).ToString(textFormat);
			time = 0f;
			frames = 0;
		}
	}
}
