import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { LeaderboardScore } from "./../model";

@Component({
    templateUrl: "app/leaderboard/leaderboard.html"
})

export class LeaderboardComponent implements OnInit {

    defaultScores: LeaderboardScore[];
    topScores: LeaderboardScore[];
    aroundMeScores: LeaderboardScore[];

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
        this._api.getLeaderboard("default")
            .subscribe(s => this.defaultScores = s);

        this._api.getLeaderboard("top")
            .subscribe(s => this.topScores = s);

        this._api.getLeaderboard("aroundMe")
            .subscribe(s => this.aroundMeScores = s);

    }
}

