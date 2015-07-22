using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class FullscreenToggle : MonoBehaviour
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos (IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	private static extern IntPtr FindWindow (System.String className, System.String windowName);

	public static void SetPosition(int x, int y, int resX=0, int resY=0)
	{
		SetWindowPos (FindWindow (null, "DIS Interop"), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
	}
#endif

	void Awake ()
	{
		SetPosition (1600, 0, 1024, 768);
	}
	void Start ()
	{
		PlayerPrefs.DeleteAll ();
	}
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
	}


}
