import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { HttpModule } from "@angular/http";

import { AppComponent } from "./app.component";
import { PlayersComponent } from "./players/players.component";

@NgModule({
  imports: [
      BrowserModule,
      FormsModule, ReactiveFormsModule,
     HttpModule,
     RouterModule.forRoot([
       { path: "players", component: PlayersComponent },
       { path: "", redirectTo: "players", pathMatch: "full" },
       { path: "**", redirectTo: "players", pathMatch: "full" }
     ])

   ],
   declarations: [
     AppComponent,
     PlayersComponent
   ],
   bootstrap: [
      AppComponent
   ]
})

export class AppModule { }