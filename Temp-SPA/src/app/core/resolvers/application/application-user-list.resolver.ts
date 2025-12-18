import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserListApplication } from 'src/app/core/models/application';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { ApplicationService } from 'src/app/core/services/application.service';

@Injectable()
export class ApplicationUserListResolver implements Resolve<UserListApplication[]> {

    constructor(
        private applicationService: ApplicationService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<UserListApplication[]>  {
        return this.applicationService.getUserApplications(route.params['id']).pipe(
            catchError(() => {
                this.alertify.error('Unable to list user applications');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}