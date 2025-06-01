using GameData;
using UnityEngine;
using UnityEngine.UI;

public class KeywordUseResultSlot : MonoBehaviour
{
	public Image m_imgDotFace;

	private const int MENTAL_SPR_CNT = 4;

	public GameObject[] m_goMental = new GameObject[4];

	public GameObject m_goCutKeyword;

	public Text m_textPointGetIcon;

	private void OnEnable()
	{
		FontManager.ResetTextFontByCurrentLanguage(m_textPointGetIcon);
	}

	public void Init()
	{
		base.gameObject.SetActive(value: true);
		m_imgDotFace.gameObject.SetActive(value: true);
	}

	public void SetFaceImage(Sprite sprFace)
	{
		m_imgDotFace.sprite = sprFace;
	}

	public void SetMentalImage(byte byMental)
	{
		if (BitCalc.CheckArrayIdx(byMental, 5))
		{
			int num = byMental - 1;
			for (int i = 0; i < 4; i++)
			{
				m_goMental[i].SetActive(i == num);
			}
		}
	}

	public void SetCutKeyword(bool isOn)
	{
		m_goCutKeyword.SetActive(isOn);
	}
}
