#JavaScript SDK for Nether

## Work in Progress

## Downloading the JavaScript SDK for Nether

### Installing the JavaScript SDK for Nether

## Usage
To use the nether SDK include the nether script in your project. Reference the script in the webpage using 
```html
<script src="/scripts/nether.js"></script>
```
```javascript
var config = {
        netherClientId: '<client Id>',
        netherClientSecret: '<client secret>',
        facebookAppId: '<facebook app Id>',
        netherBaseUrl: '<nether url>'
    }
    nether.init(config, facebookInitailised, netherInitialised);
```
To initialise Nether use the ***nether.init*** method passing the Nether configuration parameters and the callbacks. When nether is initialised, Nether checks to see if the user is logged into facebook and if so proceeds to authenticate the user with Nether.
nether.init provides two callbacks: -
### facebookInitialised(loggedin)
The facebookInitialised callback provides you with information about whether the user is logged into facebook by parsing a boolean value.
#### Example usage
```html
    <script>
    function facebookInitialised(loggedin) {
        if (connected === true) {
            console.log('The user is logged into facebook')
        } else {
            console.log('The user is not logged into facebook');
        }
    }
    </script>
``` 
### netherInitialised(loggedin)
The netherInitialised callback provides you with information about whether the user is logged into nether. 
#### Example usage
```html
<script>
    function netherInitialised(loggedin) {
        if (connected === true) {
            console.log('The user is logged into nether')
        } else {
            console.log('The user is not logged into nether');
        }
    }
    </script>
```
### Analytics
The analytics methods in Nether can be used to send analytics events. Nether supports the following methods: -
#### Count 
You can use the ***nether.analytics.count(name, value, args)*** method to send a count measurement to analytics.
##### Example usage
```javascript
var args = {}
args.data = 'my own value';

nether.analytics.count('shots', 10, args);
```
***name*** The name parameter is used to as a display name for the event.
***value*** The count value for the event.
***args.data*** The properties for the event.

#### Session Start
You can use the ***nether.analytics.sessionStart(name, Id)*** method to send session start events.
##### Example usage
```javascript
nether.analytics.sessionStart('session started', 'de346ea2-4e59-49d8-863b-e0b2823b4ebd');
```
***name*** The name parameter is used to as a display name for the event.
***id*** The event correlation Id

#### Session Stop
You can use the ***nether.analytics.sessionStop(Id)*** method to send session stopped events.
##### Example usage
```javascript
nether.analytics.sessionStop('de346ea2-4e59-49d8-863b-e0b2823b4ebd');
```
***id*** The event correlation Id. This should be the same as the session start Id.

#### Game Start
You can use the ***nether.analytics.gameStart*** method to send game start events.
##### Example usage
```javascript
nether.analytics.gameStart();
```
The game session Id and the gamer tag are logged as parameters when a game start event is created.

#### Game Stop
You can use the ***nether.analytics.gameStop*** method to send game stop events.
##### Example usage
```javascript
nether.analytics.gameStop();
```
The game session Id is logged as a parameter when a game stop event is created.

#### Level Start
You can use the ***nether.analytics.levelStart(level)*** method to send level start events.
##### Example usage
```javascript
nether.analytics.levelStart(1);
```
***level*** The level Id or the level name.
The game session Id is logged as a parameter when a level start event is created.

#### Level Finished
You can use the ***nether.analytics.levelFinished(level)*** method to send level finished events.
##### Example usage
```javascript
nether.analytics.levelFinished(1);
```
***level*** The level Id or the level name.
The game session Id is logged as a parameter when a level finished event is created.

#### Custom event
You can use the ***nether.analytics.customEven(event)*** method to create custom events.
##### Example usage
```javascript
var event = {}
event.name = 'example event';
event.value = 10;
event.id = 'de346ea2-4e59-49d8-863b-e0b2823b4ebd';
nether.analytics.customEvent(event)
```
### Identity


### Leaderboard

