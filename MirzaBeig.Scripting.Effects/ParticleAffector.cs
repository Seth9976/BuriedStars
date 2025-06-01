using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.Scripting.Effects;

public abstract class ParticleAffector : MonoBehaviour
{
	protected struct GetForceParameters
	{
		public float distanceToAffectorCenterSqr;

		public Vector3 scaledDirectionToAffectorCenter;

		public Vector3 particlePosition;
	}

	[Header("Common Controls")]
	public float radius = float.PositiveInfinity;

	public float force = 5f;

	public Vector3 offset = Vector3.zero;

	public AnimationCurve scaleForceByDistance = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	private ParticleSystem particleSystem;

	public ParticleSystem[] _particleSystems;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private ParticleSystem.Particle[][] particleSystemParticles;

	private ParticleSystem.MainModule[] particleSystemMainModules;

	private Renderer[] particleSystemRenderers;

	protected ParticleSystem currentParticleSystem;

	public bool alwaysUpdate;

	public float scaledRadius => radius * base.transform.lossyScale.x;

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	protected virtual void perParticleSystemSetup()
	{
	}

	protected virtual Vector3 getForce(GetForceParameters parameters)
	{
		return Vector3.zero;
	}

	protected virtual void Update()
	{
	}

	protected virtual void LateUpdate()
	{
		float num = scaledRadius;
		float num2 = num * num;
		float num3 = force * Time.deltaTime;
		Vector3 vector = base.transform.position + offset;
		if ((bool)particleSystem)
		{
			if (particleSystems.Count == 1)
			{
				particleSystems[0] = particleSystem;
			}
			else
			{
				particleSystems.Clear();
				particleSystems.Add(particleSystem);
			}
		}
		else if (_particleSystems.Length != 0)
		{
			if (particleSystems.Count != _particleSystems.Length)
			{
				particleSystems.Clear();
				particleSystems.AddRange(_particleSystems);
			}
			else
			{
				for (int i = 0; i < _particleSystems.Length; i++)
				{
					particleSystems[i] = _particleSystems[i];
				}
			}
		}
		else
		{
			particleSystems.Clear();
			particleSystems.AddRange(UnityEngine.Object.FindObjectsOfType<ParticleSystem>());
		}
		GetForceParameters parameters = default(GetForceParameters);
		int count = particleSystems.Count;
		if (particleSystemParticles == null || particleSystemParticles.Length < count)
		{
			particleSystemParticles = new ParticleSystem.Particle[count][];
			particleSystemMainModules = new ParticleSystem.MainModule[count];
			particleSystemRenderers = new Renderer[count];
			for (int j = 0; j < count; j++)
			{
				ref ParticleSystem.MainModule reference = ref particleSystemMainModules[j];
				reference = particleSystems[j].main;
				particleSystemRenderers[j] = particleSystems[j].GetComponent<Renderer>();
			}
		}
		for (int k = 0; k < count; k++)
		{
			currentParticleSystem = particleSystems[k];
			if (!particleSystemRenderers[k].isVisible && !alwaysUpdate)
			{
				continue;
			}
			int maxParticles = particleSystemMainModules[k].maxParticles;
			if (particleSystemParticles[k] == null || particleSystemParticles[k].Length < maxParticles)
			{
				particleSystemParticles[k] = new ParticleSystem.Particle[maxParticles];
			}
			perParticleSystemSetup();
			int particles = currentParticleSystem.GetParticles(particleSystemParticles[k]);
			float multiplier = currentParticleSystem.externalForces.multiplier;
			ParticleSystemSimulationSpace simulationSpace = particleSystemMainModules[k].simulationSpace;
			ParticleSystemScalingMode scalingMode = particleSystemMainModules[k].scalingMode;
			Transform transform = currentParticleSystem.transform;
			Transform customSimulationSpace = particleSystemMainModules[k].customSimulationSpace;
			if (simulationSpace == ParticleSystemSimulationSpace.World)
			{
				for (int l = 0; l < particles; l++)
				{
					parameters.particlePosition = particleSystemParticles[k][l].position;
					parameters.scaledDirectionToAffectorCenter.x = vector.x - parameters.particlePosition.x;
					parameters.scaledDirectionToAffectorCenter.y = vector.y - parameters.particlePosition.y;
					parameters.scaledDirectionToAffectorCenter.z = vector.z - parameters.particlePosition.z;
					parameters.distanceToAffectorCenterSqr = parameters.scaledDirectionToAffectorCenter.sqrMagnitude;
					if (parameters.distanceToAffectorCenterSqr < num2)
					{
						float time = parameters.distanceToAffectorCenterSqr / num2;
						float num4 = scaleForceByDistance.Evaluate(time);
						Vector3 vector2 = getForce(parameters);
						float num5 = num3 * num4 * multiplier;
						vector2.x *= num5;
						vector2.y *= num5;
						vector2.z *= num5;
						Vector3 velocity = particleSystemParticles[k][l].velocity;
						velocity.x += vector2.x;
						velocity.y += vector2.y;
						velocity.z += vector2.z;
						particleSystemParticles[k][l].velocity = velocity;
					}
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				Vector3 one = Vector3.one;
				Transform transform2 = transform;
				switch (simulationSpace)
				{
				case ParticleSystemSimulationSpace.Local:
					zero = transform2.position;
					identity = transform2.rotation;
					one = transform2.localScale;
					break;
				case ParticleSystemSimulationSpace.Custom:
					transform2 = customSimulationSpace;
					zero = transform2.position;
					identity = transform2.rotation;
					one = transform2.localScale;
					break;
				default:
					throw new NotSupportedException($"Unsupported scaling mode '{simulationSpace}'.");
				}
				for (int m = 0; m < particles; m++)
				{
					parameters.particlePosition = particleSystemParticles[k][m].position;
					if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
					{
						switch (scalingMode)
						{
						case ParticleSystemScalingMode.Hierarchy:
							parameters.particlePosition = transform2.TransformPoint(particleSystemParticles[k][m].position);
							break;
						case ParticleSystemScalingMode.Local:
							parameters.particlePosition = Vector3.Scale(parameters.particlePosition, one);
							parameters.particlePosition = identity * parameters.particlePosition;
							parameters.particlePosition += zero;
							break;
						case ParticleSystemScalingMode.Shape:
							parameters.particlePosition = identity * parameters.particlePosition;
							parameters.particlePosition += zero;
							break;
						default:
							throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
						}
					}
					parameters.scaledDirectionToAffectorCenter.x = vector.x - parameters.particlePosition.x;
					parameters.scaledDirectionToAffectorCenter.y = vector.y - parameters.particlePosition.y;
					parameters.scaledDirectionToAffectorCenter.z = vector.z - parameters.particlePosition.z;
					parameters.distanceToAffectorCenterSqr = parameters.scaledDirectionToAffectorCenter.sqrMagnitude;
					if (!(parameters.distanceToAffectorCenterSqr < num2))
					{
						continue;
					}
					float time2 = parameters.distanceToAffectorCenterSqr / num2;
					float num6 = scaleForceByDistance.Evaluate(time2);
					Vector3 vector3 = getForce(parameters);
					float num7 = num3 * num6 * multiplier;
					vector3.x *= num7;
					vector3.y *= num7;
					vector3.z *= num7;
					if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
					{
						switch (scalingMode)
						{
						case ParticleSystemScalingMode.Hierarchy:
							vector3 = transform2.InverseTransformVector(vector3);
							break;
						case ParticleSystemScalingMode.Local:
							vector3 = Quaternion.Inverse(identity) * vector3;
							vector3 = Vector3.Scale(vector3, new Vector3(1f / one.x, 1f / one.y, 1f / one.z));
							break;
						case ParticleSystemScalingMode.Shape:
							vector3 = Quaternion.Inverse(identity) * vector3;
							break;
						default:
							throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
						}
					}
					Vector3 velocity2 = particleSystemParticles[k][m].velocity;
					velocity2.x += vector3.x;
					velocity2.y += vector3.y;
					velocity2.z += vector3.z;
					particleSystemParticles[k][m].velocity = velocity2;
				}
			}
			particleSystems[k].SetParticles(particleSystemParticles[k], particles);
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + offset, scaledRadius);
	}
}
