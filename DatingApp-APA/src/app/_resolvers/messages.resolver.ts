import { AuthService } from './../_services/auth.service';
import { MessageParams } from './../_models/MessageParams.model';
import { catchError } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { UserService } from './../_services/user.service';
import { Injectable } from "@angular/core";
import { Resolve, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { Message } from '../_models/message.model';

@Injectable({
    providedIn:'root'
})
export class MessagesResolver implements Resolve<Message[]>{

    messageParams:MessageParams = new MessageParams();

    constructor(private userService:UserService,
                private authService:AuthService,
                private toastr:ToastrService,
                private router:Router) {
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
        :  Observable<Message[]> {
            return this.userService.getUsersMessages(this.authService.decodedToken.nameid,this.messageParams)
                .pipe(
                    catchError(err =>{
                        this.toastr.error('Error during geting users data');
                        this.router.navigateByUrl('/');
                        return of(null);
                    }));
    }

}