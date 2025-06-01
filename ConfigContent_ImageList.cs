using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfigContent_ImageList : ConfigContent_Base
{
	[Serializable]
	public class ImageMemberData
	{
		public string m_ImageTag = string.Empty;

		public Sprite m_SpriteAsset;
	}

	[Serializable]
	public class LanguageMemberData
	{
		public SystemLanguage m_Language = SystemLanguage.Unknown;

		public Sprite m_SpriteAsset;
	}

	[Serializable]
	public class TextMemberData
	{
		public string m_ItemTextKey = string.Empty;

		public string m_Value = string.Empty;
	}

	public class ItemData
	{
		public object m_Tag;

		public Sprite m_SpriteAsset;

		public string m_Text;
	}

	[Header("Language List Memebers")]
	public Button m_PrevButton;

	public Button m_NextButton;

	public Image m_CurrentImage;

	public Text m_CurrentValue;

	private ItemData[] m_ItemDatas;

	private int m_curItemIndex = -1;

	public ItemData[] ItemDatas
	{
		get
		{
			return m_ItemDatas;
		}
		set
		{
			m_ItemDatas = value;
		}
	}

	public int SelectedIndex
	{
		get
		{
			return m_curItemIndex;
		}
		set
		{
			SetCurrentItemIndex(value);
		}
	}

	public ConfigContent_ImageList()
	{
		m_Type = ContentType.ImageList;
	}

	private void Update()
	{
		if (m_ParentMenu.isInputBlock || !base.Selected)
		{
			return;
		}
		Vector2 lStickMove = GamePadInput.GetLStickMove();
		lStickMove.Normalize();
		if (Mathf.Abs(lStickMove.x) >= 0.5f)
		{
			if (GamePadInput.IsLStickState_Left(PadInput.ButtonState.Down))
			{
				OnClick_ChangeCurrentItem(isToNext: false);
			}
			else if (GamePadInput.IsLStickState_Right(PadInput.ButtonState.Down))
			{
				OnClick_ChangeCurrentItem(isToNext: true);
			}
		}
	}

	public object GetCurrentItemTag()
	{
		int num = ((m_ItemDatas != null) ? m_ItemDatas.Length : 0);
		return (m_curItemIndex >= 0 && m_curItemIndex < num) ? m_ItemDatas[m_curItemIndex].m_Tag : null;
	}

	public void SetCurrentItemIndex(int itemIndex)
	{
		int num = ((m_ItemDatas != null) ? m_ItemDatas.Length : 0);
		if (itemIndex < 0 || itemIndex >= num)
		{
			itemIndex = -1;
		}
		if (m_Type == ContentType.TextList)
		{
			if (m_CurrentValue != null)
			{
				m_CurrentValue.gameObject.SetActive(value: true);
				m_CurrentValue.text = ((itemIndex >= 0) ? m_ItemDatas[itemIndex].m_Text : string.Empty);
			}
			if (m_CurrentImage != null)
			{
				m_CurrentImage.gameObject.SetActive(value: false);
			}
		}
		else
		{
			if (m_CurrentImage != null)
			{
				m_CurrentImage.gameObject.SetActive(value: true);
				m_CurrentImage.sprite = ((itemIndex >= 0) ? m_ItemDatas[itemIndex].m_SpriteAsset : null);
			}
			if (m_CurrentValue != null)
			{
				m_CurrentValue.gameObject.SetActive(value: false);
			}
		}
		m_curItemIndex = itemIndex;
		if (base.OnEventNotice != null)
		{
			base.OnEventNotice(this, GetCurrentItemTag());
		}
	}

	public void SetCurrentItemByTag(object itemTag)
	{
		int currentItemIndex = -1;
		int num = ((m_ItemDatas != null) ? m_ItemDatas.Length : 0);
		for (int i = 0; i < num; i++)
		{
			if (object.Equals(m_ItemDatas[i].m_Tag, itemTag))
			{
				currentItemIndex = i;
				break;
			}
		}
		SetCurrentItemIndex(currentItemIndex);
	}

	public void OnClick_ChangeCurrentItem(bool isToNext)
	{
		int num = ((m_ItemDatas != null) ? m_ItemDatas.Length : 0);
		if (num > 1)
		{
			int num2 = m_curItemIndex + (isToNext ? 1 : (-1));
			if (num2 < 0)
			{
				num2 = num - 1;
			}
			else if (num2 >= num)
			{
				num2 = 0;
			}
			SetCurrentItemIndex(num2);
			if ((bool)m_AudioManager)
			{
				m_AudioManager.PlayUISound("Menu_Select");
			}
		}
	}

	public override void ResetFontByCurrentLanguage()
	{
		base.ResetFontByCurrentLanguage();
		if (m_CurrentValue != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_CurrentValue);
		}
	}

	public void TouchButtonPrevNext(bool isToNext)
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_ChangeCurrentItem(isToNext);
		}
	}
}
