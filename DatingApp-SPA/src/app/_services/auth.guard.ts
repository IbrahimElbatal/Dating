import { AuthService } from './auth.service';
import { Injectable } from "@angular/core";
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({
    providedIn:'root'
})
export class AuthGuard implements CanActivate{
    
    constructor(private authService:AuthService) {        
    }
    canActivate(route: ActivatedRouteSnapshot, state:RouterStateSnapshot)
        : boolean  {
            return this.authService.loggedIn();
    }

}