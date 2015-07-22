using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Openflight
{
	public static class FltUtils
	{
		public static float switchDist = 0f;
		public static void ShowHighestLOD()
		{
			FltLOD[] lods = GameObject.FindObjectsOfType<FltLOD> ();
			switchDist = 0.1f;
			foreach (FltLOD lod in lods) {
				lod.SetDistance(switchDist);
			}
		}
		public static void ShowHigherLOD()
		{
			FltLOD[] lods = GameObject.FindObjectsOfType<FltLOD> ();
			float[] switchDistances = new float[lods.Length];

			//collect all switch distances
			for (int i = 0; i < lods.Length; i++)
				switchDistances[i] = lods[i].switchOutDistance;

			//order switch distances
			switchDistances = switchDistances.OrderBy (dist => dist).ToArray();

			//get next item
			try { 
				switchDist = switchDistances.Last (dist => dist < switchDist);
			} catch {
				switchDist = switchDistances.Min();
			}

			Debug.Log ("Setting distance to " + switchDist);
			foreach (FltLOD lod in lods) {
				lod.SetDistance(switchDist);
			}
		}
		public static void ShowLowerLOD()
		{
			FltLOD[] lods = GameObject.FindObjectsOfType<FltLOD> ();
			float[] switchDistances = new float[lods.Length];
			
			//collect all switch distances
			for (int i = 0; i < lods.Length; i++)
				switchDistances[i] = lods[i].switchOutDistance;
			
			//order switch distances
			switchDistances = switchDistances.OrderBy (dist => dist).ToArray();
			
			//get next item
			try { 
				switchDist = switchDistances.First (dist => dist > switchDist);
			} catch {
				switchDist = switchDistances.Max();
			}
			
			Debug.Log ("Setting distance to " + switchDist);
			foreach (FltLOD lod in lods) {
				lod.SetDistance(switchDist);
			}
		}
		public static void ShowLowestLOD()
		{
			FltLOD[] lods = GameObject.FindObjectsOfType<FltLOD> ();
			switchDist = 0f;
			
			//find smallest switch distance
			foreach (FltLOD lod in lods)
				switchDist = lod.switchOutDistance > switchDist ? lod.switchOutDistance : switchDist;
			
			foreach (FltLOD lod in lods) {
				lod.SetDistance(switchDist);
			}
		}
		public static string ModelFolder()
		{
			string path = @"Assets/Openflight/";
			return path;
		}
		public static string TextureFolder()
		{
			return @"Assets/Openflight/Textures/";
		}
	}
}
