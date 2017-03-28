using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Include Facebook namespace
using Facebook.Unity;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Globalization;
using NetherSDK;
using UnityEngine.UI;
using NetherSDK.Models;

public class UIScript : MonoBehaviour
{

	public Text StatusText;

	// Awake function from Unity's MonoBehavior
	void Awake ()
	{
		//swagger https://netherwebjppy3olypaayo.azurewebsites.net/api/swagger/ui/index.html
		//string url = "https://netherwebjppy3olypaayo.azurewebsites.net";

		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
		}


	}

	private void InitCallback ()
	{
		var perms = new List<string> () { "public_profile", "email", "user_friends" };
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp ();
			// Continue with Facebook SDK
			// ...
			FB.LogInWithReadPermissions (perms, AuthCallback);
		} else {
			Debug.Log ("Failed to Initialize the Facebook SDK");
		}
	}



	private void AuthCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);
			// Print current access token's granted permissions
			//foreach (string perm in aToken.Permissions)
			//{
			//    Debug.Log(perm);
			//}
			NetherClient.Instance.GetAccessToken (aToken.TokenString, s => {
				Debug.Log ("Got token " + s);
			});

		} else {
			Debug.Log ("User cancelled login");
		}
	}




	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void PostScoreAction ()
	{
		NetherClient.Instance.PostScore (new Score () { country = "Greece", score = 50 }, result => {
			if (result.Status == CallBackResult.Success) {
				Debug.Log ("SUCCESS posting score");
				StatusText.text = "SUCCESS posting score";
			} else {
				Debug.Log (result.Exception.Message);
				StatusText.text = result.Exception.Message;
				if (result.NetherError != null)
					Debug.Log (result.NetherError.ToString ());
			}
		});
	}


	public void GetPlayerAction ()
	{
		NetherClient.Instance.GetPlayer (result => {
			if (result.Status == CallBackResult.Success) {
				Debug.Log (result.Result.gamertag.ToString ());
				StatusText.text = result.Result.gamertag.ToString ();
			} else {
				Debug.Log (result.Exception.Message);
				StatusText.text = result.Exception.Message;
			}
		});
	}

	public void PutPlayerAction ()
	{
		NetherClient.Instance.PutPlayer (new Player () {
			gamertag = "dgkanatsios",
			country = "Greece",
			customTag = "lala"
		}, result => {
			if (result.Status == CallBackResult.Success) {
				Debug.Log ("SUCCESS putting player");
				StatusText.text = "SUCCESS putting player";
			} else {
				Debug.Log (result.Exception.Message);
				StatusText.text = result.Exception.Message;
				if (result.NetherError != null)
					Debug.Log (result.NetherError.ToString ());
			}
		});
	}

	public void GetLeaderboardsAction ()
	{
		NetherClient.Instance.GetLeaderboards (result => {
			if (result.Result != null && result.Result.Length > 0) {
				foreach (var item in result.Result) {
					Debug.Log (item.name + " " + item._link);
				}
			} else
				Debug.Log ("no leaderboards available");
		});
	}



}


