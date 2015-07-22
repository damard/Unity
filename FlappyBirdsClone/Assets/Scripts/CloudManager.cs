using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;

public class CloudManager
{
    private static CloudManager sInstance = new CloudManager();

    private bool mAuthenticating = false;

    public static CloudManager Instance { get { return sInstance;}}

    public bool Authenticated
    {
        get { return Social.Active.localUser.authenticated; }
    }
    public bool Authenticating
    {
        get { return mAuthenticating;}
    }

#if UNITY_ANDROID
    public void Authenticate()
    {
        if (Authenticated || mAuthenticating)   //ignores repeated call to Authenticate() while it's authenticating
        {
            return;
        }

        PlayGamesPlatform.DebugLogEnabled = Constants.debug;

        PlayGamesPlatform.Activate();
        ((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(Constants.leaderboardID);

        mAuthenticating = true;
        Social.localUser.Authenticate((bool success) => {
            mAuthenticating = false;
            if (success)
            {

            }
            else
            {
                Debug.LogWarning("Failed to sign in with Google Play Games.");
            }
        });
    }
#else
    public void Authenticate()
    {
        mAuthenticating = false;
    }
#endif

    public void SignOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }


    public void ShowLeaderboardUI()
    {
        if (Authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }
    public void ShowAchievementsUI()
    {
        if (Authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }
}
