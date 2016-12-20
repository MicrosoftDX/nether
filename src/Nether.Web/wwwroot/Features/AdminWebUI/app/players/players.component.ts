import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/players/players.html"
})

export class PlayersComponent implements OnInit {

    allPlayers: Player[];

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
        this._api.getAllPlayers().subscribe(all => this.allPlayers = all);
    }
}

