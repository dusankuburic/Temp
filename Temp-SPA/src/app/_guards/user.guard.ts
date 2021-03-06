import { Injectable } from '@angular/core';
import { CanActivate, Router} from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})

export class UserGuard implements CanActivate {
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
