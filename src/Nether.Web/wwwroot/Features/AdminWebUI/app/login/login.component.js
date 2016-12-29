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
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var nether_api_1 = require("./../nether.api");
var LoginComponent = (function () {
    function LoginComponent(_api, fb, _router) {
        this._api = _api;
        this._router = _router;
        this.inputUsername = new forms_1.FormControl("", forms_1.Validators.required);
        this.inputPassword = new forms_1.FormControl("", forms_1.Validators.required);
        this.form = fb.group({
            "inputUsername": this.inputUsername,
            "inputPassword": this.inputPassword
        });
    }
    LoginComponent.prototype.ngOnInit = function () {
    };
    LoginComponent.prototype.onLogin = function () {
        var _this = this;
        console.log("logging in as " + this.inputUsername.value);
        this._api.login(this.inputUsername.value, this.inputPassword.value)
            .subscribe(function (token) { return _this._router.navigate(["players"]); }, function (error) { return alert("login error"); });
    };
    return LoginComponent;
}());
LoginComponent = __decorate([
    core_1.Component({
        templateUrl: "app/login/login.html"
    }),
    __metadata("design:paramtypes", [nether_api_1.NetherApiService, forms_1.FormBuilder, router_1.Router])
], LoginComponent);
exports.LoginComponent = LoginComponent;
//# sourceMappingURL=login.component.js.map