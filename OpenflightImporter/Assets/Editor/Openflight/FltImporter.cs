using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Openflight
{
	public class FltImporter
	{
		private GameObject topNode;
		private GameObject currentParent;
		private GameObject lastNode;

		private List<Vector3> fileVertices = new List<Vector3>();
		private List<Vector2> fileUVs = new List<Vector2>();
		private List<Vector3> fileNormals = new List<Vector3>();

		private List<FltMesh> meshes = new List<FltMesh> ();
		private bool editingMesh = false;

		private List<GameObject> switchNodes = new List<GameObject> ();
		private List<FltRecord> switchData = new List<FltRecord> ();

		public void Import(string filename)
		{
			double t0 = EditorApplication.timeSinceStartup;
			//read file
			ReadFile (filename);

			double t1 = EditorApplication.timeSinceStartup;
			Debug.Log ("Read Openflight file took " + (t1 - t0) + "seconds.");
			t0 = t1;

			//create meshes
			if (meshes.Count() > 0)
				foreach (FltMesh mesh in meshes)
					mesh.CreateMesh2 (fileVertices, fileUVs, fileNormals);

			t1 = EditorApplication.timeSinceStartup;
			Debug.Log ("Creating Meshes took " + (t1 - t0) + "seconds.");
			t0 = t1;

			//create switches
			for (int i=0; i<switchNodes.Count; i++) {
				AddSwitch( switchNodes[i], switchData[i]);
			}
			//set highest lod visible
			FltUtils.ShowHighestLOD ();

			//PrefabUtility.CreatePrefab (FltUtils.ModelFolder() + topNode.name + ".prefab", topNode);
		}

		private void ReadFile(string filename)
		{
			FileStream fs = File.OpenRead (filename);
			byte[] data = new byte[fs.Length];
			fs.Read (data, 0, data.Length);
			fs.Close ();
			
			//Top container node
			topNode = new GameObject (Path.GetFileNameWithoutExtension(filename));
			currentParent = topNode;
			lastNode = currentParent;
			
			int offset = 0;
			while (offset < data.Length) {
				FltRecord record = new FltRecord (data, ref offset);
				
				switch (record.type)
				{
					//Hierarchy traversal nodes
				case FltRecordType.PushLevel:
					if (lastNode != null)
						currentParent = lastNode;
					break;
				case FltRecordType.PopLevel:
					if (lastNode != null){
						currentParent = currentParent.transform.parent.gameObject;
						if (editingMesh)
							editingMesh = false;
					}
					lastNode = currentParent;
					break;
					
					//Hierarchy nodes
				case FltRecordType.Header:
					CreateNode (record, FltRecordType.Header);
					break;
				case FltRecordType.Group:
					CreateNode(record, FltRecordType.Group);
					break;
				case FltRecordType.Object:
					CreateNode (record, FltRecordType.Object);
					break;
				case FltRecordType.LevelOfDetail:
					CreateNode (record, FltRecordType.LevelOfDetail);
					AddLOD (lastNode, record);
					break;
				case FltRecordType.DOF:
					CreateNode (record, FltRecordType.DOF);
					break;
				case FltRecordType.Switch:
					CreateNode (record, FltRecordType.Switch);
					switchNodes.Add(lastNode);
					switchData.Add(record);
					//AddSwitch(lastNode, record);
					break;
				case FltRecordType.ExternalReference:
					CreateNode (record, FltRecordType.ExternalReference);
					break;
				case FltRecordType.Matrix:
					Debug.Log ("Matrix!");
					break;
					
					//Mesh Nodes
				case FltRecordType.Face:	//fix for pop after vertex lists
					lastNode = null;
					break;
				case FltRecordType.VertexList:	//face vertices
					lastNode = null;
					if (!editingMesh)
					{
						editingMesh = true;
						meshes.Add(new FltMesh(currentParent));
					}
					meshes[meshes.Count()-1].AddFace(record);
					break;
					
				case FltRecordType.VertexColorNormalUV:
					AddVertex(record);
					break;
				case FltRecordType.VertexColor:
					Debug.Log ("Unsupported record: " + record.type);
					break;
				case FltRecordType.VertexColorNormal:
					Debug.Log ("Unsupported record: " + record.type);
					break;
				case FltRecordType.VertexColorUV:
					Debug.Log ("Unsupported record: " + record.type);
					break;

				case FltRecordType.TexturePalette:
					//AddTextures(record, ref model);
					break;
				default: 
					break;
				}
			}
		}
		private void CreateNode(FltRecord record, FltRecordType type)
		{
			string name = record.GetName();
			GameObject node = new GameObject(type.ToString() + " " + name);
			if (currentParent != null) {
				node.transform.parent = currentParent.transform;
			}
			lastNode = node;
		}
		private void AddVertex(FltRecord record)
		{
			float x = Convert.ToSingle(FltBitConverter.ToDouble (record.data,8));
			float z = Convert.ToSingle(FltBitConverter.ToDouble (record.data,16));
			float y = Convert.ToSingle(FltBitConverter.ToDouble (record.data,24));
			float i = FltBitConverter.ToFloat (record.data,32);
			float k = FltBitConverter.ToFloat (record.data,36);
			float j = FltBitConverter.ToFloat (record.data,40);
			float u = FltBitConverter.ToFloat (record.data,44);
			float v = FltBitConverter.ToFloat (record.data,48);
			
			fileVertices.Add (new Vector3 (x, y, z));
			fileNormals.Add (new Vector3 (i, j, k));
			fileUVs.Add (new Vector2(u, v));
		}
		private void AddLOD (GameObject node, FltRecord record)
		{
			FltLOD lod = node.AddComponent<FltLOD> ();
			lod.switchInDistance = (float)FltBitConverter.ToDouble(record.data, 16);
			lod.switchOutDistance = (float)FltBitConverter.ToDouble(record.data, 24);

			float x = (float)FltBitConverter.ToDouble (record.data, 40);
			float y = (float)FltBitConverter.ToDouble (record.data, 48);
			float z = (float)FltBitConverter.ToDouble (record.data, 56);

			lod.lodCenter = new Vector3 (x, y, z);
		}
		private void AddSwitch (GameObject node, FltRecord record)
		{
			FltSwitch sw = node.AddComponent<FltSwitch> ();

			int currentMask = FltBitConverter.ToInt32 (record.data, 16);
			int maskCount = FltBitConverter.ToInt32 (record.data, 20);
			int numWordPerMask = FltBitConverter.ToInt32 (record.data, 24);
			int numChildren = node.transform.childCount;

			sw.currentMask = currentMask;
			sw.masks = new FltSwitchMask[maskCount];

			if (maskCount > 0 && numChildren > 0) {
				int i = 0;
				for (int m = 0; m < maskCount; m++) {
					for (int w = 0; w < numWordPerMask; w++) {
						//Debug.Log(BinConverter.ToUInt32(record.data, 28 + i*4));
						BitArray bits = new BitArray (FltBitConverter.GetInvertedSubArray (record.data, 28 + i * 4, 4));
						sw.masks [m] = new FltSwitchMask (numChildren);
						for (int b = 0; b < numChildren; b++)
							sw.masks [m].states [b] = bits [b];
						i++;
					}
				}
			}
		}

		private void AddTextures(FltRecord record)
		{
			//int textureID = BinConverter.ToInt32 (record.data, 204);
			//model.textureIDs.Add(textureID);

			//string textureFilename = BinConverter.CharToString (record.data, 4, 200);
			//model.textures.Add (textureFilename);
		}

		public void PrintHierarchy(string filename)
		{
			FileStream fs = File.OpenRead (filename);
			byte[] data = new byte[fs.Length];
			fs.Read (data, 0, data.Length);
			fs.Close ();

			FileStream fout = new FileStream (Path.ChangeExtension (filename, ".debug"), FileMode.OpenOrCreate); 
			StreamWriter sw = new StreamWriter (fout);
			
			int offset = 0;
			while (offset < data.Length) {
				FltRecord record = new FltRecord (data, ref offset);
				sw.WriteLine(record.type + "\t" + record.GetName());
			}
			sw.Close ();
			fout.Close ();
		}
	}
}
