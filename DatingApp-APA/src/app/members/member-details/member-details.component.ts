import { AuthService } from './../../_services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from './../../_services/user.service';
import { User } from './../../_models/user.model';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery-9';
import { Message } from 'src/app/_models/message.model';
import { TabsetComponent } from 'ngx-bootstrap/tabs/public_api';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css']
})
export class MemberDetailsComponent implements OnInit {

  messages:Message[];
  user:User;
  userId =this.authService.decodedToken.nameid;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  
  @ViewChild('staticTabSet',{static: true }) tabSet :TabsetComponent;

  constructor(private route:ActivatedRoute,
              private toastr:ToastrService,
              private userService:UserService,
              private authService:AuthService) { }

  ngOnInit(): void {
    this.loadUser();
    this.getMessages();
    var index  = +this.route.snapshot.queryParamMap.get('tab');
    if(index != undefined && index !=null){
      this.selectTab(index);
    }
    this.galleryOptions = [
      {
          width: '400px',
          height: '400px',
          thumbnailsColumns: 4,
          imageAnimation: NgxGalleryAnimation.Slide
      }
  ];
  this.galleryImages = this.getGallaryImages();

  }

  loadUser(){
    this.route.data
      .subscribe(
        data => this.user = data.user,
        error =>this.toastr.error(error)
      );
  }
  getGallaryImages(){
    return this.user.photos.map(photo => {
      return {
        small: photo.url,
        medium: photo.url,
        big: photo.url
      }
    });
   }

   getMessages(){
     this.userService.getUserMessageThread(this.userId,+this.route.snapshot.paramMap.get('id'))
     .subscribe((res:any) => {

        this.messages = res;
        this.messages.forEach(message => {
          if(message.isRead == false && message.recipientId == this.userId){
            this.userService.markAsRead(message.id,this.userId);
            message.isRead = true;
          }
        });
      })
   }
   newMessage(text){
     var data = {
       "content" :text.value,
       "senderId" :this.userId,
       "recipientId" :this.route.snapshot.paramMap.get('id')
     }

     this.userService.newMessage(this.userId,data)
      .subscribe((res:any) => {
        this.messages.unshift(res);
      })
   }

   selectTab(tab){
     this.tabSet.tabs[tab].active = true;  
   }
}
