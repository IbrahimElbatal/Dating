import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  constructor(private authService:AuthService,
              private jwtService:JwtHelperService) {
    
  }
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if(token){
      const decodedToken =this.jwtService.decodeToken(token);
      this.authService.userName =decodedToken.unique_name; 
    }
    const user:User = JSON.parse(localStorage.getItem('user'));
    if(user){
      this.authService.onChangePhoto(user.photoUrl);
    }
  }

}
