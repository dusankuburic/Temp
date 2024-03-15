import { Component, OnInit } from '@angular/core';
import { FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Admin } from '../../core/models/admin';
import { User } from '../../core/models/user';
import { AlertifyService } from '../../core/services/alertify.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  admin: Admin;
  user: User;

  username = new FormControl('', [Validators.required]);
  password = new FormControl('', [Validators.required]);

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: this.username,
      password: this.password
    });
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  login(): void {
    if (this.loginForm.valid) {
      this.user = { ...this.loginForm.value };
     
      this.authService.login(this.user).subscribe(() => {
        this.loginForm.reset();
        this.router.navigate(['/home']);
        this.alertify.success('Login successful');
      }, () => {
        this.alertify.error('Can not login');
      });

    }
  }

  cancel(): void {
    this.loginForm.patchValue({
      username: '',
      password: ''
    })
  }

}
