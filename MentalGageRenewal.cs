using System;
using System.Collections;
using GameData;
using GameEvent;
using UnityEngine;

public class MentalGageRenewal : MonoBehaviour
{
	public enum Level
	{
		Broken,
		Level_1,
		Level_2,
		Level_3,
		Level_4,
		Level_5,
		Beyond
	}

	private enum AlertState
	{
		Normal,
		Caution,
		Danger
	}

	private enum ChangedMentalMode
	{
		None,
		ValueUp,
		ValueDown,
		LevelUp,
		LevelDown,
		ChainLevelUp,
		ChainLevelDown,
		ToBeyond,
		ToBroken
	}

	private enum DirectionStep
	{
		None,
		GageAppear,
		GageIdle,
		GageDisappear,
		ChangedMarkShow,
		ChangedMarkShowBeyond,
		ChangedMarkShowBroken,
		Tutorial
	}

	private enum MainAnimationState
	{
		appear,
		idle,
		disappear,
		change_appear,
		change_idle,
		change_disappear,
		broken_beyond_appear,
		broken_beyond_idle,
		broken_beyond_disappear
	}

	private enum HeartIconAnimationState
	{
		idle_normal,
		idle_caution,
		idle_danger,
		up_normal,
		up_caution,
		up_danger,
		up_normal_chain,
		up_caution_chain,
		up_danger_chain,
		down_normal,
		down_caution,
		down_danger,
		down_normal_chain,
		down_caution_chain,
		down_danger_chain,
		beyond_appear,
		beyond_idle,
		broken_appear,
		broken_idle
	}

	public enum InBlockLevel
	{
		less = 0,
		less2 = 1,
		less3 = 2,
		less4 = 3,
		Full = 4,
		Count = 5,
		Empty = -1,
		Unknown = -2
	}

	private enum BlockAnimationState
	{
		less_normal,
		less2_normal,
		less3_normal,
		less4_normal,
		full_normal,
		less_caution,
		less2_caution,
		less3_caution,
		less4_caution,
		full_caution,
		less_danger,
		less2_danger,
		less3_danger,
		less4_danger,
		full_danger,
		empty,
		beyond_appear,
		beyond,
		broken_appear,
		broken
	}

	[Serializable]
	public class LevelBlock
	{
		public GameObject m_ChangeInfoRoot;

		public Animator m_ChangeSmallUpAni;

		public Animator m_ChangeSmallDownAni;

		public Animator m_ChangeLevelUpAni;

		public Animator m_ChangeLevelDownAni;

		public Animator m_ChangeChainLevelUpAni;

		public Animator m_ChangeChainLevelDownAni;

		public Animator m_ChangeSpecialLevelAni;

		public GameObject m_BlockRoot;

		public Animator m_BlockAnimator;
	}

	private delegate void BaseDelegate();

	public const int c_MaxLevel = 4;

	private readonly string c_MainAniState_Appear = MainAnimationState.appear.ToString();

	private readonly string c_MainAniState_Idle = MainAnimationState.idle.ToString();

	private readonly string c_MainAniState_Disappear = MainAnimationState.disappear.ToString();

	private readonly string c_MainAniState_ChangeAppear = MainAnimationState.change_appear.ToString();

	private readonly string c_MainAniState_ChangeIdle = MainAnimationState.change_idle.ToString();

	private readonly string c_MainAniState_ChangeDisappear = MainAnimationState.change_disappear.ToString();

	private readonly string c_MainAniState_BrokenBeyondAppear = MainAnimationState.broken_beyond_appear.ToString();

	private readonly string c_MainAniState_BrokenBeyondIdle = MainAnimationState.broken_beyond_idle.ToString();

	private readonly string c_MainAniState_BrokenBeyondDisappear = MainAnimationState.broken_beyond_disappear.ToString();

	private readonly string c_HeartIconAniState_IdleNormal = HeartIconAnimationState.idle_normal.ToString();

	private readonly string c_HeartIconAniState_IdleCaution = HeartIconAnimationState.idle_caution.ToString();

	private readonly string c_HeartIconAniState_IdleDanger = HeartIconAnimationState.idle_danger.ToString();

	private readonly string c_HeartIconAniState_UpNormal = HeartIconAnimationState.up_normal.ToString();

	private readonly string c_HeartIconAniState_UpCaution = HeartIconAnimationState.up_caution.ToString();

	private readonly string c_HeartIconAniState_UpDanger = HeartIconAnimationState.up_danger.ToString();

	private readonly string c_HeartIconAniState_UpNormalChain = HeartIconAnimationState.up_normal_chain.ToString();

	private readonly string c_HeartIconAniState_UpCautionChain = HeartIconAnimationState.up_caution_chain.ToString();

