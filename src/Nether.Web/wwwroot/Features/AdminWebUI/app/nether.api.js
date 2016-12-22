"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var ng2_cookies_1 = require("ng2-cookies/ng2-cookies");
require("rxjs/add/operator/catch");
require("rxjs/add/operator/do");
require("rxjs/add/operator/map");
require("rxjs/add/operator/mergeMap");
require("rxjs/add/observable/of");
var NetherApiService = (function () {
    function NetherApiService(_http) {
        this._http = _http;
        this._serverUrl = "http://localhost:5000/";
        this.authCacheKey = "tempAuth";
        this._clientId = "resourceowner-test";
        this._clientSecret = "devsecret";
        var authCookie = ng2_cookies_1.Cookie.get(this.authCacheKey);
        if (authCookie) {
            this._token = JSON.parse(authCookie);
            this.cachePlayer();
        }
    }
    NetherApiService.prototype.login = function (username, password) {
        var _this = this;
        return this._http.get(this._serverUrl + ".well-known/openid-configuration")
            .map(function (response) { return response.json(); })
            .flatMap(function (config) {
            _this._endpointConfig = config;
            console.log("token endpoint: " + config.token_endpoint);
            var authHead = btoa(_this._clientId + ":" + _this._clientSecret);
            var formData = "grant_type=password&username=" + username + "&password=" + password + "&scope=nether-all";
            return _this._http.post(config.token_endpoint, formData, new http_1.RequestOptions({
                headers: new http_1.Headers({
                    "Authorization": "Basic " + authHead,
                    "Content-Type": "application/x-www-form-urlencoded"
                })
            }))
                .map(function (response) {
                var token = response.json();
                console.log(token);
                _this._token = token;
                // cache token
                ng2_cookies_1.Cookie.set(_this.authCacheKey, JSON.stringify(token));
                _this.cachePlayer();
                return token.access_token;
            });
        });
    };
    NetherApiService.prototype.getCurrentPlayer = function () {
        return this._http.get(this._serverUrl + "api/player", this.getRequestOptions())
            .map(function (response) { return response.json().player; });
    };
    NetherApiService.prototype.getPlayer = function (gamertag) {
        return this._http.get(this._serverUrl + "api/players/" + gamertag, this.getRequestOptions())
            .map(function (r) { return r.json().player; });
    };
    NetherApiService.prototype.getAllPlayers = function () {
        return this._http.get(this._serverUrl + "api/players", this.getRequestOptions())
            .map(function (response) { return response.json().players; });
    };
    NetherApiService.prototype.getLeaderboard = function (type) {
        return this._http.get(this._serverUrl + "api/leaderboard/" + type, this.getRequestOptions())
            .map(function (response) { return response.json().entries; });
    };
    NetherApiService.prototype.postScore = function (score, country, customTag, gamerTag) {
        return this._http.post(this._serverUrl + "api/leaderboard", {
            score: score,
            country: country,
            customTag: customTag
        }, this.getRequestOptions());
    };
    NetherApiService.prototype.createPlayer = function (player) {
        return this._http.post(this._serverUrl + "api/players", player, this.getRequestOptions());
    };
    NetherApiService.prototype.updatePlayer = function (player) {
        return this._http.post(this._serverUrl + "api/players", player, this.getRequestOptions());
    };
    NetherApiService.prototype.getAllGroups = function () {
        return this._http.get(this._serverUrl + "api/groups", this.getRequestOptions())
            .map(function (r) { return r.json().groups; });
    };
    NetherApiService.prototype.createGroup = function (group) {
        return this._http.post(this._serverUrl + "api/groups", group, this.getRequestOptions());
    };
    NetherApiService.prototype.cachePlayer = function () {
        var _this = this;
        this.getCurrentPlayer().subscribe(function (p) { return _this._currentPlayer = p; });
    };
    NetherApiService.prototype.getRequestOptions = function () {
        return new http_1.RequestOptions({
            headers: new http_1.Headers({
                "Authorization": this._token.token_type + " " + this._token.access_token
            })
        });
    };
    return NetherApiService;
}());
NetherApiService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], NetherApiService);
exports.NetherApiService = NetherApiService;
// subset of endpoint configuraiton
var EndpointConfiguration = (function () {
    function EndpointConfiguration() {
    }
    return EndpointConfiguration;
}());
// subset of token response
var TokenResponse = (function () {
    function TokenResponse() {
    }
    return TokenResponse;
}());
//# sourceMappingURL=nether.api.js.map