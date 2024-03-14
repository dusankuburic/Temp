import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ModeratorListApplication } from 'src/app/core/models/application';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { ApplicationService } from 'src/app/core/services/application.service';

@Injectable()
export class ApplicationModeratorListResolver  {

    constructor(
        private applicationService: ApplicationService,
        private alertify: AlertifyService){}

    resolve(route: ActivatedRouteSnapshot): Observable<ModeratorListApplication[]> {
        const user = JSON.parse(localStorage.getItem('user'));
        console.log(user);
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