import { User } from './../_models/user.model';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators'
import { JwtHelperService } from "@auth0/angular-jwt";
import { BehaviorSubject } from 'rxjs';

@Injectable({
        providedIn : 'root'
    })
export class AuthService{

    url:string = environment.baseUrl +'/Auth/';
    userName:string;
    decodedToken:any;
    currentPhoto  = new BehaviorSubject<string>('../../assets/user.png');
    currentPhotoUrl = this.currentPhoto.asObservable();

    constructor(private http:HttpClient,
                private jwtService:JwtHelperService) {
    }

    register(model:User){
        return this.http.post(this.url+'register',model);
    }
    login(model:any){
        return this.http.post(this.url+'login',model)
            .pipe(
                tap((res:any) =>{
                    if(res && res.token){
                        localStorage.setItem('token',res.token);
                        localStorage.setItem('user',JSON.stringify(res.user));
                        this.decodedToken = this.jwtService.decodeToken(res.token);
                        this.userName = this.decodedToken.unique_name;
                        this.onChangePhoto(res.user.photoUrl);
                    }
                })
            );
    }
    loggedIn(){
        var token = localStorage.getItem('token');
        return !this.jwtService.isTokenExpired(token);
    }
    onChangePhoto(url:string){
        this.currentPhoto.next(url);
    }
}