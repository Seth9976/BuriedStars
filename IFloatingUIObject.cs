using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IFloatingUIObject
{
	GameObject foGameObject { get; }

	RectTransform foRectTransform { get; }

	Animator foAnimator { get; set; }

	FloatingUIHandler foHandler { get; set; }

	List<FloatingUIRoot.EventBase> foEvents { get; }

	Vector3 foRotateAngle { get; set; }

	FloatingUIRoot.ScalingParams foScalingParmas { get; }

	int foMotionLoopCount { get; set; }

	bool foIsMotionComplete { get; set; }

	string foTag { get; set; }

	Button foPsIconButton { get; }
}
