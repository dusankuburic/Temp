import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})

export class UserGuard  {
  role = 'User';

  constructor(
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService){}

  canActivate(): boolean {
    if (this.authService.loggedIn() && this.authService.decodedToken?.role === this.role){
      return true;
    }

    this.alertify.error('You shall not pass!!!!');
    this.router.navigate(['/home']);
    return false;
  }

}
