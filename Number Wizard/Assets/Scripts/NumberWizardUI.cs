using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NumberWizardUI : MonoBehaviour {
	
	int max = 1000;
	int min = 0;
	int maxGuessesAllowed = 10;
	int guess;
	
	public Text text;
	public Text attemptLeft;
	
	// Use this for initialization
	void Start () {
		StartGame();
	}
	
	public void GuessHigher() {
		min = Mathf.Clamp(guess + 1, min, max);
		MakeGuess();
	}
	
	public void CorrectGuess() {
		Application.LoadLevel("Lose");
	}
	
	public void GuessLower() {
		max = Mathf.Clamp(guess - 1, min, max);
		MakeGuess();
	}
	
	void StartGame () {
		MakeGuess();
	}
	
	void MakeGuess(){
		guess = Random.Range(min, max); //(max + min) / 2;
		text.text = guess.ToString();
		
		//Debug.Log("Min = " + min + "; Max = " + max);
		
		maxGuessesAllowed -= 1;
		
		attemptLeft.text = maxGuessesAllowed.ToString();
		if (maxGuessesAllowed <= 0) {
			Application.LoadLevel("Win");
		}
	}
}
