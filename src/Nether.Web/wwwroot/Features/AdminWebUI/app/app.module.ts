import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { HttpModule } from "@angular/http";

import { AppComponent } from "./app.component";
import { PlayersComponent } from "./players/players.component";
import { LoginComponent } from "./login/login.component";

@NgModule({
    imports: [
        BrowserModule,
        FormsModule, ReactiveFormsModule,
        HttpModule,
        RouterModule.forRoot([
            { path: "players", component: PlayersComponent },
            { path: "login", component: LoginComponent },
            { path: "", redirectTo: "login", pathMatch: "full" },
            { path: "**", redirectTo: "login", pathMatch: "full" }
        ])
    ],
    declarations: [
        AppComponent,
        PlayersComponent,
        LoginComponent
   ],
   bootstrap: [
      AppComponent
   ]
})

export class AppModule { }