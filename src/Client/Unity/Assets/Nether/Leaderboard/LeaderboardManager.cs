using Azure.Functions;
using RestClient;
using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nether {
  public class LeaderboardManager : MonoBehaviour, ITableViewDataSource {
    // Delegates - broadcast when async tasks are complete
    public delegate void LoadLeaderboardSuccess(LeaderboardItem[] scores);
    public delegate void LoadLeaderboardFail();
    public static event LoadLeaderboardSuccess OnLoadLeaderboardSuccess;
    public static event LoadLeaderboardFail OnLoadLeaderboardFail;

    public delegate void SubmitScoreSuccess();
    public delegate void SubmitScoreFail();
    public static event SubmitScoreSuccess OnSubmitScoreSuccess;
    public static event SubmitScoreFail OnSubmitScoreFail;

    [Header("Azure Functions")]
    [SerializeField]
    private string account;
    [SerializeField]
    private string key;

    [Space(10)]
    [SerializeField]
    private string leaderboardFunction = "Leaderboard";
    [SerializeField]
    private string scoreFunction = "Score";

    [Header("Config")]
    [SerializeField]
    private string leaderboard = "Global";

    [Header("Unity UI")]
    [SerializeField]
    private Canvas canvas;

    [Header("TSTableView")]

    [SerializeField]
    private TableView tableViewPrefab;
    [SerializeField]
    private ScoreCell tableViewCellPrefab;
    private TableView tableView;
    private List<LeaderboardItem> highscores = new List<LeaderboardItem>();

    private AzureFunctionClient client;
    private AzureFunction leaderboardService;
    private AzureFunction scoreService;

    private IEnumerator coroutine;

    private static string kTAG = "Nether";

    // Use this for initialization
    void Start() {
      if (canvas == null) {
        Debug.unityLogger.LogError(kTAG, "Attach 'canvas' property to root Canvas game object in hierarchy.");
        return;
      }

      if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(key)) {
        Debug.unityLogger.LogError(kTAG, "Azure Function account and key required.");
        return;
      }

      if (tableViewPrefab == null || tableViewCellPrefab == null) {
        Debug.unityLogger.LogError(kTAG, "To use the table view the associated prefabs must be attached.");
        return;
      }

      client = AzureFunctionClient.Create(account, key);
      leaderboardService = new AzureFunction(leaderboardFunction, client);
      scoreService = new AzureFunction(scoreFunction, client);
    }

    // Update is called once per frame
    void Update() {

    }

    #region Show/hide leaderboard

    public bool IsLeaderboardOpen() {
      if (tableView != null && tableView.enabled) {
        return true;
      }
      return false;
    }

    public void ToggleLeaderboard() {
      if (tableView == null) {
        CreateLeaderboard();
        LoadLeaderboard();
      } else {
        RemoveLeaderboard();
      }
    }

    private void CreateLeaderboard() {
      if (tableView == null) {
        tableView = Instantiate<TableView>(tableViewPrefab, canvas.transform);
        tableView.dataSource = this;
        // move to back in Canvas
        tableView.transform.SetAsFirstSibling();
      }
    }

    private void RemoveLeaderboard() {
      if (tableView != null) {
        tableView.enabled = false;
        // cancel async tasks associated with Unity game object to be destroyed
        StopCoroutine(coroutine);
        Destroy(tableView.gameObject);
      }
    }

    #endregion

    #region Load highscores

    public void LoadLeaderboard() {
      coroutine = leaderboardService.Get<LeaderboardItem>(leaderboard, LoadLeaderboardCompleted);
      StartCoroutine(coroutine);
    }

    private void LoadLeaderboardCompleted(IRestResponse<LeaderboardItem[]> response) {
      if (response.IsError) {
        Debug.unityLogger.LogError(kTAG, "Error loading leaderboard. " + response.ErrorMessage);
        OnLoadLeaderboardFail();
        return;
      }
      highscores = new List<LeaderboardItem>(response.Data);
      Debug.unityLogger.Log("Successfully loaded leaderboard.");
      if (tableView != null) {
        tableView.ReloadData();
      }
      OnLoadLeaderboardSuccess(highscores.ToArray());
    }

    #endregion

    #region Submit new highscore

    public void SubmitScore(string player, double score) {
      ScoreItem body = new ScoreItem();
      body.player = player;
      body.score = score;
      body.playerId = player.ToLower();
      body.leaderboard = leaderboard;
      StartCoroutine(scoreService.Post<ScoreItem, string>(body, SubmitScoreCompleted));
    }

    private void SubmitScoreCompleted(IRestResponse<string> response) {
      if (response.IsError) {
        Debug.unityLogger.LogError(kTAG, "Error sending score. " + response.ErrorMessage);
        OnSubmitScoreFail();
        return;
      }
      Debug.unityLogger.Log("Successfully submitted score: \n" + response.Content);
      OnSubmitScoreSuccess();
      if (tableView != null) {
        LoadLeaderboard();
      }
    }

    #endregion

    #region TSTableView methods

    public int GetNumberOfRowsForTableView(TableView tableView) {
      return highscores.Count;
    }

    public float GetHeightForRowInTableView(TableView tableView, int row) {
      return (tableViewCellPrefab.transform as RectTransform).rect.height;
    }

    public TableViewCell GetCellForRowInTableView(TableView tableView, int row) {
      ScoreCell cell = tableView.GetReusableCell(tableViewCellPrefab.reuseIdentifier) as ScoreCell;
      if (cell == null) {
        cell = (ScoreCell)GameObject.Instantiate(tableViewCellPrefab);
      }
      LeaderboardItem data = highscores[row];
      cell.Name.text = data.player;
      cell.Score.text = data.score.ToString();
      cell.Rank.text = data.rank.ToString();
      return cell;
    }

    #endregion
  }
}
