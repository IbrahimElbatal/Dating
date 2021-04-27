import { Pagination } from './../../_models/paging-result.model';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UserService } from './../../_services/user.service';
import { User } from './../../_models/user.model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  users:User[];
  pagination:Pagination;
  fromAge;
  toAge;
  gender;
  orderBy;
  genders =[{value :"male" ,display :'Male'} , {value :"female" ,display :'Female'}]
  
  constructor(private route:ActivatedRoute,
              private toastr:ToastrService,
              private userService:UserService) { }

  ngOnInit(): void {
    this.route.data.subscribe(
      data=> {
        this.users = data['users'].result;
        this.pagination = data['users'].pagination;
      },error => this.toastr.error(error,'Error'));
    
      const user:User = JSON.parse(localStorage.getItem('user'));
      this.gender = user.gender === 'male' ? 'female' : 'male';
      this.toAge =99;
      this.fromAge = 18;
      this.orderBy = 'created';
  }

  loadUsers(){
   this.userService
   .getUsers(this.pagination.currentPage,this.pagination.itemsPerPage,this.fromAge,this.toAge,this.gender,this.orderBy)
    .subscribe(res => {
      this.users = res.result;
      this.pagination = res.pagination;
    });
  }
  pageChanged(event){
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  applyFilter(){
    this.loadUsers();
  }
  resetFilter(){
    
    const user:User = JSON.parse(localStorage.getItem('user'));
    this.gender = user.gender === 'male' ? 'female' : 'male';
    this.toAge =99;
    this.fromAge = 18;
    this.orderBy ='created';
    this.loadUsers();

  }
}
