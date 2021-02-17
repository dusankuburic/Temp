import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  loggedIn(): any {
    return this.authService.loggedIn();
  }

  isAdmin() {
    if(this.authService.decodedToken.role === 'Admin'){
      return true;
    }
    return false;
  }

  isUser() {
    if(this.authService.decodedToken.role === 'User'){
      return true;
    }
    return false;
  }
}
