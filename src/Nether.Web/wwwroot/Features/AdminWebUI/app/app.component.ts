import { Component } from "@angular/core";
import { NetherApiService } from "./nether.api";

@Component({
   selector: "nether-app",
   templateUrl: "app/app.html",
   providers: [ NetherApiService ]
})

export class AppComponent {

    loggedIn: boolean;

    constructor(private _api: NetherApiService) {
        this.loggedIn = this._api.isLoggedIn();

        this._api.loggedInChanged.subscribe((loggedIn: boolean) => {
            this.loggedIn = loggedIn;
        });
    }
}