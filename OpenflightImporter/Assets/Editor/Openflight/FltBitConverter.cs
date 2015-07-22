using System;
namespace Openflight
{
	public static class FltBitConverter	//conversion for big endian ?
	{
		public static byte[] GetSubArray(byte[] data, int start, int count)
		{
			byte[] bytes = new byte[count];
			Array.Copy(data, start, bytes, 0, count);
			return bytes;
		}
		public static byte[] GetInvertedSubArray(byte[] data, int start, int count)
		{
			byte[] bytes = GetSubArray (data, start, count);
			Array.Reverse (bytes);
			return bytes;
		}
		public static Int16 ToInt16(byte[] data, int offset)
		{
			return BitConverter.ToInt16 (GetInvertedSubArray (data, offset, 2), 0);
		}
		public static Int32 ToInt32(byte[] data, int offset)
		{
			return BitConverter.ToInt32 (GetInvertedSubArray (data, offset, 4), 0);
		}
		public static UInt16 ToUInt16(byte[] data, int offset)
		{
			return BitConverter.ToUInt16 (GetInvertedSubArray(data, offset, 2), 0);
		}
		public static UInt32 ToUInt32(byte[] data, int offset)
		{
			return BitConverter.ToUInt32 (GetInvertedSubArray(data, offset, 4), 0);
		}
		public static char ToChar(byte[] data, int offset, int lenght)
		{
			return BitConverter.ToChar (GetInvertedSubArray(data, offset, lenght), 0);
		}
		public static double ToDouble(byte[] data, int offset)
		{
			return BitConverter.ToDouble (GetInvertedSubArray (data, offset, 8), 0);
		}
		public static float ToFloat(byte[] data, int offset)
		{
			return BitConverter.ToSingle (GetInvertedSubArray (data, offset, 4), 0);
		}
		public static string CharToString(byte[] data, int offset, int lenght)
		{
			byte[] subData = GetSubArray (data, offset, lenght);

			return (System.Text.Encoding.ASCII.GetString (subData));
		}
	}
}