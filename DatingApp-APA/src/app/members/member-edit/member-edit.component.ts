import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_models/user.model';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  user:User;
  @ViewChild('form',{static:true}) editForm:NgForm;
  @HostListener('window:onbeforeunload',['$event']) 
  preventCloseBrowser($event:any) {
      if(this.editForm.dirty){
        $event.preventDefault();
        $event.returnValue = false;
      }
    }
  constructor(private route:ActivatedRoute,
              private toastr:ToastrService,
              private userService:UserService,
              private authService:AuthService) { }

  ngOnInit(): void {
    this.loadUser();
    
  }

  loadUser(){
    this.route.data
      .subscribe(
        data=> this.user = data.user,
        error => this.toastr.error(error,'Error')
      );
  }

  editProfile(){
    this.userService.updateUser(this.authService.decodedToken.nameid,this.user)
      .subscribe(
        _ => {
          this.editForm.reset(this.user);
          this.toastr.success('updated successfully');
        },
        err => this.toastr.error(err)
      );
  }
  onCurrentPhotoChanged($event){
    this.user.photoUrl = $event;
  }
}
