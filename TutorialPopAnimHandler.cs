using UnityEngine;

public class TutorialPopAnimHandler : MonoBehaviour
{
	private void SetNextPageText()
	{
		if (TutorialPopup.instance != null)
		{
			TutorialPopup.instance.SetNextPageText();
		}
	}

	private void SetStateIdle()
	{
		if (TutorialPopup.instance != null)
		{
			TutorialPopup.instance.SetStateIdle();
		}
	}

	private void SetBuildText()
	{
		if (TutorialPopup.instance != null)
		{
			TutorialPopup.instance.BuildTagText();
		}
	}
}
