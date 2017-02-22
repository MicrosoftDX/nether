// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { LeaderboardScore } from "./../model";

@Component({
    templateUrl: "app/leaderboard/leaderboard.html"
})

export class LeaderboardComponent implements OnInit {

    top5Scores: LeaderboardScore[];
    top10Scores: LeaderboardScore[];
    aroundMe5Scores: LeaderboardScore[];
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
        this._api.getLeaderboard("Top_10")
            .subscribe((s: LeaderboardScore[]) => this.top10Scores = s);

        this._api.getLeaderboard("Top_5")
            .subscribe((s: LeaderboardScore[]) => this.top5Scores = s);

        this._api.getLeaderboard("5_AroundMe")
            .subscribe((s: LeaderboardScore[]) => this.aroundMe5Scores = s);
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
            .subscribe((r: any) => {
                this.clearScore();
                this.refreshLeaderboard();
            });
    }
}

