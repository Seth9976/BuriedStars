using UnityEngine;
using UnityEngine.UI;

public class KeywordAnswerSlot : MonoBehaviour
{
	private const int ciSlotCnt = 2;

	public GameObject[] m_goSlot = new GameObject[2];

	public RectTransform[] m_rtfSlot = new RectTransform[2];

	public Sprite m_sprDefKeywordIcon;

	public Image[] m_imgKeywordIcon = new Image[2];

	public Text[] m_textKeywordName = new Text[2];

	public GameObject[] m_goTag = new GameObject[2];

	public Text[] m_textTagOrder = new Text[2];

	public Animator[] m_animSlot = new Animator[2];

	public GameObject m_goFocus;
}
