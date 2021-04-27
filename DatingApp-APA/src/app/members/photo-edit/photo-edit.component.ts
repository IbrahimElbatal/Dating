import { UserService } from './../../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from './../../../environments/environment';
import { AuthService } from './../../_services/auth.service';
import { Photo } from './../../_models/photo.model';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { User } from 'src/app/_models/user.model';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css']
})
export class PhotoEditComponent implements OnInit {

  URL = environment.baseUrl + `/users/${this.authService.decodedToken.nameid}/photos`
  @Input() photos: Photo[];
  uploader:FileUploader;
  hasBaseDropZoneOver = false;
  @Output() currentPhoto = new EventEmitter<string>();

  constructor(private authService:AuthService,
              private toastr : ToastrService,
              private userService:UserService) { }

  ngOnInit(): void {
    this.initializeUploader();
  }
  
  fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url : this.URL,
      authToken : 'Bearer ' + localStorage.getItem('token'),
      isHTML5 : true,
      maxFileSize : 10 *1024 *1024,
      removeAfterUpload : true,
      autoUpload : false,
      allowedFileType :['image']
    });

    this.uploader.onAfterAddingFile = (file) => file.withCredentials =false;

    this.uploader.onSuccessItem = (item,response,status,header) => {
      if(response){
        let res: Photo = JSON.parse(response);
        const photo = {...res};
        this.photos.push(photo);
        if(res.isMain){
          this.authService.onChangePhoto(res.url);
          let user:User = JSON.parse(localStorage.getItem('user'));
          user.photoUrl = photo.url;
          localStorage.setItem('user',JSON.stringify(user));
        }
      }
    }
  }

  removePhoto(photo:Photo){
    this.userService.deletePhoto(photo.id,+this.authService.decodedToken.nameid)
      .subscribe(_ => {
         let index = this.photos.findIndex(p => p.id === photo.id);
        this.photos.splice(index,1);
        this.toastr.success('Photo deleted');
      }, err => {
        console.log(err);
        this.toastr.error(err);
      });
  }

  setMainPhoto(photo:Photo){
    this.userService.setMainPhoto(photo.id,+this.authService.decodedToken.nameid)
      .subscribe((res:Photo) => {
        let currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
        currentMainPhoto.isMain = false;

        photo.isMain = true;
        this.currentPhoto.emit(res.url);
        this.authService.onChangePhoto(photo.url);
        let user:User = JSON.parse(localStorage.getItem('user'));
        user.photoUrl = photo.url;
        localStorage.setItem('user',JSON.stringify(user));
      }, err => {
        console.log(err);
        this.toastr.error(err);
      });
  }
}
