using UnityEngine;
using System.Collections;

public class VehicleDebugger : MonoBehaviour {

	public Rect rect;

	private Rigidbody rb;
	
	void Awake () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void OnGUI () {
		string speed = "Speed = " + (transform.InverseTransformVector(rb.velocity).magnitude * 3.6f).ToString() + " km/h";
		GUI.Label (rect, new GUIContent (speed));
	}
}
