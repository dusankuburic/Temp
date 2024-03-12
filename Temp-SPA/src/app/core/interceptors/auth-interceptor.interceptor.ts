import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable } from "rxjs";

@Injectable()
export class AddAuthHeaderInterceptor implements HttpInterceptor {
    jwtHelper = new JwtHelperService();
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        let token = localStorage.getItem('token')?.toString();
        if (this.jwtHelper.isTokenExpired(token))
            token = '';
        else 
            token = `Bearer ${token}`

        const authorized: HttpRequest<any> = req.clone({
            setHeaders: {'Authorization': token}
        });

        return next.handle(authorized);
    }
}