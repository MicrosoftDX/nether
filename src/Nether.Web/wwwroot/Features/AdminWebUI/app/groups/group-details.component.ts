// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    allPlayers: Player[];
    leftPlayers: Player[];
    selectedGamertag: string;

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

                console.log("loading group members");
                this._api.getGroupPlayers(group.name)
                    .subscribe((members: string[]) => {
                        this.members = members;

                        console.log("members loaded, getting all players");
                        this._api.getAllPlayers()
                            .subscribe((all: Player[]) => {
                                this.allPlayers = all;
                                console.log("loaded " + all.length + " players");
                                this.leftPlayers = all.filter(a => members.indexOf(a.gamertag) === -1);
                                console.log(`${this.leftPlayers.length} of them can be added to the group`);
                            });
                    });
            });

        // todo: this won't work if you have too many players, probably need a search box with autocomplete instead
    }

    updateGroup(): void {
        this._api.updateGroup(this.group).subscribe((r: any) => {
            console.log("group updated, going back...");
            this._router.navigate(["groups"]);
        });
    }

    removeMember(gamertag: string) {
        console.log(`removing ${gamertag}...`);

        this._api.removePlayerFromGroup(gamertag, this.name)
            .subscribe((r: any) => {
                console.log("removed");
                this.members = this.members.filter(m => m !== gamertag);
                this.leftPlayers = this.allPlayers.filter(a => this.members.indexOf(a.gamertag) === -1);
            });
    }

    addMember() {
        console.log(`adding ${this.selectedGamertag} to group`);

        this._api.addPlayerToGroup(this.selectedGamertag, this.name)
            .subscribe((r: any) => {
                console.log("player added");
                this.members.push(this.selectedGamertag);
                this.leftPlayers = this.allPlayers.filter(a => this.members.indexOf(a.gamertag) === -1);
                this.selectedGamertag = null;
            });
    }
}
