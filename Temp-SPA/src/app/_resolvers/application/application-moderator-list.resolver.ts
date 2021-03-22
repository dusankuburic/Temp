import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ModeratorListApplication } from 'src/app/_models/application';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ApplicationService } from 'src/app/_services/application.service';

@Injectable()
export class ApplicationModeratorListResolver implements Resolve<ModeratorListApplication[]> {

    constructor(
        private applicationService: ApplicationService,
        private router: Router,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<ModeratorListApplication[]> {
        return this.applicationService.getTeamApplications(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error('Problem retriving data');
                this.router.navigate(['']);
                return of(null);
            })
        );
    }
}