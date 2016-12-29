import { Component, OnInit } from "@angular/core";
import { SecurityService } from "./services/security";
import { NetherApiService } from "./nether.api";

@Component({
   selector: "nether-app",
   templateUrl: "app/app.html",
   providers: [ NetherApiService ]
})
export class AppComponent implements OnInit {

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

    ngOnInit(): void {
        if (window.location.hash) {
            // if there is a hash in URL request is coming from authorization server
            this.securityService.authorizedCallback();
        }
    }

    logIn(): void {
        console.log("logging in");
        this.securityService.login();
    }

    logOut(): void {
        console.log("logging out");
        this.securityService.logOut();
    }
}