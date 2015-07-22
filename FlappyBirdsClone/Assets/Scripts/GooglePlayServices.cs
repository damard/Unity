using UnityEngine;
//using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public enum SocialStates
{
    LoggedOut,
    LoggedIn,
    Authenticating,
    SigningOut,
    AuthenticationError
}

public class GooglePlayServices : MonoBehaviour {

    private bool debug = true;
	// Use this for initialization
	void Start () {
        GooglePlayGames.PlayGamesPlatform.Activate();
	}
	
	// Update is called once per frame
	void OnGUI () {
        if (debug && Social.localUser.authenticated)
        {
            
        }
	}
}
