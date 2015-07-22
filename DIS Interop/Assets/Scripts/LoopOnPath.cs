using UnityEngine;
using System.Collections;

public class LoopOnPath : MonoBehaviour 
{

	public string pathName;
	public int loopTime;

	// Use this for initialization
	void Start () 
	{	
		iTween.MoveTo(this.gameObject, iTween.Hash("path", iTweenPath.GetPath(pathName),
		                                           "time", loopTime,
		                                           "orientToPath", true,
		                                           "lookAhead", 0.01,
		                                           "lookTime", 0.2,
		                                           "loopType", "loop",
		                                           "delay", 0,
		                                           "easeType", "linear"));
	}
}
