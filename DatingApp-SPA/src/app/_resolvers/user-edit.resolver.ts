import { AuthService } from './../_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { UserService } from './../_services/user.service';
import { User } from './../_models/user.model';
import { Injectable } from "@angular/core";
import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
    providedIn:'root'
})
export class UserEditResolver implements Resolve<User>{
    
    constructor(private userService:UserService,
                private authService:AuthService,
                private toastr:ToastrService,
                private router:Router) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
        : Observable<User> {;
            return this.userService.getUser(this.authService.decodedToken.nameid)
                .pipe(
                    catchError(err =>{
                        this.toastr.error('Error during geting user data');
                        this.router.navigateByUrl('/members');
                        return of(null);
                    })
                    );
    }

}