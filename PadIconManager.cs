using System;
using UnityEngine;

public class PadIconManager : MonoBehaviour
{
	public enum PadIconType
	{
		None,
		DirectionAll,
		DirectionX,
		DirectionY,
		ButtonCircle,
		ButtonCross,
		ButtonTriangle,
		ButtonSquare,
		ButtonL1,
		ButtonR1,
		ButtonOption,
		ButtonTouchpad,
		ListScrollX,
		ListScrollY,
		ButtonSkip
	}

	[Serializable]
	public class IconPair
	{
		public Sprite m_Normal;

		public Sprite m_Pressed;
	}

	[Serializable]
	public class PadIcons
	{
		public IconPair m_DirectionAll;

		public IconPair m_DirectionX;

		public IconPair m_DirectionY;

		public IconPair m_ButtonCircle;

		public IconPair m_ButtonCross;

		public IconPair m_ButtonTriangle;

		public IconPair m_ButtonSquare;

		public IconPair m_ButtonL1;

		public IconPair m_ButtonR1;

		public IconPair m_ButtonOption;

		public IconPair m_ButtonTouchpad;

		public IconPair m_ListScrollX;

		public IconPair m_ListScrollY;

		public IconPair m_ButtonSkip;
	}

	[Serializable]
	public class PadIconGroup
	{
		public PadIcons m_Default = new PadIcons();

		public PadIcons m_Xbox = new PadIcons();

		public PadIcons m_Nswitch = new PadIcons();

		public PadIcons m_Keyboard = new PadIcons();
	}

	public PadIconGroup[] m_PadIconGroups;

	private static PadIconManager s_instance;

	public static PadIconManager instance => s_instance;

	private void Awake()
	{
		s_instance = this;
	}

	private void OnDestroy()
	{
		s_instance = null;
	}

	public static IconPair GetPadIcon(int groupIndex, PadIconType iconType)
	{
		return (!(s_instance != null)) ? null : s_instance._GetPadIcon(groupIndex, iconType);
	}

	private IconPair _GetPadIcon(int groupIndex, PadIconType iconType)
	{
		if (m_PadIconGroups == null || groupIndex < 0 || groupIndex >= m_PadIconGroups.Length)
		{
			return null;
		}
		PadIconGroup padIconGroup = m_PadIconGroups[groupIndex];
		if (padIconGroup == null)
		{
			return null;
		}
		PadIcons padIcons = null;
		padIcons = PadInput.CurButtonIconType switch
		{
			PadInput.ButtonIconType.XBox => padIconGroup.m_Xbox, 
			PadInput.ButtonIconType.Switch => padIconGroup.m_Nswitch, 
			PadInput.ButtonIconType.Keyboard => padIconGroup.m_Keyboard, 
			_ => padIconGroup.m_Default, 
		};
		if (padIcons == null)
		{
			return null;
		}
		return iconType switch
		{
			PadIconType.DirectionAll => padIcons.m_DirectionAll, 
			PadIconType.DirectionX => padIcons.m_DirectionX, 
			PadIconType.DirectionY => padIcons.m_DirectionY, 
			PadIconType.ButtonCircle => padIcons.m_ButtonCircle, 
			PadIconType.ButtonCross => padIcons.m_ButtonCross, 
			PadIconType.ButtonTriangle => padIcons.m_ButtonTriangle, 
			PadIconType.ButtonSquare => padIcons.m_ButtonSquare, 
			PadIconType.ButtonL1 => padIcons.m_ButtonL1, 
			PadIconType.ButtonR1 => padIcons.m_ButtonR1, 
			PadIconType.ButtonOption => padIcons.m_ButtonOption, 
			PadIconType.ButtonTouchpad => padIcons.m_ButtonTouchpad, 
			PadIconType.ListScrollX => padIcons.m_ListScrollX, 
			PadIconType.ListScrollY => padIcons.m_ListScrollY, 
			PadIconType.ButtonSkip => padIcons.m_ButtonSkip, 
			_ => null, 
		};
	}
}
