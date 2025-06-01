using System.Collections.Generic;

public class CategoryInfo<T> where T : Xls.XmlDataBase
{
	private int m_ID = -1;

	private string m_Name = string.Empty;

	public List<T> m_xlsDatas = new List<T>();

	public float m_CompleteRate;

	public int m_ValidContentCount;

	public int m_NewContentCount;

	public CommonTabButtonPlus m_LinkedTabButton;

	public int ID => m_ID;

	public string Name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public CategoryInfo(int categoryID, string categoryName)
	{
		m_ID = categoryID;
		m_Name = categoryName;
	}

	public void ReflashLinkedTabButton()
	{
		if (!(m_LinkedTabButton == null))
		{
			m_LinkedTabButton.SetVisibleNewSymbol(m_NewContentCount > 0);
		}
	}
}
