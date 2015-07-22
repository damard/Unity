using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCarController : MonoBehaviour {

	public List<Axle> axles;
	public float maxMotorTorque;
	public float maxSteeringAngle;

	public float steeringInput;
	public float forwardInput;

	[System.Serializable]
	public class Axle {
		public WheelCollider leftWheel;
		public WheelCollider rightWheel;
		public bool isMotor;
		public bool isSteering;
	}

	void Update()
	{
		forwardInput = Input.GetAxis ("Vertical");
		steeringInput = Input.GetAxis ("Horizontal");
	}

	void FixedUpdate()
	{
		float motorTorque = maxMotorTorque * Input.GetAxis ("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis ("Horizontal");

		foreach (Axle axle in axles) {
			if (axle.isSteering) {
				axle.leftWheel.steerAngle = steering;
				axle.rightWheel.steerAngle = steering;
			}
			if (axle.isMotor) {
				axle.leftWheel.motorTorque = motorTorque;
				axle.rightWheel.motorTorque = motorTorque;
			}

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
}
