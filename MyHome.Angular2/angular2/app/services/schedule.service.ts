import { Injectable }              from '@angular/core';
import { Http, Response, URLSearchParams, Headers }          from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { Presets, PresetModel, PresetRowModel, PresetColumnModel, SettingModel, ScheduleModel } from 'app/models';

@Injectable()
export class ScheduleService {
    private api = '/schedule';

    constructor(private http: Http) { 
    }
     
    get(id: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.append('id', id);

        return this.http.get(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    } 

    query() {
        let params: URLSearchParams = new URLSearchParams();

        return this.http.get(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    }   

    post(model: ScheduleModel) {
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
        params.append('id', id);

        return this.http.delete(
            `${API_URL}` + this.api, 
            { search: params })
            .map((res: Response) => res.json());
    } 
}