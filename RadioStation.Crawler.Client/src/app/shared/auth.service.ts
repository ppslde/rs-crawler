import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TokenService } from './token.service';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private tokenSrv: TokenService) { }

  login(username: string, password: string): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      this.http.post(`${environment.apiUrl}/api/user/signin`, {
        username,
        password
      }, httpOptions).toPromise().then(response => {
        this.tokenSrv.saveToken(response[`token`]);
        resolve(true);
      }, error => {
        console.log(error);
        reject(false);
      });
    });
  }

  register(user): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/user/signup`, {
      username: user.username,
      email: user.email,
      password: user.password
    }, httpOptions);
  }
}
