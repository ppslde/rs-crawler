import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { IResponse } from '../core';

@Injectable({
  providedIn: 'root'
})
export class MetricsService {

  constructor(private http: HttpClient) {
  }

  private apislug = '/api/metrics/';

  loadMetrics(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get<IResponse<any>>(`${environment.apiUrl}${this.apislug}full`).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          }else{
            resolve([]);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }

  loadUntaggedMetrics(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get<IResponse<any>>(`${environment.apiUrl}${this.apislug}untagged`).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          }else{
            resolve([]);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }
}
