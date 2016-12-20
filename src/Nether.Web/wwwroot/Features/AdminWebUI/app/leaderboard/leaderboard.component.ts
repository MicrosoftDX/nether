import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/leaderboard/leaderboard.html"
})

export class LeaderboardComponent implements OnInit {

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
    }
}

