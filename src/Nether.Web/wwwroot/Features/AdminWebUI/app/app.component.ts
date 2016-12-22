import { Component } from "@angular/core";
import { NetherApiService } from "./nether.api";

@Component({
   selector: "nether-app",
   templateUrl: "app/app.html",
   providers: [ NetherApiService ]
})

export class AppComponent {
}