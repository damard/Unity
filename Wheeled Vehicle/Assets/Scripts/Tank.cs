using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : MonoBehaviour {
	
	public List<Axle> axles;
	
	private float forwardInput = 0.0f;
	private float steerInput = 0.0f;
	
	public float maxForwardTorque = 1500;
	public float maxRearTorque = 800;
	public float maxBrakePower = 2000;
	public float turnTorque = 200;
	
	public bool isBraking = false;
	private Rigidbody rb;
	public float speed = 0.0f;
	
	void Awake () {
		rb = GetComponent<Rigidbody> ();
	}
	
	void Update () {
		forwardInput = Input.GetAxis ("Vertical");
		steerInput = Input.GetAxis ("Horizontal");
		
		if (Input.GetAxis ("Jump") > 0)
			isBraking = true;
		else
			isBraking = (speed > 0.01 && forwardInput < 0) || (speed < -0.01 && forwardInput > 0);
		
	}
	
	void FixedUpdate () {
		speed = transform.InverseTransformVector(rb.velocity).z;
		
		float wheelTorque = forwardInput >= 0 ? maxForwardTorque : maxRearTorque;
		
		foreach (Axle axle in axles) {
			axle.leftWheel.motorTorque = wheelTorque * forwardInput + turnTorque * steerInput;
			axle.rightWheel.motorTorque = wheelTorque * forwardInput - turnTorque * steerInput;

			if (isBraking) {
				axle.leftWheel.brakeTorque = maxBrakePower * Mathf.Abs (forwardInput);
				axle.rightWheel.brakeTorque = maxBrakePower * Mathf.Abs (forwardInput);
			} else {
				axle.leftWheel.brakeTorque = 0;
				axle.rightWheel.brakeTorque = 0;
			}

			//rotate visible geometry
			ApplyLocalPositionToVisuals(axle.leftWheel);
			ApplyLocalPositionToVisuals(axle.rightWheel);
		}
	}
	
	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}
		
		Transform visualWheel = collider.transform.GetChild(0);
		
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}
	
	[System.Serializable]
	public class Axle {
		public WheelCollider leftWheel;
		public WheelCollider rightWheel;
	}
}
