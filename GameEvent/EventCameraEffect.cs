using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Colorful;
using GameData;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace GameEvent;

public class EventCameraEffect
{
	public enum Effects
	{
		DoubleVision,
		Glitch,
		Wiggle,
		AnalogTV,
		ScreenOverlay,
		CameraFlare,
		GrayScale,
		Negative,
		ColorCurve,
		GaussianBlur,
		RadialBlur,
		Cover,
		Flash,
		BackgroundStream,
		FocusLine,
		FocusLine_Hori,
		FallingStone,
		FallingStoneBlur,
		KeywordMenuBG,
		SNSMenuBG
	}

	public enum CoverType
	{
		Unknown,
		Covered,
		Fade,
		FadeCircle,
		Normal,
		Flat,
		FlatBlur,
		Brush,
		Concrete,
		ConcreteBlur,
		EyeView
	}

	private enum EffectState
	{
		Disable,
		Enable,
		Activating,
		Deactivating
	}

	private delegate void updateEffectProc();

	private delegate void changeCameraProc(Camera newCam);

	private class EffectInfo
	{
		public int m_LayerID;

		public EffectState m_State;

		public GameObject m_GameObj;

		public MonoBehaviour m_Compnent;

		public object[] m_Params;

		public updateEffectProc m_fpUpdate;

		public changeCameraProc m_fpChangeCam;

		public CoverType m_CoverType;

		public bool m_isLoadingAsset;

		public string m_assetPath;
	}

	private struct SaveData
	{
		public uint m_uiActivedFlag;

		public int m_dvLayerFlag;

		public float m_dvMaxAmount;

		public float m_dvDisplaceX;

		public float m_dvDisplaceY;

		public int m_glitchLayerFlag;

		public float m_glitchSpeed;

		public float m_glitchDensity;

		public float m_glitchMaxDisplacement;

		public int m_wiggleLayerFlag;

		public float m_wiggleSpeed;

		public float m_wiggleFreq;

		public float m_wiggleAmplitude;

		public int m_analogTvLayerFlag;

		public float m_analogTvNoiseIntensity;

		public int m_soLayerFlag;

		public int m_soTextureNameLength;

		public float m_soMaxIntensity;

		public int m_cfLayerFlag;

		public float m_cfWorldPosX;

		public float m_cfWorldPosY;

		public float m_cfScaleX;

		public float m_cfScaleY;

		public float m_cfMaxAmount;

		public int m_cfPrefabNameLength;

		public int m_grayScaleLayerFlag;

		public float m_grayScaleMaxAmount;

		public int m_negativeLayerFlag;

		public float m_negativeMaxAmount;

		public int m_cccLayerFlag;

		public float m_cccRedSteepness;

		public float m_cccRedGamma;

		public float m_cccGreenSteepness;

		public float m_cccGreenGamma;

		public float m_cccBlueSteepness;

		public float m_cccBlueGamma;

		public int m_gbLayerFlag;

		public int m_gbPasses;

		public float m_gbDownscaling;

		public int m_rbLayerFlag;

		public float m_rbMaxStrength;

		public float m_rbCenterPosX;

		public float m_rbCenterPosY;

		public bool m_rbEnableVignette;

		public float m_rbSharpness;

		public float m_rbDarkness;

		public int m_coverLayerFlag;

		public float m_coverMaxAmount;

		public int m_coverR;

		public int m_coverG;

		public int m_coverB;

		public int m_flashLayerFlag;

		public float m_flashMaxAmount;

		public int m_flashR;

		public int m_flashG;

		public int m_flashB;

		public int m_bgStreamLayerFlag;

		public int m_bgStreamType;

		public float m_bgStreamSpeedRate;

		public int m_bgStreamOrderInLayer;

		public int m_focusLineLayerFlag;

		public int m_focusLineOrderInLayer;

		public int m_focusLineHoriLayerFlag;

		public int m_focusLineHoriOrderInLayer;

		public int m_fsLayerFlag;

		public int m_fsType;

		public float m_fsSpeedRate;

		public int m_fsOrderInLayer;

		public int m_fsbLayerFlag;

		public int m_fsbType;

		public float m_fsbSpeedRate;

		public int m_fsbOrderInLayer;

		public int m_kmbgNoiseLevel;

		public static int GetSize()
		{
			return Marshal.SizeOf(typeof(SaveData));
		}

		public void Save(byte[] saveBuffer, ref int offset)
		{
			BitCalc.IntToByteNCO((int)m_uiActivedFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_dvLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_dvMaxAmount, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_dvDisplaceX, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_dvDisplaceX, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_glitchLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_glitchSpeed, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_glitchDensity, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_glitchMaxDisplacement, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_wiggleLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_wiggleSpeed, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_wiggleFreq, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_wiggleAmplitude, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_analogTvLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_analogTvNoiseIntensity, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_soLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_soTextureNameLength, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_soMaxIntensity, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_cfLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cfWorldPosX, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cfWorldPosY, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cfScaleX, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cfScaleY, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cfMaxAmount, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_cfPrefabNameLength, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_grayScaleLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_grayScaleMaxAmount, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_negativeLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_negativeMaxAmount, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_cccLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccRedSteepness, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccRedGamma, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccGreenSteepness, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccGreenGamma, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccBlueSteepness, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_cccBlueGamma, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_gbLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_gbPasses, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_gbDownscaling, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_rbLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_rbMaxStrength, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_rbCenterPosX, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_rbCenterPosY, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_rbEnableVignette ? 1 : 0, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_rbSharpness, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_rbDarkness, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_coverLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_coverMaxAmount, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_coverR, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_coverG, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_coverB, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_flashLayerFlag, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_flashMaxAmount, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_flashR, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_flashG, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_flashB, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_bgStreamLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_bgStreamType, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_bgStreamSpeedRate, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_bgStreamOrderInLayer, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_focusLineLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_focusLineOrderInLayer, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_focusLineHoriLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_focusLineHoriOrderInLayer, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsType, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_fsSpeedRate, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsOrderInLayer, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsbLayerFlag, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsbType, saveBuffer, ref offset);
			BitCalc.FloatToByteNCO(m_fsbSpeedRate, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_fsbOrderInLayer, saveBuffer, ref offset);
			BitCalc.IntToByteNCO(m_kmbgNoiseLevel, saveBuffer, ref offset);
		}

		public void Load(byte[] loadBuffer, ref int offset)
		{
			m_uiActivedFlag = (uint)BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_dvLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_dvMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_dvDisplaceX = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_dvDisplaceX = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_glitchLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_glitchSpeed = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_glitchDensity = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_glitchMaxDisplacement = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_wiggleLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_wiggleSpeed = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_wiggleFreq = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_wiggleAmplitude = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_analogTvLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_analogTvNoiseIntensity = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_soLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_soTextureNameLength = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_soMaxIntensity = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_cfWorldPosX = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfWorldPosY = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfScaleX = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfScaleY = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cfPrefabNameLength = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_grayScaleLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_grayScaleMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_negativeLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_negativeMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_cccRedSteepness = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccRedGamma = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccGreenSteepness = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccGreenGamma = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccBlueSteepness = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_cccBlueGamma = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_gbLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_gbPasses = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_gbDownscaling = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_rbLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_rbMaxStrength = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_rbCenterPosX = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_rbCenterPosY = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_rbEnableVignette = BitCalc.ByteToIntNCO(loadBuffer, ref offset) != 0;
			m_rbSharpness = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_rbDarkness = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_coverLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_coverMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_coverR = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_coverG = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_coverB = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_flashLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_flashMaxAmount = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_flashR = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_flashG = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_flashB = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_bgStreamLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_bgStreamType = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_bgStreamSpeedRate = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_bgStreamOrderInLayer = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_focusLineLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_focusLineOrderInLayer = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_focusLineHoriLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_focusLineHoriOrderInLayer = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsType = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsSpeedRate = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_fsOrderInLayer = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsbLayerFlag = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsbType = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_fsbSpeedRate = BitCalc.ByteToFloatNCO(loadBuffer, ref offset);
			m_fsbOrderInLayer = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
			m_kmbgNoiseLevel = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
		}
	}

	private delegate void delSaveProc(EffectInfo effectInfo, ref SaveData saveData, bool isActivated);

	private static EventCameraEffect s_Instance;

	private const string c_strAssetName_CameraFlareTypeDef = "Prefabs/CameraEffect/CameraFlareType1";

	private const string c_strAssetName_CoverInPrefab = "Prefabs/CameraEffect/Cover_Eff_in";

	private const string c_strAssetName_CoverOutPrefab = "Prefabs/CameraEffect/Cover_Eff_out";

	private const string c_strAssetName_CoverFlatInPrefab = "Prefabs/CameraEffect/Cover_Eff_Flat_in";

	private const string c_strAssetName_CoverFlatOutPrefab = "Prefabs/CameraEffect/Cover_Eff_Flat_out";

	private const string c_strAssetName_CoverFlatBlurInPrefab = "Prefabs/CameraEffect/Cover_Eff_flat_blur_in";

	private const string c_strAssetName_CoverFlatBlurOutPrefab = "Prefabs/CameraEffect/Cover_Eff_flat_blur_out";

	private const string c_strAssetName_CoverBrushInPrefab = "Prefabs/CameraEffect/Cover_Brush_Eff_in";

	private const string c_strAssetName_CoverBrushOutPrefab = "Prefabs/CameraEffect/Cover_Brush_Eff_out";

	private const string c_strAssetName_CoverConcreteInPrefab = "Prefabs/CameraEffect/Cover_Eff_concrete_in";

	private const string c_strAssetName_CoverConcreteOutPrefab = "Prefabs/CameraEffect/Cover_Eff_concrete_out";

	private const string c_strAssetName_CoverConcreteBlurInPrefab = "Prefabs/CameraEffect/Cover_Eff_concrete_blur_in";

	private const string c_strAssetName_CoverConcreteBlurOutPrefab = "Prefabs/CameraEffect/Cover_Eff_concrete_blur_out";

	private const string c_strAssetName_CoverEyeOpen = "Prefabs/CameraEffect/Eye_Blur";

	private const string c_strAssetName_CoverEyeClose = "Prefabs/CameraEffect/Eye_Blur_out";

	private const string c_strAssetName_BGStreamPrefab = "Prefabs/CameraEffect/BackgroundFlow";

	private const string c_strAssetName_BGStreamVertPrefab = "Prefabs/CameraEffect/BackgroundFlow_Vertical";

	private const string c_strAssetName_FocusLinePrefab = "Prefabs/CameraEffect/Intensive_Line";

	private const string c_strAssetName_FocusLineHoriPrefab = "Prefabs/CameraEffect/Intensive_Line_Horizontal";

	private const string c_strAssetName_KeywordMenuBGPrefab = "Prefabs/CameraEffect/Keyword_CHRBG_Eff";

	private const string c_strAssetName_SNSMenuBGPrefab = "Prefabs/CameraEffect/SNS_CHRBG_Eff";

	private const string c_strAssetName_FallingStone = "Prefabs/CameraEffect/Falling_Stone";

	private const string c_strAssetName_FallingStoneBlur = "Prefabs/CameraEffect/Falling_Stone_blur";

	private Dictionary<string, AssetBundleObjectHandler> m_dicAssetBundleObjectHdrs = new Dictionary<string, AssetBundleObjectHandler>();

	private EffectInfo[] m_EffectInfos;

	private Camera m_CurCamera;

	private GameObject m_CanvasObj;

	private Canvas m_CanvasComponent;

	private SaveData m_SavedData;

	private string m_SavedScreenOverlayTextureName = string.Empty;

	private string m_SavedCameraFlarePrefabName = string.Empty;

	private bool m_returnValue_CoverInCommonCanvasObj;

	private bool m_returnValue_CoverOutCommonCanvasObj;

