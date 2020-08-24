import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Station, RscResponse } from '../core'
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs/internal/Observable';
import { catchError, map, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Injectable({
  providedIn: 'root'
})
export class StationService {

  
  constructor(private http: HttpClient) { }

  private stations: Station[] = [];

  getStations(): Promise<Station[]> {
    return new Promise<Station[]>((resolve, reject) => {
      if (this.stations.length == 0) {
        this.http.get<RscResponse>(`${environment.apiUrl}/api/station`).toPromise()
          .then(r => {
            this.stations = r.data;
            resolve(this.stations);
          })
          .catch(e => {
            console.log(e);
            reject(e);
          })
      } else {
        resolve(this.stations);
      }
    });
  }

  update(station: Station): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/station`, station, httpOptions);
  }
}