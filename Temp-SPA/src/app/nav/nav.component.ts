import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs';
import { AlertifyService } from '../core/services/alertify.service';
import { AuthService } from '../core/services/auth.service';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { DestroyableComponent } from '../core/base/destroyable.component';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.scss'],
    standalone: false
})
export class NavComponent extends DestroyableComponent {
  signOutIcon = faSignOutAlt
  model: any = {};


  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) {
    super();
  }


  loggedIn(): any {
    return this.authService.loggedIn();
  }

  logout(): void {
    this.authService.logout()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.authService.clearStorage();
          this.alertify.message('logged out');
          this.router.navigate(['']);
        },
        error: () => {
          this.authService.clearStorage();
          this.alertify.error('Unable to logout');
        }
      });
  }

  

}
