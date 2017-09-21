using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Azure.Functions;
using RestClient;
using Tacticsoft;
using System;

namespace Nether
{
    public class FunctionScript : MonoBehaviour, ITableViewDataSource
    {

        [Header("Azure Functions")]
        [SerializeField]
        private string account;
        [SerializeField]
        private string key;

        [Space(10)]
        [SerializeField]
        private string scoreFunction = "Score";
        [SerializeField]
        private string leaderboardFunction = "TopNLeaderboard";

        [Header("Nether")]
        [SerializeField]
        private string leaderboard = "Global";

        [Header("Unity")]
        [SerializeField]
        private InputField inputPlayer;
        [SerializeField]
        private InputField inputScore;
        [SerializeField]
        private Text output;

        [Header("TSTableView")]
        [SerializeField]
        private TableView tableView;
        [SerializeField]
        private ScoreCell tableViewCell;
        private List<LeaderboardItem> highscores = new List<LeaderboardItem>();

        private AzureFunctionClient client;
        private AzureFunction scoreService;
        private AzureFunction leaderboardService;

        // Use this for initialization
        void Start()
        {
            if (inputPlayer == null ||
                inputScore == null ||
                output == null)
            {
                Debug.LogError("Inspector elements should be linked up to game objects in hierarchy.");
                return;
            }

            if (tableView == null ||
                tableViewCell == null)
            {
                Debug.LogError("TableView elements should be linked up to game objects in hierarchy.");
                return;
            }

            if (string.IsNullOrEmpty(account) ||
                string.IsNullOrEmpty(key))
            {
                Debug.LogError("Azure Function account and key required.");
                return;
            }

            client = AzureFunctionClient.Create(account, key);
            scoreService = new AzureFunction(scoreFunction, client);
            leaderboardService = new AzureFunction(leaderboardFunction, client);

            // set TSTableView delegate
            tableView.dataSource = this;

            // load leaderboard top scores
            LoadLeaderboard();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Load highscores

        public void ClickRefresh()
        {
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            StartCoroutine(leaderboardService.Get<LeaderboardItem>(LoadCompleted));
        }

        private void LoadCompleted(IRestResponse<LeaderboardItem[]> response)
        {
            if (response.IsError)
            {
                Debug.LogError("Opps, something went wrong. " + response.ErrorMessage);
                return;
            }
            highscores = new List<LeaderboardItem>( response.Data );
            tableView.ReloadData();
        }

        #endregion

        #region Submit new highscore

        public void ClickSubmit()
        {
            if (string.IsNullOrEmpty(inputPlayer.text))
            {
                Debug.LogWarning("Please enter a player name...");
                return;
            }
            if (string.IsNullOrEmpty(inputScore.text))
            {
                Debug.LogWarning("Please enter a score...");
                return;
            }

            double score = 0;
            Double.TryParse(inputScore.text, out score);

            ScoreItem body = new ScoreItem();
            body.player = inputPlayer.text;
            body.score = score;
            body.playerId = inputPlayer.text.ToLower();
            body.leaderboard = leaderboard;
            StartCoroutine(scoreService.Post<ScoreItem, string>(body, SubmitCompleted));
        }

        private void SubmitCompleted(IRestResponse<string> response)
        {
            if (response.IsError)
            {
                Debug.LogError("Opps, something went wrong. " + response.ErrorMessage);
                return;
            }
            Debug.Log("Response: " + response.Content);
			output.text = response.Content;
        }

        #endregion

        #region TSTableView methods

        public int GetNumberOfRowsForTableView(TableView tableView)
        {
            return highscores.Count;
        }

        public float GetHeightForRowInTableView(TableView tableView, int row)
        {
            return (tableViewCell.transform as RectTransform).rect.height;
        }

        public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
        {
            ScoreCell cell = tableView.GetReusableCell(tableViewCell.reuseIdentifier) as ScoreCell;
            if (cell == null)
            {
                cell = (ScoreCell)GameObject.Instantiate(tableViewCell);
            }
            LeaderboardItem data = highscores[row];
            cell.Name.text = data.player;
            cell.Score.text = data.score.ToString();
            cell.Rank.text = (row + 1).ToString();
            return cell;
        }

        #endregion
    }

}