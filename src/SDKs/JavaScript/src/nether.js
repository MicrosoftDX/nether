/* TODO: Review callbacks for improvements
         Review code for any further issues
         Check if identity/users, identity/users/{userid} methods are required
*/
var nether = (function () {
    var nether = {
        netherBaseUrl: ''
    };

    // Initialise nether
    nether.init = function(config, facebookCallback, netherCallback, netherHost) {
        nether.player.identity.netherClientId = config.netherClientId;
        nether.player.identity.netherClientSecret = config.netherClientSecret;
        nether.player.identity.facebookAppId = config.facebookAppId;
        nether.netherBaseUrl = config.netherBaseUrl;

        if (typeof FB === "undefined") {
            // Load the facebook SDK asynchronously and then initialise nether
            (function(d, s, id) {
                var js, fjs = d.getElementsByTagName(s)[0];
                if (d.getElementById(id)) return;
                js = d.createElement(s); js.id = id;
                js.src = '//connect.facebook.net/en_US/sdk.js';
                fjs.parentNode.insertBefore(js, fjs);
            }(netherHost? netherHost : document, 'script', 'facebook-jssdk'));

            fbAsyncInit = function() {
                FB.init({
                    appId: nether.player.identity.facebookAppId,
                    cookie: true,
                    xfbml: true,
                    version: 'v2.8'
                });

                nether.analytics.init();
                nether.player.identity.init(facebookCallback, netherCallback);
            }
        }
        else {
            fbAsyncInit = function() {
                FB.init({
                    appId: nether.player.identity.facebookAppId,
                    cookie: true,
                    xfbml: true,
                    version: 'v2.8'
                });

                nether.analytics.init();
                nether.player.identity.init(facebookCallback, netherCallback);
            }
        }
    };
    
    return nether;
}());

nether.common = (function() {
    var common = {};

    common.ajax = function(args) {
        
        var xhr = new XMLHttpRequest();
        xhr.open(args.method || 'GET', args.url);

        if (args.json === true)
            xhr.setRequestHeader('Content-type', 'application/json');

        xhr.onload = function() {
            if (args.callback)
                args.callback(xhr.status, xhr.responseText);
        };

        if (args.authorise){
            xhr.setRequestHeader('Authorization', 'Bearer ' + nether.player.identity.accessToken);
        }

        if (args.headers) {
            for(var key in args.headers) {
                xhr.setRequestHeader(key, args.headers[key]);
            }
        }

        var data = args.data;

        if (data) {
            if (typeof(data) !== 'string')
                data = JSON.stringify(data);

            xhr.send(data);
        } else {
            xhr.send();
        }
    };

    common.send = function() {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', this.analyticsUrl, true);
        xhr.setRequestHeader('Content-Type', 'application/json;charset=utf-8');
        xhr.setRequestHeader('Authorization', this.analyticsAuth);
        xhr.send(JSON.stringify(data));
    };

    return common;
}());

