using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AssetBundles;
using GameData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameGlobalUtil
{
	public const string STR_KEY_PUSH_MOT = "push";

	public const string STR_PUSH_MOT = "steam_push";

	public const float c_fAlmostZero = 0.0001f;

	public static Sprite m_sprLoadFromImgXls;

	public static GameObject m_goLoadAssetBundleObject;

	public static UnityEngine.Object s_loadedAsset;

	public static void AddEventTrigger(GameObject gameObj, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
	{
		EventTrigger eventTrigger = gameObj.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = gameObj.AddComponent<EventTrigger>();
		}
		AddEventTrigger(eventTrigger, triggerType, callback);
	}

	public static void AddEventTrigger(EventTrigger evtTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback, bool isCheckOldEvent = true)
	{
		EventTrigger.Entry entry = null;
		if (isCheckOldEvent)
		{
			EventTrigger.Entry entry2 = null;
			int count = evtTrigger.triggers.Count;
			for (int i = 0; i < count; i++)
			{
				entry2 = evtTrigger.triggers[i];
				if (entry2.eventID == triggerType)
				{
					entry = entry2;
					break;
				}
			}
		}
		if (entry == null)
		{
			entry = new EventTrigger.Entry();
			entry.eventID = triggerType;
			entry.callback = new EventTrigger.TriggerEvent();
			evtTrigger.triggers.Add(entry);
		}
		entry.callback.AddListener(callback.Invoke);
	}

	public static bool IsAlmostSame(float fLeft, float fRight)
	{
		return fLeft <= fRight + 0.0001f && fLeft >= fRight - 0.0001f;
	}

	public static Color HexToColor(int HexVal)
	{
		byte byValue = (byte)((HexVal >> 16) & 0xFF);
		byte byValue2 = (byte)((HexVal >> 8) & 0xFF);
		byte byValue3 = (byte)(HexVal & 0xFF);
		return new Color(ColorValueByteToFloat(byValue), ColorValueByteToFloat(byValue2), ColorValueByteToFloat(byValue3), 1f);
	}

	private static float ColorValueByteToFloat(byte byValue)
	{
		return (float)(int)byValue / 255f;
	}

	public static string GetPathStrFromImageXls(string strImgID, bool isThumbnailFirst = false, bool isOnFileBundle = true)
	{
		Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(strImgID);
		if (data_byKey == null)
		{
			return null;
		}
		return (!isThumbnailFirst || string.IsNullOrEmpty(data_byKey.m_strAssetPath_Thumbnail)) ? data_byKey.m_strAssetPath : data_byKey.m_strAssetPath_Thumbnail;
	}

	public static IEnumerator GetSprRequestFromImageXls(string strImgID, bool isThumbnailFirst = false, bool isOneFileBundle = true)
	{
		Xls.ImageFile xlsImageFileData = Xls.ImageFile.GetData_byKey(strImgID);
		if (xlsImageFileData != null)
		{
			string strAssetPath = ((!isThumbnailFirst || string.IsNullOrEmpty(xlsImageFileData.m_strAssetPath_Thumbnail)) ? xlsImageFileData.m_strAssetPath : xlsImageFileData.m_strAssetPath_Thumbnail);
			yield return GetSprRequestFromImgPath(strAssetPath, isOneFileBundle);
		}
	}

	public static IEnumerator GetSprRequestFromImgPath(string strPath, bool isOneFileBundle = true)
	{
		m_sprLoadFromImgXls = null;
		string strAssetBundleName = strPath.Clone() as string;
		string strAssetName = string.Empty;
		AssetBundleManager.SeperateAssetNameFromAssetBundleName(ref strAssetBundleName, out strAssetName, isOneFileBundle);
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(strAssetBundleName, strAssetName, typeof(UnityEngine.Object));
		if (request == null)
		{
			yield break;
		}
		while (!request.IsDone())
		{
			yield return null;
		}
		UnityEngine.Object loadedAsset = request.GetAsset<UnityEngine.Object>();
		if (!(loadedAsset == null))
		{
			if (loadedAsset is Sprite)
			{
				m_sprLoadFromImgXls = loadedAsset as Sprite;
			}
			else if (loadedAsset is Texture2D)
			{
				Texture2D texture2D = loadedAsset as Texture2D;
				Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
				m_sprLoadFromImgXls = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f));
			}
			AssetBundleManager.UnloadAssetBundle(strAssetBundleName);
		}
	}

	public static void UnloadAssetBundle(string strPath, bool isOneFileBundle = true)
	{
		if (strPath != null)
		{
			AssetBundleManager.UnloadAssetBundle(strPath, isOneFileBundle);
		}
	}

	public static IEnumerator InstantiateLoadAssetBundleObj(string strPath)
	{
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(strPath, typeof(GameObject));
		if (request != null)
		{
			while (!request.IsDone())
			{
				yield return null;
			}
			m_goLoadAssetBundleObject = UnityEngine.Object.Instantiate(request.GetAsset<GameObject>());
		}
	}

	public static IEnumerator LoadAssetBundleObj(string strPath)
	{
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(strPath, typeof(GameObject));
		if (request != null)
		{
			while (!request.IsDone())
			{
				yield return null;
			}
			m_goLoadAssetBundleObject = request.GetAsset<GameObject>();
			if (!(m_goLoadAssetBundleObject != null))
			{
			}
		}
	}

	public static IEnumerator LoadAssetAsync(string strPath, Type type = null)
	{
		s_loadedAsset = null;
		ResourceRequest request = Resources.LoadAsync(strPath, type);
		if (request != null)
		{
			while (!request.isDone)
			{
				yield return null;
			}
			s_loadedAsset = request.asset;
		}
	}

	public static Sprite GetSpriteFromImageXls(string strImgID, bool isThumbnailFirst = false)
	{
		Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(strImgID);
		if (data_byKey == null)
		{
			return null;
		}
		string path = ((!isThumbnailFirst || string.IsNullOrEmpty(data_byKey.m_strAssetPath_Thumbnail)) ? data_byKey.m_strAssetPath : data_byKey.m_strAssetPath_Thumbnail);
		return Resources.Load<Sprite>(path);
	}

	public static Sprite GetThumbnailSpriteFromImageXls(string strImgID)
	{
		Xls.ImageFile data_byKey = Xls.ImageFile.GetData_byKey(strImgID);
		if (data_byKey == null)
		{
			return null;
		}
		return Resources.Load<Sprite>(data_byKey.m_strAssetPath_Thumbnail);
	}

	public static Sprite GetSpriteFromCollImageXls(string strCollImgID, bool isThumbnailFirst = false)
	{
		Xls.CollImages data_byKey = Xls.CollImages.GetData_byKey(strCollImgID);
		if (data_byKey == null)
		{
			return null;
		}
		return GetSpriteFromImageXls(data_byKey.m_strIDImg, isThumbnailFirst);
	}

	public static bool HasStateInAnimator(Animator animator, GameDefine.UIAnimationState eState, int layer = 0)
	{
		return HasStateInAnimator(animator, eState.ToString(), layer);
	}

	public static bool HasStateInAnimator(Animator animator, string stateName, int layer = 0)
	{
		return animator != null && animator.HasState(layer, Animator.StringToHash(stateName));
	}

	public static GameDefine.UIAnimationState GetAnimationState(Animator animator)
	{
		GameDefine.UIAnimationState result = GameDefine.UIAnimationState.none;
		if (animator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(GameDefine.cStrAnimStateAppear))
			{
				result = GameDefine.UIAnimationState.appear;
			}
			else if (currentAnimatorStateInfo.IsName(GameDefine.cStrAnimStateIdle))
			{
				result = GameDefine.UIAnimationState.idle;
			}
			else if (currentAnimatorStateInfo.IsName(GameDefine.cStrAnimStateIdle2))
			{
				result = GameDefine.UIAnimationState.idle2;
			}
			else if (currentAnimatorStateInfo.IsName(GameDefine.cStrAnimStateDisappear))
			{
				result = GameDefine.UIAnimationState.disappear;
			}
		}
		return result;
	}

	public static void PlayAllChildrenUIAnimation(GameObject goParent, GameDefine.UIAnimationState eState, Animator animCheck, ref GameDefine.eAnimChangeState eChgState)
	{
		Animator[] componentsInChildren = goParent.GetComponentsInChildren<Animator>();
		int num = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (componentsInChildren[i].gameObject.activeInHierarchy)
			{
				if (componentsInChildren[i] == animCheck)
				{
					PlayUIAnimation(componentsInChildren[i], eState, ref eChgState);
				}
				else
				{
					PlayUIAnimation(componentsInChildren[i], eState);
				}
			}
		}
	}

	public static void PlayUIAnimation(Animator animator, GameDefine.UIAnimationState eState, ref GameDefine.eAnimChangeState eChgState)
	{
		eChgState = GameDefine.eAnimChangeState.changing;
		PlayUIAnimation(animator, eState);
	}

	public static void PlayUIAnimation(Animator animator, GameDefine.UIAnimationState eState)
	{
		if (!(animator == null) && animator.gameObject.activeInHierarchy)
		{
			string strMot = eState.ToString();
			PlayUIAnimation(animator, strMot);
		}
	}

	public static void PlayUIAnimation(Animator animator, string strMot, ref GameDefine.eAnimChangeState eChgState)
	{
		eChgState = GameDefine.eAnimChangeState.changing;
		PlayUIAnimation(animator, strMot);
	}

	public static void PlayUIAnimation(Animator animator, string strMot)
	{
		if (animator.gameObject.activeInHierarchy && HasStateInAnimator(animator, strMot) && !animator.GetCurrentAnimatorStateInfo(0).IsName(strMot))
		{
			animator.Play(strMot);
		}
	}

	public static Animator PlayUIAnimation_WithChidren(GameObject obj, string strMot, ref GameDefine.eAnimChangeState eChgState, float speedRate = 1f)
	{
		Animator animator = PlayUIAnimation_WithChidren(obj, strMot, speedRate);
		if (animator != null)
		{
			eChgState = GameDefine.eAnimChangeState.changing;
		}
		return animator;
	}

	public static Animator PlayUIAnimation_WithChidren(GameObject obj, string strMot, float speedRate = 1f)
	{
		if (obj == null)
		{
			return null;
		}
		Animator[] componentsInChildren = obj.GetComponentsInChildren<Animator>();
		if (componentsInChildren == null || componentsInChildren.Length <= 0)
		{
			return null;
		}
		int num = componentsInChildren.Length;
		Animator animator = null;
		Animator animator2 = null;
		for (int i = 0; i < num; i++)
		{
			animator = componentsInChildren[i];
			if (!(animator == null) && animator.gameObject.activeInHierarchy)
			{
				PlayUIAnimation(animator, strMot);
				animator.speed = speedRate;
				if (animator.enabled && animator2 == null)
				{
					animator2 = animator;
				}
			}
		}
		return animator2;
	}

	public static bool CheckPlayEndUIAnimation(Animator animator, GameDefine.UIAnimationState eState, ref GameDefine.eAnimChangeState eChgState)
	{
		return CheckPlayEndUIAnimation(animator, eState.ToString(), ref eChgState);
	}

	public static bool CheckPlayEndUIAnimation(Animator animator, string strMot, ref GameDefine.eAnimChangeState eChgState)
	{
		if (eChgState == GameDefine.eAnimChangeState.changing)
		{
			eChgState = GameDefine.eAnimChangeState.changed;
			return false;
		}
		bool flag = CheckPlayEndUIAnimation(animator, strMot);
		if (flag)
		{
			eChgState = GameDefine.eAnimChangeState.play_end;
		}
		return flag;
	}

	public static bool CheckPlayEndUIAnimation(Animator animator, GameDefine.UIAnimationState eState)
	{
		if (animator == null)
		{
			return true;
		}
		string strMot = eState.ToString();
		return CheckPlayEndUIAnimation(animator, strMot);
	}

	public static bool CheckPlayEndUIAnimation(Animator animator, string strMot)
	{
		if (animator == null)
		{
			return true;
		}
		if (!animator.enabled || !animator.gameObject.activeInHierarchy)
		{
			return true;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!currentAnimatorStateInfo.IsName(strMot))
		{
			return true;
		}
		if (currentAnimatorStateInfo.normalizedTime < 1f)
		{
			return false;
		}
		return true;
	}

	public static bool IsCheckPlayAnimation(Animator animator, string strMot)
	{
		if (animator == null)
		{
			return false;
		}
		if (animator.GetCurrentAnimatorStateInfo(0).IsName(strMot))
		{
			return true;
		}
		return false;
	}

	public static bool IsCheckPlayUIAnimation(Animator animator, string strMot)
	{
		bool result = false;
		if (animator == null)
		{
			return result;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(strMot))
		{
			if (currentAnimatorStateInfo.loop)
			{
				result = true;
			}
			else if (currentAnimatorStateInfo.normalizedTime > 0f && currentAnimatorStateInfo.normalizedTime < 1f)
			{
				result = true;
			}
		}
		return result;
	}

	public static float GetPlayTimeAnimator(Animator animator, string strMot)
	{
		float result = 0f;
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.IsName(strMot))
		{
			result = currentAnimatorStateInfo.normalizedTime;
		}
		return result;
	}

	public static CommonButtonGuide GetCommonButtonGuide()
	{
		return (!(MainLoadThing.instance != null)) ? null : MainLoadThing.instance.commonButtonGuide;
	}

	public static AudioManager GetAudioManager()
	{
		string name = "AudioManager";
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<AudioManager>();
	}

	public static string GetCurrentGameTimeString(string format = null)
	{
		int iH = 0;
		int iM = 0;
		GameSwitch.GetInstance().GetGameTime(ref iH, ref iM);
		bool flag = iH >= 12;
		if (flag)
		{
			iH -= 12;
		}
		string xlsProgramText = GetXlsProgramText((!flag) ? "AM" : "PM");
		return (!string.IsNullOrEmpty(format)) ? string.Format(format, xlsProgramText, iH, iM) : $"{xlsProgramText} {iH:D2}:{iM:D2}";
	}

	public static string GetGameTimeString(float gameTime, string format = null)
	{
		int iH = 0;
		int iM = 0;
		GameSwitch.GetInstance().GetHMByFloatTime(gameTime, ref iH, ref iM);
		bool flag = iH >= 12;
		if (flag)
		{
			iH -= 12;
		}
		string xlsProgramText = GetXlsProgramText((!flag) ? "AM" : "PM");
		return (!string.IsNullOrEmpty(format)) ? string.Format(format, xlsProgramText, iH, iM) : $"{xlsProgramText} {iH:D2}:{iM:D2}";
	}

	public static IEnumerator GetSWatchBGImageSpriteRequest()
	{
		m_sprLoadFromImgXls = null;
		int switchValue = GameSwitch.GetInstance().GetSWBackImage();
		if (switchValue < 0)
		{
			yield break;
		}
		Xls.CollImages xlsColImage = Xls.CollImages.GetData_bySwitchIdx(switchValue);
		if (xlsColImage != null)
		{
			Xls.ImageFile xlsImageFile = Xls.ImageFile.GetData_byKey(xlsColImage.m_strIDImg);
			if (xlsImageFile != null)
			{
				yield return GetSprRequestFromImgPath(xlsImageFile.m_strAssetPath);
			}
		}
	}

	public static string[] GetSeparateText(string strText, char[] chSeparate, bool isAddNullChar = true)
	{
		char[] array = strText.ToCharArray();
		int num = array.Length;
		char[] array2 = new char[30];
		int num2 = chSeparate.Length;
		int length = 0;
		string text = null;
		Queue<string> queue = new Queue<string>();
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			flag = false;
			for (int j = 0; j < num2; j++)
			{
				if (!flag && array[i] == chSeparate[j])
				{
					flag = true;
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				array2[length++] = array[i];
				if (i == num - 1)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (isAddNullChar)
				{
					array2[length++] = '\0';
				}
				text = new string(array2);
				text = text.Substring(0, length);
				queue.Enqueue(text);
				BitCalc.InitArray(array2);
				length = 0;
			}
		}
		string[] array3 = null;
		int count = queue.Count;
		if (count > 0)
		{
			array3 = new string[count];
			for (int k = 0; k < count; k++)
			{
				array3[k] = queue.Dequeue();
			}
		}
		return array3;
	}

	public static int[] GetIntSeparateText(string strText, char chSeparate)
	{
		char[] array = strText.ToCharArray();
		int num = array.Length;
		char[] array2 = new char[30];
		int num2 = 0;
		Queue<int> queue = new Queue<int>();
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			flag = false;
			if (!flag && array[i] == chSeparate)
			{
				flag = true;
			}
			if (!flag)
			{
				if ((array[i] >= '0' && array[i] <= '9') || array[i] == '-')
				{
					array2[num2++] = array[i];
				}
				if (i == num - 1)
				{
					flag = true;
				}
			}
			if (flag)
			{
				array2[num2++] = '\0';
				queue.Enqueue(int.Parse(new string(array2)));
				BitCalc.InitArray(array2);
				num2 = 0;
			}
		}
		int[] array3 = null;
		int count = queue.Count;
		if (count > 0)
		{
			array3 = new int[count];
			for (int j = 0; j < count; j++)
			{
				array3[j] = queue.Dequeue();
			}
		}
		return array3;
	}

	public static string GetXlsProgramText(string xlsDataName)
	{
		if (string.IsNullOrEmpty(xlsDataName))
		{
			return string.Empty;
		}
		Xls.ProgramText data_byKey = Xls.ProgramText.GetData_byKey(xlsDataName);
		return (data_byKey == null) ? string.Empty : data_byKey.m_strTxt;
	}

	public static string GetXlsProgramGlobalText(string xlsDataName)
	{
		if (string.IsNullOrEmpty(xlsDataName))
		{
			return string.Empty;
		}
		Xls.ProgramGlobalText data_byKey = Xls.ProgramGlobalText.GetData_byKey(xlsDataName);
		return (data_byKey == null) ? string.Empty : data_byKey.m_strTxt;
	}

	public static int GetXlsScriptKeyValue(string strEventKey)
	{
		int result = -1;
		if (strEventKey != null)
		{
			Xls.ScriptKeyValue data_byKey = Xls.ScriptKeyValue.GetData_byKey(strEventKey);
			if (data_byKey != null)
			{
				result = data_byKey.m_iValue;
			}
		}
		return result;
	}

	public static string GetXlsProgramDefineStr(string strKey)
	{
		string result = null;
		if (strKey != null)
		{
			Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey(strKey);
			if (data_byKey != null)
			{
				result = data_byKey.m_strTxt;
			}
		}
		return result;
	}

	public static float GetXlsProgramDefineStrToFloat(string strKey)
	{
		float result = 0f;
		string xlsProgramDefineStr = GetXlsProgramDefineStr(strKey);
		if (xlsProgramDefineStr != null)
		{
			result = float.Parse(xlsProgramDefineStr, CultureInfo.InvariantCulture);
		}
		return result;
	}

	public static string GetXlsTextData(string strKey)
	{
		string result = string.Empty;
		if (strKey != null)
		{
			Xls.TextData data_byKey = Xls.TextData.GetData_byKey(strKey);
			if (data_byKey != null)
			{
				result = data_byKey.m_strTxt;
			}
		}
		return result;
	}

	public static bool ConvertHexStrToRGB(string strHexColor, ref float[] fRGB)
	{
		if (string.IsNullOrEmpty(strHexColor) || strHexColor.Length < 9)
		{
			return false;
		}
		strHexColor = strHexColor.Substring(1);
		int num = 3;
		string[] array = new string[num];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			array[i] = strHexColor.Substring(num2, 2);
			fRGB[i] = (float)Convert.ToInt32(array[i], 16) / 255f;
			num2 += 2;
		}
		return true;
	}

	public static GamePadInput.StickDir GetMouseWheelAxis()
	{
		GamePadInput.StickDir result = GamePadInput.StickDir.None;
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			result = ((axis < 0f) ? GamePadInput.StickDir.Right : GamePadInput.StickDir.Left);
		}
		return result;
	}
}
