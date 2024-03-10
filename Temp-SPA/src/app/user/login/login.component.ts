import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Admin } from '../../core/models/admin';
import { Moderator } from '../../core/models/moderator';
import { User } from '../../core/models/user';
import { AlertifyService } from '../../core/services/alertify.service';
import { AuthService } from '../../core/services/auth.service';

import { SelectionOption } from 'src/app/shared/components/tmp-select/tmp-select.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  roleList: SelectionOption[] = [
    {value: '', display: 'Select Role', disabled: true},
    {value: 'User', display: 'User'},
    {value: 'Admin', display: 'Admin'},
    {value: 'Moderator', display: 'Moderator'}
  ];
  admin: Admin;
  user: User;
  moderator: Moderator;
  roleState: any = {};

  username = new FormControl('', [Validators.required]);
  password = new FormControl('', [Validators.required]);
  role = new FormControl('', [Validators.required])

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: this.username,
      password: this.password,
      role: this.role
    });
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  login(): void {
    if (this.loginForm.valid) {

      if (this.role.value === 'User') {
        this.user = { ...this.loginForm.value };
     
        this.authService.loginUser(this.user).subscribe(() => {
          this.loginForm.reset();
          this.router.navigate(['/home']);
          this.alertify.success('Login successful');
        }, error => {
          this.alertify.error('Can not login');
        });
      }

      if (this.role.value === 'Moderator') {
        this.moderator = { ...this.loginForm.value };
   
        this.authService.loginModerator(this.moderator).subscribe(() => {
          this.loginForm.reset();
          this.router.navigate(['/home']);
          this.alertify.success('Login successful');
        }, error => {
          this.alertify.error('Can not login');
        });
      }

      if (this.role.value === 'Admin') {
        this.admin = { ...this.loginForm.value };
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
    this.loginForm.patchValue({
      role: '',
      username: '',
      password: ''
    })
  }

}
