using UnityEngine;

public class Mountain : MonoBehaviour
{
	public float maxSpeed = 3;
	public float maxScale = 3;
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();


		float speed = Random.Range (0.8f, 1.0f) * maxSpeed;
		float scale = Random.Range (0.4f, 1.0f) * maxScale;
		int mirror = Random.value >= 0.5 ? -1 : 1;
		rigidbody2D.velocity = new Vector2(-speed,0);
		transform.localScale = new Vector2 (scale * mirror, scale);

		Destroy (gameObject, 10);
	}
}