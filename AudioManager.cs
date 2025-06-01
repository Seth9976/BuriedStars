using System.Collections;
using System.IO;
using AssetBundles;
using GameData;
using GameEvent;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
	public enum eSND_VOLUME
	{
		BGM,
		EFF,
		VOICE,
		CNT
	}

	public enum eVIDEO_NATION
	{
		KOREAN,
		ENGLISH,
		JAPAN,
		CHINA_SIMPLE,
		CHINA_TRADITIONAL
	}

	public enum eSND_CHANNEL
	{
		BGM,
		SE_0,
		SE_1,
		MOOD_0,
		MOOD_1,
		VOICE,
		REACTION,
		UI,
		UI_2,
		UI_skip
	}

	public class ASData
	{
		public string m_strClipName = string.Empty;

		public bool m_isOneFileBundle = true;

		public float m_fVol;

		public float m_fChangeVolTime;

		public float m_fTime;

		public float m_fFirstVol;

		public float m_fNextVol;

		public float m_fScriptVol;

		public bool m_isClipMaking;

		public string m_strLoadName;

		public string m_strMixerGroupName;

		public int m_iMixerIdx = -1;
	}

	public class ASMixerInfo
	{
		public string m_strSnapshotName;
	}

	public const float OPT_MAX_VOL = 1f;

	private float[] m_fOptVol = new float[3];

	private float m_fSkipVol = 1f;

	private float m_fBGVolPercentPlayVoice;

	private bool m_isPreparedVoice;

	private bool m_isPlayVoice;

	private float m_fVoiceTime;

	private float m_fPassVoiceTime;

	private bool m_isPreparedReaction;

	private bool m_isPlayReaction;

	private float m_fReactionTime;

	private float m_fPassReactionTime;

	private GameDefine.EventProc m_cbFpEndVoice;

	private GameDefine.EventProc m_cbFpEndReaction;

	private const int SND_CHANNEL_CNT = 10;

	private GameSwitch m_GameSwitch;

	public ASData[] m_asData;

	private AudioSource[] m_asChannel = new AudioSource[10];

	public AudioMixer[] m_amMixer;

	private ASMixerInfo[] m_miInfo;

	private byte[] m_byMenuBackup;

	private int[] m_iBackupMixerIdx = new int[10];

	private string[] m_strBackupMixerGroupName = new string[10];

	private bool m_isPlayVideo;

	private bool m_isPauseVideo;

	public GameObject m_goMovie;

	public GameObject m_goPS4Video;

	public Image m_imagePS4Video;

	public GameObject m_goPSPVideo;

	public RenderTexture m_PSP2RenderTexture;

	public GameObject m_goWindowVideoPlayer;

	private VideoPlayer m_videoPlayer;

	private RenderTexture m_rtVideoPlayer;

	private bool m_isPreparePlayVideo;

	private EventEngine m_EventEngine;

	private static AudioManager s_Instance;

	public static AudioManager instance => s_Instance;

	private void Awake()
	{
		s_Instance = this;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		m_cbFpEndVoice = null;
		m_cbFpEndReaction = null;
		m_GameSwitch = null;
		m_EventEngine = null;
		InitAllClip();
		s_Instance = null;
	}

	private void InitAllClip()
	{
		bool flag = true;
		for (int i = 0; i < 10; i++)
		{
			flag = true;
			Stop(i);
			if (m_asData != null && m_asData[i] != null)
			{
				string strClipName = m_asData[i].m_strClipName;
				if (!string.IsNullOrEmpty(strClipName))
				{
					for (int j = i; j < 10; j++)
					{
						if (j != i && m_asData[j].m_strClipName == strClipName)
						{
							flag = false;
							break;
						}
					}
					AssetBundleManager.UnloadAssetBundle(Path.Combine("Sounds/", strClipName), unloadAllLoadedAssets: true);
				}
				m_asData[i].m_strClipName = string.Empty;
			}
			if (m_asChannel[i].clip != null)
			{
				if (flag)
				{
					Resources.UnloadAsset(m_asChannel[i].clip);
				}
				m_asChannel[i].clip = null;
			}
		}
	}

	public void Init()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_asChannel[i] == null)
			{
				m_asChannel[i] = base.gameObject.AddComponent<AudioSource>();
			}
		}
		if (m_asData == null)
		{
			m_asData = new ASData[10];
			for (int j = 0; j < 10; j++)
			{
				m_asData[j] = new ASData();
			}
		}
		if (m_amMixer != null)
		{
			int num = m_amMixer.Length;
			m_miInfo = new ASMixerInfo[num];
			for (int k = 0; k < num; k++)
			{
				m_miInfo[k] = new ASMixerInfo();
			}
		}
		m_isPlayVideo = false;
		InitOptSound();
		m_isPreparePlayVideo = false;
		m_videoPlayer = m_goWindowVideoPlayer.AddComponent<VideoPlayer>();
		if (m_EventEngine == null)
		{
			m_EventEngine = EventEngine.GetInstance();
		}
		m_fBGVolPercentPlayVoice = GameGlobalUtil.GetXlsProgramDefineStrToFloat("BG_VOL_PERCENT_VOICE_PLAY");
		m_isPlayVoice = false;
		m_fPassVoiceTime = (m_fVoiceTime = 0f);
		m_isPlayReaction = false;
		m_fPassReactionTime = (m_fReactionTime = 0f);
	}

	private void Update()
	{
		ProcVol();
		ProcVoice();
		if (!m_isPauseVideo)
		{
			if (m_isPlayVideo || m_isPreparePlayVideo)
			{
				ProcWidowVideo();
			}
			ProcVideoKey();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			if (m_isPlayVideo)
			{
				PauseVideo();
			}
			BackupCurGamePlaySound();
		}
		else
		{
			if (m_isPlayVideo)
			{
				ResumeVideo();
			}
			RestoreGamePlaySound();
		}
	}

	public void ProcVideoKey()
	{
		if (m_isPlayVideo)
		{
			GameSwitch.eUIButType uIButType = GameSwitch.GetInstance().GetUIButType();
			if ((uIButType == GameSwitch.eUIButType.KEYMOUSE && GamePadInput.IsButtonState_Down(PadInput.GameInput.SkipButton)) || (uIButType != GameSwitch.eUIButType.KEYMOUSE && GamePadInput.IsButtonState_Down(PadInput.GameInput.OptionButton)))
			{
				SetSkipVideo();
			}
		}
	}

	public void SetSkipVideo()
	{
		if (!PopupDialoguePlus.IsAnyPopupActivated())
		{
			PauseVideo();
			PopupDialoguePlus.ShowPopup_YesNo(GameGlobalUtil.GetXlsProgramText("VIDEO_SKIP_POPUP_TEXT"), CB_PopupExit);
		}
	}

	private void CB_PopupExit(PopupDialoguePlus.Result result)
	{
		if (result == PopupDialoguePlus.Result.Yes)
		{
			QuitPlay();
		}
		else
		{
			ResumeVideo();
		}
	}

	private void PauseVideo()
	{
		m_isPauseVideo = true;
		if (m_isPlayVideo)
		{
			m_videoPlayer.Pause();
		}
	}

	private void ResumeVideo()
	{
		m_isPauseVideo = false;
		if (m_isPlayVideo)
		{
			m_videoPlayer.Play();
		}
	}

	private void ProcWidowVideo()
	{
		if (m_videoPlayer == null)
		{
			return;
		}
		if (m_isPreparePlayVideo)
		{
			if (m_videoPlayer.isPrepared)
			{
				m_isPreparePlayVideo = false;
				m_goMovie.SetActive(value: true);
				m_videoPlayer.Play();
				m_videoPlayer.GetComponent<AudioSource>().Play();
				m_isPlayVideo = true;
			}
		}
		else if (m_isPlayVideo && !m_videoPlayer.isPlaying)
		{
			QuitPlay();
		}
	}

	public int GetSavePlaySoundSize(bool isOnlyLoadFile = false)
	{
		int num = 0;
		string text = null;
		for (int i = 0; i < 10; i++)
		{
			text = null;
			num += GameSwitch.SIZE_INT;
			if (m_asChannel[i].loop)
			{
				text = ((!isOnlyLoadFile) ? m_asData[i].m_strClipName : m_asData[i].m_strLoadName);
				num += BitCalc.GetStringToByteSize(text);
			}
			num += GameSwitch.SIZE_INT;
			num += GameSwitch.SIZE_INT;
			num += BitCalc.GetStringToByteSize(m_asData[i].m_strMixerGroupName);
			num += GameSwitch.SIZE_FLOAT;
		}
		int num2 = m_amMixer.Length;
		num += GameSwitch.SIZE_INT;
		for (int j = 0; j < num2; j++)
		{
			num += GameSwitch.SIZE_INT;
			num += BitCalc.GetStringToByteSize(m_miInfo[j].m_strSnapshotName);
		}
		return num;
	}

	public void SavePlaySound(byte[] bySaveBuf, ref int iOffset, int iBefVer = -1)
	{
		bool flag = false;
		for (int i = 0; i < 10; i++)
		{
			if (iBefVer != -1)
			{
				BitCalc.StringToByteWithSizeNCO((m_asData[i].m_strLoadName == null) ? null : m_asData[i].m_strLoadName, bySaveBuf, ref iOffset);
			}
			else
			{
				BitCalc.StringToByteWithSizeNCO((i < 0 || i > 4 || !m_asChannel[i].isPlaying || !m_asChannel[i].loop) ? null : m_asData[i].m_strClipName, bySaveBuf, ref iOffset);
			}
			BitCalc.IntToByteNCO(m_asData[i].m_iMixerIdx, bySaveBuf, ref iOffset);
			BitCalc.StringToByteWithSizeNCO(m_asData[i].m_strMixerGroupName, bySaveBuf, ref iOffset);
			BitCalc.FloatToByteNCO(m_asData[i].m_fScriptVol, bySaveBuf, ref iOffset);
		}
		int num = m_amMixer.Length;
		BitCalc.IntToByteNCO(num, bySaveBuf, ref iOffset);
		for (int j = 0; j < num; j++)
		{
			BitCalc.StringToByteWithSizeNCO(m_miInfo[j].m_strSnapshotName, bySaveBuf, ref iOffset);
		}
	}

	public void LoadPlaySound(byte[] byLoadBuf, ref int iOffset, bool isInit = false)
	{
		if (isInit)
		{
			InitAllClip();
		}
		for (int i = 0; i < 10; i++)
		{
			string strLoadName = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			m_asData[i].m_strLoadName = strLoadName;
			int num = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
			string text = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			if (text != null && num != -1)
			{
				SetOutputMixer(i, num, text);
			}
			SetVol(i, BitCalc.ByteToFloatNCO(byLoadBuf, ref iOffset), 0f);
		}
		int num2 = m_amMixer.Length;
		int num3 = BitCalc.ByteToIntNCO(byLoadBuf, ref iOffset);
		for (int j = 0; j < num3; j++)
		{
			if (j < num2)
			{
				string text2 = BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
				if (text2 != null)
				{
					SetSnapShot(j, text2, 0f);
				}
			}
			else
			{
				BitCalc.ByteToStringWithSizeNCO(byLoadBuf, ref iOffset);
			}
		}
	}

	public void BackupMixerForBacklog()
	{
		for (int i = 0; i < 10; i++)
		{
			m_iBackupMixerIdx[i] = m_asData[i].m_iMixerIdx;
			m_strBackupMixerGroupName[i] = m_asData[i].m_strMixerGroupName;
			SetOutputMixer(i, 0, null);
		}
	}

	public void RestoreMixerForBacklog()
	{
		for (int i = 0; i < 10; i++)
		{
			SetOutputMixer(i, m_iBackupMixerIdx[i], m_strBackupMixerGroupName[i]);
		}
	}

	public void BackupCurGamePlaySound()
	{
		m_byMenuBackup = null;
		int savePlaySoundSize = GetSavePlaySoundSize();
		m_byMenuBackup = new byte[savePlaySoundSize];
		int iOffset = 0;
		SavePlaySound(m_byMenuBackup, ref iOffset);
	}

	public void RestoreGamePlaySound()
	{
		if (m_byMenuBackup != null)
		{
			int iOffset = 0;
			LoadPlaySound(m_byMenuBackup, ref iOffset);
			InitAfterFirstLoad();
			m_byMenuBackup = null;
		}
	}

	public void InitAfterFirstLoad()
	{
		for (int i = 0; i < 10; i++)
		{
			if (i != 9 && m_asData[i].m_strLoadName != null)
			{
				StartCoroutine(PlayFileName(i, m_asData[i].m_strLoadName, isSetVol: false, 0f, m_asData[i].m_fVol, isLoop: true));
				m_asData[i].m_strLoadName = null;
			}
		}
	}

	public void InitOptSound()
	{
		m_fOptVol[0] = GameGlobalUtil.GetXlsProgramDefineStrToFloat("OPT_VOL_BGM");
		m_fOptVol[1] = GameGlobalUtil.GetXlsProgramDefineStrToFloat("OPT_VOL_EFF");
		m_fOptVol[2] = GameGlobalUtil.GetXlsProgramDefineStrToFloat("OPT_VOL_VOICE");
		for (int i = 0; i < 10; i++)
		{
			SetVol(i, 1f, 0f);
		}
	}

	public int GetOptSoundSize()
	{
		return 3 * GameSwitch.SIZE_FLOAT + 10 * GameSwitch.SIZE_FLOAT;
	}

	public void SaveOptSound(byte[] byArrData, ref int iOffset)
	{
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			BitCalc.FloatToByteNCO(m_fOptVol[i], byArrData, ref iOffset);
		}
		num = 10;
		for (int j = 0; j < num; j++)
		{
			BitCalc.FloatToByteNCO(m_asData[j].m_fScriptVol, byArrData, ref iOffset);
		}
	}

	public void LoadOptSound(byte[] byArrData, ref int iOffset)
	{
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			m_fOptVol[i] = BitCalc.ByteToFloatNCO(byArrData, ref iOffset);
		}
		num = 10;
		for (int j = 0; j < num; j++)
		{
			float fVol = BitCalc.ByteToFloatNCO(byArrData, ref iOffset);
			SetVol(j, fVol, 0f);
		}
	}

	public void SetOptVolume(eSND_VOLUME eSndVolType, float fVol)
	{
		if (fVol > 1f)
		{
			fVol = 1f;
		}
		m_fOptVol[(int)eSndVolType] = fVol;
		switch (eSndVolType)
		{
		case eSND_VOLUME.BGM:
			SetVol(0, m_asData[0].m_fScriptVol, 0f);
			break;
		case eSND_VOLUME.EFF:
			SetVol(1, m_asData[1].m_fScriptVol, 0f);
			SetVol(2, m_asData[2].m_fScriptVol, 0f);
			SetVol(3, m_asData[3].m_fScriptVol, 0f);
			SetVol(4, m_asData[4].m_fScriptVol, 0f);
			SetVol(7, m_asData[7].m_fScriptVol, 0f);
			break;
		case eSND_VOLUME.VOICE:
			SetVol(5, m_asData[5].m_fScriptVol, 0f);
			SetVol(6, m_asData[6].m_fScriptVol, 0f);
			break;
		}
	}

	public float GetOptVolume(eSND_VOLUME eSndVolType)
	{
		return m_fOptVol[(int)eSndVolType];
	}

	public void SetSkip(bool isSkip)
	{
		if (isSkip)
		{
			StopVoice();
		}
		SetSkipVol((!isSkip) ? 1 : 0);
		for (int i = 0; i < 9; i++)
		{
			if (m_asData[i].m_fTime < m_asData[i].m_fChangeVolTime)
			{
				SetChannelVol(i, m_asData[i].m_fNextVol);
				m_asData[i].m_fTime = (m_asData[i].m_fChangeVolTime = 0f);
			}
			SetVol(i, m_asData[i].m_fScriptVol, 0f, isForceSkipSet: true);
		}
		if (isSkip)
		{
			PlayUISound("Skip_Sound");
		}
		else
		{
			Stop(9);
		}
	}

	private void SetSkipVol(float fVol)
	{
		m_fSkipVol = fVol;
	}

	private string GetSoundFileName(string strSoundKey)
	{
		string result = string.Empty;
		Xls.SoundFile data_byKey = Xls.SoundFile.GetData_byKey(strSoundKey);
		if (data_byKey != null)
		{
			result = data_byKey.m_strFileName;
		}
		return result;
	}

	public int GetChannelIdx(string strChannel)
	{
		return GameGlobalUtil.GetXlsScriptKeyValue(strChannel);
	}

	public void Stop(string strChannel)
	{
		int channelIdx = GetChannelIdx(strChannel);
		if (channelIdx >= 0 && channelIdx < 10)
		{
			m_asChannel[channelIdx].Stop();
		}
	}

	public void Stop(int iChannel = 10)
	{
		switch (iChannel)
		{
		default:
			return;
		case 10:
		{
			for (int i = 0; i < 10; i++)
			{
				m_asChannel[i].Stop();
			}
			break;
		}
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
			m_asChannel[iChannel].Stop();
			break;
		}
		switch (iChannel)
		{
		case 5:
			if (m_cbFpEndVoice != null)
			{
				m_cbFpEndVoice(null, null);
			}
			break;
		case 6:
			if (m_cbFpEndReaction != null)
			{
				m_cbFpEndReaction(null, null);
			}
			break;
		}
		if (iChannel == 5 || iChannel == 6)
		{
			InitVoiceVar(iChannel);
		}
	}

	public void StopVoice()
	{
		Stop(5);
		Stop(6);
		RevertScriptVol();
	}

	public void PlayUISound(string strUIXlsKey)
	{
		Xls.UISound data_byKey = Xls.UISound.GetData_byKey(strUIXlsKey);
		if (data_byKey != null)
		{
			int channelIdx = GetChannelIdx(data_byKey.m_strChannel);
			StartCoroutine(PlayFileName(channelIdx, data_byKey.m_strFileName, isSetVol: true, 0f, data_byKey.m_fVol, data_byKey.m_iLoop != 0, isBefSoundStopForce: true));
		}
	}

	public void StopUISound(string strUIXlsKey)
	{
		Xls.UISound data_byKey = Xls.UISound.GetData_byKey(strUIXlsKey);
		if (data_byKey != null)
		{
			int channelIdx = GetChannelIdx(data_byKey.m_strChannel);
			Stop(channelIdx);
		}
	}

	public void Play(string strChannel, string strKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false)
	{
		int channelIdx = GetChannelIdx(strChannel);
		if (channelIdx >= 0 && channelIdx < 10)
		{
			PlayKey(channelIdx, strKey, isSetVol, fTime, fVol, isLoop);
		}
	}

	public void PlayKey(int iChannel, string strKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false)
	{
		string soundFileName = GetSoundFileName(strKey);
		StartCoroutine(PlayFileName(iChannel, soundFileName, isSetVol, fTime, fVol, isLoop));
	}

	public IEnumerator PlayKeyReturnEnumerator(int iChannel, string strKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false)
	{
		string strFileName = GetSoundFileName(strKey);
		yield return PlayFileName(iChannel, strFileName, isSetVol, fTime, fVol, isLoop);
	}

	private void ProcVoice()
	{
		if (!m_isPreparedVoice && !m_isPreparedReaction && !m_isPlayVoice && !m_isPlayReaction)
		{
			return;
		}
		if (m_isPreparedVoice && !m_isPlayVoice && m_asChannel[5].isPlaying)
		{
			m_isPlayVoice = true;
		}
		if (m_isPreparedReaction && !m_isPlayReaction && m_asChannel[6].isPlaying)
		{
			m_isPlayReaction = true;
		}
		if (m_isPlayVoice)
		{
			m_fPassVoiceTime += Time.deltaTime;
			if (m_fPassVoiceTime > m_fVoiceTime)
			{
				RevertScriptVol();
				if (m_EventEngine != null && !m_EventEngine.GetSkip())
				{
					m_isPlayVoice = false;
					m_isPreparedVoice = false;
					if (m_cbFpEndVoice != null)
					{
						m_cbFpEndVoice(null, null);
					}
				}
			}
		}
		if (!m_isPlayReaction)
		{
			return;
		}
		m_fPassReactionTime += Time.deltaTime;
		if (!(m_fPassReactionTime > m_fReactionTime))
		{
			return;
		}
		RevertScriptVol();
		if (m_EventEngine != null && !m_EventEngine.GetSkip())
		{
			m_isPlayReaction = false;
			m_isPreparedReaction = false;
			if (m_cbFpEndReaction != null)
			{
				m_cbFpEndReaction(null, null);
			}
		}
	}

	private void InitVoiceVar(int iChannel)
	{
		switch (iChannel)
		{
		case 5:
			m_cbFpEndVoice = null;
			m_isPlayVoice = false;
			m_isPreparedVoice = false;
			break;
		case 6:
			m_cbFpEndReaction = null;
			m_isPlayReaction = false;
			m_isPreparedReaction = false;
			break;
		}
	}

	private void RevertScriptVol()
	{
		if (m_EventEngine != null && !m_EventEngine.GetSkip())
		{
			for (int i = 0; i < 5; i++)
			{
				SetVol(i, m_asData[i].m_fScriptVol, 0f);
			}
		}
	}

	public void PlayVoice(string strVoiceKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false, GameDefine.EventProc cbFpVoiceEnd = null)
	{
		StartCoroutine(PlayVoice(5, strVoiceKey, isSetVol, fTime, fVol, isLoop, cbFpVoiceEnd));
	}

	public void PlayReaction(string strReactionKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false, GameDefine.EventProc cbFpReactionEnd = null)
	{
		StartCoroutine(PlayVoice(6, strReactionKey, isSetVol, fTime, fVol, isLoop, cbFpReactionEnd));
	}

	private IEnumerator PlayVoice(int iChannel, string strVoiceKey, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false, GameDefine.EventProc cbFpVoiceEnd = null)
	{
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
		Xls.Voice xlsVoice = Xls.Voice.GetData_byKey(strVoiceKey);
		if ((m_EventEngine != null && m_EventEngine.GetSkip()) || ((m_isPlayVoice || m_isPreparedVoice) && iChannel == 5) || ((m_isPlayReaction || m_isPreparedReaction) && iChannel == 6) || xlsVoice == null)
		{
			yield break;
		}
		string strFullFileName = null;
		string strFileName = null;
		string strFolderName = null;
		bool isPackFile = false;
		if (m_fOptVol[2] == 0f)
		{
			cbFpVoiceEnd?.Invoke(null, null);
			yield break;
		}
		switch (m_GameSwitch.GetVoiceLang())
		{
		case ConstGameSwitch.eVoiceLang.KOR:
			strFileName = xlsVoice.m_srtVoiceKo;
			strFolderName = "VOICE_KOR";
			break;
		case ConstGameSwitch.eVoiceLang.JPN:
			strFileName = xlsVoice.m_srtVoiceJp;
			strFolderName = "VOICE_JPN";
			break;
		}
		string strXlsFolderName = GameGlobalUtil.GetXlsProgramDefineStr(strFolderName);
		if (strXlsFolderName == null)
		{
			yield break;
		}
		strFullFileName = strXlsFolderName + strFileName;
		if (strFileName == null || strFileName == string.Empty)
		{
			yield break;
		}
		yield return StartCoroutine(PlayFileName(iChannel, strFullFileName, isSetVol, fTime, fVol, isLoop, isBefSoundStopForce: false, isPackFile));
		if ((iChannel == 5 || iChannel == 6) && m_asChannel[iChannel].clip != null && m_asChannel[iChannel].volume > 0f)
		{
			for (int i = 0; i < 5; i++)
			{
				SetVol(i, m_asData[i].m_fScriptVol, 0f, isForceSkipSet: false, isSetHalfBaseVol: false, isSetVoiceVol: true);
				SetChannelVol(i, m_asData[i].m_fNextVol);
			}
			if (iChannel == 5)
			{
				m_fPassVoiceTime = 0f;
				m_fVoiceTime = m_asChannel[iChannel].clip.length;
				m_isPreparedVoice = true;
				m_isPlayVoice = false;
				m_cbFpEndVoice = cbFpVoiceEnd;
			}
			else
			{
				m_fPassReactionTime = 0f;
				m_fReactionTime = m_asChannel[iChannel].clip.length;
				m_isPreparedReaction = true;
				m_isPlayReaction = false;
				m_cbFpEndReaction = cbFpVoiceEnd;
			}
		}
	}

	public IEnumerator PlayFileName(int iChannel, string strFileName, bool isSetVol = false, float fTime = 0f, float fVol = 0f, bool isLoop = false, bool isBefSoundStopForce = false, bool isPackFile = false)
	{
		if (strFileName.Equals(string.Empty))
		{
			yield break;
		}
		AudioSource asChannel = m_asChannel[iChannel];
		ASData asData = m_asData[iChannel];
		if (asData.m_isClipMaking)
		{
			yield break;
		}
		asData.m_isClipMaking = true;
		bool isStopAndPlay = asChannel.isPlaying && !asData.m_strClipName.Equals(strFileName);
		if (isBefSoundStopForce || isStopAndPlay)
		{
			asChannel.Stop();
		}
		if (!asData.m_strClipName.Equals(strFileName))
		{
			bool isRelease = true;
			string strUnloadBundleName = asData.m_strClipName;
			for (int i = 0; i < 10; i++)
			{
				if (i != iChannel && m_asData[i].m_strClipName == strUnloadBundleName)
				{
					isRelease = false;
					break;
				}
			}
			if (isRelease)
			{
				if (!string.IsNullOrEmpty(strUnloadBundleName))
				{
					AssetBundleManager.UnloadAssetBundle(Path.Combine("Sounds/", strUnloadBundleName), asData.m_isOneFileBundle);
				}
				Resources.UnloadAsset(asChannel.clip);
			}
			asChannel.clip = null;
			m_asData[iChannel].m_strClipName = null;
			bool isNeedLoad = true;
			for (int j = 0; j < 10; j++)
			{
				if (j != iChannel && m_asData[j].m_strClipName == strFileName)
				{
					asChannel.clip = m_asChannel[j].clip;
					isNeedLoad = false;
					break;
				}
			}
			if (isNeedLoad)
			{
				string strPath = Path.Combine("Sounds/", strFileName);
				AssetBundleLoadAssetOperation request = null;
				request = AssetBundleManager.LoadAssetAsync(strPath, typeof(AudioClip));
				if (request == null)
				{
					asData.m_strClipName = string.Empty;
					asData.m_isClipMaking = false;
					yield break;
				}
				while (!request.IsDone())
				{
					yield return null;
				}
				asChannel.clip = request.GetAsset<AudioClip>();
			}
		}
		if (isStopAndPlay || !asChannel.isPlaying)
		{
			asChannel.Play();
		}
		asData.m_strClipName = strFileName;
		if (isSetVol)
		{
			SetVol(iChannel, fVol, fTime);
		}
		asChannel.loop = isLoop;
		asData.m_isOneFileBundle = !isPackFile;
		asData.m_isClipMaking = false;
	}

	public void PauseSound(string strChannel)
	{
		int channelIdx = GetChannelIdx(strChannel);
		PauseChannel(channelIdx);
	}

	public void PauseChannel(int iChannel)
	{
		if (iChannel >= 0 && iChannel < 10)
		{
			m_asChannel[iChannel].Pause();
		}
	}

	public bool IsPlayingChannel(int iChannel)
	{
		if (m_asChannel[iChannel] == null)
		{
			return false;
		}
		return m_asChannel[iChannel].isPlaying;
	}

	public void SetVol(string strChannel, float fVol, float fTime)
	{
		int channelIdx = GetChannelIdx(strChannel);
		if (channelIdx >= 0 && channelIdx < 10)
		{
			SetVol(channelIdx, fVol, fTime);
		}
	}

	public void SetVol(int iChannel, float fVol, float fTime, bool isForceSkipSet = false, bool isSetHalfBaseVol = false, bool isSetVoiceVol = false)
	{
		float num = 1f;
		switch (iChannel)
		{
		case 0:
			num = m_fOptVol[0];
			break;
		case 1:
		case 2:
		case 3:
		case 4:
		case 7:
		case 8:
		case 9:
			num = m_fOptVol[1];
			break;
		case 5:
		case 6:
			num = m_fOptVol[2];
			break;
		}
		if (isSetHalfBaseVol)
		{
			num *= 0.1f;
		}
		else if (isSetVoiceVol)
		{
			num *= m_fBGVolPercentPlayVoice;
		}
		if (iChannel != 9)
		{
			num *= m_fSkipVol;
		}
		float fValue = fVol * num;
		GameSwitch.CheckMinMax(ref fValue, 0f, 1f);
		m_asData[iChannel].m_fScriptVol = fVol;
		if (isForceSkipSet || (m_EventEngine != null && !m_EventEngine.GetSkip()) || m_EventEngine == null)
		{
			if (fTime <= 0f)
			{
				m_asData[iChannel].m_fNextVol = fValue;
				SetChannelVol(iChannel, fValue);
			}
			else
			{
				m_asData[iChannel].m_fFirstVol = m_asChannel[iChannel].volume;
				m_asData[iChannel].m_fNextVol = fValue;
				m_asData[iChannel].m_fChangeVolTime = fTime;
				m_asData[iChannel].m_fTime = 0f;
			}
		}
		if (iChannel == 7)
		{
			SetVol(8, fVol, fTime);
			SetVol(9, fVol, fTime);
		}
	}

	private void SetChannelVol(int iChannel, float fVol)
	{
		m_asChannel[iChannel].volume = fVol;
	}

	public void SetBgVolHalfAndStopTheOtherSnds()
	{
		int num = 0;
		SetVol(num, m_asData[num].m_fScriptVol, 0f, isForceSkipSet: false, isSetHalfBaseVol: true);
		for (int i = 0; i < 10; i++)
		{
			if (i != num)
			{
				Stop(i);
			}
		}
	}

	private void ProcVol()
	{
		if (m_EventEngine != null && m_EventEngine.GetSkip())
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			if (m_asData != null && m_asData[i] != null && m_asChannel != null && (m_asData == null || m_asData[i] == null || !(m_asChannel[i].volume >= m_asData[i].m_fNextVol) || !(m_asData[i].m_fTime >= m_asData[i].m_fChangeVolTime)))
			{
				m_asData[i].m_fTime += Time.deltaTime;
				SetChannelVol(i, Mathf.Lerp(m_asData[i].m_fFirstVol, m_asData[i].m_fNextVol, m_asData[i].m_fTime / m_asData[i].m_fChangeVolTime));
			}
		}
	}

	public void SetOutputMixer(string strChannel, int iMixerIdx, string strMixerGroupName)
	{
		int channelIdx = GetChannelIdx(strChannel);
		SetOutputMixer(channelIdx, iMixerIdx, strMixerGroupName);
	}

	public void SetOutputMixer(int iChannel, int iMixerIdx, string strMixerGroupName)
	{
		if (iChannel < 0 || iChannel >= 10)
		{
			return;
		}
		bool flag = false;
		if (strMixerGroupName != string.Empty && iMixerIdx >= 0 && iMixerIdx < m_amMixer.Length && m_amMixer[iMixerIdx] != null)
		{
			AudioMixerGroup[] array = m_amMixer[iMixerIdx].FindMatchingGroups(strMixerGroupName);
			if (array.Length > 0)
			{
				m_asData[iChannel].m_strMixerGroupName = strMixerGroupName;
				m_asData[iChannel].m_iMixerIdx = iMixerIdx;
				m_asChannel[iChannel].outputAudioMixerGroup = array[0];
				flag = true;
			}
		}
		if (!flag)
		{
			m_asData[iChannel].m_strMixerGroupName = null;
			m_asData[iChannel].m_iMixerIdx = -1;
			m_asChannel[iChannel].outputAudioMixerGroup = null;
		}
	}

	public void SetSnapShot(int iMixerIdx, string strSnapshotName, float fTransitionTime)
	{
		if (iMixerIdx >= 0 && iMixerIdx < m_amMixer.Length && !(m_amMixer[iMixerIdx] == null))
		{
			AudioMixerSnapshot audioMixerSnapshot = m_amMixer[iMixerIdx].FindSnapshot(strSnapshotName);
			if (!(audioMixerSnapshot == null))
			{
				audioMixerSnapshot.TransitionTo(fTransitionTime);
				m_miInfo[iMixerIdx].m_strSnapshotName = strSnapshotName;
			}
		}
	}

	private int GetXlsVideoLanguage()
	{
		int result = 0;
		string key = "SUBS_LANG_KOR";
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
		switch (m_GameSwitch.GetCurSubtitleLanguage())
		{
		case SystemLanguage.Korean:
			key = "SUBS_LANG_KOR";
			break;
		case SystemLanguage.English:
			key = "SUBS_LANG_ENG";
			break;
		case SystemLanguage.Japanese:
			key = "SUBS_LANG_JPN";
			break;
		case SystemLanguage.Chinese:
			key = "SUBS_LANG_CHN";
			break;
		}
		Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey(key);
		if (data_byKey != null)
		{
			result = int.Parse(data_byKey.m_strTxt);
		}
		return result;
	}

	public void PlayVideo(string strVideoKey)
	{
		Xls.VideoFile data_byKey = Xls.VideoFile.GetData_byKey(strVideoKey);
		BackupCurGamePlaySound();
		Stop();
		float volume = GetOptVolume(eSND_VOLUME.BGM) * 0.43f;
		string text = data_byKey.m_strKrVideoFileName;
		switch (GetVideoNation(strVideoKey))
		{
		case eVIDEO_NATION.JAPAN:
			text = data_byKey.m_strJpVideoFileName;
			break;
		case eVIDEO_NATION.ENGLISH:
			text = data_byKey.m_strEnVideoFileName;
			break;
		case eVIDEO_NATION.CHINA_SIMPLE:
			text = data_byKey.m_strSmpCnVideoFileName;
			break;
		case eVIDEO_NATION.CHINA_TRADITIONAL:
			text = data_byKey.m_strTrdCnVideoFileName;
			break;
		}
		m_videoPlayer.gameObject.SetActive(value: true);
		if (m_rtVideoPlayer == null)
		{
			m_rtVideoPlayer = new RenderTexture(Screen.width, Screen.height, 24);
		}
		m_videoPlayer.targetTexture = m_rtVideoPlayer;
		m_goMovie.GetComponent<RawImage>().texture = m_rtVideoPlayer;
		m_videoPlayer.source = VideoSource.Url;
		AudioSource component = m_videoPlayer.GetComponent<AudioSource>();
		m_videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		m_videoPlayer.EnableAudioTrack(0, enabled: true);
		m_videoPlayer.controlledAudioTrackCount = 1;
		m_videoPlayer.SetTargetAudioSource(0, component);
		component.volume = volume;
		string xlsProgramDefineStr = GameGlobalUtil.GetXlsProgramDefineStr("VIDEO_DEF_FOLDER");
		if (xlsProgramDefineStr != null)
		{
			xlsProgramDefineStr = Path.Combine(Application.streamingAssetsPath, xlsProgramDefineStr);
			text = text + "." + data_byKey.m_strFileExtension;
			string url = Path.Combine(xlsProgramDefineStr, text);
			m_videoPlayer.url = url;
			m_videoPlayer.isLooping = false;
			component.playOnAwake = true;
			m_videoPlayer.playOnAwake = true;
			m_videoPlayer.waitForFirstFrame = false;
			m_videoPlayer.Prepare();
			m_isPreparePlayVideo = true;
		}
	}

	private void QuitPlay(bool isNeedStop = true)
	{
		m_videoPlayer.gameObject.SetActive(value: false);
		m_goMovie.SetActive(value: false);
		m_rtVideoPlayer = null;
		RestoreGamePlaySound();
		m_isPauseVideo = false;
		m_isPlayVideo = false;
		Resources.UnloadUnusedAssets();
	}

	public bool IsPlayingVideo()
	{
		return !m_isPlayVideo && !m_isPreparePlayVideo;
	}

	public eVIDEO_NATION GetVideoNation(string strVideoKey)
	{
		eVIDEO_NATION result = eVIDEO_NATION.KOREAN;
		if (m_GameSwitch == null)
		{
			m_GameSwitch = GameSwitch.GetInstance();
		}
		if (strVideoKey == "video_00000001")
		{
			result = m_GameSwitch.GetCurSubtitleLanguage() switch
			{
				SystemLanguage.English => eVIDEO_NATION.ENGLISH, 
				SystemLanguage.Japanese => eVIDEO_NATION.JAPAN, 
				SystemLanguage.ChineseSimplified => eVIDEO_NATION.CHINA_SIMPLE, 
				SystemLanguage.ChineseTraditional => eVIDEO_NATION.CHINA_TRADITIONAL, 
				_ => eVIDEO_NATION.KOREAN, 
			};
		}
		else
		{
			switch (m_GameSwitch.GetVoiceLang())
			{
			case ConstGameSwitch.eVoiceLang.KOR:
				result = eVIDEO_NATION.KOREAN;
				break;
			case ConstGameSwitch.eVoiceLang.JPN:
				result = eVIDEO_NATION.JAPAN;
				break;
			}
		}
		return result;
	}
}
