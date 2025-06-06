using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Offset")]
public class UIButtonOffset : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = Vector3.zero;

	public Vector3 pressed = new Vector3(2f, -2f);

	public float duration = 0.2f;

	[NonSerialized]
	private Vector3 mPos;

	[NonSerialized]
	private bool mStarted;

	[NonSerialized]
	private bool mPressed;

	private void Start()
	{
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null)
			{
				tweenTarget = base.transform;
			}
			mPos = tweenTarget.localPosition;
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	private void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenPosition component = tweenTarget.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.value = mPos;
				component.enabled = false;
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		mPressed = isPressed;
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenPosition.Begin(tweenTarget.gameObject, duration, isPressed ? (mPos + pressed) : ((!UICamera.IsHighlighted(base.gameObject)) ? mPos : (mPos + hover))).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenPosition.Begin(tweenTarget.gameObject, duration, (!isOver) ? mPos : (mPos + hover)).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnDragOver()
	{
		if (mPressed)
		{
			TweenPosition.Begin(tweenTarget.gameObject, duration, mPos + hover).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnDragOut()
	{
		if (mPressed)
		{
			TweenPosition.Begin(tweenTarget.gameObject, duration, mPos).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			OnHover(isSelected);
		}
	}
}
