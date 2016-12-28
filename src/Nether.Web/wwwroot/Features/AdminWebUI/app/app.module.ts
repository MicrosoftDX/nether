import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule, Routes } from "@angular/router";
import { HttpModule } from "@angular/http";

import { AppComponent } from "./app.component";
import { PlayersComponent } from "./players/players.component";
import { PlayerDetailsComponent } from "./players/player-details.component";
import { LoginComponent } from "./login/login.component";
import { LeaderboardComponent } from "./leaderboard/leaderboard.component";
import { LeaderboardScoresComponent } from "./leaderboard/leaderboard-scores.component";
import { GroupsComponent } from "./groups/groups.component";
import { GroupDetailsComponent } from "./groups/group-details.component";
import { AuthGuard } from "./_guards/auth.guard";
import { NetherApiService } from "./nether.api";

const appRoutes: Routes = [
    { path: "players", component: PlayersComponent, canActivate: [AuthGuard] },
    { path: "player/:tag", component: PlayerDetailsComponent, canActivate: [AuthGuard] },
    { path: "groups", component: GroupsComponent, canActivate: [AuthGuard] },
    { path: "group/:name", component: GroupDetailsComponent, canActivate: [AuthGuard] },
    { path: "login", component: LoginComponent },
    { path: "leaderboard", component: LeaderboardComponent, canActivate: [AuthGuard] },
    { path: "", redirectTo: "login", pathMatch: "full" },
    { path: "**", redirectTo: "login", pathMatch: "full" }
];

@NgModule({
    imports: [
        BrowserModule,
        FormsModule, ReactiveFormsModule,
        HttpModule,
        RouterModule.forRoot(appRoutes)
    ],
    declarations: [
        AppComponent,
        PlayersComponent, PlayerDetailsComponent,
        LoginComponent,
        LeaderboardScoresComponent, LeaderboardComponent,
        GroupsComponent, GroupDetailsComponent
    ],
    providers: [
        AuthGuard, NetherApiService
    ],
    bootstrap: [
      AppComponent
   ]
})

export class AppModule { }