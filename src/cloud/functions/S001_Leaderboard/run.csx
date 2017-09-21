    using System.Net;
    using System.Linq;
    using System.Configuration;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;

    public static HttpResponseMessage Run(HttpRequestMessage req, TraceWriter log)
    {
        string DB = ConfigurationManager.AppSettings["DB"];    
        string COLLECTION = ConfigurationManager.AppSettings["COLLECTION"];
        string ENDPOINT = ConfigurationManager.AppSettings["ENDPOINT"];
        string KEY = ConfigurationManager.AppSettings["KEY"];
        const int LENGTH_OF_LEADERBOARD = 10;

        var client = new DocumentClient(new Uri(ENDPOINT), KEY);

        try
        {
            var collection = client.CreateDocumentQuery<ScoreItem>(
                UriFactory.CreateDocumentCollectionUri(DB, COLLECTION), new FeedOptions { EnableCrossPartitionQuery = true });
            
            var query = 
                (from s in collection
                orderby s.Score descending
                select new LeaderboardItem {Player = s.Player, Score = s.Score}).Take(LENGTH_OF_LEADERBOARD);

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
