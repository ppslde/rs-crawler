import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { TokenService } from './shared/token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private tokenSrv: TokenService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const token = this.tokenSrv.getToken();
    if (token) {
        // // check if route is restricted by role
        // if (route.data.roles && route.data.roles.indexOf(user.role) === -1) {
        //     // role not authorised so redirect to home page
        //     this.router.navigate(['/']);
        //     return false;
        // }

        // authorised so return true
        return true;
    }

    this.router.navigate(['/home']);
    return false;
  }

}
