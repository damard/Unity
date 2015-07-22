using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Openflight
{
	public class FltRecord
	{
		public FltRecordType type;
		public int lenght;
		public byte[] data;

		public FltRecord (byte[] data, ref int offset)
		{
			int recordType = FltBitConverter.ToInt16(data, offset);
			int recordLenght = FltBitConverter.ToInt16 (data, offset + 2);
			
			byte[] recordData = new byte[recordLenght];
			Array.Copy (data, offset, recordData, 0, recordLenght); //Saves Sub byte array = Record Data
			
			offset += recordLenght;

			this.type = (FltRecordType)recordType;
			this.lenght = recordLenght;
			this.data = recordData;
		}

		public string GetName()
		{
			if (this.lenght >= 12)
				return FltBitConverter.CharToString (this.data, 4, 8);
			else
				return "null";
		}
	}
	
}
