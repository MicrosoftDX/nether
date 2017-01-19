// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Component, Input } from "@angular/core";
import { LeaderboardScore } from "./../model";

@Component({
    selector: "nether-leaderboard-scores",
    templateUrl: "app/leaderboard/leaderboard-scores.html"
})
export class LeaderboardScoresComponent {

    @Input() scores: LeaderboardScore[];

}