using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public void LoadLevel(string levelName) {
		Debug.Log("Level load requested for: " + levelName);
		Application.LoadLevel(levelName);
	}
	
	public void QuitGame() {
		Debug.Log("Requested level quit");
	}
}
