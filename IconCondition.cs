using GameData;
using UnityEngine;
using UnityEngine.UI;

public class IconCondition : MonoBehaviour
{
	private const int MAX_COND_CNT = 10;

	private const int MAX_COND_TYPE_CNT = 5;

	public Image[] m_imgIconCond = new Image[10];

	private static IconCondition s_instance;

	private GameSwitch m_GameSwitch;

	public static IconCondition instance => s_instance;

	private void Awake()
	{
		s_instance = this;
		m_GameSwitch = GameSwitch.GetInstance();
	}

	private void OnDestroy()
	{
		m_GameSwitch = null;
		s_instance = null;
	}

	public Sprite GetConditionSprite(string strCharKey, bool isSel = true)
	{
		int charIdx = m_GameSwitch.GetCharIdx(strCharKey);
		return GetConditionSprite(charIdx);
	}

	public Sprite GetConditionSprite(int iCharIdx, bool isSel = true)
	{
		Sprite result = null;
		if (m_GameSwitch != null)
		{
			GameSwitch.eChrIconState charIconState = m_GameSwitch.GetCharIconState(iCharIdx);
			if (BitCalc.CheckArrayIdx((int)charIconState, 5))
			{
				result = ((!isSel) ? m_imgIconCond[(int)(5 + charIconState)].sprite : m_imgIconCond[(int)charIconState].sprite);
			}
		}
		return result;
	}
}
