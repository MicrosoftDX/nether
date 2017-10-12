using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Azure.AppServices;
using Azure.Functions;
using RESTClient;
using Tacticsoft;
using Nether;

namespace NetherDemo {
  public class LeaderboardDemoScript : MonoBehaviour {
    [Header("Unity UI")]
    [SerializeField]
    private GameObject loginUI;
    [SerializeField]
    private Button logoutButton;
    [SerializeField]
    private InputField inputUserAccessToken;
    [SerializeField]
    private InputField inputUserSecret;
    [SerializeField]
    private InputField inputPlayer;

    [SerializeField]
    private InputField inputScore;
    [SerializeField]
    private Text outputText;

    [Header("Nether")]
    [SerializeField]
    private LeaderboardManager leaderboardManager;
    private AuthenticationProvider identityProvider = AuthenticationProvider.Facebook; // default to Facebook

    // Use this for initialization
    void Start() {
      if (leaderboardManager == null ||
          inputPlayer == null ||
          inputScore == null ||
          inputUserAccessToken == null ||
          inputUserSecret == null ||
          outputText == null) {
        Debug.LogError("Unity missing links to game objects in hierarchy.");
        return;
      }
    }

    // Update is called once per frame
    void Update() {

    }

    #region Demo UI

    public void ChangeIdentityProvider(Dropdown dropdown) {
      if (dropdown == null) {
        Debug.LogWarning("Unity missing link to dropdown element");
        return;
      }
      identityProvider = (AuthenticationProvider)dropdown.value;
      if (identityProvider.Equals(AuthenticationProvider.Twitter) || identityProvider.Equals(AuthenticationProvider.Google)) {
        inputUserSecret.placeholder.GetComponent<Text>().text =
          identityProvider.Equals(AuthenticationProvider.Twitter) ? "Enter access token secret..." : "Enter id token...";
        inputUserSecret.gameObject.SetActive(true);
      } else {
        inputUserSecret.gameObject.SetActive(false);
      }
    }

    public void ClickLogin() {
      string token = inputUserAccessToken.text;
      if (string.IsNullOrEmpty(token)) {
        outputText.text = "Please enter access token!";
        return;
      }
      string secret = inputUserSecret.text;
      leaderboardManager.Login(identityProvider, token, secret);
    }

    public void ClickLogout() {
      leaderboardManager.Logout();
    }

    public void ClickLeaderboard() {
      if (!leaderboardManager.IsLeaderboardOpen()) {
        outputText.text = "Loading...";
      } else {
        outputText.text = "";
      }
      leaderboardManager.ToggleLeaderboard();
    }

    public void ClickSubmit() {
      if (string.IsNullOrEmpty(inputPlayer.text)) {
        outputText.text = "Please enter a player name!";
        return;
      }
      if (string.IsNullOrEmpty(inputScore.text)) {
        outputText.text = "Please enter a score!";
        return;
      }

      double score = 0;
      Double.TryParse(inputScore.text, out score);

      outputText.text = "Submitting...";
      leaderboardManager.SubmitScore(inputPlayer.text, score);
    }

    private void ChangeLoginUI(bool isLoggedIn = false) {
      loginUI.SetActive(!isLoggedIn);
      logoutButton.gameObject.SetActive(isLoggedIn);
    }

    #endregion

    #region Event handlers (optional)

    void OnEnable() {
      LeaderboardManager.OnLoginSuccess += OnLoginSuccess;
      LeaderboardManager.OnLoginFail += OnLoginFail;
      LeaderboardManager.OnLogoutSuccess += OnLogoutSuccess;
      LeaderboardManager.OnLogoutFail += OnLogoutFail;
      LeaderboardManager.OnLoadLeaderboardSuccess += OnLoadLeaderboardSuccess;
      LeaderboardManager.OnLoadLeaderboardFail += OnLoadLeaderboardFail;
      LeaderboardManager.OnSubmitScoreSuccess += OnSubmitScoreSuccess;
      LeaderboardManager.OnSubmitScoreFail += OnSubmitScoreFail;
    }


    void OnDisable() {
      LeaderboardManager.OnLoginSuccess -= OnLoginSuccess;
      LeaderboardManager.OnLoginFail -= OnLoginFail;
      LeaderboardManager.OnLogoutSuccess -= OnLogoutSuccess;
      LeaderboardManager.OnLogoutFail -= OnLogoutFail;
      LeaderboardManager.OnLoadLeaderboardSuccess -= OnLoadLeaderboardSuccess;
      LeaderboardManager.OnLoadLeaderboardFail -= OnLoadLeaderboardFail;
      LeaderboardManager.OnSubmitScoreSuccess -= OnSubmitScoreSuccess;
      LeaderboardManager.OnSubmitScoreFail -= OnSubmitScoreFail;
    }

    private void OnLoginSuccess() {
      outputText.text = "Logged in!";
      ChangeLoginUI(true);
    }

    private void OnLoginFail() {
      outputText.text = "Failed to login";
    }

    private void OnLogoutSuccess() {
      outputText.text = "Logged out!";
      ChangeLoginUI(false);
    }

    private void OnLogoutFail() {
      outputText.text = "Failed to logout";
    }

    private void OnLoadLeaderboardSuccess(LeaderboardItem[] scores) {
      outputText.text = "Loaded " + scores.Length + " scores.";
    }

    private void OnLoadLeaderboardFail() {
      outputText.text = "Failed to load scores.";
    }

    private void OnSubmitScoreSuccess() {
      outputText.text = "Successfully submitted score.";
    }

    private void OnSubmitScoreFail() {
      outputText.text = "Failed to submit score.";
    }

    #endregion
  }
}
