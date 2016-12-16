import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { HttpModule } from "@angular/http";

import { AppComponent } from "./app.component";

@NgModule({
   imports: [
      BrowserModule,
      FormsModule, ReactiveFormsModule,
      HttpModule
   ],
   declarations: [
      AppComponent
   ],
   bootstrap: [
      AppComponent
   ]
})

export class AppModule { }