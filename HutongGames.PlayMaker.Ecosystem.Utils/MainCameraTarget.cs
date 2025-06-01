using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils;

[Serializable]
public class MainCameraTarget
{
	public CameraSelectionOptions selection;

	[SerializeField]
	private GameObject _gameObject;

	public Component component;

	public string expectedComponentType;

	public GameObject gameObject
	{
		get
		{
			if (selection == CameraSelectionOptions.MainCamera)
			{
				return Camera.main.gameObject;
			}
			return _gameObject;
		}
		set
		{
			if (selection == CameraSelectionOptions.MainCamera && value != Camera.main)
			{
				UnityEngine.Debug.LogError("MainCameraTarget: you are trying to assign a GameObject that is not the MainCamera");
			}
			else
			{
				_gameObject = value;
			}
		}
	}
}
