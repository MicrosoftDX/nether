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

        globalRank = query.Count<ScoreItem>() + 1;

        List<LeaderboardItem> sortedResult = new List<LeaderboardItem>();

        // Get and add to output specific number of players (radiusOfLeaderboard constant) with scores below the current player)
        query =
            (from s in scores
             where s.Score < currentPlayer.Score && s.Leaderboard == leaderboard
             orderby s.Score descending
             select s).Take(radiusOfLeaderboard);

        var sortedList = query.ToList<ScoreItem>().OrderBy(a => a.Score);

        foreach (ScoreItem scr in sortedList)
        {
            sortedResult.Add(new LeaderboardItem { Rank = 0, Player = scr.Player, Score = scr.Score });
        }
        // Add current player to output
        sortedResult.Add(new LeaderboardItem { Rank = globalRank, Player = currentPlayer.Player, Score = currentPlayer.Score });

        // Get and add to output specific number of players (radiusOfLeaderboard constant) with scores above the current player)
        query =
            (from s in scores
             where s.Score > currentPlayer.Score && s.Leaderboard == leaderboard
             orderby s.Score ascending
             select s).Take(radiusOfLeaderboard);

        foreach (ScoreItem scr in query)
        {
            sortedResult.Add(new LeaderboardItem { Rank = 0, Player = scr.Player, Score = scr.Score });
        }

        //Update the ranks in the output with global positions
        for (int i = 0; i < sortedResult.Count; i++)
        {
            sortedResult[i].Rank = globalRank - radiusOfLeaderboard + i;
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
