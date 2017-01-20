// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { Injectable } from "@angular/core";

@Injectable()
export class Configuration {

    public AuthServerUrl: string = "http://localhost:5000/";

    public ResourceServerUrl: string = "http://localhost:5000/";

    public RedirectUrl: string = "http://localhost:5000";

    public AuthScope: string = "openid nether-all";
}