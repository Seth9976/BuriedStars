using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("Project_BAL")]
public class GetActiveStateName : FsmStateAction
{
	public LogLevel logLevel;

	public override void OnEnter()
	{
		GetStateList();
		Finish();
	}

	private void GetStateList()
	{
		List<Fsm> list = new List<Fsm>(Fsm.FsmList);
		if (list.Count > 0)
		{
			Finish();
		}
		foreach (Fsm item in list)
		{
			GameObject gameObject = item.GameObject;
			GameObject gameObject2 = ((!(gameObject.transform.parent == null)) ? gameObject.transform.parent.gameObject : null);
			if (gameObject2 != null)
			{
				GameObject gameObject3 = ((!(gameObject2.transform.parent == null)) ? gameObject2.transform.parent.gameObject : null);
				string text = gameObject2.name.Substring(0, 3);
				string text2 = string.Empty;
				string text3 = gameObject2.name + "/" + item.GameObjectName;
				if (gameObject3 != null)
				{
					text3 = gameObject3.name + "/" + text3;
					text2 = gameObject3.name.Substring(0, 3);
				}
				if (text == "evt" || text2 == "evt")
				{
					text3 = "Hierarchy::" + text3 + "\n FSM :: " + item.Name + " // State :: " + item.ActiveStateName;
				}
			}
		}
	}
}
