using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.Scripting.Effects;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour
{
	public float maxDistance = 1f;

	public int maxConnections = 5;

	public int maxLineRenderers = 100;

	[Range(0f, 1f)]
	public float widthFromParticle = 0.125f;

	[Range(0f, 1f)]
	public float colourFromParticle = 1f;

	[Range(0f, 1f)]
	public float alphaFromParticle = 1f;

	private ParticleSystem particleSystem;

	private ParticleSystem.Particle[] particles;

	private ParticleSystem.MainModule particleSystemMainModule;

	public LineRenderer lineRendererTemplate;

	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	private Transform _transform;

	private float timer;

	[Range(0f, 1f)]
	public float delay;

	private bool visible;

	public bool alwaysUpdate;

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		particleSystemMainModule = particleSystem.main;
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.radius = 2.25f;
		_transform = base.transform;
	}

	private void OnDisable()
	{
		for (int i = 0; i < lineRenderers.Count; i++)
		{
			lineRenderers[i].enabled = false;
		}
	}

	private void OnBecameVisible()
	{
		visible = true;
	}

	private void OnBecameInvisible()
	{
		visible = false;
	}

	private void LateUpdate()
	{
		int num = lineRenderers.Count;
		if (num > maxLineRenderers)
		{
			for (int i = maxLineRenderers; i < num; i++)
			{
				UnityEngine.Object.Destroy(lineRenderers[i].gameObject);
			}
			lineRenderers.RemoveRange(maxLineRenderers, num - maxLineRenderers);
			num -= num - maxLineRenderers;
		}
		if (!alwaysUpdate && !visible)
		{
			return;
		}
		int maxParticles = particleSystemMainModule.maxParticles;
		if (particles == null || particles.Length < maxParticles)
		{
			particles = new ParticleSystem.Particle[maxParticles];
		}
		timer += Time.deltaTime;
		if (!(timer >= delay))
		{
			return;
		}
		timer = 0f;
		int num2 = 0;
		if (maxConnections > 0 && maxLineRenderers > 0)
		{
			particleSystem.GetParticles(particles);
			int particleCount = particleSystem.particleCount;
			float num3 = maxDistance * maxDistance;
			ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;
			ParticleSystemScalingMode scalingMode = particleSystemMainModule.scalingMode;
			Transform customSimulationSpace = particleSystemMainModule.customSimulationSpace;
			Color startColor = lineRendererTemplate.startColor;
			Color endColor = lineRendererTemplate.endColor;
			float a = lineRendererTemplate.startWidth * lineRendererTemplate.widthMultiplier;
			float a2 = lineRendererTemplate.endWidth * lineRendererTemplate.widthMultiplier;
			if (simulationSpace == ParticleSystemSimulationSpace.World)
			{
				for (int j = 0; j < particleCount; j++)
				{
					if (num2 == maxLineRenderers)
					{
						break;
					}
					Color b = particles[j].GetCurrentColor(particleSystem);
					Color startColor2 = Color.LerpUnclamped(startColor, b, colourFromParticle);
					startColor2.a = Mathf.LerpUnclamped(startColor.a, b.a, alphaFromParticle);
					float startWidth = Mathf.LerpUnclamped(a, particles[j].GetCurrentSize(particleSystem), widthFromParticle);
					int num4 = 0;
					for (int k = j + 1; k < particleCount; k++)
					{
						Vector3 vector = new Vector3(particles[j].position.x - particles[k].position.x, particles[j].position.y - particles[k].position.y, particles[j].position.z - particles[k].position.z);
						float num5 = Vector3.SqrMagnitude(vector);
						if (num5 <= num3)
						{
							LineRenderer item;
							if (num2 == num)
							{
								item = UnityEngine.Object.Instantiate(lineRendererTemplate, _transform, worldPositionStays: false);
								lineRenderers.Add(item);
								num++;
							}
							item = lineRenderers[num2];
							item.enabled = true;
							item.SetPosition(0, particles[j].position);
							item.SetPosition(1, particles[k].position);
							item.startColor = startColor2;
							b = particles[k].GetCurrentColor(particleSystem);
							Color endColor2 = Color.LerpUnclamped(endColor, b, colourFromParticle);
							endColor2.a = Mathf.LerpUnclamped(endColor.a, b.a, alphaFromParticle);
							item.endColor = endColor2;
							float currentSize = particles[j].GetCurrentSize(particleSystem);
							item.startWidth = startWidth;
							item.endWidth = Mathf.LerpUnclamped(a2, particles[k].GetCurrentSize(particleSystem), widthFromParticle);
							num2++;
							num4++;
							if (num4 == maxConnections || num2 == maxLineRenderers)
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				Vector3 one = Vector3.one;
				Transform transform = _transform;
				switch (simulationSpace)
				{
				case ParticleSystemSimulationSpace.Local:
					zero = transform.position;
					identity = transform.rotation;
					one = transform.localScale;
					break;
				case ParticleSystemSimulationSpace.Custom:
					transform = customSimulationSpace;
					zero = transform.position;
					identity = transform.rotation;
					one = transform.localScale;
					break;
				default:
					throw new NotSupportedException($"Unsupported scaling mode '{simulationSpace}'.");
				}
				Vector3 position = Vector3.zero;
				Vector3 position2 = Vector3.zero;
				for (int l = 0; l < particleCount; l++)
				{
					if (num2 == maxLineRenderers)
					{
						break;
					}
					if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
					{
						switch (scalingMode)
						{
						case ParticleSystemScalingMode.Hierarchy:
							position = transform.TransformPoint(particles[l].position);
							break;
						case ParticleSystemScalingMode.Local:
							position = Vector3.Scale(particles[l].position, one);
							position = identity * position;
							position += zero;
							break;
						case ParticleSystemScalingMode.Shape:
							position = identity * particles[l].position;
							position += zero;
							break;
						default:
							throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
						}
					}
					Color b2 = particles[l].GetCurrentColor(particleSystem);
					Color startColor3 = Color.LerpUnclamped(startColor, b2, colourFromParticle);
					startColor3.a = Mathf.LerpUnclamped(startColor.a, b2.a, alphaFromParticle);
					float startWidth2 = Mathf.LerpUnclamped(a, particles[l].GetCurrentSize(particleSystem), widthFromParticle);
					int num6 = 0;
					for (int m = l + 1; m < particleCount; m++)
					{
						if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
						{
							switch (scalingMode)
							{
							case ParticleSystemScalingMode.Hierarchy:
								position2 = transform.TransformPoint(particles[m].position);
								break;
							case ParticleSystemScalingMode.Local:
								position2 = Vector3.Scale(particles[m].position, one);
								position2 = identity * position2;
								position2 += zero;
								break;
							case ParticleSystemScalingMode.Shape:
								position2 = identity * particles[m].position;
								position2 += zero;
								break;
							default:
								throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
							}
						}
						Vector3 vector2 = new Vector3(position.x - position2.x, position.y - position2.y, position.z - position2.z);
						float num7 = Vector3.SqrMagnitude(vector2);
						if (num7 <= num3)
						{
							LineRenderer item2;
							if (num2 == num)
							{
								item2 = UnityEngine.Object.Instantiate(lineRendererTemplate, _transform, worldPositionStays: false);
								lineRenderers.Add(item2);
								num++;
							}
							item2 = lineRenderers[num2];
							item2.enabled = true;
							item2.SetPosition(0, position);
							item2.SetPosition(1, position2);
							item2.startColor = startColor3;
							b2 = particles[m].GetCurrentColor(particleSystem);
							Color endColor3 = Color.LerpUnclamped(endColor, b2, colourFromParticle);
							endColor3.a = Mathf.LerpUnclamped(endColor.a, b2.a, alphaFromParticle);
							item2.endColor = endColor3;
							float currentSize2 = particles[l].GetCurrentSize(particleSystem);
							item2.startWidth = startWidth2;
							item2.endWidth = Mathf.LerpUnclamped(a2, particles[m].GetCurrentSize(particleSystem), widthFromParticle);
							num2++;
							num6++;
							if (num6 == maxConnections || num2 == maxLineRenderers)
							{
								break;
							}
						}
					}
				}
			}
		}
		for (int n = num2; n < num; n++)
		{
			if (lineRenderers[n].enabled)
			{
				lineRenderers[n].enabled = false;
			}
		}
	}
}
