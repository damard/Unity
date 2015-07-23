using UnityEngine;
using System.Collections;

public class CameraVehicle1stPerson : MonoBehaviour {
	
	public Transform vehicle;
	public Vector3 eyeOffset;
	private Vector3 viewDir; //relative to vehicle
	
	// Use this for initialization
	void Awake () {
		viewDir = new Vector3(0,0,1);
	
		SetCameraPosition();
		SetViewDirection();
	}
	
	// Update is called once per frame
	void Update () {
		SetCameraPosition();
		SetViewDirection();
	}
	
	void SetViewDirection() {
		transform.rotation = vehicle.rotation;
		//transform.forward = vehicle.TransformVector(viewDir);
	}
	
	void SetCameraPosition() {
		transform.position = vehicle.TransformPoint(eyeOffset);
	}
}
