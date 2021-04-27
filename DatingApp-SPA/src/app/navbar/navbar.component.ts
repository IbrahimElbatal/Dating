import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  model:any ={};
  photoUrl :string;
  constructor(public authService:AuthService,
              private toastr :ToastrService,
              private router:Router,
              private jwtService:JwtHelperService) { }

  ngOnInit(): void {
    this.authService.decodedToken = 
      this.jwtService.decodeToken(localStorage.getItem('token'));

    this.authService.currentPhotoUrl.subscribe(photoUrl => {
      this.photoUrl = photoUrl;
    })
  }

  login(){
    this.authService.login(this.model)
      .subscribe(
        next => {
          this.toastr.success('Logged In Successfully');
          this.model = {};
          this.router.navigateByUrl('/members');
        },
        error =>this.toastr.error(error,'Error')
        );
  }
  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.currentPhoto.next('../../assets/user.png');
    this.toastr.info('Logged Out');
    this.router.navigateByUrl('/');
  }
  isLoggedIn(){
    return this.authService.loggedIn();
  }
}
