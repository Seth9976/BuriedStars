using System;
using UnityEngine;

namespace MirzaBeig.ParticleSystems;

[Serializable]
[RequireComponent(typeof(Renderer))]
public class RendererSortingOrder : MonoBehaviour
{
	public int sortingOrder;

	private void Awake()
	{
	}

	private void Start()
	{
		GetComponent<Renderer>().sortingOrder = sortingOrder;
	}

	private void Update()
	{
	}
}
