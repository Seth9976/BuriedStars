using System;
using UnityEngine;

namespace GameEvent;

public class EventCamera
{
	private enum SHAKE_OPT
	{
		FIX_VALUE,
		RAND_TO_ORIGIN,
		RAND_REVERSE,
		RANDOM
	}

	private enum SHAKE_DIR
	{
		HORIZONTAL,
		VERTICAL,
		BOTH
	}

	private EventEngine m_EventEngine;

	private Camera m_CamMain;

	private Vector3 m_vecStandard_Main_Position;

	private bool m_isMoveSet;

	private bool m_isViewSet;

	private Vector3 m_vecStartPos;

	private Vector3 m_vecTargetPos;

	private Vector3 m_vecLookPos;

	private Quaternion m_quatLookStartPos;

	private Transform m_tfCamera;

	private float m_fMoveTime;

	private float m_fViewTime;

	private float m_fMovePassTime;

	private float m_fViewPassTime;

	private int m_iMoveType;

	private int m_iViewType;

	private bool m_isRotateSet;

	private float m_fRotateTime;

	private int m_iCameraRotateType;

	private Vector3 m_vecRotatePos;

	private Vector3 m_vecRotateStart;

	private float m_RotPassTime;

	private int m_iRotType;

	private SHAKE_DIR m_iShakeDir;

	private SHAKE_OPT m_iShakeOpt;

	private bool m_isFirstPhase;

	private bool m_isShake;

	private float[] m_fShakePower = new float[2];

	private float[] m_fShakeGapT = new float[2];

	private float[] m_fShakePlayT = new float[2];

	private Vector3 m_vecBeforeCameraShakePos;

	private float m_fCurShakeGapT;

	private float m_fCurShakePlayT;

	private float m_fCurShakePlayPassT;

	private Vector3 m_vecAddShakePos;

	private Vector3 m_vecBefAddShakePos;

	private Vector3 m_vecBefPlusAddShakePos = Vector3.zero;

	private bool m_isToOrigin;

	private bool m_isShakeEnd = true;

	private float m_fInitHeightAtDist;

	private Vector3 m_vecDollyTargetPos;

	private float m_fDollyPassTime;

	private float m_fDollyMoveVal;

	private int m_iFovType;

	private float m_fFovSet;

	private float m_fFovTime;

	private float m_fFovPassTime;

	private float m_fFovStart;

	private bool m_isExistSavedCamPos;

	private Vector3 m_vecSavedPos;

	private Vector3 m_vecSavedScale;

	private Vector3 m_vecSavedRotate;

	private float m_fSavedFOV;

	public void InitCameraEvtEnd()
	{
		m_isShakeEnd = true;
	}

	public void InitAfterLoad()
	{
		m_CamMain = Camera.main;
		RenderManager instance = RenderManager.instance;
		instance.ReflashRenderCamera();
		m_EventEngine = EventEngine.GetInstance();
		m_tfCamera = m_CamMain.transform;
		m_isExistSavedCamPos = false;
	}

	public void Free()
	{
		m_EventEngine = null;
		m_CamMain = null;
	}

	public void InitMainCameraPos()
	{
		m_CamMain.transform.position = Vector3.zero;
	}

