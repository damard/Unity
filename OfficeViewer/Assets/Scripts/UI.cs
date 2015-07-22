using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
	private float btnWidth = 120f;
	private float btnHeight = 30f;
	private float screenPadding = 20f;
	void OnGUI()
	{
		if (GUI.Button (new Rect (Screen.width - btnWidth - screenPadding,
		                          Screen.height - btnHeight - screenPadding,
		                          btnWidth,
		                          btnHeight), "Center Camera"))
		{
			CameraController cc = transform.GetComponent<CameraController>();
			cc.ZoomExtent(transform);
		}
	}
}
