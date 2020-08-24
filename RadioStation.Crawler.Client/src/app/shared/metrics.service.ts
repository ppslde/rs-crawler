import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { RscResponse } from '../core';
import { repeat } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MetricsService {

  constructor(private http: HttpClient) {
  }

  loadMetrics(): Promise<any> {
    return new Promise((resolve, reject) => {
      this.http.get<RscResponse>(`${environment.apiUrl}/api/metrics/full`).toPromise()
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
      this.http.get<RscResponse>(`${environment.apiUrl}/api/metrics/untagged`).toPromise()
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
