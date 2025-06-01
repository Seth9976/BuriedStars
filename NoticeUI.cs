using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
	public Image m_IconImage;

	public Text m_Text;

	public int m_RightInterval = 10;

	private void Start()
	{
		RectTransform component = m_IconImage.gameObject.GetComponent<RectTransform>();
		RectTransform component2 = base.gameObject.GetComponent<RectTransform>();
		component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rect.width + m_Text.preferredWidth + (float)m_RightInterval);
	}

	public void SetContent(Sprite iconSprite, string text)
	{
		FontManager.ResetTextFontByCurrentLanguage(m_Text);
		m_IconImage.sprite = iconSprite;
		m_Text.text = text;
	}
}
