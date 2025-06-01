using GameEvent;
using UnityEngine;

public class ComponentGroup : MonoBehaviour
{
	public MonoBehaviour[] Components;

	public MonoBehaviour[] m_BGLayerComponent;

	private void Start()
	{
		EventEngine.GetInstance().SetTempCam(isActive: false, isFullCoverActive: false);
	}

	private void OnEnable()
	{
		base.enabled = true;
	}

	private void OnDisable()
	{
		base.enabled = false;
	}

	public void ActivateComponents()
	{
		if (!base.enabled || Components == null || Components.Length <= 0)
		{
			return;
		}
		MonoBehaviour[] components = Components;
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if (!(monoBehaviour == null))
			{
				monoBehaviour.enabled = true;
			}
		}
	}

	public void DeactivateComponents()
	{
		if (!base.enabled || Components == null || Components.Length <= 0)
		{
			return;
		}
		MonoBehaviour[] components = Components;
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if (!(monoBehaviour == null))
			{
				monoBehaviour.enabled = false;
			}
		}
	}

	public void ActivateBGLayerComponent()
	{
		if (!base.enabled || m_BGLayerComponent == null || m_BGLayerComponent.Length <= 0)
		{
			return;
		}
		MonoBehaviour[] bGLayerComponent = m_BGLayerComponent;
		foreach (MonoBehaviour monoBehaviour in bGLayerComponent)
		{
			if (!(monoBehaviour == null))
			{
				monoBehaviour.enabled = true;
			}
		}
	}

	public void DeactivateBGLayerComponent()
	{
		if (!base.enabled || m_BGLayerComponent == null || m_BGLayerComponent.Length <= 0)
		{
			return;
		}
		MonoBehaviour[] bGLayerComponent = m_BGLayerComponent;
		foreach (MonoBehaviour monoBehaviour in bGLayerComponent)
		{
			if (!(monoBehaviour == null))
			{
				monoBehaviour.enabled = false;
			}
		}
	}
}
