import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ModeratorListApplication } from 'src/app/_models/application';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ApplicationService } from 'src/app/_services/application.service';

@Injectable()
export class ApplicationModeratorListResolver  {

    constructor(
        private applicationService: ApplicationService,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<ModeratorListApplication[]> {
        const user = JSON.parse(localStorage.getItem('user'));
        return this.applicationService.getTeamApplicationsForModerator(
            route.params['id'], 
            user.id).pipe(
            catchError(error => {
                this.alertify.error(error.error);
                return of(null);
            })
        );
    }
}