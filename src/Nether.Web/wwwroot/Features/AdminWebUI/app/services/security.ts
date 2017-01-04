import { Injectable } from "@angular/core";
import { Http, Headers } from "@angular/http";
import "rxjs/add/operator/map";
import { Observable } from "rxjs/Observable";
import { Configuration } from "./../app.constants";
import { Router } from "@angular/router";

@Injectable()
export class SecurityService {

    private actionUrl: string;
    private headers: Headers;
    private storage: any;
    private isAuthorizedKey: string = "isAuthorized";
    private hasAdminRoleKey: string = "hasAdminRole";

    constructor(private _http: Http, private _configuration: Configuration, private _router: Router) {

        this.actionUrl = _configuration.ResourceServerUrl + "api/DataEventRecords/";

        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "application/json");
        this.storage = sessionStorage; // localStorage;

        let isAuthorized: any = this.retrieve(this.isAuthorizedKey);
        if (isAuthorized) {
            this.IsAuthorized = <boolean>isAuthorized;
            this.HasAdminRole = this.retrieve(this.hasAdminRoleKey);
        }

        console.log(this.IsAuthorized);
    }

    public IsAuthorized: boolean = false;
    public HasAdminRole: boolean = false;

    public getToken(): any {
        return this.retrieve("authorizationData");
    }

    public resetAuthorizationData(): void {
        this.store("authorizationData", "");
        this.store("authorizationDataIdToken", "");

        this.IsAuthorized = false;
        this.HasAdminRole = false;
        this.store(this.hasAdminRoleKey, false);
        this.store(this.isAuthorizedKey, false);
    }

    public UserData: any;
    public SetAuthorizationData(token: any, id_token: any): void {
        if (this.retrieve("authorizationData") !== "") {
            this.store("authorizationData", "");
        }

        this.store("authorizationData", token);
        this.store("authorizationDataIdToken", id_token);
        this.IsAuthorized = true;
        this.store(this.isAuthorizedKey, true);

        this.getUserData()
            .subscribe((data: string[]) => this.UserData = data,
            error => this.HandleError(error),
            () => {
                for (var i = 0; i < this.UserData.role.length; i++) {
                    if (this.UserData.role[i] === "dataEventRecords.admin") {
                        this.HasAdminRole = true;
                        this.store(this.hasAdminRoleKey, true);
                    }
                }
            });

        // if the role was returned in the id_token, the roundtrip is not required
        // var data: any = this.getDataFromToken(id_token);
        // for (var i = 0; i < data.role.length; i++) {
        //    if (data.role[i] === "dataEventRecords.admin") {
        //        this.HasAdminRole = true;
        //        this.store(this.hasAdminRoleKey, true)
        //    }
        // }
    }

    public login() {
        this.resetAuthorizationData();

        console.log("BEGIN Authorize, no auth data");

        let authorizationUrl: string = this._configuration.AuthServerUrl + "connect/authorize";
        let clientId: string = "angular2client";
        let redirectUri: string = this._configuration.RedirectUrl;
        let responseType: string = "id_token token";
        let scope: string = this._configuration.AuthScope;
        let nonce: string = "N" + Math.random() + "" + Date.now();
        let state: string = Date.now() + "" + Math.random();

        this.store("authStateControl", state);
        this.store("authNonce", nonce);
        console.log("AuthorizedController created. adding myautostate: " + this.retrieve("authStateControl"));

        let url: string =
            authorizationUrl + "?" +
            "response_type=" + encodeURI(responseType) + "&" +
            "client_id=" + encodeURI(clientId) + "&" +
            "redirect_uri=" + encodeURI(redirectUri) + "&" +
            "scope=" + encodeURI(scope) + "&" +
            "nonce=" + encodeURI(nonce) + "&" +
            "state=" + encodeURI(state);

        console.log("navigating to - " + url);

        window.location.href = url;
    }

    public authorizedCallback(): void {
        console.log("BEGIN AuthorizedCallback, no auth data");
        this.resetAuthorizationData();

        let hash: string = window.location.hash.substr(1);

        var result: any = hash.split("&").reduce(function (result: any, item: string) {
            var parts = item.split("=");
            result[parts[0]] = parts[1];
            return result;
        }, {});

        console.log(result);
        console.log("AuthorizedCallback created, begin token validation");

        var token = "";
        var id_token = "";
        var authResponseIsValid = false;
        if (!result.error) {

            if (result.state !== this.retrieve("authStateControl")) {
                console.log("AuthorizedCallback incorrect state");
            } else {

                token = result.access_token;
                id_token = result.id_token;

                var dataIdToken: any = this.getDataFromToken(id_token);
                console.log(dataIdToken);

                // validate nonce
                if (dataIdToken.nonce !== this.retrieve("authNonce")) {
                    console.log("AuthorizedCallback incorrect nonce");
                } else {
                    this.store("authNonce", "");
                    this.store("authStateControl", "");

                    authResponseIsValid = true;
                    console.log("AuthorizedCallback state and nonce validated, returning access token");
                }
            }
        }

        if (authResponseIsValid) {
            this.SetAuthorizationData(token, id_token);
            console.log(this.retrieve("authorizationData"));

            // router navigate to DataEventRecordsList
            this._router.navigate(["/dataeventrecords/list"]);
        } else {
            this.resetAuthorizationData();
            this._router.navigate(["/Unauthorized"]);
        }
    }

    public logOut(): void {
        // /connect/endsession?id_token_hint=...&post_logout_redirect_uri=https://myapp.com
        console.log("BEGIN Authorize, no auth data");

        var authorizationUrl = "https://localhost:44318/connect/endsession";

        let id_token_hint: any = this.retrieve("authorizationDataIdToken");
        var post_logout_redirect_uri = "https://localhost:44311/Unauthorized";

        let url: string =
            authorizationUrl + "?" +
            "id_token_hint=" + encodeURI(id_token_hint) + "&" +
            "post_logout_redirect_uri=" + encodeURI(post_logout_redirect_uri);

        this.resetAuthorizationData();

        window.location.href = url;
    }

    public HandleError(error: any) {
        console.log(error);
        if (error.status === 403) {
            this._router.navigate(["/Forbidden"]);
        } else if (error.status === 401) {
            this.resetAuthorizationData();
            this._router.navigate(["/Unauthorized"]);
        }
    }

    private urlBase64Decode(str: string): string {
        var output = str.replace("-", "+").replace("_", "/");
        switch (output.length % 4) {
            case 0:
                break;
            case 2:
                output += "==";
                break;
            case 3:
                output += "=";
                break;
            default:
                throw "Illegal base64url string!";
        }

        return window.atob(output);
    }

    private getDataFromToken(token: any): any {
        var data = {};
        if (typeof token !== "undefined") {
            let encoded: string = token.split(".")[1];
            data = JSON.parse(this.urlBase64Decode(encoded));
        }

        return data;
    }

    private retrieve(key: string): any {
        var item = this.storage.getItem(key);

        if (item && item !== "undefined") {
            return JSON.parse(this.storage.getItem(key));
        }

        return;
    }

    private store(key: string, value: any) {
        this.storage.setItem(key, JSON.stringify(value));
    }

    private getUserData = (): Observable<string[]> => {
        this.setHeaders();
        return this._http.get("https://localhost:44318/connect/userinfo", {
            headers: this.headers,
            body: ""
        }).map(res => res.json());
    };

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "application/json");

        var token = this.getToken();

        if (token !== "") {
            this.headers.append("Authorization", "Bearer " + token);
        }
    }
}