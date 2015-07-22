using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float scale = 1.5f;
	public int jumpStrength = 300;

	public bool mirrorGravity = false;
	private Vector2 jumpUp;
	private Vector2 jumpDown;

	// Use this for initialization
	void Start () 
	{
		jumpUp = new Vector2 (0, jumpStrength);
		jumpDown = new Vector2 (0, -jumpStrength);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//jump logic
		if (Input.GetMouseButtonDown(0))
		{
			//jump
			if (rigidbody2D.velocity.y == 0)
			{
				rigidbody2D.AddForce(mirrorGravity ? jumpDown : jumpUp);
			}
			//invert gravity
			else
			{
				mirrorGravity = !mirrorGravity;
				rigidbody2D.gravityScale *= -1;
			}
		}

		
		//mirror object if gravity is reverted
		//transform.localScale = mirrorGravity ? new Vector2 (scale, -scale) : new Vector2 (scale, scale);
	}
}
