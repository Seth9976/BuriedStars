using UnityEngine;
using UnityEngine.UI;

public class SWSub_RecordContent : MonoBehaviour
{
	public Text m_ContentText;

	private RectTransform m_RectTransform;

	public string contentText
	{
		get
		{
			return m_ContentText.text;
		}
		set
		{
			m_ContentText.text = value;
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

	private void OnDestroy()
	{
		m_RectTransform = null;
	}
}
