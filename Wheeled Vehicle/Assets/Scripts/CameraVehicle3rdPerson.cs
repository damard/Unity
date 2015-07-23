using UnityEngine;
using System.Collections;

public class CameraVehicle3rdPerson : MonoBehaviour {

	public Transform target;
	public Vector3 targetOffset;
	public Vector3 cameraOffset;
	
	public float smoothing = 0.15f;
	public float orbitSpeed = 3f;
	
	public bool isOrbiting = false;
	private float targetRadius;

	public float horizontalInput;
	public float verticalInput;

	void Awake () {
		targetRadius = (cameraOffset - targetOffset).magnitude;
		transform.position = target.TransformPoint (cameraOffset);
	}
	
	// Use this for initialization
	void Update () {
		if (Input.GetMouseButtonDown (1)) {//rightclick
			isOrbiting = true;
			targetRadius = (transform.position - (target.TransformPoint(targetOffset))).magnitude;
		}
		if (Input.GetMouseButtonUp (1))
			isOrbiting = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!isOrbiting) {
			transform.position = Vector3.Lerp (transform.position, target.TransformPoint (cameraOffset), smoothing);
		}
		else {
			Orbit ();
		}

		transform.LookAt (target.position + targetOffset);
	}

	void Orbit ()
	{
		horizontalInput = Input.GetAxisRaw ("Mouse X");
		verticalInput = Input.GetAxisRaw ("Mouse Y");

		Vector3 targetPosition = target.TransformPoint (targetOffset);
		Vector3 camVector = transform.position - targetPosition;

		if (verticalInput != 0)
			transform.RotateAround (targetPosition, Vector3.right, verticalInput * -orbitSpeed);
		if (horizontalInput != 0)
			transform.RotateAround (targetPosition, Vector3.up, horizontalInput * orbitSpeed);


		transform.position = targetPosition + camVector.normalized * targetRadius;
	}
}
