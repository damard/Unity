using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour {

	[Range(1,100)]
	public float iconSize = 20;

	//private bool isFlipped = false;

	// Use this for initialization
	void Start () {
		SetScale (iconSize);
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*float ry = this.transform.rotation.eulerAngles.y;

		if (ry > 0 && ry < 180)
		{
			if (!isFlipped)
			{
				SetScale (-iconSize);
				isFlipped = true;
			}
		}
		else 
		{
			if (isFlipped)
			{
				SetScale (iconSize);
				isFlipped = false;
			}
		}*/
	}

	public void SetScale (float scale)
	{
		Transform tm = this.transform.FindChild("IconPlane").transform;
		tm.localScale = new Vector3(Mathf.Abs (scale), Mathf.Abs (scale), scale);
	}
	
}