using UnityEngine;
using System.Collections;

public class PlaneAnimator : MonoBehaviour {

    public Sprite[] sprites;
    public int framesPerSecond;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (spriteRenderer != null)
        {

        }
	}
}
