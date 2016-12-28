import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Player, Group } from "./../model";
import { Router, ActivatedRoute, Params } from "@angular/router";
import "rxjs/add/operator/switchMap";

@Component({
    templateUrl: "app/groups/group-details.html"
})

export class GroupDetailsComponent implements OnInit {
    readonly groupNameParam: string = "name";

    name: string;
    group: Group;
    members: string[];

    constructor(private _api: NetherApiService, private _route: ActivatedRoute, private _router: Router) {

    }

    ngOnInit(): void {
        this._route.params
            .switchMap((params: Params) => {
                this.name = params[this.groupNameParam];
                console.log(`loading group '${this.name}'`);
                return this._api.getGroup(this.name);
            })
            .subscribe((group: Group) => {
                console.log("group loaded");
                console.log(group);
                this.group = group;

                this._api.getGroupPlayers(group.name)
                    .subscribe((members: string[]) => this.members = members);
            });
    }

    updateGroup(): void {
        this._api.updateGroup(this.group).subscribe((r: any) => {
            console.log("group updated, going back...");
            this._router.navigate(["groups"]);
        });
    }

    removeMember(gamertag: string) {
        console.log(`removing ${gamertag}...`);
    }
}
