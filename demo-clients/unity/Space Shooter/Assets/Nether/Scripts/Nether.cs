using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Globalization;

public class Nether : MonoBehaviour
{
    public string analyticsBaseUrl;
    public string gamerTag;
    public int renewAuthNMinutesAheadOfTime;
    public int sendEventsEveryNSeconds;

    private NetherAnalyticsClient analyticsClient;
    private DateTime lastEventSentAt;

    // Use this for initialization
    void Start()
    {
        analyticsClient = new NetherAnalyticsClient("anonymous", analyticsBaseUrl, TimeSpan.FromMinutes(renewAuthNMinutesAheadOfTime));
        SendSessionStarted();
    }

    // Update is called once per frame
    void Update()
    {
        if ((DateTime.Now - lastEventSentAt).TotalSeconds > sendEventsEveryNSeconds)
            SendHeartbeat();
    }

    private void SendSessionStarted()
    {
        SendGameEvent(new SessionStartedGameEvent(gamerTag));
    }

    private void SendSessionEnded()
    {
        SendGameEvent(new SessionEndedGameEvent(gamerTag));
    }

    private void SendHeartbeat()
    {
        SendGameEvent(new HeartBeatGameEvent(gamerTag));
    }

    private void SendScoreAchieved(int score)
    {
        SendGameEvent(new ScoreAchieved(gamerTag, score));
    }

    private void SendGameEvent(GameEvent e)
    {
        lastEventSentAt = DateTime.Now;
        StartCoroutine(analyticsClient.SendGameEvent(e));
    }

}



public class NetherAnalyticsClient
{
    private string _gamerTag;
    private string _analyticsBaseUrl;
    private TimeSpan _renewAuthorizationAheadOfTime;

    // Game Events
    private string _verb;
    private string _url;
    private string _contentType;
    private string _authorization;
    private DateTime _validUntil;

    public NetherAnalyticsClient(string gamerTag, string analyticsBaseUrl, TimeSpan renewAuthorizationAheadOfTime)
    {
        _gamerTag = gamerTag;

        if (!analyticsBaseUrl.EndsWith("/"))
            analyticsBaseUrl += "/";

        _analyticsBaseUrl = analyticsBaseUrl;
        
        _renewAuthorizationAheadOfTime = renewAuthorizationAheadOfTime;
    }

    private IEnumerator GetEndpointInfo()
    {
        var request = UnityWebRequest.Get(_analyticsBaseUrl + "endpoint");

        yield return request.Send();

        if (request.isError)
            Debug.LogError("Error occurred while retrieving Nether Analytics Endpoint Information\n" + request.error);

        var json = request.downloadHandler.text;
        var info = JsonUtility.FromJson<AnalyticsEndpointInfo>(json);

        if (info.httpVerb.ToLower() != "post")
            throw new NotImplementedException("NetherAnalyticsClient doesn't support other HTTP Verbs than POST at this time");
        if (info.contentType.ToLower() != "application/json")
            throw new NotImplementedException("NetherAnalyticsClient doesn't support other ContentTypes than application/json at this time");

        _verb = info.httpVerb;
        _url = info.url;
        _contentType = info.contentType;
        _authorization = info.authorization;
        _validUntil = DateTime.Parse(info.validUntilUtc, null, DateTimeStyles.None);

        Debug.Log("NetherAnalyticsClient configured with...\n" +
            "verb: " + _verb + "\n" +
            "url: " + _url + "\n" +
            "contentType: " + _contentType + "\n" +
            "authorization: " + _authorization + "\n" +
            "validUntil: " + _validUntil.ToString(CultureInfo.InvariantCulture)
        );

    }

    public IEnumerator SendGameEvent(GameEvent gameEvent)
    {
        var json = JsonUtility.ToJson(gameEvent);

        if (DateTime.Now - _validUntil > _renewAuthorizationAheadOfTime)
            yield return GetEndpointInfo();
        

        var body = System.Text.Encoding.UTF8.GetBytes(json); 
        
        var uploader = new UploadHandlerRaw(body);
        uploader.contentType = _contentType;

        var wr = new UnityWebRequest(_url);
        wr.method = "POST";
        wr.SetRequestHeader("Authorization", _authorization);
        wr.uploadHandler = uploader;

        // var wr = UnityWebRequest.Post(_url, json);
        // wr.SetRequestHeader("Content-Type", _contentType);
        // wr.SetRequestHeader("Authorization", _authorization);

        yield return wr.Send();

        if (wr.isError)
            Debug.LogError("Error occurred while sending game event to " + _url);
        else
            Debug.Log("Game event sent to " + _url + "\n" + json);
    }
}

[Serializable]
public class AnalyticsEndpointInfo
{
    public string httpVerb;
    public string url;
    public string contentType;
    public string authorization;
    public string validUntilUtc;
}

[Serializable]
public abstract class GameEvent
{
    public string eventType;
    public string version;
    public string utcDateTime;
    public string gamerTag;

    public GameEvent(string gamerTag)
    {
        this.utcDateTime = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        this.gamerTag = gamerTag;
    }
}

[Serializable]
public class HeartBeatGameEvent : GameEvent
{
    public HeartBeatGameEvent(string gamerTag) : base(gamerTag)
    {
        this.version = "1.0.0";
        this.eventType = "HeartBeat";
    }
}

[Serializable]
public class SessionStartedGameEvent : GameEvent
{
    public SessionStartedGameEvent(string gamerTag) : base(gamerTag)
    {
        this.version = "1.0.0";
        this.eventType = "SessionStarted";
    }
}

[Serializable]
public class SessionEndedGameEvent : GameEvent
{
    public SessionEndedGameEvent(string gamerTag) : base(gamerTag)
    {
        this.version = "1.0.0";
        this.eventType = "SessionEnded";
    }
}

[Serializable]
public class ScoreAchieved : GameEvent
{
    public int score;
    public ScoreAchieved(string gamerTag, int score) : base(gamerTag)
    {
        this.version = "1.0.0";
        this.eventType = "ScoreAchieved";
        this.score = score;
    }
}