import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";
import { ActivatedRoute, Params } from "@angular/router";
import "rxjs/add/operator/switchMap";

@Component({
    templateUrl: "app/players/player-details.html"
})

export class PlayerDetailsComponent implements OnInit {

    player: Player;

    constructor(private _api: NetherApiService, private _route: ActivatedRoute) {
    }

    ngOnInit(): void {
        // call web service to get player frout the gamertag specified in the route
        this._route.params
            .switchMap((params: Params) => this._api.getPlayer(params["tag"]))
            .subscribe((player: Player) => this.player = player);
    }
}
