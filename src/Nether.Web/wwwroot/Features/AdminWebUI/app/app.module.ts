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

const appRoutes: Routes = [
    { path: "players", component: PlayersComponent },
    { path: "player/:tag", component: PlayerDetailsComponent },
    { path: "login", component: LoginComponent },
    { path: "leaderboard", component: LeaderboardComponent },
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
        LeaderboardScoresComponent, LeaderboardComponent
   ],
   bootstrap: [
      AppComponent
   ]
})

export class AppModule { }