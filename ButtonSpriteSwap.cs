using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSwap
{
	private class ButtonObj
	{
		public Button m_butOrigin;

		public Sprite m_sprNormal;
	}

	private static List<ButtonObj> m_lstButtonObj = new List<ButtonObj>();

	public static void PressButton(Button button, bool isPress)
	{
		ButtonObj buttonObj = FindButtonObj(button);
		if (buttonObj == null)
		{
			buttonObj = new ButtonObj();
			buttonObj.m_butOrigin = button;
			buttonObj.m_sprNormal = button.GetComponent<Image>().sprite;
			m_lstButtonObj.Add(buttonObj);
		}
		if (isPress)
		{
			button.GetComponent<Image>().sprite = button.spriteState.pressedSprite;
		}
		else
		{
			button.GetComponent<Image>().sprite = buttonObj.m_sprNormal;
		}
	}

	public static void DelButton(Button button)
	{
		ButtonObj buttonObj = FindButtonObj(button);
		if (buttonObj != null)
		{
			m_lstButtonObj.Remove(buttonObj);
		}
	}

	public static void DeleteAllButton()
	{
		m_lstButtonObj.Clear();
	}

	private static ButtonObj FindButtonObj(Button button)
	{
		ButtonObj result = null;
		for (int i = 0; i < m_lstButtonObj.Count; i++)
		{
			if (m_lstButtonObj[i].m_butOrigin == button)
			{
				result = m_lstButtonObj[i];
				break;
			}
		}
		return result;
	}
}
