# Unity SDK for Nether

## DRAFT (REALLY DRAFT) Unity implementation (or better, fiddling with API calls) of Nether SDK
Below instructions serve merely as design discussions as the SDK is still Work in Progress. Nether SDK for Unity has no dependencies on other packages.

## Downloading the Unity SDK for Nether
Nether Unity SDK is open source and freely downloadable from Unity Asset Store. Current code is built with 5.5.2 but should work on all 5.x editions of Unity.

### Installing the Unity SDK for Nether
Installations is simple. After you download the package from the Unity Asset Store

1. drag a prefab into the scene - Unity SDK for Nether has a singleton class for easy access
2. set your Nether deployment endpoint URL on the prefab's properties
3. configure authentication in your game (e.g. Facebook). You may need to download external library for this purpose, e.g. you can find the Facebook SDK for Unity [here](https://developers.facebook.com/docs/unity/)
4. You're done! Easy, right?

### Usage

After you get the token from the authentication provider of your choice, you need to get a Nether token.

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

Token is saved in the **NetherClient.Instance** instances, and you can then safely call implemented class methods.

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

Generally, Nether access methods have the signature **NetherSDK.Instance.DoSomething(instanceForPutOrPost, callbackWithResultOrError)**

### Questions/observations/things attempted
1. Tried to generate C# classes with [NSwag](https://github.com/NSwag/NSwag). Nope :) Dependency on JSON.NET, lots of MVVM code (INotifyPropertyChanged etc), Unity deserializer does not support properties but fields (!!!!!) etc.
2. Thought of messing with NSwag code to output Unity compatible classes, but abandoned the idea due to (3)
3. (Most important) Which methods are needed? Do we really need all Nether API calls available to a Unity game? Maybe we should finalize what methods are needed
4. Unity does not support generic covariance and contravariance. This is probably why this does not work: cannot convert from 'System.Action<NetherSDK.Models.CallbackResponse<NetherSDK.Models.Player>>' to 'System.Action<NetherSDK.Models.CallbackResponse>'	
5. Forced to use reflection in order to have a single method to interact with Nether API. Reflection sucks! Open to suggestions
6. UnityWebRequest requires use of StartCoroutine (works like a JS Promise) => no await/async => callbacks, pyramid of doom, etc.
7. Facebook Unity SDK for obvious reasons cannot be distributed with NetherUnitySDK
8. We need to make sure that we have the Nether token before using methods
9. Leaderboards endpoint. Maybe rename to Leaderboard? Check Leaderboards.cs :)
10. Error handling needs work
11. clientID and clientSecret. Should they remain?
12. How often should Nether token be refreshed?


### Methods we need - this serves as a design discussion
1. A method to connect to Nether. This sends the Facebook token and gets the Nether token back
2. Get Player
3. Put Player (Update)
4. Get Leaderboards
5. Post Score

### More

The game can interact with server APIs, some of them are exposed as custom WebAPIs (that we've written) and some other part of Nether are exposed as native Azure Services. For the game developer it should be seen as one... hopefully
Example:

Game communicates with API to send score
Game send telemetry about session started
 Those two HTTP POSTs are sent to two different endpoints

And there are some features that we want to automize from the client SDKs with this and that is:

1. The game client should never have to care about that we use different services in Azure. Meaning the negotiation of a signed EventHub URL should be invisible to the game client if they chose to use the client SDK
2. We want the client SDKs to send things automatically that we know are needed. For example there is one event (being sent to EventHub) that we call "Heartbeat". The Client SDK should send such an event at least ever minute.
3. Some game studios has also asked us to provide a tight interaction with the underlying operating system, providing "out of the box" analytic information about what device the game is running on. Things like available memory, screen size, operating system, etc.
Those kind of info should also be sent automatically
Off course anything that is sent automatically also need to be configurable to not being sent. We don't want anything being sent unless they want it to ;-)
