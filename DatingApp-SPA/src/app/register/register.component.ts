import { Router } from '@angular/router';
import { User } from './../_models/user.model';
import { environment } from './../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancel  = new EventEmitter<boolean>();
  registerForm:FormGroup;

  constructor(private authService:AuthService,
              private toastr :ToastrService,
              private fb:FormBuilder,
              private router:Router) { }

  ngOnInit(): void {
    this.initForm();
  }

  register(){
    const user: User = Object.assign({},this.registerForm.value);
    this.authService.register(user)
      .subscribe(
        next => {},
        error => this.toastr.error(error,'Error'),
        () => {
          this.authService.login(user).subscribe(_ =>{
            this.toastr.success('Logged In Successfully');
            this.router.navigateByUrl('/members');
            });
        });
  }
  initForm(){
    this.registerForm = this.fb.group({
      'userName' : ['',Validators.required],
      'gender' : ['male',Validators.required],
      'knownAs' : ['',Validators.required],
      'dateOfBirth' : [null,Validators.required],
      'city' : ['',Validators.required],
      'country' : ['',Validators.required],
      'password' : ['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      'confirmPassword' : ['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
    },{validators : this.passwordMustMatch});
  }

  passwordMustMatch(fg : FormGroup){
    return fg.get('password').value === fg.get('confirmPassword').value ? null : {'mustMatch' : true};
  }
  cancelRegister(){
    this.cancel.emit(false);
  }
}
