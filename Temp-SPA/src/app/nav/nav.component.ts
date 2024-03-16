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


  loggedIn(): any {
    return this.authService.loggedIn();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.authService.clearStorage();
        this.alertify.message('logged out');
        this.router.navigate(['']);
      },
      error: () => {
        this.authService.clearStorage();
        this.alertify.error('Unable to logout');
      }
    })
  }

  

}
