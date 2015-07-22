using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Openflight
{
	public class FltMesh
	{
		public GameObject parent;
		public List<List<int>> faces = new List<List<int>>();

		public FltMesh(GameObject p)
		{
			parent = p;
		}

		public void AddFace (FltRecord record)
		{
			int numVerts = (record.lenght - 4) / 4;

			List<int> face = new List<int> ();
			for (int i = 1; i <= numVerts; i++) {
				int vertByteOffset = (FltBitConverter.ToInt32 (record.data, 4 * i));
				face.Add ((vertByteOffset - 8) / 64);
			}
			faces.Add (face);	
		}

		public void CreateMesh2(List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals)
		{
			double t0 = EditorApplication.timeSinceStartup;
			//Debug.Log ("Face Count: " + faces.Count ());
			if (faces.Count () <= 0)
				return;
			
			List<int> mVerticesID = new List<int> ();
			List<int> mTriangles = new List<int> ();

			//populate mVerticesID containing all vertices used in all faces of mesh
			foreach (List<int> face in faces) {
				mVerticesID.AddRange(face);
			}
			//make it unique
			int[] mVerticesIDArr = mVerticesID.Distinct ().ToArray ();

			int[] vertLookup = new int[vertices.Count];
			for (int i=0; i<vertLookup.Length; i++) { 
				vertLookup[i] = -1; //init values to -1
			}	
			for (int i = 0; i < mVerticesIDArr.Length; i++) { 
				vertLookup[mVerticesIDArr[i]] = i; 
			}

			Vector3[] mVertices = new Vector3[mVerticesIDArr.Length];
			Vector2[] mUVs = new Vector2[mVerticesIDArr.Length];
			Vector3[] mNormals = new Vector3[mVerticesIDArr.Length];

			//populate data array
			for (int i = 0; i < mVerticesIDArr.Length; i++) {
				mVertices[i]= vertices[mVerticesIDArr[i]];
				mUVs[i] 	= uvs[mVerticesIDArr[i]];
				mNormals[i] = normals[mVerticesIDArr[i]];
			}

			Debug.Log ("Preparing vert data took: " + (t0 - EditorApplication.timeSinceStartup));
			t0 = EditorApplication.timeSinceStartup;

			double tVertLookup = 0;
			double tFaceCreation = 0;

			double tf;
			//generate face and vertices data
			foreach (List<int> face in faces) {
				tf = EditorApplication.timeSinceStartup;

				List<int> newFace = new List<int> ();
				foreach (int vertID in face){

					int newID = vertLookup[vertID];//Array.IndexOf(mVerticesIDArr, vertID); //mVerticesIDArr.Contains(vertID);					
					if (newID >= 0){
						newFace.Add(newID);
					}
				}
				tVertLookup += EditorApplication.timeSinceStartup - tf;

				tf = EditorApplication.timeSinceStartup;
				newFace.Reverse();
				if (newFace.Count() == 3) {
					mTriangles.AddRange(newFace);
				} else {	//implement face triangulation algorithm
					Debug.Log ("Non-triangle face");
				}
				tFaceCreation += EditorApplication.timeSinceStartup - tf;
			}
			Debug.Log ("Computing face verts took: " + (t0 - EditorApplication.timeSinceStartup));
			Debug.Log ("  vertex lookup: " + tVertLookup);
			Debug.Log ("  face data creation: " + tFaceCreation);
			t0 = EditorApplication.timeSinceStartup;

			//Generate mesh object
			Mesh mesh = new Mesh ();
			mesh.vertices = mVertices.ToArray ();
			mesh.uv = mUVs.ToArray ();
			mesh.normals = mNormals.ToArray ();
			mesh.triangles = mTriangles.ToArray ();
			
			//add meshrenderer / meshfilters to parent and apply mesh
			MeshRenderer mr = parent.AddComponent<MeshRenderer> ();
			mr.sharedMaterial = new Material (Shader.Find ("Standard"));
			MeshFilter mf = parent.AddComponent<MeshFilter> ();
			mf.sharedMesh = mesh;
		}
		public void CreateMesh(List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals)
		{
			//Debug.Log ("Face Count: " + faces.Count ());
			if (faces.Count () <= 0)
				return;

			List<int> mVerticesID = new List<int> ();
			List<Vector3> mVertices = new List<Vector3> ();
			List<Vector2> mUVs = new List<Vector2> ();
			List<Vector3> mNormals = new List<Vector3> ();
			List<int> mTriangles = new List<int> ();

			double t0 = EditorApplication.timeSinceStartup;
			double tVertTest = 0;
			double tFacedata = 0;
			double tRevertFace = 0;

			//generate face and vertices data
			foreach (List<int> face in faces) {

				List<int> newFace = new List<int> ();
				foreach (int vertID in face){
					int newID;
					double tVertTest0 = EditorApplication.timeSinceStartup;
					bool vertTest = mVerticesID.Contains(vertID);
					tVertTest += EditorApplication.timeSinceStartup - tVertTest0;

					if (vertTest){
						newID = mVerticesID.IndexOf (vertID);
					}else{
						mVerticesID.Add (vertID);
						mVertices.Add (vertices[vertID]);
						mUVs.Add (uvs[vertID]);
						mNormals.Add (normals[vertID]);
						newID = mVerticesID.Count() - 1;
					}
					newFace.Add(newID);
				}
				newFace.Reverse();
				if (newFace.Count() == 3) {
					mTriangles.AddRange(newFace);
				} else {	//implement face triangulation algorithm
					Debug.Log ("Non-triangle face");
				}
			}
			double t1 = EditorApplication.timeSinceStartup;
			Debug.Log ("Computing mesh data: " + (t1 - t0) + "seconds.");
			t0 = t1;

			Debug.Log ("\tTesting Vertex Existance: " + tVertTest + "seconds.");

			//Generate mesh object
			Mesh mesh = new Mesh ();
			mesh.vertices = mVertices.ToArray ();
			mesh.uv = mUVs.ToArray ();
			mesh.normals = mNormals.ToArray ();
			mesh.triangles = mTriangles.ToArray ();

			t1 = EditorApplication.timeSinceStartup;
			Debug.Log ("Creating mesh object: " + (t1 - t0) + "seconds.");
			t0 = t1;

			//add meshrenderer / meshfilters to parent and apply mesh
			MeshRenderer mr = parent.AddComponent<MeshRenderer> ();
			mr.sharedMaterial = new Material (Shader.Find ("Standard"));
			MeshFilter mf = parent.AddComponent<MeshFilter> ();
			mf.sharedMesh = mesh;

			t1 = EditorApplication.timeSinceStartup;
			Debug.Log ("Applying mesh to object: " + (t1 - t0) + "seconds.");
			t0 = t1;
		}


	}
	
}
