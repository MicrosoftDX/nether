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
var model_1 = require("./../model");
var router_1 = require("@angular/router");
var PlayersComponent = (function () {
    function PlayersComponent(_api, _router) {
        this._api = _api;
        this._router = _router;
        this.resetPlayer();
    }
    PlayersComponent.prototype.ngOnInit = function () {
        this.refreshPlayers();
    };
    PlayersComponent.prototype.refreshPlayers = function () {
        var _this = this;
        this._api.getAllPlayers().subscribe(function (all) { return _this.allPlayers = all; });
    };
    PlayersComponent.prototype.createPlayer = function () {
        var _this = this;
        this._api.createPlayer(this.newPlayer).subscribe(function (r) {
            _this.refreshPlayers();
            _this.resetPlayer();
        });
    };
    PlayersComponent.prototype.selectPlayer = function (player) {
        console.log("navigating to player " + player.gamertag);
        this._router.navigate(["player", player.gamertag]);
    };
    PlayersComponent.prototype.resetPlayer = function () {
        this.newPlayer = new model_1.Player();
    };
    return PlayersComponent;
}());
PlayersComponent = __decorate([
    core_1.Component({
        templateUrl: "app/players/players.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService, typeof (_a = typeof router_1.Router !== "undefined" && router_1.Router) === "function" && _a || Object])
], PlayersComponent);
exports.PlayersComponent = PlayersComponent;
var _a;
//# sourceMappingURL=players.component.js.map