	private readonly string c_HeartIconAniState_UpDangerChain = HeartIconAnimationState.up_danger_chain.ToString();

	private readonly string c_HeartIconAniState_DownNormal = HeartIconAnimationState.down_normal.ToString();

	private readonly string c_HeartIconAniState_DownCaution = HeartIconAnimationState.down_caution.ToString();

	private readonly string c_HeartIconAniState_DownDanger = HeartIconAnimationState.down_danger.ToString();

	private readonly string c_HeartIconAniState_DownNormalChain = HeartIconAnimationState.down_normal_chain.ToString();

	private readonly string c_HeartIconAniState_DownCautionChain = HeartIconAnimationState.down_caution_chain.ToString();

	private readonly string c_HeartIconAniState_DownDangerChain = HeartIconAnimationState.down_danger_chain.ToString();

	private readonly string c_HeartIconAniState_BeyondAppear = HeartIconAnimationState.beyond_appear.ToString();

	private readonly string c_HeartIconAniState_BeyondIdle = HeartIconAnimationState.beyond_idle.ToString();

	private readonly string c_HeartIconAniState_BrokenAppear = HeartIconAnimationState.broken_appear.ToString();

	private readonly string c_HeartIconAniState_BrokenIdle = HeartIconAnimationState.broken_idle.ToString();

	private readonly string[] c_BlockAniStates_Normal = new string[5]
	{
		BlockAnimationState.less_normal.ToString(),
		BlockAnimationState.less2_normal.ToString(),
		BlockAnimationState.less3_normal.ToString(),
		BlockAnimationState.less4_normal.ToString(),
		BlockAnimationState.full_normal.ToString()
	};

	private readonly string[] c_BlockAniStates_Caution = new string[5]
	{
		BlockAnimationState.less_caution.ToString(),
		BlockAnimationState.less2_caution.ToString(),
		BlockAnimationState.less3_caution.ToString(),
		BlockAnimationState.less4_caution.ToString(),
		BlockAnimationState.full_caution.ToString()
	};

	private readonly string[] c_BlockAniStates_Danger = new string[5]
	{
		BlockAnimationState.less_danger.ToString(),
		BlockAnimationState.less2_danger.ToString(),
		BlockAnimationState.less3_danger.ToString(),
		BlockAnimationState.less4_danger.ToString(),
		BlockAnimationState.full_danger.ToString()
	};

	private readonly string c_BlockAniState_Empty = BlockAnimationState.empty.ToString();

	private readonly string c_BlockAniState_BeyondAppear = BlockAnimationState.beyond_appear.ToString();

	private readonly string c_BlockAniState_Beyond = BlockAnimationState.beyond.ToString();

	private readonly string c_BlockAniState_BrokenAppear = BlockAnimationState.broken_appear.ToString();

	private readonly string c_BlockAniState_Broken = BlockAnimationState.broken.ToString();

	public Animator m_GageMainAnimator;

	public GameObject m_HeartIconRoot;

	public Animator m_HeartIconAnimator;

	private HeartIconAnimationState m_HeartIconAniState;

	public LevelBlock[] m_LevelBlocks;

	private Level m_CurrentLevel = Level.Level_1;

	private Level m_PrevLevel = Level.Level_1;

	private Level m_DirectionLevel = Level.Level_1;

	private InBlockLevel m_inBlockLevel = InBlockLevel.Empty;

	private InBlockLevel m_inBlockLevelPrev = InBlockLevel.Empty;

	private bool m_isChangedLevel;

	private bool m_isChangedTwoOverLevels;

	private ChangedMentalMode m_ChangedMentalMode;

	private AlertState m_AlertState;

	private DirectionStep m_DirectionStep;

	private string m_curGageDirectionAniName = string.Empty;

	private Animator m_ChangeMarkAnimator;

	private GameDefine.EventProc m_fpHidedCB;

	private BaseDelegate m_fpUpdate;

	private AudioManager m_AudioManager;

	private const int c_CanvasOrder = 150;

	private Canvas m_Canvas;

	private const string c_PrefabAssetPath = "Prefabs/InGame/Game/UI_MentalGageRenewal";

	private static GameObject s_SrcObject;

	public static MentalGageRenewal s_Instance;

	public const int c_PointPerLevel = 20;

	public const int c_LessLevelBound = 5;

	public const int c_Less2LevelBound = 10;

	public const int c_Less3LevelBound = 15;

	public const int c_Less4LevelBound = 20;

	public int canvasOrder
	{
		get
		{
			return (m_Canvas != null) ? m_Canvas.sortingOrder : 0;
		}
		set
		{
			if (m_Canvas == null)
			{
				m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
			}
			m_Canvas.sortingOrder = value;
		}
	}

