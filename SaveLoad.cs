using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using GameData;
using GameEvent;
using UnityEngine;

public class SaveLoad
{
	private enum eSaveStep
	{
		NONE,
		START,
		DONE
	}

	private class SaveLoadInfo
	{
		public bool m_isDelete;

		public bool m_isSave;

		public eSaveType m_eType;

		public int m_iSaveSlot;

		public eSaveStep m_eSaveStep;

		public SaveLoadInfo(bool isSave, eSaveType eType, int iSlotIdx = 0)
		{
			m_isSave = isSave;
			m_eType = eType;
			m_iSaveSlot = iSlotIdx;
			m_eSaveStep = eSaveStep.NONE;
		}

		public SaveLoadInfo()
		{
		}
	}

	public delegate void SaveLoadCB(bool isExistErr);

	public enum eSaveType
	{
		GAME = 0,
		SAVE_INFO = 16,
		COLL = 17,
		OPT = 18,
		CONFIG = 19,
		VERSION = 20,
		ALL = 21
	}

	public enum eSlotIndex
	{
		AUTO = 0,
		CNT = 0x10
	}

	public class cSaveSlotInfo
	{
		public bool m_isSave;

		public float m_fGameTime;

		public int m_iSeqIdx = -1;

		public int m_iCutIdx = -1;

		public int m_iCurYear;

		public int m_iCurMonth;

		public int m_iCurDay;

		public int m_iCurH;

		public int m_iCurM;
	}

	public enum eSaveWhat
	{
		eSaveGameInfoColl,
		eSaveCollOptConfig,
		eSaveColl,
		eSaveOptConfig,
		eLoadCollGame,
		eLoadColl,
		eLoadOptConfigInfoColl,
		eLoadEntireBuf,
		eSaveDataForDelete,
		eConvertForVersionUp,
		eLoadAllOptConfigInfoColl,
		eDeleteAndSave
	}

	public class Crypt
	{
		private byte[] key = new byte[16]
		{
			226, 27, 241, 146, 176, 195, 93, 158, 170, 188,
			49, 82, 71, 56, 225, 163
		};

		private byte[] iv = new byte[16]
		{
			233, 123, 140, 96, 64, 151, 212, 110, 46, 156,
			255, 197, 226, 74, 43, 92
		};

		public byte[] Encrypt(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			using Aes aes = Aes.Create();
			aes.KeySize = 128;
			aes.BlockSize = 128;
			aes.Padding = PaddingMode.Zeros;
			aes.Key = key;
			aes.IV = iv;
			using ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
			return PerformCryptography(data, cryptoTransform);
		}

		public byte[] Decrypt(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			using Aes aes = Aes.Create();
			aes.KeySize = 128;
			aes.BlockSize = 128;
			aes.Padding = PaddingMode.Zeros;
			aes.Key = key;
			aes.IV = iv;
			using ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);
			return PerformCryptography(data, cryptoTransform);
		}

