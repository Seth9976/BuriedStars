using System;
using System.Collections.Generic;
using AmplifyMotion;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("")]
public class AmplifyMotionObjectBase : MonoBehaviour
{
	public enum MinMaxCurveState
	{
		Scalar,
		Curve,
		TwoCurves,
		TwoScalars
	}

	internal static bool ApplyToChildren = true;

	[SerializeField]
	private bool m_applyToChildren = ApplyToChildren;

	private ObjectType m_type;

	private Dictionary<Camera, MotionState> m_states = new Dictionary<Camera, MotionState>();

	private bool m_fixedStep;

	private int m_objectId;

	private Vector3 m_lastPosition = Vector3.zero;

	private int m_resetAtFrame = -1;

	internal bool FixedStep => m_fixedStep;

	internal int ObjectId => m_objectId;

	public ObjectType Type => m_type;

	internal void RegisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		if ((component.cullingMask & (1 << base.gameObject.layer)) != 0 && !m_states.ContainsKey(component))
		{
			MotionState motionState = null;
			motionState = m_type switch
			{
				ObjectType.Solid => new SolidState(camera, this), 
				ObjectType.Skinned => new SkinnedState(camera, this), 
				ObjectType.Cloth => new ClothState(camera, this), 
				ObjectType.Particle => new ParticleState(camera, this), 
				_ => throw new Exception("[AmplifyMotion] Invalid object type."), 
			};
			camera.RegisterObject(this);
			m_states.Add(component, motionState);
		}
	}

	internal void UnregisterCamera(AmplifyMotionCamera camera)
	{
		Camera component = camera.GetComponent<Camera>();
		if (m_states.TryGetValue(component, out var value))
		{
			camera.UnregisterObject(this);
			if (m_states.TryGetValue(component, out value))
			{
				value.Shutdown();
			}
			m_states.Remove(component);
		}
	}

	private bool InitializeType()
	{
		Renderer component = GetComponent<Renderer>();
		if (AmplifyMotionEffectBase.CanRegister(base.gameObject, autoReg: false))
		{
			ParticleSystem component2 = GetComponent<ParticleSystem>();
			if (component2 != null)
			{
				m_type = ObjectType.Particle;
				AmplifyMotionEffectBase.RegisterObject(this);
			}
			else if (component != null)
			{
				if (component.GetType() == typeof(MeshRenderer))
				{
					m_type = ObjectType.Solid;
				}
				else if (component.GetType() == typeof(SkinnedMeshRenderer))
				{
					if (GetComponent<Cloth>() != null)
					{
						m_type = ObjectType.Cloth;
					}
					else
					{
						m_type = ObjectType.Skinned;
					}
				}
				AmplifyMotionEffectBase.RegisterObject(this);
			}
		}
		return component != null;
	}

	private void OnEnable()
	{
		bool flag = InitializeType();
		if (flag)
		{
			if (m_type == ObjectType.Cloth)
			{
				m_fixedStep = false;
			}
			else if (m_type == ObjectType.Solid)
			{
				Rigidbody component = GetComponent<Rigidbody>();
				if (component != null && component.interpolation == RigidbodyInterpolation.None && !component.isKinematic)
				{
					m_fixedStep = true;
				}
			}
		}
		if (m_applyToChildren)
		{
			foreach (Transform item in base.gameObject.transform)
			{
				AmplifyMotionEffectBase.RegisterRecursivelyS(item.gameObject);
			}
		}
		if (!flag)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		AmplifyMotionEffectBase.UnregisterObject(this);
	}

	private void TryInitializeStates()
	{
		Dictionary<Camera, MotionState>.Enumerator enumerator = m_states.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MotionState value = enumerator.Current.Value;
			if (value.Owner.Initialized && !value.Error && !value.Initialized)
			{
				value.Initialize();
			}
		}
	}

	private void Start()
	{
		if (AmplifyMotionEffectBase.Instance != null)
		{
			TryInitializeStates();
		}
		m_lastPosition = base.transform.position;
	}

	private void Update()
	{
		if (AmplifyMotionEffectBase.Instance != null)
		{
			TryInitializeStates();
		}
	}

	private static void RecursiveResetMotionAtFrame(Transform transform, AmplifyMotionObjectBase obj, int frame)
	{
		if (obj != null)
		{
			obj.m_resetAtFrame = frame;
		}
		foreach (Transform item in transform)
		{
			RecursiveResetMotionAtFrame(item, item.GetComponent<AmplifyMotionObjectBase>(), frame);
		}
	}

	public void ResetMotionNow()
	{
		RecursiveResetMotionAtFrame(base.transform, this, Time.frameCount);
	}

	public void ResetMotionAtFrame(int frame)
	{
		RecursiveResetMotionAtFrame(base.transform, this, frame);
	}

	private void CheckTeleportReset(AmplifyMotionEffectBase inst)
	{
		if (Vector3.SqrMagnitude(base.transform.position - m_lastPosition) > inst.MinResetDeltaDistSqr)
		{
			RecursiveResetMotionAtFrame(base.transform, this, Time.frameCount + inst.ResetFrameDelay);
		}
	}

	internal void OnUpdateTransform(AmplifyMotionEffectBase inst, Camera camera, CommandBuffer updateCB, bool starting)
	{
		if (m_states.TryGetValue(camera, out var value) && !value.Error)
		{
			CheckTeleportReset(inst);
			bool flag = m_resetAtFrame > 0 && Time.frameCount >= m_resetAtFrame;
			value.UpdateTransform(updateCB, starting || flag);
		}
		m_lastPosition = base.transform.position;
	}

	internal void OnRenderVectors(Camera camera, CommandBuffer renderCB, float scale, Quality quality)
	{
		if (m_states.TryGetValue(camera, out var value) && !value.Error)
		{
			value.RenderVectors(camera, renderCB, scale, quality);
			if (m_resetAtFrame > 0 && Time.frameCount >= m_resetAtFrame)
			{
				m_resetAtFrame = -1;
			}
		}
	}
}
