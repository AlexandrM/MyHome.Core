import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable()
export class ElementItemEnumService {
    private api = '/elementItemEnum';

    constructor(private http: Http) { 
    }
     
    query() {
        let params: URLSearchParams = new URLSearchParams();

        return this.http.get(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    }    
}