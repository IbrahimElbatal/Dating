import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { User } from './../../_models/user.model';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() user: User;
  constructor(private userService :UserService,
              private toastr:ToastrService,
              private authService:AuthService) { }

  ngOnInit(): void {
  }

  likeUser(userId){
    console.log(userId);
    this.userService.sendLike(this.authService.decodedToken.nameid,userId)
      .subscribe(
        (res:any) => this.toastr.success(res.message),
        err => this.toastr.error(err)
      );
  }
}
