using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Xls
{
	public delegate IEnumerator LoadFP(Stream stream);

	public delegate void ClearFP();

	private struct String
	{
		public int pos;

		public string strCache;

		public string Get(int streamOffset)
		{
			if (strCache != null)
			{
				return strCache;
			}
			strCache = GetString(pos + streamOffset);
			return strCache;
		}
	}

	public class XmlDataBase
	{
		protected static SystemLanguage s_curLang = SystemLanguage.Unknown;

		protected const int c_yieldUnit = 1000;

		public static SystemLanguage Language
		{
			get
			{
				return s_curLang;
			}
			set
			{
				s_curLang = value;
			}
		}
	}

	public class VideoFile : XmlDataBase
	{
		private String key;

		private String name;

		private String en_name;

		private String jp_name;

		private String cn_smp_name;

		private String cn_trd_name;

		private String extension;

		public const string DataName = "VideoFile";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<VideoFile> s_Datas = new List<VideoFile>();

		private static SortedDictionary<string, VideoFile> s_Map = new SortedDictionary<string, VideoFile>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strKrVideoFileName => name.Get(s_posStringInStream);

		public string m_strEnVideoFileName => en_name.Get(s_posStringInStream);

		public string m_strJpVideoFileName => jp_name.Get(s_posStringInStream);

		public string m_strSmpCnVideoFileName => cn_smp_name.Get(s_posStringInStream);

		public string m_strTrdCnVideoFileName => cn_trd_name.Get(s_posStringInStream);

		public string m_strFileExtension => extension.Get(s_posStringInStream);

		public static List<VideoFile> datas => s_Datas;

		public static SortedDictionary<string, VideoFile> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, name_Values, 0, dataSize);
			int[] en_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, en_name_Values, 0, dataSize);
			int[] jp_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, jp_name_Values, 0, dataSize);
			int[] cn_smp_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, cn_smp_name_Values, 0, dataSize);
			int[] cn_trd_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, cn_trd_name_Values, 0, dataSize);
			int[] extension_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, extension_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				VideoFile data = new VideoFile
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					name = 
					{
						pos = name_Values[i],
						strCache = null
					},
					en_name = 
					{
						pos = en_name_Values[i],
						strCache = null
					},
					jp_name = 
					{
						pos = jp_name_Values[i],
						strCache = null
					},
					cn_smp_name = 
					{
						pos = cn_smp_name_Values[i],
						strCache = null
					},
					cn_trd_name = 
					{
						pos = cn_trd_name_Values[i],
						strCache = null
					},
					extension = 
					{
						pos = extension_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static VideoFile GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static VideoFile GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class UISound : XmlDataBase
	{
		private String key;

		private String filename;

		private int loop;

		private float volume;

		private String channel;

		public const string DataName = "UISound";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<UISound> s_Datas = new List<UISound>();

		private static SortedDictionary<string, UISound> s_Map = new SortedDictionary<string, UISound>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strFileName => filename.Get(s_posStringInStream);

		public int m_iLoop => loop;

		public float m_fVol => volume;

		public string m_strChannel => channel.Get(s_posStringInStream);

		public static List<UISound> datas => s_Datas;

		public static SortedDictionary<string, UISound> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] filename_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, filename_Values, 0, dataSize);
			int[] loop_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, loop_Values, 0, dataSize);
			float[] volume_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, volume_Values, 0, dataSize);
			int[] channel_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, channel_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				UISound data = new UISound
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					filename = 
					{
						pos = filename_Values[i],
						strCache = null
					},
					loop = loop_Values[i],
					volume = volume_Values[i],
					channel = 
					{
						pos = channel_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static UISound GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static UISound GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class SoundFile : XmlDataBase
	{
		private String key;

		private String name;

		public const string DataName = "SoundFile";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<SoundFile> s_Datas = new List<SoundFile>();

		private static SortedDictionary<string, SoundFile> s_Map = new SortedDictionary<string, SoundFile>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strFileName => name.Get(s_posStringInStream);

		public static List<SoundFile> datas => s_Datas;

		public static SortedDictionary<string, SoundFile> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, name_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				SoundFile data = new SoundFile
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					name = 
					{
						pos = name_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static SoundFile GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static SoundFile GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class SelectData : XmlDataBase
	{
		private int index;

		private String sel_key;

		private String quest_id;

		private String ans0_id;

		private String ans1_id;

		private String ans0_sub_id;

		private String ans1_sub_id;

		public const string DataName = "SelectData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<SelectData> s_Datas = new List<SelectData>();

		private static SortedDictionary<string, SelectData> s_Map = new SortedDictionary<string, SelectData>();

		private static SortedDictionary<int, SelectData> s_SwitchIdxMap = new SortedDictionary<int, SelectData>();

		public int m_iIndex => index;

		public string m_strSelKey => sel_key.Get(s_posStringInStream);

		public string m_strIDQuest => quest_id.Get(s_posStringInStream);

		public string m_strIDAns0 => ans0_id.Get(s_posStringInStream);

		public string m_strIDAns1 => ans1_id.Get(s_posStringInStream);

		public string m_strIDAns0Sub => ans0_sub_id.Get(s_posStringInStream);

		public string m_strIDAns1Sub => ans1_sub_id.Get(s_posStringInStream);

		public static List<SelectData> datas => s_Datas;

		public static SortedDictionary<string, SelectData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] sel_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, sel_key_Values, 0, dataSize);
			int[] quest_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, quest_id_Values, 0, dataSize);
			int[] ans0_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ans0_id_Values, 0, dataSize);
			int[] ans1_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ans1_id_Values, 0, dataSize);
			int[] ans0_sub_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ans0_sub_id_Values, 0, dataSize);
			int[] ans1_sub_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ans1_sub_id_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				SelectData data = new SelectData
				{
					index = index_Values[i],
					sel_key = 
					{
						pos = sel_key_Values[i],
						strCache = null
					},
					quest_id = 
					{
						pos = quest_id_Values[i],
						strCache = null
					},
					ans0_id = 
					{
						pos = ans0_id_Values[i],
						strCache = null
					},
					ans1_id = 
					{
						pos = ans1_id_Values[i],
						strCache = null
					},
					ans0_sub_id = 
					{
						pos = ans0_sub_id_Values[i],
						strCache = null
					},
					ans1_sub_id = 
					{
						pos = ans1_sub_id_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.sel_key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static SelectData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static SelectData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static SelectData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ScriptSpeechData : XmlDataBase
	{
		private String text_id;

		private String title_ko;

		private String text_en;

		private String text_jp;

		private String text_sc;

		private String text_tc;

		private String sound;

		public const string DataName = "SpeechData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ScriptSpeechData> s_Datas = new List<ScriptSpeechData>();

		private static SortedDictionary<string, ScriptSpeechData> s_Map = new SortedDictionary<string, ScriptSpeechData>();

		public string m_strIDText => text_id.Get(s_posStringInStream);

		public string m_strTxtKo => title_ko.Get(s_posStringInStream);

		public string m_strTxtEn => text_en.Get(s_posStringInStream);

		public string m_strTxtJp => text_jp.Get(s_posStringInStream);

		public string m_strTxtSc => text_sc.Get(s_posStringInStream);

		public string m_strTxtTc => text_tc.Get(s_posStringInStream);

		public string m_strSound => sound.Get(s_posStringInStream);

		public string m_strTxt => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTxtKo, 
			SystemLanguage.English => m_strTxtEn, 
			SystemLanguage.Japanese => m_strTxtJp, 
			SystemLanguage.ChineseSimplified => m_strTxtSc, 
			SystemLanguage.ChineseTraditional => m_strTxtTc, 
			_ => m_strTxtKo, 
		};

		public static List<ScriptSpeechData> datas => s_Datas;

		public static SortedDictionary<string, ScriptSpeechData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] text_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_id_Values, 0, dataSize);
			int[] title_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_ko_Values, 0, dataSize);
			int[] text_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_en_Values, 0, dataSize);
			int[] text_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_jp_Values, 0, dataSize);
			int[] text_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_sc_Values, 0, dataSize);
			int[] text_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_tc_Values, 0, dataSize);
			int[] sound_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, sound_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ScriptSpeechData data = new ScriptSpeechData
				{
					text_id = 
					{
						pos = text_id_Values[i],
						strCache = null
					},
					title_ko = 
					{
						pos = title_ko_Values[i],
						strCache = null
					},
					text_en = 
					{
						pos = text_en_Values[i],
						strCache = null
					},
					text_jp = 
					{
						pos = text_jp_Values[i],
						strCache = null
					},
					text_sc = 
					{
						pos = text_sc_Values[i],
						strCache = null
					},
					text_tc = 
					{
						pos = text_tc_Values[i],
						strCache = null
					},
					sound = 
					{
						pos = sound_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.text_id.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ScriptSpeechData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ScriptSpeechData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class StaffRoll : XmlDataBase
	{
		private String category;

		private int size;

		private String text_key;

		private float x_pos;

		public const string DataName = "StaffRoll";

		public const bool IsUseKey = false;

		private static int s_posStringInStream = 0;

		private static List<StaffRoll> s_Datas = new List<StaffRoll>();

		public string m_strCtg => category.Get(s_posStringInStream);

		public int m_iSize => size;

		public string m_strTxtKey => text_key.Get(s_posStringInStream);

		public float m_fXPos => x_pos;

		public static List<StaffRoll> datas => s_Datas;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] category_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, category_Values, 0, dataSize);
			int[] size_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, size_Values, 0, dataSize);
			int[] text_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_key_Values, 0, dataSize);
			float[] x_pos_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, x_pos_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				StaffRoll data = new StaffRoll
				{
					category = 
					{
						pos = category_Values[i],
						strCache = null
					},
					size = size_Values[i],
					text_key = 
					{
						pos = text_key_Values[i],
						strCache = null
					},
					x_pos = x_pos_Values[i]
				};
				s_Datas.Add(data);
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
		}

		public static StaffRoll GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class InGameTime : XmlDataBase
	{
		private String key;

		private String text_InGameTime;

		public const string DataName = "InGameTime";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<InGameTime> s_Datas = new List<InGameTime>();

		private static SortedDictionary<string, InGameTime> s_Map = new SortedDictionary<string, InGameTime>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strInGameTime => text_InGameTime.Get(s_posStringInStream);

		public static List<InGameTime> datas => s_Datas;

		public static SortedDictionary<string, InGameTime> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_InGameTime_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_InGameTime_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				InGameTime data = new InGameTime
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_InGameTime = 
					{
						pos = text_InGameTime_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static InGameTime GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static InGameTime GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class AccountData : XmlDataBase
	{
		private String key;

		private String sns_pic;

		private String mes_pic;

		private String lineup;

		private String nickname;

		private String char_key;

		public const string DataName = "AccountData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<AccountData> s_Datas = new List<AccountData>();

		private static SortedDictionary<string, AccountData> s_Map = new SortedDictionary<string, AccountData>();

		public string m_key => key.Get(s_posStringInStream);

		public string m_snspicID => sns_pic.Get(s_posStringInStream);

		public string m_mespicID => mes_pic.Get(s_posStringInStream);

		public string m_lineupID => lineup.Get(s_posStringInStream);

		public string m_nicknameID => nickname.Get(s_posStringInStream);

		public string m_charKey => char_key.Get(s_posStringInStream);

		public static List<AccountData> datas => s_Datas;

		public static SortedDictionary<string, AccountData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] sns_pic_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, sns_pic_Values, 0, dataSize);
			int[] mes_pic_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, mes_pic_Values, 0, dataSize);
			int[] lineup_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lineup_Values, 0, dataSize);
			int[] nickname_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, nickname_Values, 0, dataSize);
			int[] char_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, char_key_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				AccountData data = new AccountData
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					sns_pic = 
					{
						pos = sns_pic_Values[i],
						strCache = null
					},
					mes_pic = 
					{
						pos = mes_pic_Values[i],
						strCache = null
					},
					lineup = 
					{
						pos = lineup_Values[i],
						strCache = null
					},
					nickname = 
					{
						pos = nickname_Values[i],
						strCache = null
					},
					char_key = 
					{
						pos = char_key_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static AccountData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static AccountData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class MessengerChatroomData : XmlDataBase
	{
		private String id;

		private String textId;

		private String profileImage;

		public const string DataName = "MessengerChatroomData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<MessengerChatroomData> s_Datas = new List<MessengerChatroomData>();

		private static SortedDictionary<string, MessengerChatroomData> s_Map = new SortedDictionary<string, MessengerChatroomData>();

		public string m_strID => id.Get(s_posStringInStream);

		public string m_strTextID => textId.Get(s_posStringInStream);

		public string m_strProfileImagePath => profileImage.Get(s_posStringInStream);

		public static List<MessengerChatroomData> datas => s_Datas;

		public static SortedDictionary<string, MessengerChatroomData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_Values, 0, dataSize);
			int[] textId_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, textId_Values, 0, dataSize);
			int[] profileImage_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, profileImage_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				MessengerChatroomData data = new MessengerChatroomData
				{
					id = 
					{
						pos = id_Values[i],
						strCache = null
					},
					textId = 
					{
						pos = textId_Values[i],
						strCache = null
					},
					profileImage = 
					{
						pos = profileImage_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.id.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static MessengerChatroomData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static MessengerChatroomData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class MessengerTalkData : XmlDataBase
	{
		private int index;

		private String id;

		private String id_seq;

		private String id_chatroom;

		private String id_account;

		private String id_text;

		private String id_img;

		public const string DataName = "MessengerTalkData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<MessengerTalkData> s_Datas = new List<MessengerTalkData>();

		private static SortedDictionary<string, MessengerTalkData> s_Map = new SortedDictionary<string, MessengerTalkData>();

		private static SortedDictionary<int, MessengerTalkData> s_SwitchIdxMap = new SortedDictionary<int, MessengerTalkData>();

		public int m_iIdx => index;

		public string m_strID => id.Get(s_posStringInStream);

		public string m_strIDSeq => id_seq.Get(s_posStringInStream);

		public string m_strIDchat => id_chatroom.Get(s_posStringInStream);

		public string m_strIDAcc => id_account.Get(s_posStringInStream);

		public string m_strIDText => id_text.Get(s_posStringInStream);

		public string m_strIDImg => id_img.Get(s_posStringInStream);

		public static List<MessengerTalkData> datas => s_Datas;

		public static SortedDictionary<string, MessengerTalkData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_Values, 0, dataSize);
			int[] id_seq_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_seq_Values, 0, dataSize);
			int[] id_chatroom_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_chatroom_Values, 0, dataSize);
			int[] id_account_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_account_Values, 0, dataSize);
			int[] id_text_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_text_Values, 0, dataSize);
			int[] id_img_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_img_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				MessengerTalkData data = new MessengerTalkData
				{
					index = index_Values[i],
					id = 
					{
						pos = id_Values[i],
						strCache = null
					},
					id_seq = 
					{
						pos = id_seq_Values[i],
						strCache = null
					},
					id_chatroom = 
					{
						pos = id_chatroom_Values[i],
						strCache = null
					},
					id_account = 
					{
						pos = id_account_Values[i],
						strCache = null
					},
					id_text = 
					{
						pos = id_text_Values[i],
						strCache = null
					},
					id_img = 
					{
						pos = id_img_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.id.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static MessengerTalkData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static MessengerTalkData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static MessengerTalkData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class WatchFaterAutoText : XmlDataBase
	{
		private int index;

		private String key;

		private String textId;

		private int m_switch;

		public const string DataName = "WatchFaterAutoText";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<WatchFaterAutoText> s_Datas = new List<WatchFaterAutoText>();

		private static SortedDictionary<string, WatchFaterAutoText> s_Map = new SortedDictionary<string, WatchFaterAutoText>();

		private static SortedDictionary<int, WatchFaterAutoText> s_SwitchIdxMap = new SortedDictionary<int, WatchFaterAutoText>();

		public int m_iIndex => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTextID => textId.Get(s_posStringInStream);

		public int m_iEvtSwitch => m_switch;

		public static List<WatchFaterAutoText> datas => s_Datas;

		public static SortedDictionary<string, WatchFaterAutoText> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] textId_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, textId_Values, 0, dataSize);
			int[] m_switch_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, m_switch_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				WatchFaterAutoText data = new WatchFaterAutoText
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					textId = 
					{
						pos = textId_Values[i],
						strCache = null
					},
					m_switch = m_switch_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static WatchFaterAutoText GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static WatchFaterAutoText GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static WatchFaterAutoText GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CharData_ForProfile : XmlDataBase
	{
		private String key;

		private String name;

		private String updateThropyKey;

		private int updateThropySwitch;

		private String profileText;

		private String imageKey;

		private String profileText_Update;

		private String imageKey_Update;

		private int categoryIdx;

		public const string DataName = "CharData_ForProfile";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CharData_ForProfile> s_Datas = new List<CharData_ForProfile>();

		private static SortedDictionary<string, CharData_ForProfile> s_Map = new SortedDictionary<string, CharData_ForProfile>();

		private static SortedDictionary<int, CharData_ForProfile> s_SwitchIdxMap = new SortedDictionary<int, CharData_ForProfile>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strName => name.Get(s_posStringInStream);

		public string m_strUpdateThropyKey => updateThropyKey.Get(s_posStringInStream);

		public int m_iUpdateThropySwitch => updateThropySwitch;

		public string m_strProfileText => profileText.Get(s_posStringInStream);

		public string m_strImageKey => imageKey.Get(s_posStringInStream);

		public string m_strProfileText_Update => profileText_Update.Get(s_posStringInStream);

		public string m_strImageKey_Update => imageKey_Update.Get(s_posStringInStream);

		public int m_iCategoryIdx => categoryIdx;

		public static List<CharData_ForProfile> datas => s_Datas;

		public static SortedDictionary<string, CharData_ForProfile> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, name_Values, 0, dataSize);
			int[] updateThropyKey_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, updateThropyKey_Values, 0, dataSize);
			int[] updateThropySwitch_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, updateThropySwitch_Values, 0, dataSize);
			int[] profileText_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, profileText_Values, 0, dataSize);
			int[] imageKey_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, imageKey_Values, 0, dataSize);
			int[] profileText_Update_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, profileText_Update_Values, 0, dataSize);
			int[] imageKey_Update_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, imageKey_Update_Values, 0, dataSize);
			int[] categoryIdx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, categoryIdx_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CharData_ForProfile data = new CharData_ForProfile
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					name = 
					{
						pos = name_Values[i],
						strCache = null
					},
					updateThropyKey = 
					{
						pos = updateThropyKey_Values[i],
						strCache = null
					},
					updateThropySwitch = updateThropySwitch_Values[i],
					profileText = 
					{
						pos = profileText_Values[i],
						strCache = null
					},
					imageKey = 
					{
						pos = imageKey_Values[i],
						strCache = null
					},
					profileText_Update = 
					{
						pos = profileText_Update_Values[i],
						strCache = null
					},
					imageKey_Update = 
					{
						pos = imageKey_Update_Values[i],
						strCache = null
					},
					categoryIdx = categoryIdx_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.categoryIdx;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CharData_ForProfile GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CharData_ForProfile GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CharData_ForProfile GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class SNSPostData : XmlDataBase
	{
		private int index;

		private String id;

		private String id_seq;

		private int phase;

		private int CheckSwitch;

		private int PostInitState;

		private int postType;

		private int isSelPost;

		private String id_account;

		private String postTime;

		private String id_text;

		private int frequency;

		private int retweetCnt;

		private String id_reply;

		private int reply_group;

		private String id_keyword;

		private int keyword_group;

		private int mentalDelta;

		private String id_ads;

		private String id_img;

		public const string DataName = "SNSPostData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<SNSPostData> s_Datas = new List<SNSPostData>();

		private static SortedDictionary<string, SNSPostData> s_Map = new SortedDictionary<string, SNSPostData>();

		private static SortedDictionary<int, SNSPostData> s_SwitchIdxMap = new SortedDictionary<int, SNSPostData>();

		public int m_iIdx => index;

		public string m_strID => id.Get(s_posStringInStream);

		public string m_strIDSeq => id_seq.Get(s_posStringInStream);

		public int m_iPhase => phase;

		public int m_iCheckEvtSwitch => CheckSwitch;

		public int m_iPostInitState => PostInitState;

		public int m_iPostType => postType;

		public int m_iIsSelPost => isSelPost;

		public string m_strIDAcc => id_account.Get(s_posStringInStream);

		public string m_strPostTime => postTime.Get(s_posStringInStream);

		public string m_strIDText => id_text.Get(s_posStringInStream);

		public int m_iFrequency => frequency;

		public int m_iRetweetCnt => retweetCnt;

		public string m_strIDReply => id_reply.Get(s_posStringInStream);

		public int m_iReplyGroup => reply_group;

		public string m_strIDKeyword => id_keyword.Get(s_posStringInStream);

		public int m_iGroupKeyword => keyword_group;

		public int m_iMentalDelta => mentalDelta;

		public string m_strIDAds => id_ads.Get(s_posStringInStream);

		public string m_strIDImg => id_img.Get(s_posStringInStream);

		public static List<SNSPostData> datas => s_Datas;

		public static SortedDictionary<string, SNSPostData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_Values, 0, dataSize);
			int[] id_seq_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_seq_Values, 0, dataSize);
			int[] phase_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, phase_Values, 0, dataSize);
			int[] CheckSwitch_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, CheckSwitch_Values, 0, dataSize);
			int[] PostInitState_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, PostInitState_Values, 0, dataSize);
			int[] postType_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, postType_Values, 0, dataSize);
			int[] isSelPost_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, isSelPost_Values, 0, dataSize);
			int[] id_account_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_account_Values, 0, dataSize);
			int[] postTime_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, postTime_Values, 0, dataSize);
			int[] id_text_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_text_Values, 0, dataSize);
			int[] frequency_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, frequency_Values, 0, dataSize);
			int[] retweetCnt_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, retweetCnt_Values, 0, dataSize);
			int[] id_reply_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_reply_Values, 0, dataSize);
			int[] reply_group_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, reply_group_Values, 0, dataSize);
			int[] id_keyword_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_keyword_Values, 0, dataSize);
			int[] keyword_group_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, keyword_group_Values, 0, dataSize);
			int[] mentalDelta_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, mentalDelta_Values, 0, dataSize);
			int[] id_ads_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_ads_Values, 0, dataSize);
			int[] id_img_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_img_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				SNSPostData data = new SNSPostData
				{
					index = index_Values[i],
					id = 
					{
						pos = id_Values[i],
						strCache = null
					},
					id_seq = 
					{
						pos = id_seq_Values[i],
						strCache = null
					},
					phase = phase_Values[i],
					CheckSwitch = CheckSwitch_Values[i],
					PostInitState = PostInitState_Values[i],
					postType = postType_Values[i],
					isSelPost = isSelPost_Values[i],
					id_account = 
					{
						pos = id_account_Values[i],
						strCache = null
					},
					postTime = 
					{
						pos = postTime_Values[i],
						strCache = null
					},
					id_text = 
					{
						pos = id_text_Values[i],
						strCache = null
					},
					frequency = frequency_Values[i],
					retweetCnt = retweetCnt_Values[i],
					id_reply = 
					{
						pos = id_reply_Values[i],
						strCache = null
					},
					reply_group = reply_group_Values[i],
					id_keyword = 
					{
						pos = id_keyword_Values[i],
						strCache = null
					},
					keyword_group = keyword_group_Values[i],
					mentalDelta = mentalDelta_Values[i],
					id_ads = 
					{
						pos = id_ads_Values[i],
						strCache = null
					},
					id_img = 
					{
						pos = id_img_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.id.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static SNSPostData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static SNSPostData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static SNSPostData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class SNSReplyData : XmlDataBase
	{
		private String key;

		private String leftPostID;

		private String leftTextSub;

		private String leftText;

		private int leftPostSwitch;

		private String leftPostConti;

		private String rightPostID;

		private String rightTextSub;

		private String rightText;

		private int rightPostSwitch;

		private String rightPostConti;

		public const string DataName = "SNSReplyData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<SNSReplyData> s_Datas = new List<SNSReplyData>();

		private static SortedDictionary<string, SNSReplyData> s_Map = new SortedDictionary<string, SNSReplyData>();

		public string m_key => key.Get(s_posStringInStream);

		public string m_leftPostID => leftPostID.Get(s_posStringInStream);

		public string m_leftTextSub => leftTextSub.Get(s_posStringInStream);

		public string m_leftText => leftText.Get(s_posStringInStream);

		public int m_leftPostSwitch => leftPostSwitch;

		public string m_leftPostConti => leftPostConti.Get(s_posStringInStream);

		public string m_rightPostID => rightPostID.Get(s_posStringInStream);

		public string m_rightTextSub => rightTextSub.Get(s_posStringInStream);

		public string m_rightText => rightText.Get(s_posStringInStream);

		public int m_rightPostSwitch => rightPostSwitch;

		public string m_rightPostConti => rightPostConti.Get(s_posStringInStream);

		public static List<SNSReplyData> datas => s_Datas;

		public static SortedDictionary<string, SNSReplyData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] leftPostID_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, leftPostID_Values, 0, dataSize);
			int[] leftTextSub_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, leftTextSub_Values, 0, dataSize);
			int[] leftText_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, leftText_Values, 0, dataSize);
			int[] leftPostSwitch_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, leftPostSwitch_Values, 0, dataSize);
			int[] leftPostConti_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, leftPostConti_Values, 0, dataSize);
			int[] rightPostID_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rightPostID_Values, 0, dataSize);
			int[] rightTextSub_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rightTextSub_Values, 0, dataSize);
			int[] rightText_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rightText_Values, 0, dataSize);
			int[] rightPostSwitch_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rightPostSwitch_Values, 0, dataSize);
			int[] rightPostConti_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rightPostConti_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				SNSReplyData data = new SNSReplyData
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					leftPostID = 
					{
						pos = leftPostID_Values[i],
						strCache = null
					},
					leftTextSub = 
					{
						pos = leftTextSub_Values[i],
						strCache = null
					},
					leftText = 
					{
						pos = leftText_Values[i],
						strCache = null
					},
					leftPostSwitch = leftPostSwitch_Values[i],
					leftPostConti = 
					{
						pos = leftPostConti_Values[i],
						strCache = null
					},
					rightPostID = 
					{
						pos = rightPostID_Values[i],
						strCache = null
					},
					rightTextSub = 
					{
						pos = rightTextSub_Values[i],
						strCache = null
					},
					rightText = 
					{
						pos = rightText_Values[i],
						strCache = null
					},
					rightPostSwitch = rightPostSwitch_Values[i],
					rightPostConti = 
					{
						pos = rightPostConti_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static SNSReplyData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static SNSReplyData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class SequenceData : XmlDataBase
	{
		private int index;

		private String key;

		private String nameID;

		private String keywordSet;

		private String saveimage;

		public const string DataName = "SequenceData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<SequenceData> s_Datas = new List<SequenceData>();

		private static SortedDictionary<string, SequenceData> s_Map = new SortedDictionary<string, SequenceData>();

		private static SortedDictionary<int, SequenceData> s_SwitchIdxMap = new SortedDictionary<int, SequenceData>();

		public int m_iIdx => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strIDName => nameID.Get(s_posStringInStream);

		public string m_strKeywordSet => keywordSet.Get(s_posStringInStream);

		public string m_strSaveImg => saveimage.Get(s_posStringInStream);

		public static List<SequenceData> datas => s_Datas;

		public static SortedDictionary<string, SequenceData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] nameID_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, nameID_Values, 0, dataSize);
			int[] keywordSet_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, keywordSet_Values, 0, dataSize);
			int[] saveimage_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, saveimage_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				SequenceData data = new SequenceData
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					nameID = 
					{
						pos = nameID_Values[i],
						strCache = null
					},
					keywordSet = 
					{
						pos = keywordSet_Values[i],
						strCache = null
					},
					saveimage = 
					{
						pos = saveimage_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static SequenceData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static SequenceData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static SequenceData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class Voice : XmlDataBase
	{
		private String key;

		private String bundleName;

		private String voice_ko;

		private String voice_en;

		private String voice_jp;

		public const string DataName = "Voice";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<Voice> s_Datas = new List<Voice>();

		private static SortedDictionary<string, Voice> s_Map = new SortedDictionary<string, Voice>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strBundleName => bundleName.Get(s_posStringInStream);

		public string m_srtVoiceKo => voice_ko.Get(s_posStringInStream);

		public string m_srtVoiceEn => voice_en.Get(s_posStringInStream);

		public string m_srtVoiceJp => voice_jp.Get(s_posStringInStream);

		public static List<Voice> datas => s_Datas;

		public static SortedDictionary<string, Voice> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] bundleName_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, bundleName_Values, 0, dataSize);
			int[] voice_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, voice_ko_Values, 0, dataSize);
			int[] voice_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, voice_en_Values, 0, dataSize);
			int[] voice_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, voice_jp_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				Voice data = new Voice
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					bundleName = 
					{
						pos = bundleName_Values[i],
						strCache = null
					},
					voice_ko = 
					{
						pos = voice_ko_Values[i],
						strCache = null
					},
					voice_en = 
					{
						pos = voice_en_Values[i],
						strCache = null
					},
					voice_jp = 
					{
						pos = voice_jp_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static Voice GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static Voice GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ImageFile : XmlDataBase
	{
		private String key;

		private String file_name;

		private String thumbnail_name;

		public const string DataName = "ImageFile";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ImageFile> s_Datas = new List<ImageFile>();

		private static SortedDictionary<string, ImageFile> s_Map = new SortedDictionary<string, ImageFile>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strAssetPath => file_name.Get(s_posStringInStream);

		public string m_strAssetPath_Thumbnail => thumbnail_name.Get(s_posStringInStream);

		public static List<ImageFile> datas => s_Datas;

		public static SortedDictionary<string, ImageFile> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] file_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, file_name_Values, 0, dataSize);
			int[] thumbnail_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, thumbnail_name_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ImageFile data = new ImageFile
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					file_name = 
					{
						pos = file_name_Values[i],
						strCache = null
					},
					thumbnail_name = 
					{
						pos = thumbnail_name_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ImageFile GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ImageFile GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ScriptKeyValue : XmlDataBase
	{
		private String key;

		private int value;

		public const string DataName = "ScriptKeyValue";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ScriptKeyValue> s_Datas = new List<ScriptKeyValue>();

		private static SortedDictionary<string, ScriptKeyValue> s_Map = new SortedDictionary<string, ScriptKeyValue>();

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iValue => value;

		public static List<ScriptKeyValue> datas => s_Datas;

		public static SortedDictionary<string, ScriptKeyValue> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] value_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, value_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ScriptKeyValue data = new ScriptKeyValue
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					value = value_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ScriptKeyValue GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ScriptKeyValue GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ScriptKeyTextValue : XmlDataBase
	{
		private String key;

		private String value;

		public const string DataName = "ScriptKeyTextValue";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ScriptKeyTextValue> s_Datas = new List<ScriptKeyTextValue>();

		private static SortedDictionary<string, ScriptKeyTextValue> s_Map = new SortedDictionary<string, ScriptKeyTextValue>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strValue => value.Get(s_posStringInStream);

		public static List<ScriptKeyTextValue> datas => s_Datas;

		public static SortedDictionary<string, ScriptKeyTextValue> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] value_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, value_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ScriptKeyTextValue data = new ScriptKeyTextValue
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					value = 
					{
						pos = value_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ScriptKeyTextValue GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ScriptKeyTextValue GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CameraShakeValue : XmlDataBase
	{
		private String key;

		private float powerMin;

		private float powerMax;

		private float playTMin;

		private float playTMax;

		private float gapMin;

		private float gapMax;

		public const string DataName = "CameraShakeValue";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CameraShakeValue> s_Datas = new List<CameraShakeValue>();

		private static SortedDictionary<string, CameraShakeValue> s_Map = new SortedDictionary<string, CameraShakeValue>();

		public string m_strKey => key.Get(s_posStringInStream);

		public float m_fPowerMin => powerMin;

		public float m_fPowerMax => powerMax;

		public float m_fPlayTMin => playTMin;

		public float m_fPlayTMax => playTMax;

		public float m_fGapMin => gapMin;

		public float m_fGapMax => gapMax;

		public static List<CameraShakeValue> datas => s_Datas;

		public static SortedDictionary<string, CameraShakeValue> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			float[] powerMin_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, powerMin_Values, 0, dataSize);
			float[] powerMax_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, powerMax_Values, 0, dataSize);
			float[] playTMin_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, playTMin_Values, 0, dataSize);
			float[] playTMax_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, playTMax_Values, 0, dataSize);
			float[] gapMin_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, gapMin_Values, 0, dataSize);
			float[] gapMax_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, gapMax_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CameraShakeValue data = new CameraShakeValue
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					powerMin = powerMin_Values[i],
					powerMax = powerMax_Values[i],
					playTMin = playTMin_Values[i],
					playTMax = playTMax_Values[i],
					gapMin = gapMin_Values[i],
					gapMax = gapMax_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static CameraShakeValue GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CameraShakeValue GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CharRelationEvt : XmlDataBase
	{
		private int idx;

		private String key;

		private int text_xls_key;

		private String char_res_name;

		private String check_event_num;

		private String check_trophy_key;

		public const string DataName = "CharRelationEvt";

		public const bool IsUseKey = false;

		private static int s_posStringInStream = 0;

		private static List<CharRelationEvt> s_Datas = new List<CharRelationEvt>();

		private static SortedDictionary<int, CharRelationEvt> s_SwitchIdxMap = new SortedDictionary<int, CharRelationEvt>();

		public int m_iIdx => idx;

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iRelationMax => text_xls_key;

		public string m_strContiName => char_res_name.Get(s_posStringInStream);

		public string m_strCheckEvtNum => check_event_num.Get(s_posStringInStream);

		public string m_strCheckTrpKey => check_trophy_key.Get(s_posStringInStream);

		public static List<CharRelationEvt> datas => s_Datas;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, idx_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_xls_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_xls_key_Values, 0, dataSize);
			int[] char_res_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, char_res_name_Values, 0, dataSize);
			int[] check_event_num_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, check_event_num_Values, 0, dataSize);
			int[] check_trophy_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, check_trophy_key_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CharRelationEvt data = new CharRelationEvt
				{
					idx = idx_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_xls_key = text_xls_key_Values[i],
					char_res_name = 
					{
						pos = char_res_name_Values[i],
						strCache = null
					},
					check_event_num = 
					{
						pos = check_event_num_Values[i],
						strCache = null
					},
					check_trophy_key = 
					{
						pos = check_trophy_key_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				int switchIdx = data.idx;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CharRelationEvt GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CharRelationEvt GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CharMotData : XmlDataBase
	{
		private int idx;

		private String mot_id;

		private String mot_name;

		public const string DataName = "CharMotData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CharMotData> s_Datas = new List<CharMotData>();

		private static SortedDictionary<string, CharMotData> s_Map = new SortedDictionary<string, CharMotData>();

		private static SortedDictionary<int, CharMotData> s_SwitchIdxMap = new SortedDictionary<int, CharMotData>();

		public int m_iIdx => idx;

		public string m_strID => mot_id.Get(s_posStringInStream);

		public string m_strMotName => mot_name.Get(s_posStringInStream);

		public static List<CharMotData> datas => s_Datas;

		public static SortedDictionary<string, CharMotData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, idx_Values, 0, dataSize);
			int[] mot_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, mot_id_Values, 0, dataSize);
			int[] mot_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, mot_name_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CharMotData data = new CharMotData
				{
					idx = idx_Values[i],
					mot_id = 
					{
						pos = mot_id_Values[i],
						strCache = null
					},
					mot_name = 
					{
						pos = mot_name_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.mot_id.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.idx;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CharMotData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CharMotData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CharMotData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CharData : XmlDataBase
	{
		private int idx;

		private String key;

		private String text_xls_key;

		private String rank_img_url;

		private String char_res_name;

		private int Left;

		private int isActor;

		private String profile_icon;

		private String result_slot_image;

		private String call_list_image;

		private int call_list_slot;

		private String dot_icon_image;

		private int use_idx;

		private String talk_window_name_color;

		private String text_cv_name;

		public const string DataName = "CharData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CharData> s_Datas = new List<CharData>();

		private static SortedDictionary<string, CharData> s_Map = new SortedDictionary<string, CharData>();

		private static SortedDictionary<int, CharData> s_SwitchIdxMap = new SortedDictionary<int, CharData>();

		public int m_iIdx => idx;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strNameKey => text_xls_key.Get(s_posStringInStream);

		public string m_strRankImg => rank_img_url.Get(s_posStringInStream);

		public string m_strResName => char_res_name.Get(s_posStringInStream);

		public int m_iShowLeft => Left;

		public int m_iActor => isActor;

		public string m_strProfIcon => profile_icon.Get(s_posStringInStream);

		public string m_strResultSlotImage => result_slot_image.Get(s_posStringInStream);

		public string m_strCallListImage => call_list_image.Get(s_posStringInStream);

		public int m_iCallListSlot => call_list_slot;

		public string m_strDotIconImage => dot_icon_image.Get(s_posStringInStream);

		public int m_iUseIdx => use_idx;

		public string m_strTalkColor => talk_window_name_color.Get(s_posStringInStream);

		public string m_strCVName => text_cv_name.Get(s_posStringInStream);

		public static List<CharData> datas => s_Datas;

		public static SortedDictionary<string, CharData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, idx_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_xls_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_xls_key_Values, 0, dataSize);
			int[] rank_img_url_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, rank_img_url_Values, 0, dataSize);
			int[] char_res_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, char_res_name_Values, 0, dataSize);
			int[] Left_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, Left_Values, 0, dataSize);
			int[] isActor_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, isActor_Values, 0, dataSize);
			int[] profile_icon_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, profile_icon_Values, 0, dataSize);
			int[] result_slot_image_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, result_slot_image_Values, 0, dataSize);
			int[] call_list_image_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, call_list_image_Values, 0, dataSize);
			int[] call_list_slot_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, call_list_slot_Values, 0, dataSize);
			int[] dot_icon_image_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, dot_icon_image_Values, 0, dataSize);
			int[] use_idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, use_idx_Values, 0, dataSize);
			int[] talk_window_name_color_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, talk_window_name_color_Values, 0, dataSize);
			int[] text_cv_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_cv_name_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CharData data = new CharData
				{
					idx = idx_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_xls_key = 
					{
						pos = text_xls_key_Values[i],
						strCache = null
					},
					rank_img_url = 
					{
						pos = rank_img_url_Values[i],
						strCache = null
					},
					char_res_name = 
					{
						pos = char_res_name_Values[i],
						strCache = null
					},
					Left = Left_Values[i],
					isActor = isActor_Values[i],
					profile_icon = 
					{
						pos = profile_icon_Values[i],
						strCache = null
					},
					result_slot_image = 
					{
						pos = result_slot_image_Values[i],
						strCache = null
					},
					call_list_image = 
					{
						pos = call_list_image_Values[i],
						strCache = null
					},
					call_list_slot = call_list_slot_Values[i],
					dot_icon_image = 
					{
						pos = dot_icon_image_Values[i],
						strCache = null
					},
					use_idx = use_idx_Values[i],
					talk_window_name_color = 
					{
						pos = talk_window_name_color_Values[i],
						strCache = null
					},
					text_cv_name = 
					{
						pos = text_cv_name_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.idx;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CharData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CharData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CharData GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CharZoomPos : XmlDataBase
	{
		private int index;

		private float zoomValue;

		private float posX;

		private float posY;

		public const string DataName = "CharZoomPos";

		public const bool IsUseKey = false;

		private static int s_posStringInStream = 0;

		private static List<CharZoomPos> s_Datas = new List<CharZoomPos>();

		private static SortedDictionary<int, CharZoomPos> s_SwitchIdxMap = new SortedDictionary<int, CharZoomPos>();

		public int m_iIndex => index;

		public float m_fZoomValue => zoomValue;

		public float m_fPosX => posX;

		public float m_fPosY => posY;

		public static List<CharZoomPos> datas => s_Datas;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			float[] zoomValue_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, zoomValue_Values, 0, dataSize);
			float[] posX_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posX_Values, 0, dataSize);
			float[] posY_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posY_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CharZoomPos data = new CharZoomPos
				{
					index = index_Values[i],
					zoomValue = zoomValue_Values[i],
					posX = posX_Values[i],
					posY = posY_Values[i]
				};
				s_Datas.Add(data);
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CharZoomPos GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CharZoomPos GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CollSounds : XmlDataBase
	{
		private int index;

		private String key;

		private int data_type;

		private String sound_id;

		private String record_id;

		private String textlist_id;

		private int sound_ctg;

		public const string DataName = "CollSounds";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CollSounds> s_Datas = new List<CollSounds>();

		private static SortedDictionary<string, CollSounds> s_Map = new SortedDictionary<string, CollSounds>();

		private static SortedDictionary<int, CollSounds> s_SwitchIdxMap = new SortedDictionary<int, CollSounds>();

		public int m_iIdx => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iDataType => data_type;

		public string m_strIDSnd => sound_id.Get(s_posStringInStream);

		public string m_strIDrecord => record_id.Get(s_posStringInStream);

		public string m_strIDtext => textlist_id.Get(s_posStringInStream);

		public int m_iCategory => sound_ctg;

		public static List<CollSounds> datas => s_Datas;

		public static SortedDictionary<string, CollSounds> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] data_type_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, data_type_Values, 0, dataSize);
			int[] sound_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, sound_id_Values, 0, dataSize);
			int[] record_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, record_id_Values, 0, dataSize);
			int[] textlist_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, textlist_id_Values, 0, dataSize);
			int[] sound_ctg_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, sound_ctg_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CollSounds data = new CollSounds
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					data_type = data_type_Values[i],
					sound_id = 
					{
						pos = sound_id_Values[i],
						strCache = null
					},
					record_id = 
					{
						pos = record_id_Values[i],
						strCache = null
					},
					textlist_id = 
					{
						pos = textlist_id_Values[i],
						strCache = null
					},
					sound_ctg = sound_ctg_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CollSounds GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CollSounds GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CollSounds GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CollImages : XmlDataBase
	{
		private int index;

		private String key;

		private int image_ctg;

		private String img_id;

		private String textlist_id;

		private String colImageDest_id;

		public const string DataName = "CollImages";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CollImages> s_Datas = new List<CollImages>();

		private static SortedDictionary<string, CollImages> s_Map = new SortedDictionary<string, CollImages>();

		private static SortedDictionary<int, CollImages> s_SwitchIdxMap = new SortedDictionary<int, CollImages>();

		public int m_iIdx => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iCategory => image_ctg;

		public string m_strIDImg => img_id.Get(s_posStringInStream);

		public string m_strIDtext => textlist_id.Get(s_posStringInStream);

		public string m_strIDColImageDest => colImageDest_id.Get(s_posStringInStream);

		public static List<CollImages> datas => s_Datas;

		public static SortedDictionary<string, CollImages> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] image_ctg_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, image_ctg_Values, 0, dataSize);
			int[] img_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, img_id_Values, 0, dataSize);
			int[] textlist_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, textlist_id_Values, 0, dataSize);
			int[] colImageDest_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, colImageDest_id_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CollImages data = new CollImages
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					image_ctg = image_ctg_Values[i],
					img_id = 
					{
						pos = img_id_Values[i],
						strCache = null
					},
					textlist_id = 
					{
						pos = textlist_id_Values[i],
						strCache = null
					},
					colImageDest_id = 
					{
						pos = colImageDest_id_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CollImages GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CollImages GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CollImages GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ColImageDescListData : XmlDataBase
	{
		private String id_ColImageDesc;

		private String ColImageDescText;

		public const string DataName = "ColImageDescListData";

		public const bool IsUseKey = false;

		private static int s_posStringInStream = 0;

		private static List<ColImageDescListData> s_Datas = new List<ColImageDescListData>();

		public string m_strColImageDescID => id_ColImageDesc.Get(s_posStringInStream);

		public string m_strColImageDescText => ColImageDescText.Get(s_posStringInStream);

		public static List<ColImageDescListData> datas => s_Datas;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] id_ColImageDesc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_ColImageDesc_Values, 0, dataSize);
			int[] ColImageDescText_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ColImageDescText_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ColImageDescListData data = new ColImageDescListData
				{
					id_ColImageDesc = 
					{
						pos = id_ColImageDesc_Values[i],
						strCache = null
					},
					ColImageDescText = 
					{
						pos = ColImageDescText_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
		}

		public static ColImageDescListData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class CollKeyword : XmlDataBase
	{
		private int index;

		private String key;

		private String icon_id;

		private int ctg;

		private int seq_idx;

		private String key_name;

		private String chkRelation;

		private String img_id;

		private String text;

		private String min_t;

		private String seo_t;

		private String oh_t;

		private String lee_t;

		private String chang_t;

		private String ha_t;

		private String min_m;

		private String seo_m;

		private String oh_m;

		private String lee_m;

		private String change_m;

		private String ha_m;

		private String min_f_t;

		private String seo_f_t;

		private String oh_f_t;

		private String lee_f_t;

		private String change_f_t;

		private String ha_f_t;

		private String min_r_t;

		private String seo_r_t;

		private String oh_r_t;

		private String lee_r_t;

		private String change_r_t;

		private String req_h;

		private String req_kw;

		public const string DataName = "CollKeyword";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<CollKeyword> s_Datas = new List<CollKeyword>();

		private static SortedDictionary<string, CollKeyword> s_Map = new SortedDictionary<string, CollKeyword>();

		private static SortedDictionary<int, CollKeyword> s_SwitchIdxMap = new SortedDictionary<int, CollKeyword>();

		public int m_iIndex => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strIconImgID => icon_id.Get(s_posStringInStream);

		public int m_iCtg => ctg;

		public int m_Sequence => seq_idx;

		public string m_strTitleID => key_name.Get(s_posStringInStream);

		public string m_strChkRelation => chkRelation.Get(s_posStringInStream);

		public string m_strImgID => img_id.Get(s_posStringInStream);

		public string m_strTextID => text.Get(s_posStringInStream);

		public string m_strTalkMin => min_t.Get(s_posStringInStream);

		public string m_strTalkSeo => seo_t.Get(s_posStringInStream);

		public string m_strTalkOh => oh_t.Get(s_posStringInStream);

		public string m_strTalkLee => lee_t.Get(s_posStringInStream);

		public string m_strTalkChang => chang_t.Get(s_posStringInStream);

		public string m_strTalkHa => ha_t.Get(s_posStringInStream);

		public string m_strFMotionMin => min_m.Get(s_posStringInStream);

		public string m_strFMotionSeo => seo_m.Get(s_posStringInStream);

		public string m_strFMotionOh => oh_m.Get(s_posStringInStream);

		public string m_strFMotionLee => lee_m.Get(s_posStringInStream);

		public string m_strFMotionChang => change_m.Get(s_posStringInStream);

		public string m_strFMotionHa => ha_m.Get(s_posStringInStream);

		public string m_strFTalkMin => min_f_t.Get(s_posStringInStream);

		public string m_strFTalkSeo => seo_f_t.Get(s_posStringInStream);

		public string m_strFTalkOh => oh_f_t.Get(s_posStringInStream);

		public string m_strFTalkLee => lee_f_t.Get(s_posStringInStream);

		public string m_strFTalkChang => change_f_t.Get(s_posStringInStream);

		public string m_strFTalkHa => ha_f_t.Get(s_posStringInStream);

		public string m_strReactionMin => min_r_t.Get(s_posStringInStream);

		public string m_strReactionSeo => seo_r_t.Get(s_posStringInStream);

		public string m_strReactionOh => oh_r_t.Get(s_posStringInStream);

		public string m_strReactionLee => lee_r_t.Get(s_posStringInStream);

		public string m_strReactionChang => change_r_t.Get(s_posStringInStream);

		public string m_strRequiredHint => req_h.Get(s_posStringInStream);

		public string m_strRequiredkw => req_kw.Get(s_posStringInStream);

		public static List<CollKeyword> datas => s_Datas;

		public static SortedDictionary<string, CollKeyword> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] icon_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, icon_id_Values, 0, dataSize);
			int[] ctg_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ctg_Values, 0, dataSize);
			int[] seq_idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seq_idx_Values, 0, dataSize);
			int[] key_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_name_Values, 0, dataSize);
			int[] chkRelation_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, chkRelation_Values, 0, dataSize);
			int[] img_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, img_id_Values, 0, dataSize);
			int[] text_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_Values, 0, dataSize);
			int[] min_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, min_t_Values, 0, dataSize);
			int[] seo_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seo_t_Values, 0, dataSize);
			int[] oh_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, oh_t_Values, 0, dataSize);
			int[] lee_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lee_t_Values, 0, dataSize);
			int[] chang_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, chang_t_Values, 0, dataSize);
			int[] ha_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ha_t_Values, 0, dataSize);
			int[] min_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, min_m_Values, 0, dataSize);
			int[] seo_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seo_m_Values, 0, dataSize);
			int[] oh_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, oh_m_Values, 0, dataSize);
			int[] lee_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lee_m_Values, 0, dataSize);
			int[] change_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, change_m_Values, 0, dataSize);
			int[] ha_m_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ha_m_Values, 0, dataSize);
			int[] min_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, min_f_t_Values, 0, dataSize);
			int[] seo_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seo_f_t_Values, 0, dataSize);
			int[] oh_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, oh_f_t_Values, 0, dataSize);
			int[] lee_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lee_f_t_Values, 0, dataSize);
			int[] change_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, change_f_t_Values, 0, dataSize);
			int[] ha_f_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ha_f_t_Values, 0, dataSize);
			int[] min_r_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, min_r_t_Values, 0, dataSize);
			int[] seo_r_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seo_r_t_Values, 0, dataSize);
			int[] oh_r_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, oh_r_t_Values, 0, dataSize);
			int[] lee_r_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lee_r_t_Values, 0, dataSize);
			int[] change_r_t_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, change_r_t_Values, 0, dataSize);
			int[] req_h_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, req_h_Values, 0, dataSize);
			int[] req_kw_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, req_kw_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				CollKeyword data = new CollKeyword
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					icon_id = 
					{
						pos = icon_id_Values[i],
						strCache = null
					},
					ctg = ctg_Values[i],
					seq_idx = seq_idx_Values[i],
					key_name = 
					{
						pos = key_name_Values[i],
						strCache = null
					},
					chkRelation = 
					{
						pos = chkRelation_Values[i],
						strCache = null
					},
					img_id = 
					{
						pos = img_id_Values[i],
						strCache = null
					},
					text = 
					{
						pos = text_Values[i],
						strCache = null
					},
					min_t = 
					{
						pos = min_t_Values[i],
						strCache = null
					},
					seo_t = 
					{
						pos = seo_t_Values[i],
						strCache = null
					},
					oh_t = 
					{
						pos = oh_t_Values[i],
						strCache = null
					},
					lee_t = 
					{
						pos = lee_t_Values[i],
						strCache = null
					},
					chang_t = 
					{
						pos = chang_t_Values[i],
						strCache = null
					},
					ha_t = 
					{
						pos = ha_t_Values[i],
						strCache = null
					},
					min_m = 
					{
						pos = min_m_Values[i],
						strCache = null
					},
					seo_m = 
					{
						pos = seo_m_Values[i],
						strCache = null
					},
					oh_m = 
					{
						pos = oh_m_Values[i],
						strCache = null
					},
					lee_m = 
					{
						pos = lee_m_Values[i],
						strCache = null
					},
					change_m = 
					{
						pos = change_m_Values[i],
						strCache = null
					},
					ha_m = 
					{
						pos = ha_m_Values[i],
						strCache = null
					},
					min_f_t = 
					{
						pos = min_f_t_Values[i],
						strCache = null
					},
					seo_f_t = 
					{
						pos = seo_f_t_Values[i],
						strCache = null
					},
					oh_f_t = 
					{
						pos = oh_f_t_Values[i],
						strCache = null
					},
					lee_f_t = 
					{
						pos = lee_f_t_Values[i],
						strCache = null
					},
					change_f_t = 
					{
						pos = change_f_t_Values[i],
						strCache = null
					},
					ha_f_t = 
					{
						pos = ha_f_t_Values[i],
						strCache = null
					},
					min_r_t = 
					{
						pos = min_r_t_Values[i],
						strCache = null
					},
					seo_r_t = 
					{
						pos = seo_r_t_Values[i],
						strCache = null
					},
					oh_r_t = 
					{
						pos = oh_r_t_Values[i],
						strCache = null
					},
					lee_r_t = 
					{
						pos = lee_r_t_Values[i],
						strCache = null
					},
					change_r_t = 
					{
						pos = change_r_t_Values[i],
						strCache = null
					},
					req_h = 
					{
						pos = req_h_Values[i],
						strCache = null
					},
					req_kw = 
					{
						pos = req_kw_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static CollKeyword GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static CollKeyword GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static CollKeyword GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class Trophys : XmlDataBase
	{
		private int index;

		private String key;

		private int ctg;

		private int ps_tropyIDX;

		private String steam_tropyIDX;

		private String android_AchvID;

		private String ios_AchvID;

		private String trophy_name;

		private int trophy_goal;

		private int trophy_max;

		public const string DataName = "Trophys";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<Trophys> s_Datas = new List<Trophys>();

		private static SortedDictionary<string, Trophys> s_Map = new SortedDictionary<string, Trophys>();

		private static SortedDictionary<int, Trophys> s_SwitchIdxMap = new SortedDictionary<int, Trophys>();

		public int m_iIndex => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iCategory => ctg;

		public int m_iPSTrpIdx => ps_tropyIDX;

		public string m_isteamTrpIdx => steam_tropyIDX.Get(s_posStringInStream);

		public string m_strAndroidAchvId => android_AchvID.Get(s_posStringInStream);

		public string m_strIOSAchvId => ios_AchvID.Get(s_posStringInStream);

		public string m_strName => trophy_name.Get(s_posStringInStream);

		public int m_iGoal => trophy_goal;

		public int m_iMax => trophy_max;

		public static List<Trophys> datas => s_Datas;

		public static SortedDictionary<string, Trophys> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] ctg_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ctg_Values, 0, dataSize);
			int[] ps_tropyIDX_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ps_tropyIDX_Values, 0, dataSize);
			int[] steam_tropyIDX_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, steam_tropyIDX_Values, 0, dataSize);
			int[] android_AchvID_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, android_AchvID_Values, 0, dataSize);
			int[] ios_AchvID_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ios_AchvID_Values, 0, dataSize);
			int[] trophy_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, trophy_name_Values, 0, dataSize);
			int[] trophy_goal_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, trophy_goal_Values, 0, dataSize);
			int[] trophy_max_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, trophy_max_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				Trophys data = new Trophys
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					ctg = ctg_Values[i],
					ps_tropyIDX = ps_tropyIDX_Values[i],
					steam_tropyIDX = 
					{
						pos = steam_tropyIDX_Values[i],
						strCache = null
					},
					android_AchvID = 
					{
						pos = android_AchvID_Values[i],
						strCache = null
					},
					ios_AchvID = 
					{
						pos = ios_AchvID_Values[i],
						strCache = null
					},
					trophy_name = 
					{
						pos = trophy_name_Values[i],
						strCache = null
					},
					trophy_goal = trophy_goal_Values[i],
					trophy_max = trophy_max_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static Trophys GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static Trophys GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static Trophys GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class Profiles : XmlDataBase
	{
		private int index;

		private String key;

		private String char_txt_key;

		private int ctg;

		private String key_name;

		private String img_id;

		public const string DataName = "Profiles";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<Profiles> s_Datas = new List<Profiles>();

		private static SortedDictionary<string, Profiles> s_Map = new SortedDictionary<string, Profiles>();

		private static SortedDictionary<int, Profiles> s_SwitchIdxMap = new SortedDictionary<int, Profiles>();

		public int m_iIdx => index;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strChrTxtKey => char_txt_key.Get(s_posStringInStream);

		public int m_iCtgIdx => ctg;

		public string m_strName => key_name.Get(s_posStringInStream);

		public string m_strIDImg => img_id.Get(s_posStringInStream);

		public static List<Profiles> datas => s_Datas;

		public static SortedDictionary<string, Profiles> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] index_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, index_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] char_txt_key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, char_txt_key_Values, 0, dataSize);
			int[] ctg_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, ctg_Values, 0, dataSize);
			int[] key_name_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_name_Values, 0, dataSize);
			int[] img_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, img_id_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				Profiles data = new Profiles
				{
					index = index_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					char_txt_key = 
					{
						pos = char_txt_key_Values[i],
						strCache = null
					},
					ctg = ctg_Values[i],
					key_name = 
					{
						pos = key_name_Values[i],
						strCache = null
					},
					img_id = 
					{
						pos = img_id_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.index;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static Profiles GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static Profiles GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static Profiles GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TalkCutSetting : XmlDataBase
	{
		private int idx;

		private String key;

		private String IDCutName;

		private String tip_sns;

		private String conti_prefix;

		private String han_pos;

		private String min_pos;

		private String seo_pos;

		private String oh_pos;

		private String lee_pos;

		private String chang_pos;

		private String keyword_set;

		public const string DataName = "TalkCutSetting";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<TalkCutSetting> s_Datas = new List<TalkCutSetting>();

		private static SortedDictionary<string, TalkCutSetting> s_Map = new SortedDictionary<string, TalkCutSetting>();

		private static SortedDictionary<int, TalkCutSetting> s_SwitchIdxMap = new SortedDictionary<int, TalkCutSetting>();

		public int m_iIdx => idx;

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strIDCutName => IDCutName.Get(s_posStringInStream);

		public string m_strSNSID => tip_sns.Get(s_posStringInStream);

		public string m_strContiPreName => conti_prefix.Get(s_posStringInStream);

		public string m_strHanPos => han_pos.Get(s_posStringInStream);

		public string m_strMinPos => min_pos.Get(s_posStringInStream);

		public string m_strSeoPos => seo_pos.Get(s_posStringInStream);

		public string m_strOhPos => oh_pos.Get(s_posStringInStream);

		public string m_strLeePos => lee_pos.Get(s_posStringInStream);

		public string m_strChangPos => chang_pos.Get(s_posStringInStream);

		public string m_strKeywordSet => keyword_set.Get(s_posStringInStream);

		public static List<TalkCutSetting> datas => s_Datas;

		public static SortedDictionary<string, TalkCutSetting> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, idx_Values, 0, dataSize);
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] IDCutName_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, IDCutName_Values, 0, dataSize);
			int[] tip_sns_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, tip_sns_Values, 0, dataSize);
			int[] conti_prefix_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, conti_prefix_Values, 0, dataSize);
			int[] han_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, han_pos_Values, 0, dataSize);
			int[] min_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, min_pos_Values, 0, dataSize);
			int[] seo_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, seo_pos_Values, 0, dataSize);
			int[] oh_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, oh_pos_Values, 0, dataSize);
			int[] lee_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, lee_pos_Values, 0, dataSize);
			int[] chang_pos_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, chang_pos_Values, 0, dataSize);
			int[] keyword_set_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, keyword_set_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TalkCutSetting data = new TalkCutSetting
				{
					idx = idx_Values[i],
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					IDCutName = 
					{
						pos = IDCutName_Values[i],
						strCache = null
					},
					tip_sns = 
					{
						pos = tip_sns_Values[i],
						strCache = null
					},
					conti_prefix = 
					{
						pos = conti_prefix_Values[i],
						strCache = null
					},
					han_pos = 
					{
						pos = han_pos_Values[i],
						strCache = null
					},
					min_pos = 
					{
						pos = min_pos_Values[i],
						strCache = null
					},
					seo_pos = 
					{
						pos = seo_pos_Values[i],
						strCache = null
					},
					oh_pos = 
					{
						pos = oh_pos_Values[i],
						strCache = null
					},
					lee_pos = 
					{
						pos = lee_pos_Values[i],
						strCache = null
					},
					chang_pos = 
					{
						pos = chang_pos_Values[i],
						strCache = null
					},
					keyword_set = 
					{
						pos = keyword_set_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				int switchIdx = data.idx;
				if (!s_SwitchIdxMap.ContainsKey(switchIdx))
				{
					s_SwitchIdxMap.Add(switchIdx, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
			s_SwitchIdxMap.Clear();
		}

		public static TalkCutSetting GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static TalkCutSetting GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static TalkCutSetting GetData_bySwitchIdx(int switchIdx)
		{
			if (s_SwitchIdxMap.ContainsKey(switchIdx))
			{
				return s_SwitchIdxMap[switchIdx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TalkCutChrSetting : XmlDataBase
	{
		private String key;

		private int TalkCursorVisible;

		private int size;

		private String motion;

		private int direction;

		private float posX;

		private float posY;

		private float posY_M;

		private float posX_M;

		public const string DataName = "TalkCutChrSetting";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<TalkCutChrSetting> s_Datas = new List<TalkCutChrSetting>();

		private static SortedDictionary<string, TalkCutChrSetting> s_Map = new SortedDictionary<string, TalkCutChrSetting>();

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iIconShow => TalkCursorVisible;

		public int m_iSize => size;

		public string m_strMotion => motion.Get(s_posStringInStream);

		public int m_iDir => direction;

		public float m_fPosX => posX;

		public float m_fPosY => posY;

		public float m_fPosY_Keyword => posY_M;

		public float m_fPosX_Keyword => posX_M;

		public static List<TalkCutChrSetting> datas => s_Datas;

		public static SortedDictionary<string, TalkCutChrSetting> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] TalkCursorVisible_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, TalkCursorVisible_Values, 0, dataSize);
			int[] size_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, size_Values, 0, dataSize);
			int[] motion_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, motion_Values, 0, dataSize);
			int[] direction_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, direction_Values, 0, dataSize);
			float[] posX_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posX_Values, 0, dataSize);
			float[] posY_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posY_Values, 0, dataSize);
			float[] posY_M_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posY_M_Values, 0, dataSize);
			float[] posX_M_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, posX_M_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TalkCutChrSetting data = new TalkCutChrSetting
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					TalkCursorVisible = TalkCursorVisible_Values[i],
					size = size_Values[i],
					motion = 
					{
						pos = motion_Values[i],
						strCache = null
					},
					direction = direction_Values[i],
					posX = posX_Values[i],
					posY = posY_Values[i],
					posY_M = posY_M_Values[i],
					posX_M = posX_M_Values[i]
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static TalkCutChrSetting GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static TalkCutChrSetting GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class KeywordUsing : XmlDataBase
	{
		private String key;

		private int HeaderTextType;

		private String text_id;

		private String keyword_set;

		public const string DataName = "KeywordUsing";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<KeywordUsing> s_Datas = new List<KeywordUsing>();

		private static SortedDictionary<string, KeywordUsing> s_Map = new SortedDictionary<string, KeywordUsing>();

		public string m_strKey => key.Get(s_posStringInStream);

		public int m_iTitleTextType => HeaderTextType;

		public string m_strQuestTextID => text_id.Get(s_posStringInStream);

		public string m_strKeywordSet => keyword_set.Get(s_posStringInStream);

		public static List<KeywordUsing> datas => s_Datas;

		public static SortedDictionary<string, KeywordUsing> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] HeaderTextType_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, HeaderTextType_Values, 0, dataSize);
			int[] text_id_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_id_Values, 0, dataSize);
			int[] keyword_set_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, keyword_set_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				KeywordUsing data = new KeywordUsing
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					HeaderTextType = HeaderTextType_Values[i],
					text_id = 
					{
						pos = text_id_Values[i],
						strCache = null
					},
					keyword_set = 
					{
						pos = keyword_set_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static KeywordUsing GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static KeywordUsing GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TextData : XmlDataBase
	{
		private String key;

		private String text_ko;

		private String text_en;

		private String text_jp;

		private String text_sc;

		private String text_tc;

		public const string DataName = "TextData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<TextData> s_Datas = new List<TextData>();

		private static SortedDictionary<string, TextData> s_Map = new SortedDictionary<string, TextData>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTxtKo => text_ko.Get(s_posStringInStream);

		public string m_strTxtEn => text_en.Get(s_posStringInStream);

		public string m_strTxtJp => text_jp.Get(s_posStringInStream);

		public string m_strTxtSc => text_sc.Get(s_posStringInStream);

		public string m_strTxtTc => text_tc.Get(s_posStringInStream);

		public string m_strTxt => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTxtKo, 
			SystemLanguage.English => m_strTxtEn, 
			SystemLanguage.Japanese => m_strTxtJp, 
			SystemLanguage.ChineseSimplified => m_strTxtSc, 
			SystemLanguage.ChineseTraditional => m_strTxtTc, 
			_ => m_strTxtKo, 
		};

		public static List<TextData> datas => s_Datas;

		public static SortedDictionary<string, TextData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ko_Values, 0, dataSize);
			int[] text_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_en_Values, 0, dataSize);
			int[] text_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_jp_Values, 0, dataSize);
			int[] text_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_sc_Values, 0, dataSize);
			int[] text_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_tc_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TextData data = new TextData
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_ko = 
					{
						pos = text_ko_Values[i],
						strCache = null
					},
					text_en = 
					{
						pos = text_en_Values[i],
						strCache = null
					},
					text_jp = 
					{
						pos = text_jp_Values[i],
						strCache = null
					},
					text_sc = 
					{
						pos = text_sc_Values[i],
						strCache = null
					},
					text_tc = 
					{
						pos = text_tc_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static TextData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static TextData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TextListData : XmlDataBase
	{
		private String key;

		private String title_ko;

		private String text_ko;

		private String text_en;

		private String title_en;

		private String text_jp;

		private String title_jp;

		private String text_sc;

		private String title_sc;

		private String text_tc;

		private String title_tc;

		public const string DataName = "TextListData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<TextListData> s_Datas = new List<TextListData>();

		private static SortedDictionary<string, TextListData> s_Map = new SortedDictionary<string, TextListData>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTitleKo => title_ko.Get(s_posStringInStream);

		public string m_strTxtKo => text_ko.Get(s_posStringInStream);

		public string m_strTxtEn => text_en.Get(s_posStringInStream);

		public string m_strTitleEn => title_en.Get(s_posStringInStream);

		public string m_strTxtJp => text_jp.Get(s_posStringInStream);

		public string m_strTitleJp => title_jp.Get(s_posStringInStream);

		public string m_strTxtSc => text_sc.Get(s_posStringInStream);

		public string m_strTitleSc => title_sc.Get(s_posStringInStream);

		public string m_strTxtTc => text_tc.Get(s_posStringInStream);

		public string m_strTitleTc => title_tc.Get(s_posStringInStream);

		public string m_strTitle => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTitleKo, 
			SystemLanguage.English => m_strTitleEn, 
			SystemLanguage.Japanese => m_strTitleJp, 
			SystemLanguage.ChineseSimplified => m_strTitleSc, 
			SystemLanguage.ChineseTraditional => m_strTitleTc, 
			_ => m_strTitleKo, 
		};

		public string m_strText => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTxtKo, 
			SystemLanguage.English => m_strTxtEn, 
			SystemLanguage.Japanese => m_strTxtJp, 
			SystemLanguage.ChineseSimplified => m_strTxtSc, 
			SystemLanguage.ChineseTraditional => m_strTxtTc, 
			_ => m_strTxtKo, 
		};

		public static List<TextListData> datas => s_Datas;

		public static SortedDictionary<string, TextListData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] title_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_ko_Values, 0, dataSize);
			int[] text_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ko_Values, 0, dataSize);
			int[] text_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_en_Values, 0, dataSize);
			int[] title_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_en_Values, 0, dataSize);
			int[] text_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_jp_Values, 0, dataSize);
			int[] title_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_jp_Values, 0, dataSize);
			int[] text_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_sc_Values, 0, dataSize);
			int[] title_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_sc_Values, 0, dataSize);
			int[] text_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_tc_Values, 0, dataSize);
			int[] title_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_tc_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TextListData data = new TextListData
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					title_ko = 
					{
						pos = title_ko_Values[i],
						strCache = null
					},
					text_ko = 
					{
						pos = text_ko_Values[i],
						strCache = null
					},
					text_en = 
					{
						pos = text_en_Values[i],
						strCache = null
					},
					title_en = 
					{
						pos = title_en_Values[i],
						strCache = null
					},
					text_jp = 
					{
						pos = text_jp_Values[i],
						strCache = null
					},
					title_jp = 
					{
						pos = title_jp_Values[i],
						strCache = null
					},
					text_sc = 
					{
						pos = text_sc_Values[i],
						strCache = null
					},
					title_sc = 
					{
						pos = title_sc_Values[i],
						strCache = null
					},
					text_tc = 
					{
						pos = text_tc_Values[i],
						strCache = null
					},
					title_tc = 
					{
						pos = title_tc_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static TextListData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static TextListData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TutorialData : XmlDataBase
	{
		private String id_tutorial;

		private String id_text_list;

		private String id_img;

		public const string DataName = "TutorialData";

		public const bool IsUseKey = false;

		private static int s_posStringInStream = 0;

		private static List<TutorialData> s_Datas = new List<TutorialData>();

		public string m_strTutorialID => id_tutorial.Get(s_posStringInStream);

		public string m_strTextKey => id_text_list.Get(s_posStringInStream);

		public string m_strImgKey => id_img.Get(s_posStringInStream);

		public static List<TutorialData> datas => s_Datas;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] id_tutorial_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_tutorial_Values, 0, dataSize);
			int[] id_text_list_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_text_list_Values, 0, dataSize);
			int[] id_img_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_img_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TutorialData data = new TutorialData
				{
					id_tutorial = 
					{
						pos = id_tutorial_Values[i],
						strCache = null
					},
					id_text_list = 
					{
						pos = id_text_list_Values[i],
						strCache = null
					},
					id_img = 
					{
						pos = id_img_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
		}

		public static TutorialData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class TutorialListData : XmlDataBase
	{
		private int idx;

		private String id_tutorial;

		private float delay;

		public const string DataName = "TutorialListData";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<TutorialListData> s_Datas = new List<TutorialListData>();

		private static SortedDictionary<string, TutorialListData> s_Map = new SortedDictionary<string, TutorialListData>();

		public int m_iTutorialIdx => idx;

		public string m_strTutorialID => id_tutorial.Get(s_posStringInStream);

		public float m_fDelay => delay;

		public static List<TutorialListData> datas => s_Datas;

		public static SortedDictionary<string, TutorialListData> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] idx_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, idx_Values, 0, dataSize);
			int[] id_tutorial_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, id_tutorial_Values, 0, dataSize);
			float[] delay_Values = new float[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, delay_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				TutorialListData data = new TutorialListData
				{
					idx = idx_Values[i],
					id_tutorial = 
					{
						pos = id_tutorial_Values[i],
						strCache = null
					},
					delay = delay_Values[i]
				};
				s_Datas.Add(data);
				string key = data.id_tutorial.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static TutorialListData GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static TutorialListData GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ProgramDefineStr : XmlDataBase
	{
		private String key;

		private String text_ko;

		public const string DataName = "ProgramDefineStr";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ProgramDefineStr> s_Datas = new List<ProgramDefineStr>();

		private static SortedDictionary<string, ProgramDefineStr> s_Map = new SortedDictionary<string, ProgramDefineStr>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTxt => text_ko.Get(s_posStringInStream);

		public static List<ProgramDefineStr> datas => s_Datas;

		public static SortedDictionary<string, ProgramDefineStr> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ko_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ProgramDefineStr data = new ProgramDefineStr
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_ko = 
					{
						pos = text_ko_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ProgramDefineStr GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ProgramDefineStr GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ProgramText : XmlDataBase
	{
		private String key;

		private String title_ko;

		private String text_en;

		private String text_jp;

		private String text_sc;

		private String text_tc;

		public const string DataName = "ProgramText";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ProgramText> s_Datas = new List<ProgramText>();

		private static SortedDictionary<string, ProgramText> s_Map = new SortedDictionary<string, ProgramText>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTxtKo => title_ko.Get(s_posStringInStream);

		public string m_strTxtEn => text_en.Get(s_posStringInStream);

		public string m_strTxtJp => text_jp.Get(s_posStringInStream);

		public string m_strTxtSc => text_sc.Get(s_posStringInStream);

		public string m_strTxtTc => text_tc.Get(s_posStringInStream);

		public string m_strTxt => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTxtKo, 
			SystemLanguage.English => m_strTxtEn, 
			SystemLanguage.Japanese => m_strTxtJp, 
			SystemLanguage.ChineseSimplified => m_strTxtSc, 
			SystemLanguage.ChineseTraditional => m_strTxtTc, 
			_ => m_strTxtKo, 
		};

		public static List<ProgramText> datas => s_Datas;

		public static SortedDictionary<string, ProgramText> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] title_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, title_ko_Values, 0, dataSize);
			int[] text_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_en_Values, 0, dataSize);
			int[] text_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_jp_Values, 0, dataSize);
			int[] text_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_sc_Values, 0, dataSize);
			int[] text_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_tc_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ProgramText data = new ProgramText
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					title_ko = 
					{
						pos = title_ko_Values[i],
						strCache = null
					},
					text_en = 
					{
						pos = text_en_Values[i],
						strCache = null
					},
					text_jp = 
					{
						pos = text_jp_Values[i],
						strCache = null
					},
					text_sc = 
					{
						pos = text_sc_Values[i],
						strCache = null
					},
					text_tc = 
					{
						pos = text_tc_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ProgramText GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ProgramText GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	public class ProgramGlobalText : XmlDataBase
	{
		private String key;

		private String text_ko;

		private String text_en;

		private String text_jp;

		private String text_ch_sc;

		private String text_ch_tc;

		public const string DataName = "ProgramGlobalText";

		public const bool IsUseKey = true;

		public static readonly Type KeyType = typeof(string);

		private static int s_posStringInStream = 0;

		private static List<ProgramGlobalText> s_Datas = new List<ProgramGlobalText>();

		private static SortedDictionary<string, ProgramGlobalText> s_Map = new SortedDictionary<string, ProgramGlobalText>();

		public string m_strKey => key.Get(s_posStringInStream);

		public string m_strTxtKo => text_ko.Get(s_posStringInStream);

		public string m_strTxtEn => text_en.Get(s_posStringInStream);

		public string m_strTxtJp => text_jp.Get(s_posStringInStream);

		public string m_strTxtChSc => text_ch_sc.Get(s_posStringInStream);

		public string m_strTxtChTc => text_ch_tc.Get(s_posStringInStream);

		public string m_strTxt => XmlDataBase.s_curLang switch
		{
			SystemLanguage.Korean => m_strTxtKo, 
			SystemLanguage.English => m_strTxtEn, 
			SystemLanguage.Japanese => m_strTxtJp, 
			SystemLanguage.ChineseSimplified => m_strTxtChSc, 
			SystemLanguage.ChineseTraditional => m_strTxtChTc, 
			_ => m_strTxtKo, 
		};

		public static List<ProgramGlobalText> datas => s_Datas;

		public static SortedDictionary<string, ProgramGlobalText> sortedDic => s_Map;

		public static IEnumerator Load(Stream stream)
		{
			stream.Read(s_byteBuffer, 0, 4);
			int dataCount = BitConverter.ToInt32(s_byteBuffer, 0);
			stream.Read(s_byteBuffer, 0, 4);
			s_posStringInStream = BitConverter.ToInt32(s_byteBuffer, 0);
			s_posStringInStream += (int)s_posDataInStream;
			int dataSize = 0;
			int bufferMaxSize = 8 * dataCount;
			if (s_byteBuffer.Length < bufferMaxSize)
			{
				s_byteBuffer = new byte[bufferMaxSize];
			}
			int[] key_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, key_Values, 0, dataSize);
			int[] text_ko_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ko_Values, 0, dataSize);
			int[] text_en_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_en_Values, 0, dataSize);
			int[] text_jp_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_jp_Values, 0, dataSize);
			int[] text_ch_sc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ch_sc_Values, 0, dataSize);
			int[] text_ch_tc_Values = new int[dataCount];
			dataSize = 4 * dataCount;
			stream.Read(s_byteBuffer, 0, dataSize);
			Buffer.BlockCopy(s_byteBuffer, 0, text_ch_tc_Values, 0, dataSize);
			int i = 0;
			int j = 0;
			while (i < dataCount)
			{
				ProgramGlobalText data = new ProgramGlobalText
				{
					key = 
					{
						pos = key_Values[i],
						strCache = null
					},
					text_ko = 
					{
						pos = text_ko_Values[i],
						strCache = null
					},
					text_en = 
					{
						pos = text_en_Values[i],
						strCache = null
					},
					text_jp = 
					{
						pos = text_jp_Values[i],
						strCache = null
					},
					text_ch_sc = 
					{
						pos = text_ch_sc_Values[i],
						strCache = null
					},
					text_ch_tc = 
					{
						pos = text_ch_tc_Values[i],
						strCache = null
					}
				};
				s_Datas.Add(data);
				string key = data.key.Get(s_posStringInStream);
				if (!s_Map.ContainsKey(key))
				{
					s_Map.Add(key, data);
				}
				if (j >= 1000)
				{
					j = 0;
					yield return null;
				}
				i++;
				j++;
			}
		}

		public static void Clear()
		{
			s_Datas.Clear();
			s_Map.Clear();
		}

		public static ProgramGlobalText GetData_byIdx(int idx)
		{
			if (idx >= 0 && idx < s_Datas.Count)
			{
				return s_Datas[idx];
			}
			return null;
		}

		public static ProgramGlobalText GetData_byKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			if (s_Map.ContainsKey(key))
			{
				return s_Map[key];
			}
			return null;
		}

		public static int GetDataCount()
		{
			return s_Datas.Count;
		}
	}

	private static MemoryStream s_Stream = null;

	private static long s_posDataInStream = 0L;

	private static byte[] s_byteBuffer = new byte[1024];

	public static MemoryStream Stream
	{
		get
		{
			return s_Stream;
		}
		set
		{
			s_Stream = value;
		}
	}

	public static long PosDataInStream
	{
		get
		{
			return s_posDataInStream;
		}
		set
		{
			s_posDataInStream = value;
		}
	}

	public static LoadFP GetLoadFunc(string className)
	{
		return className switch
		{
			"VideoFile" => VideoFile.Load, 
			"UISound" => UISound.Load, 
			"SoundFile" => SoundFile.Load, 
			"SelectData" => SelectData.Load, 
			"SpeechData" => ScriptSpeechData.Load, 
			"StaffRoll" => StaffRoll.Load, 
			"InGameTime" => InGameTime.Load, 
			"AccountData" => AccountData.Load, 
			"MessengerChatroomData" => MessengerChatroomData.Load, 
			"MessengerTalkData" => MessengerTalkData.Load, 
			"WatchFaterAutoText" => WatchFaterAutoText.Load, 
			"CharData_ForProfile" => CharData_ForProfile.Load, 
			"SNSPostData" => SNSPostData.Load, 
			"SNSReplyData" => SNSReplyData.Load, 
			"SequenceData" => SequenceData.Load, 
			"Voice" => Voice.Load, 
			"ImageFile" => ImageFile.Load, 
			"ScriptKeyValue" => ScriptKeyValue.Load, 
			"ScriptKeyTextValue" => ScriptKeyTextValue.Load, 
			"CameraShakeValue" => CameraShakeValue.Load, 
			"CharRelationEvt" => CharRelationEvt.Load, 
			"CharMotData" => CharMotData.Load, 
			"CharData" => CharData.Load, 
			"CharZoomPos" => CharZoomPos.Load, 
			"CollSounds" => CollSounds.Load, 
			"CollImages" => CollImages.Load, 
			"ColImageDescListData" => ColImageDescListData.Load, 
			"CollKeyword" => CollKeyword.Load, 
			"Trophys" => Trophys.Load, 
			"Profiles" => Profiles.Load, 
			"TalkCutSetting" => TalkCutSetting.Load, 
			"TalkCutChrSetting" => TalkCutChrSetting.Load, 
			"KeywordUsing" => KeywordUsing.Load, 
			"TextData" => TextData.Load, 
			"TextListData" => TextListData.Load, 
			"TutorialData" => TutorialData.Load, 
			"TutorialListData" => TutorialListData.Load, 
			"ProgramDefineStr" => ProgramDefineStr.Load, 
			"ProgramText" => ProgramText.Load, 
			"ProgramGlobalText" => ProgramGlobalText.Load, 
			_ => null, 
		};
	}

	public static ClearFP GetClearFunc(string className)
	{
		return className switch
		{
			"VideoFile" => VideoFile.Clear, 
			"UISound" => UISound.Clear, 
			"SoundFile" => SoundFile.Clear, 
			"SelectData" => SelectData.Clear, 
			"SpeechData" => ScriptSpeechData.Clear, 
			"StaffRoll" => StaffRoll.Clear, 
			"InGameTime" => InGameTime.Clear, 
			"AccountData" => AccountData.Clear, 
			"MessengerChatroomData" => MessengerChatroomData.Clear, 
			"MessengerTalkData" => MessengerTalkData.Clear, 
			"WatchFaterAutoText" => WatchFaterAutoText.Clear, 
			"CharData_ForProfile" => CharData_ForProfile.Clear, 
			"SNSPostData" => SNSPostData.Clear, 
			"SNSReplyData" => SNSReplyData.Clear, 
			"SequenceData" => SequenceData.Clear, 
			"Voice" => Voice.Clear, 
			"ImageFile" => ImageFile.Clear, 
			"ScriptKeyValue" => ScriptKeyValue.Clear, 
			"ScriptKeyTextValue" => ScriptKeyTextValue.Clear, 
			"CameraShakeValue" => CameraShakeValue.Clear, 
			"CharRelationEvt" => CharRelationEvt.Clear, 
			"CharMotData" => CharMotData.Clear, 
			"CharData" => CharData.Clear, 
			"CharZoomPos" => CharZoomPos.Clear, 
			"CollSounds" => CollSounds.Clear, 
			"CollImages" => CollImages.Clear, 
			"ColImageDescListData" => ColImageDescListData.Clear, 
			"CollKeyword" => CollKeyword.Clear, 
			"Trophys" => Trophys.Clear, 
			"Profiles" => Profiles.Clear, 
			"TalkCutSetting" => TalkCutSetting.Clear, 
			"TalkCutChrSetting" => TalkCutChrSetting.Clear, 
			"KeywordUsing" => KeywordUsing.Clear, 
			"TextData" => TextData.Clear, 
			"TextListData" => TextListData.Clear, 
			"TutorialData" => TutorialData.Clear, 
			"TutorialListData" => TutorialListData.Clear, 
			"ProgramDefineStr" => ProgramDefineStr.Clear, 
			"ProgramText" => ProgramText.Clear, 
			"ProgramGlobalText" => ProgramGlobalText.Clear, 
			_ => null, 
		};
	}

	private static string GetString(int posInStream)
	{
		long position = s_Stream.Position;
		s_Stream.Position = posInStream;
		s_Stream.Read(s_byteBuffer, 0, 4);
		int num = BitConverter.ToInt32(s_byteBuffer, 0);
		if (num > s_byteBuffer.Length)
		{
			s_byteBuffer = new byte[num];
		}
		s_Stream.Read(s_byteBuffer, 0, num);
		string result = Encoding.Unicode.GetString(s_byteBuffer, 0, num);
		s_Stream.Position = position;
		return result;
	}
}
