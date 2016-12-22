import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/players/player-details.html"
})

export class PlayerDetailsComponent implements OnInit {

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
    }
}
