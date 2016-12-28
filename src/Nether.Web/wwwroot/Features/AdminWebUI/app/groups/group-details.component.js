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
var GroupDetailsComponent = (function () {
    function GroupDetailsComponent(_api, _route, _router) {
        this._api = _api;
        this._route = _route;
        this._router = _router;
        this.groupNameParam = "name";
    }
    GroupDetailsComponent.prototype.ngOnInit = function () {
        var _this = this;
        this._route.params
            .switchMap(function (params) {
            _this.name = params[_this.groupNameParam];
            console.log("loading group '" + _this.name + "'");
            return _this._api.getGroup(_this.name);
        })
            .subscribe(function (group) {
            console.log("group loaded");
            _this.group = group;
        });
    };
    return GroupDetailsComponent;
}());
GroupDetailsComponent = __decorate([
    core_1.Component({
        templateUrl: "app/groups/group-details.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService, router_1.ActivatedRoute, router_1.Router])
], GroupDetailsComponent);
exports.GroupDetailsComponent = GroupDetailsComponent;
//# sourceMappingURL=group-details.component.js.map