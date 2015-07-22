using UnityEngine;
using System.Collections;
using UnityEditor;
using Openflight;

public class FltImporterUI : Editor 
{
	[MenuItem ("Openflight/Import Model", false, 0)]
	static void ImportModelDialog() { //@"C:\Users\David\Desktop\Zone_Vie_p.flt";//
		string filename = EditorUtility.OpenFilePanel ("Select Openflight(.flt) file to import", @"C:\fltImporter", "flt");
		if (filename.Length != 0) {
			FltImporter fltReader = new FltImporter();
			fltReader.Import(filename);
		}
	}

	[MenuItem ("Openflight/Print Hierarchy", false, 1)]
	static void PrintHierarchyDialog() { //@"C:\Users\David\Desktop\Zone_Vie_p.flt";//
		string filename = EditorUtility.OpenFilePanel ("Select Openflight(.flt) file to import", @"C:\", "flt");
		if (filename.Length != 0) {
			FltImporter fltReader = new FltImporter();
			fltReader.PrintHierarchy(filename);
		}
	}

	[MenuItem ("Openflight/Highest LOD", false, 20)]
	static void ShowHighestLOD () { 
		FltUtils.ShowHighestLOD ();
	}
	[MenuItem ("Openflight/Higher LOD", false, 21)]
	static void ShowHigherLOD () { 
		FltUtils.ShowHigherLOD ();
	}
	[MenuItem ("Openflight/Lower LOD", false, 22)]
	static void ShowLowerLOD () { 
		FltUtils.ShowLowerLOD ();
	}
	[MenuItem ("Openflight/Lowest LOD", false, 23)]
	static void ShowLowestLOD () { 
		FltUtils.ShowLowestLOD ();
	}
}
