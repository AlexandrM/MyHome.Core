import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers }          from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { Presets, PresetModel, PresetRowModel, PresetColumnModel, SettingModel } from 'app/models';

@Injectable()
export class SettingService {
    private api = '/setting';

    constructor(private http: Http) { 
    }
     
    query(group: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('group', 'presets');

        return this.http.get(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    }

    get(id?: string, name?: string) {
        let params: URLSearchParams = new URLSearchParams();
        if (id != null)
            params.set('id', id);
        if (name != null)
            params.set('name', name);

        return this.http.get(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    }

    post(model: SettingModel) {
        var headers = new Headers();
        headers.append('Content-Type', 'application/json');        
       
        return this.http.post(
            `${API_URL}` + this.api, 
            JSON.stringify(model),
            { headers: headers}
            ).map((res: Response) => res.json());
    }

    delete(id: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('id', id);

        return this.http.delete(
            `${API_URL}` + this.api, 
            { search: params },
            ).map((res: Response) => res.json());
    }
}