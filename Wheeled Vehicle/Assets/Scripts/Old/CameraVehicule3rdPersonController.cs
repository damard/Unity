using UnityEngine;
using System.Collections;

public class CameraVehicule3rdPersonController : MonoBehaviour {

	public Transform target;
	public Vector3 targetOffset;
	public Vector3 cameraOffset;

	public float smoothing = 0.15f;
	public float orbitSpeed = 3f;

	private bool isOrbiting = false;
	private float targetRadius;

	void Awake () {
		targetRadius = (cameraOffset - targetOffset).magnitude;
	}

	// Use this for initialization
	void Update () {
		if (Input.GetMouseButtonDown (1))
			isOrbiting = true;
		if (Input.GetMouseButtonUp (1))
			isOrbiting = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isOrbiting) {
			transform.position = Vector3.Lerp (transform.position, target.TransformPoint (cameraOffset), smoothing);
		}
		else {
			Vector3 worldTargetOffset = target.TransformPoint(targetOffset);
			transform.position = worldTargetOffset + (transform.position - worldTargetOffset).normalized * targetRadius;
			transform.RotateAround(worldTargetOffset, Vector3.up, Input.GetAxisRaw("Mouse X") * orbitSpeed);
			transform.RotateAround(worldTargetOffset, Vector3.right, Input.GetAxisRaw("Mouse Y") * -orbitSpeed);
		}
		transform.LookAt (target.position + targetOffset);

	}
}
