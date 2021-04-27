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
export class ListResolver implements Resolve<User[]>{

    pageNumber = 1;
    pageSize = 6;
    likeParams = 'Likers';
    constructor(private userService:UserService,
                private toastr:ToastrService,
                private router:Router) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
        :  Observable<User[]> {
            return this.userService.getUsers(this.pageNumber,this.pageSize,null,null,null,null,this.likeParams)
                .pipe(
                    catchError(err =>{
                        this.toastr.error('Error during geting users data');
                        this.router.navigateByUrl('/');
                        return of(null);
                    }));
    }

}