nether.analytics = (function () {
    var analytics = {
        analyticsUrl:'',
        analyticsAuth:'',
        version: '1.0.0'
    };

    analytics.init = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/endpoint',
            method: 'GET',
            headers: {
                'Content-type': 'application/json'
            },
            callback: function(status, res) {
                var data;

                if (status === 200) {
                    try {
                        data = JSON.parse(res);
                        analytics.analyticsUrl = data.url;
                        analytics.analyticsAuth = data.authorization
                    } catch(ex) { }
                    if (callback !== undefined)
                    {
                        callback(null);
                    }
                    
                }
                else if (callback !== undefined)
                {
                    callback(new Error('Failed to get analytics endpoint: ' + status + ': ' + res));
                }
            }
        });
    };

    analytics.count = function (name, value, args) {
        var data = {
            type: 'count',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: args && args.related || nether.player.gameSessionId,
            displayName: name,
            value: value || 1,
            properties: args && args.data || { }
        };

        this.send(data);
    };
    
    analytics.sessionStart = function(name, id) {
        var data = {
            type: 'start',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            eventCorrelationId: id,
            displayName: name,
            properties: { }
        };

        this.send(data);
    };

    analytics.sessionStop =  function(id) {
        var data = {
            type: 'stop',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            eventCorrelationId: id,
            properties: { }
        };

        this.send(data);
    };
    
    analytics.gameStart = function() {
        this.gameSessionId = '';

        // generate random id
        for(var i = 0; i < 16; i++)
            this.gameSessionId += ('00' + Math.floor(Math.random() * 16).toString(16)).slice(-2);

        var data = {
            type: 'game-start',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: nether.player.gameSessionId,
            gamertag: nether.player.gamertag,
            properties: { }
        };

        this.send(data);
    };

    analytics.gameStop = function() {
        var data = {
            type: 'game-stop',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: nether.player.gameSessionId,
            properties: { }
        };

        this.send(data);
    };

    analytics.levelStart = function(level) {
        var data = {
            type: 'level-start',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: nether.player.gameSessionId,
            level: level,
            properties: { }
        };

        this.send(data);
    };

    analytics.levelFinished = function(level) {
        var data = {
            type: 'level-finished',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: nether.player.gameSessionId,
            level: level,
            properties: { }
        };

        this.send(data);
    };

    analytics.customEvent = function(event) {
        var data = {
            type: 'custom-event',
            version: analytics.value,
            clientUtcTime: new Date().toISOString(),
            gameSessionId: nether.player.gameSessionId,
            event: event,
            properties: { }
        };

        this.send(data);
    };

    return analytics;

}());

nether.leaderboard = (function () {
	var leaderboard = {};
	
    leaderboard.getAllLeaderboards = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/leaderboards',
            authorise: true,
            callback: function(status, res) {
                var data;

                if (status === 200) {
                    data = JSON.parse(res);
                    callback(status, data.leaderboards);
                }
                else {
                    callback(status, res);
                }
            }
        });
    };

    leaderboard.getLeaderboard = function(leaderboard, callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/leaderboards/' + leaderboard,
            authorise: true,
            callback: function(status, res) {
                var data;

                if (status === 200) {
                    data = JSON.parse(res);
                    callback(status, data);
                }
                else {
                    callback(status, res);
                }
            }
        });
    };

	return leaderboard;
}());

nether.scores = (function() {
    var scores = {};

    scores.addScore = function (score, callback) {
        // Check score is a number
        if (isNaN(score)) {
            callback(false);
            return;
        }

        // Check score is a positive number
        if (score < 0)
        {
            callback(false);
            return;
        }

        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/scores',
            method: 'POST',
            json: true,
            authorise: true,
            data: {
                country: nether.player.country,
                score: score
            },
            callback: function(status) {
                if (status === 200) {
                    callback(true);
                } else {
                    callback(false);
                }
            }
        });
    };

    scores.dropMyScore = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/scores',
            method: 'DELETE',
            authorise: true,
            callback: function(status) {
                if (status === 204) {
                    callback(true);
                    return;
                } else {
                    callback(false);
                }
            }
        });
    };

    return scores;
}());

