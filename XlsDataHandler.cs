using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class XlsDataHandler
{
	private class XlsClassInfo
	{
		public string m_Name;

		public long m_DataOffset;

		public int m_DataCount;
	}

	private class LanguageDataInfo
	{
		public string m_ClassName;

		public long m_DataOffset;

		public int m_DataCount;

		public string[] m_propertyNames;
	}

	private class LanguageHeaderInfo
	{
		public long m_dataChunckOffset;

		public LanguageDataInfo[] m_classHeaderInfos;
	}

	private static string s_AssetName = string.Empty;

	private static TextAsset s_Asset = null;

	private static MemoryStream s_DataStream = null;

	private static long s_DataStreamOffset = 0L;

	private static bool s_isInitailized = false;

	private static AssetBundleObjectHandler s_assetBundleObjHandler = null;

	private static SortedDictionary<string, XlsClassInfo> s_InfoMap = new SortedDictionary<string, XlsClassInfo>();

	private static SortedDictionary<string, List<string>> s_CollectionImageDestListDatas = new SortedDictionary<string, List<string>>();

	private static Dictionary<SystemLanguage, LanguageHeaderInfo> s_LanguageHeaderInfos = new Dictionary<SystemLanguage, LanguageHeaderInfo>();

	public static bool isInitailized => s_isInitailized;

	public static IEnumerator Init(string assetName, bool isLoadAllData = true, bool isForceLoad = false, MonoBehaviour parentBehaviour = null)
	{
		if (s_isInitailized && !isForceLoad)
		{
			yield break;
		}
		s_InfoMap.Clear();
		if (parentBehaviour == null)
		{
			yield break;
		}
		yield return parentBehaviour.StartCoroutine(LoadAsset(assetName, parentBehaviour));
		if (s_assetBundleObjHandler == null || s_assetBundleObjHandler.loadedAssetBundleObject == null || s_DataStream == null)
		{
			yield break;
		}
		s_AssetName = assetName;
		byte[] byteBuffer = new byte[1024];
		s_DataStream.Read(byteBuffer, 0, 8);
		s_DataStreamOffset = BitConverter.ToInt64(byteBuffer, 0);
		s_DataStream.Read(byteBuffer, 0, 4);
		int classInfoCnt = BitConverter.ToInt32(byteBuffer, 0);
		for (int i = 0; i < classInfoCnt; i++)
		{
			s_DataStream.Read(byteBuffer, 0, 4);
			int num = BitConverter.ToInt32(byteBuffer, 0);
			if (num > 0)
			{
				XlsClassInfo xlsClassInfo = new XlsClassInfo();
				s_DataStream.Read(byteBuffer, 0, num);
				xlsClassInfo.m_Name = Encoding.Unicode.GetString(byteBuffer, 0, num);
				s_DataStream.Read(byteBuffer, 0, 8);
				xlsClassInfo.m_DataOffset = BitConverter.ToInt64(byteBuffer, 0);
				xlsClassInfo.m_DataOffset += s_DataStreamOffset;
				s_InfoMap.Add(xlsClassInfo.m_Name, xlsClassInfo);
			}
		}
		Xls.Stream = s_DataStream;
		Xls.PosDataInStream = s_DataStreamOffset;
		if (isLoadAllData)
		{
			yield return parentBehaviour.StartCoroutine(LoadAllData(parentBehaviour));
		}
		InitCollectionImageDestListDatas();
		s_isInitailized = true;
	}

	private static IEnumerator LoadAsset(string assetName, MonoBehaviour parentBehaviour)
	{
		if (parentBehaviour == null)
		{
			yield break;
		}
		s_assetBundleObjHandler = new AssetBundleObjectHandler(assetName);
		yield return parentBehaviour.StartCoroutine(s_assetBundleObjHandler.LoadAssetBundle());
		if (!(s_assetBundleObjHandler.loadedAssetBundleObject == null))
		{
			s_Asset = s_assetBundleObjHandler.GetLoadedAsset<TextAsset>();
			if (!(s_Asset == null))
			{
				s_DataStream = new MemoryStream(s_Asset.bytes);
			}
		}
	}

	private static void UnloadAsset()
	{
		if (s_DataStream != null)
		{
			s_DataStream.Close();
			s_DataStream = null;
		}
		if (s_Asset != null)
		{
			if (s_assetBundleObjHandler != null)
			{
				s_assetBundleObjHandler.UnloadAssetBundle();
				s_assetBundleObjHandler = null;
			}
			s_Asset = null;
		}
	}

	public static void Release()
	{
		if (s_InfoMap != null)
		{
			s_InfoMap.Clear();
		}
		s_InfoMap = null;
		if (s_CollectionImageDestListDatas != null)
		{
			s_CollectionImageDestListDatas.Clear();
		}
		s_CollectionImageDestListDatas = null;
		if (s_LanguageHeaderInfos != null)
		{
			s_LanguageHeaderInfos.Clear();
		}
		s_LanguageHeaderInfos = null;
		UnloadAsset();
	}

	public static IEnumerator LoadAllData(MonoBehaviour parentBehaviour)
	{
		if (parentBehaviour == null)
		{
			yield break;
		}
		XlsClassInfo info = null;
		Xls.LoadFP loadFunc = null;
		foreach (KeyValuePair<string, XlsClassInfo> item in s_InfoMap)
		{
			info = item.Value;
			loadFunc = Xls.GetLoadFunc(info.m_Name);
			if (loadFunc != null)
			{
				s_DataStream.Position = info.m_DataOffset;
				yield return parentBehaviour.StartCoroutine(loadFunc(s_DataStream));
				yield return null;
			}
		}
	}

	private static void InitCollectionImageDestListDatas()
	{
		s_CollectionImageDestListDatas.Clear();
		int count = Xls.ColImageDescListData.datas.Count;
		for (int i = 0; i < count; i++)
		{
			Xls.ColImageDescListData colImageDescListData = Xls.ColImageDescListData.datas[i];
			string strColImageDescID = colImageDescListData.m_strColImageDescID;
			if (s_CollectionImageDestListDatas.ContainsKey(strColImageDescID))
			{
				s_CollectionImageDestListDatas[strColImageDescID].Add(colImageDescListData.m_strColImageDescText);
				continue;
			}
			List<string> list = new List<string>();
			list.Add(colImageDescListData.m_strColImageDescText);
			s_CollectionImageDestListDatas.Add(strColImageDescID, list);
		}
	}

	public static List<string> GetCollectionImageDestList(string key)
	{
		List<string> value = null;
		return (!s_CollectionImageDestListDatas.TryGetValue(key, out value)) ? null : value;
	}

	public static IEnumerator SetCurrentLanguage(SystemLanguage language, bool clearPrevLanguageText = false, GameDefine.EventProc eventFinished = null)
	{
		yield return MainLoadThing.instance.StartCoroutine(FontManager.LoadFontAssetBundle(language));
		yield return null;
		Xls.XmlDataBase.Language = language;
		eventFinished?.Invoke(null, null);
	}

	private static IEnumerator LoadLanguagePack(SystemLanguage language)
	{
		string assetName = $"{s_AssetName}_{language.ToString()}".ToLower();
		AssetBundleObjectHandler assetBundleHdr = new AssetBundleObjectHandler(assetName);
		yield return MainLoadThing.instance.StartCoroutine(assetBundleHdr.LoadAssetBundle());
		if (assetBundleHdr.loadedAssetBundleObject == null)
		{
			yield break;
		}
		TextAsset tAsset = assetBundleHdr.GetLoadedAsset<TextAsset>();
		if (tAsset == null)
		{
			yield break;
		}
		MemoryStream stream = new MemoryStream(tAsset.bytes);
		LanguageHeaderInfo langHeaderInfo = ReadLanguageDataHeader(stream);
		if (langHeaderInfo == null || langHeaderInfo.m_classHeaderInfos == null || langHeaderInfo.m_classHeaderInfos.Length <= 0)
		{
			yield break;
		}
		byte[] byteBufferInt = new byte[4];
		byte[] byteBufferlong = new byte[8];
		byte[] byteBufferString = new byte[1024];
		int strLength = 0;
		int readedByteCnt = 0;
		LanguageDataInfo[] classHeaderInfos = langHeaderInfo.m_classHeaderInfos;
		foreach (LanguageDataInfo langDataInfo in classHeaderInfos)
		{
			if (langDataInfo.m_propertyNames == null || langDataInfo.m_propertyNames.Length <= 0)
			{
				continue;
			}
			Type classType = Type.GetType("Xls+" + langDataInfo.m_ClassName);
			if (classType == null)
			{
				continue;
			}
			PropertyInfo propertyInfo_DataObjs = classType.GetProperty("datas");
			if (propertyInfo_DataObjs == null || !(propertyInfo_DataObjs.GetValue(null, null) is IList dataList))
			{
				continue;
			}
			if (dataList.Count != langDataInfo.m_DataCount)
			{
				yield break;
			}
			int propertyCount = langDataInfo.m_propertyNames.Length;
			PropertyInfo[] propertyInfos = new PropertyInfo[propertyCount];
			for (int j = 0; j < langDataInfo.m_propertyNames.Length; j++)
			{
				propertyInfos[j] = classType.GetProperty(langDataInfo.m_propertyNames[j]);
			}
			stream.Position = langHeaderInfo.m_dataChunckOffset + langDataInfo.m_DataOffset;
			object dataObject = null;
			string value = null;
			for (int k = 0; k < langDataInfo.m_DataCount; k++)
			{
				dataObject = dataList[k];
				for (int l = 0; l < propertyCount; l++)
				{
					readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
					strLength = BitConverter.ToInt32(byteBufferInt, 0);
					if (strLength > byteBufferString.Length)
					{
						byteBufferString = new byte[strLength];
					}
					readedByteCnt = stream.Read(byteBufferString, 0, strLength);
					value = Encoding.Unicode.GetString(byteBufferString, 0, strLength);
					propertyInfos[l].SetValue(dataObject, value, null);
				}
			}
			yield return null;
		}
		stream.Close();
		assetBundleHdr.UnloadAssetBundle();
	}

	private static LanguageHeaderInfo ReadLanguageDataHeader(Stream stream)
	{
		byte[] array = new byte[4];
		byte[] array2 = new byte[8];
		byte[] array3 = new byte[1024];
		int num = 0;
		int num2 = stream.Read(array, 0, array.Length);
		int num3 = BitConverter.ToInt32(array, 0);
		num2 = stream.Read(array, 0, array.Length);
		int num4 = BitConverter.ToInt32(array, 0);
		if (num4 <= 0)
		{
			return null;
		}
		LanguageHeaderInfo languageHeaderInfo = new LanguageHeaderInfo();
		languageHeaderInfo.m_classHeaderInfos = new LanguageDataInfo[num4];
		for (int i = 0; i < num4; i++)
		{
			LanguageDataInfo languageDataInfo = new LanguageDataInfo();
			languageHeaderInfo.m_classHeaderInfos[i] = languageDataInfo;
			num2 = stream.Read(array, 0, array.Length);
			num = BitConverter.ToInt32(array, 0);
			if (num > array3.Length)
			{
				array3 = new byte[num];
			}
			num2 = stream.Read(array3, 0, num);
			languageDataInfo.m_ClassName = Encoding.Unicode.GetString(array3, 0, num);
			num2 = stream.Read(array, 0, array.Length);
			int num5 = BitConverter.ToInt32(array, 0);
			if (num5 >= 0)
			{
				languageDataInfo.m_propertyNames = new string[num5];
				for (int j = 0; j < num5; j++)
				{
					num2 = stream.Read(array, 0, array.Length);
					num = BitConverter.ToInt32(array, 0);
					if (num > array3.Length)
					{
						array3 = new byte[num];
					}
					num2 = stream.Read(array3, 0, num);
					languageDataInfo.m_propertyNames[j] = Encoding.Unicode.GetString(array3, 0, num);
				}
			}
			num2 = stream.Read(array2, 0, array2.Length);
			languageDataInfo.m_DataOffset = BitConverter.ToInt64(array2, 0);
			num2 = stream.Read(array, 0, array.Length);
			languageDataInfo.m_DataCount = BitConverter.ToInt32(array, 0);
		}
		languageHeaderInfo.m_dataChunckOffset = num3;
		return languageHeaderInfo;
	}

	private static IEnumerator ClearLanguageTextToEmpty(LanguageHeaderInfo langHeaderInfo)
	{
		string emptyString = string.Empty;
		LanguageDataInfo[] classHeaderInfos = langHeaderInfo.m_classHeaderInfos;
		foreach (LanguageDataInfo langDataInfo in classHeaderInfos)
		{
			if (langDataInfo.m_propertyNames == null || langDataInfo.m_propertyNames.Length <= 0)
			{
				continue;
			}
			Type classType = Type.GetType("Xls+" + langDataInfo.m_ClassName);
			if (classType == null)
			{
				continue;
			}
			PropertyInfo propertyInfo_DataObjs = classType.GetProperty("datas");
			if (propertyInfo_DataObjs == null || !(propertyInfo_DataObjs.GetValue(null, null) is IList dataList))
			{
				continue;
			}
			int propertyCount = langDataInfo.m_propertyNames.Length;
			PropertyInfo[] propertyInfos = new PropertyInfo[propertyCount];
			for (int j = 0; j < langDataInfo.m_propertyNames.Length; j++)
			{
				propertyInfos[j] = classType.GetProperty(langDataInfo.m_propertyNames[j]);
			}
			object dataObject = null;
			for (int k = 0; k < dataList.Count; k++)
			{
				dataObject = dataList[k];
				for (int l = 0; l < propertyCount; l++)
				{
					propertyInfos[l].SetValue(dataObject, emptyString, null);
				}
			}
			yield return null;
		}
	}
}