	private void Init()
	{
		if (s_Instance == null)
		{
			s_Instance = this;
		}
		if (m_AudioManager == null)
		{
			m_AudioManager = GameGlobalUtil.GetAudioManager();
		}
		if (m_Canvas == null)
		{
			m_Canvas = base.gameObject.GetComponentInChildren<Canvas>();
		}
	}

	private void OnDestroy()
	{
		m_ChangeMarkAnimator = null;
		m_fpHidedCB = null;
		m_fpUpdate = null;
		s_SrcObject = null;
		m_Canvas = null;
		m_AudioManager = null;
		s_Instance = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		EventEngine instance = EventEngine.GetInstance();
		float speed = ((instance == null || !instance.GetSkip()) ? 1f : instance.GetRelationAnimatorSkipValue());
		if (m_GageMainAnimator != null)
		{
			m_GageMainAnimator.speed = speed;
		}
		if (m_ChangeMarkAnimator != null)
		{
			m_ChangeMarkAnimator.speed = speed;
		}
		if (m_HeartIconAnimator != null)
		{
			m_HeartIconAnimator.speed = speed;
		}
		if (m_fpUpdate != null)
		{
			m_fpUpdate();
		}
	}

	private void UpdateIdleState()
	{
		if (m_GageMainAnimator != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = m_GageMainAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(c_MainAniState_Disappear) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
			{
				CompleteHided();
			}
		}
	}

