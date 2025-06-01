using System;
using System.Collections;
using UnityEngine;

public class GameDefine
{
	public delegate void EventProc(object sender, object arg);

	public delegate IEnumerator ienuEventProc(object sender, object arg);

	public enum UIAnimationState
	{
		none,
		idle,
		appear,
		disappear,
		idle2
	}

	public enum eAnimChangeState
	{
		none,
		changing,
		changed,
		play_end
	}

	public enum CollectionSoundType
	{
		All = -1,
		BGM,
		Record,
		PhoneTalk,
		RingSound,
		Etc,
		HideInCollectionMenu
	}

	public const int CHR_LAYER_COUNT = 5;

	public const string CHR_LAYER_NAME_0 = "CHR_0";

	public const string CHR_LAYER_NAME_1 = "CHR_1";

	public const string CHR_LAYER_NAME_2 = "CHR_2";

	public const string CHR_LAYER_NAME_3 = "CHR_3";

	public const string CHR_LAYER_NAME_4 = "CHR_4";

	public const string CHR_BOUND_LAYER_NAME = "CHR_BOUND";

	public const string BG_BOUND_LAYER_NAME = "BG";

	public const string UI_SORTING_LAYER_NAME_0 = "LAYER_CAMERA_UI_0";

	public const string UI_SORTING_LAYER_NAME_1 = "LAYER_CAMERA_UI_1";

	public const string UI_SORTING_LAYER_NAME_2 = "LAYER_CAMERA_UI_2";

	public const string UI_SORTING_LAYER_NAME_3 = "LAYER_CAMERA_UI_3";

	public const string UI_SORTING_LAYER_NAME_4 = "LAYER_CAMERA_UI_4";

	public const string UI_SORTING_LAYER_NAME_5 = "LAYER_CAMERA_UI_5";

	public static readonly string cStrAnimStateAppear = UIAnimationState.appear.ToString();

	public static readonly string cStrAnimStateIdle = UIAnimationState.idle.ToString();

	public static readonly string cStrAnimStateIdle2 = UIAnimationState.idle2.ToString();

	public static readonly string cStrAnimStateDisappear = UIAnimationState.disappear.ToString();

	private static string[] s_UIAnimationStateNames = Enum.GetNames(typeof(UIAnimationState));

	public const int c_iMaxPhaseLevel = 100;

	public const int c_iPopupUICanvasSortOrder = 100;

	public const float c_fBaseCanvasWidth = 1920f;

	public const float c_fBaseCanvasHeight = 1080f;

	public const string RESOURCES_CONTROLLER_BUTTONS_TYPE1 = "Image/ControllerButtons_1";

	public const string RESOURCES_CONTROLLER_BUTTONS_TYPE2 = "Image/ControllerButtons_2";

	public const string RESOURCES_CONTROLLER_BUTTONS_XBOX_TYPE1 = "Image/ControllerButtons_1_Xpad";

	public const string RESOURCES_CONTROLLER_BUTTONS_XBOX_TYPE2 = "Image/ControllerButtons_2_Xpad";

	public const string RESOURCES_CONTROLLER_BUTTONS_NS_TYPE1 = "Image/ControllerButtons_1_NSwitch";

	public const string RESOURCES_CONTROLLER_BUTTONS_NS_TYPE2 = "Image/ControllerButtons_2_NSwitch";

	public const string RESOURCES_CONTROLLER_BUTTONS_KEYBOARD_TYPE1 = "Image/KeyboardKeys_1";

	public const string RESOURCES_CONTROLLER_BUTTONS_KEYBOARD_TYPE2 = "Image/KeyboardKeys_2";

	public const string RESOURCES_CONTROLLER_BUTTONS_PRESSED = "_pressed";

	public const int MAIN_ACTOR_INDEX = 0;

	public static int GetLayerID_byCharLayer(int iCharLayer)
	{
		return iCharLayer switch
		{
			0 => LayerMask.NameToLayer("CHR_0"), 
			1 => LayerMask.NameToLayer("CHR_1"), 
			2 => LayerMask.NameToLayer("CHR_2"), 
			3 => LayerMask.NameToLayer("CHR_3"), 
			4 => LayerMask.NameToLayer("CHR_4"), 
			5 => LayerMask.NameToLayer("CHR_BOUND"), 
			_ => 0, 
		};
	}

	public static int GetSortLayerID_byCharLayer(int iCharLayer)
	{
		return iCharLayer switch
		{
			0 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_0"), 
			1 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_1"), 
			2 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_2"), 
			3 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_3"), 
			4 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_4"), 
			5 => SortingLayer.GetLayerValueFromName("LAYER_CAMERA_UI_5"), 
			_ => 0, 
		};
	}

	public static string GetAnimationStateName(UIAnimationState aniState)
	{
		return s_UIAnimationStateNames[(int)aniState];
	}
}
