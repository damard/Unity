using UnityEngine;
using System.Collections;

public class NumberWizard : MonoBehaviour {
	
	int max, min, guess;

	// Use this for initialization
	void Start () {
		StartGame();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)){
			//print ("Up arrow pressed");
			min = guess + 1;
			NextGuess();
		} else if (Input.GetKeyDown (KeyCode.DownArrow)){
			print ("Down arrow pressed");
			max = guess - 1;
			NextGuess();
		} else if (Input.GetKeyDown(KeyCode.Return)) {
			print ("I won!");
			StartGame();
		}
	}

	void StartGame () {
		max = 1000;
		min = 0;
		
		print ("========================================");
		print ("Welcome to Number Wizard");
		print ("Pick a number in your head, but don't tell me!");
		
		print ("The highest number you can pick is " + max);
		print ("The lowest number you can pick is " + min);
		
		NextGuess();
	}

	void NextGuess(){
		guess = (max + min) / 2;
		print ("Is the number higher or lower than " + guess + "?");
		print ("Up arrow for higher, Down for lower, Return if equal");
	}
}
