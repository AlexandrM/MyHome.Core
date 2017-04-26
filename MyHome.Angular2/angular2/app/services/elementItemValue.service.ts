import { Injectable }              from '@angular/core';
import { Http, Response, URLSearchParams, Headers }          from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { Presets, PresetModel, PresetRowModel, PresetColumnModel, SettingModel } from 'app/models';

@Injectable()
export class ElementItemValueService {
    private api = '/elementItemValue';

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