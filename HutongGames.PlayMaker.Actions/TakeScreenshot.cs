using System;
using System.IO;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Application)]
[Tooltip("Saves a Screenshot. NOTE: Does nothing in Web Player. On Android, the resulting screenshot is available some time later.")]
public class TakeScreenshot : FsmStateAction
{
	public enum Destination
	{
		MyPictures,
		PersistentDataPath,
		CustomPath
	}

	[Tooltip("Where to save the screenshot.")]
	public Destination destination;

	[Tooltip("Path used with Custom Path Destination option.")]
	public FsmString customPath;

	[RequiredField]
	public FsmString filename;

	[Tooltip("Add an auto-incremented number to the filename.")]
	public FsmBool autoNumber;

	[Tooltip("Factor by which to increase resolution.")]
	public FsmInt superSize;

	[Tooltip("Log saved file info in Unity console.")]
	public FsmBool debugLog;

	private int screenshotCount;

	public override void Reset()
	{
		destination = Destination.MyPictures;
		filename = string.Empty;
		autoNumber = null;
		superSize = null;
		debugLog = null;
	}

	public override void OnEnter()
	{
		if (string.IsNullOrEmpty(filename.Value))
		{
			return;
		}
		string text = (destination switch
		{
			Destination.MyPictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), 
			Destination.PersistentDataPath => Application.persistentDataPath, 
			Destination.CustomPath => customPath.Value, 
			_ => string.Empty, 
		}).Replace("\\", "/") + "/";
		string path = text + filename.Value + ".png";
		if (autoNumber.Value)
		{
			while (File.Exists(path))
			{
				screenshotCount++;
				path = text + filename.Value + screenshotCount + ".png";
			}
		}
		if (debugLog.Value)
		{
		}
		ScreenCapture.CaptureScreenshot(path, superSize.Value);
		Finish();
	}
}
