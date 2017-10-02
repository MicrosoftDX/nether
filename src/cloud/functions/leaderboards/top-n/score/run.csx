using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Configuration;

// Read application settings
private static string db = ConfigurationManager.AppSettings["DOCUMENTDB_DATABASE"];    
private static string collection = ConfigurationManager.AppSettings["DOCUMENTDB_COLLECTION"];
private static string endpoint = ConfigurationManager.AppSettings["DOCUMENTDB_ENDPOINT"];
private static string key = ConfigurationManager.AppSettings["DOCUMENTDB_PRIMARY_KEY"];

private static bool runOnce = true;
private static DocumentClient client;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // Run initialization only once.
    // Remarks: This initialization will run once on every instance and on every reset of the app
    if (runOnce)
    {
        log.Info("Running initialization");

        // Check required application settings
        if (string.IsNullOrWhiteSpace(db)) log.Error("DOCUMENTDB_DATABASE settings wasn't provided");
        if (string.IsNullOrWhiteSpace(collection)) log.Error("DOCUMENTDB_COLLECTION settings wasn't provided");
        if (string.IsNullOrWhiteSpace(endpoint)) log.Error("DOCUMENTDB_ENDPOINT settings wasn't provided");
        if (string.IsNullOrWhiteSpace(key)) log.Error("DOCUMENTDB_PRIMARY_KEY settings wasn't provided");

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
    string leaderboard = data?.leaderboard;

    if (string.IsNullOrEmpty(id) || data?.score == null || string.IsNullOrEmpty(player) || string.IsNullOrEmpty(leaderboard))
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass an playerId, player(name), leaderboard(name) and score in the request body");
    
    var postedScore = new ScoreItem();
    postedScore.PlayerId = data?.playerId;
    postedScore.Leaderboard = data?.leaderboard;
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