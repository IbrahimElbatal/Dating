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
export class UserDetailsResolver implements Resolve<User>{
    
    constructor(private userService:UserService,
                private toastr:ToastrService,
                private router:Router) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
        : User | Observable<User> | Promise<User> {
            return this.userService.getUser(+route.paramMap.get('id'))
                .pipe(
                    catchError(err =>{
                        this.toastr.error('Error during geting user data');
                        this.router.navigateByUrl('/members');
                        return of(null);
                    })
                    );
    }

}