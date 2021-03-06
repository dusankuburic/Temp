import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { Admin } from '../_models/admin';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser!: User;

constructor(private http: HttpClient) { }

loginUser(model: any): any {
  return this.http.post(this.baseUrl + 'users/login', model)
  .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
      }
    })
  );
}

loginModerator(model: any): any {
  return this.http.post(this.baseUrl + 'moderators/login', model)
  .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
      }
    })
  );
}

loginAdmin(model: any): any {
  return this.http.post(this.baseUrl + 'admins/login', model)
  .pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        localStorage.setItem('user', JSON.stringify(user.user));
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        this.currentUser = user.user;
      }
    })
  );
}





registerUser(user: User): any {
  return this.http.post(this.baseUrl + 'users/register', user);
}

registerAdmin(admin: Admin): any {
  return this.http.post(this.baseUrl + 'admins/register', admin);
}

loggedIn(): any {
  const token = localStorage.getItem('token')?.toString();
  return !this.jwtHelper.isTokenExpired(token);
}

}
