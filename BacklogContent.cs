using UnityEngine;
using UnityEngine.UI;

public class BacklogContent : MonoBehaviour
{
	public Text m_TextDialog;

	public Text m_TextCharName;

	public Button m_ButtonVoicePlay;

	private string m_VoiceName = string.Empty;

	public void SetBacklogData(BacklogData data)
	{
		if (m_TextDialog != null)
		{
			m_TextDialog.text = TagText.TransTagTextToUnityText(data.m_strDialog, isIgnoreHideTag: false);
			m_TextDialog.gameObject.SetActive(!string.IsNullOrEmpty(data.m_strDialog));
		}
		if (m_TextCharName != null)
		{
			m_TextCharName.text = data.m_strCharName;
			m_TextCharName.gameObject.SetActive(!string.IsNullOrEmpty(data.m_strCharName));
		}
		if (m_ButtonVoicePlay != null)
		{
			m_VoiceName = data.m_strVoiceName;
			m_ButtonVoicePlay.gameObject.SetActive(!string.IsNullOrEmpty(data.m_strVoiceName));
		}
	}

	public void OnClick_PlayVoiceButton()
	{
	}
}
