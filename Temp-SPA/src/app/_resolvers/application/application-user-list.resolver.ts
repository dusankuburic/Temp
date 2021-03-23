import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserListApplication } from 'src/app/_models/application';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ApplicationService } from 'src/app/_services/application.service';

@Injectable({ providedIn: 'root' })
export class ApplicationUserListResolver implements Resolve<UserListApplication[]> {

    constructor(
        private applicationService: ApplicationService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<UserListApplication[]>  {
        return this.applicationService.getUserApplications(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}