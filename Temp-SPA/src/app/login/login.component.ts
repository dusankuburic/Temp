import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Admin } from '../models/admin';
import { Moderator } from '../models/moderator';
import { User } from '../models/user';
import { AlertifyService } from '../services/alertify.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm: UntypedFormGroup;
  roleList = [
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'}
  ];

  admin: Admin;
  user: User;
  moderator: Moderator;
  roleState: any = {};

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: UntypedFormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.createLoginForm();
  }

  createLoginForm(): void {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      role: [ null, Validators.required]
    });
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  login(): any {
    if (this.loginForm.valid) {

      if (this.loginForm.get('role').value === 'User') {
        this.user = Object.assign({}, this.loginForm.value);
        this.authService.loginUser(this.user).subscribe(() => {
          this.loginForm.reset();
          this.router.navigate(['/home']);
          this.alertify.success('Login successful');
        }, error => {
          this.alertify.error('Can not login');
        });
      }

      if (this.loginForm.get('role').value === 'Moderator') {
        this.moderator = Object.assign({}, this.loginForm.value);
        this.authService.loginModerator(this.moderator).subscribe(() => {
          this.loginForm.reset();
          this.router.navigate(['/home']);
          this.alertify.success('Login successful');
        }, error => {
          this.alertify.error('Can not login');
        });
      }

      if (this.loginForm.get('role').value === 'Admin') {
        this.admin = Object.assign({}, this.loginForm.value);
        this.authService.loginAdmin(this.admin).subscribe(() => {
          this.loginForm.reset();
          this.router.navigate(['/home']);
          this.alertify.success('Login successful');
        }, error => {
          this.alertify.error('Can not login');
        });
      }

    }
  }

  cancel(): void {
  }

}
