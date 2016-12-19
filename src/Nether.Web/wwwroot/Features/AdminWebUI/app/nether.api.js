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
require("rxjs/add/operator/catch");
require("rxjs/add/operator/do");
require("rxjs/add/operator/map");
require("rxjs/add/observable/of");
var NetherApiService = (function () {
    function NetherApiService(_http) {
        this._http = _http;
        this._apiBase = "/api/";
        this._headers = new http_1.Headers({ "Content-Type": "application/json" });
    }
    NetherApiService.prototype.getCurrentPlayer = function () {
        return this._http.get(this._apiBase + "player")
            .map(function (response) { return response.json().player; });
    };
    return NetherApiService;
}());
NetherApiService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [http_1.Http])
], NetherApiService);
exports.NetherApiService = NetherApiService;
//# sourceMappingURL=nether.api.js.map