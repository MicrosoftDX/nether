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
var platform_browser_1 = require("@angular/platform-browser");
var forms_1 = require("@angular/forms");
var router_1 = require("@angular/router");
var http_1 = require("@angular/http");
var app_constants_1 = require("./app.constants");
var security_1 = require("./services/security");
var app_component_1 = require("./app.component");
var players_component_1 = require("./players/players.component");
var player_details_component_1 = require("./players/player-details.component");
var login_component_1 = require("./login/login.component");
var leaderboard_component_1 = require("./leaderboard/leaderboard.component");
var leaderboard_scores_component_1 = require("./leaderboard/leaderboard-scores.component");
var groups_component_1 = require("./groups/groups.component");
var group_details_component_1 = require("./groups/group-details.component");
var auth_guard_1 = require("./_guards/auth.guard");
var nether_api_1 = require("./nether.api");
var appRoutes = [
    { path: "players", component: players_component_1.PlayersComponent, canActivate: [auth_guard_1.AuthGuard] },
    { path: "player/:tag", component: player_details_component_1.PlayerDetailsComponent, canActivate: [auth_guard_1.AuthGuard] },
    { path: "groups", component: groups_component_1.GroupsComponent, canActivate: [auth_guard_1.AuthGuard] },
    { path: "group/:name", component: group_details_component_1.GroupDetailsComponent, canActivate: [auth_guard_1.AuthGuard] },
    { path: "login", component: login_component_1.LoginComponent },
    { path: "leaderboard", component: leaderboard_component_1.LeaderboardComponent, canActivate: [auth_guard_1.AuthGuard] },
    { path: "", redirectTo: "login", pathMatch: "full" },
    { path: "**", redirectTo: "login", pathMatch: "full" }
];
var AppModule = (function () {
    function AppModule() {
    }
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        imports: [
            platform_browser_1.BrowserModule,
            forms_1.FormsModule, forms_1.ReactiveFormsModule,
            http_1.HttpModule,
            router_1.RouterModule.forRoot(appRoutes)
        ],
        declarations: [
            app_component_1.AppComponent,
            players_component_1.PlayersComponent, player_details_component_1.PlayerDetailsComponent,
            login_component_1.LoginComponent,
            leaderboard_scores_component_1.LeaderboardScoresComponent, leaderboard_component_1.LeaderboardComponent,
            groups_component_1.GroupsComponent, group_details_component_1.GroupDetailsComponent
        ],
        providers: [
            auth_guard_1.AuthGuard,
            nether_api_1.NetherApiService,
            app_constants_1.Configuration,
            security_1.SecurityService
        ],
        bootstrap: [
            app_component_1.AppComponent
        ]
    }),
    __metadata("design:paramtypes", [])
], AppModule);
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map