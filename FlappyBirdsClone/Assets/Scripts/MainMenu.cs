using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class MainMenu : MonoBehaviour {	

	public Button loginButton;
    public Button leaderboardButton;

    private bool mWaitingForAuth = false;
    private Text loginText;

	void Start()
	{
        GooglePlayGames.PlayGamesPlatform.Activate();
        loginText = loginButton.GetComponentInChildren<Text>();
	}
	void Update () 
	{
		if (Input.GetKey (KeyCode.Escape))
		{
			Application.Quit();
		}

		/*if (Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel (1);
		}*/
	}

	void OnGUI()
	{
		if (CloudManager.Instance.Authenticated)
        {
            loginText.text = "Sign Out " + Social.localUser.userName;
            leaderboardButton.gameObject.SetActive(true);
        }
        else
        {
            loginText.text = "Sign In";
            leaderboardButton.gameObject.SetActive(false);
        }
	}

	public void StartGame()
	{
		Application.LoadLevel (1);
	}
	public void QuitGame()
	{
		Application.Quit ();
	}
    public void ShowLeaderboard()
    {
        CloudManager.Instance.ShowLeaderboardUI();
    }
	public void Authenticate()
	{
        CloudManager.Instance.Authenticate();
	}
}
