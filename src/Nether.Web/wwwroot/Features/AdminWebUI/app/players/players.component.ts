import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/players/players.html"
})

export class PlayersComponent implements OnInit {

    private _player: Player;

    constructor(private _api: NetherApiService) {
    }

    ngOnInit(): void {
        this._api.login("testuser", "testuser").subscribe();

        //this._api.getCurrentPlayer().subscribe(p => this._player = p);
    }
}

