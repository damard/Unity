using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	static MusicPlayer instance = null;

	// Use this for initialization
	void A () {

		if (instance != null) {
			Destroy(gameObject);
			Debug.Log("Duplicate music player; Self-Destructing");
		} else {
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);

			AudioSource audioSource = GetComponent<AudioSource>();
			audioSource.Play();
		}
	}
}
