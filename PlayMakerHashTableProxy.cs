using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayMakerHashTableProxy : PlayMakerCollectionProxy
{
	public Hashtable _hashTable;

	private Hashtable _snapShot;

	public Hashtable hashTable => _hashTable;

	public void Awake()
	{
		_hashTable = new Hashtable();
		PreFillHashTable();
		TakeSnapShot();
	}

	public bool isCollectionDefined()
	{
		return hashTable != null;
	}

	public void TakeSnapShot()
	{
		_snapShot = new Hashtable();
		foreach (object key in _hashTable.Keys)
		{
			_snapShot[key] = _hashTable[key];
		}
	}

	public void RevertToSnapShot()
	{
		_hashTable = new Hashtable();
		foreach (object key in _snapShot.Keys)
		{
			_hashTable[key] = _snapShot[key];
		}
	}

	public void InspectorEdit(int index)
	{
		dispatchEvent(setEvent, index, "int");
	}

	[ContextMenu("Copy HashTable Content")]
	private void CopyContentToPrefill()
	{
		preFillCount = hashTable.Count;
		preFillKeyList = hashTable.Keys.OfType<string>().ToList();
		switch (preFillType)
		{
		case VariableEnum.Bool:
			preFillBoolList = new List<bool>(new bool[preFillCount]);
			break;
		case VariableEnum.Color:
			preFillColorList = new List<Color>(new Color[preFillCount]);
			break;
		case VariableEnum.Float:
			preFillFloatList = new List<float>(new float[preFillCount]);
			break;
		case VariableEnum.GameObject:
			preFillGameObjectList = new List<GameObject>(new GameObject[preFillCount]);
			break;
		case VariableEnum.Int:
			preFillIntList = new List<int>(new int[preFillCount]);
			break;
		case VariableEnum.Material:
			preFillMaterialList = new List<Material>(preFillCount);
			break;
		case VariableEnum.Quaternion:
			preFillQuaternionList = new List<Quaternion>(preFillCount);
			break;
		case VariableEnum.Rect:
			preFillRectList = new List<Rect>(preFillCount);
			break;
		case VariableEnum.String:
			preFillStringList = new List<string>(new string[preFillCount]);
			break;
		case VariableEnum.Texture:
			preFillTextureList = new List<Texture2D>(preFillCount);
			break;
		case VariableEnum.Vector2:
			preFillVector2List = new List<Vector2>(preFillCount);
			break;
		case VariableEnum.Vector3:
			preFillVector3List = new List<Vector3>(new Vector3[preFillCount]);
			break;
		case VariableEnum.AudioClip:
			preFillAudioClipList = new List<AudioClip>(preFillCount);
			break;
		case VariableEnum.Byte:
			preFillByteList = new List<byte>(preFillCount);
			break;
		case VariableEnum.Sprite:
			preFillSpriteList = new List<Sprite>(preFillCount);
			break;
		}
		for (int i = 0; i < preFillKeyList.Count; i++)
		{
			switch (preFillType)
			{
			case VariableEnum.Bool:
				preFillBoolList[i] = Convert.ToBoolean(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Color:
				preFillColorList[i] = PlayMakerUtils.ConvertToColor(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Float:
				preFillFloatList[i] = Convert.ToSingle(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.GameObject:
				preFillGameObjectList[i] = hashTable[preFillKeyList[i]] as GameObject;
				break;
			case VariableEnum.Int:
				preFillIntList[i] = Convert.ToInt32(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Material:
				preFillMaterialList[i] = hashTable[preFillKeyList[i]] as Material;
				break;
			case VariableEnum.Quaternion:
				preFillQuaternionList[i] = PlayMakerUtils.ConvertToQuaternion(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Rect:
				preFillRectList[i] = PlayMakerUtils.ConvertToRect(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.String:
				preFillStringList[i] = Convert.ToString(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Texture:
				preFillTextureList[i] = hashTable[preFillKeyList[i]] as Texture2D;
				break;
			case VariableEnum.Vector2:
				preFillVector2List[i] = (Vector2)hashTable[preFillKeyList[i]];
				break;
			case VariableEnum.Vector3:
				preFillVector3List[i] = PlayMakerUtils.ConvertToVector3(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.AudioClip:
				preFillAudioClipList[i] = hashTable[preFillKeyList[i]] as AudioClip;
				break;
			case VariableEnum.Byte:
				preFillByteList[i] = Convert.ToByte(hashTable[preFillKeyList[i]]);
				break;
			case VariableEnum.Sprite:
				preFillSpriteList[i] = hashTable[preFillKeyList[i]] as Sprite;
				break;
			}
		}
	}

	private void PreFillHashTable()
	{
		for (int i = 0; i < preFillKeyList.Count; i++)
		{
			switch (preFillType)
			{
			case VariableEnum.Bool:
				hashTable[preFillKeyList[i]] = preFillBoolList[i];
				break;
			case VariableEnum.Color:
				hashTable[preFillKeyList[i]] = preFillColorList[i];
				break;
			case VariableEnum.Float:
				hashTable[preFillKeyList[i]] = preFillFloatList[i];
				break;
			case VariableEnum.GameObject:
				hashTable[preFillKeyList[i]] = preFillGameObjectList[i];
				break;
			case VariableEnum.Int:
				hashTable[preFillKeyList[i]] = preFillIntList[i];
				break;
			case VariableEnum.Material:
				hashTable[preFillKeyList[i]] = preFillMaterialList[i];
				break;
			case VariableEnum.Quaternion:
				hashTable[preFillKeyList[i]] = preFillQuaternionList[i];
				break;
			case VariableEnum.Rect:
				hashTable[preFillKeyList[i]] = preFillRectList[i];
				break;
			case VariableEnum.String:
				hashTable[preFillKeyList[i]] = preFillStringList[i];
				break;
			case VariableEnum.Texture:
				hashTable[preFillKeyList[i]] = preFillTextureList[i];
				break;
			case VariableEnum.Vector2:
				hashTable[preFillKeyList[i]] = preFillVector2List[i];
				break;
			case VariableEnum.Vector3:
				hashTable[preFillKeyList[i]] = preFillVector3List[i];
				break;
			case VariableEnum.AudioClip:
				hashTable[preFillKeyList[i]] = preFillAudioClipList[i];
				break;
			case VariableEnum.Byte:
				hashTable[preFillKeyList[i]] = preFillByteList[i];
				break;
			case VariableEnum.Sprite:
				hashTable[preFillKeyList[i]] = preFillSpriteList[i];
				break;
			}
		}
	}
}
