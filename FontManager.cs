using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;
using UnityEngine.UI;

public class FontManager
{
	private class FontGroupInfo
	{
		public SystemLanguage m_language = SystemLanguage.Unknown;

		public string m_bundleName = string.Empty;

		public string m_fontNameNormal = string.Empty;

		public string m_fontNameBold = string.Empty;

		public Font m_fontNormal;

		public Font m_fontBold;

		public AssetBundle m_assetBundle;
	}

	private enum FontType
	{
		Unknown,
		Normal,
		Bold
	}

	private static Dictionary<SystemLanguage, FontGroupInfo> s_fontGroupInfos = new Dictionary<SystemLanguage, FontGroupInfo>();

	private static readonly SystemLanguage[] c_languageOrder = new SystemLanguage[5]
	{
		SystemLanguage.Korean,
		SystemLanguage.English,
		SystemLanguage.Japanese,
		SystemLanguage.ChineseSimplified,
		SystemLanguage.ChineseTraditional
	};

	private static readonly string[] c_fontGroupDataKeys = new string[5] { "FONT_GROUP_KR", "FONT_GROUP_EN", "FONT_GROUP_JP", "FONT_GROUP_SC", "FONT_GROUP_TC" };

	private static readonly char[] c_seperatores = new char[1] { ',' };

	private static string[] s_fontNormalNames = null;

	private static string[] s_fontBoldNames = null;

	private static UnityEngine.Object s_templaryLoadedAsset = null;

	public static void Init()
	{
		Clear();
		int num = c_languageOrder.Length;
		s_fontNormalNames = new string[num];
		s_fontBoldNames = new string[num];
		for (int i = 0; i < num; i++)
		{
			string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr(c_fontGroupDataKeys[i]);
			SystemLanguage systemLanguage = c_languageOrder[i];
			FontGroupInfo fontGroupInfo = CreateFontGroupInfo(xlsProgramDefineStr);
			if (fontGroupInfo != null)
			{
				fontGroupInfo.m_language = systemLanguage;
				s_fontGroupInfos.Add(systemLanguage, fontGroupInfo);
				s_fontNormalNames[i] = fontGroupInfo.m_fontNameNormal;
				s_fontBoldNames[i] = fontGroupInfo.m_fontNameBold;
			}
		}
	}

