import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser!: User;

constructor(private http: HttpClient) { }

login(model: any): any {
  return this.http.post(this.baseUrl + 'accounts/login', model)
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
  return this.http.post(this.baseUrl + 'accounts/register', user);
}


loggedIn(): any {
  const token = localStorage.getItem('token')?.toString();
  return !this.jwtHelper.isTokenExpired(token);
}

logout(): any {
  return this.http.post(this.baseUrl + 'accounts/logout', {});
}

clearStorage(): void {
  localStorage.removeItem('token');
  this.decodedToken = null;
  localStorage.removeItem('user');
  this.currentUser = null;
}

}
