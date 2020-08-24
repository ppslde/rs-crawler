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

  metrics: any;
  loading: boolean;

  loadMetrics(): Promise<boolean> {
    this.loading = true;
    return new Promise((resolve, reject) =>{
      this.http.get<RscResponse>(`${environment.apiUrl}/api/metrics/full`).toPromise()
      .then(r => {
        if (r.data) {
          this.metrics = r.data;
        }
        resolve(true);
      }).catch(e => {
        console.log(e);
        reject(e);
      }).finally(() => {
        this.loading = false;
      });
    })
    
  }
}
