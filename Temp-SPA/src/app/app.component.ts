import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));

    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }

    if (user) {
      this.authService.currentUser = user;
    }
  }
}
