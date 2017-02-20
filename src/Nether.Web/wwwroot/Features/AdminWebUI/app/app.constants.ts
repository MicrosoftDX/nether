// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Injectable } from "@angular/core";

@Injectable()
export class Configuration {

    public AuthServerUrl: string = this.EnsureEndsWith(this.GetConfigFromPage("authServerUrl"), "/");

    public ApiBaseUrl: string = this.EnsureEndsWith(this.GetConfigFromPage("apiBaseUrl"), "/");

    public RedirectUrl: string = this.EnsureEndsWith(this.GetConfigFromPage("redirectUrl"), "/");

    public AuthScope: string = "openid nether-all";

    private GetConfigFromPage(configKey:string): string {
        let windowAny: any = window;
        return windowAny.__CONFIG__[configKey];
    }
    private EnsureEndsWith(value : string, end: string){
        if (value.charAt(value.length-1)!= end){
            value += end;
        }
        return value;
    }
}