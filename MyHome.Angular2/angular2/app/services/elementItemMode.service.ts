import { Injectable }              from '@angular/core';
import { Http, Response, URLSearchParams, Headers }          from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { PresetElementModel } from 'app/models/models';

@Injectable()
export class ElementItemModeService {
    private api = '/ElementItemMode';

    constructor(private http: Http) { 
    }
     
    post(model: any) {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');        
       
        return this.http.post(
            `${API_URL}` + this.api, 
            JSON.stringify(model),
            { headers: headers}
            ).map((res: Response) => res.json());
    }    
}