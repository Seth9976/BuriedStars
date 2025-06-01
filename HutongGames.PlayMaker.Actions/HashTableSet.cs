using System;

namespace HutongGames.PlayMaker.Actions;

[ActionCategory("ArrayMaker/HashTable")]
[Tooltip("Set an key/value pair to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy)")]
public class HashTableSet : HashTableActions
{
	[ActionSection("Set up")]
	[RequiredField]
	[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
	[CheckForComponent(typeof(PlayMakerHashTableProxy))]
	public FsmOwnerDefault gameObject;

	[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
	public FsmString reference;

	[RequiredField]
	[UIHint(UIHint.FsmString)]
	[Tooltip("The Key value for that hash set")]
	public FsmString key;

	[ActionSection("Data")]
	[Tooltip("The variable to set.")]
	public FsmVar variable;

	[Tooltip("Ints can be stored as bytes, useful when serializing over network for efficiency")]
	public bool convertIntToByte;

	public override void Reset()
	{
		gameObject = null;
		reference = null;
		key = null;
		convertIntToByte = false;
		variable = null;
	}

	public override void OnEnter()
	{
		if (SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
		{
			SetHashTable();
		}
		Finish();
	}

	public void SetHashTable()
	{
		if (isProxyValid())
		{
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, variable);
			if (variable.Type == VariableType.Int && convertIntToByte)
			{
				proxy.hashTable[key.Value] = Convert.ToByte(valueFromFsmVar);
			}
			else
			{
				proxy.hashTable[key.Value] = valueFromFsmVar;
			}
		}
	}
}
