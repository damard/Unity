using UnityEngine;
using System.Collections;

public class FltLOD : MonoBehaviour {

	public float switchInDistance = 0f;
	public float switchOutDistance = 0f;
	public Vector3 lodCenter = Vector3.zero;

	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance (Camera.main.transform.position, lodCenter);
		//Debug.Log (distance);
		SetDistance (distance);

	}

	public void SetDistance (float distance)
	{
		if (distance < switchOutDistance || distance >= switchInDistance) {
			ToggleChildren (false);
		} else {
			ToggleChildren (true);
		}
	}

	private void ToggleChildren (bool state)
	{
		foreach( Transform child in transform )
			child.gameObject.SetActive( state );
	}
		 
}