#### Get Leaderboard
You can use the ***nether.leaderboard.getAllLeaderboards(callback)*** method to get the currently configured Leaderboard.
##### Example usage
```javascript
showLeaderboards = function(leaderboards) {
    for (let leaderboard of leaderboards) {
        var leaderboardName = leaderboard.name;
        var leaderboardUrl = leaderboard._link;
    }
}
nether.leaderboard.getAllLeaderboards(showLeaderboards);

```
### Get Leaderboard by name
You can use the ***nether.leaderboard.getLeaderboard(leaderboardname)*** method to get all of the gamer tags and highscores for the specified Leaderboard.
#### Example usage
```javascript
showLeaderboard = function(leaderboard) {
    for (let entry of leaderboard.entries) {
        var gamertag = entry.gamertag;
        var isCurrentPlayer = entry.isCurrentPlayer;
        var rank = entry.rank;
        var score = entry.score;
    }
    var currentPlayerGamerTag = leaderboard.currentPlayer.gamertag;
    var currentPlayerRank = leaderboard.currentPlayer.rank;
    var currentPlayerScore = leaderboard.currentPlayer.score;
}
nether.leaderboard.getLeaderboard('default', showLeaderboard);

```

### Put score
You can use the ***nether.score.addScore(score, callback)*** method to add a players score.
#### Example usage
```javascript
scoreAdded = function(scoreAdded) {
    if (scoreAdded === false) 
        console.log('Unable to add score');
}
nether.scores.addScore(100, scoreAdded);
```

### Drop scores
You can use the ***nether.score.dropMyScore(callback)*** method to delete all scores for the logged in player.
#### Example usage
```javascript
scoreDropped = function(scoreDropped) {
    if (scoreDropped === false) 
        console.log('Unable to drop score');
}
nether.scores.dropMyScore(scoreDropped);
```
### Player Management

### Get the current Player
You can use the ***nether.player.getPlayer(callback)*** method to get the current player details.
#### Example usage
```javascript
showCurrentPlayer = function(player) {
    console.log(player.gamertag);
    console.log(player.country);
    console.log(player.customTag);
}
nether.player.getPlayer(showCurrentPlayer);
```
### Put Player
You can use the ***nether.player.setPlayer(country, gamerTag, customTag, callback)*** method to update Players details.
#### Example usage
```javascript
playerSet = function() {
    console.log('player updated');
}
nether.player.setPlayer('uk', 'testplayer' 'this player is great', playerSet);
```

### Delete player
You can use the ***nether.player.deleteCurrentPlayer(callback)*** method to delete the current player.
#### Example usage
```javascript
playerDeleted = function(deleted) {
    if (deleted === true)
        console.log('player deleted');
}
nether.player.deletePlayer(playerDeleted);
```

### Get player state
You can use the ***nether.player.getState(callback)*** method to get the state of the current player.
#### Example usage
```javascript
showState = function(gamerTag, state) {
    console.log(gamerTag);
    console.log(state);
}
nether.player.getState(showState);
```

### Set player state
You can use the ***nether.player.setState(state, callback)*** method to update the current players player state.
#### Example usage
```javascript
checkState = function(state) {
    console.log(state);
}
nether.player.setState('state', checkState);
```

## Player Identity

### Facebook login
You can use the ***nether.player.identity.facebookLogin(callback)*** method to invoke a facebook login. 
#### Example usage
```html
<input type="button" onclick="loginUser()" value="Login to facebook" />
<script>
checkLogin = function(status) {
    if (status === true) 
        console.log('User logged into facebook');
}
loginUser = function() {
    nether.player.Identity.facebookLogin(checkLogin);
}
</script>
```

### authWithFacebookToken
You can use the ***nether.player.identity.authWithFacebookToken(callback)*** method to authenticate a user logged into facebook with Nether.
The user must be logged into facebook first before calling this method.
#### Example usage
```javascript
checkAuth = function(status) {
    if (status === true) 
        console.log('User authenticated with nether');
}
nether.player.identity.authWithFacebookToken(checkAuth);
```

