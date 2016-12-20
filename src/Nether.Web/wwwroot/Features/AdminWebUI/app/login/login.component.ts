import { Component, OnInit } from "@angular/core";
import { Form, FormGroup, FormControl, Validators, FormBuilder } from "@angular/forms";
import { Router } from "@angular/router"
import { NetherApiService } from "./../nether.api";
import { Player } from "./../model";

@Component({
    templateUrl: "app/login/login.html"
})

export class LoginComponent implements OnInit {

    form: FormGroup;
    inputUsername: FormControl = new FormControl("", Validators.required);
    inputPassword: FormControl = new FormControl("", Validators.required);

    constructor(private _api: NetherApiService, fb: FormBuilder, private _router: Router) {
        this.form = fb.group({
            "inputUsername": this.inputUsername,
            "inputPassword": this.inputPassword
        });
    }

    ngOnInit(): void {
        
    }

    onLogin(): void {
        console.log("logging in as " + this.inputUsername.value);

        this._api.login(this.inputUsername.value, this.inputPassword.value)
            .subscribe(
                token => this._router.navigate(["players"]),
                error => alert("login error")
            );
    }
}

