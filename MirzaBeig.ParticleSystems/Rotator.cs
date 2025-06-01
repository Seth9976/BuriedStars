using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
public class Rotator : MonoBehaviour
{
	public Vector3 localRotationSpeed;

	public Vector3 worldRotationSpeed;

	public bool executeInEditMode;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnRenderObject()
	{
		if (executeInEditMode && !Application.isPlaying)
		{
			rotate();
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			rotate();
		}
	}

	private void rotate()
	{
		if (localRotationSpeed != Vector3.zero)
		{
			base.transform.Rotate(localRotationSpeed * Time.deltaTime, Space.Self);
		}
		if (worldRotationSpeed != Vector3.zero)
		{
			base.transform.Rotate(worldRotationSpeed * Time.deltaTime, Space.World);
		}
	}
}
