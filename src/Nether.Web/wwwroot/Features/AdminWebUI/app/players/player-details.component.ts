import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player, Group } from "./../model";
import { Router, ActivatedRoute, Params } from "@angular/router";
import "rxjs/add/operator/switchMap";

@Component({
    templateUrl: "app/players/player-details.html"
})

export class PlayerDetailsComponent implements OnInit {

    gamertag: string = null;
    player: Player = null;
    playerGroups: Group[] = null;
    allGroups: Group[] = null;

    constructor(private _api: NetherApiService, private _route: ActivatedRoute, private _router: Router) {
    }

    ngOnInit(): void {
        // call web service to get player frout the gamertag specified in the route
        this._route.params
            .switchMap((params: Params) => {
                this.gamertag = params["tag"];
                console.log("loading player details, gamertag: " + this.gamertag);
                this.reloadGroups();
                return this._api.getPlayer(this.gamertag);
            })
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

    reloadGroups(): void {
        this._api.getPlayerGroups(this.gamertag).subscribe(gs => this.playerGroups = gs);
        this._api.getAllGroups().subscribe(ag => this.allGroups = ag);
    }

    toggleGroupMembership(g: Group): void {
        let isMember: boolean = this.isMemberOf(g);
        console.log(`switching '${g.name}' ${isMember}=>${!isMember}`);

        if (!isMember) {
            this._api.addPlayerToGroup(this.player.gamertag, g.name)
                .subscribe((r: any) => this.playerGroups.push(g));
        } else {
            this._api.removePlayerFromGroup(this.player.gamertag, g.name)
                .subscribe((r: any) => this.playerGroups = this.playerGroups.filter(g => g.name !== g.name));
        }
    }

    isMemberOf(g: Group): boolean {
        return this.playerGroups.some(i => i.name === g.name);
    }
}
