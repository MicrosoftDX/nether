using System.Configuration;
using System.Linq;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

// Read application settings
private static string db = ConfigurationManager.AppSettings["DOCUMENTDB_DATABASE"];    
private static string collection = ConfigurationManager.AppSettings["DOCUMENTDB_COLLECTION"];
private static string endpoint = ConfigurationManager.AppSettings["DOCUMENTDB_ENDPOINT"];
private static string key = ConfigurationManager.AppSettings["DOCUMENTDB_PRIMARY_KEY"];
private static int lengthOfLeaderboard = int.Parse(ConfigurationManager.AppSettings["LENGTH_OF_LEADERBOARD"] ?? "10");

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

    try
    {
        var leaderboard = client.CreateDocumentQuery<ScoreItem>(
            UriFactory.CreateDocumentCollectionUri(db, collection), new FeedOptions { EnableCrossPartitionQuery = true });
        
        var query = 
            (from s in leaderboard
            orderby s.Score descending
            select new LeaderboardItem {Player = s.Player, Score = s.Score}).Take(lengthOfLeaderboard);

        var leaders = query.ToList();

        for (int i=0; i < leaders.Count; i++)
        {
            leaders[i].Rank = i + 1;
        }

        return req.CreateResponse(HttpStatusCode.OK, leaders);

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

public class LeaderboardItem
{
    [JsonProperty(PropertyName = "rank")]
    public int Rank { get; set; }
    [JsonProperty(PropertyName = "player")]
    public string Player { get; set; }
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set; }
}
