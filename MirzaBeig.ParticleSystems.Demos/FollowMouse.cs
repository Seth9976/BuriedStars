using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems.Demos;

[Serializable]
public class FollowMouse : MonoBehaviour
{
	public float speed = 8f;

	public float distanceFromCamera = 5f;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = distanceFromCamera;
		Vector3 b = Camera.main.ScreenToWorldPoint(mousePosition);
		Vector3 position = Vector3.Lerp(base.transform.position, b, 1f - Mathf.Exp((0f - speed) * Time.deltaTime));
		base.transform.position = position;
	}

	private void LateUpdate()
	{
	}
}
