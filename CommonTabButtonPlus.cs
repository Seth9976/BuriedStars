using System;
using UnityEngine;
using UnityEngine.UI;

public class CommonTabButtonPlus : MonoBehaviour
{
	public enum State
	{
		NotSelected,
		Selected,
		Selecting
	}

	[Serializable]
	public class ChildInfo
	{
		public GameObject m_Skin;

		public Image m_SkinImage;

		public Text m_Text;

		public GameObject m_NewSymbol;

		public Sprite m_NormalSkin;

		public Color m_NormalTextColor = default(Color);

		public Sprite m_DisableSkin;

		public Color m_DisableTextColor = default(Color);
	}

	public ChildInfo m_SelButton = new ChildInfo();

	public ChildInfo m_NotSelButton = new ChildInfo();

	public ChildInfo m_Banner = new ChildInfo();

	[Header("SelectionCursor/PadButton")]
	public GameObject m_SelectionCursor;

	private State m_curState;

	private string m_Text = string.Empty;

	private int m_BannerNum;

	private bool m_isEnable = true;

	private GameDefine.EventProc m_EventProc_Selected;

	public State curState
	{
		get
		{
			return m_curState;
		}
		set
		{
			switch (value)
			{
			case State.NotSelected:
				m_SelButton.m_Skin.SetActive(value: false);
				m_NotSelButton.m_Skin.SetActive(value: true);
				if (m_SelectionCursor != null)
				{
					m_SelectionCursor.SetActive(value: false);
				}
				break;
			case State.Selected:
				m_SelButton.m_Skin.SetActive(value: true);
				m_NotSelButton.m_Skin.SetActive(value: false);
				if (m_SelectionCursor != null)
				{
					m_SelectionCursor.SetActive(value: false);
				}
				break;
			case State.Selecting:
				m_SelButton.m_Skin.SetActive(value: true);
				m_NotSelButton.m_Skin.SetActive(value: false);
				if (m_SelectionCursor != null)
				{
					m_SelectionCursor.SetActive(value: true);
				}
				break;
			}
			m_curState = value;
		}
	}

	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			m_Text = value;
			FontManager.ResetTextFontByCurrentLanguage(m_SelButton.m_Text);
			FontManager.ResetTextFontByCurrentLanguage(m_NotSelButton.m_Text);
			m_SelButton.m_Text.text = m_Text;
			m_NotSelButton.m_Text.text = m_Text;
		}
	}

	public int bannerNum
	{
		get
		{
			return m_BannerNum;
		}
		set
		{
			m_BannerNum = Mathf.Clamp(value, 0, 99);
			if (m_Banner.m_Text != null)
			{
				FontManager.ResetTextFontByCurrentLanguage(m_Banner.m_Text);
				m_Banner.m_Text.text = m_BannerNum.ToString();
			}
			if (m_Banner.m_Skin != null)
			{
				m_Banner.m_Skin.SetActive(m_BannerNum > 0);
			}
		}
	}

	public bool isEnableButton
	{
		get
		{
			return m_isEnable;
		}
		set
		{
			m_isEnable = value;
			if (m_SelButton.m_SkinImage != null)
			{
				m_SelButton.m_SkinImage.sprite = ((!m_isEnable) ? m_SelButton.m_DisableSkin : m_SelButton.m_NormalSkin);
			}
			if (m_SelButton.m_Text != null)
			{
				m_SelButton.m_Text.color = ((!m_isEnable) ? m_SelButton.m_DisableTextColor : m_SelButton.m_NormalTextColor);
			}
			if (m_NotSelButton.m_SkinImage != null)
			{
				m_NotSelButton.m_SkinImage.sprite = ((!m_isEnable) ? m_NotSelButton.m_DisableSkin : m_NotSelButton.m_NormalSkin);
			}
			if (m_NotSelButton.m_Text != null)
			{
				m_NotSelButton.m_Text.color = ((!m_isEnable) ? m_NotSelButton.m_DisableTextColor : m_NotSelButton.m_NormalTextColor);
			}
		}
	}

	public GameDefine.EventProc onSelectedProc
	{
		get
		{
			return m_EventProc_Selected;
		}
		set
		{
			m_EventProc_Selected = ((value == null) ? null : new GameDefine.EventProc(value.Invoke));
		}
	}

	private void OnDestroy()
	{
		m_SelectionCursor = null;
		m_EventProc_Selected = null;
	}

	public void OnSelected()
	{
		if (m_curState != State.Selected && m_EventProc_Selected != null)
		{
			m_EventProc_Selected(base.gameObject, this);
		}
	}

	public void SetVisibleNewSymbol(bool visible)
	{
		if (m_SelButton.m_NewSymbol != null)
		{
			m_SelButton.m_NewSymbol.SetActive(visible);
		}
		if (m_NotSelButton.m_NewSymbol != null)
		{
			m_NotSelButton.m_NewSymbol.SetActive(visible);
		}
	}

	public void TouchSelected()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnSelected();
		}
	}
}
