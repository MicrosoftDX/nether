// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            .subscribe((s: LeaderboardScore[]) => this.defaultScores = s);

        this._api.getLeaderboard("top")
            .subscribe((s: LeaderboardScore[]) => this.topScores = s);

        this._api.getLeaderboard("aroundMe")
            .subscribe((s: LeaderboardScore[]) => this.aroundMeScores = s);
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

