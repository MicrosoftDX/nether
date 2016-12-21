import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/players/players.html"
})

export class PlayersComponent implements OnInit {

    allPlayers: Player[];
    newPlayer: Player;

    constructor(private _api: NetherApiService) {
        this.resetPlayer();
    }

    ngOnInit(): void {
        this.refreshPlayers();
    }

    refreshPlayers(): void {
        this._api.getAllPlayers().subscribe((all: Player[]) => this.allPlayers = all);
    }

    createPlayer(): void {
        this._api.createPlayer(this.newPlayer).subscribe((r: any) => {
            this.refreshPlayers();
            this.resetPlayer();
        });
    }

    private resetPlayer() {
        this.newPlayer = new Player();
    }
}

