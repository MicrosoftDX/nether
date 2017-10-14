// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Configuration;

// Read application settings
private static string db = ConfigurationManager.AppSettings["COSMOSDB_DBNAME"];    
private static string endpoint = ConfigurationManager.AppSettings["COSMOSDB_ENDPOINT"];
private static string key = ConfigurationManager.AppSettings["COSMOSDB_PRIMARY_MASTER_KEY"];

private const string collection = "scores";
private static bool runOnce = true;
private static DocumentClient client;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // Run initialization only once.
    // Remarks: This initialization will run once on every instance and on every recompile of this function
    if (runOnce)
    {
        log.Info("Running initialization");

        // Check required application settings
        if (string.IsNullOrWhiteSpace(db)) log.Error("COSMOSDB_DBNAME settings wasn't provided");
        if (string.IsNullOrWhiteSpace(collection)) log.Error("COSMOSDB_COLLECTION settings wasn't provided");
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

    dynamic data = await req.Content.ReadAsAsync<object>();
    string id = data?.playerId;
    string player = data?.player;

    if (string.IsNullOrEmpty(id) || data?.score == null || string.IsNullOrEmpty(player))
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an playerId, player(name) and score in the request body");
    
    var postedScore = new ScoreItem();
    postedScore.PlayerId = data?.playerId;
    postedScore.Player = data?.player;
    postedScore.Score = data?.score;

    try
    {
        var scoreItems = client.CreateDocumentQuery<ScoreItem>(UriFactory.CreateDocumentCollectionUri(db, collection), new FeedOptions { EnableCrossPartitionQuery = true });
        var existingPlayer = new ScoreItem();
        var query  = 
            from s in scoreItems  
            where s.PlayerId == postedScore.PlayerId         
            select s;

        var result = query.ToList<ScoreItem>();
        if (result.Count()>1)
            existingPlayer = query.ToList<ScoreItem>().First();

        if (string.IsNullOrEmpty(existingPlayer.PlayerId))
            {                        
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(db, collection), postedScore);
                return req.CreateResponse(HttpStatusCode.OK, $"The following user with score data was created successfully: id:{postedScore.PlayerId}, Name:{postedScore.Player}, Score:{postedScore.Score.ToString()}");
            }
        else
            {                
                postedScore.Id = existingPlayer.Id;
                if (postedScore.Score>existingPlayer.Score)
                {
                    existingPlayer.Score = postedScore.Score;
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(db, collection, existingPlayer.Id), postedScore);
                    return req.CreateResponse(HttpStatusCode.OK, $"The score  for user {existingPlayer.PlayerId} has been updated to {postedScore.Score}");
                } 
                else
                {
                return req.CreateResponse(HttpStatusCode.OK, $"The score  for user {existingPlayer.PlayerId} is not higher than existing {postedScore.Score}");
                }                
            };
    }
    catch (DocumentClientException)
    {
            return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an playerId, player(name) and score in the request body");
    }
}

public class ScoreItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set;}    
    [JsonProperty(PropertyName = "player")]
    public string Player { get; set;}
    [JsonProperty(PropertyName = "playerId")]
    public string PlayerId { get; set;}
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set;}
}