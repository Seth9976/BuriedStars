using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/HashTable")]
[Tooltip("Add key/value pairs to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
public class HashTableAddMany : HashTableActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
	[CheckForComponent(typeof(PlayMakerHashTableProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[ActionSection("Data")]
	[CompoundArray("Count", "Key", "Value")]
	[RequiredField]
	[UIHint(UIHint.FsmString)]
	[Tooltip("The Key")]
	public FsmString[] keys;

	[RequiredField]
	[Tooltip("The value for that key")]
	public FsmVar[] variables;

	[Tooltip("Ints can be stored as bytes, useful when serializing over network for efficiency")]
	public bool convertIntsToBytes;

	[ActionSection("Result")]
	[UIHint(UIHint.FsmEvent)]
	[Tooltip("The event to trigger when elements are added")]
	public FsmEvent successEvent;

	[UIHint(UIHint.FsmEvent)]
	[Tooltip("The event to trigger when elements exists already")]
	public FsmEvent keyExistsAlreadyEvent;

	public override void Reset()
	{
		gameObject = null;
		reference = null;
		keys = null;
		variables = null;
		convertIntsToBytes = false;
		successEvent = null;
		keyExistsAlreadyEvent = null;
	}

	public override void OnEnter()
	{
		if (SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			if (keyExistsAlreadyEvent != null)
			{
				FsmString[] array = keys;
				foreach (FsmString fsmString in array)
				{
					if (proxy.hashTable.ContainsKey(fsmString.Value))
					{
						base.Fsm.Event(keyExistsAlreadyEvent);
						Finish();
					}
				}
			}
			AddToHashTable();
			base.Fsm.Event(successEvent);
		}
		Finish();
	}

	public void AddToHashTable()
	{
		if (!isProxyValid())
		{
			return;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, variables[i]);
			if (variables[i].Type == VariableType.Int && convertIntsToBytes)
			{
				proxy.hashTable.Add(keys[i].Value, Convert.ToByte(valueFromFsmVar));
			}
			else
			{
				proxy.hashTable.Add(keys[i].Value, valueFromFsmVar);
			}
		}
	}
}
