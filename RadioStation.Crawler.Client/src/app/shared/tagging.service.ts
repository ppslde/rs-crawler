import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Mapping, IResponse, IUntagged, IRecord, IReCrawl } from '../core';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class TaggingService {

  constructor(private http: HttpClient) {
  }

  private apislug = '/api/tagging/';

  getSongTagData(uts: IUntagged): Promise<IReCrawl> {
    return new Promise((resolve, reject) => {
      this.http.get<IResponse<IReCrawl>>(`${environment.apiUrl}${this.apislug}tagdata`, {
        params: {
          artist: uts.artist,
          song: uts.track
        }
      }).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          } else {
            resolve();
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }

  getUntaggedSongs(): Promise<IUntagged[]> {
    return new Promise((resolve, reject) => {
      this.http.get<IResponse<IUntagged[]>>(`${environment.apiUrl}${this.apislug}untagged`).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          } else {
            resolve([]);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }

  loadMappings(): Promise<Array<Mapping>> {
    return new Promise((resolve, reject) => {
      this.http.get<IResponse<Mapping[]>>(`${environment.apiUrl}${this.apislug}mappings`).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          } else {
            resolve([]);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }

  createMapping(map: Mapping): Promise<Mapping> {
    return new Promise((resolve, reject) => {
      this.http.post<IResponse<Mapping>>(`${environment.apiUrl}${this.apislug}mappings`, map, httpOptions).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          } else {
            resolve(null);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }

  updateMapping(map: Mapping): Promise<Mapping> {
    return new Promise((resolve, reject) => {
      this.http.put<IResponse<Mapping>>(`${environment.apiUrl}${this.apislug}mappings`, map, httpOptions).toPromise()
        .then(r => {
          if (r.data) {
            resolve(r.data);
          } else {
            resolve(null);
          }
        }).catch(e => {
          console.log(e);
          reject(e);
        });
    });
  }
}
