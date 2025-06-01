using System.Collections;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class OXChangeButton : MonoBehaviour
{
	public Image m_imgBaseButton;

	public Button m_butPress;

	public Sprite[] m_BaseOXButton = new Sprite[2];

	public Sprite[] m_PressOXButton = new Sprite[2];

	private void OnEnable()
	{
		if (!(m_imgBaseButton == null) && !(m_butPress == null))
		{
			if (m_imgBaseButton.sprite == m_butPress.spriteState.pressedSprite)
			{
				StartCoroutine(ButtonStateCheck());
			}
			else
			{
				SpriteExChange();
			}
		}
	}

	private IEnumerator ButtonStateCheck()
	{
		if (m_imgBaseButton.sprite == m_butPress.spriteState.pressedSprite)
		{
			yield return null;
		}
		yield return null;
		SpriteExChange();
	}

	private void SpriteExChange()
	{
		SpriteState spriteState = m_butPress.spriteState;
		int oXType = GameSwitch.GetInstance().GetOXType();
		m_imgBaseButton.sprite = m_BaseOXButton[oXType];
		spriteState.pressedSprite = m_PressOXButton[oXType];
		m_butPress.spriteState = spriteState;
	}
}