		private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
		{
			if (data == null)
			{
				return null;
			}
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
			return memoryStream.ToArray();
		}
	}

	private static int i_SAVE_DATA_BROKEN_ERR_CODE = -2137063409;

	private static int i_SAVE_DATA_NO_SPACE = -2137063414;

	private const string SAVE_VER = "34";

	private int I_SAVE_VER = int.Parse("34");

	public const string NAME_UNITED_SAVE_FILE_BACK = "BSSF.sav";

	public const string NAME_UNITED_SAVE_FILE = "34BSSF.sav";

	private byte[] m_byArrSaveBuf;

	private int[] m_iArrFileSize;

	public const string NAME_SAVE_INFO = "34si";

	public const string NAME_SAVE_COLL = "34co";

	public const string NAME_AUTO_SAVE = "34as";

	public const string NAME_USER_SAVE = "34us";

	public const string NAME_OPTION_SAVE = "34opt";

	public const string NAME_CONFIG_SAVE = "34cfg";

	public const string NAME_VERSION_SAVE = "ver";

	public const int SIZE_GAME_SAVE = 0;

	public const string NAME_DIR_NAME_FOR_PS4 = "BS";

	private static SaveLoad s_Instance;

	private MainLoadThing m_mainLoadThing;

	private List<SaveLoadInfo> m_lstSaveInfo;

	private bool m_isNeedSave;

	private bool m_isSaving;

	private bool m_isSaveFlow;

	private const string cTITLE_ID = "BURIED STARS";

	private const string cSUB_TITLE = "Save Data";

	private const string cDETAIL = "BURIED STARS";

	private SaveLoadCB m_CBSaveLoadFunc;

	private SaveLoadCB m_CBDeleteFunc;

	private bool m_isLoadSaveStart;

	private bool m_isShowCloseIcon;

	private bool m_isRealSave;

	private Crypt m_Crypt;

	public static int USER_SLOT_CNT = 15;

	private cSaveSlotInfo[] m_clSaveSlotInfo;

	private bool m_isLoadingIconNotClose;

	private int m_iLoadVer;

	private bool m_isBefFileLoad;

	private int m_iBefFileVer = -1;

	private SaveLoad()
	{
		MakeSaveSlotInfo();
		m_Crypt = new Crypt();
		m_CBSaveLoadFunc = null;
		m_isLoadSaveStart = false;
		m_isBefFileLoad = false;
		m_iBefFileVer = -1;
	}

	public static SaveLoad GetInstance()
	{
		if (s_Instance == null)
		{
			s_Instance = new SaveLoad();
		}
		return s_Instance;
	}

	public static void ReleaseInstance()
	{
		if (s_Instance != null)
		{
			s_Instance.m_mainLoadThing = null;
			s_Instance.m_byArrSaveBuf = null;
			s_Instance.m_iArrFileSize = null;
			if (s_Instance.m_lstSaveInfo != null)
			{
				s_Instance.m_lstSaveInfo.Clear();
			}
			s_Instance.m_CBSaveLoadFunc = null;
			s_Instance.m_CBDeleteFunc = null;
			s_Instance.m_clSaveSlotInfo = null;
		}
		s_Instance.m_Crypt = null;
		s_Instance = null;
	}

	~SaveLoad()
	{
		m_clSaveSlotInfo = null;
		m_Crypt = null;
	}

	private void MakeSaveSlotInfo()
	{
		m_lstSaveInfo = new List<SaveLoadInfo>();
		m_clSaveSlotInfo = new cSaveSlotInfo[16];
		m_iArrFileSize = new int[21];
		int num = 16;
		for (int i = 0; i < num; i++)
		{
			m_clSaveSlotInfo[i] = new cSaveSlotInfo();
		}
	}

	public static void Update()
	{
		if (s_Instance == null)
		{
			GetInstance();
		}
		if (s_Instance != null && s_Instance.m_isLoadSaveStart && !PopupDialoguePlus.IsAnyPopupActivated())
		{
			s_Instance.ProcSaveLoad();
		}
	}

	public void SetMainLoadThing(MainLoadThing mainLoadThing)
	{
		m_mainLoadThing = mainLoadThing;
	}

	private string GetEventSaveFileName(eSaveType eSType, int iSaveSlot = 0)
	{
		string path = null;
		switch (eSType)
		{
		case eSaveType.SAVE_INFO:
			path = "34si";
			break;
		case eSaveType.COLL:
			path = "34co";
			break;
		case eSaveType.GAME:
			path = ((iSaveSlot != 0) ? ("34us" + iSaveSlot) : "34as");
			break;
		case eSaveType.OPT:
			path = "34opt";
			break;
		case eSaveType.CONFIG:
			path = "34cfg";
			break;
		case eSaveType.VERSION:
			path = "ver";
			break;
		case eSaveType.ALL:
			path = ((!m_isBefFileLoad) ? "34BSSF.sav" : (m_iBefFileVer + "BSSF.sav"));
			break;
		}
		return Path.Combine(Application.persistentDataPath, path);
	}

	public bool ExistEventSaveFile(eSaveType eSType, ref uint iReturnValue, int iSaveSlot = 0)
	{
		bool flag = false;
		if (eSType == eSaveType.ALL)
		{
			string eventSaveFileName = GetEventSaveFileName(eSType, iSaveSlot);
			flag = File.Exists(eventSaveFileName);
		}
		else
		{
			flag = m_byArrSaveBuf != null;
		}
		if (flag)
		{
			int uniteSaveFileCheckIdx = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
			flag = m_byArrSaveBuf[uniteSaveFileCheckIdx] == 1;
		}
		return flag;
	}

	public int SaveSlotInfoSize()
	{
		return 16 * (1 + GameSwitch.SIZE_FLOAT + GameSwitch.SIZE_INT + GameSwitch.SIZE_INT + GameSwitch.SIZE_INT * 5);
	}

	public cSaveSlotInfo GetSlotInfo(int iIndex)
	{
		if (iIndex >= 16)
		{
			return null;
		}
		return m_clSaveSlotInfo[iIndex];
	}

	public void SaveSlotInfo(int iSaveSlot, byte[] bySaveBuf, ref int iOffset)
	{
		if (iSaveSlot < 16)
		{
			GameSwitch instance = GameSwitch.GetInstance();
			m_clSaveSlotInfo[iSaveSlot].m_isSave = true;
			m_clSaveSlotInfo[iSaveSlot].m_fGameTime = instance.GetGameTime();
			m_clSaveSlotInfo[iSaveSlot].m_iSeqIdx = instance.GetCurSequence();
			m_clSaveSlotInfo[iSaveSlot].m_iCutIdx = instance.GetCurCutIdx();
			DateTime now = DateTime.Now;
			m_clSaveSlotInfo[iSaveSlot].m_iCurYear = now.Year;
			m_clSaveSlotInfo[iSaveSlot].m_iCurMonth = now.Month;
			m_clSaveSlotInfo[iSaveSlot].m_iCurDay = now.Day;
			m_clSaveSlotInfo[iSaveSlot].m_iCurH = now.Hour;
			m_clSaveSlotInfo[iSaveSlot].m_iCurM = now.Minute;
			int num = 16;
			for (int i = 0; i < num; i++)
			{
				BitCalc.BooleanToByteNCO(m_clSaveSlotInfo[i].m_isSave, bySaveBuf, ref iOffset);
				BitCalc.FloatToByteNCO(m_clSaveSlotInfo[i].m_fGameTime, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iSeqIdx, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCutIdx, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCurYear, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCurMonth, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCurDay, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCurH, bySaveBuf, ref iOffset);
				BitCalc.IntToByteNCO(m_clSaveSlotInfo[i].m_iCurM, bySaveBuf, ref iOffset);
			}
		}
	}

	public void LoadSlotInfo(byte[] byLoadBuf, ref int iOffset)
	{
		int num = 16;
		for (int i = 0; i < num; i++)
		{
			m_clSaveSlotInfo[i].m_isSave = BitCalc.ByteToBooleanNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_fGameTime = BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iSeqIdx = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCutIdx = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCurYear = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCurMonth = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCurDay = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCurH = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			m_clSaveSlotInfo[i].m_iCurM = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		}
	}

	public void InitSlotInfo()
	{
		int num = 16;
		for (int i = 0; i < num; i++)
		{
			m_clSaveSlotInfo[i].m_isSave = false;
		}
	}

	private int GetUniteSaveSize()
	{
		int num = 21;
		return num + num * 4;
	}

	private void SetNeedSave(bool isNeedSave, bool isAllInit = false)
	{
		m_isNeedSave = isNeedSave;
		if (isNeedSave)
		{
			m_isSaveFlow = isNeedSave;
		}
		if (isAllInit)
		{
			m_isSaveFlow = false;
		}
	}

	public void SaveLoadWhat(eSaveWhat eWhat, int iSlotIdx = 0, SaveLoadCB cbFuncSaveLoad = null, bool isShowCloseIcon = true, bool isLoadingIconNotClose = false)
	{
		m_isLoadingIconNotClose = isLoadingIconNotClose;
		m_isLoadSaveStart = true;
		m_isShowCloseIcon = isShowCloseIcon;
		if (eWhat != eSaveWhat.eDeleteAndSave)
		{
			m_CBSaveLoadFunc = cbFuncSaveLoad;
		}
		m_lstSaveInfo.Clear();
		SetNeedSave(isNeedSave: false, isAllInit: true);
		m_isRealSave = false;
		m_isSaving = false;
		if (eWhat == eSaveWhat.eLoadCollGame || eWhat == eSaveWhat.eLoadColl || eWhat == eSaveWhat.eLoadOptConfigInfoColl || eWhat == eSaveWhat.eLoadEntireBuf)
		{
			LoadingScreen.Show();
		}
		else
		{
			SavingScreen.Show();
		}
		switch (eWhat)
		{
		case eSaveWhat.eSaveGameInfoColl:
		{
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.GAME, iSlotIdx));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.SAVE_INFO, iSlotIdx));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.COLL));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.VERSION));
			SetNeedSave(isNeedSave: true);
			GameSwitch instance = GameSwitch.GetInstance();
			if (instance != null)
			{
				instance.SetShowEnding(isShowEnding: false);
				instance.SetReLoadSlotIdx(iSlotIdx);
			}
			break;
		}
		case eSaveWhat.eSaveCollOptConfig:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.COLL));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.OPT));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.CONFIG));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.VERSION));
			SetNeedSave(isNeedSave: true);
			break;
		case eSaveWhat.eSaveColl:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.COLL));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.VERSION));
			SetNeedSave(isNeedSave: true);
			break;
		case eSaveWhat.eSaveOptConfig:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.OPT));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.CONFIG));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.VERSION));
			SetNeedSave(isNeedSave: true);
			break;
		case eSaveWhat.eLoadCollGame:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.COLL));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.GAME, iSlotIdx));
			SetNeedSave(isNeedSave: false, isAllInit: true);
			break;
		case eSaveWhat.eLoadColl:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.COLL));
			SetNeedSave(isNeedSave: false, isAllInit: true);
			break;
		case eSaveWhat.eLoadOptConfigInfoColl:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.OPT));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.CONFIG));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.SAVE_INFO, iSlotIdx));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.COLL));
			SetNeedSave(isNeedSave: false, isAllInit: true);
			break;
		case eSaveWhat.eLoadEntireBuf:
			CheckConvertBefSaveFileToCurSaveFile();
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.ALL));
			SetNeedSave(isNeedSave: false, isAllInit: true);
			break;
		case eSaveWhat.eSaveDataForDelete:
			SetNeedSave(isNeedSave: true);
			break;
		case eSaveWhat.eDeleteAndSave:
		{
			SavingScreen.Show();
			SaveLoadInfo saveLoadInfo = new SaveLoadInfo();
			saveLoadInfo.m_isDelete = true;
			m_lstSaveInfo.Add(saveLoadInfo);
			SetNeedSave(isNeedSave: true);
			break;
		}
		case eSaveWhat.eConvertForVersionUp:
			ConvertBefVerToCurVer();
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: true, eSaveType.VERSION));
			SetNeedSave(isNeedSave: true);
			break;
		case eSaveWhat.eLoadAllOptConfigInfoColl:
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.OPT));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.CONFIG));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.SAVE_INFO, iSlotIdx));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.COLL));
			m_lstSaveInfo.Add(new SaveLoadInfo(isSave: false, eSaveType.ALL));
			SetNeedSave(isNeedSave: false, isAllInit: true);
			break;
		}
	}

	private void SetSaveStep(SaveLoadInfo saveInfo, eSaveStep eStep)
	{
		if (saveInfo.m_eSaveStep < eStep)
		{
			saveInfo.m_eSaveStep = eStep;
		}
	}

	private void DeleteDone(bool isExistErr)
	{
		SetLastSaveLoadListDone();
	}

	public bool ProcSaveLoad()
	{
		bool result = false;
		int count = m_lstSaveInfo.Count;
		if (count <= 0)
		{
			result = true;
			SetLastSaveLoadListDone();
		}
		else
		{
			SaveLoadInfo saveLoadInfo = m_lstSaveInfo[count - 1];
			if (saveLoadInfo.m_eSaveStep == eSaveStep.NONE)
			{
				SetNeedSave(m_isNeedSave || saveLoadInfo.m_isSave);
				if (saveLoadInfo.m_isDelete)
				{
					saveLoadInfo.m_eSaveStep = eSaveStep.START;
					m_CBDeleteFunc = DeleteDone;
					DeleteFile("34BSSF.sav");
					return false;
				}
				if (saveLoadInfo.m_isSave)
				{
					SaveType(saveLoadInfo.m_eType, saveLoadInfo.m_iSaveSlot);
				}
				else
				{
					LoadType(saveLoadInfo.m_eType, saveLoadInfo.m_iSaveSlot);
				}
				SetSaveStep(saveLoadInfo, eSaveStep.DONE);
			}
			else if (saveLoadInfo.m_eSaveStep == eSaveStep.DONE)
			{
				m_lstSaveInfo.Remove(saveLoadInfo);
				if (m_lstSaveInfo.Count <= 0)
				{
					result = true;
					SetLastSaveLoadListDone();
				}
			}
		}
		return result;
	}

	public void SetLastSaveLoadListDone(bool isForceEnd = false)
	{
		if (isForceEnd)
		{
			m_lstSaveInfo.Clear();
		}
		int count = m_lstSaveInfo.Count;
		if (count <= 0)
		{
			if (m_isNeedSave)
			{
				Save("34BSSF.sav", m_byArrSaveBuf);
				if (!isForceEnd)
				{
					SetNeedSave(isNeedSave: false);
				}
			}
			else
			{
				EndSaveLoad(isForceEnd);
			}
		}
		else
		{
			SetSaveStep(m_lstSaveInfo[count - 1], eSaveStep.DONE);
		}
	}

	private void EndSaveLoad(bool isForceEnd = false)
	{
		if (m_isLoadSaveStart)
		{
			m_isLoadSaveStart = false;
			if (m_isShowCloseIcon)
			{
				SavingScreen.Close();
				if (!m_isLoadingIconNotClose)
				{
					LoadingScreen.Close();
				}
			}
			m_isLoadingIconNotClose = false;
		}
		if (m_CBSaveLoadFunc != null)
		{
			m_CBSaveLoadFunc(isForceEnd);
		}
		if (isForceEnd)
		{
			m_lstSaveInfo.Clear();
		}
		if (m_lstSaveInfo.Count == 0)
		{
			m_CBSaveLoadFunc = null;
		}
	}

	private int GetCalcCurSaveFileSize(ref int[] iArrFileSize, int iCurSlot = 0)
	{
		int num = 0;
		int num2 = 0;
		num += GetUniteSaveSize();
		for (eSaveType eSaveType = eSaveType.GAME; eSaveType < eSaveType.ALL; eSaveType++)
		{
			switch (eSaveType)
			{
			case eSaveType.GAME:
				num2 = ((iCurSlot != (int)eSaveType) ? GetGameSaveFileSize((int)eSaveType) : GetGameSaveFileSize());
				break;
			case eSaveType.SAVE_INFO:
			case eSaveType.COLL:
			case eSaveType.OPT:
			case eSaveType.CONFIG:
			case eSaveType.VERSION:
				num2 = GetSaveFileSize(eSaveType);
				break;
			}
			iArrFileSize[(int)eSaveType] = num2;
			num += num2;
		}
		return num;
	}

	private void SaveType(eSaveType eSType, int iSaveSlot = 0)
	{
		byte[] array = null;
		int num = 0;
		int num2 = 0;
		GameSwitch instance = GameSwitch.GetInstance();
		if (m_byArrSaveBuf == null)
		{
			num2 = GetCalcCurSaveFileSize(ref m_iArrFileSize, iSaveSlot);
		}
		else
		{
			int uniteSaveFileCheckIdx = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
			int saveFileSize = GetSaveFileSize(eSType);
			if (m_iArrFileSize[uniteSaveFileCheckIdx] != saveFileSize)
			{
				CheckMakeNotSaveBufChangePos(uniteSaveFileCheckIdx, m_iArrFileSize[uniteSaveFileCheckIdx], saveFileSize);
			}
		}
		num = GetSaveFileOffset(eSType, iSaveSlot, m_iArrFileSize);
		if (m_byArrSaveBuf == null && num2 > 0)
		{
			m_byArrSaveBuf = new byte[num2];
			BitCalc.InitArray(m_byArrSaveBuf, 0);
		}
		array = m_byArrSaveBuf;
		MakeSaveBuf(eSType, iSaveSlot, m_byArrSaveBuf, ref num);
		int uniteSaveFileCheckIdx2 = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
		m_byArrSaveBuf[uniteSaveFileCheckIdx2] = 1;
		num = 21;
		for (int i = 0; i < 21; i++)
		{
			BitCalc.IntToByteNCO(m_iArrFileSize[i], m_byArrSaveBuf, ref num);
		}
	}

	private void CheckMakeNotSaveBufChangePos(int iCheckFileIdx, int iBefSize, int iCurSize)
	{
		if (m_byArrSaveBuf == null)
		{
			return;
		}
		int num = 21;
		int uniteSaveSize = GetUniteSaveSize();
		int num2 = uniteSaveSize;
		for (int i = 0; i < num; i++)
		{
			num2 = ((i != iCheckFileIdx) ? (num2 + m_iArrFileSize[i]) : (num2 + iCurSize));
		}
		byte[] array = new byte[num2];
		int iSrcOffset = 0;
		int iDstOffset = 0;
		BitCalc.ByteToByte(m_byArrSaveBuf, iSrcOffset, array, iDstOffset, uniteSaveSize);
		iSrcOffset = uniteSaveSize;
		iDstOffset = uniteSaveSize;
		for (int j = 0; j < num; j++)
		{
			if (j != iCheckFileIdx)
			{
				BitCalc.ByteToByte(m_byArrSaveBuf, iSrcOffset, array, iDstOffset, m_iArrFileSize[j]);
			}
			iSrcOffset += m_iArrFileSize[j];
			iDstOffset += ((j != iCheckFileIdx) ? m_iArrFileSize[j] : iCurSize);
		}
		m_iArrFileSize[iCheckFileIdx] = iCurSize;
		m_byArrSaveBuf = null;
		m_byArrSaveBuf = new byte[num2];
		iSrcOffset = 0;
		iDstOffset = 0;
		BitCalc.ByteToByte(array, iSrcOffset, m_byArrSaveBuf, iDstOffset, num2);
	}

	private void MakeSaveBuf(eSaveType eSType, int iSaveSlot, byte[] bySaveArr, ref int iOffset, int iBefVer = -1)
	{
		switch (eSType)
		{
		case eSaveType.SAVE_INFO:
			SaveSlotInfo(iSaveSlot, bySaveArr, ref iOffset);
			break;
		case eSaveType.GAME:
			GameSwitch.GetInstance().SaveGameDataAll(bySaveArr, ref iOffset, iBefVer);
			break;
		case eSaveType.COLL:
			GameSwitch.GetInstance().SaveCollData(bySaveArr, ref iOffset);
			break;
		case eSaveType.OPT:
		{
			AudioManager instance = AudioManager.instance;
			if (instance != null)
			{
				instance.SaveOptSound(bySaveArr, ref iOffset);
			}
			break;
		}
		case eSaveType.CONFIG:
			GameSwitch.GetInstance().SaveConfigData(bySaveArr, ref iOffset);
			break;
		case eSaveType.VERSION:
			BitCalc.IntToByteNCO(I_SAVE_VER, bySaveArr, ref iOffset);
			break;
		}
	}

	private int GetUniteSaveFileCheckIdx(eSaveType eSType, int iSaveSlot)
	{
		return (eSType != eSaveType.GAME) ? ((int)eSType) : iSaveSlot;
	}

	private int GetGameSaveFileSize(int iSlotIdx = -1, bool isOnlyLoadFile = false)
	{
		int result = 0;
		if (iSlotIdx == -1)
		{
			result = GetSaveFileSize(eSaveType.GAME, isOnlyLoadFile);
		}
		else if (m_byArrSaveBuf != null)
		{
			int iIdx = 21 + 4 * iSlotIdx;
			result = ((m_byArrSaveBuf[iSlotIdx] == 1) ? BitCalc.ByteToInt(m_byArrSaveBuf, iIdx) : 0);
		}
		return result;
	}

	private int GetSaveFileSize(eSaveType eSType, bool isOnlyLoadFile = false)
	{
		int num = 0;
		switch (eSType)
		{
		case eSaveType.SAVE_INFO:
			num = SaveSlotInfoSize();
			break;
		case eSaveType.GAME:
			num = GameSwitch.GetInstance().GetSaveEventDataSize(isOnlyLoadFile);
			break;
		case eSaveType.COLL:
			num = GameSwitch.GetInstance().GetSaveCollDataSize();
			break;
		case eSaveType.OPT:
		{
			AudioManager instance = AudioManager.instance;
			if (instance != null)
			{
				num = instance.GetOptSoundSize();
			}
			break;
		}
		case eSaveType.CONFIG:
			num = GameSwitch.GetInstance().GetConfigDataSize();
			break;
		case eSaveType.VERSION:
			num = 4;
			break;
		case eSaveType.ALL:
		{
			int num2 = 0;
			num += GetUniteSaveSize();
			for (eSaveType eSaveType = eSaveType.GAME; eSaveType < eSaveType.ALL; eSaveType++)
			{
				switch (eSaveType)
				{
				case eSaveType.GAME:
					num2 = GetGameSaveFileSize((int)eSaveType);
					break;
				case eSaveType.SAVE_INFO:
				case eSaveType.COLL:
				case eSaveType.OPT:
				case eSaveType.CONFIG:
				case eSaveType.VERSION:
					num2 = GetSaveFileSize(eSaveType);
					break;
				}
				num += num2;
			}
			break;
		}
		}
		return num;
	}

	private int GetSaveFileOffset(eSaveType eSType, int iSaveSlot, int[] iArrFileSize)
	{
		int num = 0;
		num += GetUniteSaveSize();
		int uniteSaveFileCheckIdx = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
		for (int i = 0; i < 21 && i < uniteSaveFileCheckIdx; i++)
		{
			num += iArrFileSize[i];
		}
		return num;
	}

	private void LoadType(eSaveType eSType, int iSaveSlot = 0, int iBefVer = -1)
	{
		string empty = string.Empty;
		empty = GetEventSaveFileName(eSType, iSaveSlot);
		int iOffset = 0;
		if (eSType == eSaveType.ALL)
		{
			m_byArrSaveBuf = Load(empty);
			SetLoadedByteArray(eSType, m_byArrSaveBuf, ref iOffset);
			return;
		}
		int uniteSaveFileCheckIdx = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
		if (eSType == eSaveType.ALL)
		{
			iOffset = GetSaveFileOffset(eSType, iSaveSlot, m_iArrFileSize);
			SetLoadedByteArray(eSType, m_byArrSaveBuf, ref iOffset);
		}
		else if (m_byArrSaveBuf != null)
		{
			if (m_byArrSaveBuf[uniteSaveFileCheckIdx] == 1)
			{
				iOffset = GetSaveFileOffset(eSType, iSaveSlot, m_iArrFileSize);
				SetLoadedByteArray(eSType, m_byArrSaveBuf, ref iOffset, iBefVer);
			}
			else
			{
				InitByteArray(eSType);
			}
		}
	}

	private void InitByteArray(eSaveType eSType)
	{
		if (eSType == eSaveType.COLL)
		{
			GameSwitch.GetInstance().InitCollectionSwitch();
		}
	}

	private void SetLoadedByteArray(eSaveType eSType, byte[] byLoadArr, ref int iOffset, int iBefVer = -1)
	{
		if (byLoadArr != null)
		{
			switch (eSType)
			{
			case eSaveType.SAVE_INFO:
				LoadSlotInfo(byLoadArr, ref iOffset);
				break;
			case eSaveType.GAME:
				GameSwitch.GetInstance().LoadGameDataAll(byLoadArr, ref iOffset, iBefVer, I_SAVE_VER);
				EventEngine.GetInstance().SetIsSaveFileLoad(isSet: true);
				break;
			case eSaveType.COLL:
				GameSwitch.GetInstance().LoadCollData(byLoadArr, ref iOffset, iBefVer, I_SAVE_VER);
				break;
			case eSaveType.OPT:
			{
				AudioManager instance = AudioManager.instance;
				if (instance != null)
				{
					instance.LoadOptSound(byLoadArr, ref iOffset);
				}
				break;
			}
			case eSaveType.CONFIG:
				GameSwitch.GetInstance().LoadConfigData(byLoadArr, ref iOffset);
				break;
			case eSaveType.VERSION:
				m_iLoadVer = BitCalc.ByteToIntNCO(byLoadArr, ref iOffset);
				break;
			case eSaveType.ALL:
			{
				int iIdx = 21;
				m_byArrSaveBuf = byLoadArr;
				if (m_iArrFileSize == null)
				{
					m_iArrFileSize = new int[21];
				}
				for (int i = 0; i < 21; i++)
				{
					m_iArrFileSize[i] = BitCalc.ByteToIntNCO(m_byArrSaveBuf, ref iIdx);
				}
				break;
			}
			}
		}
		if (iBefVer == -1)
		{
			SetLastSaveLoadListDone();
		}
	}

	public void DeleteAll(SaveLoadCB cbFuncDelete = null)
	{
		DeleteAllGameAndCollData(cbFuncDelete);
	}

	private void DeleteAllGameAndCollData(SaveLoadCB cbFuncDelete = null)
	{
		if (m_byArrSaveBuf != null)
		{
			int uniteSaveSize = GetUniteSaveSize();
			int num = uniteSaveSize;
			for (eSaveType eSaveType = eSaveType.OPT; eSaveType < eSaveType.ALL; eSaveType++)
			{
				num += m_iArrFileSize[(int)eSaveType];
			}
			byte[] array = new byte[num];
			BitCalc.InitArray(array, 0);
			int num2 = 0;
			int num3 = 0;
			num2 = GetSaveFileOffset(eSaveType.OPT, 0, m_iArrFileSize);
			num3 = uniteSaveSize;
			int num4 = 0;
			for (eSaveType eSaveType2 = eSaveType.OPT; eSaveType2 < eSaveType.ALL; eSaveType2++)
			{
				num4 += m_iArrFileSize[(int)eSaveType2];
			}
			BitCalc.ByteToByte(m_byArrSaveBuf, num2, array, num3, num4);
			for (eSaveType eSaveType3 = eSaveType.GAME; eSaveType3 < eSaveType.OPT; eSaveType3++)
			{
				m_iArrFileSize[(int)eSaveType3] = 0;
			}
			int iIdx = 21;
			int num5 = 0;
			for (eSaveType eSaveType4 = eSaveType.GAME; eSaveType4 < eSaveType.ALL; eSaveType4++)
			{
				num5 = (int)eSaveType4;
				array[num5] = ((eSaveType4 >= eSaveType.OPT) ? ((byte)1) : ((byte)0));
				BitCalc.IntToByteNCO(m_iArrFileSize[(int)eSaveType4], array, ref iIdx);
			}
			InitSlotInfo();
			m_byArrSaveBuf = null;
			m_byArrSaveBuf = new byte[num];
			BitCalc.ByteToByte(array, 0, m_byArrSaveBuf, 0, num);
			GameSwitch.GetInstance().InitCollectionSwitch();
			SaveLoadWhat(eSaveWhat.eSaveDataForDelete, 0, cbFuncDelete);
		}
	}

	private void DeleteType(bool isAll = false, eSaveType eSType = eSaveType.GAME, int iSaveSlot = 0, SaveLoadCB cbFuncDelete = null)
	{
		if (isAll)
		{
			m_byArrSaveBuf = null;
			m_clSaveSlotInfo = null;
			MakeSaveSlotInfo();
			GameSwitch.GetInstance().InitCollectionSwitch();
			DeleteFile("34BSSF.sav");
		}
		else
		{
			int uniteSaveFileCheckIdx = GetUniteSaveFileCheckIdx(eSType, iSaveSlot);
			CheckMakeNotSaveBufChangePos(uniteSaveFileCheckIdx, m_iArrFileSize[uniteSaveFileCheckIdx], 0);
			m_iArrFileSize[uniteSaveFileCheckIdx] = 0;
			m_byArrSaveBuf[uniteSaveFileCheckIdx] = 0;
			int iIdx = 21;
			for (int i = 0; i < 21; i++)
			{
				BitCalc.IntToByteNCO(m_iArrFileSize[i], m_byArrSaveBuf, ref iIdx);
			}
			SetNeedSave(isNeedSave: true);
			SetLastSaveLoadListDone();
		}
		cbFuncDelete?.Invoke(isExistErr: false);
	}

	private void DeleteFile(string strFileName)
	{
		File.Delete(strFileName);
	}

	public void Save(string strFileName, byte[] bySaveArr)
	{
		if (strFileName != null && bySaveArr != null)
		{
			bySaveArr = m_Crypt.Encrypt(bySaveArr);
			string path = Path.Combine(Application.persistentDataPath, strFileName);
			FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			fileStream.Write(bySaveArr, 0, bySaveArr.Length);
			fileStream.Close();
		}
	}

	private void SaveByFileSystem(string strFilePath, byte[] bySaveArr)
	{
		FileStream fileStream = File.Open(strFilePath, FileMode.Create, FileAccess.ReadWrite);
		fileStream.Write(bySaveArr, 0, bySaveArr.Length);
		fileStream.Close();
	}

	public byte[] Load(string strFileName)
	{
		byte[] array = null;
		string strFilePath = Path.Combine(Application.persistentDataPath, strFileName);
		array = ReadByFileSystem(strFilePath);
		return m_Crypt.Decrypt(array);
	}

	private byte[] ReadByFileSystem(string strFilePath)
	{
		byte[] array = null;
		if (File.Exists(strFilePath))
		{
			FileStream fileStream = File.Open(strFilePath, FileMode.Open);
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
		}
		return array;
	}

	public byte[] GetSaveFileForCrossSave()
	{
		if (m_byArrSaveBuf == null)
		{
			return null;
		}
		return m_byArrSaveBuf;
	}

	public void ApplySaveFileForCrossSave(byte[] byLoadBuf, SaveLoadCB cbFuncSaveLoad = null)
	{
		m_byArrSaveBuf = null;
		int num = byLoadBuf.Length;
		if (num <= 0)
		{
			cbFuncSaveLoad?.Invoke(isExistErr: false);
			return;
		}
		m_byArrSaveBuf = new byte[num];
		Buffer.BlockCopy(byLoadBuf, 0, m_byArrSaveBuf, 0, num);
		int iIdx = 21;
		for (int i = 0; i < 21; i++)
		{
			m_iArrFileSize[i] = BitCalc.ByteToIntNCO(m_byArrSaveBuf, ref iIdx);
		}
		SaveLoadWhat(eSaveWhat.eLoadOptConfigInfoColl, 0, cbFuncSaveLoad);
		SetNeedSave(isNeedSave: true);
	}

	private void ShowReSavePop(string strText = null)
	{
		SavingScreen.Close();
		LoadingScreen.Close();
		if (strText == null)
		{
			strText = "SAVEFILE_SAVE_ETC_ERR";
		}
		PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramGlobalText(strText), cbBrokenFileSavePopupOK);
	}

	private void cbBrokenFileSavePopupOK(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			SaveLoadWhat(eSaveWhat.eDeleteAndSave);
			return;
		}
		m_isSaving = false;
		SetLastSaveLoadListDone(isForceEnd: true);
	}

	private void ShowErrPop()
	{
		if (!PopupDialoguePlus.IsAnyPopupActivated())
		{
			SavingScreen.Close();
			LoadingScreen.Close();
			string xlsDataName = ((!m_isSaveFlow) ? "LOAD_FAIL" : "SAVE_FAIL");
			PopupDialoguePlus.ShowPopup_OK(GameGlobalUtil.GetXlsProgramGlobalText(xlsDataName), cbErrPopupOK);
		}
	}

	private void cbErrPopupOK(PopupDialoguePlus.Result result)
	{
		if (!m_isSaving)
		{
			SetLastSaveLoadListDone(isForceEnd: true);
		}
	}

	private void CheckConvertBefSaveFileToCurSaveFile()
	{
	}

	private void ConvertBefVerToCurVer()
	{
		if (!m_isBefFileLoad)
		{
			return;
		}
		int num = 0;
		byte[] array = null;
		int[] array2 = new int[21];
		int num2 = 0;
		int num3 = USER_SLOT_CNT + 1;
		eSaveType eSType = eSaveType.GAME;
		EventEngine instance = EventEngine.GetInstance();
		for (int i = 0; i < num3; i++)
		{
			instance.InitForConverLoad();
			LoadType(eSType, i, m_iBefFileVer);
			array2[GetUniteSaveFileCheckIdx(eSType, i)] = GetGameSaveFileSize(i, isOnlyLoadFile: true);
		}
		for (eSaveType eSaveType = eSaveType.SAVE_INFO; eSaveType < eSaveType.ALL; eSaveType++)
		{
			if (eSaveType != eSaveType.COLL)
			{
				array2[(int)eSaveType] = m_iArrFileSize[(int)eSaveType];
			}
		}
		array2[17] = GetSaveFileSize(eSaveType.COLL);
		num += GetUniteSaveSize();
		for (eSaveType eSaveType2 = eSaveType.GAME; eSaveType2 < eSaveType.ALL; eSaveType2++)
		{
			num += array2[(int)eSaveType2];
		}
		if (num > 0)
		{
			array = new byte[num];
		}
		BitCalc.InitArray(array, 0);
		Array.Copy(m_byArrSaveBuf, 0, array, 0, 21);
		for (int j = 0; j < 21; j++)
		{
			if (j >= 16)
			{
				eSType = (eSaveType)j;
			}
			int iSaveSlot = ((j < num3) ? j : 0);
			if (m_byArrSaveBuf[j] == 1)
			{
				LoadType(eSType, iSaveSlot, m_iBefFileVer);
				num2 = GetSaveFileOffset(eSType, iSaveSlot, array2);
				MakeSaveBuf(eSType, iSaveSlot, array, ref num2, m_iBefFileVer);
				if (j < num3)
				{
					array2[GetUniteSaveFileCheckIdx(eSType, iSaveSlot)] = GetGameSaveFileSize(j, isOnlyLoadFile: true);
				}
			}
		}
		num2 = 21;
		for (eSaveType eSaveType3 = eSaveType.GAME; eSaveType3 < eSaveType.ALL; eSaveType3++)
		{
			BitCalc.IntToByteNCO(m_iArrFileSize[(int)eSaveType3], array, ref num2);
		}
		m_byArrSaveBuf = null;
		m_byArrSaveBuf = new byte[num];
		Array.Copy(array, m_byArrSaveBuf, num);
		Array.Copy(array2, m_iArrFileSize, 21);
		CompVerConvertSaveFile();
	}

	public bool GetBefFileLoad()
	{
		CheckConvertBefSaveFileToCurSaveFile();
		return m_isBefFileLoad;
	}

	private void CompVerConvertSaveFile()
	{
		m_isBefFileLoad = false;
		m_iBefFileVer = -1;
	}
}