	public static void Clear()
	{
		Dictionary<SystemLanguage, FontGroupInfo>.Enumerator enumerator = s_fontGroupInfos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			UnloadFontAsset(enumerator.Current.Value);
		}
		s_fontGroupInfos.Clear();
	}

	private static FontGroupInfo CreateFontGroupInfo(string fontInfo)
	{
		string bundleName = string.Empty;
		string fontNameNormal = string.Empty;
		string fontNameBold = string.Empty;
		if (!ParseFontInfo(fontInfo, ref bundleName, ref fontNameNormal, ref fontNameBold))
		{
			return null;
		}
		FontGroupInfo fontGroupInfo = new FontGroupInfo();
		fontGroupInfo.m_bundleName = bundleName;
		fontGroupInfo.m_fontNameNormal = fontNameNormal;
		fontGroupInfo.m_fontNameBold = fontNameBold;
		return fontGroupInfo;
	}

	private static bool ParseFontInfo(string fontInfo, ref string bundleName, ref string fontNameNormal, ref string fontNameBold)
	{
		bundleName = string.Empty;
		fontNameNormal = string.Empty;
		fontNameBold = string.Empty;
		if (string.IsNullOrEmpty(fontInfo))
		{
			return false;
		}
		string[] array = fontInfo.Split(c_seperatores, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || array.Length < 3)
		{
			return false;
		}
		bundleName = $"font/{array[0].ToLower()}";
		fontNameNormal = array[1].Trim();
		fontNameBold = array[2].Trim();
		return true;
	}

	public static IEnumerator LoadFontAssetBundle(SystemLanguage language, GameDefine.EventProc fpFinished = null)
	{
		bool succeeded = false;
		if (s_fontGroupInfos.ContainsKey(language))
		{
			FontGroupInfo fontGroupInfo = s_fontGroupInfos[language];
			if (fontGroupInfo != null)
			{
				if (fontGroupInfo.m_assetBundle != null)
				{
					succeeded = true;
				}
				else
				{
					string assetBundleName = fontGroupInfo.m_bundleName;
					AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, string.Empty, typeof(UnityEngine.Object));
					if (request != null)
					{
						while (!request.IsDone())
						{
							yield return null;
						}
						yield return null;
						yield return null;
						string errorMsg = string.Empty;
						LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out errorMsg);
						if (loadedAssetBundle != null && !(loadedAssetBundle.m_AssetBundle == null))
						{
							fontGroupInfo.m_assetBundle = loadedAssetBundle.m_AssetBundle;
							yield return MainLoadThing.instance.StartCoroutine(LoadFontAsset(fontGroupInfo.m_assetBundle, fontGroupInfo.m_fontNameNormal));
							if (s_templaryLoadedAsset != null && s_templaryLoadedAsset is Font)
							{
								fontGroupInfo.m_fontNormal = s_templaryLoadedAsset as Font;
							}
							if (!string.IsNullOrEmpty(fontGroupInfo.m_fontNameBold))
							{
								yield return MainLoadThing.instance.StartCoroutine(LoadFontAsset(fontGroupInfo.m_assetBundle, fontGroupInfo.m_fontNameBold));
								if (s_templaryLoadedAsset != null && s_templaryLoadedAsset is Font)
								{
									fontGroupInfo.m_fontBold = s_templaryLoadedAsset as Font;
								}
							}
							succeeded = true;
						}
					}
				}
			}
		}
		fpFinished?.Invoke(null, succeeded);
	}

	private static IEnumerator LoadFontAsset(AssetBundle bundle, string assetName)
	{
		s_templaryLoadedAsset = null;
		if (bundle == null)
		{
			yield break;
		}
		AssetBundleRequest request = bundle.LoadAssetAsync(assetName);
		if (request != null)
		{
			while (!request.isDone)
			{
				yield return null;
			}
			if (!(request.asset == null))
			{
				s_templaryLoadedAsset = request.asset;
			}
		}
	}

	public static void UnloadFontAsset(SystemLanguage language)
	{
		FontGroupInfo value = null;
		if (s_fontGroupInfos.TryGetValue(language, out value))
		{
			UnloadFontAsset(value);
		}
	}

	private static void UnloadFontAsset(FontGroupInfo fontGroupInfo)
	{
		if (fontGroupInfo != null)
		{
			if (fontGroupInfo.m_fontNormal != null)
			{
				Resources.UnloadAsset(fontGroupInfo.m_fontNormal);
				fontGroupInfo.m_fontNormal = null;
			}
			if (fontGroupInfo.m_fontBold != null)
			{
				Resources.UnloadAsset(fontGroupInfo.m_fontBold);
				fontGroupInfo.m_fontBold = null;
			}
			if (fontGroupInfo.m_assetBundle != null)
			{
				AssetBundleManager.UnloadAssetBundle(fontGroupInfo.m_bundleName);
				fontGroupInfo.m_assetBundle = null;
			}
		}
	}

	public static void ResetTextFontByCurrentLanguage(Text textComp, bool isSetFontMaterial = true)
	{
		if (textComp == null)
		{
			return;
		}
		FontGroupInfo value = null;
		if (s_fontGroupInfos.TryGetValue(Xls.XmlDataBase.Language, out value) && value != null)
		{
			Font font = null;
			switch (GetFontType(textComp.font))
			{
			case FontType.Unknown:
				return;
			case FontType.Normal:
				font = value.m_fontNormal;
				break;
			case FontType.Bold:
				font = value.m_fontBold;
				break;
			}
			textComp.font = font;
			if (textComp.fontStyle == FontStyle.Bold)
			{
				textComp.fontStyle = FontStyle.Normal;
			}
		}
	}

	public static void ResetTextFontByCurrentLanguage(Text[] textComps, bool isSetFontMaterial = true)
	{
		if (textComps != null && textComps.Length > 0)
		{
			foreach (Text textComp in textComps)
			{
				ResetTextFontByCurrentLanguage(textComp, isSetFontMaterial);
			}
		}
	}

	public static void ResetTagTextFontByCurrentLanguage(TagText tagTextComp)
	{
		if (tagTextComp == null)
		{
			return;
		}
		FontGroupInfo value = null;
		if (s_fontGroupInfos.TryGetValue(Xls.XmlDataBase.Language, out value) && value != null)
		{
			Font font = null;
			switch (GetFontType(tagTextComp.font))
			{
			case FontType.Unknown:
				return;
			case FontType.Normal:
				font = value.m_fontNormal;
				break;
			case FontType.Bold:
				font = value.m_fontBold;
				break;
			}
			tagTextComp.font = font;
			if (tagTextComp.fontStyle == FontStyle.Bold)
			{
				tagTextComp.fontStyle = FontStyle.Normal;
			}
		}
	}

	public static void ResetTagTextFontByCurrentLanguage(TagText[] tagTextComps)
	{
		if (tagTextComps != null && tagTextComps.Length > 0)
		{
			foreach (TagText tagTextComp in tagTextComps)
			{
				ResetTagTextFontByCurrentLanguage(tagTextComp);
			}
		}
	}

	private static FontType GetFontType(Font font)
	{
		if (font == null)
		{
			return FontType.Unknown;
		}
		string[] array = s_fontNormalNames;
		foreach (string b in array)
		{
			if (string.Equals(font.name, b))
			{
				return FontType.Normal;
			}
		}
		string[] array2 = s_fontBoldNames;
		foreach (string b2 in array2)
		{
			if (string.Equals(font.name, b2))
			{
				return FontType.Bold;
			}
		}
		return FontType.Unknown;
	}
}
