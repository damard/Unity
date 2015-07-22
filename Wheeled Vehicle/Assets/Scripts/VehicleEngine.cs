using UnityEngine;
using System.Collections;

public class VehicleEngine : MonoBehaviour {

	public float[] torqueCurve = new float[11]{400, 420, 440, 445, 450, 465, 480, 490, 480, 450, 400}; //500 rpm increments
	public float minEngineRPM = 1000;
	public float maxEngineRPM = 6000;
	private float curEngineRPM;

	public float[] gearRatios = new float[7]{2.66, 0, 1.78, 1.30, 1.0, 0.74, 0.5}; //R, N, 1, 2, 3, 4, 5, 6
	public int currentGear;

	public float differentialRatio = 3.42;
	public float transmissionEfficiency = 0.7; //assumes a 30% energy loss in heat, etc.

	public float wheelRadius; //in meters
	private float wheelCircumference;

	private float throttle;

	private bool isBraking = false;
	private float maxBrakeTorque;

	// Use this for initialization
	void Awake () 
	{
		wheelCircumference = 2.0 * wheelRadius * Mathf.PI;
		currentGear = 1;
	}
	
	void Update () 
	{
		//input controls
		throttle = Mathf.Abs(Input.GetAxis ("Vertical"));
	}

	void FixedUpdate () 
	{
		//compute current engine torque from wheel rotation speed

		//Compute the current torque output
		float driveTorque = GetEngineTorque (curEngineRPM) * gearRatios [currentGear] * differentialRatio * transmissionEfficiency * throttle;
	}

	private float GetEngineTorque (float engineRPM)
	{
		if (engineRPM <= minEngineRPM)
			return torqueCurve [0];
		if (engineRPM >= maxEngineRPM)
			return torqueCurve [torqueCurve.Length - 1];

		float rpmRatio = (engineRPM - minEngineRPM) / (maxEngineRPM - minEngineRPM) * (torqueCurve.Length - 1); //gets the ratio of current torque relative to min/max torque and interpolates which element of the torque array it is

		float infTorque = torqueCurve [Mathf.FloorToInt (rpmRatio)];
		float supTorque = torqueCurve [Mathf.CeilToInt (rpmRatio)];
		float actualTorque = infTorque + (supTorque - infTorque) * (rpmRatio % Mathf.Floor (rpmRatio));
	}
	
}
