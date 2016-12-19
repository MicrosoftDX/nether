import { Injectable } from "@angular/core";
import { Http, Response, Headers, RequestOptions } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { Player } from "./model";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
import "rxjs/add/observable/of";

@Injectable()
export class NetherApiService {

   private _apiBase: string = "/api/";
   private _headers: Headers = new Headers({ "Content-Type": "application/json" });

   constructor(private _http: Http) {
   }

   getCurrentPlayer(): Observable<Player> {
      return this._http.get(this._apiBase + "player")
         .map((response: Response) => response.json().player);
   }
}