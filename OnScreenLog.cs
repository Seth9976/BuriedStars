using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
	private static int msgCount = 0;

	private static List<string> log = new List<string>();

	private static string m_strPlayConti = null;

	private static int maxLines = 16;

	private static int fontSize = 24;

	private static int iContiFontSize = 12;

	private const int c_baseFontSize = 24;

	private const int c_baseScreenWdith = 1980;

	private int iFirstRectY = 10;

	private float fSecondRectYPlusH = 6f;

	private float deltaTime;

	private bool m_isViewLog;

	private List<byte[]> m_allocTest = new List<byte[]>();

	public void SetViewLog(bool isViewLog)
	{
		m_isViewLog = isViewLog;
	}

	private void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI()
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void Add(string msg)
	{
		string text = msg.Replace("\r", " ");
		text = text.Replace("\n", " ");
		Console.WriteLine("[APP] " + text);
		log.Add(text);
		msgCount++;
		if (msgCount > maxLines)
		{
			log.RemoveAt(0);
		}
	}

	public static void SetContiName(string strContiName)
	{
		m_strPlayConti = strContiName;
	}
}
