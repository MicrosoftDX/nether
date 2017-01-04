import { Injectable } from "@angular/core";
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { NetherApiService } from "./../nether.api";

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private _router: Router, private _api: NetherApiService) {
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (this._api.isLoggedIn()) {
            return true;
        }

        this._router.navigate(["login"]);
        return false;
    }
}