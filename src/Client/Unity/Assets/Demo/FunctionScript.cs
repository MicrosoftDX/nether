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

        [Header("Unity")]
        [SerializeField]
        private InputField input;
        [SerializeField]
        private Text output;

        [Header("TSTableView")]
        [SerializeField]
        private TableView tableView;
        [SerializeField]
        private ScoreCell tableViewCell;
        private List<Highscore> highscores = new List<Highscore>();

        private AzureFunction service;
        private AzureFunctionClient client;

        // Use this for initialization
        void Start()
        {
            if (input == null ||
                output == null)
            {
                Debug.LogError("Inspector elements should be linked up to game objects in hierarchy.");
            }

            if (tableView == null ||
                tableViewCell == null)
            {
                Debug.LogError("TableView elements should be linked up to game objects in hierarchy.");
            }

            if (string.IsNullOrEmpty(account) ||
                string.IsNullOrEmpty(key))
            {
                Debug.LogError("Azure Function account and key required.");
            }

            client = AzureFunctionClient.Create(account, key);
            service = new AzureFunction("HelloWorld", client);

            // test data
            var score = new Highscore();
            score.Id = "ONE";
            score.Player = "unity";
            score.Score = 120;
            highscores.Add(score);

            // set TSTableView delegate
            tableView.dataSource = this;
            //tableView.ReloadData();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ClickSubmit()
        {
            string name = input.text;
            if (name.Length == 0)
            {
                Debug.LogWarning("Please enter your name...");
                return;
            }
			
            HelloWorld body = new HelloWorld(name);
            StartCoroutine(service.Post<HelloWorld,string>(body, SubmitCompleted));
        }

        private void SubmitCompleted(IRestResponse<string> response)
        {
			Debug.Log("Response: " + response.Content);
			output.text = response.Content;
        }

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
            Highscore data = highscores[row];
            cell.Name.text = data.Player;
            cell.Score.text = data.Score.ToString();
            cell.Rank.text = (row + 1).ToString();
            return cell;
        }

        #endregion
    }

}