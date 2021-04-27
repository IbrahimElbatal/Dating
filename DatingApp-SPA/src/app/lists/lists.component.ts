import { User } from 'src/app/_models/user.model';
import { PaginatedResult, Pagination } from './../_models/paging-result.model';
import { ActivatedRoute } from '@angular/router';
import { UserService } from './../_services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  pagination:Pagination;
  users:User[];
  likeParams = 'Likers';
  
  constructor(private userService:UserService,
              private route:ActivatedRoute) { }

                      
  ngOnInit(): void {
    this.route.data.subscribe(data =>{
      this.pagination = data['list'].pagination;
      this.users = data['list'].result;
    });
  }

  loadUsers(){
    this.userService
   .getUsers(this.pagination.currentPage,this.pagination.itemsPerPage,null,null,null,null,this.likeParams)
    .subscribe(res => {
      this.users = res.result;
      this.pagination = res.pagination;
    });
  }
  
  pageChanged(event){
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
}
