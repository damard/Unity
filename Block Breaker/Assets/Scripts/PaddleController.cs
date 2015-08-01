using UnityEngine;
using System.Collections;

public class PaddleController : MonoBehaviour {

	private Rigidbody2D rb;
	
	void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float mousePosInBlocks = Input.mousePosition.x / Screen.width * 16f;
		print (mousePosInBlocks);
	}
}
