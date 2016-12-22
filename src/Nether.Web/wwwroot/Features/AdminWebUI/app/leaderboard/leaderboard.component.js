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
var nether_api_1 = require("./../nether.api");
var LeaderboardComponent = (function () {
    function LeaderboardComponent(_api) {
        this._api = _api;
        this.scoreToPost = 0;
        this.gamertagToPost = null;
        this.countryToPost = "UK";
        this.customTagToPost = null;
    }
    LeaderboardComponent.prototype.ngOnInit = function () {
        this.refreshLeaderboard();
    };
    LeaderboardComponent.prototype.refreshLeaderboard = function () {
        var _this = this;
        this._api.getLeaderboard("default")
            .subscribe(function (s) { return _this.defaultScores = s; });
        this._api.getLeaderboard("top")
            .subscribe(function (s) { return _this.topScores = s; });
        this._api.getLeaderboard("aroundMe")
            .subscribe(function (s) { return _this.aroundMeScores = s; });
    };
    LeaderboardComponent.prototype.clearScore = function () {
        this.scoreToPost = 0;
        this.gamertagToPost = null;
        this.countryToPost = "UK";
        this.customTagToPost = null;
    };
    LeaderboardComponent.prototype.postScore = function () {
        var _this = this;
        this._api.postScore(this.scoreToPost, this.countryToPost, this.customTagToPost)
            .subscribe(function (r) {
            _this.clearScore();
            _this.refreshLeaderboard();
        });
    };
    return LeaderboardComponent;
}());
LeaderboardComponent = __decorate([
    core_1.Component({
        templateUrl: "app/leaderboard/leaderboard.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService])
], LeaderboardComponent);
exports.LeaderboardComponent = LeaderboardComponent;
//# sourceMappingURL=leaderboard.component.js.map