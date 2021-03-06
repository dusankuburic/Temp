import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent implements OnInit {

  user: any;
  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.user = JSON.parse(localStorage.getItem('user'));
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  isAdmin(): boolean {
    if (this.authService.decodedToken.role === 'Admin'){
      return true;
    }
    return false;
  }

  isUser(): boolean {
    if (this.authService.decodedToken.role === 'User'){
      return true;
    }
    return false;
  }

  isModerator(): boolean {
    if (this.authService.decodedToken.role === 'Moderator'){
      return true;
    }
    return false;
  }
}
