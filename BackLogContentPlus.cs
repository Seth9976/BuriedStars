using UnityEngine;
using UnityEngine.UI;

public class BackLogContentPlus : MonoBehaviour
{
	public GameObject m_SelectionCursor;

	public Text m_TextDialog;

	public Text m_TextCharName;

	public Image m_BoundLineImage;

	public Sprite m_ContinuousLineSprite;

	public Button m_ButtonVoicePlay;

	public Button m_ButtonVoicePlayOnCursor;

	public Button m_SelectionIconButton;

	private string m_VoiceName = string.Empty;

	private bool m_isExistVoice;

	private bool m_isSelected;

	private RectTransform m_RectTransform;

	private bool m_isInitialized;

	private BackLogMenuPlus m_backlogMenu;

	private const float c_fSingleLineHRate = 0.6f;

	public string VoiceName => m_VoiceName;

	public bool IsExistVoice => m_isExistVoice;

	public bool selected
	{
		get
		{
			return m_isSelected;
		}
		set
		{
			m_isSelected = value;
			if (m_SelectionCursor != null)
			{
				m_SelectionCursor.SetActive(m_isSelected);
			}
			if (m_ButtonVoicePlay != null)
			{
				m_ButtonVoicePlay.gameObject.SetActive(!m_isSelected && m_isExistVoice);
			}
			if (m_ButtonVoicePlayOnCursor != null)
			{
				m_ButtonVoicePlayOnCursor.gameObject.SetActive(m_isSelected && m_isExistVoice);
			}
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public bool isInitialized => m_isInitialized;

	public BackLogMenuPlus BacklogMenu
	{
		set
		{
			m_backlogMenu = value;
		}
	}

	private void Start()
	{
		RectTransform rectTransform = this.rectTransform;
		float height = rectTransform.rect.height;
		if (m_TextDialog.cachedTextGeneratorForLayout.lineCount <= 1)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.6f);
		}
		m_isInitialized = true;
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		Text[] textComps = new Text[2] { m_TextDialog, m_TextCharName };
		FontManager.ResetTextFontByCurrentLanguage(textComps);
	}

	private void OnDestroy()
	{
		m_VoiceName = null;
		m_RectTransform = null;
		m_backlogMenu = null;
	}

	public void SetBacklogData(string text, string charName, Color colorName, string voiceName, bool isContinuous)
	{
		if (m_TextDialog != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_TextDialog);
			m_TextDialog.text = text;
			m_TextDialog.gameObject.SetActive(!string.IsNullOrEmpty(text));
			RectTransform component = m_TextDialog.gameObject.GetComponent<RectTransform>();
			m_TextDialog.cachedTextGeneratorForLayout.Populate(m_TextDialog.text, m_TextDialog.GetGenerationSettings(new Vector2(component.rect.width, component.rect.height)));
		}
		if (m_TextCharName != null)
		{
			FontManager.ResetTextFontByCurrentLanguage(m_TextCharName);
			m_TextCharName.text = charName;
			bool flag = !string.IsNullOrEmpty(charName);
			m_TextCharName.gameObject.SetActive(flag);
			if (flag)
			{
				m_TextCharName.color = colorName;
			}
		}
		m_isExistVoice = false;
		if (m_ButtonVoicePlay != null)
		{
			m_VoiceName = voiceName;
			m_isExistVoice = !string.IsNullOrEmpty(voiceName);
		}
		if (m_BoundLineImage != null && isContinuous)
		{
			m_BoundLineImage.sprite = m_ContinuousLineSprite;
		}
		selected = false;
	}

	public bool OnProc_SubmitButton()
	{
		if (!m_isExistVoice)
		{
			return false;
		}
		if (!ButtonPadInput.PressInputButton(PadInput.GameInput.CircleButton, m_SelectionIconButton, m_ButtonVoicePlayOnCursor))
		{
			return false;
		}
		OnClick_PlayVoiceButton();
		return true;
	}

	public void OnClick_PlayVoiceButton()
	{
		if (m_backlogMenu != null)
		{
			m_backlogMenu.PlayVoice(this);
		}
	}

	public TextGenerationSettings GetTextGenerationSetting()
	{
		return (!(m_TextDialog != null)) ? default(TextGenerationSettings) : m_TextDialog.GetGenerationSettings(rectTransform.rect.size);
	}

	public void TouchPlayVoiceButton()
	{
		if (MainLoadThing.instance.IsTouchableState())
		{
			OnClick_PlayVoiceButton();
		}
	}

	public void OnClicked_Content()
	{
		if (m_backlogMenu != null)
		{
			m_backlogMenu.SetOnCursorContent(this);
		}
	}
}