	public static EventCameraEffect Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new EventCameraEffect();
			}
			return s_Instance;
		}
	}

	private EventCameraEffect()
	{
		int num = Enum.GetNames(typeof(Effects)).Length;
		m_EffectInfos = new EffectInfo[num];
		m_CurCamera = Camera.main;
	}

	public static void ReleaseInstance()
	{
		s_Instance = null;
	}

	~EventCameraEffect()
	{
		m_EffectInfos = null;
	}

	public IEnumerator PreLoadPrefabAssets()
	{
		ClearSrcGameObjects();
		string[] assetPaths = new string[23]
		{
			"Prefabs/CameraEffect/CameraFlareType1", "Prefabs/CameraEffect/Cover_Eff_in", "Prefabs/CameraEffect/Cover_Eff_out", "Prefabs/CameraEffect/Cover_Eff_Flat_in", "Prefabs/CameraEffect/Cover_Eff_Flat_out", "Prefabs/CameraEffect/Cover_Eff_flat_blur_in", "Prefabs/CameraEffect/Cover_Eff_flat_blur_out", "Prefabs/CameraEffect/Cover_Brush_Eff_in", "Prefabs/CameraEffect/Cover_Brush_Eff_out", "Prefabs/CameraEffect/Cover_Eff_concrete_in",
			"Prefabs/CameraEffect/Cover_Eff_concrete_out", "Prefabs/CameraEffect/Cover_Eff_concrete_blur_in", "Prefabs/CameraEffect/Cover_Eff_concrete_blur_out", "Prefabs/CameraEffect/Eye_Blur", "Prefabs/CameraEffect/Eye_Blur_out", "Prefabs/CameraEffect/BackgroundFlow", "Prefabs/CameraEffect/BackgroundFlow_Vertical", "Prefabs/CameraEffect/Intensive_Line", "Prefabs/CameraEffect/Intensive_Line_Horizontal", "Prefabs/CameraEffect/Keyword_CHRBG_Eff",
			"Prefabs/CameraEffect/SNS_CHRBG_Eff", "Prefabs/CameraEffect/Falling_Stone", "Prefabs/CameraEffect/Falling_Stone_blur"
		};
		string[] array = assetPaths;
		foreach (string assetPath in array)
		{
			AssetBundleObjectHandler asObjectHdr = new AssetBundleObjectHandler(assetPath);
			yield return asObjectHdr.LoadAssetBundle();
			if (asObjectHdr.loadedAssetBundleObject != null)
			{
				m_dicAssetBundleObjectHdrs.Add(assetPath, asObjectHdr);
			}
		}
	}

	public void ClearSrcGameObjects()
	{
		Dictionary<string, AssetBundleObjectHandler>.ValueCollection.Enumerator enumerator = m_dicAssetBundleObjectHdrs.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current?.UnloadAssetBundle();
		}
		m_dicAssetBundleObjectHdrs.Clear();
	}

	private string GetPrefabAssetPath(Effects effType, CoverType coverType = CoverType.Unknown, int subType = 0)
	{
		return effType switch
		{
			Effects.Cover => coverType switch
			{
				CoverType.Normal => (subType != 0) ? "Prefabs/CameraEffect/Cover_Eff_out" : "Prefabs/CameraEffect/Cover_Eff_in", 
				CoverType.Flat => (subType != 0) ? "Prefabs/CameraEffect/Cover_Eff_Flat_out" : "Prefabs/CameraEffect/Cover_Eff_Flat_in", 
				CoverType.FlatBlur => (subType != 0) ? "Prefabs/CameraEffect/Cover_Eff_flat_blur_out" : "Prefabs/CameraEffect/Cover_Eff_flat_blur_in", 
				CoverType.Brush => (subType != 0) ? "Prefabs/CameraEffect/Cover_Brush_Eff_out" : "Prefabs/CameraEffect/Cover_Brush_Eff_in", 
				CoverType.Concrete => (subType != 0) ? "Prefabs/CameraEffect/Cover_Eff_concrete_out" : "Prefabs/CameraEffect/Cover_Eff_concrete_in", 
				CoverType.ConcreteBlur => (subType != 0) ? "Prefabs/CameraEffect/Cover_Eff_concrete_blur_out" : "Prefabs/CameraEffect/Cover_Eff_concrete_blur_in", 
				CoverType.EyeView => (subType != 0) ? "Prefabs/CameraEffect/Eye_Blur" : "Prefabs/CameraEffect/Eye_Blur_out", 
				_ => null, 
			}, 
			Effects.BackgroundStream => (subType != 0) ? "Prefabs/CameraEffect/BackgroundFlow_Vertical" : "Prefabs/CameraEffect/BackgroundFlow", 
			Effects.FocusLine => "Prefabs/CameraEffect/Intensive_Line", 
			Effects.FocusLine_Hori => "Prefabs/CameraEffect/Intensive_Line_Horizontal", 
			Effects.FallingStone => "Prefabs/CameraEffect/Falling_Stone", 
			Effects.FallingStoneBlur => "Prefabs/CameraEffect/Falling_Stone", 
			_ => null, 
		};
	}

	private GameObject GetPrefabSrcObject(Effects effType, CoverType coverType = CoverType.Unknown, int subType = 0)
	{
		string prefabAssetPath = GetPrefabAssetPath(effType, coverType, subType);
		if (string.IsNullOrEmpty(prefabAssetPath))
		{
			return null;
		}
		return GetPrefabSrcObject(prefabAssetPath);
	}

	private GameObject GetPrefabSrcObject(string assetPath)
	{
		return (!m_dicAssetBundleObjectHdrs.ContainsKey(assetPath)) ? null : m_dicAssetBundleObjectHdrs[assetPath].GetLoadedAsset_ToGameObject();
	}

	private string GetAssetBundleName(string assetPath)
	{
		return (!m_dicAssetBundleObjectHdrs.ContainsKey(assetPath)) ? null : m_dicAssetBundleObjectHdrs[assetPath].assetBundleName;
	}

	private AssetBundleObjectHandler GetAssetBundleObjectHandler(string assetPath)
	{
		return (!m_dicAssetBundleObjectHdrs.ContainsKey(assetPath)) ? null : m_dicAssetBundleObjectHdrs[assetPath];
	}

	private IEnumerator LoadPrefabSrcObject(string assetPath, bool isOneFileBundle = true)
	{
		if (!m_dicAssetBundleObjectHdrs.ContainsKey(assetPath))
		{
			AssetBundleObjectHandler asObjectHdr = new AssetBundleObjectHandler(assetPath, isOneFileBundle);
			yield return asObjectHdr.LoadAssetBundle();
			if (asObjectHdr.loadedAssetBundleObject != null)
			{
				m_dicAssetBundleObjectHdrs.Add(assetPath, asObjectHdr);
			}
		}
	}

	private void UnloadPrefabSrcObject(string assetPath)
	{
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler(assetPath);
		if (assetBundleObjectHandler != null)
		{
			m_dicAssetBundleObjectHdrs.Remove(assetPath);
			assetBundleObjectHandler.UnloadAssetBundle();
		}
	}

	public void Clear()
	{
		if (m_EffectInfos != null)
		{
			EffectInfo effectInfo = null;
			int num = m_EffectInfos.Length;
			for (int i = 0; i < num; i++)
			{
				effectInfo = m_EffectInfos[i];
				if (effectInfo != null)
				{
					if (effectInfo.m_Compnent != null)
					{
						UnityEngine.Object.Destroy(effectInfo.m_Compnent);
						effectInfo.m_Compnent = null;
					}
					if (effectInfo.m_GameObj != null)
					{
						UnityEngine.Object.Destroy(effectInfo.m_GameObj);
						effectInfo.m_GameObj = null;
					}
					effectInfo.m_State = EffectState.Disable;
					effectInfo.m_isLoadingAsset = false;
					effectInfo.m_assetPath = null;
				}
			}
		}
		if (m_CanvasObj != null)
		{
			UnityEngine.Object.Destroy(m_CanvasObj);
			m_CanvasObj = null;
		}
		m_CanvasComponent = null;
	}

	public void SetCurrentCamera(Camera cam = null)
	{
		if (cam == null)
		{
			cam = Camera.main;
		}
		if (m_CurCamera == cam)
		{
			return;
		}
		int num = m_EffectInfos.Length;
		EffectInfo effectInfo = null;
		for (int i = 0; i < num; i++)
		{
			effectInfo = m_EffectInfos[i];
			if (effectInfo != null && !(effectInfo.m_Compnent == null) && effectInfo.m_fpChangeCam != null)
			{
				Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(effectInfo.m_LayerID);
				effectInfo.m_fpChangeCam(camera_byLayerId);
			}
		}
		if (m_CanvasComponent != null)
		{
			m_CanvasComponent.worldCamera = RenderManager.instance.GetCamera_byLayerId(m_CanvasComponent.gameObject.layer);
			m_CanvasComponent.planeDistance = m_CanvasComponent.worldCamera.nearClipPlane + 0.1f;
		}
		m_CurCamera = cam;
	}

	public void Update()
	{
		Camera main = Camera.main;
		if (m_CurCamera != main)
		{
			SetCurrentCamera(main);
		}
		int num = m_EffectInfos.Length;
		EffectInfo effectInfo = null;
		for (int i = 0; i < num; i++)
		{
			effectInfo = m_EffectInfos[i];
			if (effectInfo != null && effectInfo.m_State != EffectState.Disable && effectInfo.m_fpUpdate != null)
			{
				effectInfo.m_fpUpdate();
			}
		}
	}

	public bool IsCompleteEffectProc(Effects effType)
	{
		EffectInfo effectInfo = m_EffectInfos[(int)effType];
		if (effectInfo == null)
		{
			return true;
		}
		return !effectInfo.m_isLoadingAsset && (effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable);
	}

	public bool IsActivatedEffect(Effects effect)
	{
		EffectInfo effectInfo = m_EffectInfos[(int)effect];
		return effectInfo != null && effectInfo.m_State != EffectState.Disable;
	}

	public bool IsActivatedAnyCoveredEffect()
	{
		return IsActivatedEffect(Effects.Cover);
	}

	public Color GetFillCameraColor()
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo != null && effectInfo.m_State == EffectState.Enable)
		{
			ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
			if (screenFade != null)
			{
				return new Color(screenFade.Red, screenFade.Green, screenFade.Blue);
			}
		}
		effectInfo = m_EffectInfos[12];
		if (effectInfo != null && effectInfo.m_State == EffectState.Enable)
		{
			ScreenFade screenFade2 = effectInfo.m_Compnent as ScreenFade;
			if (screenFade2 != null)
			{
				return new Color(screenFade2.Red, screenFade2.Green, screenFade2.Blue);
			}
		}
		return new Color(0f, 0f, 0f, 0f);
	}

	public void GetBoundLayerIds(ref SortedList<int, int> list)
	{
		if (list == null)
		{
			list = new SortedList<int, int>();
		}
		list.Clear();
		EffectInfo[] effectInfos = m_EffectInfos;
		foreach (EffectInfo effectInfo in effectInfos)
		{
			if (effectInfo != null && effectInfo.m_State != EffectState.Disable && !(effectInfo.m_Compnent == null) && !list.ContainsKey(effectInfo.m_LayerID))
			{
				list.Add(effectInfo.m_LayerID, 1);
			}
		}
	}

	private EffectInfo SetEffectInfo(int layerID, Effects effectType, EffectState state, MonoBehaviour component, int ParamCount)
	{
		EffectInfo effectInfo = m_EffectInfos[(int)effectType];
		if (effectInfo == null)
		{
			effectInfo = new EffectInfo();
			m_EffectInfos[(int)effectType] = effectInfo;
		}
		effectInfo.m_LayerID = layerID;
		effectInfo.m_State = state;
		effectInfo.m_Compnent = component;
		effectInfo.m_isLoadingAsset = false;
		effectInfo.m_assetPath = null;
		if (effectInfo.m_Params == null || effectInfo.m_Params.Length < ParamCount)
		{
			effectInfo.m_Params = new object[ParamCount];
		}
		return effectInfo;
	}

	private void ReleaseEffectInfo(EffectInfo effInfo, bool isDestroyGameObject = false)
	{
		effInfo.m_State = EffectState.Disable;
		effInfo.m_CoverType = CoverType.Unknown;
		effInfo.m_isLoadingAsset = false;
		effInfo.m_assetPath = null;
		if (effInfo.m_Compnent != null)
		{
			UnityEngine.Object.DestroyImmediate(effInfo.m_Compnent);
			effInfo.m_Compnent = null;
		}
		if (effInfo.m_GameObj != null)
		{
			if (isDestroyGameObject)
			{
				UnityEngine.Object.DestroyImmediate(effInfo.m_GameObj);
				effInfo.m_GameObj = null;
			}
			else
			{
				effInfo.m_GameObj.SetActive(value: false);
			}
		}
	}

	private void SetCanvasObject(int iLayerID, int iSortingLayerID, int iOrderInLayer = 0)
	{
		if (m_CanvasObj != null && m_CanvasComponent != null)
		{
			m_CanvasObj.layer = iLayerID;
			m_CanvasComponent.sortingLayerName = SortingLayer.layers[iSortingLayerID].name;
			m_CanvasComponent.sortingOrder = iOrderInLayer;
			m_CanvasComponent.worldCamera = RenderManager.instance.GetCamera_byLayerId(iLayerID);
			m_CanvasComponent.planeDistance = m_CanvasComponent.worldCamera.nearClipPlane + 0.1f;
			return;
		}
		m_CanvasObj = new GameObject();
		m_CanvasObj.name = "Canvas_CameraEffect";
		m_CanvasComponent = m_CanvasObj.AddComponent<Canvas>();
		m_CanvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
		Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(iLayerID);
		if (camera_byLayerId != null)
		{
			m_CanvasComponent.worldCamera = camera_byLayerId;
			m_CanvasComponent.planeDistance = m_CanvasComponent.worldCamera.nearClipPlane + 0.1f;
		}
		m_CanvasObj.layer = iLayerID;
		m_CanvasComponent.sortingLayerName = SortingLayer.layers[iSortingLayerID].name;
		m_CanvasComponent.sortingOrder = iOrderInLayer;
		CanvasScaler canvasScaler = m_CanvasObj.GetComponent<CanvasScaler>();
		if (canvasScaler == null)
		{
			canvasScaler = m_CanvasObj.AddComponent<CanvasScaler>();
		}
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.matchWidthOrHeight = 0f;
		canvasScaler.referencePixelsPerUnit = 100f;
		if (MainLoadThing.instance != null && MainLoadThing.instance.m_goMainLoadThing != null)
		{
			GameObject gameObject = ((!(MainLoadThing.instance.m_goMainLoadThing != null)) ? MainLoadThing.instance.gameObject : MainLoadThing.instance.m_goMainLoadThing);
			m_CanvasObj.GetComponent<Transform>().SetParent(gameObject.GetComponent<Transform>());
		}
	}

	private int GetLayerID_byLayerFlag(int layerFlag)
	{
		return GameDefine.GetLayerID_byCharLayer(layerFlag);
	}

	private int GetLayerFlag_byLayerID(int layerID)
	{
		if (layerID == LayerMask.NameToLayer("CHR_0"))
		{
			return 0;
		}
		if (layerID == LayerMask.NameToLayer("CHR_1"))
		{
			return 1;
		}
		if (layerID == LayerMask.NameToLayer("CHR_2"))
		{
			return 2;
		}
		if (layerID == LayerMask.NameToLayer("CHR_3"))
		{
			return 3;
		}
		if (layerID == LayerMask.NameToLayer("CHR_4"))
		{
			return 4;
		}
		if (layerID == LayerMask.NameToLayer("CHR_BOUND"))
		{
			return 5;
		}
		return 0;
	}

	private int GetSortingLayerID_byLayerFlag(int layerFlag)
	{
		return GameDefine.GetSortLayerID_byCharLayer(layerFlag);
	}

	private Camera GetCamera_byLayerFlag(int layerFlag)
	{
		return RenderManager.instance.GetCamera_byLayerId(GetLayerID_byLayerFlag(layerFlag));
	}

	private ScreenFade GetComponent_ScreenFade(GameObject gameObj, Effects effType)
	{
		ScreenFade[] components = gameObj.GetComponents<ScreenFade>();
		if (components == null || components.Length <= 0)
		{
			return null;
		}
		ScreenFade screenFade = null;
		for (int i = 0; i < components.Length; i++)
		{
			screenFade = components[i];
			if (screenFade.UsedEffType == effType)
			{
				return screenFade;
			}
		}
		return null;
	}

	private void SaveProcDummy(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
	}

	public void SaveCameraEfffectState(byte[] saveBuffer, ref int offset, int iBefVer = -1)
	{
		if (iBefVer != -1)
		{
			int iIdx = offset;
			int iSrc = 0;
			BitCalc.IntToByteNCO(iSrc, saveBuffer, ref offset);
			int size = SaveData.GetSize();
			BitCalc.IntToByteNCO(size, saveBuffer, ref offset);
			m_SavedData.Save(saveBuffer, ref offset);
			if (m_SavedData.m_soTextureNameLength > 0 && !string.IsNullOrEmpty(m_SavedScreenOverlayTextureName))
			{
				int iSrcOffset = 0;
				byte[] bytes = Encoding.Unicode.GetBytes(m_SavedScreenOverlayTextureName);
				BitCalc.ByteToByteNCO(bytes, ref iSrcOffset, saveBuffer, ref offset, bytes.Length);
			}
			if (m_SavedData.m_cfPrefabNameLength > 0 && !string.IsNullOrEmpty(m_SavedCameraFlarePrefabName))
			{
				int iSrcOffset2 = 0;
				byte[] bytes2 = Encoding.Unicode.GetBytes(m_SavedCameraFlarePrefabName);
				BitCalc.ByteToByteNCO(bytes2, ref iSrcOffset2, saveBuffer, ref offset, bytes2.Length);
			}
			BitCalc.IntToByte(offset, saveBuffer, iIdx);
			return;
		}
		SaveData saveData = default(SaveData);
		delSaveProc[] array = new delSaveProc[20]
		{
			SaveDoubleVisionState, SaveGlitchState, SaveWiggleState, SaveAnalogTVState, SaveScreenOverlayState, SaveCameraFlareState, SaveGrayScaleState, SaveNegativeState, SaveColorCurveState, SaveGaussianBlurState,
			SaveRadialBlurState, SaveCoverState, SaveFlashState, SaveBGStreamState, SaveFocusLineState, SaveFocusLineHoriState, SaveFallingStoneState, SaveFallingStoneBlurState, SaveKeywordMenuBGState, SaveProcDummy
		};
		saveData.m_uiActivedFlag = 0u;
		bool flag = false;
		EffectState effectState = EffectState.Disable;
		int num = m_EffectInfos.Length;
		for (int i = 0; i < num; i++)
		{
			flag = false;
			if (m_EffectInfos[i] != null)
			{
				effectState = m_EffectInfos[i].m_State;
				flag = effectState == EffectState.Enable || effectState == EffectState.Activating;
			}
			if (flag)
			{
				saveData.m_uiActivedFlag |= (uint)(1 << i);
			}
			array[i](m_EffectInfos[i], ref saveData, flag);
		}
		int iIdx2 = offset;
		int iSrc2 = 0;
		BitCalc.IntToByteNCO(iSrc2, saveBuffer, ref offset);
		int size2 = SaveData.GetSize();
		BitCalc.IntToByteNCO(size2, saveBuffer, ref offset);
		saveData.Save(saveBuffer, ref offset);
		if (saveData.m_soTextureNameLength > 0 && !string.IsNullOrEmpty(m_SavedScreenOverlayTextureName))
		{
			int iSrcOffset3 = 0;
			byte[] bytes3 = Encoding.Unicode.GetBytes(m_SavedScreenOverlayTextureName);
			BitCalc.ByteToByteNCO(bytes3, ref iSrcOffset3, saveBuffer, ref offset, bytes3.Length);
		}
		if (saveData.m_cfPrefabNameLength > 0 && !string.IsNullOrEmpty(m_SavedCameraFlarePrefabName))
		{
			int iSrcOffset4 = 0;
			byte[] bytes4 = Encoding.Unicode.GetBytes(m_SavedCameraFlarePrefabName);
			BitCalc.ByteToByteNCO(bytes4, ref iSrcOffset4, saveBuffer, ref offset, bytes4.Length);
		}
		BitCalc.IntToByte(offset, saveBuffer, iIdx2);
	}

	public void LoadCameraEfffectState(byte[] loadBuffer, ref int offset)
	{
		m_SavedData = default(SaveData);
		int num = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
		int num2 = BitCalc.ByteToIntNCO(loadBuffer, ref offset);
		if (num2 != SaveData.GetSize())
		{
			offset = num;
			return;
		}
		m_SavedData.Load(loadBuffer, ref offset);
		if (m_SavedData.m_soTextureNameLength > 0)
		{
			byte[] array = new byte[m_SavedData.m_soTextureNameLength];
			int iDstOffset = 0;
			BitCalc.ByteToByteNCO(loadBuffer, ref offset, array, ref iDstOffset, m_SavedData.m_soTextureNameLength);
			m_SavedScreenOverlayTextureName = Encoding.Unicode.GetString(array);
		}
		if (m_SavedData.m_cfPrefabNameLength > 0)
		{
			byte[] array2 = new byte[m_SavedData.m_cfPrefabNameLength];
			int iDstOffset2 = 0;
			BitCalc.ByteToByteNCO(loadBuffer, ref offset, array2, ref iDstOffset2, m_SavedData.m_cfPrefabNameLength);
			m_SavedCameraFlarePrefabName = Encoding.Unicode.GetString(array2);
		}
	}

	public void InitForConvertLoad()
	{
		m_SavedData.m_soTextureNameLength = 0;
		m_SavedData.m_cfPrefabNameLength = 0;
	}

	public IEnumerator ActivateLoadedData()
	{
		Clear();
		uint mask = 0u;
		int effectCount = m_EffectInfos.Length;
		for (int i = 0; i < effectCount; i++)
		{
			mask = (uint)(1 << i);
			if ((m_SavedData.m_uiActivedFlag & mask) == mask)
			{
				switch (i)
				{
				case 0:
					Activate_DoubleVision(m_SavedData.m_dvLayerFlag, 0f, m_SavedData.m_dvMaxAmount, m_SavedData.m_dvDisplaceX, m_SavedData.m_dvDisplaceY);
					break;
				case 1:
					Activate_Glitch(m_SavedData.m_glitchLayerFlag, m_SavedData.m_glitchSpeed, m_SavedData.m_glitchDensity, m_SavedData.m_glitchMaxDisplacement);
					break;
				case 2:
					Activate_Wiggle(m_SavedData.m_wiggleLayerFlag, 0f, m_SavedData.m_wiggleSpeed, m_SavedData.m_wiggleFreq, m_SavedData.m_wiggleAmplitude);
					break;
				case 3:
					Activate_AnalogTV(m_SavedData.m_analogTvLayerFlag, m_SavedData.m_analogTvNoiseIntensity);
					break;
				case 4:
					yield return MainLoadThing.instance.StartCoroutine(Activate_ScreenOverlay(m_SavedData.m_soLayerFlag, 0f, m_SavedScreenOverlayTextureName, m_SavedData.m_soMaxIntensity));
					break;
				case 5:
					yield return MainLoadThing.instance.StartCoroutine(Activate_CameraFlare_byWorld(m_SavedData.m_cfLayerFlag, 0f, m_SavedData.m_cfWorldPosX, m_SavedData.m_cfWorldPosY, m_SavedData.m_cfScaleX, m_SavedData.m_cfScaleY, m_SavedData.m_cfMaxAmount, m_SavedCameraFlarePrefabName));
					break;
				case 6:
					Activate_GrayScale(m_SavedData.m_grayScaleLayerFlag, 0f, m_SavedData.m_grayScaleMaxAmount);
					break;
				case 7:
					Activate_Negative(m_SavedData.m_negativeLayerFlag, 0f, m_SavedData.m_negativeMaxAmount);
					break;
				case 8:
					Activate_ColorCurve(m_SavedData.m_cccLayerFlag, 0f, m_SavedData.m_cccRedSteepness, m_SavedData.m_cccRedGamma, m_SavedData.m_cccGreenSteepness, m_SavedData.m_cccGreenGamma, m_SavedData.m_cccBlueSteepness, m_SavedData.m_cccBlueGamma);
					break;
				case 9:
					Activate_GaussianBlur(m_SavedData.m_gbLayerFlag, 0f, m_SavedData.m_gbPasses, m_SavedData.m_gbDownscaling);
					break;
				case 10:
					Activate_RadialBlur(m_SavedData.m_rbLayerFlag, 0f, m_SavedData.m_rbMaxStrength, m_SavedData.m_rbCenterPosX, m_SavedData.m_rbCenterPosY, m_SavedData.m_rbEnableVignette, m_SavedData.m_rbSharpness, m_SavedData.m_rbDarkness);
					break;
				case 11:
					Activate_Covered(m_SavedData.m_coverLayerFlag, m_SavedData.m_coverMaxAmount, m_SavedData.m_coverR, m_SavedData.m_coverG, m_SavedData.m_coverB);
					break;
				case 12:
					Activate_FlashIn(m_SavedData.m_flashLayerFlag, 0f, m_SavedData.m_flashMaxAmount, m_SavedData.m_flashR, m_SavedData.m_flashG, m_SavedData.m_flashB);
					break;
				case 13:
					yield return MainLoadThing.instance.StartCoroutine(Activate_BackgroundStream(m_SavedData.m_bgStreamLayerFlag, m_SavedData.m_bgStreamType, m_SavedData.m_bgStreamSpeedRate, m_SavedData.m_bgStreamOrderInLayer));
					break;
				case 14:
					yield return MainLoadThing.instance.StartCoroutine(Activate_FocusLine(m_SavedData.m_focusLineLayerFlag, m_SavedData.m_focusLineOrderInLayer));
					break;
				case 15:
					yield return MainLoadThing.instance.StartCoroutine(Activate_FocusLineHori(m_SavedData.m_focusLineHoriLayerFlag, m_SavedData.m_focusLineHoriOrderInLayer));
					break;
				case 16:
					yield return MainLoadThing.instance.StartCoroutine(Activate_FallingStone(m_SavedData.m_fsLayerFlag, m_SavedData.m_fsbType, m_SavedData.m_fsSpeedRate, m_SavedData.m_fsOrderInLayer));
					break;
				case 17:
					yield return MainLoadThing.instance.StartCoroutine(Activate_FallingStoneBlur(m_SavedData.m_fsbLayerFlag, m_SavedData.m_fsbType, m_SavedData.m_fsbSpeedRate, m_SavedData.m_fsbOrderInLayer));
					break;
				case 18:
					yield return MainLoadThing.instance.StartCoroutine(Activate_KeywordMenuBG(m_SavedData.m_kmbgNoiseLevel));
					break;
				case 19:
					yield return MainLoadThing.instance.StartCoroutine(Activate_SNSMenuBG());
					break;
				}
			}
		}
	}

	public int GetSaveDataSize(bool isOnlyLoadFile = false)
	{
		int num = 0;
		num += 4;
		num += 4;
		num += SaveData.GetSize();
		if (isOnlyLoadFile)
		{
			num += m_SavedData.m_soTextureNameLength;
			num += m_SavedData.m_cfPrefabNameLength;
		}
		else
		{
			EffectInfo effectInfo = m_EffectInfos[4];
			EffectState effectState = effectInfo?.m_State ?? EffectState.Disable;
			if (effectState == EffectState.Enable || effectState == EffectState.Activating)
			{
				byte[] bytes = Encoding.Unicode.GetBytes(effectInfo.m_Params[4] as string);
				num += bytes.Length;
			}
			effectInfo = m_EffectInfos[5];
			effectState = effectInfo?.m_State ?? EffectState.Disable;
			if (effectState == EffectState.Enable || effectState == EffectState.Activating)
			{
				byte[] bytes2 = Encoding.Unicode.GetBytes(effectInfo.m_Params[4] as string);
				num += bytes2.Length;
			}
		}
		return num;
	}

	public bool Activate_DoubleVision(int layerFlag, float fTime, float fMaxAmount, float fDisplaceX, float fDisplaceY)
	{
		if (IsActivatedEffect(Effects.DoubleVision))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		DoubleVision doubleVision = gameObject.GetComponent<DoubleVision>();
		if (doubleVision == null)
		{
			doubleVision = gameObject.AddComponent<DoubleVision>();
			doubleVision.Shader = Shader.Find("Hidden/Colorful/Double Vision");
		}
		doubleVision.Displace = new Vector2(fDisplaceX, fDisplaceY);
		doubleVision.Amount = ((!(fTime <= 0f)) ? 0f : num);
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.DoubleVision, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, doubleVision, 4);
		effectInfo.m_fpUpdate = UpdateProc_DoubleVision;
		effectInfo.m_fpChangeCam = ChangeCamera_DoubleVision;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = doubleVision.Amount;
		return true;
	}

	public bool Deactivate_DoubleVision(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[0];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_DoubleVision()
	{
		EffectInfo effectInfo = m_EffectInfos[0];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as DoubleVision).Amount = num5;
	}

	private void ChangeCamera_DoubleVision(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[0];
		DoubleVision doubleVision = effectInfo.m_Compnent as DoubleVision;
		GameObject gameObject = newCamera.gameObject;
		DoubleVision doubleVision2 = gameObject.GetComponent<DoubleVision>();
		if (doubleVision2 == null)
		{
			doubleVision2 = gameObject.AddComponent<DoubleVision>();
			doubleVision2.Shader = Shader.Find("Hidden/Colorful/Double Vision");
		}
		doubleVision2.Displace = doubleVision.Displace;
		doubleVision2.Amount = doubleVision.Amount;
		UnityEngine.Object.Destroy(doubleVision);
		effectInfo.m_Compnent = doubleVision2;
	}

	private void SaveDoubleVisionState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_dvLayerFlag = 0;
			saveData.m_dvMaxAmount = 0f;
			saveData.m_dvDisplaceX = 0f;
			saveData.m_dvDisplaceY = 0f;
		}
		else
		{
			DoubleVision doubleVision = effectInfo.m_Compnent as DoubleVision;
			saveData.m_dvLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_dvMaxAmount = (float)effectInfo.m_Params[1];
			saveData.m_dvDisplaceX = doubleVision.Displace.x;
			saveData.m_dvDisplaceY = doubleVision.Displace.y;
		}
	}

	public bool Activate_Glitch(int layerFlag, float fSpeed = 10f, float fDensity = 8f, float fMaxDisplacement = 2f)
	{
		if (IsActivatedEffect(Effects.Glitch))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		Glitch glitch = gameObject.GetComponent<Glitch>();
		if (glitch == null)
		{
			glitch = gameObject.AddComponent<Glitch>();
			glitch.Shader = Shader.Find("Hidden/Colorful/Glitch");
		}
		glitch.Mode = Glitch.GlitchingMode.Interferences;
		glitch.SettingsInterferences = new Glitch.InterferenceSettings();
		glitch.SettingsInterferences.Speed = fSpeed;
		glitch.SettingsInterferences.Density = fDensity;
		glitch.SettingsInterferences.MaxDisplacement = fMaxDisplacement;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Glitch, EffectState.Enable, glitch, 0);
		effectInfo.m_fpChangeCam = ChangeCamera_Glitch;
		return true;
	}

	public bool Deactivate_Glitch()
	{
		EffectInfo effectInfo = m_EffectInfos[1];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void ChangeCamera_Glitch(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[1];
		Glitch glitch = effectInfo.m_Compnent as Glitch;
		GameObject gameObject = newCamera.gameObject;
		Glitch glitch2 = gameObject.GetComponent<Glitch>();
		if (glitch2 == null)
		{
			glitch2 = gameObject.AddComponent<Glitch>();
			glitch2.Shader = Shader.Find("Hidden/Colorful/Glitch");
		}
		glitch2.Mode = glitch.Mode;
		if (glitch.SettingsInterferences != null)
		{
			if (glitch2.SettingsInterferences == null)
			{
				glitch2.SettingsInterferences = new Glitch.InterferenceSettings();
			}
			glitch2.SettingsInterferences.Speed = glitch.SettingsInterferences.Speed;
			glitch2.SettingsInterferences.Density = glitch.SettingsInterferences.Density;
			glitch2.SettingsInterferences.MaxDisplacement = glitch.SettingsInterferences.MaxDisplacement;
		}
		else
		{
			glitch2.SettingsInterferences = null;
		}
		if (glitch.SettingsTearing != null)
		{
			if (glitch2.SettingsTearing == null)
			{
				glitch2.SettingsTearing = new Glitch.TearingSettings();
			}
			glitch2.SettingsTearing.AllowFlipping = glitch.SettingsTearing.AllowFlipping;
			glitch2.SettingsTearing.Intensity = glitch.SettingsTearing.Intensity;
			glitch2.SettingsTearing.MaxDisplacement = glitch.SettingsTearing.MaxDisplacement;
			glitch2.SettingsTearing.Speed = glitch.SettingsTearing.Speed;
			glitch2.SettingsTearing.YuvColorBleeding = glitch.SettingsTearing.YuvColorBleeding;
			glitch2.SettingsTearing.YuvOffset = glitch.SettingsTearing.YuvOffset;
		}
		else
		{
			glitch2.SettingsTearing = null;
		}
		UnityEngine.Object.Destroy(glitch);
		effectInfo.m_Compnent = glitch2;
	}

	private void SaveGlitchState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_glitchLayerFlag = 0;
			saveData.m_glitchSpeed = 0f;
			saveData.m_glitchDensity = 0f;
			saveData.m_glitchMaxDisplacement = 0f;
		}
		else
		{
			Glitch glitch = effectInfo.m_Compnent as Glitch;
			saveData.m_glitchLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_glitchSpeed = glitch.SettingsInterferences.Speed;
			saveData.m_glitchDensity = glitch.SettingsInterferences.Density;
			saveData.m_glitchMaxDisplacement = glitch.SettingsInterferences.MaxDisplacement;
		}
	}

	public bool Activate_Wiggle(int layerFlag, float fTime, float fSpeed, float fFreq, float fAmplitude)
	{
		if (IsActivatedEffect(Effects.Wiggle))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		if (fSpeed <= 0f || fFreq <= 0f || fAmplitude <= 0f)
		{
			return false;
		}
		fAmplitude /= 100f;
		GameObject gameObject = camera_byLayerFlag.gameObject;
		Wiggle wiggle = gameObject.GetComponent<Wiggle>();
		if (wiggle == null)
		{
			wiggle = gameObject.AddComponent<Wiggle>();
			wiggle.Shader = Shader.Find("Hidden/Colorful/Wiggle");
		}
		wiggle.Speed = fSpeed;
		wiggle.Frequency = fFreq;
		wiggle.Amplitude = ((!(fTime <= 0f)) ? 0f : fAmplitude);
		wiggle.AutomaticTimer = false;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Wiggle, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, wiggle, 5);
		effectInfo.m_fpUpdate = UpdateProc_Wiggle;
		effectInfo.m_fpChangeCam = ChangeCamera_Wiggle;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = fAmplitude;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = ((!(fTime <= 0f)) ? 0f : fAmplitude);
		effectInfo.m_Params[4] = 1000f;
		return true;
	}

	public bool Deactivate_Wiggle(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[2];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_Wiggle()
	{
		EffectInfo effectInfo = m_EffectInfos[2];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable)
		{
			return;
		}
		Wiggle wiggle = effectInfo.m_Compnent as Wiggle;
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		float num6 = (float)effectInfo.m_Params[4];
		switch (effectInfo.m_State)
		{
		case EffectState.Activating:
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
			effectInfo.m_Params[2] = num4;
			effectInfo.m_Params[3] = num5;
			wiggle.Amplitude = num5;
			break;
		case EffectState.Deactivating:
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
			effectInfo.m_Params[2] = num4;
			effectInfo.m_Params[3] = num5;
			wiggle.Amplitude = num5;
			break;
		}
		if (wiggle.Timer > num6)
		{
			wiggle.Timer -= num6;
		}
		wiggle.Timer += wiggle.Speed * num;
	}

	private void ChangeCamera_Wiggle(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[2];
		Wiggle wiggle = effectInfo.m_Compnent as Wiggle;
		GameObject gameObject = newCamera.gameObject;
		Wiggle wiggle2 = gameObject.GetComponent<Wiggle>();
		if (wiggle2 == null)
		{
			wiggle2 = gameObject.AddComponent<Wiggle>();
			wiggle2.Shader = Shader.Find("Hidden/Colorful/Wiggle");
		}
		wiggle2.Amplitude = wiggle.Amplitude;
		wiggle2.AutomaticTimer = wiggle.AutomaticTimer;
		wiggle2.Frequency = wiggle.Frequency;
		wiggle2.Mode = wiggle.Mode;
		wiggle2.Speed = wiggle.Speed;
		wiggle2.Timer = wiggle.Timer;
		UnityEngine.Object.Destroy(wiggle);
		effectInfo.m_Compnent = wiggle2;
	}

	private void SaveWiggleState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_wiggleLayerFlag = 0;
			saveData.m_wiggleSpeed = 0f;
			saveData.m_wiggleFreq = 0f;
			saveData.m_wiggleAmplitude = 0f;
		}
		else
		{
			Wiggle wiggle = effectInfo.m_Compnent as Wiggle;
			saveData.m_wiggleLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_wiggleSpeed = wiggle.Speed;
			saveData.m_wiggleFreq = wiggle.Frequency;
			saveData.m_wiggleAmplitude = (float)effectInfo.m_Params[1];
		}
	}

	public bool Activate_AnalogTV(int layerFlag, float fNoiseIntensity = 0.5f)
	{
		if (IsActivatedEffect(Effects.AnalogTV))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float noiseIntensity = Mathf.Clamp01(fNoiseIntensity);
		GameObject gameObject = camera_byLayerFlag.gameObject;
		AnalogTV analogTV = gameObject.GetComponent<AnalogTV>();
		if (analogTV == null)
		{
			analogTV = gameObject.AddComponent<AnalogTV>();
			analogTV.Shader = Shader.Find("Hidden/Colorful/Analog TV");
		}
		analogTV.NoiseIntensity = noiseIntensity;
		analogTV.AutomaticPhase = false;
		analogTV.Distortion = 0.03f;
		analogTV.CubicDistortion = 0f;
		analogTV.Scale = 1f;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.AnalogTV, EffectState.Enable, analogTV, 0);
		effectInfo.m_fpUpdate = UpdateProc_AnalogTV;
		effectInfo.m_fpChangeCam = ChangeCamera_AnalogTV;
		return true;
	}

	public bool Deactivate_AnalogTV()
	{
		EffectInfo effectInfo = m_EffectInfos[3];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void UpdateProc_AnalogTV()
	{
		EffectInfo effectInfo = m_EffectInfos[3];
		if (effectInfo != null && !(effectInfo.m_Compnent == null) && effectInfo.m_State != EffectState.Disable)
		{
			AnalogTV analogTV = effectInfo.m_Compnent as AnalogTV;
			if (analogTV.Phase > 1000f)
			{
				analogTV.Phase = 10f;
			}
			analogTV.Phase += Time.deltaTime * 0.25f;
		}
	}

	private void ChangeCamera_AnalogTV(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[3];
		AnalogTV analogTV = effectInfo.m_Compnent as AnalogTV;
		GameObject gameObject = newCamera.gameObject;
		AnalogTV analogTV2 = gameObject.GetComponent<AnalogTV>();
		if (analogTV2 == null)
		{
			analogTV2 = gameObject.AddComponent<AnalogTV>();
			analogTV2.Shader = Shader.Find("Hidden/Colorful/Analog TV");
		}
		analogTV2.AutomaticPhase = analogTV.AutomaticPhase;
		analogTV2.ConvertToGrayscale = analogTV.ConvertToGrayscale;
		analogTV2.CubicDistortion = analogTV.CubicDistortion;
		analogTV2.Distortion = analogTV.Distortion;
		analogTV2.NoiseIntensity = analogTV.NoiseIntensity;
		analogTV2.Phase = analogTV.Phase;
		analogTV2.Scale = analogTV.Scale;
		analogTV2.ScanlinesCount = analogTV.ScanlinesCount;
		analogTV2.ScanlinesIntensity = analogTV.ScanlinesIntensity;
		analogTV2.ScanlinesOffset = analogTV.ScanlinesOffset;
		UnityEngine.Object.Destroy(analogTV);
		effectInfo.m_Compnent = analogTV2;
	}

	private void SaveAnalogTVState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_analogTvLayerFlag = 0;
			saveData.m_analogTvNoiseIntensity = 0f;
		}
		else
		{
			AnalogTV analogTV = effectInfo.m_Compnent as AnalogTV;
			saveData.m_analogTvLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_analogTvNoiseIntensity = analogTV.NoiseIntensity;
		}
	}

	public IEnumerator Activate_ScreenOverlay(int layerFlag, float fTime, string strTextureName, float fMaxIntensity = 1f)
	{
		if (IsActivatedEffect(Effects.ScreenOverlay) || fMaxIntensity == 0f)
		{
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		EffectInfo effInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.ScreenOverlay, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, null, 5);
		Texture2D overlayTexture = null;
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(strTextureName));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler asBundleObjectHdr = GetAssetBundleObjectHandler(strTextureName);
		if (asBundleObjectHdr != null)
		{
			effInfo.m_assetPath = strTextureName;
			overlayTexture = asBundleObjectHdr.GetLoadedAsset<Texture2D>();
			if (!(overlayTexture == null))
			{
				GameObject objCamera = camera.gameObject;
				ScreenOverlay component = objCamera.GetComponent<ScreenOverlay>();
				if (component == null)
				{
					component = objCamera.AddComponent<ScreenOverlay>();
					component.overlayShader = Shader.Find("Hidden/BlendModesOverlay");
				}
				component.blendMode = ScreenOverlay.OverlayBlendMode.Overlay;
				component.texture = overlayTexture;
				component.intensity = ((!(fTime <= 0f)) ? 0f : fMaxIntensity);
				effInfo.m_Compnent = component;
				effInfo.m_fpUpdate = UpdateProc_ScreenOverlay;
				effInfo.m_fpChangeCam = ChangeCamera_ScreenOverlay;
				effInfo.m_Params[0] = fTime;
				effInfo.m_Params[1] = fMaxIntensity;
				effInfo.m_Params[2] = 0f;
				effInfo.m_Params[3] = component.intensity;
				effInfo.m_Params[4] = strTextureName;
				yield break;
			}
		}
		ReleaseEffectInfo(effInfo);
	}

	public bool Deactivate_ScreenOverlay(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[4];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			UnloadPrefabSrcObject(effectInfo.m_assetPath);
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_ScreenOverlay()
	{
		EffectInfo effectInfo = m_EffectInfos[4];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				UnloadPrefabSrcObject(effectInfo.m_assetPath);
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as ScreenOverlay).intensity = num5;
	}

	private void ChangeCamera_ScreenOverlay(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[4];
		ScreenOverlay screenOverlay = effectInfo.m_Compnent as ScreenOverlay;
		GameObject gameObject = newCamera.gameObject;
		ScreenOverlay screenOverlay2 = gameObject.GetComponent<ScreenOverlay>();
		if (screenOverlay2 == null)
		{
			screenOverlay2 = gameObject.AddComponent<ScreenOverlay>();
			screenOverlay2.overlayShader = Shader.Find("Hidden/BlendModesOverlay");
		}
		screenOverlay2.blendMode = screenOverlay.blendMode;
		screenOverlay2.texture = screenOverlay.texture;
		screenOverlay2.intensity = screenOverlay.intensity;
		UnityEngine.Object.Destroy(screenOverlay);
		effectInfo.m_Compnent = screenOverlay2;
	}

	private void SaveScreenOverlayState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_soLayerFlag = 0;
			saveData.m_soTextureNameLength = 0;
			saveData.m_soMaxIntensity = 0f;
			return;
		}
		saveData.m_soLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
		saveData.m_soMaxIntensity = (float)effectInfo.m_Params[1];
		saveData.m_soTextureNameLength = 0;
		m_SavedScreenOverlayTextureName = effectInfo.m_Params[4] as string;
		if (!string.IsNullOrEmpty(m_SavedScreenOverlayTextureName))
		{
			byte[] bytes = Encoding.Unicode.GetBytes(m_SavedScreenOverlayTextureName);
			saveData.m_soTextureNameLength = bytes.Length;
		}
	}

	public IEnumerator Activate_CameraFlare(int layerFlag, float fTime, float fNormalizedPosX, float fNormalizedPosY, float fScaleX = 1f, float fScaleY = 1f, float fMaxAmount = 0.5f, string prefabName = "Prefabs/CameraEffect/CameraFlareType1")
	{
		if (IsActivatedEffect(Effects.CameraFlare))
		{
			ReleaseEffectInfo(m_EffectInfos[5], isDestroyGameObject: true);
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		float fNormalizeMaxAmount = Mathf.Clamp01(fMaxAmount);
		if (fNormalizeMaxAmount <= 0f)
		{
			yield break;
		}
		EffectInfo effInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.CameraFlare, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, null, 5);
		GameObject prefabObj = null;
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(prefabName));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler asBundleObjectHdr = GetAssetBundleObjectHandler(prefabName);
		if (asBundleObjectHdr != null)
		{
			effInfo.m_assetPath = prefabName;
			prefabObj = asBundleObjectHdr.GetLoadedAsset_ToGameObject();
			if (!(prefabObj == null))
			{
				GameObject effObject = UnityEngine.Object.Instantiate(prefabObj);
				if (!(effObject == null))
				{
					effInfo.m_GameObj = effObject;
					effInfo.m_GameObj.layer = effInfo.m_LayerID;
					float fScreenPosX = (float)Screen.width * fNormalizedPosX;
					float fScreenPosY = (float)Screen.height * fNormalizedPosY;
					Transform tr = effInfo.m_GameObj.transform;
					tr.position = camera.ScreenToWorldPoint(new Vector3(fScreenPosX, fScreenPosY, 1f));
					tr.localScale = new Vector3(fScaleX, fScaleY, 1f);
					tr.SetParent(camera.transform, worldPositionStays: true);
					tr.localRotation = Quaternion.identity;
					effInfo.m_GameObj.SetActive(value: true);
					MeshRenderer renderer = effInfo.m_GameObj.GetComponent<MeshRenderer>();
					Color color = renderer.material.GetColor("_TintColor");
					color.a = ((!(fTime <= 0f)) ? 0f : fMaxAmount);
					renderer.material.SetColor("_TintColor", color);
					effInfo.m_fpUpdate = UpdateProc_CameraFlare;
					effInfo.m_fpChangeCam = ChangeCamera_CameraFlare;
					effInfo.m_Params[0] = fTime;
					effInfo.m_Params[1] = fNormalizeMaxAmount;
					effInfo.m_Params[2] = 0f;
					effInfo.m_Params[3] = 0f;
					effInfo.m_Params[4] = prefabName;
					yield break;
				}
			}
		}
		ReleaseEffectInfo(effInfo);
	}

	public IEnumerator Activate_CameraFlare_byScreen(int layerFlag, float fTime, float fScreenPosX, float fScreenPosY, float fScaleX = 1f, float fScaleY = 1f, float fMaxAmount = 0.5f, string prefabName = "Prefabs/CameraEffect/CameraFlareType1")
	{
		float fNormalizedX = fScreenPosX / 1920f;
		float fNormalizedY = fScreenPosY / 1080f;
		yield return MainLoadThing.instance.StartCoroutine(Activate_CameraFlare(layerFlag, fTime, fNormalizedX, fNormalizedY, fScaleX, fScaleY, fMaxAmount, prefabName));
	}

	public IEnumerator Activate_CameraFlare_byWorld(int layerFlag, float fTime, float fWorldPosX, float fWorldPosY, float fScaleX = 1f, float fScaleY = 1f, float fMaxAmount = 0.5f, string prefabName = "Prefabs/CameraEffect/CameraFlareType1")
	{
		if (IsActivatedEffect(Effects.CameraFlare))
		{
			ReleaseEffectInfo(m_EffectInfos[5], isDestroyGameObject: true);
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		float fNormalizeMaxAmount = Mathf.Clamp01(fMaxAmount);
		if (fNormalizeMaxAmount <= 0f)
		{
			yield break;
		}
		EffectInfo effInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.CameraFlare, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, null, 5);
		GameObject prefabObj = null;
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(prefabName));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler asBundleObjectHdr = GetAssetBundleObjectHandler(prefabName);
		if (asBundleObjectHdr != null)
		{
			effInfo.m_assetPath = prefabName;
			prefabObj = asBundleObjectHdr.GetLoadedAsset_ToGameObject();
			if (!(prefabObj == null))
			{
				GameObject effObject = UnityEngine.Object.Instantiate(prefabObj);
				if (!(effObject == null))
				{
					effInfo.m_GameObj = effObject;
					effInfo.m_GameObj.layer = effInfo.m_LayerID;
					Transform tr = effInfo.m_GameObj.transform;
					tr.SetParent(camera.transform, worldPositionStays: true);
					tr.localPosition = new Vector3(fWorldPosX, fWorldPosY, 1f);
					tr.localScale = new Vector3(fScaleX, fScaleY, 1f);
					tr.localRotation = Quaternion.identity;
					effInfo.m_GameObj.SetActive(value: true);
					MeshRenderer renderer = effInfo.m_GameObj.GetComponent<MeshRenderer>();
					Color color = renderer.material.GetColor("_TintColor");
					color.a = ((!(fTime <= 0f)) ? 0f : fMaxAmount);
					renderer.material.SetColor("_TintColor", color);
					effInfo.m_fpUpdate = UpdateProc_CameraFlare;
					effInfo.m_fpChangeCam = ChangeCamera_CameraFlare;
					effInfo.m_Params[0] = fTime;
					effInfo.m_Params[1] = fNormalizeMaxAmount;
					effInfo.m_Params[2] = 0f;
					effInfo.m_Params[3] = 0f;
					effInfo.m_Params[4] = prefabName;
					yield break;
				}
			}
		}
		ReleaseEffectInfo(effInfo);
	}

	public bool Deactivate_CameraFlare(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[5];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			UnloadPrefabSrcObject(effectInfo.m_assetPath);
			ReleaseEffectInfo(effectInfo, isDestroyGameObject: true);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_CameraFlare()
	{
		EffectInfo effectInfo = m_EffectInfos[5];
		if (effectInfo == null || effectInfo.m_GameObj == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				UnloadPrefabSrcObject(effectInfo.m_assetPath);
				ReleaseEffectInfo(effectInfo, isDestroyGameObject: true);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		MeshRenderer component = effectInfo.m_GameObj.GetComponent<MeshRenderer>();
		Color color = component.material.GetColor("_TintColor");
		color.a = num5;
		component.material.SetColor("_TintColor", color);
	}

	private void ChangeCamera_CameraFlare(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[5];
		if (effectInfo != null && !(effectInfo.m_GameObj == null))
		{
			Transform transform = effectInfo.m_GameObj.transform;
			transform.SetParent(newCamera.transform, worldPositionStays: false);
			transform.localRotation = Quaternion.identity;
		}
	}

	private void SaveCameraFlareState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null)
		{
			saveData.m_cfLayerFlag = 0;
			saveData.m_cfWorldPosX = 0f;
			saveData.m_cfWorldPosY = 0f;
			saveData.m_cfScaleX = 0f;
			saveData.m_cfScaleY = 0f;
			saveData.m_cfMaxAmount = 0f;
			saveData.m_cfPrefabNameLength = 0;
			return;
		}
		Transform component = effectInfo.m_GameObj.GetComponent<Transform>();
		saveData.m_cfLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
		saveData.m_cfWorldPosX = component.localPosition.x;
		saveData.m_cfWorldPosY = component.localPosition.y;
		saveData.m_cfScaleX = component.localScale.x;
		saveData.m_cfScaleY = component.localScale.y;
		saveData.m_cfMaxAmount = (float)effectInfo.m_Params[1];
		saveData.m_cfPrefabNameLength = 0;
		m_SavedCameraFlarePrefabName = effectInfo.m_Params[4] as string;
		if (!string.IsNullOrEmpty(m_SavedCameraFlarePrefabName))
		{
			byte[] bytes = Encoding.Unicode.GetBytes(m_SavedCameraFlarePrefabName);
			saveData.m_cfPrefabNameLength = bytes.Length;
		}
	}

	public bool Activate_GrayScale(int layerFlag, float fTime = 0f, float fMaxAmount = 1f)
	{
		if (IsActivatedEffect(Effects.GrayScale))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		Colorful.Grayscale grayscale = gameObject.GetComponent<Colorful.Grayscale>();
		if (grayscale == null)
		{
			grayscale = gameObject.AddComponent<Colorful.Grayscale>();
			grayscale.Shader = Shader.Find("Hidden/Colorful/Grayscale");
		}
		grayscale.Amount = ((!(fTime <= 0f)) ? 0f : fMaxAmount);
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.GrayScale, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, grayscale, 4);
		effectInfo.m_fpUpdate = Update_GrayScale;
		effectInfo.m_fpChangeCam = ChangeCamera_GrayScale;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = grayscale.Amount;
		return true;
	}

	public bool Deactivate_GrayScale(float fTime = 0f)
	{
		EffectInfo effectInfo = m_EffectInfos[6];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void Update_GrayScale()
	{
		EffectInfo effectInfo = m_EffectInfos[6];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as Colorful.Grayscale).Amount = num5;
	}

	private void ChangeCamera_GrayScale(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[6];
		Colorful.Grayscale grayscale = effectInfo.m_Compnent as Colorful.Grayscale;
		GameObject gameObject = newCamera.gameObject;
		Colorful.Grayscale grayscale2 = gameObject.GetComponent<Colorful.Grayscale>();
		if (grayscale2 == null)
		{
			grayscale2 = gameObject.AddComponent<Colorful.Grayscale>();
			grayscale2.Shader = Shader.Find("Hidden/Colorful/Grayscale");
		}
		grayscale2.Amount = grayscale.Amount;
		grayscale2.BlueLuminance = grayscale.BlueLuminance;
		grayscale2.GreenLuminance = grayscale.GreenLuminance;
		grayscale2.RedLuminance = grayscale.RedLuminance;
		UnityEngine.Object.Destroy(grayscale);
		effectInfo.m_Compnent = grayscale2;
	}

	private void SaveGrayScaleState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_grayScaleLayerFlag = 0;
			saveData.m_grayScaleMaxAmount = 0f;
		}
		else
		{
			saveData.m_grayScaleLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_grayScaleMaxAmount = (float)effectInfo.m_Params[1];
		}
	}

	public bool Activate_Negative(int layerFlag, float fTime = 0f, float fMaxAmount = 1f)
	{
		if (IsActivatedEffect(Effects.Negative))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		Negative negative = gameObject.GetComponent<Negative>();
		if (negative == null)
		{
			negative = gameObject.AddComponent<Negative>();
			negative.Shader = Shader.Find("Hidden/Colorful/Negative");
		}
		negative.Amount = ((!(fTime <= 0f)) ? 0f : fMaxAmount);
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Negative, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, negative, 4);
		effectInfo.m_fpUpdate = Update_Negative;
		effectInfo.m_fpChangeCam = ChangeCamera_Negative;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = negative.Amount;
		return true;
	}

	public bool Deactivate_Negative(float fTime = 0f)
	{
		EffectInfo effectInfo = m_EffectInfos[7];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void Update_Negative()
	{
		EffectInfo effectInfo = m_EffectInfos[7];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as Negative).Amount = num5;
	}

	private void ChangeCamera_Negative(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[7];
		Negative negative = effectInfo.m_Compnent as Negative;
		GameObject gameObject = newCamera.gameObject;
		Negative negative2 = gameObject.GetComponent<Negative>();
		if (negative2 == null)
		{
			negative2 = gameObject.AddComponent<Negative>();
			negative2.Shader = Shader.Find("Hidden/Colorful/Negative");
		}
		negative2.Amount = negative.Amount;
		UnityEngine.Object.Destroy(negative);
		effectInfo.m_Compnent = negative2;
	}

	private void SaveNegativeState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_negativeLayerFlag = 0;
			saveData.m_negativeMaxAmount = 0f;
		}
		else
		{
			saveData.m_negativeLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_negativeMaxAmount = (float)effectInfo.m_Params[1];
		}
	}

	public bool Activate_ColorCurve(int layerFlag, float fTime, float fRedSteepness = 1f, float fRedGamma = 1f, float fGreenSteepness = 1f, float fGreenGamma = 1f, float fBlueSteepness = 1f, float fBlueGamma = 1f)
	{
		if (IsActivatedEffect(Effects.ColorCurve))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		SCurveContrast sCurveContrast = gameObject.GetComponent<SCurveContrast>();
		if (sCurveContrast == null)
		{
			sCurveContrast = gameObject.AddComponent<SCurveContrast>();
			sCurveContrast.Shader = Shader.Find("Hidden/Colorful/SCurveContrast");
		}
		EffectState state;
		if (fTime <= 0f)
		{
			sCurveContrast.RedSteepness = fRedSteepness;
			sCurveContrast.RedGamma = fRedGamma;
			sCurveContrast.GreenSteepness = fGreenSteepness;
			sCurveContrast.GreenGamma = fGreenGamma;
			sCurveContrast.BlueSteepness = fBlueSteepness;
			sCurveContrast.BlueGamma = fBlueGamma;
			state = EffectState.Enable;
		}
		else
		{
			sCurveContrast.RedSteepness = 1f;
			sCurveContrast.RedGamma = 1f;
			sCurveContrast.GreenSteepness = 1f;
			sCurveContrast.GreenGamma = 1f;
			sCurveContrast.BlueSteepness = 1f;
			sCurveContrast.BlueGamma = 1f;
			state = EffectState.Activating;
		}
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.ColorCurve, state, sCurveContrast, 14);
		effectInfo.m_fpUpdate = Update_ColorCurve;
		effectInfo.m_fpChangeCam = ChangeCamera_ColorCurve;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = fRedSteepness;
		effectInfo.m_Params[2] = fRedGamma;
		effectInfo.m_Params[3] = fGreenSteepness;
		effectInfo.m_Params[4] = fGreenGamma;
		effectInfo.m_Params[5] = fBlueSteepness;
		effectInfo.m_Params[6] = fBlueGamma;
		effectInfo.m_Params[7] = 0f;
		effectInfo.m_Params[8] = sCurveContrast.RedSteepness;
		effectInfo.m_Params[9] = sCurveContrast.RedGamma;
		effectInfo.m_Params[10] = sCurveContrast.GreenSteepness;
		effectInfo.m_Params[11] = sCurveContrast.GreenGamma;
		effectInfo.m_Params[12] = sCurveContrast.BlueSteepness;
		effectInfo.m_Params[13] = sCurveContrast.BlueGamma;
		return true;
	}

	public bool Deactivate_ColorCurve(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[8];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[8];
		effectInfo.m_Params[2] = effectInfo.m_Params[9];
		effectInfo.m_Params[3] = effectInfo.m_Params[10];
		effectInfo.m_Params[4] = effectInfo.m_Params[11];
		effectInfo.m_Params[5] = effectInfo.m_Params[12];
		effectInfo.m_Params[6] = effectInfo.m_Params[13];
		effectInfo.m_Params[7] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void Update_ColorCurve()
	{
		EffectInfo effectInfo = m_EffectInfos[8];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		float num6 = (float)effectInfo.m_Params[4];
		float num7 = (float)effectInfo.m_Params[5];
		float num8 = (float)effectInfo.m_Params[6];
		float num9 = (float)effectInfo.m_Params[7];
		float num10 = (float)effectInfo.m_Params[8];
		float num11 = (float)effectInfo.m_Params[9];
		float num12 = (float)effectInfo.m_Params[10];
		float num13 = (float)effectInfo.m_Params[11];
		float num14 = (float)effectInfo.m_Params[12];
		float num15 = (float)effectInfo.m_Params[13];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num9 += num;
			if (num9 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num9 = num2;
				num10 = num3;
				num11 = num4;
				num12 = num5;
				num13 = num6;
				num14 = num7;
				num15 = num8;
			}
			else
			{
				if (!num3.Equals(num10))
				{
					num10 = (num3 - 1f) / num2 * num9 + 1f;
				}
				if (!num4.Equals(num11))
				{
					num11 = (num4 - 1f) / num2 * num9 + 1f;
				}
				if (!num5.Equals(num12))
				{
					num12 = (num5 - 1f) / num2 * num9 + 1f;
				}
				if (!num6.Equals(num13))
				{
					num13 = (num6 - 1f) / num2 * num9 + 1f;
				}
				if (!num7.Equals(num14))
				{
					num14 = (num7 - 1f) / num2 * num9 + 1f;
				}
				if (!num8.Equals(num15))
				{
					num15 = (num8 - 1f) / num2 * num9 + 1f;
				}
			}
		}
		else
		{
			num9 -= num;
			if (num9 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			if (!num3.Equals(1f))
			{
				num10 = (num3 - 1f) / num2 * num9 + 1f;
			}
			if (!num4.Equals(1f))
			{
				num11 = (num4 - 1f) / num2 * num9 + 1f;
			}
			if (!num5.Equals(1f))
			{
				num12 = (num5 - 1f) / num2 * num9 + 1f;
			}
			if (!num6.Equals(1f))
			{
				num13 = (num6 - 1f) / num2 * num9 + 1f;
			}
			if (!num7.Equals(1f))
			{
				num14 = (num7 - 1f) / num2 * num9 + 1f;
			}
			if (!num8.Equals(1f))
			{
				num15 = (num8 - 1f) / num2 * num9 + 1f;
			}
		}
		effectInfo.m_Params[7] = num9;
		effectInfo.m_Params[8] = num10;
		effectInfo.m_Params[9] = num11;
		effectInfo.m_Params[10] = num12;
		effectInfo.m_Params[11] = num13;
		effectInfo.m_Params[12] = num14;
		effectInfo.m_Params[13] = num15;
		SCurveContrast sCurveContrast = effectInfo.m_Compnent as SCurveContrast;
		sCurveContrast.RedSteepness = num10;
		sCurveContrast.RedGamma = num11;
		sCurveContrast.GreenSteepness = num12;
		sCurveContrast.GreenGamma = num13;
		sCurveContrast.BlueSteepness = num14;
		sCurveContrast.BlueGamma = num15;
	}

	private void ChangeCamera_ColorCurve(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[8];
		SCurveContrast sCurveContrast = effectInfo.m_Compnent as SCurveContrast;
		GameObject gameObject = newCamera.gameObject;
		SCurveContrast sCurveContrast2 = gameObject.GetComponent<SCurveContrast>();
		if (sCurveContrast2 == null)
		{
			sCurveContrast2 = gameObject.AddComponent<SCurveContrast>();
			sCurveContrast2.Shader = Shader.Find("Hidden/Colorful/SCurveContrast");
		}
		sCurveContrast2.RedSteepness = sCurveContrast.RedSteepness;
		sCurveContrast2.RedGamma = sCurveContrast.RedGamma;
		sCurveContrast2.GreenSteepness = sCurveContrast.GreenSteepness;
		sCurveContrast2.GreenGamma = sCurveContrast.GreenGamma;
		sCurveContrast2.BlueSteepness = sCurveContrast.BlueSteepness;
		sCurveContrast2.BlueGamma = sCurveContrast.BlueGamma;
		UnityEngine.Object.Destroy(sCurveContrast);
		effectInfo.m_Compnent = sCurveContrast2;
	}

	private void SaveColorCurveState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_cccLayerFlag = 0;
			saveData.m_cccRedSteepness = 0f;
			saveData.m_cccRedGamma = 0f;
			saveData.m_cccGreenSteepness = 0f;
			saveData.m_cccGreenGamma = 0f;
			saveData.m_cccBlueSteepness = 0f;
			saveData.m_cccBlueGamma = 0f;
		}
		else
		{
			saveData.m_cccLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_cccRedSteepness = (float)effectInfo.m_Params[1];
			saveData.m_cccRedGamma = (float)effectInfo.m_Params[2];
			saveData.m_cccGreenSteepness = (float)effectInfo.m_Params[3];
			saveData.m_cccGreenGamma = (float)effectInfo.m_Params[4];
			saveData.m_cccBlueSteepness = (float)effectInfo.m_Params[5];
			saveData.m_cccBlueGamma = (float)effectInfo.m_Params[6];
		}
	}

	public bool Activate_GaussianBlur(int layerFlag, float fTime = 0f, int iPasses = 1, float fDownscaling = 1f)
	{
		if (IsActivatedEffect(Effects.GaussianBlur))
		{
			int layerID_byLayerFlag = GetLayerID_byLayerFlag(layerFlag);
			EffectInfo effectInfo = m_EffectInfos[9];
			if (effectInfo.m_LayerID == layerID_byLayerFlag)
			{
				return false;
			}
			Deactivate_GaussianBlur();
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		int num = Mathf.Clamp(iPasses, 0, 10);
		if (num <= 0)
		{
			return false;
		}
		float num2 = Mathf.Clamp(fDownscaling, 1f, 16f);
		GameObject gameObject = camera_byLayerFlag.gameObject;
		GaussianBlur gaussianBlur = gameObject.GetComponent<GaussianBlur>();
		if (gaussianBlur == null)
		{
			gaussianBlur = gameObject.AddComponent<GaussianBlur>();
			gaussianBlur.Shader = Shader.Find("Hidden/Colorful/Gaussian Blur");
		}
		bool flag = fTime <= 0f || num2 == 1f;
		gaussianBlur.Passes = num;
		gaussianBlur.Downscaling = ((!flag) ? 1f : num2);
		gaussianBlur.Amount = 1f;
		EffectInfo effectInfo2 = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.GaussianBlur, flag ? EffectState.Enable : EffectState.Activating, gaussianBlur, 4);
		effectInfo2.m_fpUpdate = Update_GaussianBlur;
		effectInfo2.m_fpChangeCam = ChangeCamera_GaussianBlur;
		effectInfo2.m_Params[0] = fTime;
		effectInfo2.m_Params[1] = num2;
		effectInfo2.m_Params[2] = 0f;
		effectInfo2.m_Params[3] = gaussianBlur.Downscaling;
		return true;
	}

	public bool Deactivate_GaussianBlur(float fTime = 0f)
	{
		EffectInfo effectInfo = m_EffectInfos[9];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void Update_GaussianBlur()
	{
		EffectInfo effectInfo = m_EffectInfos[9];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = ((!(num3 > 1f)) ? 1f : ((num3 - 1f) / num2 * num4 + 1f));
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = ((!(num3 > 1f)) ? 1f : ((num3 - 1f) / num2 * num4 + 1f));
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as GaussianBlur).Downscaling = num5;
	}

	private void ChangeCamera_GaussianBlur(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[9];
		GaussianBlur gaussianBlur = effectInfo.m_Compnent as GaussianBlur;
		GameObject gameObject = newCamera.gameObject;
		GaussianBlur gaussianBlur2 = gameObject.GetComponent<GaussianBlur>();
		if (gaussianBlur2 == null)
		{
			gaussianBlur2 = gameObject.AddComponent<GaussianBlur>();
			gaussianBlur2.Shader = Shader.Find("Hidden/Colorful/Gaussian Blur");
		}
		gaussianBlur2.Amount = gaussianBlur.Amount;
		gaussianBlur2.Downscaling = gaussianBlur.Downscaling;
		gaussianBlur2.Passes = gaussianBlur.Passes;
		UnityEngine.Object.Destroy(gaussianBlur);
		effectInfo.m_Compnent = gaussianBlur2;
	}

	private void SaveGaussianBlurState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_gbLayerFlag = 0;
			saveData.m_gbPasses = 0;
			saveData.m_gbDownscaling = 0f;
		}
		else
		{
			GaussianBlur gaussianBlur = effectInfo.m_Compnent as GaussianBlur;
			saveData.m_gbLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_gbPasses = gaussianBlur.Passes;
			saveData.m_gbDownscaling = (float)effectInfo.m_Params[1];
		}
	}

	public bool Activate_RadialBlur(int layerFlag, float fTime = 0f, float fMaxStrength = 0.1f, float fCenterPosX = 0.5f, float fCenterPosY = 0.5f, bool isEnableVignette = false, float fSharpness = 40f, float fDarkness = 35f)
	{
		if (IsActivatedEffect(Effects.RadialBlur))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(fMaxStrength);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		RadialBlur radialBlur = gameObject.GetComponent<RadialBlur>();
		if (radialBlur == null)
		{
			radialBlur = gameObject.AddComponent<RadialBlur>();
			radialBlur.Shader = Shader.Find("Hidden/Colorful/Radial Blur");
		}
		radialBlur.Strength = ((!(fTime <= 0f)) ? 0f : num);
		radialBlur.Center = new Vector2(Mathf.Clamp01(fCenterPosX), Mathf.Clamp01(fCenterPosY));
		radialBlur.EnableVignette = isEnableVignette;
		radialBlur.Sharpness = Mathf.Clamp(fSharpness, 0f, 100f);
		radialBlur.Darkness = Mathf.Clamp(fDarkness, 0f, 100f);
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.RadialBlur, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, radialBlur, 4);
		effectInfo.m_fpUpdate = Update_RadialBlur;
		effectInfo.m_fpChangeCam = ChangeCamera_RadialBlur;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = radialBlur.Strength;
		return true;
	}

	public bool Deactivate_RadialBlur(float fTime = 0f)
	{
		EffectInfo effectInfo = m_EffectInfos[10];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void Update_RadialBlur()
	{
		EffectInfo effectInfo = m_EffectInfos[10];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as RadialBlur).Strength = num5;
	}

	private void ChangeCamera_RadialBlur(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[10];
		RadialBlur radialBlur = effectInfo.m_Compnent as RadialBlur;
		GameObject gameObject = newCamera.gameObject;
		RadialBlur radialBlur2 = gameObject.GetComponent<RadialBlur>();
		if (radialBlur2 == null)
		{
			radialBlur2 = gameObject.AddComponent<RadialBlur>();
			radialBlur2.Shader = Shader.Find("Hidden/Colorful/Radial Blur");
		}
		radialBlur2.Center = radialBlur.Center;
		radialBlur2.Darkness = radialBlur.Darkness;
		radialBlur2.EnableVignette = radialBlur.EnableVignette;
		radialBlur2.Quality = radialBlur.Quality;
		radialBlur2.Samples = radialBlur.Samples;
		radialBlur2.Sharpness = radialBlur.Sharpness;
		radialBlur2.Strength = radialBlur.Strength;
		UnityEngine.Object.Destroy(radialBlur);
		effectInfo.m_Compnent = radialBlur2;
	}

	private void SaveRadialBlurState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_rbLayerFlag = 0;
			saveData.m_rbMaxStrength = 0f;
			saveData.m_rbCenterPosX = 0f;
			saveData.m_rbCenterPosY = 0f;
			saveData.m_rbEnableVignette = false;
			saveData.m_rbSharpness = 0f;
			saveData.m_rbDarkness = 0f;
		}
		else
		{
			RadialBlur radialBlur = effectInfo.m_Compnent as RadialBlur;
			saveData.m_rbLayerFlag = GetLayerID_byLayerFlag(effectInfo.m_LayerID);
			saveData.m_rbMaxStrength = (float)effectInfo.m_Params[1];
			saveData.m_rbCenterPosX = radialBlur.Center.x;
			saveData.m_rbCenterPosY = radialBlur.Center.x;
			saveData.m_rbEnableVignette = radialBlur.EnableVignette;
			saveData.m_rbSharpness = radialBlur.Sharpness;
			saveData.m_rbDarkness = radialBlur.Darkness;
		}
	}

	private EffectInfo Activate_Covered(int layerFlag, float fMaxAmount = 1f, int iR = 0, int iG = 0, int iB = 0)
	{
		if (IfExistCoverChangeNewLayerID(layerFlag))
		{
			return m_EffectInfos[11];
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return null;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return null;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		ScreenFade screenFade = GetComponent_ScreenFade(gameObject, Effects.Cover);
		if (screenFade == null)
		{
			screenFade = gameObject.AddComponent<ScreenFade>();
			screenFade.UsedEffType = Effects.Cover;
		}
		screenFade.Red = ((iR <= 0) ? 0f : ((float)iR / 255f));
		screenFade.Green = ((iG <= 0) ? 0f : ((float)iG / 255f));
		screenFade.Blue = ((iB <= 0) ? 0f : ((float)iB / 255f));
		screenFade.Amount = num;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Cover, EffectState.Enable, screenFade, 0);
		effectInfo.m_CoverType = CoverType.Covered;
		effectInfo.m_fpChangeCam = ChangeCamera_Cover;
		RenderManager.instance.DeactivateCamera(layerFlag);
		return effectInfo;
	}

	private EffectInfo Deactivate_Covered()
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return null;
		}
		RenderManager.instance.ActivateCamera();
		ReleaseEffectInfo(effectInfo);
		return effectInfo;
	}

	private bool IfExistCoverChangeNewLayerID(int layerFlag)
	{
		if (!IsActivatedEffect(Effects.Cover))
		{
			return false;
		}
		EffectInfo effectInfo = m_EffectInfos[11];
		int layerID_byLayerFlag = GetLayerID_byLayerFlag(layerFlag);
		if (layerID_byLayerFlag == effectInfo.m_LayerID)
		{
			return true;
		}
		effectInfo.m_LayerID = layerID_byLayerFlag;
		ChangeCamera_Cover(RenderManager.instance.GetCamera_byLayerId(layerID_byLayerFlag));
		RenderManager.instance.DeactivateCamera(layerFlag);
		return true;
	}

	private void ChangeCamera_Cover(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo.m_CoverType == CoverType.Covered || effectInfo.m_CoverType == CoverType.Fade || effectInfo.m_CoverType == CoverType.FadeCircle)
		{
			ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
			GameObject gameObject = newCamera.gameObject;
			ScreenFade screenFade2 = GetComponent_ScreenFade(gameObject, Effects.Cover);
			if (screenFade2 == null)
			{
				screenFade2 = gameObject.AddComponent<ScreenFade>();
				screenFade2.UsedEffType = Effects.Cover;
			}
			screenFade2.Red = screenFade.Red;
			screenFade2.Green = screenFade.Green;
			screenFade2.Blue = screenFade.Blue;
			screenFade2.Amount = screenFade.Amount;
			UnityEngine.Object.DestroyImmediate(screenFade);
			effectInfo.m_Compnent = screenFade2;
		}
	}

	private void SaveCoverState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_coverLayerFlag = 0;
			saveData.m_coverMaxAmount = 0f;
			saveData.m_coverR = 0;
			saveData.m_coverG = 0;
			saveData.m_coverB = 0;
		}
		else
		{
			ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
			saveData.m_coverLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_coverMaxAmount = screenFade.Amount;
			saveData.m_coverR = Mathf.RoundToInt(screenFade.Red * 255f);
			saveData.m_coverG = Mathf.RoundToInt(screenFade.Green * 255f);
			saveData.m_coverB = Mathf.RoundToInt(screenFade.Blue * 255f);
		}
	}

	private void TransTo_Covered(int layerID, float fAmount, float fR, float fG, float fB)
	{
		Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(layerID);
		GameObject gameObject = camera_byLayerId.gameObject;
		ScreenFade screenFade = GetComponent_ScreenFade(gameObject, Effects.Cover);
		if (screenFade == null)
		{
			screenFade = gameObject.AddComponent<ScreenFade>();
			screenFade.UsedEffType = Effects.Cover;
		}
		screenFade.Red = fR;
		screenFade.Green = fG;
		screenFade.Blue = fB;
		screenFade.Amount = fAmount;
		EffectInfo effectInfo = SetEffectInfo(layerID, Effects.Cover, EffectState.Enable, screenFade, 0);
		effectInfo.m_CoverType = CoverType.Covered;
		effectInfo.m_fpUpdate = null;
		effectInfo.m_fpChangeCam = ChangeCamera_Cover;
		RenderManager.instance.DeactivateCamera(GetLayerID_byLayerFlag(effectInfo.m_LayerID));
	}

	private bool TransFrom_Covered(ref EffectInfo toEffInfo, Effects effType, int paramCount)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		toEffInfo = SetEffectInfo(effectInfo.m_LayerID, effType, EffectState.Enable, effectInfo.m_Compnent, paramCount);
		ReleaseEffectInfo(effectInfo);
		RenderManager.instance.ActivateCamera();
		return true;
	}

	private IEnumerator CoverIn_CommonCanvasObj(CoverType type, int layerFlag, float speedRate = 1f, int orderInLayer = 0)
	{
		m_returnValue_CoverInCommonCanvasObj = false;
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		if (IfExistCoverChangeNewLayerID(layerFlag))
		{
			SetCanvasObject(layerID, sortingLayerID, orderInLayer);
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		if (speedRate <= 0f)
		{
			Activate_Covered(layerFlag);
			yield break;
		}
		SetCanvasObject(layerID, sortingLayerID, orderInLayer);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.Cover, EffectState.Activating, null, 0);
		effInfo.m_CoverType = type;
		if (effInfo.m_GameObj != null)
		{
			UnityEngine.Object.DestroyImmediate(effInfo.m_GameObj);
			effInfo.m_GameObj = null;
		}
		string assetPath = GetPrefabAssetPath(Effects.Cover, type);
		GameObject srcObject = GetPrefabSrcObject(assetPath);
		if (srcObject == null)
		{
			effInfo.m_isLoadingAsset = true;
			yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(assetPath));
			effInfo.m_isLoadingAsset = false;
			AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler(assetPath);
			if (assetBundleObjectHandler == null)
			{
				goto IL_02bf;
			}
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
		}
		if (srcObject == null)
		{
			goto IL_02bf;
		}
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		effInfo.m_GameObj.SetActive(value: true);
		m_returnValue_CoverInCommonCanvasObj = true;
		yield break;
		IL_02bf:
		ReleaseEffectInfo(effInfo);
		m_returnValue_CoverInCommonCanvasObj = false;
	}

	private IEnumerator CoverOut_CommonCanvasObj(CoverType type, int layerFlag, float speedRate = 1f, int orderInLayer = 0)
	{
		m_returnValue_CoverOutCommonCanvasObj = false;
		RenderManager.instance.ActivateCamera();
		if (!IsActivatedAnyCoveredEffect())
		{
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		if (speedRate <= 0f)
		{
			Deactivate_Covered();
			yield break;
		}
		EffectInfo effInfo = m_EffectInfos[11];
		if (layerFlag < 0)
		{
			layerFlag = GetLayerFlag_byLayerID(effInfo.m_LayerID);
		}
		if (effInfo.m_Compnent != null && effInfo.m_Compnent is ScreenFade)
		{
			UnityEngine.Object.Destroy(effInfo.m_Compnent);
			effInfo.m_Compnent = null;
		}
		if (effInfo.m_GameObj != null)
		{
			UnityEngine.Object.DestroyImmediate(effInfo.m_GameObj);
			effInfo.m_GameObj = null;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, orderInLayer);
		effInfo.m_LayerID = layerID;
		effInfo.m_State = EffectState.Activating;
		string assetPath = GetPrefabAssetPath(Effects.Cover, type, 1);
		GameObject srcObject = GetPrefabSrcObject(assetPath);
		if (srcObject == null)
		{
			effInfo.m_isLoadingAsset = true;
			yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(assetPath));
			effInfo.m_isLoadingAsset = false;
			AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler(assetPath);
			if (assetBundleObjectHandler == null)
			{
				goto IL_030b;
			}
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
		}
		if (srcObject == null)
		{
			goto IL_030b;
		}
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		effInfo.m_GameObj.SetActive(value: true);
		m_returnValue_CoverOutCommonCanvasObj = true;
		yield break;
		IL_030b:
		ReleaseEffectInfo(effInfo);
		m_returnValue_CoverOutCommonCanvasObj = false;
	}

	private void CompleteCB_CommonCoverIn()
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		TransTo_Covered(effectInfo.m_LayerID, 1f, 0f, 0f, 0f);
		if (effectInfo.m_GameObj != null)
		{
			UnityEngine.Object.Destroy(effectInfo.m_GameObj);
			effectInfo.m_GameObj = null;
		}
	}

	private void CompleteCB_CommonCoverIn(object sender, object arg)
	{
		CompleteCB_CommonCoverIn();
	}

	private void CompleteCB_CommonCoverOut()
	{
		EffectInfo effInfo = m_EffectInfos[11];
		ReleaseEffectInfo(effInfo, isDestroyGameObject: true);
	}

	private void CompleteCB_CommonCoverOut(object sender, object arg)
	{
		CompleteCB_CommonCoverOut();
	}

	public bool Activate_FadeIn(int layerFlag, float fTime, float fMaxAmount = 1f, int iR = 0, int iG = 0, int iB = 0)
	{
		if (IfExistCoverChangeNewLayerID(layerFlag))
		{
			return true;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			return Activate_Covered(layerFlag, fMaxAmount, iR, iG, iB) != null;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		ScreenFade screenFade = GetComponent_ScreenFade(gameObject, Effects.Cover);
		if (screenFade == null)
		{
			screenFade = gameObject.AddComponent<ScreenFade>();
			screenFade.UsedEffType = Effects.Cover;
		}
		screenFade.Red = (float)iR / 255f;
		screenFade.Green = (float)iG / 255f;
		screenFade.Blue = (float)iB / 255f;
		screenFade.Amount = 0f;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Cover, EffectState.Activating, screenFade, 4);
		effectInfo.m_CoverType = CoverType.Fade;
		effectInfo.m_fpUpdate = UpdateProc_ScreenFade;
		effectInfo.m_fpChangeCam = ChangeCamera_ScreenFade;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = 0f;
		return true;
	}

	public bool Activate_FadeOut(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_State == EffectState.Deactivating || effectInfo.m_State == EffectState.Disable)
		{
			return false;
		}
		Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(effectInfo.m_LayerID);
		if (camera_byLayerId == null)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			return Deactivate_Covered() != null;
		}
		RenderManager.instance.ActivateCamera();
		bool flag = effectInfo.m_CoverType == CoverType.Covered;
		bool flag2 = effectInfo.m_CoverType == CoverType.Fade && effectInfo.m_State == EffectState.Activating;
		if (flag)
		{
			effectInfo.m_State = EffectState.Deactivating;
			effectInfo.m_CoverType = CoverType.Fade;
			effectInfo.m_fpUpdate = UpdateProc_ScreenFade;
			effectInfo.m_fpChangeCam = ChangeCamera_ScreenFade;
			if (effectInfo.m_Params == null || effectInfo.m_Params.Length < 4)
			{
				effectInfo.m_Params = new object[4];
			}
			effectInfo.m_Params[0] = fTime;
			effectInfo.m_Params[1] = ((ScreenFade)effectInfo.m_Compnent).Amount;
			effectInfo.m_Params[2] = fTime;
			effectInfo.m_Params[3] = effectInfo.m_Params[1];
			return true;
		}
		if (flag2)
		{
			float num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
			effectInfo.m_Params[0] = num;
			effectInfo.m_Params[1] = effectInfo.m_Params[3];
			effectInfo.m_Params[2] = num;
			effectInfo.m_State = EffectState.Deactivating;
			return true;
		}
		return false;
	}

	private void UpdateProc_ScreenFade()
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				effectInfo.m_CoverType = CoverType.Covered;
				(effectInfo.m_Compnent as ScreenFade).Amount = num3;
				effectInfo.m_fpUpdate = null;
				effectInfo.m_fpChangeCam = ChangeCamera_Cover;
				RenderManager.instance.DeactivateCamera(GetLayerFlag_byLayerID(effectInfo.m_LayerID));
				return;
			}
			num5 = num3 / num2 * num4;
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as ScreenFade).Amount = num5;
	}

	private void ChangeCamera_ScreenFade(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
		GameObject gameObject = newCamera.gameObject;
		ScreenFade screenFade2 = GetComponent_ScreenFade(gameObject, Effects.Cover);
		if (screenFade2 == null)
		{
			screenFade2 = gameObject.AddComponent<ScreenFade>();
			screenFade2.UsedEffType = Effects.Cover;
		}
		screenFade2.Red = screenFade.Red;
		screenFade2.Green = screenFade.Green;
		screenFade2.Blue = screenFade.Blue;
		screenFade2.Amount = screenFade.Amount;
		UnityEngine.Object.Destroy(screenFade);
		effectInfo.m_Compnent = screenFade2;
	}

	public bool Activate_FadeCircleIn(int layerFlag, float fTime, float fCenterX = 0.5f, float fCenterY = 0.5f, float fTargetRadius = 0f, float fAlpha = 1f, int iR = 0, int iG = 0, int iB = 0)
	{
		if (IfExistCoverChangeNewLayerID(layerFlag))
		{
			return true;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		if (fTime <= 0f && fTargetRadius <= 0f)
		{
			return Activate_Covered(layerFlag, fAlpha, iR, iG, iB) != null;
		}
		float num = Mathf.Clamp01(fTargetRadius);
		if (num >= 1f)
		{
			return false;
		}
		float num2 = Mathf.Clamp01(fAlpha);
		if (num2 <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		ScreenFadeCircle screenFadeCircle = gameObject.GetComponent<ScreenFadeCircle>();
		if (screenFadeCircle == null)
		{
			screenFadeCircle = gameObject.AddComponent<ScreenFadeCircle>();
		}
		screenFadeCircle.enabled = true;
		screenFadeCircle.Red = (float)(iR & 0xFF) / 255f;
		screenFadeCircle.Green = (float)(iG & 0xFF) / 255f;
		screenFadeCircle.Blue = (float)(iB & 0xFF) / 255f;
		screenFadeCircle.Alpha = num2;
		screenFadeCircle.CenterX = fCenterX;
		screenFadeCircle.CenterY = fCenterY;
		screenFadeCircle.Radius = 1f;
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Cover, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, screenFadeCircle, 4);
		effectInfo.m_CoverType = CoverType.FadeCircle;
		effectInfo.m_fpUpdate = UpdateProc_ScreenFadeCircle;
		effectInfo.m_fpChangeCam = ChangeCamera_ScreenFadeCircle;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = screenFadeCircle.Radius;
		return true;
	}

	public bool Activate_FadeCircleOut(float fTime, float fCenterX = 0.5f, float fCenterY = 0.5f)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_State == EffectState.Deactivating || effectInfo.m_State == EffectState.Disable)
		{
			return false;
		}
		Camera camera_byLayerId = RenderManager.instance.GetCamera_byLayerId(effectInfo.m_LayerID);
		if (camera_byLayerId == null)
		{
			return false;
		}
		if (fTime <= 0f)
		{
			return Deactivate_Covered() != null;
		}
		RenderManager.instance.ActivateCamera();
		if (effectInfo.m_CoverType == CoverType.Covered)
		{
			ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
			ScreenFadeCircle screenFadeCircle = camera_byLayerId.gameObject.GetComponent<ScreenFadeCircle>();
			if (screenFadeCircle == null)
			{
				screenFadeCircle = camera_byLayerId.gameObject.AddComponent<ScreenFadeCircle>();
			}
			screenFadeCircle.Red = screenFade.Red;
			screenFadeCircle.Green = screenFade.Green;
			screenFadeCircle.Blue = screenFade.Blue;
			screenFadeCircle.Alpha = screenFade.Amount;
			screenFadeCircle.CenterX = fCenterX;
			screenFadeCircle.CenterY = fCenterY;
			screenFadeCircle.Radius = 0f;
			effectInfo.m_State = EffectState.Deactivating;
			effectInfo.m_CoverType = CoverType.FadeCircle;
			effectInfo.m_Compnent = screenFadeCircle;
			effectInfo.m_fpUpdate = UpdateProc_ScreenFadeCircle;
			effectInfo.m_fpChangeCam = ChangeCamera_ScreenFadeCircle;
			if (effectInfo.m_Params == null || effectInfo.m_Params.Length < 4)
			{
				effectInfo.m_Params = new object[4];
			}
			effectInfo.m_Params[0] = fTime;
			effectInfo.m_Params[1] = 1f;
			effectInfo.m_Params[2] = fTime;
			effectInfo.m_Params[3] = 1f;
			UnityEngine.Object.Destroy(screenFade);
			return true;
		}
		if (effectInfo.m_CoverType == CoverType.FadeCircle)
		{
			float num = fTime;
			if (effectInfo.m_State == EffectState.Activating)
			{
				num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
			}
			effectInfo.m_State = EffectState.Deactivating;
			effectInfo.m_fpUpdate = UpdateProc_ScreenFadeCircle;
			effectInfo.m_fpChangeCam = ChangeCamera_ScreenFadeCircle;
			effectInfo.m_Params[0] = num;
			effectInfo.m_Params[1] = 1f - (float)effectInfo.m_Params[3];
			effectInfo.m_Params[2] = num;
			return true;
		}
		return false;
	}

	private void UpdateProc_ScreenFadeCircle()
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				if (num3 <= 0f)
				{
					ScreenFadeCircle screenFadeCircle = effectInfo.m_Compnent as ScreenFadeCircle;
					TransTo_Covered(effectInfo.m_LayerID, screenFadeCircle.Alpha, screenFadeCircle.Red, screenFadeCircle.Green, screenFadeCircle.Blue);
					UnityEngine.Object.Destroy(screenFadeCircle);
					return;
				}
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
			}
			else
			{
				num5 = (1f - num3) / num2 * num4;
				num5 = 1f - num5;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
			num5 = 1f - num5;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as ScreenFadeCircle).Radius = num5;
	}

	private void ChangeCamera_ScreenFadeCircle(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[11];
		ScreenFadeCircle screenFadeCircle = effectInfo.m_Compnent as ScreenFadeCircle;
		GameObject gameObject = newCamera.gameObject;
		ScreenFadeCircle screenFadeCircle2 = gameObject.GetComponent<ScreenFadeCircle>();
		if (screenFadeCircle2 == null)
		{
			screenFadeCircle2 = gameObject.AddComponent<ScreenFadeCircle>();
		}
		screenFadeCircle2.Red = screenFadeCircle.Red;
		screenFadeCircle2.Green = screenFadeCircle.Green;
		screenFadeCircle2.Blue = screenFadeCircle.Blue;
		screenFadeCircle2.Alpha = screenFadeCircle.Alpha;
		screenFadeCircle2.CenterX = screenFadeCircle.CenterX;
		screenFadeCircle2.CenterY = screenFadeCircle.CenterY;
		screenFadeCircle2.Radius = screenFadeCircle.Radius;
		UnityEngine.Object.Destroy(screenFadeCircle);
		effectInfo.m_Compnent = screenFadeCircle2;
	}

	public IEnumerator Activate_CoverIn(int layerFlag, int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.Normal, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverIn, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverOut(int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.Normal, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverOut, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverFlatIn(int layerFlag, int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.Flat, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverIn, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverFlatOut(int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.Flat, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverOut, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverFlatBlurIn(int layerFlag, int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.FlatBlur, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverIn, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverFlatBlurOut(int iType, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.FlatBlur, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CoverInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverInOutEffectHdr>();
				coverEffHdr.EffType = (CoverInOutEffectHdr.EffectType)iType;
				coverEffHdr.Play(CompleteCB_CommonCoverOut, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverBrushIn(int layerFlag, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.Brush, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CoverBrushInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverBrushInOutEffectHdr>();
				coverEffHdr.Play(CompleteCB_CommonCoverIn, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverBrushOut(float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.Brush, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CoverBrushInOutEffectHdr coverEffHdr = effInfo.m_GameObj.GetComponent<CoverBrushInOutEffectHdr>();
				coverEffHdr.Play(CompleteCB_CommonCoverOut, fSpeedRate);
			}
		}
	}

	public IEnumerator Activate_CoverConcreteIn(int layerFlag, int type, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.Concrete, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CommonCoverEffect coverEff = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEff.Play(type, fSpeedRate, CompleteCB_CommonCoverIn);
			}
		}
	}

	public IEnumerator Activate_CoverConcreteOut(int type, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.Concrete, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CommonCoverEffect coverEff = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEff.Play(type, fSpeedRate, CompleteCB_CommonCoverOut);
			}
		}
	}

	public IEnumerator Activate_CoverConcreteBlurIn(int layerFlag, int type, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.ConcreteBlur, layerFlag, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CommonCoverEffect coverEff = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEff.Play(type, fSpeedRate, CompleteCB_CommonCoverIn);
			}
		}
	}

	public IEnumerator Activate_CoverConcreteBlurOut(int type, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.ConcreteBlur, -1, fSpeedRate, iOrderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(fSpeedRate <= 0f))
			{
				CommonCoverEffect coverEff = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEff.Play(type, fSpeedRate, CompleteCB_CommonCoverOut);
			}
		}
	}

	public IEnumerator Activate_EyeOpen(int layerFlag, float speedRate = 1f, int orderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverOut_CommonCanvasObj(CoverType.EyeView, layerFlag, speedRate, orderInLayer));
		if (m_returnValue_CoverOutCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null && !(speedRate <= 0f))
			{
				CommonCoverEffect coverEffect = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEffect.Play(0, speedRate, CompleteCB_CommonCoverOut);
			}
		}
	}

	public IEnumerator Activate_EyeClose(int layerFlag, float speedRate = 1f, int orderInLayer = 0)
	{
		yield return MainLoadThing.instance.StartCoroutine(CoverIn_CommonCanvasObj(CoverType.EyeView, layerFlag, speedRate, orderInLayer));
		if (m_returnValue_CoverInCommonCanvasObj)
		{
			EffectInfo effInfo = m_EffectInfos[11];
			if (effInfo != null)
			{
				CommonCoverEffect coverEffect = effInfo.m_GameObj.GetComponent<CommonCoverEffect>();
				coverEffect.Play(0, speedRate, CompleteCB_CommonCoverIn);
			}
		}
	}

	public bool Activate_FlashIn(int layerFlag, float fTime, float fMaxAmount = 1f, int iR = 255, int iG = 255, int iB = 255)
	{
		if (IsActivatedEffect(Effects.Flash))
		{
			return false;
		}
		Camera camera_byLayerFlag = GetCamera_byLayerFlag(layerFlag);
		if (camera_byLayerFlag == null)
		{
			return false;
		}
		float num = Mathf.Clamp01(fMaxAmount);
		if (num <= 0f)
		{
			return false;
		}
		GameObject gameObject = camera_byLayerFlag.gameObject;
		ScreenFade screenFade = GetComponent_ScreenFade(gameObject, Effects.Flash);
		if (screenFade == null)
		{
			screenFade = gameObject.AddComponent<ScreenFade>();
			screenFade.UsedEffType = Effects.Flash;
		}
		screenFade.Red = (float)iR / 255f;
		screenFade.Green = (float)iG / 255f;
		screenFade.Blue = (float)iB / 255f;
		screenFade.Amount = ((!(fTime <= 0f)) ? 0f : num);
		EffectInfo effectInfo = SetEffectInfo(GetLayerID_byLayerFlag(layerFlag), Effects.Flash, (fTime <= 0f) ? EffectState.Enable : EffectState.Activating, screenFade, 4);
		effectInfo.m_fpUpdate = UpdateProc_ScreenFlash;
		effectInfo.m_fpChangeCam = ChangeCamera_ScreenFlash;
		effectInfo.m_Params[0] = fTime;
		effectInfo.m_Params[1] = num;
		effectInfo.m_Params[2] = 0f;
		effectInfo.m_Params[3] = screenFade.Amount;
		return true;
	}

	public bool Activate_FlashOut(float fTime)
	{
		EffectInfo effectInfo = m_EffectInfos[12];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		RenderManager.instance.ActivateCamera();
		if (fTime <= 0f)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		float num = fTime;
		if (effectInfo.m_State == EffectState.Activating)
		{
			num = fTime * ((float)effectInfo.m_Params[2] / (float)effectInfo.m_Params[0]);
		}
		effectInfo.m_Params[0] = num;
		effectInfo.m_Params[1] = effectInfo.m_Params[3];
		effectInfo.m_Params[2] = num;
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_ScreenFlash()
	{
		EffectInfo effectInfo = m_EffectInfos[12];
		if (effectInfo == null || effectInfo.m_Compnent == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		EventEngine instance = EventEngine.GetInstance();
		float num = ((!instance.GetSkip()) ? Time.deltaTime : (instance.GetLerpSkipValue() * Time.deltaTime));
		float num2 = (float)effectInfo.m_Params[0];
		float num3 = (float)effectInfo.m_Params[1];
		float num4 = (float)effectInfo.m_Params[2];
		float num5 = (float)effectInfo.m_Params[3];
		if (effectInfo.m_State == EffectState.Activating)
		{
			num4 += num;
			if (num4 >= num2)
			{
				effectInfo.m_State = EffectState.Enable;
				num4 = num2;
				num5 = num3;
				RenderManager.instance.DeactivateCamera(GetLayerFlag_byLayerID(effectInfo.m_LayerID));
			}
			else
			{
				num5 = num3 / num2 * num4;
			}
		}
		else
		{
			num4 -= num;
			if (num4 <= 0f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
			num5 = num3 / num2 * num4;
		}
		effectInfo.m_Params[2] = num4;
		effectInfo.m_Params[3] = num5;
		(effectInfo.m_Compnent as ScreenFade).Amount = num5;
	}

	private void ChangeCamera_ScreenFlash(Camera newCamera)
	{
		EffectInfo effectInfo = m_EffectInfos[12];
		ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
		GameObject gameObject = newCamera.gameObject;
		ScreenFade screenFade2 = GetComponent_ScreenFade(gameObject, Effects.Flash);
		if (screenFade2 == null)
		{
			screenFade2 = gameObject.AddComponent<ScreenFade>();
			screenFade2.UsedEffType = Effects.Flash;
		}
		screenFade2.Red = screenFade.Red;
		screenFade2.Green = screenFade.Green;
		screenFade2.Blue = screenFade.Blue;
		screenFade2.Amount = screenFade.Amount;
		UnityEngine.Object.Destroy(screenFade);
		effectInfo.m_Compnent = screenFade2;
	}

	private void SaveFlashState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_Compnent == null)
		{
			saveData.m_flashLayerFlag = 0;
			saveData.m_flashMaxAmount = 0f;
			saveData.m_flashR = 0;
			saveData.m_flashG = 0;
			saveData.m_flashB = 0;
		}
		else
		{
			ScreenFade screenFade = effectInfo.m_Compnent as ScreenFade;
			saveData.m_flashLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_flashMaxAmount = (float)effectInfo.m_Params[1];
			saveData.m_flashR = Mathf.RoundToInt(screenFade.Red * 255f);
			saveData.m_flashG = Mathf.RoundToInt(screenFade.Green * 255f);
			saveData.m_flashB = Mathf.RoundToInt(screenFade.Blue * 255f);
		}
	}

	public IEnumerator Activate_BackgroundStream(int layerFlag, int iType = 0, float fSpeedRate = 1f, int iOrderInLayer = 0)
	{
		if (IsActivatedEffect(Effects.BackgroundStream))
		{
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, iOrderInLayer);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.BackgroundStream, (fSpeedRate <= 0f) ? EffectState.Enable : EffectState.Activating, null, 2);
		string assetPath = ((iType != 1) ? "Prefabs/CameraEffect/BackgroundFlow" : "Prefabs/CameraEffect/BackgroundFlow_Vertical");
		GameObject srcObject = GetPrefabSrcObject(assetPath);
		if (!(srcObject == null))
		{
			goto IL_01c7;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject(assetPath));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler(assetPath);
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01c7;
			}
		}
		goto IL_02cf;
		IL_02cf:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_01c7:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_02cf;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		effInfo.m_GameObj.SetActive(value: true);
		effInfo.m_Params[0] = iType;
		effInfo.m_Params[1] = fSpeedRate;
		BackgroundStreamHdr bgStreamEffHdr = effInfo.m_GameObj.GetComponent<BackgroundStreamHdr>();
		if (fSpeedRate <= 0f)
		{
			bgStreamEffHdr.Enable();
		}
		else
		{
			bgStreamEffHdr.Appear(CompleteCB_ActivateBGStream, fSpeedRate);
		}
	}

	public void CompleteCB_ActivateBGStream()
	{
		EffectInfo effectInfo = m_EffectInfos[13];
		effectInfo.m_State = EffectState.Enable;
	}

	public bool Deactivate_BackgroundStream(float fSpeedRate = 1f)
	{
		EffectInfo effectInfo = m_EffectInfos[13];
		if (effectInfo == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		if (fSpeedRate <= 0f)
		{
			CompleteCB_DeactivateBGStream();
			return true;
		}
		BackgroundStreamHdr component = effectInfo.m_GameObj.GetComponent<BackgroundStreamHdr>();
		component.Disappear(CompleteCB_DeactivateBGStream, fSpeedRate);
		return true;
	}

	public void CompleteCB_DeactivateBGStream()
	{
		EffectInfo effectInfo = m_EffectInfos[13];
		ReleaseEffectInfo(effectInfo);
		if (effectInfo.m_GameObj != null)
		{
			UnityEngine.Object.Destroy(effectInfo.m_GameObj);
			effectInfo.m_GameObj = null;
		}
	}

	private void SaveBGStreamState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_bgStreamLayerFlag = 0;
			saveData.m_bgStreamType = 0;
			saveData.m_bgStreamSpeedRate = 0f;
			saveData.m_bgStreamOrderInLayer = 0;
		}
		else
		{
			saveData.m_bgStreamLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_bgStreamType = (int)effectInfo.m_Params[0];
			saveData.m_bgStreamSpeedRate = (float)effectInfo.m_Params[1];
			saveData.m_bgStreamOrderInLayer = m_CanvasComponent.sortingOrder;
		}
	}

	public IEnumerator Activate_FocusLine(int layerFlag, int iOrderInLayer = 0)
	{
		if (IsActivatedEffect(Effects.FocusLine))
		{
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, iOrderInLayer);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.FocusLine, EffectState.Enable, null, 0);
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_020d;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/Intensive_Line");
		if (!(srcObject == null))
		{
			goto IL_01a3;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/Intensive_Line"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/Intensive_Line");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01a3;
			}
		}
		goto IL_0223;
		IL_01a3:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_0223;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_020d;
		IL_0223:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_020d:
		effInfo.m_GameObj.SetActive(value: true);
	}

	public bool Deactivate_FocusLine()
	{
		EffectInfo effectInfo = m_EffectInfos[14];
		if (effectInfo == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void SaveFocusLineState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_focusLineLayerFlag = 0;
			saveData.m_focusLineOrderInLayer = 0;
		}
		else
		{
			saveData.m_focusLineLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_focusLineOrderInLayer = m_CanvasComponent.sortingOrder;
		}
	}

	public IEnumerator Activate_FocusLineHori(int layerFlag, int iOrderInLayer = 0)
	{
		if (IsActivatedEffect(Effects.FocusLine_Hori))
		{
			yield break;
		}
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, iOrderInLayer);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.FocusLine_Hori, EffectState.Enable, null, 0);
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_020d;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/Intensive_Line_Horizontal");
		if (!(srcObject == null))
		{
			goto IL_01a3;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/Intensive_Line_Horizontal"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/Intensive_Line_Horizontal");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01a3;
			}
		}
		goto IL_0223;
		IL_01a3:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_0223;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_020d;
		IL_0223:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_020d:
		effInfo.m_GameObj.SetActive(value: true);
	}

	public bool Deactivate_FocusLineHori()
	{
		EffectInfo effectInfo = m_EffectInfos[15];
		if (effectInfo == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void SaveFocusLineHoriState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_focusLineHoriLayerFlag = 0;
			saveData.m_focusLineHoriOrderInLayer = 0;
		}
		else
		{
			saveData.m_focusLineHoriLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_focusLineHoriOrderInLayer = m_CanvasComponent.sortingOrder;
		}
	}

	public IEnumerator Activate_FallingStone(int layerFlag, int type, float speedRate = 1f, int orderInLayer = 0)
	{
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, orderInLayer);
		EffectInfo effInfo = null;
		effInfo = (IsActivatedEffect(Effects.FallingStone) ? m_EffectInfos[16] : SetEffectInfo(layerID, Effects.FallingStone, EffectState.Enable, null, 2));
		effInfo.m_Params[0] = type;
		effInfo.m_Params[1] = speedRate;
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_0258;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/Falling_Stone");
		if (!(srcObject == null))
		{
			goto IL_01ee;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/Falling_Stone"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/Falling_Stone");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01ee;
			}
		}
		goto IL_0293;
		IL_0258:
		FallingStoneEffect fallingStoneEff = effInfo.m_GameObj.GetComponent<FallingStoneEffect>();
		if (!fallingStoneEff.Show(type))
		{
			goto IL_0293;
		}
		yield break;
		IL_0293:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_01ee:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_0293;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_0258;
	}

	public bool Deactivate_FallingStone()
	{
		EffectInfo effectInfo = m_EffectInfos[16];
		if (effectInfo == null || effectInfo.m_GameObj == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		FallingStoneEffect component = effectInfo.m_GameObj.GetComponent<FallingStoneEffect>();
		component.Hide();
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void SaveFallingStoneState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_fsLayerFlag = 0;
			saveData.m_fsType = 0;
			saveData.m_fsSpeedRate = 0f;
			saveData.m_fsOrderInLayer = 0;
		}
		else
		{
			saveData.m_fsLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_fsType = (int)effectInfo.m_Params[0];
			saveData.m_fsSpeedRate = (float)effectInfo.m_Params[1];
			saveData.m_fsOrderInLayer = m_CanvasComponent.sortingOrder;
		}
	}

	public IEnumerator Activate_FallingStoneBlur(int layerFlag, int type, float speedRate = 1f, int orderInLayer = 0)
	{
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID, orderInLayer);
		EffectInfo effInfo = null;
		effInfo = (IsActivatedEffect(Effects.FallingStoneBlur) ? m_EffectInfos[17] : SetEffectInfo(layerID, Effects.FallingStoneBlur, EffectState.Enable, null, 2));
		effInfo.m_Params[0] = type;
		effInfo.m_Params[1] = speedRate;
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_0258;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/Falling_Stone");
		if (!(srcObject == null))
		{
			goto IL_01ee;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/Falling_Stone"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/Falling_Stone");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01ee;
			}
		}
		goto IL_02a9;
		IL_0258:
		FallingStoneEffect fallingStoneEff = effInfo.m_GameObj.GetComponent<FallingStoneEffect>();
		if (fallingStoneEff == null || !fallingStoneEff.Show(type))
		{
			goto IL_02a9;
		}
		yield break;
		IL_02a9:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_01ee:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_02a9;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_0258;
	}

	public bool Deactivate_FallingStoneBlur()
	{
		EffectInfo effectInfo = m_EffectInfos[17];
		if (effectInfo == null || effectInfo.m_GameObj == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		FallingStoneEffect component = effectInfo.m_GameObj.GetComponent<FallingStoneEffect>();
		component.Hide();
		ReleaseEffectInfo(effectInfo);
		return true;
	}

	private void SaveFallingStoneBlurState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_fsbLayerFlag = 0;
			saveData.m_fsbType = 0;
			saveData.m_fsbSpeedRate = 0f;
			saveData.m_fsbOrderInLayer = 0;
		}
		else
		{
			saveData.m_fsbLayerFlag = GetLayerFlag_byLayerID(effectInfo.m_LayerID);
			saveData.m_fsbType = (int)effectInfo.m_Params[0];
			saveData.m_fsbSpeedRate = (float)effectInfo.m_Params[1];
			saveData.m_fsbOrderInLayer = m_CanvasComponent.sortingOrder;
		}
	}

	public IEnumerator Activate_KeywordMenuBG(int iNosieLevel = 0)
	{
		if (IsActivatedEffect(Effects.KeywordMenuBG))
		{
			yield break;
		}
		int layer = 0;
		Camera camera = GetCamera_byLayerFlag(layer);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layer);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layer);
		SetCanvasObject(layerID, sortingLayerID);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.KeywordMenuBG, EffectState.Activating, null, 3);
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_020f;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/Keyword_CHRBG_Eff");
		if (!(srcObject == null))
		{
			goto IL_01a5;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/Keyword_CHRBG_Eff"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/Keyword_CHRBG_Eff");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01a5;
			}
		}
		goto IL_0300;
		IL_01a5:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_0300;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_020f;
		IL_0300:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_020f:
		effInfo.m_fpUpdate = UpdateProc_KeywordMenuBG;
		effInfo.m_GameObj.SetActive(value: true);
		KeywordMenuBGEffect effectHdr = effInfo.m_GameObj.GetComponent<KeywordMenuBGEffect>();
		effInfo.m_Params[0] = iNosieLevel;
		effInfo.m_Params[1] = effectHdr.m_MultiflyLayerAnimator;
		effInfo.m_Params[2] = effectHdr.m_NoiseAnimator;
		if (effectHdr.m_NoiseAnimator != null)
		{
			effectHdr.m_NoiseAnimator.gameObject.SetActive(value: false);
		}
		if (effectHdr.m_MultiflyLayerAnimator != null)
		{
			effectHdr.m_MultiflyLayerAnimator.Play(GameDefine.UIAnimationState.appear.ToString());
		}
	}

	public bool Deactivate_KeywordMenuBG()
	{
		EffectInfo effectInfo = m_EffectInfos[18];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Deactivating)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		if (effectInfo.m_Params[2] != null)
		{
			(effectInfo.m_Params[2] as Animator).gameObject.SetActive(value: false);
		}
		Animator animator = effectInfo.m_Params[1] as Animator;
		if (animator == null)
		{
			ReleaseEffectInfo(effectInfo);
			return true;
		}
		animator.Play(GameDefine.UIAnimationState.disappear.ToString());
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_KeywordMenuBG()
	{
		EffectInfo effectInfo = m_EffectInfos[18];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		bool flag = false;
		Animator animator = effectInfo.m_Params[1] as Animator;
		if (animator != null)
		{
			if (effectInfo.m_State == EffectState.Activating)
			{
				if (animator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.idle.ToString()))
				{
					effectInfo.m_State = EffectState.Enable;
					flag = true;
				}
			}
			else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
			{
				ReleaseEffectInfo(effectInfo);
				return;
			}
		}
		else if (effectInfo.m_State == EffectState.Activating)
		{
			effectInfo.m_State = EffectState.Enable;
			flag = true;
		}
		if (flag && (int)effectInfo.m_Params[0] > 0 && effectInfo.m_Params[2] != null)
		{
			int num = (int)effectInfo.m_Params[0];
			animator = effectInfo.m_Params[2] as Animator;
			animator.gameObject.SetActive(value: true);
			animator.Play(num.ToString());
		}
	}

	private void SaveKeywordMenuBGState(EffectInfo effectInfo, ref SaveData saveData, bool isActivated)
	{
		if (!isActivated || effectInfo.m_GameObj == null || m_CanvasComponent == null)
		{
			saveData.m_kmbgNoiseLevel = 0;
		}
		else
		{
			saveData.m_kmbgNoiseLevel = (int)effectInfo.m_Params[0];
		}
	}

	public IEnumerator Activate_SNSMenuBG()
	{
		if (IsActivatedEffect(Effects.SNSMenuBG))
		{
			yield break;
		}
		int layerFlag = 0;
		Camera camera = GetCamera_byLayerFlag(layerFlag);
		if (camera == null)
		{
			yield break;
		}
		int layerID = GetLayerID_byLayerFlag(layerFlag);
		int sortingLayerID = GetSortingLayerID_byLayerFlag(layerFlag);
		SetCanvasObject(layerID, sortingLayerID);
		EffectInfo effInfo = SetEffectInfo(layerID, Effects.SNSMenuBG, EffectState.Activating, null, 1);
		if (!(effInfo.m_GameObj == null))
		{
			goto IL_020f;
		}
		GameObject srcObject = GetPrefabSrcObject("Prefabs/CameraEffect/SNS_CHRBG_Eff");
		if (!(srcObject == null))
		{
			goto IL_01a5;
		}
		effInfo.m_isLoadingAsset = true;
		yield return MainLoadThing.instance.StartCoroutine(LoadPrefabSrcObject("Prefabs/CameraEffect/SNS_CHRBG_Eff"));
		effInfo.m_isLoadingAsset = false;
		AssetBundleObjectHandler assetBundleObjectHandler = GetAssetBundleObjectHandler("Prefabs/CameraEffect/SNS_CHRBG_Eff");
		if (assetBundleObjectHandler != null)
		{
			srcObject = assetBundleObjectHandler.GetLoadedAsset_ToGameObject();
			if (!(srcObject == null))
			{
				goto IL_01a5;
			}
		}
		goto IL_0284;
		IL_01a5:
		GameObject effObject = UnityEngine.Object.Instantiate(srcObject);
		if (effObject == null)
		{
			goto IL_0284;
		}
		RectTransform rt = effObject.transform as RectTransform;
		rt.SetParent(m_CanvasObj.transform, worldPositionStays: false);
		effInfo.m_GameObj = effObject;
		goto IL_020f;
		IL_0284:
		ReleaseEffectInfo(effInfo);
		yield break;
		IL_020f:
		effInfo.m_fpUpdate = UpdateProc_SNSMenuBG;
		effInfo.m_GameObj.SetActive(value: true);
		Animator animator = effInfo.m_GameObj.GetComponent<Animator>();
		effInfo.m_Params[0] = animator;
		animator.Play(GameDefine.UIAnimationState.appear.ToString());
	}

	public bool Deactivate_SNSMenuBG()
	{
		EffectInfo effectInfo = m_EffectInfos[19];
		if (effectInfo == null || effectInfo.m_State != EffectState.Enable)
		{
			return false;
		}
		if (m_CurCamera == null)
		{
			return false;
		}
		if (effectInfo.m_GameObj == null)
		{
			return false;
		}
		Animator animator = effectInfo.m_Params[0] as Animator;
		if (animator == null)
		{
			return false;
		}
		animator.Play(GameDefine.UIAnimationState.disappear.ToString());
		effectInfo.m_State = EffectState.Deactivating;
		return true;
	}

	private void UpdateProc_SNSMenuBG()
	{
		EffectInfo effectInfo = m_EffectInfos[19];
		if (effectInfo == null || effectInfo.m_State == EffectState.Disable || effectInfo.m_State == EffectState.Enable)
		{
			return;
		}
		Animator animator = effectInfo.m_Params[0] as Animator;
		if (animator == null)
		{
			return;
		}
		if (effectInfo.m_State == EffectState.Activating)
		{
			if (animator.GetCurrentAnimatorStateInfo(0).IsName(GameDefine.UIAnimationState.appear.ToString()))
			{
				effectInfo.m_State = EffectState.Enable;
			}
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(GameDefine.UIAnimationState.disappear.ToString()) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
		{
			ReleaseEffectInfo(effectInfo);
		}
	}
}
