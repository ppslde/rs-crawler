import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../shared/auth.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private authSrv: AuthService, private fb: FormBuilder,private router: Router) { 
    console.log('login component load');
  }

  hide = true;

  authForm = this.fb.group({
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required)
  });

  ngOnInit(): void {
    console.log('login component loaded');
  }

  public OnSubmit() {
    this.authSrv.login(this.authForm.value.username, this.authForm.value.password).then(result =>{
      if (result) {
        this.router.navigateByUrl("/dashboard");
      }
    }, error => {
      console.log("Login error", error)
    });
  }
}
