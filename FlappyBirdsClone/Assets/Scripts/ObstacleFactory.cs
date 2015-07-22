using UnityEngine;

public class ObstacleFactory : MonoBehaviour
{
	public GameObject rocks;

	// Use this for initialization
	void Start()
	{
		InvokeRepeating("CreateObstacle", 0.5f, 1.5f);
	}

	void CreateObstacle()
	{ 
		Instantiate(rocks);
	}
}