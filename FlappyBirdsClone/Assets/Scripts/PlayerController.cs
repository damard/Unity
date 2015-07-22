using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class PlayerController : MonoBehaviour {

	/// <summary>
	/// The force which is added when the player jumps.
	/// </summary>
	public Vector2 jumpForce = new Vector2 (0, 300);
	public GameObject scoreObject;
	public GameObject explosion;

	private int score;
	private Text scoreUI;
	private bool isAlive = true;
	private int explosionForce = 500;
	private int PlayerID;

	void Start ()
	{
		score = 0;
		scoreUI = scoreObject.GetComponent<Text> ();
		UpdateScore ();
	}
	// Update is called once per frame
	void Update () 
	{
		if (isAlive) 
		{
			//Go back to main menu if esc pressed
			if (Input.GetKey (KeyCode.Escape)) 
			{
				Application.LoadLevel (0);
			}

			// Jump
			if (Input.GetMouseButtonDown (0)) 
			{
				ApplyForce(jumpForce);
				audio.Stop();
				audio.Play();
			}

			//adjust angle to vertical 
			//transform.rotation.eulerAngles.Set(0, 0, rigidbody2D.velocity.y);
			float newAngle = 30 * rigidbody2D.velocity.y / 6;
			transform.eulerAngles = new Vector3 (0, 0, newAngle);

			// Die off-screen
			Vector2 screenPos = Camera.main.WorldToScreenPoint (transform.position);
			if (screenPos.y > Screen.height || screenPos.y < 0) 
			{
				Die ();
			}
		}
	}

	void ApplyForce (Vector2 force)
	{
		Debug.Log (force);
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.AddForce (force);
	}
	/// <summary>
	/// Die by Collision
	/// </summary>
	/// <param name="other">Other.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		// if environment die
		isAlive = false;

		Vector2 cp = other.contacts[0].point;
		Vector2 cn = other.contacts[0].normal;
		GameObject go = Instantiate(explosion) as GameObject;
		go.transform.position = cp;
		go.rigidbody2D.velocity = new Vector2(-4,0);
		ApplyForce (cn * explosionForce);

		StartCoroutine("DelayedDeath");
	}
	void OnTriggerExit2D()
	{
		score++;
		UpdateScore();
	}
	void UpdateScore()
	{
		scoreUI.text = score.ToString();
	}
	/// <summary>
	/// Die function reloads the level upon death
	/// </summary>
	IEnumerator DelayedDeath()
	{
		yield return new WaitForSeconds(1);
		Die ();
	}
	void Die()
	{
		if (Social.localUser.authenticated) 
		{
            Social.ReportScore (score, "CgkI-6LulqYHEAIQAQ", (bool success) => {
					//handle success or failure!
			});
			//Social.ShowLeaderboardUI ();
			//Application.LoadLevel (0);
		} 

		Application.LoadLevel(0); //return to main menu for now!
	}
}
