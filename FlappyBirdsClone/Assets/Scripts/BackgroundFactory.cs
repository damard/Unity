using UnityEngine;

public class BackgroundFactory : MonoBehaviour
{
	public GameObject mountain;
	
	// Use this for initialization
	void Start()
	{
		InvokeRepeating("CreateBackground", 1f, 2f);
	}
	
	void CreateBackground()
	{ 
		Instantiate(mountain);
	}
}