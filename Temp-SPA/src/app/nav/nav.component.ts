import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../core/services/alertify.service';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent {
  model: any = {};


  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) { }


  login(): void {
    this.authService.loginAdmin(this.model).subscribe(next => {
      this.alertify.success('Logged in successfully');
    }, error => {
      this.alertify.error('go away');
    }, () => {
      this.router.navigate(['/members']);
    });
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }


}