	public void InitMainCameraRotate()
	{
		m_CamMain.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	private void SetMainPos(float fX, float fY, float fZ)
	{
		SetMainPos(new Vector3(fX, fY, fZ));
	}

	private void SetMainPos(Vector3 vecPos)
	{
		if (m_isShake)
		{
			m_vecStandard_Main_Position = vecPos - m_vecBefPlusAddShakePos;
		}
		else
		{
			m_vecStandard_Main_Position = vecPos;
		}
	}

	public bool SetPresetMove(string strObj, float fTime, string strSpeedType)
	{
		GameObject gameObject = GameObject.Find(strObj);
		if (gameObject == null)
		{
			return false;
		}
		return SetMove(gameObject.transform.position, fTime, strSpeedType);
	}

	public bool SetPresetRotate(string strObj, float fTime, string strSpeedType)
	{
		GameObject gameObject = GameObject.Find(strObj);
		if (gameObject == null)
		{
			return false;
		}
		return SetRotate(gameObject.transform.eulerAngles, fTime, strSpeedType);
	}

	public bool SetPresetMoveAndRotate(string strObj, float fTime, string strSpeedType)
	{
		GameObject gameObject = GameObject.Find(strObj);
		if (gameObject == null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		Transform transform = gameObject.transform;
		flag = SetMove(transform.position, fTime, strSpeedType);
		flag2 = SetRotate(transform.eulerAngles, fTime, strSpeedType);
		return flag || flag2;
	}

	public bool SetCameraView(string strLook, float fTime, string strSpeedType)
	{
		m_isViewSet = false;
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		GameObject gameObject = GameObject.Find(strLook);
		if (gameObject == null)
		{
			return false;
		}
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			m_tfCamera.LookAt(gameObject.transform);
			SetMainPos(m_tfCamera.position);
			return false;
		}
		m_isViewSet = true;
		m_vecLookPos = gameObject.transform.position;
		m_fViewTime = fTime;
		m_fViewPassTime = 0f;
		m_iViewType = xlsScriptKeyValue;
		m_quatLookStartPos = m_tfCamera.transform.rotation;
		return true;
	}

	public bool SetMoveObj(string strObj, string strLook, float fTime, string strSpeedType)
	{
		GameObject gameObject = GameObject.Find(strObj);
		if (gameObject == null)
		{
			return false;
		}
		Vector3 position = gameObject.transform.position;
		return SetMoveAndLook(position.x, position.y, position.z, strLook, fTime, strSpeedType);
	}

	public bool SetMoveObjRelative(float fX, float fY, float fZ, string strLook, float fTime, string strSpeedType)
	{
		Vector3 vector = m_tfCamera.position + new Vector3(fX, fY, fZ);
		return SetMoveAndLook(vector.x, vector.y, vector.z, strLook, fTime, strSpeedType);
	}

	public bool SetMoveAndLook(float fX, float fY, float fZ, string strLook, float fTime, string strSpeedType)
	{
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			m_iMoveType = 4;
			SetCamera(fX, fY, fZ, isLookSet: true, strLook);
			SetMainPos(fX, fY, fZ);
			return false;
		}
		SetMove(new Vector3(fX, fY, fZ), fTime, strSpeedType);
		SetCameraView(strLook, fTime, strSpeedType);
		return true;
	}

	public bool SetMove(float fX, float fY, float fZ, float fTime, string strSpeedType)
	{
		return SetMove(new Vector3(fX, fY, fZ), fTime, strSpeedType);
	}

