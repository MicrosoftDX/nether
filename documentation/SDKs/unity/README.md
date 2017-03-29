# Unity SDK for Nether

## Work in Progress 
There is currently a beta SDK of Unity SDK for Nether that you can find in the src folder.

## Downloading the Unity SDK for Nether
Nether Unity SDK is open source and freely downloadable from Unity Asset Store. Current code is built with 5.5.2 but should work on all 5.x editions of Unity. Nether SDK for Unity has no dependencies on other packages, apart from potential libraries from authentication providers (e.g. Facebook).

### Installing the Unity SDK for Nether
Installation is simple. You download the package from the Unity Asset Store and extract all files to your project. You can find a scene named "test" and a "UIScript" script that contain some example code that you can use to bootstrap your project. You also need to download [Facebook SDK for Unity](https://developers.facebook.com/docs/unity/) for the sample to work.

### Making Unity SDK for Nether work with your project

1. Drag the prefab called "NetherClient" into the scene. This contains a singleton class for easy and reliable access
2. Set your Nether deployment endpoint URL on the prefab's properties. Also set clientID, clientSecret and scope
3. Configure authentication in your game (e.g. Facebook). You may need to download external library for this purpose, e.g. you can find the Facebook SDK for Unity [here](https://developers.facebook.com/docs/unity/)
4. You're done! Now you can safely call Nether methods

### Sample pics

#### NetherClient prefab
![NetherClient prefab](/documentation/SDKs/unity/images/1.JPG?raw=true)

#### Set Nether options
![Set Nether options](/documentation/SDKs/unity/images/2.JPG?raw=true)

### Usage

Generally, Nether API interaction methods have the signature **NetherSDK.Instance.DoSomething(instanceForPutOrPost, callbackWithResultOrError)**. The callback carries an instance of CallbackResponse<T> which you can inspect to see the results of your Nether API call. Make sure you check the Status property to determine if the call was successful or determine the cause of error.

#### Get Nether token

After you get the token from the authentication provider of your choice, you need to get a Nether token. For example, if you are using Facebok authentication, you could do the following

```csharp
if (FB.IsLoggedIn)
        {
            //get access token
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            //token is automatically saved in NetherClient.Instance
            NetherClient.Instance.GetAccessToken(aToken.TokenString, s =>
            {
                Debug.Log("Got token " + s); 
            });

        }
```

Token is saved in the **NetherClient.Instance** instance, and you can then safely call implemented class methods.

#### Post a score

You can use the **NetherClient.Instance.PostScore** method to post a Score to Nether

```csharp
public void PostScoreAction()
    {
        NetherClient.Instance.PostScore(new Score() { country = "Greece", score = 50 }, result =>
        {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log("SUCCESS posting score");
            }
            else
            {
                StatusText.text = result.Exception.Message;
                if (result.NetherError != null)
                    Debug.Log(result.NetherError.ToString());
            }
        });
    }
```

#### Get Player

You can use the **NetherClient.Instance.GetPlayer** method to get player's details

```csharp
NetherClient.Instance.GetPlayer (result => {
			if (result.Status == CallBackResult.Success) {
				Debug.Log (result.Result.gamertag.ToString ());
				StatusText.text = result.Result.gamertag.ToString ();
			} else {
				Debug.Log (result.Exception.Message);
				StatusText.text = result.Exception.Message;
			}
		});
```

#### Put Player

You can use the **NetherClient.Instance.PutPlayer** method to update player's details

```csharp
NetherClient.Instance.PutPlayer (new Player () {
			gamertag = "dgkanatsios",
			country = "Greece",
			customTag = "myCustomTag"
		}, result => {
			if (result.Status == CallBackResult.Success) {
				Debug.Log ("SUCCESS putting player");
				StatusText.text = "SUCCESS putting player";
			} else {
				Debug.Log (result.Exception.Message);
				StatusText.text = result.Exception.Message;
				if (result.NetherError != null)
					Debug.Log (result.NetherError.ToString ());
			}
		});
```

#### Get Leaderboards 

You can use the **NetherClient.Instance.GetLeaderboards** method to get leaderboards

```csharp
NetherClient.Instance.GetLeaderboards (result => {
			if (result.Result != null && result.Result.Length > 0) {
				foreach (var item in result.Result) {
					Debug.Log (JsonUtility.ToJson(item));
				}
			} else
				Debug.Log ("no leaderboards available");
		});
```

#### Get named Leaderboard 

After you call the GetLeaderboards method, you can use the **NetherClient.Instance.GetLeaderboardNamed** method to get a specific leaderboard

```csharp
 NetherClient.Instance.GetLeaderboardNamed("Default", result => {
            if (result.Result != null )
            {
                Debug.Log(JsonUtility.ToJson(result.Result));
            }
            else
                Debug.Log("no leaderboard available with that name");
        });
```

#### Post Data

You can use the **NetherClient.Instance.PostData** method to post custom data. This posts the data to Nether deployment's Event Hub instance.

```csharp
NetherClient.Instance.PostData(new DeviceCapabilities() { cpu = "ARM", ram = "2 GB" }, result => {
            if (result.Status == CallBackResult.Success)
            {
                Debug.Log("SUCCESS posting to event hub");
                StatusText.text = "SUCCESS posting to event hub";
            }
            else
            {
                Debug.Log(result.Exception.Message);
                StatusText.text = result.Exception.Message;
                if (result.NetherError != null)
                    Debug.Log(result.NetherError.ToString());
            }
        });
```


#### HeartBeat

Nether SDK for Unity supports a periodic sending of "heartbeat" data to Nether backend using Unity's [InvokeRepeating](https://docs.unity3d.com/ScriptReference/MonoBehaviour.InvokeRepeating.html) method. In the following code we call the **NetherClient.Instance.StartHearBeat** method which will send an instance of CustomGameInfo to Nether backend every 2 seconds. GPS data is also updated every second.

```csharp
public class NetherHeartbeat : MonoBehaviour {

    public Text StatusText;
    public CustomGameInfo gameInfo;

    // Use this for initialization
    void Start () {
        Debug.Log("Enabling GPS");
        Input.location.Start(); //start GPS
        this.InvokeRepeating("GetInfoFromGPS", 1.0f, 1.0f);

        NetherClient.Instance.StartHeartbeat(2.0f, 2.0f, gameInfo);
    }

    public void GetInfoFromGPS()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            gameInfo.latitude = Input.location.lastData.latitude;
            gameInfo.longitude = Input.location.lastData.longitude;
            gameInfo.timestamp = Input.location.lastData.timestamp;
        }
    }

    [Serializable]
    public class CustomGameInfo
    {
        public float latitude;
        public float longitude;
        public double timestamp;
    }
}
```