import { Injectable, EventEmitter, Output } from "@angular/core";
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Player, LeaderboardScore, Group } from "./model";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/catch";
import "rxjs/add/observable/throw";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
import "rxjs/add/operator/mergeMap";
import "rxjs/add/observable/of";

@Injectable()
export class NetherApiService {

    private _serverUrl: string = "http://localhost:5000/";
    private authCacheKey: string = "cachedToken";
    private _clientId: string = "resourceowner-test";
    private _clientSecret: string = "devsecret";
    private _endpointConfig: EndpointConfiguration;

    loggedInChanged = new EventEmitter<boolean>();

    constructor(private _http: Http) {
    }

    isLoggedIn(): boolean {
        return this.getToken() !== null;
    }

    private getToken(): TokenResponse {
        let s: string = localStorage.getItem(this.authCacheKey);
        return s ? JSON.parse(s) : null;
    }

    login(username: string, password: string): Observable<string> {
        return this._http.get(this._serverUrl + ".well-known/openid-configuration")
            .map((response: Response) => <EndpointConfiguration>response.json())
            .flatMap((config: EndpointConfiguration) => {

                this._endpointConfig = config;
                console.log("token endpoint: " + config.token_endpoint);
                let authHead: string = btoa(this._clientId + ":" + this._clientSecret);
                let formData: string = `grant_type=password&username=${username}&password=${password}&scope=nether-all`;

                return this._http.post(config.token_endpoint, formData,
                    new RequestOptions({
                        headers: new Headers({
                            "Authorization": "Basic " + authHead,
                            "Content-Type": "application/x-www-form-urlencoded"
                        })
                    }))
                    .map((response: Response) => {
                        let token: TokenResponse = <TokenResponse>response.json();
                        console.log("token obtained");

                        // cache token
                        localStorage.setItem(this.authCacheKey, JSON.stringify(token));
                        this.loggedInChanged.emit(true);

                        this.cachePlayer();

                        return token.access_token;
                    })
                    .catch(this.catchErrors);
            });
    }

    getCurrentPlayer(): Observable<Player> {
        return this._http.get(this._serverUrl + "api/player", this.getRequestOptions())
            .map((response: Response) => response.json().player);
    }

    getPlayer(gamertag: string): Observable<Player> {
        return this._http.get(this._serverUrl + "api/players/" + gamertag, this.getRequestOptions())
            .map((r: Response) => r.json().player);
    }

    getAllPlayers(): Observable<Player[]> {
        return this._http.get(this._serverUrl + "api/players", this.getRequestOptions())
            .map((response: Response) => response.json().players);
    }

    getLeaderboard(type: string): Observable<LeaderboardScore[]> {
        return this._http.get(this._serverUrl + "api/leaderboard/" + type, this.getRequestOptions())
            .map((response: Response) => response.json().entries);
    }

    postScore(score: number, country: string, customTag: string, gamerTag?: string): Observable<Response> {
        return this._http.post(this._serverUrl + "api/leaderboard", {
            score: score,
            country: country,
            customTag: customTag
        }, this.getRequestOptions());
    }

    createPlayer(player: Player): Observable<Response> {
        return this._http.post(this._serverUrl + "api/players", player, this.getRequestOptions());
    }

    updatePlayer(player: Player): Observable<Response> {
        return this._http.post(this._serverUrl + "api/players", player, this.getRequestOptions());
    }

    getAllGroups(): Observable<Group[]> {
        return this._http.get(this._serverUrl + "api/groups", this.getRequestOptions())
            .map((r: Response) => <Group[]>r.json().groups)
            .catch(this.catchErrors);
    }

    getGroup(name: string): Observable<Group> {
        return this._http.get(this._serverUrl + "api/groups/" + name, this.getRequestOptions())
            .map((r: Response) => <Group>r.json().group)
            .catch(this.catchErrors);
    }

    getGroupPlayers(name: string): Observable<string[]> {
        return this._http.get(this._serverUrl + "api/groups/" + name + "/players", this.getRequestOptions())
            .map((r: Response) => <string[]>r.json().gamertags);
    }

    updateGroup(group: Group): Observable<Response> {
        return this._http.put(this._serverUrl + "api/groups/" + group.name, group, this.getRequestOptions());
    }

    createGroup(group: Group): Observable<Response> {
        return this._http.post(this._serverUrl + "api/groups", group, this.getRequestOptions());
    }

    getPlayerGroups(gamertag: string): Observable<Group[]> {
        return this._http.get(`${this._serverUrl}api/players/${gamertag}/groups`, this.getRequestOptions())
            .map((r: Response) => <Group[]>r.json().groups);
    }

    addPlayerToGroup(gamertag: string, groupName: string): Observable<Response> {
        return this._http.put(`${this._serverUrl}api/players/${gamertag}/groups/${groupName}`, null, this.getRequestOptions());
    }

    removePlayerFromGroup(gamertag: string, groupName: string): Observable<Response> {
        return this._http.delete(`${this._serverUrl}api/groups/${groupName}/players/${gamertag}`, this.getRequestOptions());
    }

    private cachePlayer(): void {
        // this.getCurrentPlayer().subscribe((p: Player) => this._currentPlayer = p);
    }

    private getRequestOptions(): RequestOptions {

        let token: TokenResponse = this.getToken();

        return new RequestOptions({
            headers: new Headers({
                "Authorization": token.token_type + " " + token.access_token
                })
        });
    }

    private catchErrors(err: any): Observable<any> {
        console.error("call failed");
        if (err.status === 401) {
            console.error("auth failure");
            localStorage.removeItem(this.authCacheKey);
            this.loggedInChanged.emit(false);
        }
        return Observable.throw(err);
    }
}

// subset of endpoint configuraiton
class EndpointConfiguration {
    issuer: string;
    jwks_uri: string;
    authorization_endpoint: string;
    token_endpoint: string;
    scopes_supported: string[];
    claims_supported: string[];
}

// subset of token response
class TokenResponse {
    access_token: string;
    expires_in: number;
    token_type: string;
}