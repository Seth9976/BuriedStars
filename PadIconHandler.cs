using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class PadIconHandler : MonoBehaviour
{
	public PadIconManager.PadIconType m_IconType;

	public int m_IconGroupIndex;

	public Image m_LinkedImage;

	public Button m_LinkedButton;

	private GameDefine.EventProc m_fpNoticeReflashed;

	private static List<PadIconHandler> s_Instances = new List<PadIconHandler>();

	public GameDefine.EventProc fpNoticeReflashed
	{
		set
		{
			m_fpNoticeReflashed = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void OnEnable()
	{
		AddInstance(this);
		Reflash();
	}

	private void OnDisable()
	{
		RemoveInstance(this);
	}

	private void OnDestroy()
	{
		m_fpNoticeReflashed = null;
	}

	public void Reflash()
	{
		PadIconManager.PadIconType iconType = m_IconType;
		GameSwitch instance = GameSwitch.GetInstance();
		if (instance.GetUIButType() != GameSwitch.eUIButType.KEYMOUSE && GameSwitch.GetInstance().GetOXType() == 1)
		{
			if (m_IconType == PadIconManager.PadIconType.ButtonCircle)
			{
				iconType = PadIconManager.PadIconType.ButtonCross;
			}
			else if (m_IconType == PadIconManager.PadIconType.ButtonCross)
			{
				iconType = PadIconManager.PadIconType.ButtonCircle;
			}
		}
		PadIconManager.IconPair padIcon = PadIconManager.GetPadIcon(m_IconGroupIndex, iconType);
		if (padIcon != null)
		{
			if (m_LinkedImage != null)
			{
				m_LinkedImage.sprite = padIcon.m_Normal;
			}
			if (m_LinkedButton != null && m_LinkedButton.transition == Selectable.Transition.SpriteSwap)
			{
				SpriteState spriteState = new SpriteState
				{
					pressedSprite = padIcon.m_Pressed
				};
				m_LinkedButton.spriteState = spriteState;
			}
			if (m_fpNoticeReflashed != null)
			{
				m_fpNoticeReflashed(this, padIcon);
			}
		}
	}

	public static void ClearInstances()
	{
		s_Instances.Clear();
	}

	private static void AddInstance(PadIconHandler instance)
	{
		if (!s_Instances.Contains(instance))
		{
			s_Instances.Add(instance);
		}
	}

	private static void RemoveInstance(PadIconHandler instance)
	{
		if (s_Instances.Contains(instance))
		{
			s_Instances.Remove(instance);
		}
	}

	public static void ReflashIcons()
	{
		if (s_Instances.Count <= 0)
		{
			return;
		}
		foreach (PadIconHandler s_Instance in s_Instances)
		{
			s_Instance.Reflash();
		}
	}
}
