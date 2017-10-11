// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Configuration;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string DB = ConfigurationManager.AppSettings["DB"];    
    string COLLECTION = ConfigurationManager.AppSettings["COLLECTION"];
    string ENDPOINT = ConfigurationManager.AppSettings["ENDPOINT"];
    string KEY = ConfigurationManager.AppSettings["KEY"];
    
    dynamic data = await req.Content.ReadAsAsync<object>();
    string id = data?.playerId;
    string player = data?.player;
    string leaderboard = data?.leaderboard;

    if (string.IsNullOrEmpty(id) || data?.score == null || string.IsNullOrEmpty(player) || string.IsNullOrEmpty(leaderboard))
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an playerId, player(name), leaderboard(name) and score in the request body");
    
    ScoreItem postedScore = new ScoreItem();
    postedScore.PlayerId = data?.playerId;
    postedScore.Leaderboard = data?.leaderboard;
    postedScore.Player = data?.player;
    postedScore.Score = data?.score;

    DocumentClient client = new DocumentClient(new Uri(ENDPOINT), KEY);
    try
    {
        IQueryable<ScoreItem> scoreItems = client.CreateDocumentQuery<ScoreItem>(UriFactory.CreateDocumentCollectionUri(DB, COLLECTION), new FeedOptions { EnableCrossPartitionQuery = true });
        ScoreItem existingPlayer = new ScoreItem();
        var query  = 
        (
            from s in scoreItems  
            where s.PlayerId == postedScore.PlayerId         
            select s
            );
        var result = query.ToList<ScoreItem>();
        if (result.Count()>1)
        existingPlayer = query.ToList<ScoreItem>().First();

        if (string.IsNullOrEmpty(existingPlayer.PlayerId))
            {                        
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DB, COLLECTION), postedScore);
                return req.CreateResponse(HttpStatusCode.OK, $"The following user with score data was created successfully: id:{postedScore.PlayerId}, Name:{postedScore.Player}, Score:{postedScore.Score.ToString()}");
            }
        else
            {                
                postedScore.Id = existingPlayer.Id;
                if (postedScore.Score>existingPlayer.Score)
                {
                    existingPlayer.Score = postedScore.Score;
                    await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DB, COLLECTION, existingPlayer.Id), postedScore);
                    return req.CreateResponse(HttpStatusCode.OK, $"The score  for user {existingPlayer.PlayerId} has been updated to {postedScore.Score}");
                } 
                else
                {
                return req.CreateResponse(HttpStatusCode.OK, $"The score  for user {existingPlayer.PlayerId} is not higher than existing {postedScore.Score}");
                }                
            };
    }
    catch (DocumentClientException ex)
    {
            return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an playerId, player(name), leaderboard(name) and score in the request body");
    }
}

public class ScoreItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set;}    
    [JsonProperty(PropertyName = "leaderboard")]
    public string Leaderboard { get; set;}    
    [JsonProperty(PropertyName = "player")]
    public string Player { get; set;}
    [JsonProperty(PropertyName = "playerId")]
    public string PlayerId { get; set;}
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set;}
}