	public bool SetMove(Vector3 vecMovePos, float fTime, string strSpeedType)
	{
		m_isMoveSet = false;
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			m_tfCamera.position = vecMovePos;
			SetMainPos(m_tfCamera.position);
			return false;
		}
		m_isMoveSet = true;
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		m_vecStartPos = m_tfCamera.position;
		m_vecTargetPos = vecMovePos;
		m_fMovePassTime = 0f;
		m_fMoveTime = fTime;
		m_iMoveType = xlsScriptKeyValue;
		return true;
	}

	public bool ProcCameraMoveAndView()
	{
		if (m_iMoveType == 4)
		{
			return true;
		}
		bool flag = !m_isMoveSet || ProcMove();
		return (!m_isViewSet || ProcCameraView()) && flag;
	}

	public bool ProcCameraMoveAndRotate()
	{
		bool flag = !m_isMoveSet || ProcMove();
		bool flag2 = !m_isRotateSet || ProcRotate();
		return flag && flag2;
	}

	public bool ProcCameraView()
	{
		float fSmoothStepFactor = 1f;
		bool flag = m_EventEngine.ProcTime(m_iViewType, ref fSmoothStepFactor, ref m_fViewPassTime, m_fViewTime);
		if (flag)
		{
			m_tfCamera.LookAt(m_vecLookPos);
			SetMainPos(m_tfCamera.position);
			m_isViewSet = false;
		}
		else
		{
			Quaternion b = Quaternion.LookRotation(m_vecLookPos - m_tfCamera.position);
			m_tfCamera.rotation = Quaternion.Lerp(m_quatLookStartPos, b, fSmoothStepFactor);
		}
		return flag;
	}

	public bool ProcMove()
	{
		float fSmoothStepFactor = 1f;
		bool flag = m_EventEngine.ProcTime(m_iMoveType, ref fSmoothStepFactor, ref m_fMovePassTime, m_fMoveTime);
		if (flag)
		{
			m_tfCamera.position = m_vecTargetPos;
			m_isMoveSet = false;
		}
		else
		{
			m_tfCamera.position = Vector3.Lerp(m_vecStartPos, m_vecTargetPos, fSmoothStepFactor);
		}
		SetMainPos(m_tfCamera.position);
		return flag;
	}

	public void SetCamera(float fX, float fY, float fZ, bool isLookSet, string strLook)
	{
		m_tfCamera.position = new Vector3(fX, fY, fZ);
		SetMainPos(m_tfCamera.position);
		GameObject gameObject = GameObject.Find(strLook);
		if (isLookSet && gameObject != null)
		{
			m_tfCamera.LookAt(gameObject.transform);
		}
	}

	public float CorrectDegree(float fDegree)
	{
		if (fDegree < -180f)
		{
			fDegree += 360f;
		}
		else if (fDegree > 180f)
		{
			fDegree -= 360f;
		}
		while (Mathf.Abs(fDegree) > 180f)
		{
			CorrectDegree(fDegree);
		}
		return fDegree;
	}

	public bool PlusRotate(float fRotX, float fRotY, float fRotZ, float fTime, string strRotType)
	{
		float fX = CorrectDegree(Mathf.Round(m_tfCamera.transform.eulerAngles.x + fRotX));
		float fY = CorrectDegree(Mathf.Round(m_tfCamera.transform.eulerAngles.y + fRotY));
		float fZ = CorrectDegree(Mathf.Round(m_tfCamera.transform.eulerAngles.z + fRotZ));
		SetRotate(fX, fY, fZ, fTime, strRotType);
		return true;
	}

	public bool SetRotate(float fX, float fY, float fZ, float fTime, string strRotType)
	{
		return SetRotate(new Vector3(fX, fY, fZ), fTime, strRotType);
	}

	public bool SetRotate(Vector3 vecRotate, float fTime, string strRotType)
	{
		m_isRotateSet = false;
		m_vecRotatePos = vecRotate;
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			m_tfCamera.rotation = Quaternion.Euler(m_vecRotatePos);
			SetMainPos(m_tfCamera.position);
			return false;
		}
		m_isRotateSet = true;
		int xlsScriptKeyValue = GameGlobalUtil.GetXlsScriptKeyValue(strRotType);
		m_iRotType = xlsScriptKeyValue;
		m_fRotateTime = fTime;
		m_RotPassTime = 0f;
		m_vecRotateStart = m_tfCamera.transform.eulerAngles;
		return true;
	}

	public bool ProcRotate()
	{
		float fSmoothStepFactor = 1f;
		bool flag = m_EventEngine.ProcTime(m_iRotType, ref fSmoothStepFactor, ref m_RotPassTime, m_fRotateTime);
		if (flag)
		{
			m_tfCamera.rotation = Quaternion.Euler(m_vecRotatePos);
			m_isRotateSet = false;
		}
		else
		{
			m_tfCamera.rotation = Quaternion.Lerp(Quaternion.Euler(m_vecRotateStart), Quaternion.Euler(m_vecRotatePos), fSmoothStepFactor);
		}
		return flag;
	}

	public void ShakeCamera(bool isOn, string strXls = "", string strShakeOpt = "", string strShakeDir = "")
	{
		if (isOn)
		{
			SetMainPos(m_tfCamera.position);
			m_vecBefPlusAddShakePos = (m_vecBefAddShakePos = (m_vecBeforeCameraShakePos = Vector3.zero));
			m_vecAddShakePos = Vector3.zero;
			Xls.CameraShakeValue data_byKey = Xls.CameraShakeValue.GetData_byKey(strXls);
			m_isToOrigin = false;
			if (data_byKey != null)
			{
				m_fShakePower[0] = data_byKey.m_fPowerMin;
				m_fShakePower[1] = data_byKey.m_fPowerMax;
				m_fShakeGapT[0] = data_byKey.m_fGapMin;
				m_fShakeGapT[1] = data_byKey.m_fGapMax;
				m_fShakePlayT[0] = data_byKey.m_fPlayTMin;
				m_fShakePlayT[1] = data_byKey.m_fPlayTMax;
			}
			m_iShakeOpt = (SHAKE_OPT)GameGlobalUtil.GetXlsScriptKeyValue(strShakeOpt);
			m_iShakeDir = (SHAKE_DIR)GameGlobalUtil.GetXlsScriptKeyValue(strShakeDir);
			m_fCurShakePlayPassT = (m_fCurShakePlayT = 0f);
			m_fCurShakeGapT = 0f;
			m_isShakeEnd = false;
			m_isFirstPhase = true;
		}
		else
		{
			m_fCurShakeGapT = 0f;
			m_isToOrigin = true;
			m_fCurShakePlayPassT = (m_fCurShakePlayT = 0f);
			m_vecBefPlusAddShakePos = (m_vecBefAddShakePos = (m_vecBeforeCameraShakePos = Vector3.zero));
		}
		m_isShake = isOn;
	}

	public void ProcShakeCamera()
	{
		if (m_isShakeEnd)
		{
			return;
		}
		if (m_fCurShakeGapT <= 0f && m_fCurShakePlayPassT >= m_fCurShakePlayT)
		{
			bool flag = m_isToOrigin;
			bool flag2 = false;
			float x = 0f;
			float y = 0f;
			bool flag3 = false;
			m_fCurShakePlayT = UnityEngine.Random.Range(m_fShakePlayT[0], m_fShakePlayT[1]);
			m_fCurShakePlayPassT = 0f;
			if (!m_isToOrigin)
			{
				float num = 0f;
				float num2 = 0f;
				switch (m_iShakeOpt)
				{
				case SHAKE_OPT.FIX_VALUE:
					num = (num2 = ((!m_isFirstPhase) ? m_fShakePower[1] : m_fShakePower[0]));
					break;
				case SHAKE_OPT.RAND_TO_ORIGIN:
					if (!m_isFirstPhase)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
					break;
				case SHAKE_OPT.RAND_REVERSE:
					flag2 = true;
					if (!m_isFirstPhase)
					{
						flag3 = true;
					}
					break;
				case SHAKE_OPT.RANDOM:
					flag2 = true;
					break;
				}
				if (flag2)
				{
					num = UnityEngine.Random.Range(m_fShakePower[0], m_fShakePower[1]);
					if (num == 0f)
					{
						num = m_fShakePower[0];
					}
					num2 = UnityEngine.Random.Range(m_fShakePower[0], m_fShakePower[1]);
					if (num2 == 0f)
					{
						num2 = m_fShakePower[0];
					}
					if (flag3)
					{
						if ((m_vecAddShakePos.x > 0f && num > 0f) || (m_vecAddShakePos.x < 0f && num < 0f))
						{
							num = 0f - num;
						}
						if ((m_vecAddShakePos.y > 0f && num2 > 0f) || (m_vecAddShakePos.x < 0f && num < 0f))
						{
							num2 = 0f - num2;
						}
					}
				}
				switch (m_iShakeDir)
				{
				case SHAKE_DIR.HORIZONTAL:
					x = num;
					break;
				case SHAKE_DIR.VERTICAL:
					y = num;
					break;
				case SHAKE_DIR.BOTH:
					x = num;
					y = num2;
					break;
				}
				m_isFirstPhase = !m_isFirstPhase;
			}
			m_vecBefAddShakePos = m_vecAddShakePos;
			m_vecAddShakePos = new Vector3(x, y, 0f);
			if (flag)
			{
				m_fCurShakeGapT = 0f;
			}
			else
			{
				m_fCurShakeGapT = UnityEngine.Random.Range(m_fShakeGapT[0], m_fShakeGapT[1]);
			}
		}
		else if (m_fCurShakePlayPassT < m_fCurShakePlayT)
		{
			m_fCurShakePlayPassT += Time.deltaTime;
			Vector3 position = m_vecStandard_Main_Position + m_vecAddShakePos;
			float t = m_fCurShakePlayPassT / m_fCurShakePlayT;
			Vector3 vector = Vector3.Lerp(m_vecBefAddShakePos, m_vecAddShakePos, t);
			Vector3 vector2 = vector - m_vecBeforeCameraShakePos;
			m_vecBefPlusAddShakePos = vector;
			m_tfCamera.position = m_vecStandard_Main_Position + vector;
			if (m_fCurShakePlayPassT >= m_fCurShakePlayT)
			{
				m_tfCamera.position = position;
				if (!m_isShake)
				{
					m_isShakeEnd = true;
				}
			}
			Vector3 vecShake = vector2 * 20f / 100f;
			vecShake.z = 0f;
			m_EventEngine.m_TalkChar.ChangeCharPosByCameraShake(vecShake);
			m_vecBeforeCameraShakePos = vector;
		}
		else
		{
			m_fCurShakeGapT -= Time.deltaTime;
		}
	}

	public void SetDollyZoom(float fTargetDistance, float fMoveVal, float fTime)
	{
		m_fDollyPassTime = fTime;
		m_vecDollyTargetPos = m_tfCamera.position - new Vector3(0f, 0f, fTargetDistance);
		float distance = Vector3.Distance(m_tfCamera.position, m_vecDollyTargetPos);
		m_fInitHeightAtDist = FrustumHeightAtDistance(distance);
		m_fDollyMoveVal = fMoveVal;
	}

	private float FrustumHeightAtDistance(float distance)
	{
		return 2f * distance * Mathf.Tan(m_CamMain.fieldOfView * 0.5f * ((float)Math.PI / 180f));
	}

	private float FOVForHeightAndDistance(float height, float distance)
	{
		return 2f * Mathf.Atan(height * 0.5f / distance) * 57.29578f;
	}

	public bool ProcDollyZoom()
	{
		m_fDollyPassTime -= Time.deltaTime * m_EventEngine.GetLerpSkipValue();
		if (m_fDollyPassTime < 0f)
		{
			return true;
		}
		float distance = Vector3.Distance(m_tfCamera.position, m_vecDollyTargetPos);
		m_CamMain.fieldOfView = FOVForHeightAndDistance(m_fInitHeightAtDist, distance);
		RenderManager.instance.SetFOV_SubCameras(m_CamMain.fieldOfView);
		m_tfCamera.Translate(m_fDollyMoveVal * Vector3.forward * Time.deltaTime);
		SetMainPos(m_tfCamera.position);
		return false;
	}

	public bool SetFov(float fFov, float fTime, string strSpeedType)
	{
		m_iFovType = GameGlobalUtil.GetXlsScriptKeyValue(strSpeedType);
		m_fFovSet = fFov;
		if (fTime <= EventEngine.EVENT_IMMEDIATE_TIME)
		{
			m_CamMain.fieldOfView = m_fFovSet;
			RenderManager.instance.SetFOV_SubCameras(m_CamMain.fieldOfView);
			return false;
		}
		m_fFovStart = m_CamMain.fieldOfView;
		m_fFovTime = fTime;
		m_fFovPassTime = 0f;
		return true;
	}

	public bool ProcFov()
	{
		float fSmoothStepFactor = 1f;
		bool flag = m_EventEngine.ProcTime(m_iFovType, ref fSmoothStepFactor, ref m_fFovPassTime, m_fFovTime);
		if (flag)
		{
			m_CamMain.fieldOfView = m_fFovSet;
		}
		else
		{
			m_CamMain.fieldOfView = Mathf.Lerp(m_fFovStart, m_fFovSet, fSmoothStepFactor);
		}
		RenderManager.instance.SetFOV_SubCameras(m_CamMain.fieldOfView);
		return flag;
	}

	public void SetCurCameraToBuf()
	{
		if (!(m_CamMain == null))
		{
			Transform transform = m_CamMain.transform;
			m_vecSavedPos = transform.position;
			m_vecSavedScale = transform.localScale;
			m_vecSavedRotate = transform.eulerAngles;
			m_fSavedFOV = m_CamMain.fieldOfView;
			m_isExistSavedCamPos = true;
		}
	}

	public void SetBufToCurCamera()
	{
		if (m_isExistSavedCamPos)
		{
			Transform transform = m_CamMain.transform;
			transform.position = m_vecSavedPos;
			transform.localScale = m_vecSavedScale;
			transform.localEulerAngles = m_vecSavedRotate;
			m_CamMain.fieldOfView = m_fSavedFOV;
			RenderManager.instance.SetFOV_SubCameras(m_CamMain.fieldOfView);
		}
	}
}
