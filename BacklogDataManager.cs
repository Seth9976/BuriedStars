using System.Collections.Generic;
using UnityEngine;

public class BacklogDataManager
{
	private static List<BacklogData> s_backlogDatas = new List<BacklogData>();

	public static List<BacklogData> backlogDatas => s_backlogDatas;

	public static void AddBacklogData(string strText, bool isContinuous = false)
	{
		BacklogData backlogData = new BacklogData();
		backlogData.m_Type = BacklogData._Type.MonoText;
		backlogData.m_strDialog = strText;
		backlogData.m_isContinuous = isContinuous;
		s_backlogDatas.Add(backlogData);
	}

	public static void AddBacklogData(string strDialog, string strCharName, string strCharColor, string strVoiceName, bool isContinuous = false)
	{
		BacklogData backlogData = new BacklogData();
		backlogData.m_Type = BacklogData._Type.Dialog;
		backlogData.m_strDialog = strDialog;
		backlogData.m_strCharName = strCharName;
		backlogData.m_strVoiceName = strVoiceName;
		backlogData.m_isContinuous = isContinuous;
		float[] fRGB = new float[3];
		if (GameGlobalUtil.ConvertHexStrToRGB(strCharColor, ref fRGB))
		{
			backlogData.m_colorCharName = new Color(fRGB[0], fRGB[1], fRGB[2]);
		}
		s_backlogDatas.Add(backlogData);
	}

	public static void AddBacklogData_Fater(string strText, Color colorName)
	{
		BacklogData backlogData = new BacklogData();
		backlogData.m_Type = BacklogData._Type.Fater;
		backlogData.m_strDialog = strText;
		backlogData.m_strCharName = GameGlobalUtil.GetXlsProgramText("BACKLOG_FATER_CONTENT");
		backlogData.m_colorCharName = colorName;
		s_backlogDatas.Add(backlogData);
	}

	public static void AddBacklogData_Messenger(string strText, string strCharName, Color colorCharName)
	{
		BacklogData backlogData = new BacklogData();
		backlogData.m_Type = BacklogData._Type.Messenger;
		backlogData.m_strDialog = strText;
		backlogData.m_strCharName = strCharName;
		backlogData.m_colorCharName = colorCharName;
		s_backlogDatas.Add(backlogData);
	}

	public static void ClearBacklogDatas()
	{
		s_backlogDatas.Clear();
	}
}
