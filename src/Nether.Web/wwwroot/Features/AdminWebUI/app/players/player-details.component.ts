import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";
import { Router, ActivatedRoute, Params } from "@angular/router";
import "rxjs/add/operator/switchMap";

@Component({
    templateUrl: "app/players/player-details.html"
})

export class PlayerDetailsComponent implements OnInit {

    player: Player = null;

    constructor(private _api: NetherApiService, private _route: ActivatedRoute, private _router: Router) {
    }

    ngOnInit(): void {
        // call web service to get player frout the gamertag specified in the route
        console.log("loading player details");
        this._route.params
            .switchMap((params: Params) => this._api.getPlayer(params["tag"]))
            .subscribe((player: Player) => {
                console.log("player loaded");
                this.player = player;
                console.log(player);
            });
    }

    updatePlayer(): void {
        console.log("updating...");
        this._api.updatePlayer(this.player)
            .subscribe((r: any) => {
                console.log("player updated, going back...");
                this._router.navigate(["players"]);
            });
    }
}
