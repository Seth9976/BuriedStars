using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	[Conditional("ENABLE_LOG")]
	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	[Conditional("ENABLE_LOG")]
	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}

	[Conditional("ENABLE_LOG")]
	public static void Log(object message)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void Log(object message, UnityEngine.Object context)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogError(object message)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogError(object message, UnityEngine.Object context)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogWarning(object message)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("ENABLE_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	[Conditional("ENABLE_LOG")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			throw new Exception();
		}
	}

	[Conditional("ENABLE_LOG")]
	public static void Assert(bool condition, object message)
	{
		if (!condition)
		{
			UnityEngine.Debug.LogWarning(message.ToString());
			throw new Exception();
		}
	}

	[Conditional("ENABLE_LOG")]
	public static void LogErrorFormat(string format, params object[] args)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("ENABLE_LOG")]
	public static void LogException(Exception exception)
	{
	}
}