	private void UpdateChangedState()
	{
		switch (m_DirectionStep)
		{
		case DirectionStep.GageAppear:
			if (m_GageMainAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo4 = m_GageMainAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo4.IsName(m_curGageDirectionAniName) && currentAnimatorStateInfo4.normalizedTime >= 0.99f)
				{
					DirectionStep directionStep = DirectionStep.GageIdle;
					ChangeDirectionStep(m_DirectionLevel switch
					{
						Level.Beyond => DirectionStep.ChangedMarkShowBeyond, 
						Level.Broken => DirectionStep.ChangedMarkShowBroken, 
						_ => DirectionStep.ChangedMarkShow, 
					});
				}
			}
			break;
		case DirectionStep.GageIdle:
			if (m_GageMainAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo5 = m_GageMainAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo5.IsName(m_curGageDirectionAniName) && currentAnimatorStateInfo5.normalizedTime >= 0.99f)
				{
					ChangeDirectionStep(DirectionStep.GageDisappear);
				}
			}
			break;
		case DirectionStep.GageDisappear:
			if (m_GageMainAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo2 = m_GageMainAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo2.IsName(m_curGageDirectionAniName) && currentAnimatorStateInfo2.normalizedTime >= 0.99f)
				{
					CompleteHided();
					return;
				}
			}
			break;
		case DirectionStep.ChangedMarkShow:
		{
			if (!(m_ChangeMarkAnimator != null))
			{
				break;
			}
			AnimatorStateInfo currentAnimatorStateInfo3 = m_ChangeMarkAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo3.IsName(c_MainAniState_Appear) && currentAnimatorStateInfo3.normalizedTime >= 0.99f)
			{
				m_ChangeMarkAnimator.Play(c_MainAniState_Idle);
				if (m_ChangedMentalMode != ChangedMentalMode.None && m_ChangedMentalMode != ChangedMentalMode.ValueDown && m_ChangedMentalMode != ChangedMentalMode.ValueUp)
				{
					m_AlertState = GetAlertState(m_DirectionLevel);
					if (m_isChangedTwoOverLevels)
					{
						SetHeartIconState(m_DirectionLevel, m_ChangedMentalMode, isUseChainAni: true);
					}
				}
				if (m_CurrentLevel == Level.Beyond)
				{
					SetLevelBlocks(m_DirectionLevel, InBlockLevel.Full);
				}
				else
				{
					SetLevelBlocks(m_DirectionLevel, (m_DirectionLevel != m_CurrentLevel) ? InBlockLevel.Unknown : m_inBlockLevel);
				}
			}
			else
			{
				if (!currentAnimatorStateInfo3.IsName(c_MainAniState_Disappear) || !(currentAnimatorStateInfo3.normalizedTime >= 0.99f))
				{
					break;
				}
				m_ChangeMarkAnimator.gameObject.SetActive(value: false);
				if (m_DirectionLevel == m_CurrentLevel)
				{
					ChangeDirectionStep(DirectionStep.GageDisappear);
					break;
				}
				SetNextDirectionLevel(m_DirectionLevel);
				if (m_DirectionLevel == Level.Broken || m_DirectionLevel == Level.Beyond)
				{
					ChangeDirectionStep((m_DirectionLevel != Level.Beyond) ? DirectionStep.ChangedMarkShowBroken : DirectionStep.ChangedMarkShowBeyond);
				}
				else
				{
					ChangeDirectionStep(DirectionStep.ChangedMarkShow, isIgnoreSame: false);
				}
			}
			break;
		}
		case DirectionStep.ChangedMarkShowBeyond:
		case DirectionStep.ChangedMarkShowBroken:
			if (m_ChangeMarkAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_ChangeMarkAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(c_MainAniState_Appear) & (currentAnimatorStateInfo.normalizedTime >= 0.99f))
				{
					m_ChangeMarkAnimator.Play(c_MainAniState_Idle);
					SetLevelBlocks(m_DirectionLevel, InBlockLevel.Unknown);
					SetHeartIconState(m_DirectionLevel, m_ChangedMentalMode);
				}
				else if (currentAnimatorStateInfo.IsName(c_MainAniState_Disappear) && currentAnimatorStateInfo.normalizedTime >= 0.99f)
				{
					ChangeDirectionStep(DirectionStep.GageIdle);
				}
			}
			break;
		}
		if (m_HeartIconAnimator != null && m_HeartIconAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
		{
			switch (m_HeartIconAniState)
			{
			case HeartIconAnimationState.up_normal:
			case HeartIconAnimationState.up_normal_chain:
				PlayHeartIconAnimation(c_HeartIconAniState_IdleNormal);
				break;
			case HeartIconAnimationState.up_caution:
			case HeartIconAnimationState.up_caution_chain:
				PlayHeartIconAnimation((!m_isChangedLevel || m_AlertState != AlertState.Normal) ? c_HeartIconAniState_IdleCaution : c_HeartIconAniState_IdleNormal);
				break;
			case HeartIconAnimationState.up_danger:
			case HeartIconAnimationState.up_danger_chain:
				PlayHeartIconAnimation((!m_isChangedLevel || m_AlertState != AlertState.Caution) ? c_HeartIconAniState_IdleDanger : c_HeartIconAniState_IdleCaution);
				break;
			case HeartIconAnimationState.down_normal:
			case HeartIconAnimationState.down_normal_chain:
				PlayHeartIconAnimation((!m_isChangedLevel || m_AlertState != AlertState.Caution) ? c_HeartIconAniState_IdleNormal : c_HeartIconAniState_IdleCaution);
				break;
			case HeartIconAnimationState.down_caution:
			case HeartIconAnimationState.down_caution_chain:
				PlayHeartIconAnimation((!m_isChangedLevel || m_AlertState != AlertState.Danger) ? c_HeartIconAniState_IdleCaution : c_HeartIconAniState_IdleDanger);
				break;
			case HeartIconAnimationState.down_danger:
			case HeartIconAnimationState.down_danger_chain:
				PlayHeartIconAnimation(c_HeartIconAniState_IdleDanger);
				break;
			}
		}
	}

	private void OnClosed_TutorialPopup(object sender, object arg)
	{
		ChangeDirectionStep((m_DirectionLevel != Level.Beyond && m_DirectionLevel != Level.Broken) ? DirectionStep.ChangedMarkShow : DirectionStep.GageIdle);
	}

	public void ShowIdleState()
	{
		m_fpUpdate = UpdateIdleState;
		base.gameObject.SetActive(value: true);
		if (m_GageMainAnimator != null)
		{
			m_GageMainAnimator.Play(c_MainAniState_Appear);
		}
	}

	public void Hide(GameDefine.EventProc fpHidedCB = null)
	{
		m_fpHidedCB = ((fpHidedCB == null) ? null : new GameDefine.EventProc(fpHidedCB.Invoke));
		m_fpUpdate = UpdateIdleState;
		if (m_GageMainAnimator != null && m_GageMainAnimator.gameObject.activeInHierarchy)
		{
			m_GageMainAnimator.Play(c_MainAniState_Disappear);
		}
	}

	private void CompleteHided()
	{
		m_ChangedMentalMode = ChangedMentalMode.None;
		base.gameObject.SetActive(value: false);
		if (m_fpHidedCB != null)
		{
			m_fpHidedCB(this, null);
			m_fpHidedCB = null;
		}
	}

	public void SetMentalLevel(Level currentLevel, InBlockLevel inBlockLevel, bool isShow = true)
	{
		InitLevelBlocks();
		if (isShow)
		{
			ShowIdleState();
		}
		m_Canvas.sortingOrder = 0;
		m_CurrentLevel = currentLevel;
		m_PrevLevel = currentLevel;
		m_inBlockLevel = inBlockLevel;
		m_ChangedMentalMode = ChangedMentalMode.None;
		m_curGageDirectionAniName = string.Empty;
		m_ChangeMarkAnimator = null;
		m_AlertState = GetAlertState(m_CurrentLevel);
		SetLevelBlocks(m_CurrentLevel, m_inBlockLevel);
		SetHeartIconState(m_CurrentLevel, m_ChangedMentalMode);
	}

	public void SetMentalLevel_byPoint(int currentPoint, bool isShow = true)
	{
		InBlockLevel inBlockLevel;
		Level mentalLevel_byMentalPoint = GetMentalLevel_byMentalPoint(currentPoint, out inBlockLevel);
		SetMentalLevel(mentalLevel_byMentalPoint, inBlockLevel, isShow);
	}

	public void SetMentalLevel_byCurrentMentalPoint(bool isShow = true)
	{
		GameSwitch instance = GameSwitch.GetInstance();
		if (instance != null)
		{
			SetMentalLevel_byPoint(instance.GetMental());
		}
	}

	public void ShowChangedState(Level currentLevel, Level prevLevel, bool isUp, InBlockLevel inBlockLevel, InBlockLevel inBlockLevelPrev, GameDefine.EventProc fpHidedCB = null)
	{
		InitLevelBlocks();
		base.gameObject.SetActive(value: true);
		m_Canvas.sortingOrder = 150;
		m_CurrentLevel = currentLevel;
		m_PrevLevel = prevLevel;
		m_inBlockLevel = inBlockLevel;
		m_inBlockLevelPrev = inBlockLevelPrev;
		m_isChangedLevel = m_CurrentLevel != m_PrevLevel;
		m_isChangedTwoOverLevels = Mathf.Abs(m_CurrentLevel - m_PrevLevel) > 1;
		m_fpHidedCB = ((fpHidedCB == null) ? null : new GameDefine.EventProc(fpHidedCB.Invoke));
		switch (m_CurrentLevel)
		{
		case Level.Broken:
			m_ChangedMentalMode = ChangedMentalMode.ToBroken;
			break;
		case Level.Beyond:
			m_ChangedMentalMode = ChangedMentalMode.ToBeyond;
			break;
		default:
			m_ChangedMentalMode = ((m_CurrentLevel == m_PrevLevel) ? (isUp ? ChangedMentalMode.ValueUp : ChangedMentalMode.ValueDown) : ((m_CurrentLevel > m_PrevLevel) ? ((!m_isChangedTwoOverLevels) ? ChangedMentalMode.LevelUp : ChangedMentalMode.ChainLevelUp) : ((!m_isChangedTwoOverLevels) ? ChangedMentalMode.LevelDown : ChangedMentalMode.ChainLevelDown)));
			break;
		}
		m_curGageDirectionAniName = string.Empty;
		m_ChangeMarkAnimator = null;
		SetNextDirectionLevel(m_PrevLevel);
		m_AlertState = GetAlertState(m_PrevLevel);
		SetLevelBlocks(m_PrevLevel, inBlockLevelPrev);
		SetHeartIconState(m_PrevLevel, (!m_isChangedTwoOverLevels) ? m_ChangedMentalMode : ChangedMentalMode.None);
		ChangeDirectionStep(DirectionStep.GageAppear, isIgnoreSame: false);
		m_fpUpdate = UpdateChangedState;
	}

	public void ShowChangedState_byMentalPoint(int curMentalPoint, int prevMentalPoint, GameDefine.EventProc fpHidedCB = null)
	{
		if (curMentalPoint != prevMentalPoint)
		{
			bool isUp = curMentalPoint > prevMentalPoint;
			InBlockLevel inBlockLevel;
			Level mentalLevel_byMentalPoint = GetMentalLevel_byMentalPoint(curMentalPoint, out inBlockLevel);
			InBlockLevel inBlockLevel2;
			Level mentalLevel_byMentalPoint2 = GetMentalLevel_byMentalPoint(prevMentalPoint, out inBlockLevel2);
			ShowChangedState(mentalLevel_byMentalPoint, mentalLevel_byMentalPoint2, isUp, inBlockLevel, inBlockLevel2, fpHidedCB);
		}
	}

	private void InitLevelBlocks()
	{
		if (m_LevelBlocks == null || m_LevelBlocks.Length <= 0)
		{
			return;
		}
		LevelBlock[] levelBlocks = m_LevelBlocks;
		foreach (LevelBlock levelBlock in levelBlocks)
		{
			if (levelBlock != null)
			{
				if (levelBlock.m_ChangeInfoRoot != null)
				{
					levelBlock.m_ChangeInfoRoot.SetActive(value: true);
				}
				if (levelBlock.m_ChangeSmallUpAni != null)
				{
					levelBlock.m_ChangeSmallUpAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeSmallDownAni != null)
				{
					levelBlock.m_ChangeSmallDownAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeLevelUpAni != null)
				{
					levelBlock.m_ChangeLevelUpAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeLevelDownAni != null)
				{
					levelBlock.m_ChangeLevelDownAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeChainLevelUpAni != null)
				{
					levelBlock.m_ChangeChainLevelUpAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeChainLevelDownAni != null)
				{
					levelBlock.m_ChangeChainLevelDownAni.gameObject.SetActive(value: false);
				}
				if (levelBlock.m_ChangeSpecialLevelAni != null)
				{
					levelBlock.m_ChangeSpecialLevelAni.gameObject.SetActive(value: false);
				}
			}
		}
	}

	private AlertState GetAlertState(Level mentalLevel)
	{
		switch (mentalLevel)
		{
		case Level.Broken:
		case Level.Level_1:
			return AlertState.Danger;
		case Level.Level_2:
			return AlertState.Caution;
		default:
			return AlertState.Normal;
		}
	}

	private void SetLevelBlocks(Level mentalLevel, InBlockLevel inBlockLevel)
	{
		if (m_LevelBlocks == null || m_LevelBlocks.Length <= 0)
		{
			return;
		}
		switch (mentalLevel)
		{
		case Level.Broken:
		{
			LevelBlock[] levelBlocks = m_LevelBlocks;
			foreach (LevelBlock levelBlock in levelBlocks)
			{
				if (levelBlock != null && !(levelBlock.m_BlockAnimator == null))
				{
					levelBlock.m_BlockAnimator.Play(c_BlockAniState_BrokenAppear);
				}
			}
			break;
		}
		case Level.Beyond:
		{
			LevelBlock[] levelBlocks2 = m_LevelBlocks;
			foreach (LevelBlock levelBlock2 in levelBlocks2)
			{
				if (levelBlock2 != null && !(levelBlock2.m_BlockAnimator == null))
				{
					levelBlock2.m_BlockAnimator.Play(c_BlockAniState_BeyondAppear);
				}
			}
			break;
		}
		default:
		{
			for (int i = 0; i < m_LevelBlocks.Length; i++)
			{
				SetLevelBlockState(i, mentalLevel, inBlockLevel);
			}
			break;
		}
		}
	}

	private void SetLevelBlockState(int blockIdx, Level mentalLevel, InBlockLevel inBlockLevel)
	{
		if (blockIdx < 0 || blockIdx >= m_LevelBlocks.Length)
		{
			return;
		}
		int num = (int)(mentalLevel - 1);
		LevelBlock levelBlock = m_LevelBlocks[blockIdx];
		if (levelBlock == null || levelBlock.m_BlockAnimator == null)
		{
			return;
		}
		string[] array = null;
		switch (m_AlertState)
		{
		case AlertState.Normal:
			array = c_BlockAniStates_Normal;
			break;
		case AlertState.Caution:
			array = c_BlockAniStates_Caution;
			break;
		case AlertState.Danger:
			array = c_BlockAniStates_Danger;
			break;
		}
		if (array == null || array.Length < 5)
		{
			return;
		}
		string stateName = c_BlockAniState_Empty;
		if (blockIdx == num)
		{
			if (inBlockLevel >= InBlockLevel.less && (int)inBlockLevel < array.Length)
			{
				stateName = array[(int)inBlockLevel];
			}
		}
		else if (blockIdx < num)
		{
			stateName = array[4];
		}
		if (levelBlock.m_BlockAnimator.gameObject.activeInHierarchy)
		{
			levelBlock.m_BlockAnimator.Rebind();
			levelBlock.m_BlockAnimator.Play(stateName);
		}
	}

	private void SetNextDirectionLevel(Level currentDirLevel)
	{
		switch (m_ChangedMentalMode)
		{
		case ChangedMentalMode.LevelUp:
		case ChangedMentalMode.ChainLevelUp:
		case ChangedMentalMode.ToBeyond:
			m_DirectionLevel = ((currentDirLevel >= Level.Beyond) ? currentDirLevel : (currentDirLevel + 1));
			break;
		case ChangedMentalMode.LevelDown:
		case ChangedMentalMode.ChainLevelDown:
		case ChangedMentalMode.ToBroken:
			m_DirectionLevel = ((currentDirLevel <= Level.Broken) ? currentDirLevel : (currentDirLevel - 1));
			break;
		default:
			m_DirectionLevel = currentDirLevel;
			break;
		}
	}

	private void ChangeDirectionStep(DirectionStep nextDirStep, bool isIgnoreSame = true)
	{
		if (m_DirectionStep == nextDirStep && isIgnoreSame)
		{
			return;
		}
		switch (nextDirStep)
		{
		case DirectionStep.GageAppear:
			if (m_GageMainAnimator != null)
			{
				m_curGageDirectionAniName = ((m_CurrentLevel != Level.Broken && m_CurrentLevel != Level.Beyond) ? c_MainAniState_ChangeAppear : c_MainAniState_BrokenBeyondAppear);
				m_GageMainAnimator.Play(m_curGageDirectionAniName);
			}
			break;
		case DirectionStep.GageIdle:
			if (m_GageMainAnimator != null)
			{
				m_curGageDirectionAniName = ((m_CurrentLevel != Level.Broken && m_CurrentLevel != Level.Beyond) ? c_MainAniState_ChangeIdle : c_MainAniState_BrokenBeyondIdle);
				m_GageMainAnimator.Play(m_curGageDirectionAniName);
			}
			break;
		case DirectionStep.GageDisappear:
			if (m_GageMainAnimator != null)
			{
				m_curGageDirectionAniName = ((m_CurrentLevel != Level.Broken && m_CurrentLevel != Level.Beyond) ? c_MainAniState_ChangeDisappear : c_MainAniState_BrokenBeyondDisappear);
				m_GageMainAnimator.Play(m_curGageDirectionAniName);
			}
			break;
		case DirectionStep.ChangedMarkShow:
		{
			int num = (int)(m_DirectionLevel - 1);
			LevelBlock levelBlock2 = m_LevelBlocks[num];
			string text = string.Empty;
			switch (m_ChangedMentalMode)
			{
			case ChangedMentalMode.ValueUp:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeSmallUpAni;
				text = "MTL_Plus";
				break;
			case ChangedMentalMode.ValueDown:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeSmallDownAni;
				text = "Mtl_Minus";
				break;
			case ChangedMentalMode.LevelUp:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeLevelUpAni;
				text = "MTL_Plus";
				break;
			case ChangedMentalMode.LevelDown:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeLevelDownAni;
				text = "Mtl_Minus";
				break;
			case ChangedMentalMode.ChainLevelUp:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeChainLevelUpAni;
				text = "MTL_Plus";
				break;
			case ChangedMentalMode.ChainLevelDown:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeChainLevelDownAni;
				text = "Mtl_Minus";
				break;
			case ChangedMentalMode.ToBeyond:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeChainLevelUpAni;
				text = "MTL_Plus";
				break;
			case ChangedMentalMode.ToBroken:
				m_ChangeMarkAnimator = levelBlock2.m_ChangeChainLevelDownAni;
				text = "Mtl_Minus";
				break;
			default:
				m_ChangeMarkAnimator = null;
				break;
			}
			if (m_ChangeMarkAnimator != null)
			{
				if (levelBlock2.m_ChangeInfoRoot != null)
				{
					levelBlock2.m_ChangeInfoRoot.SetActive(value: true);
				}
				m_ChangeMarkAnimator.gameObject.SetActive(value: true);
				if (m_AudioManager != null && !string.IsNullOrEmpty(text))
				{
					m_AudioManager.PlayUISound(text);
				}
			}
			break;
		}
		case DirectionStep.ChangedMarkShowBeyond:
		{
			LevelBlock levelBlock3 = m_LevelBlocks[m_LevelBlocks.Length - 1];
			m_ChangeMarkAnimator = levelBlock3.m_ChangeSpecialLevelAni;
			if (m_ChangeMarkAnimator != null)
			{
				if (levelBlock3.m_ChangeInfoRoot != null)
				{
					levelBlock3.m_ChangeInfoRoot.SetActive(value: true);
				}
				m_ChangeMarkAnimator.gameObject.SetActive(value: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Mtl_Beyond");
				}
			}
			break;
		}
		case DirectionStep.ChangedMarkShowBroken:
		{
			LevelBlock levelBlock = m_LevelBlocks[0];
			m_ChangeMarkAnimator = levelBlock.m_ChangeSpecialLevelAni;
			if (m_ChangeMarkAnimator != null)
			{
				if (levelBlock.m_ChangeInfoRoot != null)
				{
					levelBlock.m_ChangeInfoRoot.SetActive(value: true);
				}
				m_ChangeMarkAnimator.gameObject.SetActive(value: true);
				if (m_AudioManager != null)
				{
					m_AudioManager.PlayUISound("Mtl_Broken");
				}
			}
			break;
		}
		}
		m_DirectionStep = nextDirStep;
	}

	private void SetHeartIconState(Level mentalLevel, ChangedMentalMode changeMentalMode, bool isUseChainAni = false)
	{
		if (m_HeartIconAnimator == null)
		{
			return;
		}
		string aniStateName = string.Empty;
		switch (mentalLevel)
		{
		case Level.Beyond:
			aniStateName = c_HeartIconAniState_BeyondAppear;
			break;
		case Level.Broken:
			aniStateName = c_HeartIconAniState_BrokenAppear;
			break;
		default:
			switch (changeMentalMode)
			{
			case ChangedMentalMode.None:
				switch (m_AlertState)
				{
				case AlertState.Normal:
					aniStateName = c_HeartIconAniState_IdleNormal;
					break;
				case AlertState.Caution:
					aniStateName = c_HeartIconAniState_IdleCaution;
					break;
				case AlertState.Danger:
					aniStateName = c_HeartIconAniState_IdleDanger;
					break;
				}
				break;
			case ChangedMentalMode.ValueUp:
			case ChangedMentalMode.LevelUp:
			case ChangedMentalMode.ChainLevelUp:
			case ChangedMentalMode.ToBeyond:
				switch (m_AlertState)
				{
				case AlertState.Normal:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_UpNormal : c_HeartIconAniState_UpNormalChain);
					break;
				case AlertState.Caution:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_UpCaution : c_HeartIconAniState_UpCautionChain);
					break;
				case AlertState.Danger:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_UpDanger : c_HeartIconAniState_UpDangerChain);
					break;
				}
				break;
			case ChangedMentalMode.ValueDown:
			case ChangedMentalMode.LevelDown:
			case ChangedMentalMode.ChainLevelDown:
			case ChangedMentalMode.ToBroken:
				switch (m_AlertState)
				{
				case AlertState.Normal:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_DownNormal : c_HeartIconAniState_DownNormalChain);
					break;
				case AlertState.Caution:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_DownCaution : c_HeartIconAniState_DownCautionChain);
					break;
				case AlertState.Danger:
					aniStateName = ((!isUseChainAni) ? c_HeartIconAniState_DownDanger : c_HeartIconAniState_DownDangerChain);
					break;
				}
				break;
			}
			break;
		}
		PlayHeartIconAnimation(aniStateName);
	}

	private void PlayHeartIconAnimation(string aniStateName)
	{
		if (!string.IsNullOrEmpty(aniStateName))
		{
			m_HeartIconAniState = (HeartIconAnimationState)Enum.Parse(typeof(HeartIconAnimationState), aniStateName);
			if (m_HeartIconAnimator.gameObject.activeInHierarchy)
			{
				m_HeartIconAnimator.Rebind();
				m_HeartIconAnimator.Play(aniStateName);
			}
		}
	}

	public bool IsCompleteChangeMentalGage()
	{
		return m_ChangedMentalMode == ChangedMentalMode.None;
	}

	public static IEnumerator LoadAssetObject()
	{
		if (!(s_SrcObject != null))
		{
			s_SrcObject = MainLoadThing.instance.m_prefabMentalGageRenewal as GameObject;
			s_SrcObject.SetActive(value: false);
			yield return null;
		}
	}

	public static void Free()
	{
		if (s_Instance != null)
		{
			UnityEngine.Object.Destroy(s_Instance.gameObject);
			s_Instance = null;
		}
		UnloadAssetObject();
	}

	public static void UnloadAssetObject()
	{
		s_SrcObject = null;
	}

	public static IEnumerator CreateInstance()
	{
		if (!(s_Instance != null))
		{
			if (s_SrcObject == null)
			{
				yield return GameMain.instance.StartCoroutine(LoadAssetObject());
			}
			GameObject instanceObj = UnityEngine.Object.Instantiate(s_SrcObject);
			MentalGageRenewal mentalGageRenewal = instanceObj.GetComponent<MentalGageRenewal>();
			s_Instance = mentalGageRenewal;
			s_Instance.Init();
			yield return null;
		}
	}

	public static Level GetMentalLevel_byMentalPoint(int mentalPoint, out InBlockLevel inBlockLevel)
	{
		inBlockLevel = InBlockLevel.Empty;
		Level level = Level.Level_1;
		int num = 1;
		if (mentalPoint <= ConstGameSwitch.MIN_MENTAL_POINT)
		{
			level = Level.Broken;
		}
		else if (mentalPoint > ConstGameSwitch.MAX_MENTAL_POINT)
		{
			level = Level.Beyond;
		}
		else
		{
			int value = mentalPoint / 20;
			value = Mathf.Clamp(value, 0, 4) + num;
			level = (Level)value;
			int num2 = (int)(level - num) * 20;
			int num3 = mentalPoint - num2;
			if (num3 < 5)
			{
				inBlockLevel = InBlockLevel.less;
			}
			else if (num3 < 10)
			{
				inBlockLevel = InBlockLevel.less2;
			}
			else if (num3 < 15)
			{
				inBlockLevel = InBlockLevel.less3;
			}
			else if (num3 < 20)
			{
				inBlockLevel = InBlockLevel.less4;
			}
			else
			{
				inBlockLevel = InBlockLevel.Full;
			}
		}
		return level;
	}
}
