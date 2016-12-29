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
var router_1 = require("@angular/router");
require("rxjs/add/operator/switchMap");
var PlayerDetailsComponent = (function () {
    function PlayerDetailsComponent(_api, _route, _router) {
        this._api = _api;
        this._route = _route;
        this._router = _router;
        this.playerTagParam = "tag";
        this.gamertag = null;
        this.player = null;
        this.playerGroups = null;
        this.allGroups = null;
    }
    PlayerDetailsComponent.prototype.ngOnInit = function () {
        var _this = this;
        // call web service to get player frout the gamertag specified in the route
        this._route.params
            .switchMap(function (params) {
            _this.gamertag = params[_this.playerTagParam];
            console.log("loading player details, gamertag: " + _this.gamertag);
            _this.reloadGroups();
            return _this._api.getPlayer(_this.gamertag);
        })
            .subscribe(function (player) {
            console.log("player loaded");
            _this.player = player;
            console.log(player);
        });
    };
    PlayerDetailsComponent.prototype.updatePlayer = function () {
        var _this = this;
        console.log("updating...");
        this._api.updatePlayer(this.player)
            .subscribe(function (r) {
            console.log("player updated, going back...");
            _this._router.navigate(["players"]);
        });
    };
    PlayerDetailsComponent.prototype.reloadGroups = function () {
        var _this = this;
        this._api.getPlayerGroups(this.gamertag).subscribe(function (gs) { return _this.playerGroups = gs; });
        this._api.getAllGroups().subscribe(function (ag) { return _this.allGroups = ag; });
    };
    PlayerDetailsComponent.prototype.toggleGroupMembership = function (g) {
        var _this = this;
        var isMember = this.isMemberOf(g);
        console.log("switching '" + g.name + "' " + isMember + "=>" + !isMember);
        if (!isMember) {
            this._api.addPlayerToGroup(this.player.gamertag, g.name)
                .subscribe(function (r) { return _this.playerGroups.push(g); });
        }
        else {
            this._api.removePlayerFromGroup(this.player.gamertag, g.name)
                .subscribe(function (r) { return _this.playerGroups = _this.playerGroups.filter(function (g) { return g.name !== g.name; }); });
        }
    };
    PlayerDetailsComponent.prototype.isMemberOf = function (g) {
        return this.playerGroups.some(function (i) { return i.name === g.name; });
    };
    return PlayerDetailsComponent;
}());
PlayerDetailsComponent = __decorate([
    core_1.Component({
        templateUrl: "app/players/player-details.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService, typeof (_a = typeof router_1.ActivatedRoute !== "undefined" && router_1.ActivatedRoute) === "function" && _a || Object, typeof (_b = typeof router_1.Router !== "undefined" && router_1.Router) === "function" && _b || Object])
], PlayerDetailsComponent);
exports.PlayerDetailsComponent = PlayerDetailsComponent;
var _a, _b;
//# sourceMappingURL=player-details.component.js.map