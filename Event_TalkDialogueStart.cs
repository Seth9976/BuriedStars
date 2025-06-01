using UnityEngine;

public class Event_TalkDialogueStart : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.transform.GetChild(0).GetComponent<Event_TalkDialogue>().InitTalkWindow();
	}
}
