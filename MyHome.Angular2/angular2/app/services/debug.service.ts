import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';

@Injectable()
export class DebugService {
  private api = '/debug';
  
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
