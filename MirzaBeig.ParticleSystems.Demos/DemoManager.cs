using System;
using UnityEngine;
using UnityEngine.UI;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class DemoManager : MonoBehaviour
{
	public enum ParticleMode
	{
		looping,
		oneshot
	}

	public enum Level
	{
		none,
		basic
	}

	public Transform cameraRotationTransform;

	public Transform cameraTranslationTransform;

	public Vector3 cameraLookAtPosition = new Vector3(0f, 3f, 0f);

	public FollowMouse mouse;

	private Vector3 targetCameraPosition;

	private Vector3 targetCameraRotation;

	private Vector3 cameraPositionStart;

	private Vector3 cameraRotationStart;

	private Vector2 input;

	private Vector3 cameraRotation;

	public float cameraMoveAmount = 2f;

	public float cameraRotateAmount = 2f;

	public float cameraMoveSpeed = 12f;

	public float cameraRotationSpeed = 12f;

	public Vector2 cameraAngleLimits = new Vector2(-8f, 60f);

	public GameObject[] levels;

	public Level currentLevel = Level.basic;

	public ParticleMode particleMode;

	public bool lighting = true;

	public bool advancedRendering = true;

	public Toggle frontFacingCameraModeToggle;

	public Toggle interactiveCameraModeToggle;

	public Toggle loopingParticleModeToggle;

	public Toggle oneshotParticleModeToggle;

	public Toggle lightingToggle;

	public Toggle advancedRenderingToggle;

	private Toggle[] levelToggles;

	public ToggleGroup levelTogglesContainer;

	public LoopingParticleSystemsManager loopingParticleSystems;

	public OneshotParticleSystemsManager oneshotParticleSystems;

	public Text particleCountText;

	public Text currentParticleSystemText;

	public Text particleSpawnInstructionText;

	public Slider timeScaleSlider;

	public Text timeScaleSliderValueText;

	public Camera UICamera;

	public Camera mainCamera;

	public Camera postEffectsCamera;

	public MonoBehaviour[] mainCameraPostEffects;

	private void Awake()
	{
		loopingParticleSystems.init();
		oneshotParticleSystems.init();
	}

	private void Start()
	{
		cameraPositionStart = cameraTranslationTransform.localPosition;
		cameraRotationStart = cameraRotationTransform.localEulerAngles;
		resetCameraTransformTargets();
		switch (particleMode)
		{
		case ParticleMode.looping:
			setToPerpetualParticleMode(set: true);
			loopingParticleModeToggle.isOn = true;
			oneshotParticleModeToggle.isOn = false;
			break;
		case ParticleMode.oneshot:
			setToInstancedParticleMode(set: true);
			loopingParticleModeToggle.isOn = false;
			oneshotParticleModeToggle.isOn = true;
			break;
		default:
			MonoBehaviour.print("Unknown case.");
			break;
		}
		setLighting(lighting);
		setAdvancedRendering(advancedRendering);
		lightingToggle.isOn = lighting;
		advancedRenderingToggle.isOn = advancedRendering;
		levelToggles = levelTogglesContainer.GetComponentsInChildren<Toggle>(includeInactive: true);
		for (int i = 0; i < levels.Length; i++)
		{
			if (i == (int)currentLevel)
			{
				levels[i].SetActive(value: true);
				levelToggles[i].isOn = true;
			}
			else
			{
				levels[i].SetActive(value: false);
				levelToggles[i].isOn = false;
			}
		}
		updateCurrentParticleSystemNameText();
		timeScaleSlider.onValueChanged.AddListener(onTimeScaleSliderValueChanged);
		onTimeScaleSliderValueChanged(timeScaleSlider.value);
	}

	public void onTimeScaleSliderValueChanged(float value)
	{
		Time.timeScale = value;
		timeScaleSliderValueText.text = value.ToString("0.00");
	}

	public void setToPerpetualParticleMode(bool set)
	{
		if (set)
		{
			oneshotParticleSystems.clear();
			loopingParticleSystems.gameObject.SetActive(value: true);
			oneshotParticleSystems.gameObject.SetActive(value: false);
			particleSpawnInstructionText.gameObject.SetActive(value: false);
			particleMode = ParticleMode.looping;
			updateCurrentParticleSystemNameText();
		}
	}

	public void setToInstancedParticleMode(bool set)
	{
		if (set)
		{
			loopingParticleSystems.gameObject.SetActive(value: false);
			oneshotParticleSystems.gameObject.SetActive(value: true);
			particleSpawnInstructionText.gameObject.SetActive(value: true);
			particleMode = ParticleMode.oneshot;
			updateCurrentParticleSystemNameText();
		}
	}

	public void setLevel(Level level)
	{
		for (int i = 0; i < levels.Length; i++)
		{
			if (i == (int)level)
			{
				levels[i].SetActive(value: true);
			}
			else
			{
				levels[i].SetActive(value: false);
			}
		}
		currentLevel = level;
	}

	public void setLevelFromToggle(Toggle toggle)
	{
		if (toggle.isOn)
		{
			setLevel((Level)Array.IndexOf(levelToggles, toggle));
		}
	}

	public void setLighting(bool value)
	{
		lighting = value;
		loopingParticleSystems.setLights(value);
		oneshotParticleSystems.setLights(value);
	}

	public void setAdvancedRendering(bool value)
	{
		advancedRendering = value;
		postEffectsCamera.gameObject.SetActive(value);
		UICamera.allowHDR = value;
		mainCamera.allowHDR = value;
		if (value)
		{
			QualitySettings.SetQualityLevel(32, applyExpensiveChanges: true);
			UICamera.renderingPath = RenderingPath.UsePlayerSettings;
			mainCamera.renderingPath = RenderingPath.UsePlayerSettings;
			lightingToggle.isOn = true;
			mouse.gameObject.SetActive(value: true);
		}
		else
		{
			QualitySettings.SetQualityLevel(0, applyExpensiveChanges: true);
			UICamera.renderingPath = RenderingPath.VertexLit;
			mainCamera.renderingPath = RenderingPath.VertexLit;
			lightingToggle.isOn = false;
			mouse.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < mainCameraPostEffects.Length; i++)
		{
			if ((bool)mainCameraPostEffects[i])
			{
				mainCameraPostEffects[i].enabled = value;
			}
		}
	}

	public static Vector3 dampVector3(Vector3 from, Vector3 to, float speed, float dt)
	{
		return Vector3.Lerp(from, to, 1f - Mathf.Exp((0f - speed) * dt));
	}

	private void Update()
	{
		input.x = Input.GetAxis("Horizontal");
		input.y = Input.GetAxis("Vertical");
		if (Input.GetKey(KeyCode.LeftShift))
		{
			targetCameraPosition.z += input.y * cameraMoveAmount;
			targetCameraPosition.z = Mathf.Clamp(targetCameraPosition.z, -6.3f, -1f);
		}
		else
		{
			targetCameraRotation.y += input.x * cameraRotateAmount;
			targetCameraRotation.x += input.y * cameraRotateAmount;
			targetCameraRotation.x = Mathf.Clamp(targetCameraRotation.x, cameraAngleLimits.x, cameraAngleLimits.y);
		}
		cameraTranslationTransform.localPosition = Vector3.Lerp(cameraTranslationTransform.localPosition, targetCameraPosition, Time.deltaTime * cameraMoveSpeed);
		cameraRotation = dampVector3(cameraRotation, targetCameraRotation, cameraRotationSpeed, Time.deltaTime);
		cameraRotationTransform.localEulerAngles = cameraRotation;
		cameraTranslationTransform.LookAt(cameraLookAtPosition);
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			next();
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			previous();
		}
		else if (Input.GetKey(KeyCode.R) && particleMode == ParticleMode.oneshot)
		{
			oneshotParticleSystems.randomize();
			updateCurrentParticleSystemNameText();
			if (!Input.GetKey(KeyCode.T))
			{
			}
		}
		if (particleMode == ParticleMode.oneshot)
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Input.GetMouseButtonDown(0))
			{
				oneshotParticleSystems.instantiateParticlePrefab(mousePosition, mouse.distanceFromCamera);
			}
			if (Input.GetMouseButton(1))
			{
				oneshotParticleSystems.instantiateParticlePrefab(mousePosition, mouse.distanceFromCamera);
			}
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			resetCameraTransformTargets();
		}
	}

	private void LateUpdate()
	{
		particleCountText.text = "PARTICLE COUNT: ";
		if (particleMode == ParticleMode.looping)
		{
			particleCountText.text += loopingParticleSystems.getParticleCount();
		}
		else if (particleMode == ParticleMode.oneshot)
		{
			particleCountText.text += oneshotParticleSystems.getParticleCount();
		}
	}

	private void resetCameraTransformTargets()
	{
		targetCameraPosition = cameraPositionStart;
		targetCameraRotation = cameraRotationStart;
	}

	private void updateCurrentParticleSystemNameText()
	{
		if (particleMode == ParticleMode.looping)
		{
			currentParticleSystemText.text = loopingParticleSystems.getCurrentPrefabName(shorten: true);
		}
		else if (particleMode == ParticleMode.oneshot)
		{
			currentParticleSystemText.text = oneshotParticleSystems.getCurrentPrefabName(shorten: true);
		}
	}

	public void next()
	{
		if (particleMode == ParticleMode.looping)
		{
			loopingParticleSystems.next();
		}
		else if (particleMode == ParticleMode.oneshot)
		{
			oneshotParticleSystems.next();
		}
		updateCurrentParticleSystemNameText();
	}

	public void previous()
	{
		if (particleMode == ParticleMode.looping)
		{
			loopingParticleSystems.previous();
		}
		else if (particleMode == ParticleMode.oneshot)
		{
			oneshotParticleSystems.previous();
		}
		updateCurrentParticleSystemNameText();
	}
}
