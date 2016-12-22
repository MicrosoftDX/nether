import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";
import { Router } from "@angular/router";

@Component({
    templateUrl: "app/players/players.html"
})

export class PlayersComponent implements OnInit {

    allPlayers: Player[];
    newPlayer: Player;

    constructor(private _api: NetherApiService, private _router: Router) {
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

    selectPlayer(player: Player): void {
        console.log(`navigating to player ${player.gamertag}`);
        this._router.navigate(["player", player.gamertag]);
    }

    private resetPlayer(): void {
        this.newPlayer = new Player();
    }
}

