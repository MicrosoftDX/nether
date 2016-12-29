import { Injectable } from '@angular/core';

@Injectable()
export class Configuration {

    public AuthServer: string = "http://localhost:5000/";

    public ResourceServer: string = "http://localhost:5000/";
}