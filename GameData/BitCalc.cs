using System;
using System.Text;
using UnityEngine;

namespace GameData;

public class BitCalc
{
	public static long ByteToLong(byte[] bySrc, int iIdx)
	{
		return ((long)ByteToInt(bySrc, iIdx + 4) << 32) | (ByteToInt(bySrc, iIdx) & 0xFFFFFFFFu);
	}

	public static void LongToByte(long lSrc, byte[] byDst, int iIdx)
	{
		byDst[iIdx] = (byte)(0xFF & lSrc);
		byDst[iIdx + 1] = (byte)(0xFF & (lSrc >> 8));
		byDst[iIdx + 2] = (byte)(0xFF & (lSrc >> 16));
		byDst[iIdx + 3] = (byte)(0xFF & (lSrc >> 24));
		byDst[iIdx + 4] = (byte)(0xFF & (lSrc >> 32));
		byDst[iIdx + 5] = (byte)(0xFF & (lSrc >> 40));
		byDst[iIdx + 6] = (byte)(0xFF & (lSrc >> 48));
		byDst[iIdx + 7] = (byte)(0xFF & (lSrc >> 56));
	}

	public static int ByteToInt(byte[] bySrc, int iIdx)
	{
		return ((bySrc[iIdx] & 0xFF) << 0) | ((bySrc[iIdx + 1] & 0xFF) << 8) | ((bySrc[iIdx + 2] & 0xFF) << 16) | ((bySrc[iIdx + 3] & 0xFF) << 24);
	}

	public static void IntToByte(int iSrc, byte[] byDst, int iIdx)
	{
		byDst[iIdx] = (byte)(0xFF & (iSrc >> 0));
		byDst[iIdx + 1] = (byte)(0xFF & (iSrc >> 8));
		byDst[iIdx + 2] = (byte)(0xFF & (iSrc >> 16));
		byDst[iIdx + 3] = (byte)(0xFF & (iSrc >> 24));
	}

	public static void Vector3ToByteNCO(Vector3 vecTemp, byte[] bySrc, ref int iOffset)
	{
		FloatToByteNCO(vecTemp.x, bySrc, ref iOffset);
		FloatToByteNCO(vecTemp.y, bySrc, ref iOffset);
		FloatToByteNCO(vecTemp.z, bySrc, ref iOffset);
	}

	public static void ByteToVector3NCO(ref Vector3 vecTemp, byte[] byDst, ref int iOffset)
	{
		vecTemp.x = ByteToFloatNCO(byDst, ref iOffset);
		vecTemp.y = ByteToFloatNCO(byDst, ref iOffset);
		vecTemp.z = ByteToFloatNCO(byDst, ref iOffset);
	}

	public static float ByteToFloat(byte[] bySrc, int iIdx)
	{
		return BitConverter.ToSingle(bySrc, iIdx);
	}

	public static void FloatToByte(float fSrc, byte[] byDst, int iIdx)
	{
		byte[] bytes = BitConverter.GetBytes(fSrc);
		Buffer.BlockCopy(bytes, 0, byDst, iIdx, 4);
	}

	public static short ByteToShort(byte[] bySrc, int iIdx)
	{
		return (short)((bySrc[iIdx] & 0xFF) | ((bySrc[iIdx + 1] & 0xFF) << 8));
	}

	public static void ShortToByte(short nSrc, byte[] byDst, int iIdx)
	{
		byDst[iIdx] = (byte)(0xFF & (nSrc >> 0));
		byDst[iIdx + 1] = (byte)(0xFF & (nSrc >> 8));
	}

	public static void ByteToByte(byte[] bySrc, int iSrcOffset, byte[] byDst, int iDstOffset, int iSize)
	{
		Buffer.BlockCopy(bySrc, iSrcOffset, byDst, iDstOffset, iSize);
	}

