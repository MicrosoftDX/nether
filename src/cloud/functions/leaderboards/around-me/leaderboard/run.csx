// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Configuration;
using System.Linq;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

// Read application settings
private static string db = ConfigurationManager.AppSettings["COSMOSDB_DBNAME"];
private static string endpoint = ConfigurationManager.AppSettings["COSMOSDB_ENDPOINT"];
private static string key = ConfigurationManager.AppSettings["COSMOSDB_PRIMARY_MASTER_KEY"];
private static string baseArchitectureVersion = ConfigurationManager.AppSettings["BASE_ARCHITECTURE_VERSION"];
private const string requiredBaseArchitectureVersion = "1.1";

private const string collection = "scores";
private const int radiusOfLeaderboard = 5;
private static bool runOnce = true;
private static DocumentClient client;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string leaderboard, string playerId, TraceWriter log)
{
    // Run initialization only once.
    // Remarks: This initialization will run once on every instance and on every recompile of this function
    if (runOnce)
    {
        log.Info("Running initialization");

        if (string.IsNullOrWhiteSpace(baseArchitectureVersion) ||
            !baseArchitectureVersion.StartsWith(requiredBaseArchitectureVersion)) log.Error($"The base architecture version doesn't match the expected version {requiredBaseArchitectureVersion}");

        // Check required application settings
        if (string.IsNullOrWhiteSpace(db)) log.Error("COSMOSDB_DBNAME settings wasn't provided");
        if (string.IsNullOrWhiteSpace(endpoint)) log.Error("COSMOSDB_ENDPOINT settings wasn't provided");
        if (string.IsNullOrWhiteSpace(key)) log.Error("COSMOSDB_PRIMARY_MASTER_KEY settings wasn't provided");

        // Create Cosmos DB Client from settings
        client = new DocumentClient(new Uri(endpoint), key);

        // Create Database and Collection in Cosmos DB Account if they don't exist
        await client.CreateDatabaseIfNotExistsAsync(new Database { Id = db });
        await client.CreateDocumentCollectionIfNotExistsAsync(
            UriFactory.CreateDatabaseUri(db),
            new DocumentCollection { Id = collection });

        runOnce = false;

        log.Info("Initialization done!");
    }

    if (string.IsNullOrWhiteSpace(leaderboard) || string.IsNullOrWhiteSpace(playerId))
        return req.CreateResponse(HttpStatusCode.BadRequest, "Both playerId and leaderboard should be provided in querystring, format is leaderboard/playerId");

    try
    {

        var scores = client.CreateDocumentQuery<ScoreItem>(
            UriFactory.CreateDocumentCollectionUri(db, collection), new FeedOptions { EnableCrossPartitionQuery = true });

        // Get score for the current player by playerId        
        var query =
            (from s in scores
             where s.PlayerId == playerId && s.Leaderboard == leaderboard
             select s);

        var player = query.ToList();
        if (player.Count < 1)
            return req.CreateResponse(HttpStatusCode.BadRequest, $"Player with Id = {playerId} in Leaderboard = {leaderboard} does not exist in the database");

        if (player.Count > 1)
            return req.CreateResponse(HttpStatusCode.BadRequest, $"There are more than one player with Id = {playerId} in Leaderboard = {leaderboard}. This is DB confuguration error - this combination should be unique");

        var currentPlayer = player.First();

        // Calculate global rank for the current player by playerId 
        int globalRank;

        query = from c in scores
                where c.Score > currentPlayer.Score && c.Leaderboard == leaderboard
                select c;

        List<LeaderboardItem> sortedResult = new List<LeaderboardItem>(); //Sorted list from highest score (start of list) to lowest for output  

        globalRank = query.Count<ScoreItem>() + 1;

        if (globalRank > 1) //if current player it not number 1 in global rank
        {
            // Get and add to output specific number of players (radiusOfLeaderboard constant) with scores above the current player)
            query =
            (from s in scores
             where s.Score > currentPlayer.Score && s.Leaderboard == leaderboard
             orderby s.Score ascending
             select s).Take(radiusOfLeaderboard);

            var sortedList = query.ToList<ScoreItem>().OrderByDescending(a => a.Score); //sort list from highest score to lowest

            foreach (ScoreItem scr in sortedList)
            {
                sortedResult.Add(new LeaderboardItem { Rank = globalRank - 1, Player = scr.Player, Score = scr.Score });
            }

            for (int i = sortedResult.Count - 1; i > 0; i--) //iterate through scores above player from lowest score/position (end of array) to higher (start of array)
            {
                if (sortedResult[i].Score == sortedResult[i - 1].Score) //if current score is the same as one position higher in the list
                {
                    int sameScoreNum;
                    sameScoreNum = NumberWithSameScores(sortedResult, i); //calculate how many people have the same score
                    for (int j = 0; j < sameScoreNum; j++) //iterate through all the people with the same score
                    {
                        sortedResult[i - j].Rank = sortedResult[i].Rank - sameScoreNum + 1 + j; //assign same rank to everyone with the same score
                    }
                    if (i + 1 - sameScoreNum >= 2) // if there are 2 or more people left to iterate
                    {
                        i = i + 2 - sameScoreNum; //jump index to the last iterated with the same score in the current segment
                    }
                    else
                    {
                        if (i + 1 - sameScoreNum == 1) //if only one left 
                        {
                            sortedResult[0].Rank = sortedResult[i - 1].Rank - 1; //update the last person in the list with the right rank
                            break;
                            //i = 0;
                        }
                        else //no one left - stop iterating
                        {
                            break;
                            //i = 0;
                        }
                    }
                }
                else // scores of compared players are not the same
                {
                    if (sortedResult[i].Score < sortedResult[i - 1].Score) //double check 
                    {
                        sortedResult[i - 1].Rank = sortedResult[i].Rank - 1; //assign rank -1 to one with higher score
                    }
                    else
                    {
                        log.Info("Error. Order of elements in score list is broken");
                    }
                }
            }
        }
        int numElementHigherThanCurrent = sortedResult.Count;

        // Add current player to output
        sortedResult.Add(new LeaderboardItem { Rank = globalRank, Player = currentPlayer.Player, Score = currentPlayer.Score });

        // Get and add to output specific number of players (radiusOfLeaderboard constant) with scores below the current player)
        query =
            (from s in scores
             where s.Score <= currentPlayer.Score && s.Leaderboard == leaderboard && s.PlayerId != currentPlayer.PlayerId
             orderby s.Score descending
             select s).Take(radiusOfLeaderboard);

        var sortedList2 = query.ToList<ScoreItem>().OrderByDescending(a => a.Score);

        if (!(sortedList2 is null))
        {
            foreach (ScoreItem scr in sortedList2)
            {
                sortedResult.Add(new LeaderboardItem { Rank = 0, Player = scr.Player, Score = scr.Score });
            }
        }

        int numWithSameScore = 0;
        //Update the ranks in the output with global positions
        for (int i = numElementHigherThanCurrent + 1; i < sortedResult.Count; i++)
        {
            if (sortedResult[i].Score < sortedResult[i - 1].Score)
            {
                sortedResult[i].Rank = sortedResult[i - 1].Rank + 1 + numWithSameScore;
                numWithSameScore = 0;
            }
            else
            {
                sortedResult[i].Rank = sortedResult[i - 1].Rank;
                numWithSameScore++;
            }
        }

        return req.CreateResponse(HttpStatusCode.OK, sortedResult);


    }
    catch (DocumentClientException ex)
    {
        log.Info(ex.ToString());

        if (ex.StatusCode == HttpStatusCode.NotFound)
            return req.CreateResponse(HttpStatusCode.BadRequest, "This user does not exist in the database.");

        return req.CreateResponse(HttpStatusCode.BadRequest, $"An unknown error has occured. Message: {ex.Message}");
    }
}


public static int NumberWithSameScores(List<LeaderboardItem> listToRank, int startPosition)
{
    int numberWithSameScores = 1;
    for (int i = startPosition; i > 0; i--)
    {
        if (listToRank[i].Score == listToRank[i - 1].Score)
        {
            numberWithSameScores++;
        }
        else
        {
            break;
        }
    }
    return numberWithSameScores;
}

public class ScoreItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "leaderboard")]
    public string Leaderboard { get; set; }
    [JsonProperty(PropertyName = "player")]
    public string Player { get; set; }
    [JsonProperty(PropertyName = "playerId")]
    public string PlayerId { get; set; }
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set; }
}

public class LeaderboardItem
{
    [JsonProperty(PropertyName = "rank")]
    public int Rank { get; set; }
    [JsonProperty(PropertyName = "player")]
    public string Player { get; set; }
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set; }
}
