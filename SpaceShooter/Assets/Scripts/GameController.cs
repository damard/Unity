using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject hazard;
	public Vector3 spawnValues;
	public int hazardCount;
	public float startWait;
	public float spawnWait;
	public float waveWait;

	public Text scoreText;
	private int score;

	public GameObject gameOverText;
	private bool gameOver;
	private bool restart;

	void Start()
	{
		gameOver = restart = false;
		gameOverText.SetActive (gameOver);
		score = 0;
		UpdateScore ();
		StartCoroutine(SpawnWaves ());
	}

	void Update()
	{
		if (restart) 
		{
			if (Input.GetButton("Fire1"))
			{
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	IEnumerator SpawnWaves()
	{
		yield return new WaitForSeconds(startWait);
		while(true)
		{
			for (int i=0; i < hazardCount; i++)
			{
				Vector3 spawnPosition = new Vector3 (Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y , spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);

			if (gameOver)
			{
				restart = true;
				break;
			}
		}
	}

	public void AddScore (int newScore)
	{
		score += newScore;
		UpdateScore ();
	}
	public void GameOver()
	{
		gameOverText.SetActive (true);
		gameOver = true;
	}
	void UpdateScore()
	{
		gameOverText.SetActive(gameOver);
		scoreText.text = scoreText.text = "Score: " + score.ToString ();
	}
}
