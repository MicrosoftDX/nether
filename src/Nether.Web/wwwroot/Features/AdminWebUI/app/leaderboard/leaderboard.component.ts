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
    scoreToPost: number = 0;
    gamertagToPost: string = null;
    countryToPost: string = "UK";
    customTagToPost: string = null;

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
        this.refreshLeaderboard();
    }

    private refreshLeaderboard(): void {
        this._api.getLeaderboard("default")
            .subscribe(s => this.defaultScores = s);

        this._api.getLeaderboard("top")
            .subscribe(s => this.topScores = s);

        this._api.getLeaderboard("aroundMe")
            .subscribe(s => this.aroundMeScores = s);
    }

    private clearScore(): void {
        this.scoreToPost = 0;
        this.gamertagToPost = null;
        this.countryToPost = "UK";
        this.customTagToPost = null;
    }

    postScore(): void {
        this._api.postScore(this.scoreToPost,
            this.countryToPost,
            this.customTagToPost)
            .subscribe(r => {
                this.clearScore();
                this.refreshLeaderboard();
            });
    }
}

