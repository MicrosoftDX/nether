import { Component } from "@angular/core";
import { SecurityService } from "./services/security";
import { NetherApiService } from "./nether.api";

@Component({
   selector: "nether-app",
   templateUrl: "app/app.html",
   providers: [ NetherApiService ]
})

export class AppComponent {

    loggedIn: boolean;
    securityService: SecurityService;

    constructor(private _api: NetherApiService, securityService: SecurityService) {
        this.loggedIn = this._api.isLoggedIn();
        this.securityService = securityService;

        this._api.loggedInChanged.subscribe((loggedIn: boolean) => {
            console.log("logged in: " + loggedIn);
            this.loggedIn = loggedIn;
        });
    }

    logIn(): void {
        console.log("logging in");
    }

    logOut(): void {
    }
}