nether.player = (function() {
    var player = {
        gameSessionId: '',
        gamertag: '',
        state: '',
        country: ''
    };

    player.getPlayer = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/player',
            authorise: true,
            callback: function(status, res) {
                var data;

                if (status === 200)
                    data = JSON.parse(res);

                if (data && data.player)
                    player.gamertag = data.player.gamertag || null;
                
                callback(data && data.player || null);
            }
        });
    };

    player.setPlayer = function(country, gamerTag, customTag, callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/player',
            method: 'PUT',
            json: true,
            authorise: true,
            data: {
                country: country,
                gamertag: gamerTag,
                customTag: customTag
            },
            callback: function(status, res) {
                if (status === 204) {
                    nether.player.gamertag = gamerTag;
                    nether.player.country = country;
                    callback();
                    return;
                } else if (status === 400) {
                    try {
                        resError = JSON.parse(res);
                        if (resError.error && resError.error.details && resError.error.details.length > 0 && resError.error.details[0].message){
                            callback(null, resError.error.details[0].message);
                            return;
                        }
                    } catch(ex) { }
                }
                callback(new Error('couldn\'t set player gamertag'));
            }
        });
    };

    player.deletePlayer = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/player',
            method: 'DELETE',
            authorise: true,
            callback: function(status) {
                if (status === 204) {
                    player.gameSessionId = '';
                    player.gamertag = '';
                    player.state = '';

                    callback(true);
                    return;
                } else {
                    callback(false);
                }
            }
        });
    };

    player.getState = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/player/state',
            authorise: true,
            callback: function(status, res) {
                var data;

                if (status === 200)
                    data = JSON.parse(res);

                if (data && data.gamertag)
                    player.gamertag = data.gamertag || null;

                if (data && data.state.state)
                    player.state = data.state.state || null;
                
                callback(data && data.gamertag || null, data && data.state.state || null);
            }
        });
    };

    player.setState = function(state, callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/api/player/state',
            method: 'PUT',
            json: true,
            authorise: true,
            data: {
                state: state
            },
            callback: function(status) {
                if (status === 204) {
                    callback();
                    return;
                } else if (status === 400) {
                    try {
                        resError = JSON.parse(res);
                        if (resError.error && resError.error.details && resError.error.details.length > 0 && resError.error.details[0].message){
                            callback(resError.error.details[0].message);
                            return;
                        }
                    } catch(ex) { }
                }
                callback(new Error('couldn\'t set player gamertag'));
            }
        });
    };

    // Is this required?
    player.getPlayerGroups = function() {

    };

    return player;
}());

nether.player.identity = (function() {
    var identity = {
        netherClientId: '',
        netherClientSecret: '',
        facebookAppId: '',
        facebookAccessToken: '',
        accessToken: '',
        loggedIn: false
    };

    identity.init = function(facebookCallback, netherCallback) {
        // Check to see if user is logged into facebook
        FB.getLoginStatus(function(res) {
            if (res.status === 'connected' && res.authResponse.accessToken) {
                identity.facebookAccessToken = res.authResponse.accessToken;
                identity.authWithFacebookToken(netherCallback);
            } else {
                nether.player.identity.facebookAccessToken = '';
            }
            // callback with facebook login status
            if (res.status === "connected") {
                facebookCallback(true);
            } else {
                facebookCallback(false);
            }
            
        });

        FB.AppEvents.logPageView();
    }

    // trigger logging in
    identity.facebookLogin = function(callback) {
        if (identity.facebookAccessToken !== '') {
            console.log('User already logged into facebook');
            if (identity.loggedIn === false) {
                identity.authWithFacebookToken(callback);
            }
        } else {
            // call the facebook sdk login
            FB.login(function (res) {
                if (res.status === 'connected' && res.authResponse.accessToken) {
                    identity.facebookAccessToken = res.authResponse.accessToken;
                    identity.authWithFacebookToken(callback);
                } else {
                    callback(status);
                }   
            });
        }
    }

    // exchange facebook access token to identity token
    identity.authWithFacebookToken = function(callback) {
        nether.common.ajax({
            url: nether.netherBaseUrl + '/identity/connect/token',
            method: 'POST',
            headers: {
                'Content-type': 'application/x-www-form-urlencoded'
            },
            data: 'grant_type=fb-usertoken&client_id=' + identity.netherClientId + '&client_secret=' + identity.netherClientSecret + '&scope=openid+profile+nether-all&token=' + identity.facebookAccessToken,
            callback: function(status, res) {
                var data = JSON.parse(res);
                
                if (data.access_token) {
                    identity.accessToken = data.access_token;
                    identity.loggedIn = true;
                    console.log(identity.accessToken);
                    callback(true);
                } else {
                    console.log('couldnt get the access token');
                    callback(new Error('couldn\'t get access token'));
                }
            }
        });
    }

    return identity;
}());
