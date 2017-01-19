// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Component, OnInit } from "@angular/core";
import { NetherApiService } from "./../nether.api";
import { Group } from "./../model";
import { Router } from "@angular/router";

@Component({
    templateUrl: "app/groups/groups.html"
})

export class GroupsComponent implements OnInit {

    allGroups: Group[];
    newGroup: Group;

    constructor(private _api: NetherApiService, private _router: Router) {
        this.resetGroup();
    }

    ngOnInit(): void {
        this.refreshGroups();
    }

    refreshGroups(): void {
        console.log("refreshing groups...");
        this._api.getAllGroups().subscribe((all: Group[]) => {
            console.log("groups refreshed");
            this.allGroups = all;
        });
    }

    createGroup(): void {
        console.log("creating group... ");
        this._api.createGroup(this.newGroup).subscribe((r: any) => {
            console.log("group created.");
            this.refreshGroups();
            this.resetGroup();
        });
    }

    private resetGroup(): void {
        this.newGroup = new Group();
    }
}