	public static int StringToByte(string strText, byte[] byDst, int iIdx)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(strText);
		int iSize = bytes.Length;
		int iSrcOffset = 0;
		ByteToByte(bytes, iSrcOffset, byDst, iIdx, iSize);
		return bytes.Length;
	}

	public static void BooleanToByteNCO(bool isValue, byte[] bySrc, ref int iIdx)
	{
		bySrc[iIdx++] = (byte)(isValue ? 1u : 0u);
	}

	public static bool ByteToBooleanNCO(byte[] byDst, ref int iIdx)
	{
		return byDst[iIdx++] == 1;
	}

	public static long ByteToLongNCO(byte[] bySrc, ref int iIdx)
	{
		long result = ByteToLong(bySrc, iIdx);
		iIdx += 8;
		return result;
	}

	public static void LongToByteNCO(long lSrc, byte[] byDst, ref int iIdx)
	{
		LongToByte(lSrc, byDst, iIdx);
		iIdx += 8;
	}

	public static int ByteToIntNCO(byte[] bySrc, ref int iIdx)
	{
		int result = ByteToInt(bySrc, iIdx);
		iIdx += 4;
		return result;
	}

	public static void IntToByteNCO(int iSrc, byte[] byDst, ref int iIdx)
	{
		IntToByte(iSrc, byDst, iIdx);
		iIdx += 4;
	}

	public static float ByteToFloatNCO(byte[] bySrc, ref int iIdx)
	{
		float result = ByteToFloat(bySrc, iIdx);
		iIdx += 4;
		return result;
	}

	public static void FloatToByteNCO(float fSrc, byte[] byDst, ref int iIdx)
	{
		FloatToByte(fSrc, byDst, iIdx);
		iIdx += 4;
	}

	public static short ByteToShortNCO(byte[] bySrc, ref int iIdx)
	{
		short result = ByteToShort(bySrc, iIdx);
		iIdx += 2;
		return result;
	}

	public static void ShortToByteNCO(short nSrc, byte[] byDst, ref int iIdx)
	{
		ShortToByte(nSrc, byDst, iIdx);
		iIdx += 2;
	}

	public static void ByteToByteNCO(byte[] bySrc, ref int iSrcOffset, byte[] byDst, ref int iDstOffset, int iSize)
	{
		Buffer.BlockCopy(bySrc, iSrcOffset, byDst, iDstOffset, iSize);
		iSrcOffset += iSize;
		iDstOffset += iSize;
	}

	public static void FloatArrToByteArrNCO(float[] fSrc, ref int iSrcOffset, byte[] byDst, ref int iDstOffset, int iSize)
	{
		int num = iSize * 4;
		Buffer.BlockCopy(fSrc, iSrcOffset, byDst, iDstOffset, num);
		iSrcOffset += iSize;
		iDstOffset += num;
	}

	public static void ByteArrToFloatArrNCO(byte[] bySrc, ref int iSrcOffset, float[] fDst, ref int iDstOffset, int iSize)
	{
		int num = iSize * 4;
		Buffer.BlockCopy(bySrc, iSrcOffset, fDst, iDstOffset, num);
		iSrcOffset += num;
		iDstOffset += iSize;
	}

	public static void ByteArrayToIntArrayNCO(byte[] bySrc, ref int iSrcOffset, int[] iDst, ref int iDstOffset, int iSize)
	{
		int num = iSize * 4;
		Buffer.BlockCopy(bySrc, iSrcOffset, iDst, iDstOffset, num);
		iDstOffset += iSize;
		iSrcOffset += num;
	}

	public static void IntArrayToByteArrayNCO(int[] iSrc, ref int iSrcOffset, byte[] byDst, ref int iDstOffset, int iSize)
	{
		int num = iSize * 4;
		Buffer.BlockCopy(iSrc, iSrcOffset, byDst, iDstOffset, num);
		iDstOffset += num;
		iSrcOffset += iSize;
	}

	public static string ByteToStringWithSizeNCO(byte[] bySrc, ref int iIdx)
	{
		string result = null;
		int num = ByteToIntNCO(bySrc, ref iIdx);
		if (num > 0)
		{
			byte[] array = new byte[num];
			int iDstOffset = 0;
			ByteToByteNCO(bySrc, ref iIdx, array, ref iDstOffset, num);
			result = Encoding.Unicode.GetString(array);
		}
		return result;
	}

	public static int GetSizeStringToByte(string strText)
	{
		int result = 0;
		byte[] array = null;
		if (strText != null)
		{
			array = Encoding.Unicode.GetBytes(strText.ToCharArray());
		}
		if (array != null)
		{
			result = array.Length;
		}
		return result;
	}

	public static void StringToByteWithSizeNCO(string strText, byte[] byDst, ref int iIdx)
	{
		int num = 0;
		if (strText == null)
		{
			IntToByteNCO(0, byDst, ref iIdx);
			return;
		}
		byte[] bytes = Encoding.Unicode.GetBytes(strText);
		int num2 = bytes.Length;
		IntToByteNCO(num2, byDst, ref iIdx);
		num = 0;
		ByteToByteNCO(bytes, ref num, byDst, ref iIdx, num2);
	}

	public static int GetStringToByteSize(string strText)
	{
		if (strText == null)
		{
			return 0;
		}
		byte[] bytes = Encoding.Unicode.GetBytes(strText);
		return bytes.Length;
	}

	public static bool CheckArrayIdx(int iIdx, int iSize)
	{
		if (iIdx < 0 || iIdx >= iSize)
		{
			return false;
		}
		return true;
	}

	public static void InitArray(byte[,] byArray, byte byInitValue = 0)
	{
		int length = byArray.GetLength(0);
		int length2 = byArray.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				byArray[i, j] = byInitValue;
			}
		}
	}

	public static void InitArray(int[,] iArray, int iInitValue = 0)
	{
		int length = iArray.GetLength(0);
		int length2 = iArray.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				iArray[i, j] = iInitValue;
			}
		}
	}

	public static void InitArray(bool[] isArray, bool isInitValue = false)
	{
		int num = isArray.Length;
		for (int i = 0; i < num; i++)
		{
			isArray[i] = isInitValue;
		}
	}

	public static void InitArray(char[] chArray, char chInitValue = '\0')
	{
		int num = chArray.Length;
		for (int i = 0; i < num; i++)
		{
			chArray[i] = chInitValue;
		}
	}

	public static void InitArray(byte[] byArray, byte byInitValue = 0)
	{
		int num = byArray.Length;
		for (int i = 0; i < num; i++)
		{
			byArray[i] = byInitValue;
		}
	}

	public static void InitArray(int[] iArray, int iInitValue = 0)
	{
		int num = iArray.Length;
		for (int i = 0; i < num; i++)
		{
			iArray[i] = iInitValue;
		}
	}

	public static void InitArray(float[] fArray, float fInitValue = 0f)
	{
		int num = fArray.Length;
		for (int i = 0; i < num; i++)
		{
			fArray[i] = fInitValue;
		}
	}

	public static void InitArray(string[] strArray, string strInitValue = null)
	{
		int num = strArray.Length;
		for (int i = 0; i < num; i++)
		{
			strArray[i] = strInitValue;
		}
	}
}
