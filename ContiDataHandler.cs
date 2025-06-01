using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ContiDataHandler
{
	public class FunctionData
	{
		public string m_strName = string.Empty;

		public string[] m_args;
	}

	public class ThreadData
	{
		public int m_iStartFuncIdx = -1;

		public int m_iEndFuncIdx = -1;
	}

	public class ContiData
	{
		public string m_strName = string.Empty;

		public FunctionData[] m_functions;

		public ThreadData[] m_threads;
	}

	private delegate bool ActionUpdateProc();

	private static SortedDictionary<string, ContiData> s_DataMap = new SortedDictionary<string, ContiData>();

	private static bool s_isInitailized = false;

	private static AssetBundleObjectHandler s_assetBundleHandler = null;

	private static ContiData s_activatingContiData = null;

	private static PMEventScript s_curEventScriptHdr = null;

	private static int s_iCurFuncitonIdx = 0;

	private static int s_iCurThreadIdx = 0;

	private static ActionUpdateProc s_curActionUpdate = null;

	private static int s_iCheckcount = 0;

	public static bool isExistActivatingConti => s_activatingContiData != null;

	public static string activatingContiName => (s_activatingContiData == null) ? string.Empty : s_activatingContiData.m_strName;

	public static IEnumerator Init(string assetName, bool isForceLoad = false)
	{
		if (s_isInitailized && !isForceLoad)
		{
			yield break;
		}
		s_DataMap.Clear();
		s_assetBundleHandler = new AssetBundleObjectHandler(assetName);
		if (s_assetBundleHandler != null)
		{
			yield return MainLoadThing.instance.StartCoroutine(s_assetBundleHandler.LoadAssetBundle());
			if (!(s_assetBundleHandler.loadedAssetBundleObject == null))
			{
				TextAsset asset = s_assetBundleHandler.GetLoadedAsset<TextAsset>();
				if (!(asset == null))
				{
					MemoryStream stream = new MemoryStream(asset.bytes);
					byte[] byteBufferStr = new byte[256];
					byte[] byteBufferInt = new byte[4];
					int readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
					int iContiDataCnt = BitConverter.ToInt32(byteBufferInt, 0);
					int strLength = 0;
					int functionCnt = 0;
					int argCnt = 0;
					int threadCnt = 0;
					for (int i = 0; i < iContiDataCnt; i++)
					{
						ContiData contiData = new ContiData();
						readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
						strLength = BitConverter.ToInt32(byteBufferInt, 0);
						if (strLength > byteBufferStr.Length)
						{
							byteBufferStr = new byte[strLength];
						}
						readedByteCnt = stream.Read(byteBufferStr, 0, strLength);
						contiData.m_strName = Encoding.Unicode.GetString(byteBufferStr, 0, strLength);
						s_DataMap.Add(contiData.m_strName, contiData);
						readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
						functionCnt = BitConverter.ToInt32(byteBufferInt, 0);
						if (functionCnt > 0)
						{
							contiData.m_functions = new FunctionData[functionCnt];
							for (int j = 0; j < functionCnt; j++)
							{
								FunctionData functionData = new FunctionData();
								contiData.m_functions[j] = functionData;
								readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
								strLength = BitConverter.ToInt32(byteBufferInt, 0);
								if (strLength > byteBufferStr.Length)
								{
									byteBufferStr = new byte[strLength];
								}
								readedByteCnt = stream.Read(byteBufferStr, 0, strLength);
								functionData.m_strName = Encoding.Unicode.GetString(byteBufferStr, 0, strLength);
								readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
								argCnt = BitConverter.ToInt32(byteBufferInt, 0);
								if (argCnt <= 0)
								{
									continue;
								}
								functionData.m_args = new string[argCnt];
								for (int k = 0; k < argCnt; k++)
								{
									readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
									strLength = BitConverter.ToInt32(byteBufferInt, 0);
									if (strLength > byteBufferStr.Length)
									{
										byteBufferStr = new byte[strLength];
									}
									readedByteCnt = stream.Read(byteBufferStr, 0, strLength);
									functionData.m_args[k] = Encoding.Unicode.GetString(byteBufferStr, 0, strLength);
								}
							}
						}
						readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
						threadCnt = BitConverter.ToInt32(byteBufferInt, 0);
						if (threadCnt > 0)
						{
							contiData.m_threads = new ThreadData[threadCnt];
							for (int l = 0; l < threadCnt; l++)
							{
								ThreadData threadData = new ThreadData();
								contiData.m_threads[l] = threadData;
								readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
								threadData.m_iStartFuncIdx = BitConverter.ToInt32(byteBufferInt, 0);
								readedByteCnt = stream.Read(byteBufferInt, 0, byteBufferInt.Length);
								threadData.m_iEndFuncIdx = BitConverter.ToInt32(byteBufferInt, 0);
							}
						}
					}
					stream.Close();
					stream = null;
					s_assetBundleHandler.UnloadAssetBundle(isUnloadLoadedAllAssets: true);
					s_assetBundleHandler = null;
					asset = null;
					s_isInitailized = true;
					yield break;
				}
			}
		}
		s_isInitailized = false;
	}

	public static bool IsExistConti(string strContiName)
	{
		return s_DataMap.ContainsKey(strContiName);
	}

	public static bool ActivateConti(string strContiName, PMEventScript eventScriptHdr)
	{
		if (s_activatingContiData != null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(strContiName))
		{
			return false;
		}
		if (!s_DataMap.ContainsKey(strContiName))
		{
			return false;
		}
		OnScreenLog.SetContiName(strContiName);
		s_activatingContiData = s_DataMap[strContiName];
		s_curEventScriptHdr = eventScriptHdr;
		if (s_activatingContiData.m_functions.Length <= 0)
		{
			CompleteConti();
			return true;
		}
		s_iCurFuncitonIdx = 0;
		s_iCurThreadIdx = 0;
		RunConti();
		return true;
	}

	public static void FinishContiForced()
	{
		CompleteConti();
	}

	private static void CompleteConti()
	{
		s_activatingContiData = null;
		s_curEventScriptHdr = null;
	}

	private static bool RunConti()
	{
		if (s_activatingContiData == null)
		{
			return true;
		}
		if (s_iCurFuncitonIdx >= s_activatingContiData.m_functions.Length)
		{
			return true;
		}
		ThreadData threadData = null;
		if (s_activatingContiData.m_threads != null && s_activatingContiData.m_threads.Length > s_iCurThreadIdx)
		{
			threadData = s_activatingContiData.m_threads[s_iCurThreadIdx];
		}
		if (threadData != null && threadData.m_iStartFuncIdx == s_iCurFuncitonIdx)
		{
			while (s_iCurFuncitonIdx < threadData.m_iEndFuncIdx)
			{
				RunFunction(s_activatingContiData.m_functions[s_iCurFuncitonIdx]);
				s_iCurFuncitonIdx++;
			}
			s_iCurThreadIdx++;
		}
		else
		{
			RunFunction(s_activatingContiData.m_functions[s_iCurFuncitonIdx]);
			s_iCurFuncitonIdx++;
		}
		return false;
	}

	private static void RunFunction(FunctionData funcData)
	{
		Type typeFromHandle = typeof(PMEventScript);
		MethodInfo method = typeFromHandle.GetMethod(funcData.m_strName);
		if (method == null)
		{
			return;
		}
		object[] array = null;
		int num = ((funcData.m_args != null) ? funcData.m_args.Length : 0);
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length != num)
		{
			return;
		}
		if (num > 0)
		{
			array = new object[num];
			for (int i = 0; i < num; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType == typeof(int))
				{
					int result = 0;
					if (!int.TryParse(funcData.m_args[i], out result))
					{
						return;
					}
					array[i] = result;
					continue;
				}
				if (parameterType == typeof(string))
				{
					array[i] = funcData.m_args[i];
					continue;
				}
				if (parameterType == typeof(bool))
				{
					bool result2 = false;
					if (!bool.TryParse(funcData.m_args[i], out result2))
					{
						return;
					}
					array[i] = result2;
					continue;
				}
				if (parameterType == typeof(float))
				{
					float result3 = 0f;
					if (!float.TryParse(funcData.m_args[i], NumberStyles.Float, CultureInfo.InvariantCulture, out result3))
					{
						return;
					}
					array[i] = result3;
					continue;
				}
				return;
			}
		}
		method.Invoke(s_curEventScriptHdr, array);
	}

	private static void RunFunction(int iFunctionIdx)
	{
		s_iCurFuncitonIdx = iFunctionIdx;
	}

	public static bool UpdateContiProc()
	{
		if (s_activatingContiData == null)
		{
			return true;
		}
		bool flag = true;
		if (!s_curEventScriptHdr.ProcScript())
		{
			return false;
		}
		if (!RunConti())
		{
			return false;
		}
		CompleteConti();
		return true;
	}

	private static void BeginAction_InCodeTest_00(int iArg, float fArg)
	{
		s_iCheckcount = iArg;
	}

	private static void BeginAction_InCodeTest_01(string strArg, bool bArg)
	{
	}

	private static void BeginAction_InCodeTest_02()
	{
		s_iCheckcount = 5;
	}

	private static bool UpdateAction_Test()
	{
		s_iCheckcount--;
		return s_iCheckcount <= 0;
	}

	private static bool UpdateAction_Dummy()
	{
		return true;
	}
}
