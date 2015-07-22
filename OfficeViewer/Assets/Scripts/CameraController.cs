using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 cameraAngle = new Vector3(-1,-1,-1);
	public float xSpeed = 250f;
	public float ySpeed = 120f;
	public float yMinLimit = 0f;
	public float yMaxLimit = 80f;
	[Range(1f,10f)]
	public float zoomSpeed = 1f;
	public float panSpeed = 0.5f;

	private Camera cam;
	private float x = 0f;
	private float y = 0f;
	public float distance = 0f;

	private Vector3 targetCenter;

	// Use this for initialization
	void Start () 
	{
		cam = Camera.main;
		ZoomExtent (target);

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		if (rigidbody)
			rigidbody.freezeRotation = true;
		
	}
	
	// Update is called once per frame
	void LateUpdate()
	{
		if (target && Input.GetMouseButton(0)) 
		{
			x += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
			y = ClampAngle (y, yMinLimit, yMaxLimit);
			UpdateCameraTransform();
		}
		if (target && Input.GetMouseButton(2))
		{
			Vector3 panVector = (transform.right * -Input.GetAxis("Mouse X") + transform.up * -Input.GetAxis("Mouse Y")) * panSpeed;
			targetCenter += panVector;
			transform.position += panVector;
			/*transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
			transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);*/
		}
		if (Input.GetAxis ("Mouse ScrollWheel") != 0)
		{
			float newDistance = distance -Input.GetAxis ("Mouse ScrollWheel")*zoomSpeed;
			distance = (newDistance > 1 && newDistance < 20) ? newDistance : distance;
			UpdateCameraTransform();
		}

	}

	private void UpdateCameraTransform()
	{
		Quaternion rotation = Quaternion.Euler (y, x, 0);
		Vector3 position = rotation * (new Vector3 (0, 0, -distance)) + targetCenter;
		transform.rotation = rotation;
		transform.position = position;
	}
	public void ZoomExtent (Transform t)
	{
		Bounds b = GetBounds (t);
		Vector3 max = b.size;
		float radius = Mathf.Max (new float[]{max.x, max.y, max.z});
		distance = radius / (Mathf.Tan (cam.fieldOfView * Mathf.Deg2Rad / 2f));
		targetCenter = b.center;
		Vector3 pos = -cameraAngle.normalized * distance + b.center;
		cam.transform.position = pos;
		cam.transform.LookAt(targetCenter);

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}
	private Bounds GetBounds (Transform t)
	{
		Bounds b = new Bounds (t.position, Vector3.zero);
		Component[] rList = t.GetComponentsInChildren(typeof(Renderer));
		foreach (Renderer r in rList)
		{
			b.Encapsulate(r.bounds);
		}
		return b;
	}
	private float ClampAngle (float angle, float min, float max) 
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect (Screen.width - 120,
		                          Screen.height - 40,
		                          100,
		                          20), "Center Camera"))
		{
			CameraController cc = transform.GetComponent<CameraController>();
			cc.ZoomExtent(target);
		}
	}
}
