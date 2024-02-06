import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Application } from 'src/app/models/application';
import { AlertifyService } from 'src/app/services/alertify.service';
import { ApplicationService } from 'src/app/services/application.service';

@Injectable()
export class ApplicationModeratorResolver  {

    constructor(
        private applicationService: ApplicationService,
        private router: Router,
        private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Application>  {
        return this.applicationService.getApplication(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}