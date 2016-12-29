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
var GroupsComponent = (function () {
    function GroupsComponent(_api, _router) {
        this._api = _api;
        this._router = _router;
        this.resetGroup();
    }
    GroupsComponent.prototype.ngOnInit = function () {
        this.refreshGroups();
    };
    GroupsComponent.prototype.refreshGroups = function () {
        var _this = this;
        console.log("refreshing groups...");
        this._api.getAllGroups().subscribe(function (all) {
            console.log("groups refreshed");
            _this.allGroups = all;
        });
    };
    GroupsComponent.prototype.createGroup = function () {
        var _this = this;
        console.log("creating group... ");
        this._api.createGroup(this.newGroup).subscribe(function (r) {
            console.log("group created.");
            _this.refreshGroups();
            _this.resetGroup();
        });
    };
    GroupsComponent.prototype.resetGroup = function () {
        this.newGroup = new model_1.Group();
    };
    return GroupsComponent;
}());
GroupsComponent = __decorate([
    core_1.Component({
        templateUrl: "app/groups/groups.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService, router_1.Router])
], GroupsComponent);
exports.GroupsComponent = GroupsComponent;
//# sourceMappingURL=groups.component.js.map