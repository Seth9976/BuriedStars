using UnityEngine;

public class BacklogData
{
	public enum _Type
	{
		MonoText,
		Dialog,
		Fater,
		Messenger
	}

	public _Type m_Type;

	public string m_strDialog = string.Empty;

	public string m_strCharName = string.Empty;

	public string m_strVoiceName = string.Empty;

	public Color m_colorCharName = Color.white;

	public bool m_isContinuous;
}
