import { MessageParams } from './../_models/MessageParams.model';
import { Pagination, PaginatedResult } from './../_models/paging-result.model';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from './../_services/auth.service';
import { UserService } from './../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message.model';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages :Message[];
  pagination:Pagination;
  messageParams = new MessageParams();

  constructor(private userService:UserService,
              private authService:AuthService,
              private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.route.data
      .subscribe((data:PaginatedResult<Message>) => {
        this.messages = data['messages'].result;
        this.pagination = data['messages'].pagination;
      })
  }

  loadMessages(){
    this.userService.getUsersMessages(this.authService.decodedToken.nameid,this.messageParams)
      .subscribe(res =>{
        this.messages = res.result;
        console.log(res.result);
        this.pagination = res.pagination;
      });
  }

  pageChanged(event){
    this.messageParams.pageNumber= event.page;
    this.loadMessages();
  }

  deleteMessage(id){
     if(confirm('Are you sure you want to delte this message ?')){
      this.userService.deleteMessage(id,this.authService.decodedToken.nameid)
      .subscribe(res => {
        const index = this.messages.findIndex(m =>m.id == id);
        this.messages.splice(index,1);
      });
     }

  }
}
