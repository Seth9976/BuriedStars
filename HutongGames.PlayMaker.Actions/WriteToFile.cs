using System;
using System.IO;
using GameData;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
[Tooltip("Write string to a File")]
public class WriteToFile : FsmStateAction
{
	[RequiredField]
	[Tooltip("Set the file path, for example : Assets/myfile.txt")]
	public FsmString fileName;

	[RequiredField]
	[UIHint(UIHint.TextArea)]
	[Tooltip("The text")]
	[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
	public FsmArray arrCharKey;

	private GameSwitch m_GameSwitch;

	private string strSFName;

	public override void Reset()
	{
		fileName = null;
	}

	public override void OnEnter()
	{
		strSFName = Path.Combine(Application.persistentDataPath, fileName.Value);
		if (!File.Exists(strSFName))
		{
			File.Create(strSFName).Dispose();
			StreamWriter streamWriter = new StreamWriter(strSFName, append: true);
			streamWriter.Write("time,user,cut_id,gametime,playtime,char,value\r\n");
			streamWriter.Close();
		}
		WriteToText();
		Finish();
	}

	private void WriteToText()
	{
		m_GameSwitch = GameSwitch.GetInstance();
		string text = string.Empty;
		string empty = string.Empty;
		string text2 = string.Empty;
		string userName = Environment.UserName;
		float gameTime = m_GameSwitch.GetGameTime();
		int num = (int)(gameTime / 3600f);
		gameTime -= (float)(num * 3600);
		int num2 = (int)(gameTime / 60f);
		int curCutIdx = m_GameSwitch.GetCurCutIdx();
		if (curCutIdx != -1)
		{
			Xls.TalkCutSetting data_byIdx = Xls.TalkCutSetting.GetData_byIdx(curCutIdx);
			if (data_byIdx != null)
			{
				text2 = data_byIdx.m_strKey;
			}
		}
		int length = arrCharKey.Length;
		for (int i = 0; i < length; i++)
		{
			text = text + DateTime.Now.ToString("MMddHHmmssfff") + ", ";
			text = text + userName + ",";
			text = text + text2 + ",";
			string text3 = text;
			text = text3 + num.ToString("00") + ":" + num2.ToString("00") + ",";
			text = text + m_GameSwitch.GetTotalGamePlayTime() + ",";
			string text4 = arrCharKey.stringValues[i];
			if (i == 0)
			{
				int mental = m_GameSwitch.GetMental();
				text3 = text;
				text = text3 + text4 + "," + mental + "\r\n";
				empty = mental.ToString();
			}
			else
			{
				empty = m_GameSwitch.GetCharRelation(text4).ToString();
				text3 = text;
				text = text3 + text4 + "," + empty + "\r\n";
			}
		}
		StreamWriter streamWriter = new StreamWriter(strSFName, append: true);
		streamWriter.Write(text);
		streamWriter.Close();
	}
}
